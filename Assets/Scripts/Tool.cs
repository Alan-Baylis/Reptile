using UnityEngine;
using System.Collections.Generic;

public class Tool : MonoBehaviour
{
    public Transform cursor;
    public LayerMask mask;

    public static Tool use;
    public static bool editing;

    void Awake()
    {
        use = this;
    }

    int px;
    int py;
    int pz;

    int nx;
    int ny;
    int nz;

    int sx;
    int sy;
    int sz;

    bool startEditing;
    Plane plane;

    Dictionary<int, byte> edits = new Dictionary<int, byte>();

    void Update()
    {
        bool toolHeld = Edit.use.bindUseTool.IsHeld() || Edit.use.bindUseToolAlt.IsHeld();
        bool toolAlt = Edit.use.bindUseToolAlt.IsHeld();

        bool planeLock = Edit.use.planeLock;
        if (Edit.use.bindPlaneLock.IsHeld()) planeLock = !planeLock;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (toolHeld && !editing)
        {
            editing = true;
            PreviewChunk.use.chunk.SetPaletteIndices(GetChunk().GetPaletteIndices());
            startEditing = true;
        }

        if (!planeLock || startEditing || !editing)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, mask))
            {
                cursor.gameObject.SetActive(true);

                Vector3 p = hit.point;
                Vector3 n = hit.normal;

                n.x = Mathf.Round(n.x);
                n.y = Mathf.Round(n.y);
                n.z = Mathf.Round(n.z);

                nx = Mathf.RoundToInt(n.x);
                ny = Mathf.RoundToInt(n.y);
                nz = Mathf.RoundToInt(n.z);

                p -= n * 0.01f;

                p.x = Mathf.Round(p.x);
                p.y = Mathf.Round(p.y);
                p.z = Mathf.Round(p.z);

                px = Mathf.RoundToInt(p.x);
                py = Mathf.RoundToInt(p.y);
                pz = Mathf.RoundToInt(p.z);

                p += n * 0.5f;

                cursor.position = p;
                cursor.forward = n;

                plane = new Plane(n, p);

                if (startEditing)
                {
                    sx = px;
                    sy = py;
                    sz = pz;
                    if ((Edit.use.tool == Edit.Tool.Place && !toolAlt) ||
                        (Edit.use.tool == Edit.Tool.Box   && !toolAlt))
                    {
                        sx += nx;
                        sy += ny;
                        sz += nz;
                    }
                    startEditing = false;
                }
            }
            else
            {
                cursor.gameObject.SetActive(false);
            }
        }else
        {
            float dist;
            if (plane.Raycast(ray, out dist))
            {
                Vector3 p = ray.GetPoint(dist);
                Vector3 n = plane.normal;

                n.x = Mathf.Round(n.x);
                n.y = Mathf.Round(n.y);
                n.z = Mathf.Round(n.z);

                nx = Mathf.RoundToInt(n.x);
                ny = Mathf.RoundToInt(n.y);
                nz = Mathf.RoundToInt(n.z);

                p -= n * 0.01f;

                p.x = Mathf.Round(p.x);
                p.y = Mathf.Round(p.y);
                p.z = Mathf.Round(p.z);

                px = Mathf.RoundToInt(p.x);
                py = Mathf.RoundToInt(p.y);
                pz = Mathf.RoundToInt(p.z);

                p += n * 0.5f;

                cursor.position = p;
                cursor.forward = n;
                
                cursor.gameObject.SetActive(!IsOutOfBounds(px+nx, py+ny, pz+nz));
            }
            else
            {
                cursor.gameObject.SetActive(false);
            }
        }
        if (toolHeld)
        {
            byte currentColor = (byte)Edit.use.tile.GetPalette().GetIndex();
            if (Edit.use.tool == Edit.Tool.Place)
            {
                if (toolAlt)
                    Paint(px, py, pz, 0, PaintMode.Filled);
                else
                    Paint(px + nx, py + ny, pz + nz, currentColor, PaintMode.Empty);
            }
            if (Edit.use.tool == Edit.Tool.Paint)
            {
                Paint(px, py, pz, currentColor, PaintMode.Filled);
            }
            if (Edit.use.tool == Edit.Tool.Fill)
            {
                if (toolAlt)
                    FloodFill(px, py, pz, 0, PaintMode.Filled);
                else
                    FloodFill(px, py, pz, currentColor, PaintMode.Filled);
            }
            if (Edit.use.tool == Edit.Tool.Box)
            {
                edits.Clear();
                PreviewChunk.use.chunk.SetPaletteIndices(GetChunk().GetPaletteIndices());
                if (toolAlt)
                    BoxFill(sx, sy, sz, px, py, pz, 0, PaintMode.Filled);
                else
                    BoxFill(sx, sy, sz, px + nx, py + ny, pz + nz, currentColor, PaintMode.Empty);
            }
        }
        if (editing && !toolHeld)
        {
            if (edits.Count > 0)
            {
                byte[] indices = GetChunk().GetPaletteIndices();
                foreach (var pair in edits)
                {
                    indices[pair.Key] = pair.Value;
                }
                Edit.Do(new EditChunkAct(indices));
                edits.Clear();
            }

            editing = false;

            PreviewChunk.use.Clear();
        }
    }

    VTileChunk GetChunk()
    {
        return Edit.use.tile.GetChunk(Edit.use.tile.GetLayerIndex(), Edit.use.tile.GetAnimationIndex(), Edit.use.tile.GetFrameIndex());
    }

    bool IsColor(int x, int y, int z, int index)
    {
        return GetChunk().GetPaletteIndexAt(x, y, z) == index;
    }

    bool IsEmpty(int x, int y, int z)
    {
        return IsColor(x, y, z, 0);
    }

    enum PaintMode
    {
        Filled,
        Empty,
        Any
    }

    void Paint(int x, int y, int z, byte color, PaintMode mode)
    {
        if (IsOutOfBounds(x, y, z)) return;
        Edit.Brush brush = Edit.use.brush;
        int size = Edit.use.brushSize;
        foreach (int index in GetSymmetryPoints(x, y, z))
        {
            int px, py, pz;
            FromIndex(index, out px, out py, out pz);
            DoPaint(px, py, pz, color, mode, brush, size);
        }
    }

    void FloodFill(int x, int y, int z, byte color, PaintMode mode)
    {
        if (IsOutOfBounds(x, y, z)) return;
        foreach (int index in GetSymmetryPoints(x, y, z))
        {
            int px, py, pz;
            FromIndex(index, out px, out py, out pz);
            DoFloodFill(px, py, pz, color, mode);
        }
    }

    void BoxFill(int sx, int sy, int sz, int dx, int dy, int dz, byte color, PaintMode mode)
    {
        if (IsOutOfBounds(sx, sy, sz) || IsOutOfBounds(dx, dy, dz)) return;
        List<int> spts = GetSymmetryPoints(sx, sy, sz);
        List<int> dpts = GetSymmetryPoints(dx, dy, dz);
        for (int i = 0; i < spts.Count; i ++)
        {
            int psx, psy, psz;
            FromIndex(spts[i], out psx, out psy, out psz);
            int pdx, pdy, pdz;
            FromIndex(dpts[i], out pdx, out pdy, out pdz);
            DoBoxFill(psx, psy, psz, pdx, pdy, pdz, color, mode);
        }
    }

    List<int> GetSymmetryPoints(int x, int y, int z)
    {
        List<int> pts = new List<int>();
        pts.Add(ToIndex(x, y, z));
        if (Edit.use.mirrorX)
            pts.Add(ToIndex(Edit.use.tile.GetWidth() - 1 - x, y, z));
        if (Edit.use.mirrorY)
            pts.Add(ToIndex(x, Edit.use.tile.GetHeight() - 1 - y, z));
        if (Edit.use.mirrorZ)
            pts.Add(ToIndex(x, y, Edit.use.tile.GetDepth() - 1 - z));
        if (Edit.use.mirrorX && Edit.use.mirrorY)
            pts.Add(ToIndex(Edit.use.tile.GetWidth() - 1 - x, Edit.use.tile.GetHeight() - 1 - y, z));
        if (Edit.use.mirrorY && Edit.use.mirrorZ)
            pts.Add(ToIndex(x, Edit.use.tile.GetHeight() - 1 - y, Edit.use.tile.GetDepth() - 1 - z));
        if (Edit.use.mirrorX && Edit.use.mirrorZ)
            pts.Add(ToIndex(Edit.use.tile.GetWidth() - 1 - x, y, Edit.use.tile.GetDepth() - 1 - z));
        if (Edit.use.mirrorX && Edit.use.mirrorY && Edit.use.mirrorZ)
            pts.Add(ToIndex(Edit.use.tile.GetWidth() - 1 - x, Edit.use.tile.GetHeight() - 1 - y, Edit.use.tile.GetDepth() - 1 - z));
        return pts;
    }

    List<int> GetFillNeighbors(int x, int y, int z, byte color)
    {
        List<int> neighbors = new List<int>();
        for (int px = -1; px <= 1; px ++)
        {
            for (int py = -1; py <= 1; py ++)
            {
                for (int pz = -1; pz <= 1; pz ++)
                {
                    if (IsOutOfBounds(x + px, y + py, z + pz) || !IsColor(x + px, y + py, z + pz, color)) continue;
                    int dist = Mathf.Abs(px) + Mathf.Abs(py) + Mathf.Abs(pz);
                    if (dist == 0) continue;
                    if (dist > 1 && !Edit.use.fillDiagonals) continue;
                    neighbors.Add(ToIndex(x + px, y + py, z + pz));
                }
            }
        }
        return neighbors;
    }

    void DoFloodFill(int x, int y, int z, byte color, PaintMode mode)
    {
        if (IsOutOfBounds(x, y, z)) return;

        byte c = GetChunk().GetPaletteIndexAt(x, y, z);

        Queue<int> open = new Queue<int>();
        HashSet<int> closed = new HashSet<int>();

        open.Enqueue(ToIndex(x, y, z));
        closed.Add(ToIndex(x, y, z));

        while (open.Count > 0)
        {
            int index = open.Dequeue();

            int px, py, pz;
            FromIndex(index, out px, out py, out pz);

            PreviewChunk.use.chunk.SetPaletteIndexAt(px, py, pz, color);
            edits[index] = color;

            foreach (int n in GetFillNeighbors(px, py, pz, c))
            {
                if (!closed.Contains(n))
                {
                    open.Enqueue(n);
                    closed.Add(n);
                }
            }
        }
    }

    void DoPaint(int x, int y, int z, byte color, PaintMode mode, Edit.Brush brush, int size)
    {
        for (int px = x - (size - 1); px <= x + (size - 1); px++)
        {
            for (int py = y - (size - 1); py <= y + (size - 1); py++)
            {
                for (int pz = z - (size - 1); pz <= z + (size - 1); pz++)
                {
                    if (IsOutOfBounds(px, py, pz)) continue;
                    if (mode == PaintMode.Filled && IsEmpty(px, py, pz)) continue;
                    if (mode == PaintMode.Empty && !IsEmpty(px, py, pz)) continue;
                    if (brush == Edit.Brush.Diamond && Mathf.Abs(px - x) + Mathf.Abs(py - y) + Mathf.Abs(pz - z) >= size) continue;
                    if (brush == Edit.Brush.Sphere && Vector3.Distance(new Vector3(x, y, z), new Vector3(px, py, pz)) > size) continue;

                    PreviewChunk.use.chunk.SetPaletteIndexAt(px, py, pz, color);
                    edits[ToIndex(px, py, pz)] = color;
                }
            }
        }
    }

    void DoBoxFill(int sx, int sy, int sz, int dx, int dy, int dz, byte color, PaintMode mode)
    {
        for (int px = Mathf.Min(sx, dx); px <= Mathf.Max(sx, dx); px ++)
        {
            for (int py = Mathf.Min(sy, dy); py <= Mathf.Max(sy, dy); py ++)
            {
                for (int pz = Mathf.Min(sz, dz); pz <= Mathf.Max(sz, dz); pz ++)
                {
                    if (IsOutOfBounds(px, py, pz)) continue;
                    if (mode == PaintMode.Filled && IsEmpty(px, py, pz)) continue;
                    if (mode == PaintMode.Empty && !IsEmpty(px, py, pz)) continue;

                    PreviewChunk.use.chunk.SetPaletteIndexAt(px, py, pz, color);
                    edits[ToIndex(px, py, pz)] = color;
                }
            }
        }
    }

    int ToIndex(int x, int y, int z)
    {
        VTileChunk c = GetChunk();
        return z * c.GetWidth() * c.GetHeight() + y * c.GetWidth() + x;
    }

    void FromIndex(int index, out int x, out int y, out int z)
    {
        VTileChunk c = GetChunk();
        z = index / (c.GetWidth() * c.GetHeight());
        index -= (z * c.GetWidth() * c.GetHeight());
        y = index / c.GetWidth();
        x = index % c.GetWidth();
    }

    bool IsOutOfBounds(int x, int y, int z)
    {
        if (x < 0 || x >= Edit.use.tile.GetWidth()) return true;
        if (y < 0 || y >= Edit.use.tile.GetHeight()) return true;
        if (z < 0 || z >= Edit.use.tile.GetDepth()) return true;
        return false;
    }

    bool IsMouseMoving()
    {
        return Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.05f;
    }
}

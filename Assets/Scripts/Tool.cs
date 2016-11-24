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

    bool badPlane;
    Plane plane;

    Dictionary<int, byte> edits = new Dictionary<int, byte>();

    void Update()
    {
        bool toolHeld = Edit.use.bindUseTool.IsHeld() || Edit.use.bindUseToolAlt.IsHeld();
        bool toolAlt = Edit.use.bindUseToolAlt.IsHeld();

        bool planeLock = Edit.use.bindPlaneLock.IsHeld();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (toolHeld && !editing)
        {
            editing = true;
            PreviewChunk.use.chunk.SetPaletteIndices(GetChunk().GetPaletteIndices());
            badPlane = true;
        }

        if (!planeLock || badPlane)
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
                badPlane = false;
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
            if (Edit.use.tool == Edit.Tool.Place && !toolAlt && !IsOutOfBounds(px + nx, py + ny, pz + nz) && IsEmpty(px + nx, py + ny, pz +nz))
            {
                Paint(px + nx, py + ny, pz + nz, (byte)Edit.use.tile.GetPalette().GetIndex());
            }
            if (Edit.use.tool == Edit.Tool.Place && toolAlt && !IsOutOfBounds(px, py, pz) && !IsEmpty(px, py, pz))
            {
                Paint(px, py, pz, 0);
            }
            if (Edit.use.tool == Edit.Tool.Paint && !IsOutOfBounds(px, py, pz) && !IsEmpty(px, py, pz))
            {
                Paint(px, py, pz, (byte)Edit.use.tile.GetPalette().GetIndex());
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

    bool IsEmpty(int x, int y, int z)
    {
        return GetChunk().GetPaletteIndexAt(x, y, z) == 0;
    }

    void Paint(int x, int y, int z, byte color)
    {
        PreviewChunk.use.chunk.SetPaletteIndexAt(x, y, z, color);
        edits[ToIndex(x, y, z)] = color;
    }

    int ToIndex(int x, int y, int z)
    {
        VTileChunk c = GetChunk();
        return z * c.GetWidth() * c.GetHeight() + y * c.GetWidth() + x;
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

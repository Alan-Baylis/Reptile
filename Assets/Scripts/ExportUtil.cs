using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExportUtil
{

    static List<Vector3> verts = new List<Vector3>();
    static List<Vector3> normals = new List<Vector3>();
    static List<Vector2> uvs = new List<Vector2>();
    static List<int> tris = new List<int>();

    static int v;

    static int indexTemp;

    static int GetIndex(VTile tile, int x, int y, int z)
    {
        for (int i = tile.GetLayerCount() - 1; i >= 0; i--)
        {
            indexTemp = tile.GetChunk(i, tile.GetAnimationIndex(), tile.GetFrameIndex()).GetPaletteIndexAt(x, y, z);
            if (indexTemp != 0) return indexTemp;
        }
        return 0;
    }

    static int colorIndexTemp;

    static VColor GetColor(VTile tile, int x, int y, int z)
    {
        colorIndexTemp = GetIndex(tile, x, y, z);
        if (colorIndexTemp >= tile.GetPalette().GetCount()) return new VColor(255, 0, 255, 255, 0, 0, 255, 0);
        else return tile.GetPalette().GetColor(colorIndexTemp);
    }

    static void AddFace(VTile tile, int x, int y, int z, Vector3 normal, bool subMesh)
    {
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        tris.Add(v + 0);
        tris.Add(v + 1);
        tris.Add(v + 2);

        tris.Add(v + 0);
        tris.Add(v + 2);
        tris.Add(v + 3);

        v += 4;

        int i = GetIndex(tile, x, y, z);

        Vector2 uv = new Vector2(i + 0.5f, 0.5f) / 256f;

        uvs.Add(uv);
        uvs.Add(uv);
        uvs.Add(uv);
        uvs.Add(uv);
    }

    public static Mesh TileToMesh(VTile tile)
    {
        Mesh mesh = new Mesh();

        v = 0;

        for (int x = 0; x < tile.GetWidth(); x++)
        {
            for (int y = 0; y < tile.GetHeight(); y++)
            {
                for (int z = 0; z < tile.GetDepth(); z++)
                {
                    VColor c = GetColor(tile, x, y, z);
                    if (GetColor(tile, x, y, z).a == 0) continue;
                    Vector3 p = new Vector3(x, y, z);

                    bool useSubMesh = c.a < 255;

                    if (x == 0 || (GetColor(tile, x - 1, y, z).a < 255 && GetColor(tile, x - 1, y, z) != c))
                    {
                        verts.Add(p + new Vector3(0f, 0f, 1f));
                        verts.Add(p + new Vector3(0f, 1f, 1f));
                        verts.Add(p + new Vector3(0f, 1f, 0f));
                        verts.Add(p + new Vector3(0f, 0f, 0f));

                        AddFace(tile, x, y, z, Vector3.left, useSubMesh);
                    }
                    if (x == tile.GetWidth() - 1 || (GetColor(tile, x + 1, y, z).a < 255 && GetColor(tile, x + 1, y, z) != c))
                    {
                        verts.Add(p + new Vector3(1f, 0f, 0f));
                        verts.Add(p + new Vector3(1f, 1f, 0f));
                        verts.Add(p + new Vector3(1f, 1f, 1f));
                        verts.Add(p + new Vector3(1f, 0f, 1f));

                        AddFace(tile, x, y, z, Vector3.right, useSubMesh);
                    }
                    if (y == 0 || (GetColor(tile, x, y - 1, z).a < 255 && GetColor(tile, x, y - 1, z) != c))
                    {
                        verts.Add(p + new Vector3(1f, 0f, 0f));
                        verts.Add(p + new Vector3(1f, 0f, 1f));
                        verts.Add(p + new Vector3(0f, 0f, 1f));
                        verts.Add(p + new Vector3(0f, 0f, 0f));

                        AddFace(tile, x, y, z, Vector3.down, useSubMesh);
                    }
                    if (y == tile.GetHeight() - 1 || (GetColor(tile, x, y + 1, z).a < 255 && GetColor(tile, x, y + 1, z) != c))
                    {
                        verts.Add(p + new Vector3(0f, 1f, 0f));
                        verts.Add(p + new Vector3(0f, 1f, 1f));
                        verts.Add(p + new Vector3(1f, 1f, 1f));
                        verts.Add(p + new Vector3(1f, 1f, 0f));

                        AddFace(tile, x, y, z, Vector3.up, useSubMesh);
                    }
                    if (z == 0 || (GetColor(tile, x, y, z - 1).a < 255 && GetColor(tile, x, y, z - 1) != c))
                    {
                        verts.Add(p + new Vector3(0f, 0f, 0f));
                        verts.Add(p + new Vector3(0f, 1f, 0f));
                        verts.Add(p + new Vector3(1f, 1f, 0f));
                        verts.Add(p + new Vector3(1f, 0f, 0f));

                        AddFace(tile, x, y, z, Vector3.back, useSubMesh);
                    }
                    if (z == tile.GetDepth() - 1 || (GetColor(tile, x, y, z + 1).a < 255 && GetColor(tile, x, y, z + 1) != c))
                    {
                        verts.Add(p + new Vector3(1f, 0f, 1f));
                        verts.Add(p + new Vector3(1f, 1f, 1f));
                        verts.Add(p + new Vector3(0f, 1f, 1f));
                        verts.Add(p + new Vector3(0f, 0f, 1f));

                        AddFace(tile, x, y, z, Vector3.forward, useSubMesh);
                    }
                }
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();

        mesh.RecalculateBounds();

        verts.Clear();
        normals.Clear();
        uvs.Clear();
        tris.Clear();

        return mesh;
    }

    public static string TileToObj(VTile tile)
    {
        return MeshToObj(TileToMesh(tile));
    }

    public static string MeshToObj(Mesh mesh)
    {
        string output = "g default\n";
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 v = mesh.vertices[i];
            output += "v " + -v.x + " " + v.y + " " + v.z + "\n";
            Vector3 n = mesh.normals[i];
            output += "vn " + n.x + " " + n.y + " " + n.z + "\n";
            Vector2 uv = mesh.uv[i];
            output += "vt " + uv.x + " " + uv.y + "\n";
        }
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int f0 = mesh.triangles[i + 0] + 1;
            int f1 = mesh.triangles[i + 1] + 1;
            int f2 = mesh.triangles[i + 2] + 1;
            output += "f " + f0 + "/" + f0 + "/" + f0 + " " + f2 + "/" + f2 + "/" + f2 + " " + f1 + "/" + f1 + "/" + f1 + "\n";
        }
        return output;
    }

    public static byte[] PaletteToPng(VPalette pal)
    {
        Texture2D tex = new Texture2D(256, 1, TextureFormat.ARGB32, false);
        for (int i = 0; i < pal.GetCount(); i++)
        {
            VColor c = pal.GetColor(i);
            tex.SetPixel(i, 0, new Color32(c.r, c.g, c.b, c.a));
        }
        tex.Apply();
        return tex.EncodeToPNG();
    }

    public static readonly uint[] vox_default_palette = {
        0x00000000, 0xffffffff, 0xffccffff, 0xff99ffff, 0xff66ffff, 0xff33ffff, 0xff00ffff, 0xffffccff, 0xffccccff, 0xff99ccff, 0xff66ccff, 0xff33ccff, 0xff00ccff, 0xffff99ff, 0xffcc99ff, 0xff9999ff,
        0xff6699ff, 0xff3399ff, 0xff0099ff, 0xffff66ff, 0xffcc66ff, 0xff9966ff, 0xff6666ff, 0xff3366ff, 0xff0066ff, 0xffff33ff, 0xffcc33ff, 0xff9933ff, 0xff6633ff, 0xff3333ff, 0xff0033ff, 0xffff00ff,
        0xffcc00ff, 0xff9900ff, 0xff6600ff, 0xff3300ff, 0xff0000ff, 0xffffffcc, 0xffccffcc, 0xff99ffcc, 0xff66ffcc, 0xff33ffcc, 0xff00ffcc, 0xffffcccc, 0xffcccccc, 0xff99cccc, 0xff66cccc, 0xff33cccc,
        0xff00cccc, 0xffff99cc, 0xffcc99cc, 0xff9999cc, 0xff6699cc, 0xff3399cc, 0xff0099cc, 0xffff66cc, 0xffcc66cc, 0xff9966cc, 0xff6666cc, 0xff3366cc, 0xff0066cc, 0xffff33cc, 0xffcc33cc, 0xff9933cc,
        0xff6633cc, 0xff3333cc, 0xff0033cc, 0xffff00cc, 0xffcc00cc, 0xff9900cc, 0xff6600cc, 0xff3300cc, 0xff0000cc, 0xffffff99, 0xffccff99, 0xff99ff99, 0xff66ff99, 0xff33ff99, 0xff00ff99, 0xffffcc99,
        0xffcccc99, 0xff99cc99, 0xff66cc99, 0xff33cc99, 0xff00cc99, 0xffff9999, 0xffcc9999, 0xff999999, 0xff669999, 0xff339999, 0xff009999, 0xffff6699, 0xffcc6699, 0xff996699, 0xff666699, 0xff336699,
        0xff006699, 0xffff3399, 0xffcc3399, 0xff993399, 0xff663399, 0xff333399, 0xff003399, 0xffff0099, 0xffcc0099, 0xff990099, 0xff660099, 0xff330099, 0xff000099, 0xffffff66, 0xffccff66, 0xff99ff66,
        0xff66ff66, 0xff33ff66, 0xff00ff66, 0xffffcc66, 0xffcccc66, 0xff99cc66, 0xff66cc66, 0xff33cc66, 0xff00cc66, 0xffff9966, 0xffcc9966, 0xff999966, 0xff669966, 0xff339966, 0xff009966, 0xffff6666,
        0xffcc6666, 0xff996666, 0xff666666, 0xff336666, 0xff006666, 0xffff3366, 0xffcc3366, 0xff993366, 0xff663366, 0xff333366, 0xff003366, 0xffff0066, 0xffcc0066, 0xff990066, 0xff660066, 0xff330066,
        0xff000066, 0xffffff33, 0xffccff33, 0xff99ff33, 0xff66ff33, 0xff33ff33, 0xff00ff33, 0xffffcc33, 0xffcccc33, 0xff99cc33, 0xff66cc33, 0xff33cc33, 0xff00cc33, 0xffff9933, 0xffcc9933, 0xff999933,
        0xff669933, 0xff339933, 0xff009933, 0xffff6633, 0xffcc6633, 0xff996633, 0xff666633, 0xff336633, 0xff006633, 0xffff3333, 0xffcc3333, 0xff993333, 0xff663333, 0xff333333, 0xff003333, 0xffff0033,
        0xffcc0033, 0xff990033, 0xff660033, 0xff330033, 0xff000033, 0xffffff00, 0xffccff00, 0xff99ff00, 0xff66ff00, 0xff33ff00, 0xff00ff00, 0xffffcc00, 0xffcccc00, 0xff99cc00, 0xff66cc00, 0xff33cc00,
        0xff00cc00, 0xffff9900, 0xffcc9900, 0xff999900, 0xff669900, 0xff339900, 0xff009900, 0xffff6600, 0xffcc6600, 0xff996600, 0xff666600, 0xff336600, 0xff006600, 0xffff3300, 0xffcc3300, 0xff993300,
        0xff663300, 0xff333300, 0xff003300, 0xffff0000, 0xffcc0000, 0xff990000, 0xff660000, 0xff330000, 0xff0000ee, 0xff0000dd, 0xff0000bb, 0xff0000aa, 0xff000088, 0xff000077, 0xff000055, 0xff000044,
        0xff000022, 0xff000011, 0xff00ee00, 0xff00dd00, 0xff00bb00, 0xff00aa00, 0xff008800, 0xff007700, 0xff005500, 0xff004400, 0xff002200, 0xff001100, 0xffee0000, 0xffdd0000, 0xffbb0000, 0xffaa0000,
        0xff880000, 0xff770000, 0xff550000, 0xff440000, 0xff220000, 0xff110000, 0xffeeeeee, 0xffdddddd, 0xffbbbbbb, 0xffaaaaaa, 0xff888888, 0xff777777, 0xff555555, 0xff444444, 0xff222222, 0xff111111
    };

    public static VTile VoxToTile(byte[] vox)
    {
        VTile tile = new VTile();

        VPalette pal = tile.GetPalette();
        for (int j = 0; j < 256; j ++)
        {
            byte[] bits = System.BitConverter.GetBytes(vox_default_palette[j]);
            VColor c = new VColor(bits[0], bits[1], bits[2], bits[3]);
            if (j >= pal.GetCount()) pal.AddColor(c);
            else pal.SetColor(j, c);
        }

        int i = 0;
        string chunk = "" + (char)vox[i + 0] + (char)vox[i + 1] + (char)vox[i + 2] + (char)vox[i + 3];
        i += 4;
        if (chunk != "VOX ") throw new System.Exception("Invalid VOX file");
        int version = System.BitConverter.ToInt32(vox, i);
        i += 4;
        if (version != 150) throw new System.Exception("Unsupported VOX version (expected 150, got " + version + ")");

        while (i < vox.Length)
        {
            chunk = "" + (char)vox[i + 0] + (char)vox[i + 1] + (char)vox[i + 2] + (char)vox[i + 3];
            i += 4;
            int contentLength = System.BitConverter.ToInt32(vox, i);
            i += 4;
            //int childrenLength = System.BitConverter.ToInt32(vox, i);
            i += 4;
            if (chunk == "MAIN")
            {

            }else if (chunk == "PACK")
            {
                int numModels = System.BitConverter.ToInt32(vox, i);
                i += 4;
                if (numModels > 1) throw new System.Exception("Unsupported VOX feature (cannot read multi-model pack files)");
            }else if (chunk == "SIZE")
            {
                int x = System.BitConverter.ToInt32(vox, i);
                i += 4;
                int y = System.BitConverter.ToInt32(vox, i);
                i += 4;
                int z = System.BitConverter.ToInt32(vox, i);
                i += 4;
                tile.Resize(x, z, y);
            }else if (chunk == "XYZI")
            {
                int count = System.BitConverter.ToInt32(vox, i);
                i += 4;
                for (int j = 0; j < count; j ++)
                {
                    byte x = vox[i + 0];
                    byte y = vox[i + 1];
                    byte z = vox[i + 2];
                    byte c = vox[i + 3];
                    tile.GetChunk(0, 0, 0).SetPaletteIndexAt(x, z, tile.GetDepth() - y - 1, (byte)(255 - c + 1));
                    i += 4;
                }
            }else if (chunk == "RGBA")
            {
                for (int j = 0; j < 256; j ++)
                {
                    VColor c = new VColor(vox[i + 0], vox[i + 1], vox[i + 2], vox[i + 3]);
                    i += 4;
                    // Palette color 0 is always transparent
                    if (j == 255) c.a = 0;
                    if (j >= pal.GetCount()) pal.AddColor(c);
                    else pal.SetColor(255 - j, c);
                }
            }
            else
            {
                i += contentLength;
            }
        }

        return tile;
    }

    static void Add<T>(List<T> arr, params T[] args)
    {
        arr.AddRange(args);
    }

    public static byte[] TileToVox(VTile tile)
    {
        List<byte> data = new List<byte>();
        Add(data, (byte)'V', (byte)'O', (byte)'X', (byte)' ');
        Add(data, System.BitConverter.GetBytes(150));
        Add(data, (byte)'M', (byte)'A', (byte)'I', (byte)'N');
        Add(data, System.BitConverter.GetBytes(0));
        int mainChildSizeIndex = data.Count;
        Add(data, (byte)'S', (byte)'I', (byte)'Z', (byte)'E');
        Add(data, System.BitConverter.GetBytes(12));
        Add(data, System.BitConverter.GetBytes(0));
        Add(data, System.BitConverter.GetBytes(tile.GetWidth()));
        Add(data, System.BitConverter.GetBytes(tile.GetHeight()));
        Add(data, System.BitConverter.GetBytes(tile.GetDepth()));
        Add(data, (byte)'X', (byte)'Y', (byte)'Z', (byte)'I');
        int xyziContentSizeIndex = data.Count;
        Add(data, System.BitConverter.GetBytes(0));
        int numVoxelsIndex = data.Count;
        int numVoxels = 0;
        for (int x = 0; x < tile.GetWidth(); x ++)
        {
            for (int y = 0; y < tile.GetHeight(); y ++)
            {
                for (int z = 0; z < tile.GetDepth(); z ++)
                {
                    int index = GetIndex(tile, x, y, z);
                    if (index != 0)
                    {
                        Add(data, (byte)x, (byte)(tile.GetHeight() - z - 1), (byte)y, (byte)(255 - index + 1));
                        numVoxels++;
                    }
                }
            }
        }
        data.InsertRange(numVoxelsIndex, System.BitConverter.GetBytes(numVoxels));
        data.InsertRange(xyziContentSizeIndex, System.BitConverter.GetBytes(data.Count - xyziContentSizeIndex - 4));
        Add(data, (byte)'R', (byte)'G', (byte)'B', (byte)'A');
        Add(data, System.BitConverter.GetBytes(256 * 4));
        Add(data, System.BitConverter.GetBytes(0));
        for (int i = 255; i >= 0; i --)
        {
            VColor c;
            if (i >= tile.GetPalette().GetCount())
                c = new VColor(255, 0, 255, 255, 0, 0, 255, 0);
            else
                c = tile.GetPalette().GetColor(i);
            Add(data, c.r, c.g, c.b, c.a);
        }
        data.InsertRange(mainChildSizeIndex, System.BitConverter.GetBytes(data.Count - mainChildSizeIndex));
        return data.ToArray();
    }
}

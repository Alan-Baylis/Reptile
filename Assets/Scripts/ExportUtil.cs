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
}

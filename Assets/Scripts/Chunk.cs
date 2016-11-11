using UnityEngine;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    public Tile tile;

    public int layerIndex;
    public int animationIndex;
    public int frameIndex;

    Mesh mesh;

    List<Vector3> verts = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> tris = new List<int>();
    List<int> subTris = new List<int>();

    int v;

    public VTileChunk GetChunk()
    {
        return tile.GetTile().GetChunk(layerIndex, animationIndex, frameIndex);
    }

    public int GetIndex(int x, int y, int z)
    {
        return GetChunk().GetPaletteIndexAt(x, y, z);
    }

    public VColor GetColor(int x, int y, int z)
    {
        return tile.GetTile().GetPalette().GetColor(GetIndex(x, y, z));
    }

    void LateUpdate()
    {
        if (GetChunk().IsDirty() || tile.GetTile().GetPalette().IsDirty()) Refresh();
    }

    public void Refresh()
    {
        if (!mesh) mesh = new Mesh();
        mesh.Clear();

        v = 0;

        for (int x = 0; x < tile.width; x ++)
        {
            for (int y = 0; y < tile.height; y ++)
            {
                for (int z = 0; z < tile.depth; z ++)
                {
                    VColor c = GetColor(x, y, z);
                    if (GetColor(x, y, z).a == 0) continue;
                    Vector3 p = new Vector3(x, y, z);

                    bool useSubMesh = c.a < 255;

                    if (x == 0 || (GetColor(x - 1, y, z).a < 255 && GetColor(x - 1, y, z) != c))
                    {
                        verts.Add(p + new Vector3(0f, 0f, 1f));
                        verts.Add(p + new Vector3(0f, 1f, 1f));
                        verts.Add(p + new Vector3(0f, 1f, 0f));
                        verts.Add(p + new Vector3(0f, 0f, 0f));

                        AddFace(x, y, z, Vector3.left, useSubMesh);
                    }
                    if (x == tile.width - 1 || (GetColor(x + 1, y, z).a < 255 && GetColor(x + 1, y, z) != c))
                    {
                        verts.Add(p + new Vector3(1f, 0f, 0f));
                        verts.Add(p + new Vector3(1f, 1f, 0f));
                        verts.Add(p + new Vector3(1f, 1f, 1f));
                        verts.Add(p + new Vector3(1f, 0f, 1f));

                        AddFace(x, y, z, Vector3.right, useSubMesh);
                    }
                    if (y == 0 || (GetColor(x, y - 1, z).a < 255 && GetColor(x, y - 1, z) != c))
                    {
                        verts.Add(p + new Vector3(1f, 0f, 0f));
                        verts.Add(p + new Vector3(1f, 0f, 1f));
                        verts.Add(p + new Vector3(0f, 0f, 1f));
                        verts.Add(p + new Vector3(0f, 0f, 0f));

                        AddFace(x, y, z, Vector3.down, useSubMesh);
                    }
                    if (y == tile.height - 1 || (GetColor(x, y + 1, z).a < 255 && GetColor(x, y + 1, z) != c))
                    {
                        verts.Add(p + new Vector3(0f, 1f, 0f));
                        verts.Add(p + new Vector3(0f, 1f, 1f));
                        verts.Add(p + new Vector3(1f, 1f, 1f));
                        verts.Add(p + new Vector3(1f, 1f, 0f));

                        AddFace(x, y, z, Vector3.up, useSubMesh);
                    }
                    if (z == 0 || (GetColor(x, y, z - 1).a < 255 && GetColor(x, y, z - 1) != c))
                    {
                        verts.Add(p + new Vector3(0f, 0f, 0f));
                        verts.Add(p + new Vector3(0f, 1f, 0f));
                        verts.Add(p + new Vector3(1f, 1f, 0f));
                        verts.Add(p + new Vector3(1f, 0f, 0f));

                        AddFace(x, y, z, Vector3.back, useSubMesh);
                    }
                    if (z == tile.depth - 1 || (GetColor(x, y, z + 1).a < 255 && GetColor(x, y, z + 1) != c))
                    {
                        verts.Add(p + new Vector3(1f, 0f, 1f));
                        verts.Add(p + new Vector3(1f, 1f, 1f));
                        verts.Add(p + new Vector3(0f, 1f, 1f));
                        verts.Add(p + new Vector3(0f, 0f, 1f));

                        AddFace(x, y, z, Vector3.forward, useSubMesh);
                    }
                }
            }
        }
        
        mesh.subMeshCount = 2;

        mesh.vertices = verts.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.SetTriangles(tris.ToArray(), 0);
        mesh.SetTriangles(subTris.ToArray(), 1);

        mesh.RecalculateBounds();

        verts.Clear();
        normals.Clear();
        uvs.Clear();
        tris.Clear();
        subTris.Clear();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void AddFace(int x, int y, int z, Vector3 normal, bool subMesh)
    {
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        List<int> triList = (subMesh) ? subTris : tris;

        triList.Add(v + 0);
        triList.Add(v + 1);
        triList.Add(v + 2);

        triList.Add(v + 0);
        triList.Add(v + 2);
        triList.Add(v + 3);

        v += 4;

        int i = GetIndex(x, y, z);

        Vector2 uv = new Vector2(i + 0.5f, 0.5f) / 256f;

        uvs.Add(uv);
        uvs.Add(uv);
        uvs.Add(uv);
        uvs.Add(uv);
    }
}

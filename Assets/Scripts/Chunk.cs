using UnityEngine;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    public Tile tile;

    public int layerIndex;
    public int animationIndex;
    public int frameIndex;

    public Texture2D transTex;
    public Texture2D lineTex;
    public Texture2D transLineTex;

    Mesh mesh;

    protected MeshFilter mf;
    protected MeshCollider mc;
    protected MeshRenderer mr;

    List<Vector3> verts = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> tris = new List<int>();
    List<int> subTris = new List<int>();

    int v;

    protected virtual void Awake()
    {
        mf = GetComponent<MeshFilter>();
        mc = GetComponent<MeshCollider>();
        mr = GetComponent<MeshRenderer>();
    }

    protected virtual VTileChunk GetChunk()
    {
        return tile.GetTile().GetChunk(layerIndex, animationIndex, frameIndex);
    }

    public int GetIndex(int x, int y, int z)
    {
        return GetChunk().GetPaletteIndexAt(x, y, z);
    }

    public VColor GetColor(int x, int y, int z)
    {
        int index = GetIndex(x, y, z);
        if (index >= tile.GetTile().GetPalette().GetCount()) return new VColor(255, 0, 255, 255, 0, 0, 255, 0);
        return tile.GetTile().GetPalette().GetColor(GetIndex(x, y, z));
    }

    void LateUpdate()
    {
        VTile t = tile.GetTile();
        VLayer l = t.GetLayer(layerIndex);
        VAnimation a = t.GetAnimation(animationIndex);

        bool active = layerIndex == t.GetLayerIndex() && animationIndex == t.GetAnimationIndex() && frameIndex == t.GetFrameIndex();

        if (GetChunk().IsDirty() || t.GetPalette().IsDirty() || l.IsDirty() || a.IsDirty()) Refresh();
        bool visible = l.GetVisible() && animationIndex == t.GetAnimationIndex() && frameIndex == t.GetFrameIndex();
        gameObject.layer = (visible && (active || (!l.GetOutline() && !l.GetTransparent()))) ? 10 : 0;
        if (Tool.editing && active) visible = false;
        if (mr) mr.enabled = visible;
    }

    public void Refresh()
    {
        bool layerTrans = (layerIndex >= 0) ? tile.GetTile().GetLayer(layerIndex).GetTransparent() : false;
        bool layerLine = (layerIndex >= 0) ? tile.GetTile().GetLayer(layerIndex).GetOutline() : false;

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

                    bool useSubMesh = c.a < 255 || layerTrans || layerLine;

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
        
        mesh.subMeshCount = (subTris.Count > 0) ? 2 : 1;

        mesh.vertices = verts.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.SetTriangles(tris.ToArray(), 0);
        if (mesh.subMeshCount > 1) mesh.SetTriangles(subTris.ToArray(), 1);

        mesh.RecalculateBounds();

        verts.Clear();
        normals.Clear();
        uvs.Clear();
        tris.Clear();
        subTris.Clear();

        if (mf) mf.sharedMesh = mesh;
        if (mc)
        {
            mc.sharedMesh = null;
            mc.sharedMesh = mesh;
        }
        if (mr)
        {
            if (mesh.subMeshCount > 1)
                mr.sharedMaterials = new[] { tile.mat0, tile.mat1 };
            else
                mr.sharedMaterials = new[] { tile.mat0 };
            foreach (Material mat in mr.materials)
            {
                mat.SetTexture("_Pattern", (layerTrans) ? ((layerLine) ? transLineTex : transTex) : ((layerLine) ? lineTex : null));
            }
        }
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

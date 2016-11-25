using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public static Tile use;

    public GameObject chunkPrefab;
    
    public int width;
    public int height;
    public int depth;

    public Material opaqueMaterial;
    public Material transparentMaterial;

    List<Chunk> chunks = new List<Chunk>();
    
    Texture2D tex0;
    Texture2D tex1;

    [HideInInspector]
    public Material mat0;
    [HideInInspector]
    public Material mat1;

    void Awake()
    {
        use = this;
    }

    void Start()
    {
        tex0 = new Texture2D(256, 1, TextureFormat.ARGB32, false);
        tex1 = new Texture2D(256, 1, TextureFormat.ARGB32, false);
        tex0.filterMode = FilterMode.Point;
        tex1.filterMode = FilterMode.Point;

        mat0 = new Material(opaqueMaterial);
        mat0.SetTexture("_MainTex", tex0);
        mat0.SetTexture("_DetailTex", tex1);

        mat1 = new Material(transparentMaterial);
        mat1.SetTexture("_MainTex", tex0);
        mat1.SetTexture("_DetailTex", tex1);

        Refresh();
    }

    void Update()
    {
        if (width != Edit.use.tile.GetWidth() || height != Edit.use.tile.GetHeight() || depth != Edit.use.tile.GetDepth())
        {
            Resize(Edit.use.tile.GetWidth(), Edit.use.tile.GetHeight(), Edit.use.tile.GetDepth());
        }
    }

    void LateUpdate()
    {
        if (GetTile().IsDirty())
        {
            RefreshChunks();
        }
        if (GetTile().GetPalette().IsDirty())
        {
            RefreshPalette();
        }
    }

    void Resize(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;

        RefreshChunks();
    }

    public VTile GetTile()
    {
        return Edit.use.tile;
    }

    public void Refresh()
    {
        RefreshPalette();
        RefreshChunks();
    }

    public void RefreshPalette()
    {
        VTile tile = GetTile();
        Color32[] cs0 = new Color32[256];
        Color32[] cs1 = new Color32[256];
        for (int i = 0; i < tile.GetPalette().GetCount(); i++)
        {
            VColor vc = tile.GetPalette().GetColor(i);
            cs0[i] = new Color32(vc.r, vc.g, vc.b, vc.a);
            cs1[i] = new Color32(vc.m, vc.s, vc.e, vc.u);
        }
        tex0.SetPixels32(cs0);
        tex0.Apply();

        tex1.SetPixels32(cs1);
        tex1.Apply();
    }

    public void RefreshChunks()
    {
        List<Chunk> keptChunks = new List<Chunk>();
        for (int layer = 0; layer < GetTile().GetLayerCount(); layer ++)
        {
            for (int anim = 0; anim < GetTile().GetAnimationCount(); anim ++)
            {
                for (int frame = 0; frame < GetTile().GetAnimation(anim).GetFrameCount(); frame ++)
                {
                    Chunk c = GetChunk(layer, anim, frame);
                    if (!c)
                    {
                        GameObject chunkObj = (GameObject)Instantiate(chunkPrefab, transform.position, Quaternion.identity);
                        c = chunkObj.GetComponent<Chunk>();
                        c.tile = this;
                        c.layerIndex = layer;
                        c.animationIndex = anim;
                        c.frameIndex = frame;
                    }
                    keptChunks.Add(c);
                }
            }
        }
        foreach (Chunk chunk in chunks)
        {
            if (!keptChunks.Contains(chunk))
            {
                Destroy(chunk.gameObject);
            }
        }
        chunks = keptChunks;
        foreach (Chunk chunk in chunks) chunk.Refresh();
    }

    Chunk GetChunk(int layerIndex, int animationIndex, int frameIndex)
    {
        foreach (Chunk chunk in chunks)
        {
            if (chunk.layerIndex == layerIndex && chunk.animationIndex == animationIndex && chunk.frameIndex == frameIndex) return chunk;
        }
        return null;
    }
}

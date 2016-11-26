using UnityEngine;
using System.Collections.Generic;
using System;

public class VTile : ISerializable
{
    bool dirty;

    public void SetDirty(bool val = true)
    {
        dirty = val;
        foreach (VLayer layer in layers) layer.SetDirty(val);
        foreach (VAnimation anim in animations) anim.SetDirty(val);
        foreach (VTileChunk chunk in chunks) chunk.SetDirty(val);
        palette.SetDirty(val);
    }

    public bool IsDirty()
    {
        return dirty;
    }

    int width = 8;
    int height = 8;
    int depth = 8;

    List<VLayer> layers = new List<VLayer>();
    List<VAnimation> animations = new List<VAnimation>();
    List<VTileChunk> chunks = new List<VTileChunk>();
    VPalette palette = new VPalette();
    
    int layerIndex;
    int animationIndex;
    int frameIndex;

    public VTile()
    {
        layers.Add(new VLayer());
        animations.Add(new VAnimation());
        chunks.Add(new VTileChunk(0, 0, 0, width, height, depth));
        SetDirty();
    }

    public VTile(IReader r)
    {
        Read(r);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public int GetDepth()
    {
        return depth;
    }

    public VLayer GetLayer(int index)
    {
        return layers[index];
    }

    public int AddLayer(VLayer layer)
    {
        layers.Add(layer);
        for (int anim = 0; anim < GetAnimationCount(); anim++)
        {
            for (int frame = 0; frame < animations[anim].GetFrameCount(); frame++)
            {
                chunks.Add(new VTileChunk(layers.Count - 1, anim, frame, width, height, depth));
            }
        }
        SetDirty();
        return layers.Count - 1;
    }

    public void InsertLayer(int index, VLayer layer)
    {
        foreach (VChunk chunk in chunks)
        {
            if (chunk.GetLayerIndex() >= index) chunk.SetLayerIndex(chunk.GetLayerIndex() + 1);
        }
        layers.Insert(index, layer);
        for (int anim = 0; anim < GetAnimationCount(); anim++)
        {
            for (int frame = 0; frame < animations[anim].GetFrameCount(); frame++)
            {
                chunks.Add(new VTileChunk(index, anim, frame, width, height, depth));
            }
        }
        SetDirty();
    }

    public void RemoveLayer(int index)
    {
        foreach (VChunk chunk in chunks)
        {
            if (chunk.GetLayerIndex() > index) chunk.SetLayerIndex(chunk.GetLayerIndex() - 1);
        }
        chunks.RemoveAll((c) => c.GetLayerIndex() == index);
        layers.RemoveAt(index);
        SetDirty();
    }
    
    public int GetLayerCount()
    {
        return layers.Count;
    }

    public VAnimation GetAnimation(int index)
    {
        return animations[index];
    }

    public int GetAnimationCount()
    {
        return animations.Count;
    }

    public VPalette GetPalette()
    {
        return palette;
    }

    public int GetLayerIndex()
    {
        return layerIndex;
    }

    public void SetLayerIndex(int value)
    {
        layerIndex = value;
        SetDirty();
    }

    public int GetAnimationIndex()
    {
        return animationIndex;
    }

    public void SetAnimationIndex(int value)
    {
        animationIndex = value;
        SetDirty();
    }

    public int GetFrameIndex()
    {
        return frameIndex;
    }

    public void SetFrameIndex(int value)
    {
        frameIndex = value;
        SetDirty();
    }

    public void Read(IReader r)
    {
        width = r.Int();
        height = r.Int();
        depth = r.Int();
        int cnt = r.Int();
        layers = new List<VLayer>(cnt);
        for (int i = 0; i < cnt; i++) layers.Add(new VLayer(r));
        cnt = r.Int();
        animations = new List<VAnimation>(cnt);
        for (int i = 0; i < cnt; i++) animations.Add(new VAnimation(r));
        cnt = r.Int();
        chunks = new List<VTileChunk>(cnt);
        for (int i = 0; i < cnt; i++) chunks.Add(new VTileChunk(r));
        palette.Read(r);

        SetDirty();
    }

    public void Write(IWriter w)
    {
        w.Int(width);
        w.Int(height);
        w.Int(depth);
        w.Int(layers.Count);
        foreach (VLayer layer in layers) layer.Write(w);
        w.Int(animations.Count);
        foreach (VAnimation anim in animations) anim.Write(w);
        w.Int(chunks.Count);
        foreach (VTileChunk chunk in chunks) chunk.Write(w);
        palette.Write(w);
    }

    public void Resize(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;

        foreach (VTileChunk chunk in chunks) chunk.Resize(width, height, depth);

        SetDirty();
    }

    public VTileChunk GetChunk(int layerIndex, int animationIndex, int frameIndex)
    {
        foreach (VTileChunk chunk in chunks)
        {
            if (chunk.GetLayerIndex() == layerIndex && chunk.GetAnimationIndex() == animationIndex && chunk.GetFrameIndex() == frameIndex) return chunk;
        }
        return null;
    }
}

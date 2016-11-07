using UnityEngine;
using System.Collections;
using System;

public abstract class VChunk : ISerializable
{
    bool dirty;

    public void SetDirty(bool val = true)
    {
        dirty = val;
    }

    public bool IsDirty()
    {
        return dirty;
    }

    int layerIndex;
    int animationIndex;
    int frameIndex;

    public VChunk()
    {

    }

    public VChunk(int layerIndex, int animationIndex, int frameIndex)
    {
        this.layerIndex = layerIndex;
        this.animationIndex = animationIndex;
        this.frameIndex = frameIndex;
    }

    public int GetLayerIndex()
    {
        return layerIndex;
    }

    public int GetAnimationIndex()
    {
        return animationIndex;
    }

    public int GetFrameIndex()
    {
        return frameIndex;
    }

    public void SetIndices(int layerIndex, int animationIndex, int frameIndex)
    {
        this.layerIndex = layerIndex;
        this.animationIndex = animationIndex;
        this.frameIndex = frameIndex;

        SetDirty();
    }

    public virtual void Read(IReader r)
    {
        layerIndex = r.Int();
        animationIndex = r.Int();
        frameIndex = r.Int();
        SetDirty();
    }

    public virtual void Write(IWriter w)
    {
        w.Int(layerIndex);
        w.Int(animationIndex);
        w.Int(frameIndex);
    }
}
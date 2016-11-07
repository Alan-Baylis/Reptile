using UnityEngine;
using System.Collections.Generic;
using System;

public class VAnimation : ISerializable
{
    bool dirty;

    public void SetDirty(bool val = true)
    {
        dirty = val;
        foreach (VFrame frame in frames) frame.SetDirty(val);
    }

    public bool IsDirty()
    {
        return dirty;
    }

    List<VFrame> frames = new List<VFrame>();

    public VAnimation()
    {
        frames.Add(new VFrame());
    }

    public VAnimation(IReader r)
    {
        Read(r);
    }

    public void AddFrame(VFrame frame)
    {
        frames.Add(frame);
        SetDirty();
    }

    public int GetFrameCount()
    {
        return frames.Count;
    }

    public void Read(IReader r)
    {
        int len = r.Int();
        frames = new List<VFrame>(len);
        for (int i = 0; i < len; i++)
        {
            frames.Add(new VFrame(r));
        }
        SetDirty();
    }

    public void Write(IWriter w)
    {
        w.Int(frames.Count);
        for (int i = 0; i < frames.Count; i++) frames[i].Write(w);
    }
}

using UnityEngine;
using System.Collections.Generic;
using System;

public class VAnimation : ISerializable
{
    string name = "Default";
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

    public VAnimation(string name) : this()
    {
        this.name = name;
    }

    public string GetName()
    {
        return name;
    }

    public void SetName(string name)
    {
        this.name = name;
        SetDirty();
    }

    public VFrame GetFrame(int index)
    {
        return frames[index];
    }

    public int AddFrame(VFrame frame)
    {
        frames.Add(frame);
        SetDirty();
        return frames.Count - 1;
    }

    public void InsertFrame(int index, VFrame frame)
    {
        frames.Insert(index, frame);
        SetDirty();
    }

    public void RemoveFrame(int index)
    {
        frames.RemoveAt(index);
        SetDirty();
    }

    public int GetFrameCount()
    {
        return frames.Count;
    }

    public float GetDuration()
    {
        float duration = 0f;
        foreach (VFrame frame in frames) duration += frame.GetDuration();
        return duration;
    }

    public void Read(IReader r)
    {
        name = r.String();
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
        w.String(name);
        w.Int(frames.Count);
        for (int i = 0; i < frames.Count; i++) frames[i].Write(w);
    }
}

using UnityEngine;
using System.Collections;
using System;

public class VFrame : ISerializable
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

    float duration = 0.1f;

    public VFrame()
    {

    }

    public VFrame(IReader r)
    {
        Read(r);
    }

    public float GetDuration()
    {
        return duration;
    }

    public void SetDuration(float value)
    {
        duration = value;
        SetDirty();
    }

    public void Read(IReader r)
    {
        duration = r.Float();

        SetDirty();
    }

    public void Write(IWriter w)
    {
        w.Float(duration);
    }
}

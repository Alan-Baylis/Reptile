
using System;
using System.Collections.Generic;

public class VPalette : ISerializable
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

    int index;
    List<VColor> colors = new List<VColor>();

    public VPalette()
    {
        colors.Add(new VColor(0, 0, 0, 0));
        colors.Add(new VColor(255, 255, 255, 255));
        index = 1;
    }

    public VPalette(IReader r)
    {
        Read(r);
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetIndex(int value)
    {
        index = value;
        SetDirty();
    }

    public VColor GetColor(int index)
    {
        return colors[index];
    }

    public void SetColor(int index, VColor color)
    {
        colors[index] = color;
        SetDirty();
    }

    public void AddColor(VColor color)
    {
        colors.Add(color);
        SetDirty();
    }

    public void RemoveColor(int index)
    {
        colors.RemoveAt(index);
    }

    public int GetCount()
    {
        return colors.Count;
    }

    public void Read(IReader r)
    {
        int len = r.Int();
        colors = new List<VColor>(len);
        for (int i = 0; i < len; i ++)
        {
            colors.Add(new VColor(0, 0, 0, 255));
            colors[i].Read(r);
        }
        if (colors.Count > 0) index = 1;
        else index = 0;

        SetDirty();
    }

    public void Write(IWriter w)
    {
        w.Int(colors.Count);
        for (int i = 0; i < colors.Count; i++) colors[i].Write(w);
    }
}

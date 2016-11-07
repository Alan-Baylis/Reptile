using UnityEngine;
using System.Collections;
using System;

public class VLayer : ISerializable
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

    string name = "Default";
    bool visible = true;
    bool transparent;
    bool outline;

    public VLayer()
    {
        
    }

    public VLayer(string name)
    {
        this.name = name;
    }

    public VLayer(IReader r)
    {
        Read(r);
    }

    public string GetName()
    {
        return name;
    }

    public void SetName(string value)
    {
        name = value;
        SetDirty();
    }

    public bool GetVisible()
    {
        return visible;
    }

    public void SetVisible(bool value)
    {
        visible = value;
        SetDirty();
    }

    public bool GetTransparent()
    {
        return transparent;
    }

    public void SetTransparent(bool value)
    {
        transparent = value;
        SetDirty();
    }

    public bool GetOutline()
    {
        return outline;
    }

    public void SetOutline(bool value)
    {
        outline = value;
        SetDirty();
    }

    public void Read(IReader r)
    {
        name = r.String();
        visible = r.Bool();
        transparent = r.Bool();
        outline = r.Bool();

        SetDirty();
    }

    public void Write(IWriter w)
    {
        w.String(name);
        w.Bool(visible);
        w.Bool(transparent);
        w.Bool(outline);
    }
}

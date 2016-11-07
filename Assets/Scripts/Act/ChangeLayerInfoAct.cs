using UnityEngine;
using System.Collections;
using System;

public class ChangeLayerInfoAct : Act
{
    int layerIndex;
    string name;
    bool visible;
    bool transparent;
    bool outline;

    string oldName;
    bool oldVisible;
    bool oldTransparent;
    bool oldOutline;

    public ChangeLayerInfoAct(int layerIndex, string name, bool visible, bool transparent, bool outline)
    {
        this.layerIndex = layerIndex;
        this.name = name;
        this.visible = visible;
        this.transparent = transparent;
        this.outline = outline;
    }

    public override void Do()
    {
        VLayer layer = Edit.use.tile.GetLayer(layerIndex);

        oldName = name;
        oldVisible = visible;
        oldTransparent = transparent;
        oldOutline = outline;

        layer.SetName(name);
        layer.SetVisible(visible);
        layer.SetTransparent(transparent);
        layer.SetOutline(outline);
    }

    public override void Undo()
    {
        VLayer layer = Edit.use.tile.GetLayer(layerIndex);

        layer.SetName(oldName);
        layer.SetVisible(oldVisible);
        layer.SetTransparent(oldTransparent);
        layer.SetOutline(oldOutline);
    }

    public override bool IsNoOp()
    {
        VLayer layer = Edit.use.tile.GetLayer(layerIndex);
        return name == layer.GetName() && visible == layer.GetVisible() && transparent == layer.GetTransparent() && outline == layer.GetOutline();
    }

    public override bool CanBatch()
    {
        return true;
    }

    public override string ToString()
    {
        return "Change Layer Data";
    }
}

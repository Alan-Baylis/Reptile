using UnityEngine;
using System.Collections;
using System;

public class AddPaletteColorAct : Act
{
    VColor color;

    public AddPaletteColorAct(VColor color)
    {
        this.color = new VColor(color);
    }

    public override void Do()
    {
        Edit.use.tile.GetPalette().AddColor(color);
    }

    public override void Undo()
    {
        Edit.use.tile.GetPalette().RemoveColor(Edit.use.tile.GetPalette().GetCount() - 1);
    }

    public override bool IsNoOp()
    {
        return false;
    }

    public override string ToString()
    {
        return "Add Palette Color";
    }
}

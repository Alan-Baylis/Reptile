using UnityEngine;
using System.Collections;
using System;

public class RemovePaletteColorAct : Act
{
    VColor color;

    public RemovePaletteColorAct()
    {

    }

    public override void Do()
    {
        color = Edit.use.tile.GetPalette().GetColor(Edit.use.tile.GetPalette().GetCount() - 1);
        Edit.use.tile.GetPalette().RemoveColor(Edit.use.tile.GetPalette().GetCount() - 1);
    }

    public override void Undo()
    {
        Edit.use.tile.GetPalette().AddColor(color);
    }

    public override bool IsNoOp()
    {
        return false;
    }

    public override string ToString()
    {
        return "Remove Palette Color";
    }
}

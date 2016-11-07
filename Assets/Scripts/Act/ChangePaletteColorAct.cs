using UnityEngine;
using System.Collections;
using System;

public class ChangePaletteColorAct : Act
{
    VColor color;
    VColor oldColor;
    int index;

    public ChangePaletteColorAct(VColor color, int index)
    {
        this.color = new VColor(color);
        this.index = index;
    }

    public override void Do()
    {
        oldColor = new VColor(Edit.use.tile.GetPalette().GetColor(index));
        Edit.use.tile.GetPalette().SetColor(index, new VColor(color));
    }

    public override void Undo()
    {
        Edit.use.tile.GetPalette().SetColor(index, new VColor(oldColor));
    }

    public override bool IsNoOp()
    {
        return color == Edit.use.tile.GetPalette().GetColor(index);
    }

    public override bool CanBatch()
    {
        return true;
    }

    public override string ToString()
    {
        return "Change Palette Color";
    }
}

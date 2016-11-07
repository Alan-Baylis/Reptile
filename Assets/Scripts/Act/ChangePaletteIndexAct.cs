using UnityEngine;
using System.Collections;
using System;

public class ChangePaletteIndexAct : Act
{
    int index;
    int oldIndex;

    public ChangePaletteIndexAct(int index)
    {
        this.index = index;
    }

    public override void Do()
    {
        oldIndex = Edit.use.tile.GetPalette().GetIndex();
        Edit.use.tile.GetPalette().SetIndex(index);
    }

    public override void Undo()
    {
        Edit.use.tile.GetPalette().SetIndex(oldIndex);
    }

    public override bool IsNoOp()
    {
        return index == Edit.use.tile.GetPalette().GetIndex();
    }

    public override string ToString()
    {
        return "Select Palette Color";
    }
}

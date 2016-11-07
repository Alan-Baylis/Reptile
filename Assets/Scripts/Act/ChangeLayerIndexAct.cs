using UnityEngine;
using System.Collections;
using System;

public class ChangeLayerIndexAct : Act
{
    int oldIndex;
    int index;

    public ChangeLayerIndexAct(int index)
    {
        this.index = index;
    }

    public override void Do()
    {
        oldIndex = Edit.use.tile.GetLayerIndex();
        Edit.use.tile.SetLayerIndex(index);
    }

    public override void Undo()
    {
        Edit.use.tile.SetLayerIndex(oldIndex);
    }

    public override bool IsNoOp()
    {
        return index == Edit.use.tile.GetLayerIndex();
    }

    public override string ToString()
    {
        return "Switch Layer";
    }
}

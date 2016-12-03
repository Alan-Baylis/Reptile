using UnityEngine;
using System.Collections;
using System;

public class ChangeFrameIndexAct : Act
{
    int oldIndex;
    int index;

    public ChangeFrameIndexAct(int index)
    {
        this.index = index;
    }

    public override void Do()
    {
        oldIndex = Edit.use.tile.GetFrameIndex();
        Edit.use.tile.SetFrameIndex(index);
    }

    public override void Undo()
    {
        Edit.use.tile.SetFrameIndex(oldIndex);
    }

    public override bool CanBatch()
    {
        return true;
    }

    public override bool IsNoOp()
    {
        return index == Edit.use.tile.GetFrameIndex();
    }

    public override string ToString()
    {
        return "Switch Frame";
    }
}

using UnityEngine;
using System.Collections;
using System;

public class ChangeAnimationIndexAct : Act
{
    int oldIndex;
    int oldFrameIndex;
    int index;

    public ChangeAnimationIndexAct(int index)
    {
        this.index = index;
    }

    public override void Do()
    {
        oldIndex = Edit.use.tile.GetAnimationIndex();
        Edit.use.tile.SetAnimationIndex(index);
        oldFrameIndex = Edit.use.tile.GetFrameIndex();
        Edit.use.tile.SetFrameIndex(0);
    }

    public override void Undo()
    {
        Edit.use.tile.SetAnimationIndex(oldIndex);
        Edit.use.tile.SetFrameIndex(oldFrameIndex);
    }

    public override bool IsNoOp()
    {
        return index == Edit.use.tile.GetAnimationIndex();
    }

    public override string ToString()
    {
        return "Switch Animation";
    }
}

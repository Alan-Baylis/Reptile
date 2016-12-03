using UnityEngine;
using System.Collections;
using System;

public class AddFrameAct : Act
{
    int animationIndex;
    int index;

    public AddFrameAct(int animationIndex)
    {
        this.animationIndex = animationIndex;
    }

    public override void Do()
    {
        index = Edit.use.tile.AddFrame(animationIndex, new VFrame());
    }

    public override void Undo()
    {
        Edit.use.tile.RemoveFrame(animationIndex, index);
    }

    public override bool IsNoOp()
    {
        return false;
    }

    public override string ToString()
    {
        return "Add Animation Frame";
    }
}

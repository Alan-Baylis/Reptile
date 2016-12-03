using UnityEngine;
using System.Collections;
using System;

public class RemoveFrameAct : Act
{
    int animationIndex;
    int frameIndex;
    byte[] data;
    string name;

    public RemoveFrameAct(int animationIndex, int frameIndex)
    {
        this.animationIndex = animationIndex;
        this.frameIndex = frameIndex;
    }

    public override void Do()
    {
        data = new BinaryWriter(Edit.use.tile).GetOutput();
        Edit.use.tile.RemoveFrame(animationIndex, frameIndex);
    }

    public override void Undo()
    {
        Edit.use.tile.Read(new BinaryReader(data));
    }

    public override bool IsNoOp()
    {
        return false;
    }

    public override string ToString()
    {
        return "Remove Animation Frame";
    }
}

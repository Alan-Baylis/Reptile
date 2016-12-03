using UnityEngine;
using System.Collections;
using System;

public class RemoveAnimationAct : Act
{
    int index;
    byte[] data;
    string name;

    public RemoveAnimationAct(int index)
    {
        this.index = index;
    }

    public override void Do()
    {
        data = new BinaryWriter(Edit.use.tile).GetOutput();
        name = Edit.use.tile.GetAnimation(index).GetName();
        Edit.use.tile.RemoveAnimation(index);
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
        return "Remove Animation \"" + name + "\"";
    }
}

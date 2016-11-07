using UnityEngine;
using System.Collections;
using System;

public class RemoveLayerAct : Act
{
    int index;
    byte[] data;
    string name;

    public RemoveLayerAct(int index)
    {
        this.index = index;
    }

    public override void Do()
    {
        data = new BinaryWriter(Edit.use.tile.GetLayer(index)).GetOutput();
        name = Edit.use.tile.GetLayer(index).GetName();
        Edit.use.tile.RemoveLayer(index);
    }

    public override void Undo()
    {
        Edit.use.tile.InsertLayer(index, new VLayer(new BinaryReader(data)));
    }

    public override bool IsNoOp()
    {
        return false;
    }

    public override string ToString()
    {
        return "Remove Layer \"" + name + "\"";
    }
}

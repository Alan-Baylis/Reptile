using UnityEngine;
using System.Collections;
using System;

public class AddLayerAct : Act
{
    string name;
    int index;

    public AddLayerAct(string name)
    {
        this.name = name;
    }

    public override void Do()
    {
        Edit.use.tile.AddLayer(new VLayer(name));
        index = Edit.use.tile.GetLayerCount() - 1;
    }

    public override void Undo()
    {
        Edit.use.tile.RemoveLayer(index);
    }

    public override bool IsNoOp()
    {
        return false;
    }

    public override string ToString()
    {
        return "Add Layer \"" + name + "\"";
    }
}

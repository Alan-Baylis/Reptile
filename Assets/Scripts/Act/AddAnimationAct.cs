using UnityEngine;
using System.Collections;
using System;

public class AddAnimationAct : Act
{
    string name;
    int index;

    public AddAnimationAct(string name)
    {
        this.name = name;
    }

    public override void Do()
    {
        Edit.use.tile.AddAnimation(new VAnimation(name));
        index = Edit.use.tile.GetAnimationCount() - 1;
    }

    public override void Undo()
    {
        Edit.use.tile.RemoveAnimation(index);
    }

    public override bool IsNoOp()
    {
        return false;
    }

    public override string ToString()
    {
        return "Add Animation \"" + name + "\"";
    }
}

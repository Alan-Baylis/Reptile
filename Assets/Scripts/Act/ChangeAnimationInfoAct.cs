using UnityEngine;
using System.Collections;
using System;

public class ChangeAnimationInfoAct : Act
{
    int animationIndex;
    string name;

    string oldName;

    public ChangeAnimationInfoAct(int animationIndex, string name)
    {
        this.animationIndex = animationIndex;
        this.name = name;
    }

    public override void Do()
    {
        VAnimation anim = Edit.use.tile.GetAnimation(animationIndex);

        oldName = name;

        anim.SetName(name);
    }

    public override void Undo()
    {
        VAnimation anim = Edit.use.tile.GetAnimation(animationIndex);

        anim.SetName(oldName);
    }

    public override bool IsNoOp()
    {
        VAnimation anim = Edit.use.tile.GetAnimation(animationIndex);
        return name == anim.GetName();
    }

    public override bool CanBatch()
    {
        return true;
    }

    public override string ToString()
    {
        return "Change Animation Data";
    }
}

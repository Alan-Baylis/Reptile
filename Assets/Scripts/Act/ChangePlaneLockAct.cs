using UnityEngine;
using System.Collections;
using System;

public class ChangePlaneLockAct : Act
{
    bool value;
    bool oldValue;

    public ChangePlaneLockAct(bool value)
    {
        this.value = value;
    }

    public override void Do()
    {
        oldValue = Edit.use.planeLock;
        Edit.use.planeLock = value;
    }

    public override void Undo()
    {
        Edit.use.planeLock = oldValue;
    }

    public override bool IsNoOp()
    {
        return value == Edit.use.planeLock;
    }

    public override string ToString()
    {
        return "Toggle Plane Locking";
    }
}

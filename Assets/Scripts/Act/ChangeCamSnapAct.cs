using UnityEngine;
using System.Collections;
using System;

public class ChangeCamSnapAct : Act
{
    bool value;
    bool oldValue;

    public ChangeCamSnapAct(bool value)
    {
        this.value = value;
    }

    public override void Do()
    {
        oldValue = Edit.use.camSnap;
        Edit.use.camSnap = value;
    }

    public override void Undo()
    {
        Edit.use.camSnap = oldValue;
    }

    public override bool IsNoOp()
    {
        return value == Edit.use.camSnap;
    }

    public override string ToString()
    {
        return "Toggle Camera Snapping";
    }
}

using UnityEngine;
using System.Collections;
using System;

public class ChangeCamOrthoAct : Act
{
    bool value;
    bool oldValue;

    public ChangeCamOrthoAct(bool value)
    {
        this.value = value;
    }

    public override void Do()
    {
        oldValue = Cam.use.cam.orthographic;
        Cam.use.cam.orthographic = value;
    }

    public override void Undo()
    {
        Cam.use.cam.orthographic = oldValue;
    }

    public override bool IsNoOp()
    {
        return value == Cam.use.cam.orthographic;
    }

    public override string ToString()
    {
        return "Change Camera Type";
    }
}

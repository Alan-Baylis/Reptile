using UnityEngine;
using System.Collections;
using System;

public class ChangeSymmetryAct : Act
{
    bool x;
    bool y;
    bool z;

    bool oldX;
    bool oldY;
    bool oldZ;

    public ChangeSymmetryAct(bool x, bool y, bool z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public override void Do()
    {
        oldX = Edit.use.mirrorX;
        oldY = Edit.use.mirrorY;
        oldZ = Edit.use.mirrorZ;

        Edit.use.mirrorX = x;
        Edit.use.mirrorY = y;
        Edit.use.mirrorZ = z;
    }

    public override void Undo()
    {
        Edit.use.mirrorX = oldX;
        Edit.use.mirrorY = oldY;
        Edit.use.mirrorZ = oldZ;
    }

    public override bool IsNoOp()
    {
        return Edit.use.mirrorX == x && Edit.use.mirrorY == y && Edit.use.mirrorZ == z;
    }

    public override string ToString()
    {
        return "Change Symmetry";
    }
}

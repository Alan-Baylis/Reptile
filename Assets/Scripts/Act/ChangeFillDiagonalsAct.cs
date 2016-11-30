using UnityEngine;
using System.Collections;
using System;

public class ChangeFillDiagonalsAct : Act
{
    bool value;
    bool oldValue;

    public ChangeFillDiagonalsAct(bool value)
    {
        this.value = value;
    }

    public override void Do()
    {
        oldValue = Edit.use.fillDiagonals;
        Edit.use.fillDiagonals = value;
    }

    public override void Undo()
    {
        Edit.use.fillDiagonals = oldValue;
    }

    public override bool IsNoOp()
    {
        return value == Edit.use.fillDiagonals;
    }

    public override string ToString()
    {
        return "Toggle Filling Diagonals";
    }
}

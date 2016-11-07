using UnityEngine;
using System.Collections;
using System;

public class UndoAct : Act
{
    public override void Do()
    {
        Act act = Edit.undos.Pop();
        act.Undo();
        Edit.redos.Push(act);
    }

    public override void Undo()
    {
        throw new NotImplementedException();
    }

    public override bool CanUndo()
    {
        return false;
    }

    public override bool IsNoOp()
    {
        return Edit.undos.Count == 0;
    }
}

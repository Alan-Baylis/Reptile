using UnityEngine;
using System.Collections;
using System;

public class RedoAct : Act
{
    public override void Do()
    {
        Act act = Edit.redos.Pop();
        act.Redo();
        Edit.undos.Push(act);
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
        return Edit.redos.Count == 0;
    }
}

using UnityEngine;
using System.Collections;

public abstract class Act {

    public abstract void Do();
    public abstract void Undo();
    public abstract bool IsNoOp();
    public virtual void Redo()
    {
        Do();
    }
    public virtual bool CanBatch()
    {
        return false;
    }
    public virtual bool CanUndo()
    {
        return true;
    }
}

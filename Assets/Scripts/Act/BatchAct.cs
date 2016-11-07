using UnityEngine;
using System.Collections.Generic;
using System;

public class BatchAct : Act
{
    string desc;
    Act[] acts;

    public BatchAct(Act[] acts, string desc)
    {
        this.acts = acts;
        this.desc = desc;
    }

    public override void Do()
    {
        for (int i = 0; i < acts.Length; i++) acts[i].Do();
    }

    public override void Undo()
    {
        for (int i = acts.Length - 1; i >= 0; i--) acts[i].Undo();
    }

    public override void Redo()
    {
        for (int i = 0; i < acts.Length; i++) acts[i].Redo();
    }

    public override bool IsNoOp()
    {
        return false;
    }

    public override string ToString()
    {
        return desc;
    }
}

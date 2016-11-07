using UnityEngine;
using System.Collections;
using System;

public class ChangeToolAct : Act
{
    Edit.Tool tool;
    Edit.Tool prevTool;

    public ChangeToolAct(Edit.Tool tool)
    {
        this.tool = tool;
    }

    public override void Do()
    {
        prevTool = Edit.use.tool;
        Edit.use.tool = tool;
    }

    public override void Undo()
    {
        Edit.use.tool = prevTool;
    }

    public override bool CanUndo()
    {
        return false;
    }

    public override bool IsNoOp()
    {
        return tool == Edit.use.tool;
    }

    public override string ToString()
    {
        return "Change Tool";
    }
}

using UnityEngine;
using System.Collections;
using System;

public class ChangeBrushAct : Act
{
    Edit.Brush brush;
    Edit.Brush prevBrush;

    public ChangeBrushAct(Edit.Brush brush)
    {
        this.brush = brush;
    }

    public override void Do()
    {
        prevBrush = Edit.use.brush;
        Edit.use.brush = brush;
    }

    public override void Undo()
    {
        Edit.use.brush = prevBrush;
    }

    public override bool CanUndo()
    {
        return false;
    }

    public override bool IsNoOp()
    {
        return brush == Edit.use.brush;
    }

    public override string ToString()
    {
        return "Change Brush";
    }
}

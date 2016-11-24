using UnityEngine;
using System.Collections;
using System;

public class ChangeBrushSizeAct : Act
{
    int size;
    int oldSize;

    public ChangeBrushSizeAct(int size)
    {
        this.size = size;
    }

    public override void Do()
    {
        oldSize = Edit.use.brushSize;
        Edit.use.brushSize = size;
    }

    public override void Undo()
    {
        Edit.use.brushSize = oldSize;
    }

    public override bool IsNoOp()
    {
        return size == Edit.use.brushSize;
    }

    public override string ToString()
    {
        return "Change Brush Size";
    }
}

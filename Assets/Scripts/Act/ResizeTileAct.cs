using UnityEngine;
using System.Collections;
using System;

public class ResizeTileAct : Act
{
    int width;
    int height;
    int depth;

    byte[] backup;

    public ResizeTileAct(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
    }

    public override void Do()
    {
        backup = new BinaryWriter(Edit.use.tile).GetOutput();
        Edit.use.tile.Resize(width, height, depth);
    }

    public override void Undo()
    {
        Edit.use.tile.Read(new BinaryReader(backup));
    }

    public override bool IsNoOp()
    {
        return width == Edit.use.tile.GetWidth() && height == Edit.use.tile.GetHeight() && depth == Edit.use.tile.GetDepth();
    }

    public override string ToString()
    {
        return "Resize Tile";
    }
}

using UnityEngine;
using System.Collections;
using System;

public class NewTileAct : Act
{
    byte[] oldData;

    public override void Do()
    {
        oldData = new BinaryWriter(Edit.use.tile).GetOutput();
        Edit.use.tile = new VTile();
    }

    public override void Undo()
    {
        Edit.use.tile = new VTile(new BinaryReader(oldData));
    }

    public override bool IsNoOp()
    {
        return false;
    }

    public override string ToString()
    {
        return "New Tile";
    }
}

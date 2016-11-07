using UnityEngine;
using System.Collections;
using System;

public class LoadTileAct : Act
{
    byte[] data;
    byte[] oldData;

    public LoadTileAct(byte[] data)
    {
        this.data = data;
    }

    public override void Do()
    {
        oldData = new BinaryWriter(Edit.use.tile).GetOutput();
        Edit.use.tile = new VTile(new BinaryReader(data));
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
        return "Load Tile";
    }
}

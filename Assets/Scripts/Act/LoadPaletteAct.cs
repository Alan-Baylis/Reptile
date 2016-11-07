using UnityEngine;
using System.Collections;
using System;

public class LoadPaletteAct : Act
{
    byte[] data;
    byte[] oldData;

    public LoadPaletteAct(byte[] data)
    {
        this.data = data;
    }

    public override void Do()
    {
        oldData = new BinaryWriter(Edit.use.tile).GetOutput();
        Edit.use.tile.GetPalette().Read(new BinaryReader(data));
    }

    public override void Undo()
    {
        Edit.use.tile.GetPalette().Read(new BinaryReader(oldData));
    }

    public override bool IsNoOp()
    {
        return false;
    }

    public override string ToString()
    {
        return "Load Palette";
    }
}

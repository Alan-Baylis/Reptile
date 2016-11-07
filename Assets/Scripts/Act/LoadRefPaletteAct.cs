using UnityEngine;
using System.Collections;
using System;

public class LoadRefPaletteAct : Act
{
    byte[] data;
    byte[] oldData;

    public LoadRefPaletteAct(byte[] data)
    {
        this.data = data;
    }

    public override void Do()
    {
        oldData = new BinaryWriter(Edit.use.tile).GetOutput();
        Edit.use.refPalette = new VPalette(new BinaryReader(data));
    }

    public override void Undo()
    {
        Edit.use.refPalette = new VPalette(new BinaryReader(oldData));
    }

    public override bool IsNoOp()
    {
        return false;
    }

    public override string ToString()
    {
        return "Load Reference Palette";
    }
}

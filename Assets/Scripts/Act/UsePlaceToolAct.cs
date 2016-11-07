using UnityEngine;
using System.Collections;
using System;

public class UsePlaceToolAct : Act
{
    int x;
    int y;
    int z;

    byte index;
    byte oldIndex;

    int layerIndex;
    int animationIndex;
    int frameIndex;

    public UsePlaceToolAct(int x, int y, int z, byte index)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.index = index;
    }

    public override void Do()
    {
        layerIndex = Edit.use.tile.GetLayerIndex();
        animationIndex = Edit.use.tile.GetAnimationIndex();
        frameIndex = Edit.use.tile.GetFrameIndex();

        VTileChunk chunk = Edit.use.tile.GetChunk(layerIndex, animationIndex, frameIndex);

        oldIndex = chunk.GetPaletteIndexAt(x, y, z);
        chunk.SetPaletteIndexAt(x, y, z, index);
    }

    public override void Undo()
    {
        VTileChunk chunk = Edit.use.tile.GetChunk(layerIndex, animationIndex, frameIndex);
        chunk.SetPaletteIndexAt(x, y, z, oldIndex);
    }

    public override bool IsNoOp()
    {
        layerIndex = Edit.use.tile.GetLayerIndex();
        animationIndex = Edit.use.tile.GetAnimationIndex();
        frameIndex = Edit.use.tile.GetFrameIndex();

        VTileChunk chunk = Edit.use.tile.GetChunk(layerIndex, animationIndex, frameIndex);

        int i = chunk.GetPaletteIndexAt(x, y, z);

        return index == i || (index != 0 && i != 0);
    }

    public override bool CanBatch()
    {
        return true;
    }

    public override string ToString()
    {
        return "Use Place Tool";
    }
}

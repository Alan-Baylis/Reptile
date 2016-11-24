using UnityEngine;
using System.Collections;
using System;

public class EditChunkAct : Act
{
    byte[] indices;
    byte[] oldIndices;

    int layerIndex;
    int animationIndex;
    int frameIndex;

    public EditChunkAct(byte[] indices)
    {
        this.indices = indices;
    }

    public override void Do()
    {
        layerIndex = Edit.use.tile.GetLayerIndex();
        animationIndex = Edit.use.tile.GetAnimationIndex();
        frameIndex = Edit.use.tile.GetFrameIndex();

        VTileChunk chunk = Edit.use.tile.GetChunk(layerIndex, animationIndex, frameIndex);

        oldIndices = chunk.GetPaletteIndices();
        chunk.SetPaletteIndices(indices);
    }

    public override void Undo()
    {
        VTileChunk chunk = Edit.use.tile.GetChunk(layerIndex, animationIndex, frameIndex);
        chunk.SetPaletteIndices(oldIndices);
    }

    public override bool IsNoOp()
    {
        layerIndex = Edit.use.tile.GetLayerIndex();
        animationIndex = Edit.use.tile.GetAnimationIndex();
        frameIndex = Edit.use.tile.GetFrameIndex();

        VTileChunk chunk = Edit.use.tile.GetChunk(layerIndex, animationIndex, frameIndex);

        byte[] curIndices = chunk.GetPaletteIndices();
        for (int i = 0; i < curIndices.Length; i ++)
        {
            if (curIndices[i] != indices[i]) return false;
        }

        return true;
    }

    public override string ToString()
    {
        return "Edit Chunk";
    }
}

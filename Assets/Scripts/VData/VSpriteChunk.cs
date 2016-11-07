using UnityEngine;
using System.Collections;
using System;

public class VSpriteChunk : VChunk
{
    public int width;
    public int height;

    byte[] data;

    public VSpriteChunk(IReader r)
    {
        Read(r);
    }

    public VSpriteChunk(int layerIndex, int animationIndex, int frameIndex, int width, int height)
        : base(layerIndex, animationIndex, frameIndex)
    {
        this.width = width;
        this.height = height;

        data = new byte[width * height];
    }

    int GetIndex(int x, int y)
    {
        return y * width + x;
    }

    public byte GetPaletteIndexAt(int x, int y)
    {
        return data[GetIndex(x, y)];
    }

    public void SetPaletteIndexAt(int x, int y, byte value)
    {
        data[GetIndex(x, y)] = value;
    }

    public void Resize(int width, int height)
    {
        byte[] newData = new byte[width * height];
        for (int y = 0; y < Mathf.Min(this.height, height); y++)
        {
            for (int x = 0; x < Mathf.Min(this.width, width); x++)
            {
                newData[y * width + x] = data[y * this.width + x];
            }
        }
        this.width = width;
        this.height = height;
        data = newData;
    }

    public override void Read(IReader r)
    {
        base.Read(r);
        width = r.Int();
        height = r.Int();
        data = new byte[width * height];
        for (int i = 0; i < width * height; i++) data[i] = r.Byte();
    }

    public override void Write(IWriter w)
    {
        base.Write(w);
        w.Int(width);
        w.Int(height);
        for (int i = 0; i < width * height; i++) w.Byte(data[i]);
    }
}
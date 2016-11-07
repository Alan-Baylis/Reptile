using UnityEngine;
using System.Collections;

public class VTileChunk : VChunk
{
    int width;
    int height;
    int depth;

    byte[] data;

    public VTileChunk(IReader r)
    {
        Read(r);
    }

    public VTileChunk(int layerIndex, int animationIndex, int frameIndex, int width, int height, int depth)
        : base(layerIndex, animationIndex, frameIndex)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;

        data = new byte[width * height * depth];
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public int GetDepth()
    {
        return depth;
    }

    int GetIndex(int x, int y, int z)
    {
        return z * width * height + y * width + x;
    }

    public byte GetPaletteIndexAt(int x, int y, int z)
    {
        return data[GetIndex(x, y, z)];
    }

    public void SetPaletteIndexAt(int x, int y, int z, byte value)
    {
        data[GetIndex(x, y, z)] = value;
        SetDirty();
    }

    public void Resize(int width, int height, int depth)
    {
        byte[] newData = new byte[width * height * depth];
        for (int z = 0; z < Mathf.Min(this.depth, depth); z++)
        {
            for (int y = 0; y < Mathf.Min(this.height, height); y++)
            {
                for (int x = 0; x < Mathf.Min(this.width, width); x++)
                {
                    newData[z * width * height + y * width + x] = data[z * this.width * this.height + y * this.width + x];
                }
            }
        }
        this.width = width;
        this.height = height;
        this.depth = depth;
        data = newData;

        SetDirty();
    }

    public override void Read(IReader r)
    {
        base.Read(r);
        width = r.Int();
        height = r.Int();
        depth = r.Int();
        data = new byte[width * height * depth];
        for (int i = 0; i < width * height * depth; i++) data[i] = r.Byte();

        SetDirty();
    }

    public override void Write(IWriter w)
    {
        base.Write(w);
        w.Int(width);
        w.Int(height);
        w.Int(depth);
        for (int i = 0; i < width * height * depth; i++) w.Byte(data[i]);
    }
}
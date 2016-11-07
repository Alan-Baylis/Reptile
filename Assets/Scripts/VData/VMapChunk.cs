using UnityEngine;
using System.Collections;
using System;

public class VMapChunk : VChunk
{
    public int width;
    public int height;
    public int depth;

    Data[] data;

    public VMapChunk(int layerIndex, int animationIndex, int frameIndex, int width, int height, int depth)
        : base(layerIndex, animationIndex, frameIndex)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;

        data = new Data[width * height * depth];
    }

    int GetIndex(int x, int y, int z)
    {
        return z * width * height + y * width + x;
    }

    public byte GetPaletteGroupIndexAt(int x, int y, int z)
    {
        return data[GetIndex(x, y, z)].group;
    }

    public void SetPaletteGroupIndexAt(int x, int y, int z, byte value)
    {
        data[GetIndex(x, y, z)].group = value;
    }

    public byte GetPaletteIndexAt(int x, int y, int z)
    {
        return data[GetIndex(x, y, z)].index;
    }

    public void SetPaletteIndexAt(int x, int y, int z, byte value)
    {
        data[GetIndex(x, y, z)].index = value;
    }

    public byte GetRotationAt(int x, int y, int z)
    {
        return data[GetIndex(x, y, z)].rotation;
    }

    public void SetRotationAt(int x, int y, int z, byte value)
    {
        data[GetIndex(x, y, z)].rotation = value;
    }

    public bool GetFlipXAt(int x, int y, int z)
    {
        return data[GetIndex(x, y, z)].flipX;
    }

    public void SetFlipXAt(int x, int y, int z, bool value)
    {
        data[GetIndex(x, y, z)].flipX = value;
    }

    public bool GetFlipZAt(int x, int y, int z)
    {
        return data[GetIndex(x, y, z)].flipZ;
    }

    public void SetFlipZAt(int x, int y, int z, bool value)
    {
        data[GetIndex(x, y, z)].flipZ = value;
    }

    public void Resize(int width, int height, int depth)
    {
        Data[] newData = new Data[width * height * depth];
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
    }

    public override void Read(IReader r)
    {
        base.Read(r);
        width = r.Int();
        height = r.Int();
        depth = r.Int();
        data = new Data[width * height * depth];
        for (int i = 0; i < width * height * depth; i++) data[i].Read(r);
    }

    public override void Write(IWriter w)
    {
        base.Write(w);
        w.Int(width);
        w.Int(height);
        w.Int(depth);
        for (int i = 0; i < width * height * depth; i++) data[i].Write(w);
    }

    struct Data : ISerializable
    {
        public byte group;
        public byte index;
        public byte rotation;
        public bool flipX;
        public bool flipZ;

        public void Read(IReader r)
        {
            throw new NotImplementedException();
        }

        public void Write(IWriter w)
        {
            throw new NotImplementedException();
        }
    }
}

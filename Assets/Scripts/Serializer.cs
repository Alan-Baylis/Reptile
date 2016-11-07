using UnityEngine;
using System.Collections.Generic;
using System;

public interface IReader
{
    bool Bool();
    byte Byte();
    sbyte SByte();
    short Short();
    ushort UShort();
    int Int();
    uint UInt();
    long Long();
    ulong ULong();
    float Float();
    double Double();
    string String();
}

public interface IWriter
{
    void Bool(bool value);
    void Byte(byte value);
    void SByte(sbyte value);
    void Short(short value);
    void UShort(ushort value);
    void Int(int value);
    void UInt(uint value);
    void Long(long value);
    void ULong(ulong value);
    void Float(float value);
    void Double(double value);
    void String(string value);
}

public interface ISerializable
{
    void Read(IReader r);
    void Write(IWriter w);
}

public class StringWriter : IWriter
{
    System.Text.StringBuilder output;
    string seperator = "\n";

    public StringWriter()
    {
        output = new System.Text.StringBuilder();
    }

    public StringWriter(ISerializable data) : this()
    {
        data.Write(this);
    }

    public void Bool(bool value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public void Byte(byte value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public void Double(double value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public void Float(float value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public void Int(int value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public void Long(long value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public void SByte(sbyte value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public void Short(short value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public void String(string value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public void UInt(uint value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public void ULong(ulong value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public void UShort(ushort value)
    {
        output.Append(value);
        output.Append(seperator);
    }

    public string GetOutput()
    {
        return output.ToString();
    }
}

public class StringReader : IReader
{
    string[] lines;
    int i = 0;
    string seperator = "\n";

    public StringReader(string source)
    {
        lines = source.Split(new[] { seperator }, StringSplitOptions.None);
    }

    public bool Bool()
    {
        return bool.Parse(lines[i++]);
    }

    public byte Byte()
    {
        return byte.Parse(lines[i++]);
    }

    public sbyte SByte()
    {
        return sbyte.Parse(lines[i++]);
    }

    public short Short()
    {
        return short.Parse(lines[i++]);
    }

    public ushort UShort()
    {
        return ushort.Parse(lines[i++]);
    }

    public int Int()
    {
        return int.Parse(lines[i++]);
    }

    public uint UInt()
    {
        return uint.Parse(lines[i++]);
    }

    public long Long()
    {
        return long.Parse(lines[i++]);
    }

    public ulong ULong()
    {
        return ulong.Parse(lines[i++]);
    }

    public float Float()
    {
        return float.Parse(lines[i++]);
    }

    public double Double()
    {
        return double.Parse(lines[i++]);
    }

    public string String()
    {
        return lines[i++];
    }
}

public class BinaryWriter : IWriter
{
    System.Text.UTF8Encoding encoding;
    List<byte> output = new List<byte>();

    public BinaryWriter()
    {
        encoding = new System.Text.UTF8Encoding(false);
    }

    public BinaryWriter(ISerializable data) : this()
    {
        data.Write(this);
    }

    public void Bool(bool value)
    {
        output.AddRange(BitConverter.GetBytes(value));
    }

    public void Byte(byte value)
    {
        output.Add(value);
    }

    public void Double(double value)
    {
        output.AddRange(BitConverter.GetBytes(value));
    }

    public void Float(float value)
    {
        output.AddRange(BitConverter.GetBytes(value));
    }

    public void Int(int value)
    {
        output.AddRange(BitConverter.GetBytes(value));
    }

    public void Long(long value)
    {
        output.AddRange(BitConverter.GetBytes(value));
    }

    public void SByte(sbyte value)
    {
        output.Add((byte)value);
    }

    public void Short(short value)
    {
        output.AddRange(BitConverter.GetBytes(value));
    }

    public void String(string value)
    {
        output.AddRange(BitConverter.GetBytes(value.Length));
        output.AddRange(encoding.GetBytes(value));
    }

    public void UInt(uint value)
    {
        output.AddRange(BitConverter.GetBytes(value));
    }

    public void ULong(ulong value)
    {
        output.AddRange(BitConverter.GetBytes(value));
    }

    public void UShort(ushort value)
    {
        output.AddRange(BitConverter.GetBytes(value));
    }

    public byte[] GetOutput()
    {
        return output.ToArray();
    }
}

public class BinaryReader : IReader
{
    System.Text.UTF8Encoding encoding;
    byte[] data;
    int i = 0;

    public BinaryReader(byte[] data)
    {
        this.data = data;
        encoding = new System.Text.UTF8Encoding(false);
    }

    public bool Bool()
    {
        return BitConverter.ToBoolean(data, i++);
    }

    public byte Byte()
    {
        return data[i++];
    }

    public double Double()
    {
        double value = BitConverter.ToDouble(data, i);
        i += 8;
        return value;
    }

    public float Float()
    {
        float value = BitConverter.ToSingle(data, i);
        i += 4;
        return value;
    }

    public int Int()
    {
        int value = BitConverter.ToInt32(data, i);
        i += 4;
        return value;
    }

    public long Long()
    {
        long value = BitConverter.ToInt64(data, i);
        i += 8;
        return value;
    }

    public sbyte SByte()
    {
        return (sbyte)data[i++];
    }

    public short Short()
    {
        short value = BitConverter.ToInt16(data, i);
        i += 2;
        return value;
    }

    public string String()
    {
        int len = Int();
        string value = encoding.GetString(data, i, len);
        i += len;
        return value;
    }

    public uint UInt()
    {
        uint value = BitConverter.ToUInt32(data, i);
        i += 4;
        return value;
    }

    public ulong ULong()
    {
        ulong value = BitConverter.ToUInt64(data, i);
        i += 8;
        return value;
    }

    public ushort UShort()
    {
        ushort value = BitConverter.ToUInt16(data, i);
        i += 2;
        return value;
    }
}

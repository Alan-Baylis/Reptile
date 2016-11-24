using UnityEngine;
using System.Collections;
using System;

public class VColor : ISerializable
{
    public byte r; // Red
    public byte g; // Green
    public byte b; // Blue
    public byte a; // Alpha (Transparency)

    public byte m; // Metalness
    public byte s; // Smoothness
    public byte e; // Emission
    public byte u; // User Data

    public static bool operator ==(VColor a, VColor b)
    {
        if (ReferenceEquals(a, null))
        {
            return ReferenceEquals(b, null);
        }
        return a.Equals(b);
    }

    public static bool operator !=(VColor a, VColor b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return BitConverter.ToInt32(new[] { r, g, b, a }, 0) ^ BitConverter.ToInt32(new[] { m, s, e, u }, 0);
    }

    public override bool Equals(object obj)
    {
        VColor c = obj as VColor;
        if (c == null) return false;
        return c.r == r &&
               c.g == g &&
               c.b == b &&
               c.a == a &&
               c.m == m &&
               c.s == s &&
               c.e == e &&
               c.u == u;
    }

    public VColor(VColor o) : this(o.r, o.g, o.b, o.a, o.m, o.s, o.e, o.u)
    {

    }

    public VColor(byte r, byte g, byte b, byte a) : this(r, g, b, a, 0, 0, 0, 0)
    {

    }

    public VColor(byte r, byte g, byte b, byte a, byte m, byte s, byte e, byte u)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
        this.m = m;
        this.s = s;
        this.e = e;
        this.u = u;
    }

    public void Read(IReader r)
    {
        this.r = r.Byte();
        g = r.Byte();
        b = r.Byte();
        a = r.Byte();

        m = r.Byte();
        s = r.Byte();
        e = r.Byte();
        u = r.Byte();
    }

    public void Write(IWriter w)
    {
        w.Byte(r);
        w.Byte(g);
        w.Byte(b);
        w.Byte(a);

        w.Byte(m);
        w.Byte(s);
        w.Byte(e);
        w.Byte(u);
    }
}

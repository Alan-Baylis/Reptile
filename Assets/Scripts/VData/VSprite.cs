using UnityEngine;
using System.Collections.Generic;

public class VSprite
{
    public List<VLayer> layers = new List<VLayer>();
    public List<VAnimation> animations = new List<VAnimation>();
    public List<VSpriteChunk> chunks = new List<VSpriteChunk>();
    public VPalette palette = new VPalette();
}

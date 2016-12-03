using UnityEngine;
using System.Collections;
using System;

public class ChangeFrameInfoAct : Act
{
    int animationIndex;
    int frameIndex;
    float duration;
    float oldDuration;

    public ChangeFrameInfoAct(int animationIndex, int frameIndex, float duration)
    {
        this.animationIndex = animationIndex;
        this.frameIndex = frameIndex;
        this.duration = duration;
    }

    public override void Do()
    {
        VFrame frame = Edit.use.tile.GetAnimation(animationIndex).GetFrame(frameIndex);

        oldDuration = frame.GetDuration();

        frame.SetDuration(duration);
    }

    public override void Undo()
    {
        VFrame frame = Edit.use.tile.GetAnimation(animationIndex).GetFrame(frameIndex);

        frame.SetDuration(oldDuration);
    }

    public override bool IsNoOp()
    {
        VFrame frame = Edit.use.tile.GetAnimation(animationIndex).GetFrame(frameIndex);
        return duration == frame.GetDuration();
    }

    public override string ToString()
    {
        return "Change Frame Duration";
    }
}

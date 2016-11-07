using UnityEngine;
using System.Collections;
using System;

public class BackfaceCull : Cull
{
    public Vector3 normal = Vector3.forward;

    protected override bool ShouldCull()
    {
        Vector3 dir = transform.TransformDirection(normal);
        float dot = Vector3.Dot(dir, (transform.position - Camera.main.transform.position).normalized);
        return dot > 0f;
    }
}

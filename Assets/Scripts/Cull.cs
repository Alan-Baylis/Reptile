using UnityEngine;
using System.Collections;

public abstract class Cull : MonoBehaviour
{
    Renderer[] rends;
    Collider[] cols;

    protected virtual void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        cols = GetComponentsInChildren<Collider>();
    }

    protected virtual void LateUpdate()
    {
        bool cull = ShouldCull();

        if (rends != null)
        {
            foreach (Renderer r in rends)
            {
                r.enabled = !cull;
            }
        }

        if (cols != null)
        {
            foreach (Collider c in cols)
            {
                c.enabled = !cull;
            }
        }
    }

    protected abstract bool ShouldCull();
}

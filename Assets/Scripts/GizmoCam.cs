using UnityEngine;
using System.Collections;

public class GizmoCam : MonoBehaviour
{
    public static GizmoCam use;
    public static Camera cam;

    void Awake()
    {
        use = this;
        cam = GetComponent<Camera>();
    }
}

using UnityEngine;
using System.Collections;

public class AxisGizmo : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        Vector3 p;
        p.x = Screen.width - 60f;
        p.y = 60f;
        p.z = 5f;
        transform.position = GizmoCam.cam.ScreenToWorldPoint(p);
    }
}

using UnityEngine;
using System.Collections;

public class Sky : MonoBehaviour
{
    public float rotateSpeed;
    public Transform skyLight;

    void Update()
    {
        if (Edit.use.bindLightRotate.IsHeld())
        {
            Vector3 rot = skyLight.eulerAngles;
            rot.y -= Input.GetAxis("Mouse X") * rotateSpeed;
            rot.x -= Input.GetAxis("Mouse Y") * rotateSpeed;
            skyLight.eulerAngles = rot;
        }
    }
}

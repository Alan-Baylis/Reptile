using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour
{
    public float rotSpeed;
    public float panSpeed;
    public float zoomSpeed;
    Vector3 focus = Vector3.one * 3.5f;
    float dist = 16f;
    Vector3 angles = Vector3.zero;

    void Start()
    {

    }
    
    void Update()
    {
        if (Edit.use.bindCamRotate.IsHeld())
        {
            angles.y += Input.GetAxisRaw("Mouse X") * rotSpeed;
            angles.x += -Input.GetAxisRaw("Mouse Y") * rotSpeed;
        }

        if (Edit.use.bindCamPan.IsHeld())
        {
            focus += transform.right * -Input.GetAxisRaw("Mouse X") * panSpeed * dist;
            focus += transform.up * -Input.GetAxisRaw("Mouse Y") * panSpeed * dist;
        }

        if (Edit.use.bindCamZoom.IsHeld())
        {
            dist += -Input.GetAxisRaw("Mouse X") * zoomSpeed + -Input.GetAxisRaw("Mouse Y") * zoomSpeed;
            if (dist < 0f) dist = 0f;
        }

        if (Edit.use.bindCamFocus.IsPressed())
        {
            focus = new Vector3(Edit.use.tile.GetWidth(), Edit.use.tile.GetHeight(), Edit.use.tile.GetDepth()) * 0.5f - Vector3.one * 0.5f;
            dist = Mathf.Max(Edit.use.tile.GetWidth(), Edit.use.tile.GetHeight(), Edit.use.tile.GetDepth()) * 2f;
        }

        transform.position = focus;
        transform.eulerAngles = angles;
        transform.position -= transform.forward * dist;
    }
}

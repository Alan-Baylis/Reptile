using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour
{
    public static Cam use;

    public float rotSpeed;
    public float panSpeed;
    public float zoomSpeed;

    [HideInInspector]
    public Vector3 focus = Vector3.one * 3.5f;

    [HideInInspector]
    public float dist = 16f;

    [HideInInspector]
    public Vector3 angles = Vector3.zero;

    [HideInInspector]
    public Camera cam;

    void Awake()
    {
        use = this;
        cam = GetComponent<Camera>();
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

        if (Edit.use.bindCamZoomIn.IsHeld() || Edit.use.bindCamZoomOut.IsHeld())
        {
            dist += zoomSpeed * ((Edit.use.bindCamZoomIn.IsHeld()) ? 1f : -1f);
            if (dist < 0f) dist = 0f;
        }

        if (Edit.use.bindCamFocus.IsPressed())
        {
            focus = new Vector3(Edit.use.tile.GetWidth(), Edit.use.tile.GetHeight(), Edit.use.tile.GetDepth()) * 0.5f - Vector3.one * 0.5f;
            dist = Mathf.Max(Edit.use.tile.GetWidth(), Edit.use.tile.GetHeight(), Edit.use.tile.GetDepth()) * 2f;
        }

        RecalculateOrthoSize();

        transform.position = focus;
        if (Edit.use.camSnap) transform.eulerAngles = new Vector3(Snap(angles.x), Snap(angles.y), Snap(angles.z));
        else transform.eulerAngles = angles;

        if (cam.orthographic) transform.position -= transform.forward * 500f;
        transform.position -= transform.forward * dist;

        if (Edit.use.bindCamOrtho.IsPressed()) Edit.Do(new ChangeCamOrthoAct(!cam.orthographic));
    }

    public void Focus()
    {
        focus = new Vector3(Edit.use.tile.GetWidth(), Edit.use.tile.GetHeight(), Edit.use.tile.GetDepth()) * 0.5f - Vector3.one * 0.5f;
        dist = Mathf.Max(Edit.use.tile.GetWidth(), Edit.use.tile.GetHeight(), Edit.use.tile.GetDepth()) * 2f;
    }

    float Snap(float angle)
    {
        return Mathf.Round(angle / 15f) * 15f;
    }

    public void RecalculateOrthoSize()
    {
        bool ortho = cam.orthographic;
        cam.orthographic = false;
        Vector3 top = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, dist));
        Vector3 bottom = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, dist));
        float height = Vector3.Distance(top, bottom);
        cam.orthographicSize = height / 2f;
        cam.orthographic = ortho;
    }
}

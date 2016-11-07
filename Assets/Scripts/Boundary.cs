using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour
{
    public static Boundary use;

    public Material mat;
    Material xyMat;
    Material zyMat;
    Material xzMat;

    public Transform top;
    public Transform bottom;
    public Transform right;
    public Transform left;
    public Transform front;
    public Transform back;

    int width;
    int height;
    int depth;

    void Start()
    {
        xyMat = new Material(mat);
        zyMat = new Material(mat);
        xzMat = new Material(mat);
        top.GetComponent<MeshRenderer>().sharedMaterial = xzMat;
        bottom.GetComponent<MeshRenderer>().sharedMaterial = xzMat;
        right.GetComponent<MeshRenderer>().sharedMaterial = zyMat;
        left.GetComponent<MeshRenderer>().sharedMaterial = zyMat;
        front.GetComponent<MeshRenderer>().sharedMaterial = xyMat;
        back.GetComponent<MeshRenderer>().sharedMaterial = xyMat;
    }
    
    void Update()
    {
        if (width != Edit.use.tile.GetWidth() || height != Edit.use.tile.GetHeight() || depth != Edit.use.tile.GetDepth())
        {
            Resize(Edit.use.tile.GetWidth(), Edit.use.tile.GetHeight(), Edit.use.tile.GetDepth());
        }
    }

    void Resize(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;

        float w = width;
        float h = height;
        float d = depth;

        xyMat.mainTextureScale = new Vector2(w, h) / 4f;
        zyMat.mainTextureScale = new Vector2(d, h) / 4f;
        xzMat.mainTextureScale = new Vector2(w, d) / 4f;

        top.localPosition = new Vector3(w / 2f, h, d / 2f);
        top.localScale = new Vector3(w, d, 1f);
        bottom.localPosition = new Vector3(w / 2f, 0f, d / 2f);
        bottom.localScale = new Vector3(w, d, 1f);
        right.localPosition = new Vector3(w, h / 2f, d / 2f);
        right.localScale = new Vector3(d, h, 1f);
        left.localPosition = new Vector3(0f, h / 2f, d / 2f);
        left.localScale = new Vector3(d, h, 1f);
        front.localPosition = new Vector3(w / 2f, h / 2f, d);
        front.localScale = new Vector3(w, h, 1f);
        back.localPosition = new Vector3(w / 2f, h / 2f, 0f);
        back.localScale = new Vector3(w, h, 1f);
    }
}

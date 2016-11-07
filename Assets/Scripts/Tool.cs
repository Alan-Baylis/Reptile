using UnityEngine;
using System.Collections;

public class Tool : MonoBehaviour
{
    public Transform cursor;
    public LayerMask mask;

    public static Tool use;

    void Awake()
    {
        use = this;
    }

    int px;
    int py;
    int pz;

    int nx;
    int ny;
    int nz;

    Plane plane;

    void Update()
    {
        bool useToolPressed = Edit.use.bindUseTool.IsPressed();
        bool useToolHeld = Edit.use.bindUseTool.IsHeld();
        bool useTool = useToolPressed || useToolHeld;

        bool useToolAltPressed = Edit.use.bindUseToolAlt.IsPressed();
        bool useToolAltHeld = Edit.use.bindUseToolAlt.IsHeld();
        bool useToolAlt = useToolAltPressed || useToolAltHeld;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!useToolHeld && !useToolAltHeld)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, mask))
            {
                cursor.gameObject.SetActive(true);

                Vector3 p = hit.point;
                Vector3 n = hit.normal;

                n.x = Mathf.Round(n.x);
                n.y = Mathf.Round(n.y);
                n.z = Mathf.Round(n.z);

                nx = Mathf.RoundToInt(n.x);
                ny = Mathf.RoundToInt(n.y);
                nz = Mathf.RoundToInt(n.z);

                p -= n * 0.01f;

                p.x = Mathf.Round(p.x);
                p.y = Mathf.Round(p.y);
                p.z = Mathf.Round(p.z);

                px = Mathf.RoundToInt(p.x);
                py = Mathf.RoundToInt(p.y);
                pz = Mathf.RoundToInt(p.z);

                p += n * 0.5f;

                cursor.position = p;
                cursor.forward = n;

                plane = new Plane(n, p);
            }
            else
            {
                cursor.gameObject.SetActive(false);
            }
        }else
        {
            cursor.gameObject.SetActive(false);

            float dist;
            if (plane.Raycast(ray, out dist))
            {

                Vector3 p = ray.GetPoint(dist);
                Vector3 n = plane.normal;

                n.x = Mathf.Round(n.x);
                n.y = Mathf.Round(n.y);
                n.z = Mathf.Round(n.z);

                nx = Mathf.RoundToInt(n.x);
                ny = Mathf.RoundToInt(n.y);
                nz = Mathf.RoundToInt(n.z);

                p -= n * 0.01f;

                p.x = Mathf.Round(p.x);
                p.y = Mathf.Round(p.y);
                p.z = Mathf.Round(p.z);

                px = Mathf.RoundToInt(p.x);
                py = Mathf.RoundToInt(p.y);
                pz = Mathf.RoundToInt(p.z);

                p += n * 0.5f;

                cursor.position = p;
                cursor.forward = n;
            }
        }
        if (useTool || useToolAlt)
        {
            if (Edit.use.tool == Edit.Tool.Place && useTool && !IsOutOfBounds(px + nx, py + ny, pz + nz))
                Edit.Do(new UsePlaceToolAct(px + nx, py + ny, pz + nz, (byte)Edit.use.tile.GetPalette().GetIndex()));
            if (Edit.use.tool == Edit.Tool.Place && useToolAlt && !IsOutOfBounds(px, py, pz))
                Edit.Do(new UsePlaceToolAct(px, py, pz, 0));
            if (Edit.use.tool == Edit.Tool.Paint && !IsOutOfBounds(px, py, pz))
                Edit.Do(new UsePaintToolAct(px, py, pz, (byte)Edit.use.tile.GetPalette().GetIndex()));
        }
    }

    bool IsOutOfBounds(int x, int y, int z)
    {
        if (x < 0 || x >= Edit.use.tile.GetWidth()) return true;
        if (y < 0 || y >= Edit.use.tile.GetHeight()) return true;
        if (z < 0 || z >= Edit.use.tile.GetDepth()) return true;
        return false;
    }

    bool IsMouseMoving()
    {
        return Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.05f;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class Edit : MonoBehaviour
{
    public static Edit use;
    public VTile tile;
    public Tool tool;
    public Brush brush;
    public int brushSize = 1;
    public bool mirrorX;
    public bool mirrorY;
    public bool mirrorZ;
    public bool fillDiagonals;

    public VPalette refPalette;

    public static Stack<Act> undos = new Stack<Act>();
    public static Stack<Act> redos = new Stack<Act>();

    public Binding bindUndo;
    public Binding bindRedo;

    public Binding bindCamRotate;
    public Binding bindCamPan;
    public Binding bindCamZoom;
    public Binding bindCamZoomIn;
    public Binding bindCamZoomOut;
    public Binding bindCamFocus;

    public Binding bindLightRotate;

    public Binding bindToolPlace;
    public Binding bindToolPaint;
    public Binding bindToolFill;
    public Binding bindToolMove;
    public Binding bindToolBox;
    public Binding bindToolWand;

    public Binding bindUseTool;
    public Binding bindUseToolAlt;

    public Binding bindPlaneLock;
    
    public static int width;
    public static int height;
    public static int depth;

    static Stack<Act> batch = new Stack<Act>();
    static float batchTime = 0f;

    public enum Tool
    {
        Place,
        Paint,
        Fill,
        Move,
        Box,
        Wand
    }

    public enum Brush
    {
        Cube,
        Sphere,
        Diamond
    }

    void Awake()
    {
        use = this;
        tile = new VTile();
        refPalette = new VPalette();

        width = tile.GetWidth();
        height = tile.GetHeight();
        depth = tile.GetDepth();
    }

    void Update()
    {
        tile.SetDirty(false);
        if (bindUndo.IsPressed() && undos.Count > 0) Do(new UndoAct());
        if (bindRedo.IsPressed() && redos.Count > 0) Do(new RedoAct());
        if (bindToolPlace.IsPressed()) Do(new ChangeToolAct(Tool.Place));
        if (bindToolPaint.IsPressed()) Do(new ChangeToolAct(Tool.Paint));
        if (bindToolFill.IsPressed()) Do(new ChangeToolAct(Tool.Fill));
        if (bindToolMove.IsPressed()) Do(new ChangeToolAct(Tool.Move));
        if (bindToolBox.IsPressed()) Do(new ChangeToolAct(Tool.Box));
        if (bindToolWand.IsPressed()) Do(new ChangeToolAct(Tool.Wand));
        if (batch.Count > 0 && Time.time > batchTime)
        {
            DoBatch();
        }
    }

    public static void Do(Act act)
    {
        if (act.IsNoOp()) return;
        if (act.CanBatch() && (batch.Count == 0 || batch.Peek().GetType() == act.GetType()))
        {
            batch.Push(act);
            act.Do();
            batchTime = Time.time + .5f;
        }else
        {
            if (batch.Count > 0) DoBatch();

            act.Do();
            if (act.CanUndo())
            {
                undos.Push(act);
                redos.Clear();
            }
        }

        width = use.tile.GetWidth();
        height = use.tile.GetHeight();
        depth = use.tile.GetDepth();
    }

    static void DoBatch()
    {
        List<Act> acts = new List<Act>();
        while (batch.Count > 0)
        {
            Act act = batch.Pop();
            act.Undo();
            acts.Insert(0, act);
        }
        Do(new BatchAct(acts.ToArray(), acts[0].ToString()));
    }

    public static string GetDirectory()
    {
        if (!PlayerPrefs.HasKey("LastPath"))
            PlayerPrefs.SetString("LastPath", System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\");
        return PlayerPrefs.GetString("LastPath");
    }
}

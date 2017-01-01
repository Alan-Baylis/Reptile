using UnityEngine;
using System.Collections.Generic;

public class UI : MonoBehaviour
{
    public static UI use;

    public string version;

    public bool debug;

    public GUISkin skin;
    public Texture2D alphaTex;
    public Texture2D blankTex;
    public Texture2D svCursorTex;
    
    Vector2 palScroll;
    Vector2 refPalScroll;

    Edit ed;

    Queue<Act> actQueue = new Queue<Act>();

    bool showTool = true;
    bool showPalette = true;
    bool showRefPalette = false;
    bool showColorPicker = true;
    bool showFrames = true;
    bool showCamera = true;
    bool showTile = true;
    bool showLayers = true;
    bool showAnimations = true;
    bool showAdvancedColor = false;
    bool showKeyBindings = false;

    bool playAnimation;
    float animationTime;
    bool loopAnimation = true;

    Texture2D hTex;
    Texture2D svTex;
    Texture2D rTex;
    Texture2D gTex;
    Texture2D bTex;
    Texture2D bwTex;

    float oldH;
    float oldS;
    float oldV;
    
    List<Rect> boxRects = new List<Rect>();
    public bool repaint;

    List<string> errMsgs = new List<string>();

    void Awake()
    {
        use = this;
        Application.logMessageReceived += (string cond, string stack, LogType type) =>
        {
            string msg = "";
            msg += cond + "\n";
            msg += "<size=12>" + stack + "</size>";
            if (type == LogType.Log) msg = "<color=cyan>" + msg + "</color>";
            else if (type == LogType.Warning) msg = "<color=orange>" + msg + "</color>";
            else msg = "<color=red>" + msg + "</color>";
            errMsgs.Add(msg);
        };
        Profiler.maxNumberOfSamplesPerFrame = -1;
    }

    void Start()
    {
        ed = Edit.use;

        hTex = new Texture2D(1, 200);
        svTex = new Texture2D(200, 200);
        rTex = new Texture2D(256, 1);
        gTex = new Texture2D(256, 1);
        bTex = new Texture2D(256, 1);

        Color32[] pixels = new Color32[200];
        for (int i = 0; i < 200; i++) pixels[199 - i] = Color.HSVToRGB(i / 199f, 1f, 1f);
        hTex.SetPixels32(pixels);
        hTex.Apply();

        bwTex = new Texture2D(256, 1);
        pixels = new Color32[256];
        for (int i = 0; i < 256; i++) pixels[i] = Color.HSVToRGB(0f, 0f, i / 255f);
        bwTex.SetPixels32(pixels);
        bwTex.Apply();
    }

    void Update()
    {
        VAnimation anim = ed.tile.GetAnimation(ed.tile.GetAnimationIndex());
        if (playAnimation)
        {
            animationTime += Time.deltaTime;
            if (loopAnimation)
            {
                animationTime %= anim.GetDuration();
            } else if (animationTime >= anim.GetDuration())
            {
                playAnimation = false;
            }
        }
        if (playAnimation) {
            float frameTime = 0f;
            for (int i = 0; i < anim.GetFrameCount(); i++)
            {
                if (i != ed.tile.GetFrameIndex() && animationTime >= frameTime && animationTime < frameTime + anim.GetFrame(i).GetDuration())
                {
                    actQueue.Enqueue(new ChangeFrameIndexAct(i));
                }
                frameTime += anim.GetFrame(i).GetDuration();
            }
        }
        while (actQueue.Count > 0) Edit.Do(actQueue.Dequeue());
    }

    public static bool IsMouseOver()
    {
        Vector2 mp = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        foreach (Rect rect in use.boxRects)
        {
            if (rect.Contains(mp)) return true;
        }
        return false;
    }

    void OnGUI()
    {
        repaint = Event.current.type == EventType.Repaint;
        if (repaint) boxRects.Clear();

        GUI.skin = skin;

        GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
        GUILayout.BeginVertical();
        Header();
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        Palette();
        RefPalette();
        ColorPicker();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        Tool();
        Console();
        GUILayout.FlexibleSpace();
        KeyBindings();
        GUILayout.FlexibleSpace();
        Camera();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        Frames();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        Tile();
        Layers();
        Animations();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndArea();

        if (debug)
        {
            GUI.color = new Color(1f, 0f, 0f, 0.2f);
            foreach (Rect rect in boxRects)
            {
                GUI.Box(rect, "");
            }
            GUI.color = Color.white;
        }
    }

    void Header()
    {
        GUILayout.BeginHorizontal("box");

        GUI.enabled = Edit.undos.Count > 0;
        if (GUILayout.Button(Edit.undos.Count > 0 ? "Undo " + Edit.undos.Peek() : "Undo")) actQueue.Enqueue(new UndoAct());
        GUI.enabled = Edit.redos.Count > 0;
        if (GUILayout.Button(Edit.redos.Count > 0 ? "Redo " + Edit.redos.Peek() : "Redo")) actQueue.Enqueue(new RedoAct());
        GUI.enabled = true;

        showKeyBindings = GUILayout.Toggle(showKeyBindings, "Key Bindings", "button");

        GUILayout.FlexibleSpace();

        GUILayout.Label("Version " + version);

        GUILayout.EndHorizontal();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());
    }

    void Tool()
    {
        GUILayout.BeginVertical("box");
        showTool = GUILayout.Toggle(showTool, "Tool", "boxhead");
        if (showTool)
        {
            Edit.Tool tool = (Edit.Tool)GUILayout.SelectionGrid((int)ed.tool, new[] { "Place", "Paint", "Fill", "Box" }, 1, "tool");
            if (tool != Edit.use.tool) actQueue.Enqueue(new ChangeToolAct(tool));
            GUILayout.Label("Brush Type");
            Edit.Brush brush = (Edit.Brush)GUILayout.SelectionGrid((int)ed.brush, new[] { "Cube", "Sphere", "Diamond" }, 1);
            if (brush != Edit.use.brush) actQueue.Enqueue(new ChangeBrushAct(brush));
            GUILayout.Label("Brush Size");
            GUILayout.BeginHorizontal();
            GUI.enabled = ed.brushSize > 1;
            if (GUILayout.Button("-")) actQueue.Enqueue(new ChangeBrushSizeAct(ed.brushSize - 1));
            GUI.enabled = true;
            int brushSize;
            if (int.TryParse(GUILayout.TextField(ed.brushSize.ToString()), out brushSize))
            {
                if (brushSize != ed.brushSize) actQueue.Enqueue(new ChangeBrushSizeAct(brushSize));
            }
            if (GUILayout.Button("+")) actQueue.Enqueue(new ChangeBrushSizeAct(ed.brushSize + 1));
            GUILayout.EndHorizontal();
            GUILayout.Label("Symmetry");
            GUILayout.BeginHorizontal();
            GUI.color = Color.Lerp(Color.red, Color.white, 0.5f);
            bool mirrorX = GUILayout.Toggle(Edit.use.mirrorX, "X", "button");
            GUI.color = Color.Lerp(Color.green, Color.white, 0.5f);
            bool mirrorY = GUILayout.Toggle(Edit.use.mirrorY, "Y", "button");
            GUI.color = Color.Lerp(Color.blue, Color.white, 0.5f);
            bool mirrorZ = GUILayout.Toggle(Edit.use.mirrorZ, "Z", "button");
            GUI.color = Color.white;
            if (mirrorX != Edit.use.mirrorX || mirrorY != Edit.use.mirrorY || mirrorZ != Edit.use.mirrorZ)
                actQueue.Enqueue(new ChangeSymmetryAct(mirrorX, mirrorY, mirrorZ));
            GUILayout.EndHorizontal();
            GUILayout.Label("Tool Options");
            if (Edit.use.bindPlaneLock.IsHeld())
            {
                bool planeLock = !GUILayout.Toggle(!Edit.use.planeLock, "Plane Lock", "button");
                if (planeLock != Edit.use.planeLock) actQueue.Enqueue(new ChangePlaneLockAct(planeLock));
            }
            else
            {
                bool planeLock = GUILayout.Toggle(Edit.use.planeLock, "Plane Lock", "button");
                if (planeLock != Edit.use.planeLock) actQueue.Enqueue(new ChangePlaneLockAct(planeLock));
            }
            if (tool == Edit.Tool.Fill)
            {
                bool fillDiagonals = GUILayout.Toggle(Edit.use.fillDiagonals, "Diagonals", "button");
                if (fillDiagonals != Edit.use.fillDiagonals) actQueue.Enqueue(new ChangeFillDiagonalsAct(fillDiagonals));
            }
        }
        GUILayout.EndVertical();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());
    }

    void Console()
    {
        GUILayout.BeginVertical();
        if (errMsgs.Count > 0)
        {
            GUILayout.Label("<size=32><color=red>Something broke! Copy the error messages below and send them to the developer, describing the last three steps you performed.</color></size>");
        }
        for (int i = 0; i < Mathf.Min(3, errMsgs.Count); i ++)
        {
            GUILayout.TextField(errMsgs[i]);
        }
        GUILayout.EndVertical();
    }

    void Camera()
    {
        GUILayout.BeginVertical("box");
        showCamera = GUILayout.Toggle(showCamera, "Camera", "boxhead");
        if (showCamera)
        {
            bool camOrtho = !GUILayout.Toggle(!Cam.use.cam.orthographic, "Perspective", "button");
            if (camOrtho != Cam.use.cam.orthographic) actQueue.Enqueue(new ChangeCamOrthoAct(camOrtho));
            camOrtho = GUILayout.Toggle(Cam.use.cam.orthographic, "Orthographic", "button");
            if (camOrtho != Cam.use.cam.orthographic) actQueue.Enqueue(new ChangeCamOrthoAct(camOrtho));
            GUILayout.Label("Options");
            bool camSnap = GUILayout.Toggle(Edit.use.camSnap, "Angle Snap", "button");
            if (camSnap != Edit.use.camSnap) actQueue.Enqueue(new ChangeCamSnapAct(camSnap));
            GUILayout.Label("Bookmarks");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+X")) SetCamBookmark(new Vector3(0f, 90f, 0f));
            if (GUILayout.Button("+Y")) SetCamBookmark(new Vector3(270f, 0f, 0f));
            if (GUILayout.Button("+Z")) SetCamBookmark(new Vector3(0f, 0f, 0f));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("-X")) SetCamBookmark(new Vector3(0f, 270f, 0f));
            if (GUILayout.Button("-Y")) SetCamBookmark(new Vector3(90, 0f, 0f));
            if (GUILayout.Button("-Z")) SetCamBookmark(new Vector3(0f, 180f, 0f));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Isometric")) SetCamBookmark(new Vector3(35.2643897f, 45f, 0f));
            if (GUILayout.Button("Top-Down")) SetCamBookmark(new Vector3(45f, 0f, 0f));
        }
        GUILayout.EndVertical();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());
    }

    void SetCamBookmark(Vector3 angles)
    {
        Cam.use.angles = angles;
        Cam.use.Focus();
    }
    
    void Frames()
    {
        int index = ed.tile.GetFrameIndex();
        int animIndex = ed.tile.GetAnimationIndex();
        if (animIndex >= ed.tile.GetAnimationCount()) return;
        VAnimation anim = ed.tile.GetAnimation(animIndex);
        int count = anim.GetFrameCount();

        GUILayout.BeginVertical("box");
        showFrames = GUILayout.Toggle(showFrames, "Timeline", "boxhead");
        if (showFrames)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Frames");
            if (count > 1 && index < count && GUILayout.Button("-", GUILayout.Width(20))) actQueue.Enqueue(new RemoveFrameAct(animIndex, index));
            for (int i = 0; i < count; i++)
            {
                if (GUILayout.Toggle(i == index, "" + i, "button") && i != index) actQueue.Enqueue(new ChangeFrameIndexAct(i));
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(20))) actQueue.Enqueue(new AddFrameAct(animIndex));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Frame Duration");
            float duration;
            if (float.TryParse(GUILayout.TextField(anim.GetFrame(index).GetDuration().ToString(), GUILayout.Width(50)), out duration))
            {
                if (duration != anim.GetFrame(index).GetDuration()) actQueue.Enqueue(new ChangeFrameInfoAct(animIndex, index, duration));
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label("Playback");
            bool play = GUILayout.Toggle(playAnimation, "Play", "button");
            if (play != playAnimation)
            {
                animationTime = 0f;
                playAnimation = play;
            }
            loopAnimation = GUILayout.Toggle(loopAnimation, "Loop", "button");
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());
    }

    void Tile()
    {
        GUILayout.BeginVertical("box");
        showTile = GUILayout.Toggle(showTile, "Tile", "boxhead");
        if (showTile)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load"))
            {
                string path = TinyFileDialogs.OpenFileDialog("Load Tile", Edit.GetDirectory(), new[] { "*.rtile" }, "Reptile Tiles (*.rtile)", false);
                if (!string.IsNullOrEmpty(path))
                {
                    actQueue.Enqueue(new LoadTileAct(System.IO.File.ReadAllBytes(path)));
                }
            }
            if (GUILayout.Button("Save"))
            {
                string path = TinyFileDialogs.SaveFileDialog("Save Tile", Edit.GetDirectory(), new[] { "*.rtile" }, "Reptile Tiles (*.rtile)");
                if (!string.IsNullOrEmpty(path))
                {
                    if (!path.Contains(".")) path += ".rtile";
                    System.IO.File.WriteAllBytes(path, new BinaryWriter(Edit.use.tile).GetOutput());
                }
            }
            if (GUILayout.Button("New"))
            {
                actQueue.Enqueue(new NewTileAct());
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Width");
            int w;
            if (int.TryParse(GUILayout.TextField(Edit.width.ToString()), out w))
            {
                if (w != ed.tile.GetWidth()) Edit.width = w;
            };
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("Height");
            int h;
            if (int.TryParse(GUILayout.TextField(Edit.height.ToString()), out h))
            {
                if (h != ed.tile.GetHeight()) Edit.height = h;
            };
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("Depth");
            int d;
            if (int.TryParse(GUILayout.TextField(Edit.depth.ToString()), out d))
            {
                if (d != ed.tile.GetDepth()) Edit.depth = d;
            };
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUI.enabled = Edit.width != ed.tile.GetWidth() || Edit.height != ed.tile.GetHeight() || Edit.depth != ed.tile.GetDepth();
            if (GUILayout.Button("Apply"))
            {
                actQueue.Enqueue(new ResizeTileAct(Edit.width, Edit.height, Edit.depth));
            }
            GUI.enabled = true;

            GUILayout.Label("Presets");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("8³")) actQueue.Enqueue(new ResizeTileAct(8, 8, 8));
            if (GUILayout.Button("16³")) actQueue.Enqueue(new ResizeTileAct(16, 16, 16));
            if (GUILayout.Button("32³")) actQueue.Enqueue(new ResizeTileAct(32, 32, 32));
            GUILayout.EndHorizontal();
            GUILayout.Label("Import");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VOX"))
            {
                string path = TinyFileDialogs.OpenFileDialog("Import VOX", Edit.GetDirectory(), new[] { "*.vox" }, "MagicaVoxel Model (*.vox)", false);
                if (!string.IsNullOrEmpty(path))
                {
                    actQueue.Enqueue(new LoadTileAct(new BinaryWriter(ExportUtil.VoxToTile(System.IO.File.ReadAllBytes(path))).GetOutput()));
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("Export");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OBJ"))
            {
                string path = TinyFileDialogs.SaveFileDialog("Export OBJ", Edit.GetDirectory(), new[] { "*.obj" }, "Wavefront OBJ (*.obj)");
                if (!string.IsNullOrEmpty(path))
                {
                    if (!path.Contains(".")) path += ".obj";
                    System.IO.File.WriteAllText(path, ExportUtil.TileToObj(Edit.use.tile));
                    System.IO.File.WriteAllBytes(path.Substring(0, path.LastIndexOf('.')) + ".png", ExportUtil.PaletteToPng(Edit.use.tile.GetPalette()));
                }
            }
            if (GUILayout.Button("VOX"))
            {
                string path = TinyFileDialogs.SaveFileDialog("Export VOX", Edit.GetDirectory(), new[] { "*.vox" }, "MagicaVoxel Model (*.vox)");
                if (!string.IsNullOrEmpty(path))
                {
                    if (!path.Contains(".")) path += ".vox";
                    System.IO.File.WriteAllBytes(path, ExportUtil.TileToVox(Edit.use.tile));
                }
            }
            if (GUILayout.Button("PNG"))
            {
                string path = TinyFileDialogs.SaveFileDialog("Export PNG", Edit.GetDirectory(), new[] { "*.png" }, "Portable Network Graphics (*.png)");
                if (!string.IsNullOrEmpty(path))
                {
                    if (!path.Contains(".")) path += ".png";
                    System.IO.File.WriteAllBytes(path, ExportUtil.PaletteToPng(Edit.use.tile.GetPalette()));
                }

            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());
    }

    void Layers()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(200));
        showLayers = GUILayout.Toggle(showLayers, "Layers", "boxhead");
        int layerIndex = ed.tile.GetLayerIndex();
        if (showLayers)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(20))) actQueue.Enqueue(new AddLayerAct("Layer " + ed.tile.GetLayerCount()));
            GUILayout.EndHorizontal();
            for (int i = ed.tile.GetLayerCount() - 1; i >= 0; i--)
            {
                VLayer layer = ed.tile.GetLayer(i);
                GUILayout.BeginHorizontal();
                layerIndex = GUILayout.Toggle(layerIndex == i, layerIndex == i ? "" : "", "button", GUILayout.Width(20)) ? i : layerIndex;
                string name = GUILayout.TextField(layer.GetName(), GUILayout.Width(60));
                bool vis = GUILayout.Toggle(layer.GetVisible(), "V", "button", GUILayout.Width(20));
                bool trans = GUILayout.Toggle(layer.GetTransparent(), "T", "button", GUILayout.Width(20));
                bool line = GUILayout.Toggle(layer.GetOutline(), "O", "button", GUILayout.Width(20));
                GUILayout.FlexibleSpace();
                if (ed.tile.GetLayerCount() > 1)
                {
                    if (GUILayout.Button("-", GUILayout.Width(20))) actQueue.Enqueue(new RemoveLayerAct(i));
                }
                else
                {
                    GUILayout.Space(25f);
                }
                GUILayout.EndHorizontal();

                if (name != layer.GetName() || vis != layer.GetVisible() || trans != layer.GetTransparent() || line != layer.GetOutline())
                {
                    actQueue.Enqueue(new ChangeLayerInfoAct(i, name, vis, trans, line));
                }
            }
        }
        GUILayout.EndVertical();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());
        
        if (layerIndex != ed.tile.GetLayerIndex()) actQueue.Enqueue(new ChangeLayerIndexAct(layerIndex));
    }
    
    void Animations()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(200));
        showAnimations = GUILayout.Toggle(showAnimations, "Animations", "boxhead");
        int animIndex = ed.tile.GetAnimationIndex();
        if (showAnimations)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(20))) actQueue.Enqueue(new AddAnimationAct("Animation " + ed.tile.GetAnimationCount()));
            GUILayout.EndHorizontal();
            for (int i = ed.tile.GetAnimationCount() - 1; i >= 0; i--)
            {
                VAnimation anim = ed.tile.GetAnimation(i);
                GUILayout.BeginHorizontal();
                animIndex = GUILayout.Toggle(animIndex == i, animIndex == i ? "" : "", "button", GUILayout.Width(20)) ? i : animIndex;
                string name = GUILayout.TextField(anim.GetName());
                if (ed.tile.GetAnimationCount() > 1)
                {
                    if (GUILayout.Button("-", GUILayout.Width(20))) actQueue.Enqueue(new RemoveAnimationAct(i));
                }
                else
                {
                    GUILayout.Space(30f);
                }
                GUILayout.EndHorizontal();

                if (name != anim.GetName())
                {
                    actQueue.Enqueue(new ChangeAnimationInfoAct(i, name));
                }
            }
        }
        GUILayout.EndVertical();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());

        if (animIndex != ed.tile.GetAnimationIndex()) actQueue.Enqueue(new ChangeAnimationIndexAct(animIndex));
    }

    void RefPalette()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(145));
        showRefPalette = GUILayout.Toggle(showRefPalette, "Source Palette", "boxhead");
        int index = -1;
        if (showRefPalette)
        {
            refPalScroll = GUILayout.BeginScrollView(refPalScroll);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load"))
            {
                string path = TinyFileDialogs.OpenFileDialog("Load Source Palette", Edit.GetDirectory(), new[] { "*.rpal" }, "Reptile Palettes (*.rpal)", false);
                if (!string.IsNullOrEmpty(path))
                {
                    actQueue.Enqueue(new LoadRefPaletteAct(System.IO.File.ReadAllBytes(path)));
                }
            }
            GUILayout.EndHorizontal();
            for (int i = 0; i < Edit.use.refPalette.GetCount(); i++)
            {
                if (i % 4 == 0)
                {
                    if (i > 0) GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                VColor c = Edit.use.refPalette.GetColor(i);
                index = Swatch(i == index, new Color32(c.r, c.g, c.b, c.a)) ? i : index;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());
        
        if (index >= 0) actQueue.Enqueue(new ChangePaletteColorAct(ed.refPalette.GetColor(index), ed.tile.GetPalette().GetIndex()));
    }

    void Palette()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(145));
        showPalette = GUILayout.Toggle(showPalette, "Palette", "boxhead");
        int index = ed.tile.GetPalette().GetIndex();
        VPalette palette = Edit.use.tile.GetPalette();
        if (showPalette)
        {
            palScroll = GUILayout.BeginScrollView(palScroll);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load"))
            {
                string path = TinyFileDialogs.OpenFileDialog("Load Palette", Edit.GetDirectory(), new[] { "*.rpal" }, "Reptile Palettes (*.rpal)", false);
                if (!string.IsNullOrEmpty(path))
                {
                    actQueue.Enqueue(new LoadPaletteAct(System.IO.File.ReadAllBytes(path)));
                }
            }
            if (GUILayout.Button("Save"))
            {
                string path = TinyFileDialogs.SaveFileDialog("Save Palette", Edit.GetDirectory(), new[] { "*.rpal" }, "Reptile Palettes (*.rpal)");
                if (!string.IsNullOrEmpty(path))
                {
                    if (!path.Contains(".")) path += ".rpal";
                    System.IO.File.WriteAllBytes(path, new BinaryWriter(palette).GetOutput());
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (palette.GetCount() > 1 && GUILayout.Button("-", GUILayout.Width(25)))
            {
                actQueue.Enqueue(new RemovePaletteColorAct());
            }
            GUILayout.FlexibleSpace();
            if (palette.GetCount() < 256 && GUILayout.Button("+", GUILayout.Width(25)))
            {
                actQueue.Enqueue(new AddPaletteColorAct(new VColor(255, 255, 255, 255)));
            }
            GUILayout.EndHorizontal();
            for (int i = 0; i < palette.GetCount(); i++)
            {
                if (i % 4 == 0)
                {
                    if (i > 0) GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                VColor c = palette.GetColor(i);
                index = Swatch(i == index, new Color32(c.r, c.g, c.b, c.a)) ? i : index;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());
        
        if (index != ed.tile.GetPalette().GetIndex()) actQueue.Enqueue(new ChangePaletteIndexAct(index));
    }

    bool Swatch(bool active, Color color)
    {
        Rect r = GUILayoutUtility.GetRect(30f, 30f, "swatch");
        GUI.DrawTexture(r, alphaTex, ScaleMode.ScaleAndCrop);
        GUI.color = color;
        GUI.DrawTexture(r, blankTex, ScaleMode.ScaleToFit);
        GUI.color = Color.white;
        return GUI.Toggle(r, active, "", "swatch");
    }

    bool BigSwatch(bool active, Color color)
    {
        Rect r = GUILayoutUtility.GetRect(50f, 50f, "bigswatch");
        GUI.DrawTexture(r, alphaTex, ScaleMode.ScaleAndCrop);
        GUI.color = color;
        GUI.DrawTexture(r, blankTex, ScaleMode.ScaleToFit);
        GUI.color = Color.white;
        return GUI.Toggle(r, active, "", "bigswatch");
    }

    string[] mbNames = new string[] { "LMB", "RMB", "MMB" };

    void KeyBindings()
    {
        if (!showKeyBindings) return;

        Binding[] bindings = new[] {
            Edit.use.bindUndo,
            Edit.use.bindRedo,
            Edit.use.bindCamRotate,
            Edit.use.bindCamPan,
            Edit.use.bindCamZoom,
            Edit.use.bindCamZoomIn,
            Edit.use.bindCamZoomOut,
            Edit.use.bindCamFocus,
            Edit.use.bindCamOrtho,
            Edit.use.bindLightRotate,
            Edit.use.bindToolPlace,
            Edit.use.bindToolPaint,
            Edit.use.bindToolFill,
            Edit.use.bindToolBox,
            Edit.use.bindUseTool,
            Edit.use.bindUseToolAlt,
            Edit.use.bindPlaneLock
        };
        
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        foreach (Binding bind in bindings)
        {
            GUILayout.Label(bind.name);
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        foreach (Binding bind in bindings)
        {
            List<string> bits = new List<string>();
            if (bind.ctrl) bits.Add("Ctrl");
            if (bind.shift) bits.Add("Shift");
            if (bind.alt) bits.Add("Alt");
            if (bind.btn >= 0) bits.Add(mbNames[bind.btn]);
            if (bind.key != KeyCode.None) bits.Add(bind.key.ToString());
            if (bind.axis != "") bits.Add(((bind.negateAxis) ? "-" : "+") + bind.axis);
            GUILayout.Label(string.Join(" + ", bits.ToArray()));
        }
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());
    }
    
    bool svDrag;

    void ColorPicker()
    {
        if (ed.tile.GetPalette().GetIndex() >= ed.tile.GetPalette().GetCount()) return;
        VColor color = ed.tile.GetPalette().GetColor(ed.tile.GetPalette().GetIndex());

        color = new VColor(color);

        GUILayout.BeginVertical("box", GUILayout.Width(145));

        showColorPicker = GUILayout.Toggle(showColorPicker, "Color", "boxhead");

        if (showColorPicker)
        {
            GUILayout.Space(5f);
            bool isMouseDown = Event.current.type == EventType.MouseDown;
            if (Event.current.type == EventType.MouseUp) svDrag = false;
            bool drag = svDrag && Event.current.type == EventType.MouseDrag;
            Vector2 mp = Event.current.mousePosition;

            Color32 c = new Color32(color.r, color.g, color.b, color.a);
            float hue, sat, val;
            Color.RGBToHSV(c, out hue, out sat, out val);

            if (hue != oldH || sat != oldS || val != oldV)
            {
                int width = svTex.width;
                int height = svTex.height;
                Color32[] pixels = new Color32[width * height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        pixels[x + y * width] = Color.HSVToRGB(hue, x / (width - 1f), y / (height - 1f));
                    }
                }
                svTex.SetPixels32(pixels);
                svTex.Apply();

                for (int i = 0; i < 256; i++)
                {
                    rTex.SetPixel(i, 0, new Color32((byte)i, c.g, c.b, 255));
                    gTex.SetPixel(i, 0, new Color32(c.r, (byte)i, c.b, 255));
                    bTex.SetPixel(i, 0, new Color32(c.r, c.g, (byte)i, 255));
                }
                rTex.Apply();
                gTex.Apply();
                bTex.Apply();

                oldH = hue;
                oldS = sat;
                oldV = val;
            }

            GUILayout.BeginHorizontal();

            Rect rect = GUILayoutUtility.GetRect(svTex.width / 2f, svTex.height / 2f);
            rect.width = Mathf.Min(rect.width, svTex.width / 2f);
            rect.height = Mathf.Min(rect.height, svTex.height / 2f);
            GUI.DrawTexture(rect, svTex);
            if ((isMouseDown || drag) && rect.Contains(mp))
            {
                svDrag = true;
                if (isMouseDown) Event.current.Use();
                mp.x = Mathf.Clamp(mp.x, rect.xMin, rect.xMax);
                mp.y = Mathf.Clamp(mp.y, rect.yMin, rect.yMax);
                int x = Mathf.RoundToInt(mp.x - rect.x) * 2;
                int y = svTex.height - Mathf.RoundToInt(mp.y - rect.y) * 2;
                Color32 tc = svTex.GetPixel(x, y);
                color.r = tc.r;
                color.g = tc.g;
                color.b = tc.b;
            }

            GUI.color = new Color32((byte)(255 - color.r), (byte)(255 - color.g), (byte)(255 - color.b), 255);
            GUI.DrawTexture(new Rect(rect.x + rect.width * sat - 4f, rect.yMax - rect.height * val - 4f, 8f, 8f), svCursorTex);
            GUI.color = Color.white;

            GUILayout.FlexibleSpace();

            float newHue = GUILayout.VerticalSlider(hue, 0f, 1f, GUILayout.Height(svTex.height / 2f));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), hTex);
            if (newHue != hue)
            {
                if (sat == 0) sat = 7f / 255f;
                Color32 tc = Color.HSVToRGB(newHue, sat, val);
                color.r = tc.r;
                color.g = tc.g;
                color.b = tc.b;
            }

            GUILayout.EndHorizontal();

            int fieldWidth = 35;

            GUILayout.BeginHorizontal();
            color.r = (byte)GUILayout.HorizontalSlider(color.r, 0, 255);
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), rTex);
            GUI.color = rTex.GetPixel(0, 0).grayscale < 0.5f ? Color.white : Color.black;
            GUI.Label(GUILayoutUtility.GetLastRect(), "Red", "tinylabel");
            GUI.color = Color.white;
            byte r;
            if (byte.TryParse(GUILayout.TextField(color.r.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out r)) color.r = r;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            color.g = (byte)GUILayout.HorizontalSlider(color.g, 0, 255);
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), gTex);
            GUI.color = gTex.GetPixel(0, 0).grayscale < 0.5f ? Color.white : Color.black;
            GUI.Label(GUILayoutUtility.GetLastRect(), "Green", "tinylabel");
            GUI.color = Color.white;
            byte g;
            if (byte.TryParse(GUILayout.TextField(color.g.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out g)) color.g = g;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            color.b = (byte)GUILayout.HorizontalSlider(color.b, 0, 255);
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bTex);
            GUI.color = bTex.GetPixel(0, 0).grayscale < 0.5f ? Color.white : Color.black;
            GUI.Label(GUILayoutUtility.GetLastRect(), "Blue", "tinylabel");
            GUI.color = Color.white;
            byte b;
            if (byte.TryParse(GUILayout.TextField(color.b.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out b)) color.b = b;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            color.a = (byte)GUILayout.HorizontalSlider(color.a, 0, 255);
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bwTex);
            GUI.Label(GUILayoutUtility.GetLastRect(), "Alpha", "tinylabel");
            byte a;
            if (byte.TryParse(GUILayout.TextField(color.a.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out a)) color.a = a;
            GUILayout.EndHorizontal();

            if (showAdvancedColor)
            {
                GUILayout.BeginHorizontal();
                color.m = (byte)GUILayout.HorizontalSlider(color.m, 0, 255);
                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bwTex);
                GUI.Label(GUILayoutUtility.GetLastRect(), "Metal", "tinylabel");
                byte m;
                if (byte.TryParse(GUILayout.TextField(color.m.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out m)) color.m = m;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                color.s = (byte)GUILayout.HorizontalSlider(color.s, 0, 255);
                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bwTex);
                GUI.Label(GUILayoutUtility.GetLastRect(), "Smooth", "tinylabel");
                byte s;
                if (byte.TryParse(GUILayout.TextField(color.s.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out s)) color.s = s;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                color.e = (byte)GUILayout.HorizontalSlider(color.e, 0, 255);
                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bwTex);
                GUI.Label(GUILayoutUtility.GetLastRect(), "Glow", "tinylabel");
                byte e;
                if (byte.TryParse(GUILayout.TextField(color.e.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out e)) color.e = e;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                color.u = (byte)GUILayout.HorizontalSlider(color.u, 0, 255);
                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bwTex);
                GUI.Label(GUILayoutUtility.GetLastRect(), "Custom", "tinylabel");
                byte u;
                if (byte.TryParse(GUILayout.TextField(color.u.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out u)) color.u = u;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Randomize")) color = new VColor((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255);
                GUILayout.EndHorizontal();
            }

            showAdvancedColor = GUILayout.Toggle(showAdvancedColor, "Advanced", "button");
        }

        GUILayout.EndVertical();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());
        
        int paletteIndex = ed.tile.GetPalette().GetIndex();
        if (color != null && color != ed.tile.GetPalette().GetColor(paletteIndex)) actQueue.Enqueue(new ChangePaletteColorAct(color, paletteIndex));
    }
}

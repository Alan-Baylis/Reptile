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

    bool showAdvancedColor;
    bool showKeyBindings;

    Texture2D hTex;
    Texture2D svTex;
    Texture2D rTex;
    Texture2D gTex;
    Texture2D bTex;
    Texture2D bwTex;

    float oldH;
    float oldS;
    float oldV;

    void Awake()
    {
        use = this;
    }

    void Start()
    {
        ed = Edit.use;

        hTex = new Texture2D(1, 256);
        svTex = new Texture2D(256, 256);
        rTex = new Texture2D(256, 1);
        gTex = new Texture2D(256, 1);
        bTex = new Texture2D(256, 1);

        Color32[] pixels = new Color32[256];
        for (int i = 0; i < 256; i++) pixels[255 - i] = Color.HSVToRGB(i / 255f, 1f, 1f);
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
        while (actQueue.Count > 0) Edit.Do(actQueue.Dequeue());
    }

    List<Rect> boxRects = new List<Rect>();

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
        bool repaint = Event.current.type == EventType.Repaint;
        if (repaint) boxRects.Clear();

        GUI.skin = skin;

        GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
        GUILayout.BeginVertical();

        Header();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());

        GUILayout.BeginHorizontal();
        
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        refPalScroll = GUILayout.BeginScrollView(refPalScroll, "box", GUILayout.Width(145));
        int refPalIndex = RefPalette();
        GUILayout.EndScrollView();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());

        palScroll = GUILayout.BeginScrollView(palScroll, "box", GUILayout.Width(145));
        int palIndex = Palette();
        GUILayout.EndScrollView();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());
        GUILayout.EndHorizontal();

        VColor palColor = ColorPicker(ed.tile.GetPalette().GetColor(ed.tile.GetPalette().GetIndex()));
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());

        GUILayout.EndVertical();

        Tool();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());

        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical();

        Tile();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());

        int layerIndex = Layers();
        if (repaint) boxRects.Add(GUILayoutUtility.GetLastRect());

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.EndArea();

        KeyBindings();
        
        int paletteIndex = ed.tile.GetPalette().GetIndex();
        if (refPalIndex >= 0)
        {
            actQueue.Enqueue(new ChangePaletteColorAct(ed.refPalette.GetColor(refPalIndex), paletteIndex));
        }

        if (palColor != null && palColor != ed.tile.GetPalette().GetColor(paletteIndex))
        {
            actQueue.Enqueue(new ChangePaletteColorAct(palColor, paletteIndex));
        }

        if (palIndex != paletteIndex) actQueue.Enqueue(new ChangePaletteIndexAct(palIndex));

        if (layerIndex != ed.tile.GetLayerIndex()) actQueue.Enqueue(new ChangeLayerIndexAct(layerIndex));

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
    }

    void Tool()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("Tool");
        Edit.Tool tool = (Edit.Tool)GUILayout.SelectionGrid((int)ed.tool, new[] { "Place", "Paint", "Fill" }, 1, "tool");
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
        GUILayout.EndVertical();
    }

    void Tile()
    {
        GUILayout.BeginVertical("box");

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
        GUILayout.Label("Resize");
        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Width", GUILayout.Width(80));
        int w;
        if (int.TryParse(GUILayout.TextField(Edit.width.ToString()), out w))
        {
            if (w != ed.tile.GetWidth()) Edit.width = w;
        };
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Height", GUILayout.Width(80));
        int h;
        if (int.TryParse(GUILayout.TextField(Edit.height.ToString()), out h))
        {
            if (h != ed.tile.GetHeight()) Edit.height = h;
        };
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Depth", GUILayout.Width(80));
        int d;
        if (int.TryParse(GUILayout.TextField(Edit.depth.ToString()), out d))
        {
            if (d != ed.tile.GetDepth()) Edit.depth = d;
        };
        GUILayout.EndHorizontal();
        GUI.enabled = Edit.width != ed.tile.GetWidth() || Edit.height != ed.tile.GetHeight() || Edit.depth != ed.tile.GetDepth();
        if (GUILayout.Button("Apply"))
        {
            actQueue.Enqueue(new ResizeTileAct(Edit.width, Edit.height, Edit.depth));
        }
        GUI.enabled = true;

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Presets");
        if (GUILayout.Button("8³")) actQueue.Enqueue(new ResizeTileAct(8, 8, 8));
        if (GUILayout.Button("16³")) actQueue.Enqueue(new ResizeTileAct(16, 16, 16));
        if (GUILayout.Button("32³")) actQueue.Enqueue(new ResizeTileAct(32, 32, 32));
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    int Layers()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(200));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.Width(25))) actQueue.Enqueue(new AddLayerAct("Layer " + ed.tile.GetLayerCount()));
        GUILayout.EndHorizontal();
        int layerIndex = ed.tile.GetLayerIndex();
        for (int i = ed.tile.GetLayerCount() - 1; i >= 0; i--)
        {
            VLayer layer = ed.tile.GetLayer(i);
            GUILayout.BeginHorizontal();
            layerIndex = GUILayout.Toggle(layerIndex == i, "", GUILayout.Width(25)) ? i : layerIndex;
            string name = GUILayout.TextField(layer.GetName());
            bool vis = GUILayout.Toggle(layer.GetVisible(), "V", "button", GUILayout.Width(25));
            bool trans = GUILayout.Toggle(layer.GetTransparent(), "T", "button", GUILayout.Width(25));
            bool line = GUILayout.Toggle(layer.GetOutline(), "O", "button", GUILayout.Width(25));
            if (ed.tile.GetLayerCount() > 1)
            {
                if (GUILayout.Button("-", GUILayout.Width(25))) actQueue.Enqueue(new RemoveLayerAct(i));
            }
            else
            {
                GUILayout.Space(30f);
            }
            GUILayout.EndHorizontal();

            if (name != layer.GetName() || vis != layer.GetVisible() || trans != layer.GetTransparent() || line != layer.GetOutline())
            {
                actQueue.Enqueue(new ChangeLayerInfoAct(i, name, vis, trans, line));
            }
        }
        GUILayout.EndVertical();

        return layerIndex;
    }

    int RefPalette()
    {
        int index = -1;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            string path = TinyFileDialogs.OpenFileDialog("Load Reference Palette", Edit.GetDirectory(), new[] { "*.rpal" }, "Reptile Palettes (*.rpal)", false);
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
        return index;
    }

    int Palette()
    {
        int index = ed.tile.GetPalette().GetIndex();
        VPalette palette = Edit.use.tile.GetPalette();
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
        if (palette.GetCount() > 1 && GUILayout.Button("-"))
        {
            actQueue.Enqueue(new RemovePaletteColorAct());
        }
        if (palette.GetCount() < 256 && GUILayout.Button("+"))
        {
            actQueue.Enqueue(new AddPaletteColorAct(new VColor((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255)));
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
        return index;
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
            Edit.use.bindLightRotate,
            Edit.use.bindToolPlace,
            Edit.use.bindToolPaint,
            Edit.use.bindToolFill,
            Edit.use.bindUseTool,
            Edit.use.bindUseToolAlt,
            Edit.use.bindPlaneLock
        };

        GUILayout.BeginArea(new Rect(0f, 25f, Screen.width, Screen.height - 25f));
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
        GUILayout.EndArea();
    }
    
    bool svDrag;

    VColor ColorPicker(VColor color)
    {
        if (ed.tile.GetPalette().GetIndex() >= ed.tile.GetPalette().GetCount()) return null;

        color = new VColor(color);

        GUILayout.BeginVertical("box", GUILayout.Width(295));

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
            for (int x = 0; x < width; x ++)
            {
                for (int y = 0; y < height; y ++)
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

        Rect rect = GUILayoutUtility.GetRect(svTex.width, svTex.height);
        rect.width = Mathf.Min(rect.width, svTex.width);
        rect.height = Mathf.Min(rect.height, svTex.height);
        GUI.DrawTexture(rect, svTex);
        if ((isMouseDown || drag) && rect.Contains(mp))
        {
            svDrag = true;
            if (isMouseDown) Event.current.Use();
            mp.x = Mathf.Clamp(mp.x, rect.xMin, rect.xMax);
            mp.y = Mathf.Clamp(mp.y, rect.yMin, rect.yMax);
            int x = Mathf.RoundToInt(mp.x - rect.x);
            int y = svTex.height - Mathf.RoundToInt(mp.y - rect.y);
            Color32 tc = svTex.GetPixel(x, y);
            color.r = tc.r;
            color.g = tc.g;
            color.b = tc.b;
        }

        GUI.color = new Color32((byte)(255 - color.r), (byte)(255 - color.g), (byte)(255 - color.b), 255);
        GUI.DrawTexture(new Rect(rect.x + rect.width * sat - 4f, rect.yMax - rect.height * val - 4f, 8f, 8f), svCursorTex);
        GUI.color = Color.white;

        float newHue = GUILayout.VerticalSlider(hue, 0f, 1f, GUILayout.Height(svTex.height));
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

        int labelWidth = 60;
        int fieldWidth = 35;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Red", GUILayout.Width(labelWidth));
        color.r = (byte)GUILayout.HorizontalSlider(color.r, 0, 255);
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), rTex);
        byte r;
        if (byte.TryParse(GUILayout.TextField(color.r.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out r)) color.r = r;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Green", GUILayout.Width(labelWidth));
        color.g = (byte)GUILayout.HorizontalSlider(color.g, 0, 255);
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), gTex);
        byte g;
        if (byte.TryParse(GUILayout.TextField(color.g.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out g)) color.g = g;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Blue", GUILayout.Width(labelWidth));
        color.b = (byte)GUILayout.HorizontalSlider(color.b, 0, 255);
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bTex);
        byte b;
        if (byte.TryParse(GUILayout.TextField(color.b.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out b)) color.b = b;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Alpha", GUILayout.Width(labelWidth));
        color.a = (byte)GUILayout.HorizontalSlider(color.a, 0, 255);
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bwTex);
        byte a;
        if (byte.TryParse(GUILayout.TextField(color.a.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out a)) color.a = a;
        GUILayout.EndHorizontal();

        if (showAdvancedColor)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Metal", GUILayout.Width(labelWidth));
            color.m = (byte)GUILayout.HorizontalSlider(color.m, 0, 255);
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bwTex);
            byte m;
            if (byte.TryParse(GUILayout.TextField(color.m.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out m)) color.m = m;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Smooth", GUILayout.Width(labelWidth));
            color.s = (byte)GUILayout.HorizontalSlider(color.s, 0, 255);
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bwTex);
            byte s;
            if (byte.TryParse(GUILayout.TextField(color.s.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out s)) color.s = s;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Glow", GUILayout.Width(labelWidth));
            color.e = (byte)GUILayout.HorizontalSlider(color.e, 0, 255);
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bwTex);
            byte e;
            if (byte.TryParse(GUILayout.TextField(color.e.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out e)) color.e = e;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Custom", GUILayout.Width(labelWidth));
            color.u = (byte)GUILayout.HorizontalSlider(color.u, 0, 255);
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), bwTex);
            byte u;
            if (byte.TryParse(GUILayout.TextField(color.u.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out u)) color.u = u;
            GUILayout.EndHorizontal();
        }

        showAdvancedColor = GUILayout.Toggle(showAdvancedColor, "Show Advanced", "button");

        GUILayout.EndVertical();

        return color;
    }
}

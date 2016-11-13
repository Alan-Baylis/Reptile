using UnityEngine;
using System.Collections.Generic;

public class UI : MonoBehaviour
{
    public static UI use;

    public string version;

    public GUISkin skin;
    public Texture2D alphaTex;
    public Texture2D blankTex;
    
    Vector2 palScroll;
    Vector2 refPalScroll;

    Edit ed;

    Queue<Act> actQueue = new Queue<Act>();

    bool showAdvancedColor;
    bool showKeyBindings;

    void Awake()
    {
        use = this;
    }

    void Start()
    {
        ed = Edit.use;
        Edit.width = ed.tile.GetWidth();
        Edit.height = ed.tile.GetHeight();
        Edit.depth = ed.tile.GetDepth();
    }

    void Update()
    {
        while (actQueue.Count > 0) Edit.Do(actQueue.Dequeue());
    }

    void OnGUI()
    {
        GUI.skin = skin;
        GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
        GUILayout.BeginVertical();

        // Begin header section
        GUILayout.BeginHorizontal();

        GUI.enabled = Edit.undos.Count > 0;
        if (GUILayout.Button(Edit.undos.Count > 0 ? "Undo " + Edit.undos.Peek() : "Undo")) actQueue.Enqueue(new UndoAct());
        GUI.enabled = Edit.redos.Count > 0;
        if (GUILayout.Button(Edit.redos.Count > 0 ? "Redo " + Edit.redos.Peek() : "Redo")) actQueue.Enqueue(new RedoAct());
        GUI.enabled = true;

        showKeyBindings = GUILayout.Toggle(showKeyBindings, "Key Bindings", "button");

        GUILayout.FlexibleSpace();

        GUILayout.Label("Version " + version);

        GUILayout.EndHorizontal();
        // End header section

        // Begin middle section
        GUILayout.BeginHorizontal();

        // Begin palette section
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        refPalScroll = GUILayout.BeginScrollView(refPalScroll, "box", GUILayout.Width(145));
        int refPalIndex = RefPalette();
        GUILayout.EndScrollView();

        palScroll = GUILayout.BeginScrollView(palScroll, "box", GUILayout.Width(145));
        int palIndex = Palette();
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();

        VColor palColor = null;
        if (ed.tile.GetPalette().GetIndex() < ed.tile.GetPalette().GetCount())
            palColor = ColorEditor(ed.tile.GetPalette().GetColor(ed.tile.GetPalette().GetIndex()));

        GUILayout.EndVertical();
        // End palette section

        // Begin tool section
        GUILayout.BeginVertical();
        Edit.Tool tool = (Edit.Tool)GUILayout.SelectionGrid((int)ed.tool, new[] { "Place", "Paint" }, 2, "tool");
        GUILayout.EndVertical();
        // End tool section

        GUILayout.FlexibleSpace();

        // Begin tile section
        GUILayout.BeginVertical();

        Tile();

        int layerIndex = Layers();

        GUILayout.EndVertical();
        // End tile section

        GUILayout.EndHorizontal();
        // End middle section

        // Footer goes here

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

        if (tool != Edit.use.tool) actQueue.Enqueue(new ChangeToolAct(tool));

        if (layerIndex != ed.tile.GetLayerIndex()) actQueue.Enqueue(new ChangeLayerIndexAct(layerIndex));
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
        GUILayout.EndHorizontal();

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

    VColor ColorEditor(VColor color)
    {
        int labelWidth = 60;
        int fieldWidth = 50;

        color = new VColor(color);
        GUILayout.BeginVertical("box", GUILayout.Width(295));

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();

        GUILayout.BeginVertical("box");
        BigSwatch(true, new Color32(color.r, color.g, color.b, color.a));
        GUILayout.EndVertical();

        if (GUILayout.Button("White")) color = new VColor(255, 255, 255, 255);
        if (GUILayout.Button("Black")) color = new VColor(0, 0, 0, 255);

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Red", GUILayout.Width(labelWidth));
        color.r = (byte)GUILayout.HorizontalSlider(color.r, 0, 255);
        byte r;
        if (byte.TryParse(GUILayout.TextField(color.r.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out r)) color.r = r;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Green", GUILayout.Width(labelWidth));
        color.g = (byte)GUILayout.HorizontalSlider(color.g, 0, 255);
        byte g;
        if (byte.TryParse(GUILayout.TextField(color.g.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out g)) color.g = g;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Blue", GUILayout.Width(labelWidth));
        color.b = (byte)GUILayout.HorizontalSlider(color.b, 0, 255);
        byte b;
        if (byte.TryParse(GUILayout.TextField(color.b.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out b)) color.b = b;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Alpha", GUILayout.Width(labelWidth));
        color.a = (byte)GUILayout.HorizontalSlider(color.a, 0, 255);
        byte a;
        if (byte.TryParse(GUILayout.TextField(color.a.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out a)) color.a = a;
        GUILayout.EndHorizontal();

        if (showAdvancedColor)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Metal", GUILayout.Width(labelWidth));
            color.m = (byte)GUILayout.HorizontalSlider(color.m, 0, 255);
            byte m;
            if (byte.TryParse(GUILayout.TextField(color.m.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out m)) color.m = m;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Smooth", GUILayout.Width(labelWidth));
            color.s = (byte)GUILayout.HorizontalSlider(color.s, 0, 255);
            byte s;
            if (byte.TryParse(GUILayout.TextField(color.s.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out s)) color.s = s;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Glow", GUILayout.Width(labelWidth));
            color.e = (byte)GUILayout.HorizontalSlider(color.e, 0, 255);
            byte e;
            if (byte.TryParse(GUILayout.TextField(color.e.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out e)) color.e = e;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Custom", GUILayout.Width(labelWidth));
            color.u = (byte)GUILayout.HorizontalSlider(color.u, 0, 255);
            byte u;
            if (byte.TryParse(GUILayout.TextField(color.u.ToString(), 3, GUILayout.MaxWidth(fieldWidth)), out u)) color.u = u;
            GUILayout.EndHorizontal();
        }

        showAdvancedColor = GUILayout.Toggle(showAdvancedColor, "Show Advanced", "button");

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        return color;
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
            Edit.use.bindUseTool,
            Edit.use.bindUseToolAlt
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
}

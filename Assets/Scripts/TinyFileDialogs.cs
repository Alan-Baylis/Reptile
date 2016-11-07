using UnityEngine;
using System;
using System.Runtime.InteropServices;

public static class TinyFileDialogs
{
    private static string stringFromChar(IntPtr ptr)
    {
        return Marshal.PtrToStringAnsi(ptr);
    }

    public enum DialogType
    {
        Ok,
        OkCancel,
        YesNo
    }

    public enum IconType
    {
        Info,
        Warning,
        Error,
        Question
    }

    static string GetDialogType(DialogType type)
    {
        if (type == DialogType.Ok) return "ok";
        else if (type == DialogType.OkCancel) return "okcancel";
        else if (type == DialogType.YesNo) return "yesno";
        throw new ArgumentOutOfRangeException("type");
    }

    static string GetIconType(IconType type)
    {
        if (type == IconType.Info) return "info";
        else if (type == IconType.Warning) return "warning";
        else if (type == IconType.Error) return "error";
        else if (type == IconType.Question) return "question";
        throw new ArgumentOutOfRangeException("type");
    }

    public static bool MessageBox(string title, string message, DialogType dialogType, IconType iconType, bool defaultOkay)
    {
        return tinyfd_messageBox(title, message, GetDialogType(dialogType), GetIconType(iconType), defaultOkay ? 1 : 0) == 1;
    }

    public static string InputBox(string title, string message, string defaultInput)
    {
        return stringFromChar(tinyfd_inputBox(title, message, defaultInput));
    }

    public static string SaveFileDialog(string title, string defaultPath, string[] filterPatterns, string filterDescription)
    {
        return stringFromChar(tinyfd_saveFileDialog(title, defaultPath, filterPatterns.Length, filterPatterns, filterDescription));
    }

    public static string OpenFileDialog(string title, string defaultPath, string[] filterPatterns, string filterDescription, bool allowMultiSelect)
    {
        return stringFromChar(tinyfd_openFileDialog(title, defaultPath, filterPatterns.Length, filterPatterns, filterDescription, (allowMultiSelect) ? 1 : 0));
    }

    public static string SelectFolderDialog(string title, string defaultPath)
    {
        return stringFromChar(tinyfd_selectFolderDialog(title, defaultPath));
    }
    
    [DllImport("tinyfiledialogs", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int tinyfd_messageBox(string aTitle, string aMessage, string aDialogTyle, string aIconType, int aDefaultButton);
    [DllImport("tinyfiledialogs", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr tinyfd_inputBox(string aTitle, string aMessage, string aDefaultInput);
    [DllImport("tinyfiledialogs", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr tinyfd_saveFileDialog(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription);
    [DllImport("tinyfiledialogs", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr tinyfd_openFileDialog(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription, int aAllowMultipleSelects);
    [DllImport("tinyfiledialogs", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr tinyfd_selectFolderDialog(string aTitle, string aDefaultPathAndFile);
    //[DllImport("tinyfiledialogs", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    //static extern IntPtr tinyfd_colorChooser(string aTitle, string aDefaultHexRGB, byte[] aDefaultRGB, byte[] aoResultRGB);
}
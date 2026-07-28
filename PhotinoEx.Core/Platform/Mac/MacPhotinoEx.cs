using System.Runtime.InteropServices;
using PhotinoEx.Core.Models;
using Point = System.Drawing.Point;
using Monitor = PhotinoEx.Core.Models.Monitor;
using Size = System.Drawing.Size;

namespace PhotinoEx.Core.Platform.Mac;

public class MacPhotinoEx : PhotinoEx
{
    private const string ObjCLibrary = "/usr/lib/libobjc.A.dylib";

    public MacPhotinoEx(PhotinoExInitParams exInitParams)
    {
        throw new NotImplementedException();
    }

    // public override void Show()
    // {
    //     throw new NotImplementedException();
    // }

    // public override void Center()
    // {
    //     throw new NotImplementedException();
    // }

    public override void ClearBrowserAutoFill()
    {
        throw new NotImplementedException();
    }

    public override void SetClipboardText(string text)
    {
        var pasteboard = Send(GetClass("NSPasteboard"), GetSelector("generalPasteboard"));
        Send(pasteboard, GetSelector("clearContents"));

        var nsString = GetClass("NSString");
        var value = Send(nsString, GetSelector("stringWithUTF8String:"), text);
        var type = Send(nsString, GetSelector("stringWithUTF8String:"), "public.utf8-plain-text");
        if (!Send(pasteboard, GetSelector("setString:forType:"), value, type))
        {
            throw new InvalidOperationException("macOS rejected the clipboard content.");
        }
    }

    public override void SetClipboardFiles(IReadOnlyList<string> paths)
    {
        var objects = Send(GetClass("NSMutableArray"), GetSelector("array"));
        var nsString = GetClass("NSString");
        var nsUrl = GetClass("NSURL");
        foreach (var path in paths)
        {
            var value = Send(nsString, GetSelector("stringWithUTF8String:"), path);
            var url = SendObject(nsUrl, GetSelector("fileURLWithPath:"), value);
            SendObject(objects, GetSelector("addObject:"), url);
        }

        var pasteboard = Send(GetClass("NSPasteboard"), GetSelector("generalPasteboard"));
        Send(pasteboard, GetSelector("clearContents"));
        if (!SendBoolObject(pasteboard, GetSelector("writeObjects:"), objects))
        {
            throw new InvalidOperationException("macOS rejected the clipboard file list.");
        }
    }

    public override Task<FileDragDropEffects> BeginFileDragAsync(
        IReadOnlyList<string> paths,
        FileDragDropEffects allowedEffects,
        CancellationToken cancellationToken
    ) => throw new PlatformNotSupportedException("Outbound file dragging is not implemented on macOS.");

    private static IntPtr GetClass(string name) => objc_getClass(name);

    private static IntPtr GetSelector(string name) => sel_registerName(name);

    [DllImport(ObjCLibrary)]
    private static extern IntPtr objc_getClass(string name);

    [DllImport(ObjCLibrary)]
    private static extern IntPtr sel_registerName(string name);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr Send(IntPtr receiver, IntPtr selector);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr Send(IntPtr receiver, IntPtr selector, string value);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendObject(IntPtr receiver, IntPtr selector, IntPtr value);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool SendBoolObject(IntPtr receiver, IntPtr selector, IntPtr value);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Send(IntPtr receiver, IntPtr selector, IntPtr value, IntPtr type);

    public override void Close()
    {
        throw new NotImplementedException();
    }

    public override bool GetTransparentEnabled()
    {
        throw new NotImplementedException();
    }

    public override bool GetContextMenuEnabled()
    {
        throw new NotImplementedException();
    }

    public override bool GetDevToolsEnabled()
    {
        throw new NotImplementedException();
    }

    public override bool GetFullScreen()
    {
        throw new NotImplementedException();
    }

    public override bool GetGrantBrowserPermissions()
    {
        throw new NotImplementedException();
    }

    public override string GetUserAgent()
    {
        throw new NotImplementedException();
    }

    public override bool GetMediaAutoplayEnabled()
    {
        throw new NotImplementedException();
    }

    public override bool GetFileSystemAccessEnabled()
    {
        throw new NotImplementedException();
    }

    public override bool GetWebSecurityEnabled()
    {
        throw new NotImplementedException();
    }

    public override bool GetJavascriptClipboardAccessEnabled()
    {
        throw new NotImplementedException();
    }

    public override bool GetMediaStreamEnabled()
    {
        throw new NotImplementedException();
    }

    public override bool GetSmoothScrollingEnabled()
    {
        throw new NotImplementedException();
    }

    public override bool GetNotificationsEnabled()
    {
        throw new NotImplementedException();
    }

    public override string GetIconFileName()
    {
        throw new NotImplementedException();
    }

    public override bool GetMaximized()
    {
        throw new NotImplementedException();
    }

    public override bool GetMinimized()
    {
        throw new NotImplementedException();
    }

    // public override Point GetPosition()
    // {
    //     throw new NotImplementedException();
    // }

    public override bool GetResizable()
    {
        throw new NotImplementedException();
    }

    public override uint GetScreenDpi()
    {
        throw new NotImplementedException();
    }

    public override Size GetSize()
    {
        throw new NotImplementedException();
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }

    public override bool GetTopmost()
    {
        throw new NotImplementedException();
    }

    public override int GetZoom()
    {
        throw new NotImplementedException();
    }

    public override bool GetIgnoreCertificateErrorsEnabled()
    {
        throw new NotImplementedException();
    }

    public override void NavigateToString(string content)
    {
        throw new NotImplementedException();
    }

    public override void NavigateToUrl(string url)
    {
        throw new NotImplementedException();
    }

    public override void Restore()
    {
        throw new NotImplementedException();
    }

    public override void SendWebMessage(string message)
    {
        throw new NotImplementedException();
    }

    public override void SetTransparentEnabled(bool enabled)
    {
        throw new NotImplementedException();
    }

    public override void SetContextMenuEnabled(bool enabled)
    {
        throw new NotImplementedException();
    }

    public override void SetDevToolsEnabled(bool enabled)
    {
        throw new NotImplementedException();
    }

    public override void SetIconFile(string filename)
    {
        throw new NotImplementedException();
    }

    public override void SetFullScreen(bool fullScreen)
    {
        throw new NotImplementedException();
    }

    public override void SetMaximized(bool maximized)
    {
        throw new NotImplementedException();
    }

    // public override void SetMaxSize(Size size)
    // {
    //     throw new NotImplementedException();
    // }

    public override void SetMinimized(bool minimized)
    {
        throw new NotImplementedException();
    }

    // public override void SetMinSize(Size size)
    // {
    //     throw new NotImplementedException();
    // }

    // public override void SetPosition(Point position)
    // {
    //     throw new NotImplementedException();
    // }

    public override void SetResizable(bool resizable)
    {
        throw new NotImplementedException();
    }

    public override void SetSize(Size size)
    {
        throw new NotImplementedException();
    }

    public override void SetTitle(string title)
    {
        throw new NotImplementedException();
    }

    public override void SetTopmost(bool topmost)
    {
        throw new NotImplementedException();
    }

    public override void SetZoom(int zoom)
    {
        throw new NotImplementedException();
    }

    public override void ShowNotification(string title, string message)
    {
        throw new NotImplementedException();
    }

    public override void WaitForExit()
    {
        throw new NotImplementedException();
    }

    public override void AddCustomSchemeName(string scheme)
    {
        throw new NotImplementedException();
    }

    public override List<Monitor> GetAllMonitors()
    {
        throw new NotImplementedException();
    }

    public override void SetClosingCallback(Func<bool> callback)
    {
        throw new NotImplementedException();
    }

    public override void SetFocusInCallback(Action callback)
    {
        throw new NotImplementedException();
    }

    public override void SetFocusOutCallback(Action callback)
    {
        throw new NotImplementedException();
    }

    public override void SetMovedCallback(Action<int, int> callback)
    {
        throw new NotImplementedException();
    }

    public override void SetResizedCallback(Action<int, int> callback)
    {
        throw new NotImplementedException();
    }

    public override void SetMaximizedCallback(Action callback)
    {
        throw new NotImplementedException();
    }

    public override void SetRestoredCallback(Action callback)
    {
        throw new NotImplementedException();
    }

    public override void SetMinimizedCallback(Action callback)
    {
        throw new NotImplementedException();
    }

    public override void Invoke(Action callback)
    {
        throw new NotImplementedException();
    }

    public override Point GetPosition()
    {
        throw new NotImplementedException();
    }

    public override void SetPosition(Point newLocation)
    {
        throw new NotImplementedException();
    }
}

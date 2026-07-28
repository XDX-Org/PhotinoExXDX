using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using PhotinoEx.Core.Models;
using PhotinoEx.Core.Platform.Windows.Utils;

namespace PhotinoEx.Core.Platform.Windows.FileDragDrop;

[ComVisible(true)]
[Guid("00000122-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWinDropTarget
{
    [PreserveSig]
    int DragEnter(IDataObject dataObject, uint keyState, Point point, ref uint effect);

    [PreserveSig]
    int DragOver(uint keyState, Point point, ref uint effect);

    [PreserveSig]
    int DragLeave();

    [PreserveSig]
    int Drop(IDataObject dataObject, uint keyState, Point point, ref uint effect);
}

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
internal sealed class WinDropTarget(IntPtr window, Action<FilesDroppedEventArgs> filesDropped)
    : IWinDropTarget
{
    private const short CfHDrop = 15;
    private const uint MaximumDroppedFiles = 4096;
    private const uint MaximumPathLength = 32767;
    private const uint DropEffectNone = 0;
    private const uint DropEffectCopy = 1;
    private static readonly FORMATETC FileDropFormat = new()
    {
        cfFormat = CfHDrop,
        dwAspect = DVASPECT.DVASPECT_CONTENT,
        lindex = -1,
        tymed = TYMED.TYMED_HGLOBAL,
    };
    private bool _canDrop;

    public int DragEnter(IDataObject dataObject, uint keyState, Point point, ref uint effect)
    {
        var format = FileDropFormat;
        _canDrop = dataObject.QueryGetData(ref format) == 0;
        effect = GetEffect(effect);
        return 0;
    }

    public int DragOver(uint keyState, Point point, ref uint effect)
    {
        effect = GetEffect(effect);
        return 0;
    }

    public int DragLeave()
    {
        _canDrop = false;
        return 0;
    }

    public int Drop(IDataObject dataObject, uint keyState, Point point, ref uint effect)
    {
        effect = GetEffect(effect);
        if (effect == DropEffectNone)
        {
            _canDrop = false;
            return 0;
        }

        var format = FileDropFormat;
        dataObject.GetData(ref format, out var medium);
        try
        {
            WinApi.ScreenToClient(window, ref point);
            filesDropped(
                new FilesDroppedEventArgs(
                    ReadPaths(medium.unionmember),
                    FileDragDropEffects.Copy,
                    point.X,
                    point.Y
                )
            );
        }
        finally
        {
            WinApi.ReleaseStgMedium(ref medium);
            _canDrop = false;
        }

        return 0;
    }

    internal static IReadOnlyList<string> ReadPaths(IntPtr drop)
    {
        var fileCount = WinApi.DragQueryFile(drop, uint.MaxValue, null, 0);
        if (fileCount > MaximumDroppedFiles)
        {
            throw new InvalidDataException("The Windows file-drop payload contains an invalid file count.");
        }

        var paths = new string[fileCount];
        for (uint index = 0; index < fileCount; index++)
        {
            var pathLength = WinApi.DragQueryFile(drop, index, null, 0);
            if (pathLength > MaximumPathLength)
            {
                throw new InvalidDataException("The Windows file-drop payload contains an invalid path length.");
            }

            var path = new StringBuilder(checked((int) pathLength + 1));
            WinApi.DragQueryFile(drop, index, path, (uint) path.Capacity);
            paths[index] = path.ToString();
        }

        return paths;
    }

    private uint GetEffect(uint allowedEffects) =>
        _canDrop && (allowedEffects & DropEffectCopy) != 0 ? DropEffectCopy : DropEffectNone;
}

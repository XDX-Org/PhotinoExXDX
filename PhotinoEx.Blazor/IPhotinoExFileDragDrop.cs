using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PhotinoEx.Core.Models;

namespace PhotinoEx.Blazor;

public interface IPhotinoExFileDragDrop
{
    event EventHandler<FilesDroppedEventArgs>? FilesDropped;

    Task<FileDragDropEffects> BeginDragAsync(
        IReadOnlyList<string> paths,
        FileDragDropEffects allowedEffects = FileDragDropEffects.Copy,
        CancellationToken cancellationToken = default
    );
}

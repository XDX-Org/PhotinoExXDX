using PhotinoEx.Core;
using PhotinoEx.Core.Models;
using Xunit;

namespace PhotinoEx.Core.Tests;

public sealed class FileDragDropTests
{
    [Fact]
    public void RegisterFilesDroppedHandlerRaisesInboundEvent()
    {
        var window = new PhotinoExWindow();
        var args = new FilesDroppedEventArgs(["/tmp/file.txt"], FileDragDropEffects.Copy, 12, 34);
        FilesDroppedEventArgs? received = null;

        Assert.Same(window, window.RegisterFilesDroppedHandler((_, value) => received = value));
        window.OnFilesDropped(args);

        Assert.Same(args, received);
        Assert.Equal(12, received!.ClientX);
        Assert.Equal(34, received.ClientY);
    }

    [Fact]
    public void NormalizeFilePathsAcceptsFilesDirectoriesAndRemovesDuplicates()
    {
        var directory = Directory.CreateTempSubdirectory();
        var file = Path.Combine(directory.FullName, "file.txt");
        File.Create(file).Dispose();

        try
        {
            var paths = PhotinoExWindow.NormalizeFilePaths(
                [file, directory.FullName, file],
                "paths"
            );

            Assert.Equal([Path.GetFullPath(file), Path.GetFullPath(directory.FullName)], paths);
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizeFilePathsRejectsBlankEntries(string path)
    {
        Assert.Throws<ArgumentException>(() =>
            PhotinoExWindow.NormalizeFilePaths([path], "paths")
        );
    }

    [Fact]
    public void NormalizeFilePathsRejectsEmptyAndMissingEntries()
    {
        Assert.Throws<ArgumentException>(() => PhotinoExWindow.NormalizeFilePaths([], "paths"));
        Assert.Throws<FileNotFoundException>(() =>
            PhotinoExWindow.NormalizeFilePaths(
                [Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid():N}")],
                "paths"
            )
        );
    }

    [Theory]
    [InlineData(FileDragDropEffects.None)]
    [InlineData((FileDragDropEffects)8)]
    public async Task BeginFileDragRejectsInvalidEffects(FileDragDropEffects effects)
    {
        var window = new PhotinoExWindow();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            window.BeginFileDragAsync(
                [Directory.GetCurrentDirectory()],
                effects,
                TestContext.Current.CancellationToken
            )
        );
    }

    [Fact]
    public async Task BeginFileDragRequiresAnInitializedWindow()
    {
        var window = new PhotinoExWindow();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            window.BeginFileDragAsync(
                [Directory.GetCurrentDirectory()],
                cancellationToken: TestContext.Current.CancellationToken
            )
        );
    }

    [Fact]
    public async Task BeginFileDragHonorsPreCancelledToken()
    {
        var window = new PhotinoExWindow();
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(
            TestContext.Current.CancellationToken
        );
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            window.BeginFileDragAsync(
                [Directory.GetCurrentDirectory()],
                cancellationToken: cancellation.Token
            )
        );
    }
}

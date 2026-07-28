using PhotinoEx.Core.Models;
using PhotinoEx.Core.Platform;
using Xunit;

namespace PhotinoEx.Core.Tests;

public sealed class TrayTests : IDisposable
{
    private readonly string _iconPath = Path.GetTempFileName();

    [Fact]
    public async Task RegisterGetAndUnregisterTracksIcon()
    {
        var tray = new FakeTray();
        var icon = await tray.RegisterAsync(new TrayIconOptions("main", _iconPath));

        Assert.True(tray.TryGet("main", out var registered));
        Assert.Same(icon, registered);
        Assert.True(await tray.UnregisterAsync("main"));
        Assert.False(tray.TryGet("main", out _));
        Assert.True(((FakeIcon)icon).IsDisposed);
    }

    [Fact]
    public async Task DuplicateIdIsRejected()
    {
        var tray = new FakeTray();
        await tray.RegisterAsync(new TrayIconOptions("main", _iconPath));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            tray.RegisterAsync(new TrayIconOptions("main", _iconPath))
        );
    }

    [Fact]
    public async Task IdCanBeReusedAfterUnregister()
    {
        var tray = new FakeTray();
        await tray.RegisterAsync(new TrayIconOptions("main", _iconPath));
        await tray.UnregisterAsync("main");

        var replacement = await tray.RegisterAsync(new TrayIconOptions("main", _iconPath));

        Assert.Equal("main", replacement.Id);
    }

    [Fact]
    public async Task BlankAndMissingIconInputsAreRejected()
    {
        var tray = new FakeTray();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            tray.RegisterAsync(new TrayIconOptions("", _iconPath))
        );
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            tray.RegisterAsync(new TrayIconOptions("main", _iconPath + ".missing"))
        );
    }

    [Fact]
    public async Task DuplicateNestedMenuIdIsRejected()
    {
        var menu = new TrayMenu
        {
            Items =
            [
                new TrayMenuCommand("same", "First", _ => Task.CompletedTask),
                new TraySubmenu("submenu", "More", [new TrayMenuSeparator("same")]),
            ],
        };

        var tray = new FakeTray();
        await Assert.ThrowsAsync<ArgumentException>(() =>
            tray.RegisterAsync(new TrayIconOptions("main", _iconPath, Menu: menu))
        );
    }

    [Fact]
    public async Task UnregisterAllDisposesEveryIcon()
    {
        var tray = new FakeTray();
        var first = (FakeIcon)await tray.RegisterAsync(new TrayIconOptions("first", _iconPath));
        var second = (FakeIcon)await tray.RegisterAsync(new TrayIconOptions("second", _iconPath));

        await tray.UnregisterAllAsync();

        Assert.True(first.IsDisposed);
        Assert.True(second.IsDisposed);
    }

    public void Dispose() => File.Delete(_iconPath);

    private sealed class FakeTray : PhotinoExTrayBase
    {
        protected override Task<IPhotinoExTrayIcon> CreateIconAsync(
            TrayIconOptions options,
            CancellationToken cancellationToken
        ) => Task.FromResult<IPhotinoExTrayIcon>(new FakeIcon(options));
    }

    private sealed class FakeIcon(TrayIconOptions options) : IPhotinoExTrayIcon
    {
        public string Id { get; } = options.Id;
        public bool IsVisible { get; private set; } = options.IsVisible;
        public bool IsDisposed { get; private set; }
        public event EventHandler<TrayIconEventArgs>? Clicked;
        public event EventHandler? MenuOpening;
        public Task SetIconAsync(string iconPath) => Task.CompletedTask;
        public Task SetToolTipAsync(string? toolTip) => Task.CompletedTask;
        public Task SetVisibleAsync(bool visible) { IsVisible = visible; return Task.CompletedTask; }
        public Task SetMenuAsync(TrayMenu? menu) => Task.CompletedTask;
        public ValueTask DisposeAsync() { IsDisposed = true; return ValueTask.CompletedTask; }
    }
}

# Integration Tests

Native clipboard integration tests are isolated in `PhotinoEx.IntegrationTests`. They create a real PhotinoEx window and read clipboard output through an independent OS consumer.

The project builds with the solution but is not treated as a test project unless explicitly enabled. Normal headless runs remain:

```bash
dotnet test PhotinoEx.slnx
```

## Running clipboard smoke tests

Run from a logged-in graphical desktop session:

```bash
PHOTINOEX_RUN_GUI_TESTS=1 dotnet test PhotinoEx.IntegrationTests
```

Requirements:

| Platform/session | Requirement |
| --- | --- |
| Windows | WebView2 runtime and an interactive desktop |
| Linux Wayland | GTK 4 and WebKitGTK 6 |
| Linux X11 | GTK 4 and WebKitGTK 6 |
| macOS | Tests are deferred until the macOS window backend is operational |

The smoke test covers Unicode text plus a mixed file/directory selection containing spaces and `#`. Windows output is read through `GetClipboardData` and `DragQueryFile`; Linux output is read through the GTK/GDK clipboard API.

These tests manipulate the user's real clipboard and briefly open a native window. Do not run them in parallel with applications or tests that also own the clipboard.

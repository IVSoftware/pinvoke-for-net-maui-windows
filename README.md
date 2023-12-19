# PInvoke in MAUI

Just to expand upon the other answers here, if you wanted to _also_ remove the native Title Bar when running in Windows, I've had some luck using P/Invoke to do that. Here's my all-in-one code:

 - Hide Navigation Bar
 - Remove System Menu
 - Resize to a "mobile" aspect ratio.

[![screenshot][1]][1]

```csharp
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

#if WINDOWS
    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.HandlerChanged += (sender, e) =>
        {
            if(sender is Window mauiWindow && mauiWindow.Handler is IElementHandler handler) 
            {
                Shell.SetNavBarIsVisible(Shell.Current, false);
                Dispatcher.Dispatch(() =>
                {
                    if (handler.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
                    {
                        var hWnd = nativeWindow.GetWindowHandle();
                        int style;
                        style = GetWindowLong(hWnd, GWL_STYLE);
                        style &= ~(WS_CAPTION | WS_SYSMENU);
                        SetWindowLong(hWnd, GWL_STYLE, style);
                    }
                    window.Width = 540;
                    window.Height = 960;
                });
            }
        };
        return window;
    }
#endif


    private const int GWL_STYLE = -16;
    private const int WS_CAPTION = 0xC00000;
    private const int WS_SYSMENU = 0x80000;

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong); 
}
```

  [1]: https://i.stack.imgur.com/1fsYQ.png
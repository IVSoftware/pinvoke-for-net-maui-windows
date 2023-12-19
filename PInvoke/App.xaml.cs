
using Microsoft.Maui.Platform;
using System.Runtime.InteropServices;

namespace PInvoke
{
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
                    Dispatcher.Dispatch(() =>
                    {
                        Shell.SetNavBarIsVisible(Shell.Current, false);
                        if (handler.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
                        {
                            var hWnd = nativeWindow.GetWindowHandle();
                            int style;
                            style = GetWindowLong(hWnd, GWL_STYLE);
                            style &= ~(WS_CAPTION | WS_SYSMENU);
                            SetWindowLong(hWnd, GWL_STYLE, style);
                        }
                        mauiWindow.Width = 540;
                        mauiWindow.Height = 960;
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
}

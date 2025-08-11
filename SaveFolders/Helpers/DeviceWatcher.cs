using System;
using System.Windows;
using System.Windows.Interop;

namespace SaveFolders.Helpers
{
    public class DeviceWatcher
    {
        private readonly Window _window;

        public event EventHandler? DeviceChanged;

        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        public DeviceWatcher(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
        }

        public void Start()
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(_window).Handle);
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                int eventCode = wParam.ToInt32();
                if (eventCode == DBT_DEVICEARRIVAL || eventCode == DBT_DEVICEREMOVECOMPLETE)
                {
                    DeviceChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            return IntPtr.Zero;
        }
    }
}

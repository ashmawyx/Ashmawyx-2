using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AshmawyX
{
    public static class WindowScanner
    {
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int maxLength);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        public static List<WindowInfo> GetAOWindows()
        {
            List<WindowInfo> windows = new List<WindowInfo>();

            EnumWindows((hWnd, lParam) =>
            {
                if (!IsWindowVisible(hWnd))
                    return true;

                var sb = new System.Text.StringBuilder(256);
                GetWindowText(hWnd, sb, sb.Capacity);

                string title = sb.ToString();

                if (title.Contains("Anarchy Online"))
                {
                    windows.Add(new WindowInfo
                    {
                        Handle = hWnd,
                        Title = title
                    });
                }

                return true;
            }, IntPtr.Zero);

            return windows;
        }
    }
}

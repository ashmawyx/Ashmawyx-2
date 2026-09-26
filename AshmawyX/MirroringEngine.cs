using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace AshmawyX
{
    public class MirroringEngine
    {
        // AO window handles
        private IntPtr mainWindow = IntPtr.Zero;
        private IntPtr mirrorWindow = IntPtr.Zero;

        // Random delay
        private double delayFrom = 0.0;
        private double delayTo = 0.0;
        private Random rng = new Random();

        // Mirrored keys (dynamic)
        private HashSet<int> mirroredKeys = new HashSet<int>();

        // Permanent blocked keys (never mirrored)
        private HashSet<int> permanentBlockedKeys = new HashSet<int>
        {
            0x57, // W
            0x41, // A
            0x53, // S
            0x44, // D
            0x58, // X
            0x5A, // Z
            0x43, // C
            0x20, // SPACE
            0x0D, // ENTER
            0x09, // TAB
            0xBF  // SLASH /
        };

        // Running state
        private bool running = false;

        // Event for LED pulse
        public event Action<int> OnMirroredKey;

        // WinAPI
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;

        public void SetWindows(IntPtr main, IntPtr mirror)
        {
            mainWindow = main;
            mirrorWindow = mirror;
        }

        public void SetRandomDelay(double from, double to)
        {
            delayFrom = from;
            delayTo = to;
        }

        public void UpdateMirroredKeys(System.Windows.Forms.ListBox.ObjectCollection items)
        {
            mirroredKeys.Clear();

            foreach (var item in items)
            {
                string key = item.ToString().Trim().ToUpper();
                int vk = KeyRoutingRules.StringToVK(key);
                if (vk != -1)
                    mirroredKeys.Add(vk);
            }
        }

        public void Start()
        {
            running = true;
        }

        public void Stop()
        {
            running = false;
        }

        public void Panic()
        {
            running = false;
            mirroredKeys.Clear();
        }

        public void ProcessKey(int vkCode)
        {
            if (!running)
                return;

            // Blocked keys
            if (permanentBlockedKeys.Contains(vkCode))
                return;

            // Not mirrored
            if (!mirroredKeys.Contains(vkCode))
                return;

            // Random delay
            double delay = 0.0;
            if (delayTo > delayFrom)
                delay = rng.NextDouble() * (delayTo - delayFrom) + delayFrom;

            if (delay > 0)
                Thread.Sleep((int)(delay * 1000));

            // Mirror key
            MirrorKey(vkCode);

            // LED pulse
            OnMirroredKey?.Invoke(vkCode);
        }

        private void MirrorKey(int vk)
        {
            if (mirrorWindow == IntPtr.Zero)
                return;

            PostMessage(mirrorWindow, WM_KEYDOWN, (IntPtr)vk, IntPtr.Zero);
            PostMessage(mirrorWindow, WM_KEYUP, (IntPtr)vk, IntPtr.Zero);
        }
    }
}

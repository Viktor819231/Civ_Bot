using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Gamebot
{
    public static class MouseControl
    {
        // Win32 API imports for mouse operations
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return $"X: {X}, Y: {Y}";
            }
        }

        /// <summary>
        /// Gets the current mouse position in screen coordinates (entire VM/display)
        /// </summary>
        public static POINT GetMousePosition()
        {
            GetCursorPos(out POINT point);
            return point;
        }

        /// <summary>
        /// Gets the current mouse position relative to a specific window
        /// This is what you want for LocationData coordinates!
        /// </summary>
        public static POINT GetMousePositionInWindow(IntPtr windowHandle)
        {
            GetCursorPos(out POINT point);
            ScreenToClient(windowHandle, ref point);
            return point;
        }

        /// <summary>
        /// Converts client coordinates to screen coordinates for a specific window
        /// </summary>
        public static POINT ClientToScreenCoordinates(IntPtr windowHandle, int x, int y)
        {
            POINT point = new POINT(x, y);
            ClientToScreen(windowHandle, ref point);
            return point;
        }

        /// <summary>
        /// Returns a formatted string with current mouse position
        /// </summary>
        public static string GetMousePositionString(bool includeWindowCoordinates = false, IntPtr windowHandle = default)
        {
            POINT screenPos = GetMousePosition();
            
            if (includeWindowCoordinates && windowHandle != IntPtr.Zero)
            {
                POINT windowPos = GetMousePositionInWindow(windowHandle);
                return $"Screen: ({screenPos.X}, {screenPos.Y}) | Window: ({windowPos.X}, {windowPos.Y})";
            }
            
            return $"Screen: ({screenPos.X}, {screenPos.Y})";
        }
    }
}

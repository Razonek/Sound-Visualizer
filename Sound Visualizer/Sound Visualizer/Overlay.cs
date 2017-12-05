using System;
using System.Runtime.InteropServices;


namespace Sound_Visualizer
{
    public static class Overlay
    {
                      
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        

        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        /// <summary>
        /// Set Window Transparent
        /// </summary>
        /// <param name="hwnd"> Window handle </param>
        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        /// <summary>
        /// Screen Height
        /// </summary>
        /// <returns> Screen Height as int</returns>
        public static int ScreenHeight()
        {
            return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;            
        }

        /// <summary>
        /// Screen Width
        /// </summary>
        /// <returns> Screen Width as int </returns>
        public static int ScreenWidth()
        {
            return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        }

    }
}

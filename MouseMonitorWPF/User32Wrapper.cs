/*
 *  User32Wrapper.cs
 *  
 *  Copyright 2013 - Holger Genth
 *  
 *  This file is part of MouseMonitorWPF.
 *
 *  MouseMonitorWPF is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  MouseMonitorWPF is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  Diese Datei ist Teil von MouseMonitorWPF.
 *
 *  MouseMonitorWPF ist Freie Software: Sie können es unter den Bedingungen
 *  der GNU General Public License, wie von der Free Software Foundation,
 *  Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren
 *  veröffentlichten Version, weiterverbreiten und/oder modifizieren.
 *
 *  MouseMonitorWPF wird in der Hoffnung, dass es nützlich sein wird, aber
 *  OHNE JEDE GEWÄHELEISTUNG, bereitgestellt; sogar ohne die implizite
 *  Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK.
 *  Siehe die GNU General Public License für weitere Details.
 *
 *  Sie sollten eine Kopie der GNU General Public License zusammen mit diesem
 *  Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
 */
using System;
using System.Runtime.InteropServices;
using System.Windows.Media;


public class User32Wrapper
{

    // Window Styles
    public const UInt32 WS_OVERLAPPED = 0;
    public const UInt32 WS_POPUP = 0x80000000;
    public const UInt32 WS_CHILD = 0x40000000;
    public const UInt32 WS_MINIMIZE = 0x20000000;
    public const UInt32 WS_VISIBLE = 0x10000000;
    public const UInt32 WS_DISABLED = 0x8000000;
    public const UInt32 WS_CLIPSIBLINGS = 0x4000000;
    public const UInt32 WS_CLIPCHILDREN = 0x2000000;
    public const UInt32 WS_MAXIMIZE = 0x1000000;
    public const UInt32 WS_CAPTION = 0xC00000;      // WS_BORDER or WS_DLGFRAME  
    public const UInt32 WS_BORDER = 0x800000;
    public const UInt32 WS_DLGFRAME = 0x400000;
    public const UInt32 WS_VSCROLL = 0x200000;
    public const UInt32 WS_HSCROLL = 0x100000;
    public const UInt32 WS_SYSMENU = 0x80000;
    public const UInt32 WS_THICKFRAME = 0x40000;
    public const UInt32 WS_GROUP = 0x20000;
    public const UInt32 WS_TABSTOP = 0x10000;
    public const UInt32 WS_MINIMIZEBOX = 0x20000;
    public const UInt32 WS_MAXIMIZEBOX = 0x10000;
    public const UInt32 WS_TILED = WS_OVERLAPPED;
    public const UInt32 WS_ICONIC = WS_MINIMIZE;
    public const UInt32 WS_SIZEBOX = WS_THICKFRAME;

    // Extended Window Styles
    public const UInt32 WS_EX_DLGMODALFRAME = 0x0001;
    public const UInt32 WS_EX_NOPARENTNOTIFY = 0x0004;
    public const UInt32 WS_EX_TOPMOST = 0x0008;
    public const UInt32 WS_EX_ACCEPTFILES = 0x0010;
    public const UInt32 WS_EX_TRANSPARENT = 0x0020;
    public const UInt32 WS_EX_MDICHILD = 0x0040;
    public const UInt32 WS_EX_TOOLWINDOW = 0x0080;
    public const UInt32 WS_EX_WINDOWEDGE = 0x0100;
    public const UInt32 WS_EX_CLIENTEDGE = 0x0200;
    public const UInt32 WS_EX_CONTEXTHELP = 0x0400;
    public const UInt32 WS_EX_RIGHT = 0x1000;
    public const UInt32 WS_EX_LEFT = 0x0000;
    public const UInt32 WS_EX_RTLREADING = 0x2000;
    public const UInt32 WS_EX_LTRREADING = 0x0000;
    public const UInt32 WS_EX_LEFTSCROLLBAR = 0x4000;
    public const UInt32 WS_EX_RIGHTSCROLLBAR = 0x0000;
    public const UInt32 WS_EX_CONTROLPARENT = 0x10000;
    public const UInt32 WS_EX_STATICEDGE = 0x20000;
    public const UInt32 WS_EX_APPWINDOW = 0x40000;
    public const UInt32 WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
    public const UInt32 WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
    public const UInt32 WS_EX_LAYERED = 0x00080000;
    public const UInt32 WS_EX_NOINHERITLAYOUT = 0x00100000; // Disable inheritence of mirroring by children
    public const UInt32 WS_EX_LAYOUTRTL = 0x00400000; // Right to left mirroring
    public const UInt32 WS_EX_COMPOSITED = 0x02000000;
    public const UInt32 WS_EX_NOACTIVATE = 0x08000000;

    public enum SystemMenu
    {
        Size = 0xF000,
        Close = 0xF060,
        Restore = 0xF120,
        Minimize = 0xF020,
        Maximize = 0xF030,
    }
    public enum WindowMessage
    {
        Destroy = 0x2,
        Close = 0x10,
        SetIcon = 0x80,
        MeasureItem = 0x2c,
        MouseMove = 0x200,
        MouseDown = 0x201,
        LButtonUp = 0x0202,
        LButtonDblClk = 0x0203,
        RButtonDown = 0x0204,
        RButtonUp = 0x0205,
        RButtonDblClk = 0x0206,
        MButtonDown = 0x0207,
        MButtonUp = 0x0208,
        MButtonDblClk = 0x0209,
        TrayMouseMessage = 0x800,
    }

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr IntGetWindowLongPtr(HandleRef hWnd, int nIndex);


    public static Int32 GWL_EXSTYLE = -20;
    public static Int32 GWLP_HINSTANCE = -6;
    public static Int32 GWLP_HWNDPARENT = -8;
    public static Int32 GWL_ID = -12;
    public static Int32 GWL_STYLE = -16;
    public static Int32 GWL_USERDATA = -21;
    public static Int32 GWL_WNDPROC = -4;
    public static Int32 DWLP_USER = 0x8;
    public static Int32 DWLP_MSGRESULT = 0x0;
    public static Int32 DWLP_DLGPROC = 0x4;
    
    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr IntSetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong);


    public UInt32 LWA_ColorKey = 1;
    public const UInt32 LWA_Alpha = 2;

    [DllImport("user32.dll")]
    public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern bool EnableMenuItem(HandleRef hMenu, SystemMenu UIDEnabledItem, int uEnable);
    public static void SetMenuItem(HandleRef hMenu, SystemMenu menu, bool isEnabled)
    {
        EnableMenuItem(hMenu, menu, (isEnabled) ? ~1 : 1);
    }

    [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetSystemMenu(HandleRef hWnd, bool bRevert);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern bool PostMessage(HandleRef hwnd, WindowMessage msg, IntPtr wparam, IntPtr lparam);

    [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern bool SetForegroundWindow(HandleRef hWnd);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int RegisterWindowMessage(string msg);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern bool DestroyIcon(IntPtr hIcon);

    #region TrayIcon
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class NOTIFYICONDATA
    {
        public int cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA));
        public IntPtr hWnd;
        public int uID;
        public NotifyIconFlags uFlags;
        public int uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
        public string szTip;
        public int dwState;
        public int dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x100)]
        public string szInfo;
        public int uTimeoutOrVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
        public string szInfoTitle;
        public int dwInfoFlags;
    }

    [Flags]
    public enum NotifyIconFlags
    {
        /// <summary>
        /// The hIcon member is valid.
        /// </summary>
        Icon = 2,
        /// <summary>
        /// The uCallbackMessage member is valid.
        /// </summary>
        Message = 1,
        /// <summary>
        /// The szTip member is valid.
        /// </summary>
        ToolTip = 4,
        /// <summary>
        /// The dwState and dwStateMask members are valid.
        /// </summary>
        State = 8,
        /// <summary>
        /// Use a balloon ToolTip instead of a standard ToolTip. The szInfo, uTimeout, szInfoTitle, and dwInfoFlags members are valid.
        /// </summary>
        Balloon = 0x10,
    }

    [DllImport("shell32", CharSet = CharSet.Auto)]
    public static extern int Shell_NotifyIcon(int message, NOTIFYICONDATA pnid);

    #endregion

    public static IntPtr GetHIcon(ImageSource source)
    {
        System.Windows.Media.Imaging.BitmapFrame frame = source as System.Windows.Media.Imaging.BitmapFrame;

        if (frame != null && frame.Decoder.Frames.Count > 0)
        {
            frame = frame.Decoder.Frames[0];

            int width = frame.PixelWidth;
            int height = frame.PixelHeight;
            int stride = width * ((frame.Format.BitsPerPixel + 7) / 8);

            byte[] bits = new byte[height * stride];

            frame.CopyPixels(bits, stride, 0);

            // pin the bytes in memory (avoids using unsafe context)
            GCHandle gcHandle = GCHandle.Alloc(bits, GCHandleType.Pinned);

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(
                width,
                height,
                stride,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb,
                gcHandle.AddrOfPinnedObject());

            IntPtr hIcon = bitmap.GetHicon();

            gcHandle.Free();

            return hIcon;
        }

        return IntPtr.Zero;
    }
}


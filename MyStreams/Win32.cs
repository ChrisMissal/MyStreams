using System;
using System.Runtime.InteropServices;

namespace MyStreams
{
	// ReSharper disable InconsistentNaming
	// ReSharper disable FieldCanBeMadeReadOnly.Local
	// ReSharper disable MemberCanBePrivate.Local
	internal static class Win32
	{
		public static readonly IntPtr HWND_TOP = new IntPtr(0);

		public const uint SWP_SHOWWINDOW = 0x0040;

		public const int INPUT_MOUSE = 0;
		public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
		public const uint MOUSEEVENTF_LEFTUP = 0x0004;

		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSEINPUT
		{
			public int dx;
			public int dy;
			public uint mouseData;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KEYBDINPUT
		{
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HARDWAREINPUT
		{
			uint uMsg;
			ushort wParamL;
			ushort wParamH;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct INPUT
		{
			[FieldOffset(0)]
			public int type;
			[FieldOffset(4)]
			public MOUSEINPUT mi;
			[FieldOffset(4)]
			public KEYBDINPUT ki;
			[FieldOffset(4)]
			HARDWAREINPUT hi;
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);
	}
	// ReSharper restore MemberCanBePrivate.Local
	// ReSharper restore FieldCanBeMadeReadOnly.Local
	// ReSharper restore InconsistentNaming
}
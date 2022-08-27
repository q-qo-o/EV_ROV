using System.Runtime.InteropServices;

namespace ROV
{
	public static class JoystickAPI
	{
		public struct JOYCAPS
		{
			public ushort wMid;

			public ushort wPid;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szPname;

			public int wXmin;

			public int wXmax;

			public int wYmin;

			public int wYmax;

			public int wZmin;

			public int wZmax;

			public int wNumButtons;

			public int wPeriodMin;

			public int wPeriodMax;

			public int wRmin;

			public int wRmax;

			public int wUmin;

			public int wUmax;

			public int wVmin;

			public int wVmax;

			public int wCaps;

			public int wMaxAxes;

			public int wNumAxes;

			public int wMaxButtons;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szRegKey;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szOEMVxD;
		}

		public struct JOYINFO
		{
			public int wXpos;

			public int wYpos;

			public int wZpos;

			public int wButtons;
		}

		public struct JOYINFOEX
		{
			public int dwSize;

			public int dwFlags;

			public int dwXpos;

			public int dwYpos;

			public int dwZpos;

			public int dwRpos;

			public int dwUpos;

			public int dwVpos;

			public int dwButtons;

			public int dwButtonNumber;

			public int dwPOV;

			public int dwReserved1;

			public int dwReserved2;
		}

		public const int JOYERR_NOERROR = 0;

		public const int JOYERR_PARMS = 165;

		public const int JOYERR_NOCANDO = 166;

		public const int JOYERR_UNPLUGGED = 167;

		public const int JOY_BUTTON1 = 1;

		public const int JOY_BUTTON2 = 2;

		public const int JOY_BUTTON3 = 4;

		public const int JOY_BUTTON4 = 8;

		public const int JOY_BUTTON5 = 16;

		public const int JOY_BUTTON6 = 32;

		public const int JOY_BUTTON7 = 64;

		public const int JOY_BUTTON8 = 128;

		public const int JOY_BUTTON9 = 256;

		public const int JOY_BUTTON10 = 512;

		public const int JOY_BUTTON1CHG = 256;

		public const int JOY_BUTTON2CHG = 512;

		public const int JOY_BUTTON3CHG = 1024;

		public const int JOY_BUTTON4CHG = 2048;

		public const int JOYSTICKID1 = 0;

		public const int JOYSTICKID2 = 1;

		public const long JOY_RETURNX = 1L;

		public const long JOY_RETURNY = 2L;

		public const long JOY_RETURNZ = 4L;

		public const long JOY_RETURNR = 8L;

		public const long JOY_RETURNU = 16L;

		public const long JOY_RETURNV = 32L;

		public const long JOY_RETURNPOV = 64L;

		public const long JOY_RETURNBUTTONS = 128L;

		public const long JOY_RETURNRAWDATA = 256L;

		public const long JOY_RETURNPOVCTS = 512L;

		public const long JOY_RETURNCENTERED = 1024L;

		public const long JOY_USEDEADZONE = 2048L;

		public const long JOY_RETURNALL = 255L;

		public const long JOY_CAL_READALWAYS = 65536L;

		public const long JOY_CAL_READRONLY = 33554432L;

		public const long JOY_CAL_READ3 = 262144L;

		public const long JOY_CAL_READ4 = 524288L;

		public const long JOY_CAL_READXONLY = 1048576L;

		public const long JOY_CAL_READYONLY = 2097152L;

		public const long JOY_CAL_READ5 = 4194304L;

		public const long JOY_CAL_READ6 = 8388608L;

		public const long JOY_CAL_READZONLY = 16777216L;

		public const long JOY_CAL_READUONLY = 67108864L;

		public const long JOY_CAL_READVONLY = 134217728L;

		[DllImport("winmm.dll")]
		public static extern int joyGetNumDevs();

		[DllImport("winmm.dll")]
		public static extern int joyGetDevCaps(int uJoyID, ref JOYCAPS pjc, int cbjc);

		[DllImport("winmm.dll")]
		public static extern int joyGetPos(int uJoyID, ref JOYINFO pji);

		[DllImport("winmm.dll")]
		public static extern int joyGetPosEx(int uJoyID, ref JOYINFOEX pji);
	}
}

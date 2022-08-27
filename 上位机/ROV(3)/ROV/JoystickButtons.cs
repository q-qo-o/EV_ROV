using System;

namespace ROV
{
	[Flags]
	public enum JoystickButtons
	{
		None = 0,
		UP = 1,
		Down = 2,
		Left = 4,
		Right = 8,
		B1 = 0x10,
		B2 = 0x20,
		B3 = 0x40,
		B4 = 0x80,
		B5 = 0x100,
		B6 = 0x200,
		B7 = 0x400,
		B8 = 0x800,
		B9 = 0x1000,
		B10 = 0x2000
	}
}

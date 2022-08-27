using System;

namespace ROV
{
	public class JoystickEventArgs : EventArgs
	{
		public int JoystickId { get; private set; }

		public JoystickButtons Buttons { get; private set; }

		public JoystickEventArgs(int joystickId, JoystickButtons buttons)
		{
			JoystickId = joystickId;
			Buttons = buttons;
		}
	}
}

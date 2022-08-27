using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ROV
{
	public class Joystick : IDisposable
	{
		private string strResult = string.Empty;

		private JoystickAPI.JOYCAPS JoystickCAPS;

		private JoystickHandle JoyHandle;

		private Timer CaptureTimer;

		private JoystickButtons PreviousButtons = JoystickButtons.None;

		public int Id { get; private set; }

		public string Name { get; private set; }

		public bool IsConnected { get; private set; }

		public bool IsCapture { get; set; }

		public event EventHandler<JoystickEventArgs> Click;

		public event EventHandler<JoystickEventArgs> ButtonDown;

		public event EventHandler<JoystickEventArgs> ButtonUp;

		public Joystick(int joystickId)
		{
			Id = joystickId;
			JoystickCAPS = default(JoystickAPI.JOYCAPS);
			if (JoystickAPI.joyGetDevCaps(joystickId, ref JoystickCAPS, Marshal.SizeOf(typeof(JoystickAPI.JOYCAPS))) == 0)
			{
				IsConnected = true;
				Name = JoystickCAPS.szPname;
			}
			else
			{
				IsConnected = false;
			}
		}

		protected void OnClick(JoystickEventArgs e)
		{
			this.Click?.Invoke(this, e);
		}

		protected void OnButtonUp(JoystickEventArgs e)
		{
			this.ButtonUp?.Invoke(this, e);
		}

		protected void OnButtonDown(JoystickEventArgs e)
		{
			this.ButtonDown?.Invoke(this, e);
		}

		public void Capture()
		{
			if (IsConnected && !IsCapture)
			{
				IsCapture = true;
			}
		}

		public void ReleaseCapture()
		{
			if (IsCapture)
			{
				CaptureTimer.Dispose();
				CaptureTimer = null;
				IsCapture = false;
			}
		}

		public JoystickHandle OnTimerCallback()
		{
			JoyHandle = new JoystickHandle();
			JoystickAPI.JOYINFOEX infoEx = default(JoystickAPI.JOYINFOEX);
			infoEx.dwSize = Marshal.SizeOf(typeof(JoystickAPI.JOYINFOEX));
			infoEx.dwFlags = 128;
			int result = JoystickAPI.joyGetPosEx(Id, ref infoEx);
			JoystickButtons Handle_Btton = GetButtons(infoEx.dwButtons);
			JoyHandle.Left_Upper = false;
			JoyHandle.Right_Upper = false;
			JoyHandle.Left_Lower = false;
			JoyHandle.Right_Lower = false;
			if (result == 0)
			{
				switch (Handle_Btton)
				{
				case JoystickButtons.B5:
					JoyHandle.Left_Upper = true;
					break;
				case JoystickButtons.B6:
					JoyHandle.Right_Upper = true;
					break;
				case JoystickButtons.B7:
					JoyHandle.Left_Lower = true;
					break;
				case JoystickButtons.B8:
					JoyHandle.Right_Lower = true;
					break;
				}
				JoyHandle.Xpos = infoEx.dwXpos;
				JoyHandle.Ypos = infoEx.dwYpos;
				JoyHandle.Zpos = infoEx.dwZpos;
				JoyHandle.Rpos = infoEx.dwRpos;
			}
			return JoyHandle;
		}

		private JoystickButtons GetButtons(int dwButtons)
		{
			JoystickButtons buttons = JoystickButtons.None;
			if ((dwButtons & 1) == 1)
			{
				buttons |= JoystickButtons.B1;
			}
			if ((dwButtons & 2) == 2)
			{
				buttons |= JoystickButtons.B2;
			}
			if ((dwButtons & 4) == 4)
			{
				buttons |= JoystickButtons.B3;
			}
			if ((dwButtons & 8) == 8)
			{
				buttons |= JoystickButtons.B4;
			}
			if ((dwButtons & 0x10) == 16)
			{
				buttons |= JoystickButtons.B5;
			}
			if ((dwButtons & 0x20) == 32)
			{
				buttons |= JoystickButtons.B6;
			}
			if ((dwButtons & 0x40) == 64)
			{
				buttons |= JoystickButtons.B7;
			}
			if ((dwButtons & 0x80) == 128)
			{
				buttons |= JoystickButtons.B8;
			}
			if ((dwButtons & 0x100) == 256)
			{
				buttons |= JoystickButtons.B9;
			}
			if ((dwButtons & 0x200) == 512)
			{
				buttons |= JoystickButtons.B10;
			}
			return buttons;
		}

		private void GetXYButtons(int x, int y, ref JoystickButtons buttons)
		{
			int i = 32767;
			if (x - i > 256)
			{
				buttons |= JoystickButtons.Right;
			}
			else if (i - x > 256)
			{
				buttons |= JoystickButtons.Left;
			}
			if (y - i > 256)
			{
				buttons |= JoystickButtons.Down;
			}
			else if (i - y > 256)
			{
				buttons |= JoystickButtons.UP;
			}
		}

		public void Dispose()
		{
			ReleaseCapture();
			CaptureTimer = null;
		}
	}
}

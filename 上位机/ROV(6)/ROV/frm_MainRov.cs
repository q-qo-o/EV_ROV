using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using SmileWei.EmbeddedApp;

namespace ROV
{
	public class frm_MainRov : Form
	{
		bool trunmethod;

		private const int DeadZone_R = 10;

		private const int DeadZone_Z = 10;

		private const int MiddleRange = 32767;

		private static Socket sokClient;

		private Thread threadClient;

		private Joystick joy;

		private JosystickSend sends;

		private int isConModel;

		private bool isCon;

		private ArrayList alistData;

		private bool b_Lighting;

		private bool b_Switchgear;

		private int i_Up_Down;

		private int i_Left_Right_Turn;

		private int i_Floating_Dive;

		private int i_Left_Right_Shift;

		private int i_Depth;

		private int i_CatchOrLaser;

		private int i_Lighting;

		private int i_Laser;

		private IContainer components = null;

		private GroupBox grp_RobotState;

		private Label lbl_Right_Cabin_Temperature;

		private Label lbl_InCabin;

		private Label lbl_Depth;

		private PictureBox picControlModule;

		private Label lbl_TDepth;

		private Label label3;

		private AttitudeIndicatorInstrumentControl Roll_Pitch_Angle;

		private HeadingIndicatorInstrumentControl lbl_Heading_Angle_Number;

		private GroupBox grp_Operation;

		private ComboBox cmb_Type;

		private Button btn_ConHandle;

		private Label lblHandle;

		private GroupBox groupBox2;

		private TextBox txt_ConRobot;

		private Button btn_ConRobot;

		private Label label4;

		private TextBox txt_Port;

		private Label label5;

		private GroupBox grpType;

		private RadioButton rdoOpenLoop;

		private RadioButton rdo_Deep;

		private RadioButton rdo_Depth_Orientation;

		private GroupBox grp_MotionControlOperation;

		private Label lbl_Lighting;

		private Label lbl_Middle_WS;

		private GroupBox grp_Handle;

		private VScrollBar vscR;

		private Label lblRightShift;

		private HScrollBar hscZ;

		private VScrollBar vscY;

		private Label lbl_Laser;

		private Label lbl_Course;

		private HScrollBar hscX;

		private Label lbl_Forward;

		private Label lbl_TurnLeft;

		private Label lbl_TurnRight;

		private Label lbl_BackOff;

		private Label lbl_Floating;

		private Label lbl_Dive;

		private Label lbl_LeftShift;

		private Label lbl_RightShift;

		private System.Windows.Forms.Timer tmr_Handle;
        private Label label6;
        private Label label2;
        private Label label1;
        private TextBox Direc_Value_Z_TextBox;
        private TextBox UpDown_Value_Y_TextBox;
        private TextBox Forward_Back_Value_R_TextBox;
        private Label label8;
        private Label label7;
        private TextBox TurnMethod_TextBox;
        private Label TurnMethod;
        private System.Windows.Forms.Timer tmr_Automatic;

		public frm_MainRov()
		{
			InitializeComponent();
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
			{
				return false;
			}
			return base.ProcessDialogKey(keyData);
		}

		private void frm_MainRov_Load(object sender, EventArgs e)
		{
			base.KeyDown += frm_MainRov_KeyDown;
			base.KeyUp += frm_MainRov_KeyUp;
			trunmethod = true;
			init();
		}

		private void frm_MainRov_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
			case Keys.W:
				set_Forward();
				break;
			case Keys.S:
				set_BackOff();
				break;
			case Keys.A:
				set_TurnLeft();
				break;
			case Keys.D:
				set_TurnRight();
				break;
			case Keys.I:
				set_Floating();
				break;
			case Keys.K:
				set_Dive();
				break;
			case Keys.Q:
				set_LeftShift();
				break;
			case Keys.E:
				set_RightShift();
				break;
			case Keys.O:
				set_OpenOrCloseLinghting();
				break;
			case Keys.B:
				set_All_Middle();
				break;
			case Keys.J:
				set_Course_Laser();
				break;
			case Keys.L:
				set_Laser_Course();
				break;
			case Keys.C:
			case Keys.F:
			case Keys.G:
			case Keys.H:
			case Keys.M:
			case Keys.N:
			case Keys.P:
			case Keys.R:
			case Keys.T:
			case Keys.U:
			case Keys.V:
				break;
			}
		}

		private void frm_MainRov_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
			case Keys.S:
			case Keys.W:
				set_Middle_WS();
				break;
			case Keys.A:
			case Keys.D:
				set_Middle_AD();
				break;
			case Keys.I:
			case Keys.K:
				set_Middle_IK();
				break;
			case Keys.E:
			case Keys.Q:
				set_Middle_QE();
				break;
			}
		}

		private void set_Middle_WS()
		{
			i_Up_Down = 100;
			set_LableBackColor(lbl_Forward, lbl_BackOff, "IDENTICAL");
			set_Send_Data_ROV();
		}

		private void set_Middle_AD()
		{
			i_Left_Right_Turn = 100;
			set_LableBackColor(lbl_TurnLeft, lbl_TurnRight, "IDENTICAL");
			set_Send_Data_ROV();
		}

		private void set_Middle_IK()
		{
			i_Floating_Dive = 100;
			set_LableBackColor(lbl_Floating, lbl_Dive, "IDENTICAL");
			set_Send_Data_ROV();
		}

		private void set_Middle_QE()
		{
			i_Left_Right_Shift = 100;
			set_LableBackColor(lbl_LeftShift, lbl_RightShift, "IDENTICAL");
			set_Send_Data_ROV();
		}

		private void set_All_Middle()
		{
			set_Middle_WS();
			set_Middle_AD();
			set_Middle_IK();
			set_Middle_QE();
		}

		private void init()
		{
			try
			{
				string str_Path = Assembly.GetExecutingAssembly().Location;
				str_Path = str_Path.Substring(0, str_Path.LastIndexOf('\\'));
				str_Path += "\\Camera\\NetCameraVideoRead.exe";
				if (File.Exists(str_Path))
				{
				}
				else
				{
					MessageBox.Show("开启操控前，请确认当前设备已安装！");
				}
			}
			catch (Exception)
			{
			}
			isConModel = 0;
			setShape(picControlModule, Color.GreenYellow);
			List<string> list = new List<string>();
			list.Add("手柄操作");
			list.Add("键鼠控制");
			list.Add("调试");
			cmb_Type.DataSource = list;
			setColor();
		}

		private void setColor()
		{
			foreach (Control c2 in grp_MotionControlOperation.Controls)
			{
				c2.BackColor = Color.Silver;
			}
			foreach (Control c in grp_Handle.Controls)
			{
				c.BackColor = Color.Silver;
			}
			i_Up_Down = 100;
			i_Left_Right_Turn = 100;
			i_Floating_Dive = 100;
			i_Left_Right_Shift = 100;
		}

		private void setShape(PictureBox pic, Color color)
		{
			GraphicsPath gp = new GraphicsPath();
			gp.AddEllipse(pic.ClientRectangle);
			Region region3 = (pic.Region = new Region(gp));
			Region region = region3;
			gp.Dispose();
			region.Dispose();
			pic.BackColor = color;
		}

		private void cmb_Type_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmb_Type.Text.ToString() == "调试")
			{
				frm_DebugMode f = new frm_DebugMode();
				f.Send_InfoItems += SendPidParameter;
				f.ShowDialog();
			}
		}

		private void SendPidParameter(List<string> str_List)
		{
			byte[] theData = new byte[40];
			int index = 0;
			theData[index++] = 254;
			theData[index++] = 254;
			theData[index++] = 3;
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[0]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[1]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[2]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[3]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[4]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[5]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[6]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[7]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[8]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[9]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[10]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[11]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[12]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[13]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[14]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[15]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[16]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[17]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[18]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[19]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[20]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[21]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[22]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[23]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[24]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[25]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[26]));
			theData[index++] = Convert.ToByte(Convert.ToInt32(str_List[27]));
			theData[index++] = Convert.ToByte(str_List[28]);
			for (int i = 0; i < 5; i++)
			{
				theData[index++] = 0;
			}
			byte sum = theData[0];
			for (int j = 1; j <= index - 1; j++)
			{
				sum = Convert.ToByte(sum ^ theData[j]);
			}
			theData[index++] = sum;
			theData[index++] = 253;
			theData[index++] = 253;
			NetSendData(theData);
		}

		private void btn_ConRobot_Click(object sender, EventArgs e)
		{
			try
			{
				string type = btn_ConRobot.Text;
				if (type == "开启")
				{
					sokClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					IPAddress address = IPAddress.Parse(txt_ConRobot.Text.Trim());
					IPEndPoint endpoint = new IPEndPoint(address, int.Parse(txt_Port.Text.Trim()));
					sokClient.Connect(endpoint);
					threadClient = new Thread(ReceiveMsg);
					threadClient.IsBackground = true;
					threadClient.SetApartmentState(ApartmentState.STA);
					threadClient.Start();
					btn_ConRobot.Text = "关闭";
					txt_ConRobot.Enabled = false;
					txt_Port.Enabled = false;
					grp_Operation.Enabled = true;
				}
				else if (isCon)
				{
					MessageBox.Show("请先停止设备链接！");
				}
				else
				{
					tmr_Handle.Enabled = false;
					tmr_Automatic.Enabled = false;
					grp_Operation.Enabled = false;
					btn_ConRobot.Text = "开启";
					txt_ConRobot.Enabled = true;
					txt_Port.Enabled = true;
				}
			}
			catch (Exception)
			{
				MessageBox.Show("请确认网络通信正常！");
			}
		}

		private void ReceiveMsg()
		{
			try
			{
				while (true)
				{
					bool flag = true;
					byte[] msgArr = new byte[4096];
					if (msgArr[0] != 0)
					{
						continue;
					}
					int len = sokClient.Receive(msgArr);
					if (len == 0)
					{
						break;
					}
					if (len < 24 || !(btn_ConRobot.Text == "关闭") || msgArr[0] != 254 || msgArr[1] != 254 || msgArr[22] != 253 || msgArr[23] != 253)
					{
						continue;
					}
					byte sum = msgArr[0];
					for (int i = 1; i <= len - 4; i++)
					{
						sum = Convert.ToByte(sum ^ msgArr[i]);
					}
					if (sum != msgArr[21])
					{
						continue;
					}
					Invoke((EventHandler)delegate
					{
						double aircraftRollAngle = ((msgArr[2] != 0) ? ((double)(-1 * (msgArr[3] * 256 + msgArr[4])) / 100.0) : ((double)(msgArr[3] * 256 + msgArr[4]) / 100.0));
						double aircraftPitchAngle = ((msgArr[5] != 0) ? ((double)(-1 * (msgArr[6] * 256 + msgArr[7])) / 100.0) : ((double)(msgArr[6] * 256 + msgArr[7]) / 100.0));
						if (msgArr[8] == 0)
						{
							double num = (double)(msgArr[9] * 256 + msgArr[10]) / 100.0;
						}
						else
						{
							double num2 = (double)(-1 * (msgArr[9] * 256 + msgArr[10])) / 100.0;
						}
						Roll_Pitch_Angle.SetAttitudeIndicatorParameters(aircraftPitchAngle, aircraftRollAngle);
						float num3 = (float)(msgArr[11] * 256 + msgArr[12]) / 100f;
						lbl_Depth.Text = num3.ToString("0.00") + "m";
						float num4 = (float)(msgArr[13] * 256 + msgArr[14]) / 100f;
						if (num4 > 50f)
						{
							lbl_Right_Cabin_Temperature.ForeColor = Color.Red;
						}
						else
						{
							lbl_Right_Cabin_Temperature.ForeColor = Color.Black;
						}
						lbl_Right_Cabin_Temperature.Text = num4.ToString("0.00") + "°C";
						int[] array = Get_Warning_Info(msgArr[15], 8);
						string empty = string.Empty;
						int[] array2 = array;
						int[] array3 = array2;
						foreach (int num5 in array3)
						{
							if (num5 == 1)
							{
								picControlModule.BackColor = Color.Red;
								empty = "漏水";
							}
							else
							{
								picControlModule.BackColor = Color.GreenYellow;
								empty = "正常";
							}
						}
					});
				}
			}
			catch (Exception)
			{
				MessageBox.Show("设备断开连接!");
			}
		}

		private int[] Get_Warning_Info(byte b_Alarm, int i_Length)
		{
			string str_Info = string.Empty;
			int[] i_Value = new int[i_Length];
			int index = 0;
			if ((b_Alarm & 1) == 1)
			{
				i_Value[index++] = 1;
			}
			else
			{
				i_Value[index++] = 0;
			}
			if ((b_Alarm & 2) == 2)
			{
				i_Value[index++] = 1;
			}
			else
			{
				i_Value[index++] = 0;
			}
			if ((b_Alarm & 4) == 4)
			{
				i_Value[index++] = 1;
			}
			else
			{
				i_Value[index++] = 0;
			}
			if ((b_Alarm & 8) == 8)
			{
				i_Value[index++] = 1;
			}
			else
			{
				i_Value[index++] = 0;
			}
			if ((b_Alarm & 0x10) == 16)
			{
				i_Value[index++] = 1;
			}
			else
			{
				i_Value[index++] = 0;
			}
			if ((b_Alarm & 0x20) == 32)
			{
				i_Value[index++] = 1;
			}
			else
			{
				i_Value[index++] = 0;
			}
			if ((b_Alarm & 0x40) == 64)
			{
				i_Value[index++] = 1;
			}
			else
			{
				i_Value[index++] = 0;
			}
			if ((b_Alarm & 0x80) == 128)
			{
				i_Value[index++] = 1;
			}
			else
			{
				i_Value[index++] = 0;
			}
			return i_Value;
		}

		private bool CheckDataAccuracy(byte[] data)
		{
			int Len = data.Length - 1;
			byte checkSum = data[0];
			for (int i = 1; i < Len; i++)
			{
				checkSum = Convert.ToByte(checkSum ^ data[i]);
			}
			if (data[Len] == checkSum)
			{
				return true;
			}
			return false;
		}

		private float GetAttitudeData(byte[] body, int index)
		{
			float signedData = 0f;
			int temp_value1 = 0;
			int temp_value2 = 0;
			int temp_value3 = 0;
			temp_value1 = body[index];
			temp_value2 = body[index + 1] * 256;
			temp_value3 = body[index + 2];
			if (temp_value1 == 1)
			{
				return -1f * (float)((double)(temp_value2 + temp_value3) / 100.0);
			}
			return (float)((double)(temp_value2 + temp_value3) / 100.0);
		}

		private void NetSendData(byte[] data)
		{
			try
			{
				sokClient.Send(data);
			}
			catch (Exception)
			{
			}
		}

		private void tmr_Handle_Tick(object sender, EventArgs e)
		{
			JoystickHandle handle = joy.OnTimerCallback();
			vscY.Value = Convert.ToInt32(handle.Ypos / 327);
			hscX.Value = Convert.ToInt32(handle.Xpos / 327);
			vscR.Value = Convert.ToInt32(handle.Rpos / 327);
			hscZ.Value = Convert.ToInt32(handle.Zpos / 327);

			if (vscY.Value == 0 && hscX.Value == 0 && vscR.Value == 0 && hscZ.Value == 0)
			{
				vscY.Value = 100;
				hscX.Value = 100;
				vscR.Value = 100;
				hscZ.Value = 100;
			}

			/*----调整:R1抓握  R2松开----*/
			if (handle.Left_Upper)
			{
				i_CatchOrLaser = 0;
				//set_LableBackColor(lbl_Release, lbl_Catch, "DIFFERENCE");
			}
			else if (handle.Right_Upper)
			{
				i_CatchOrLaser = 1;
				//set_LableBackColor(lbl_Catch, lbl_Release, "DIFFERENCE");
			}
			/*--------------------------*/

			if (handle.Left_Lower)
			{
				b_Lighting = true;
				//set_LableBackColor(lbl_Lighting_Open, lbl_Lighting_Close, "DIFFERENCE");
			}
			else if (handle.Right_Lower)
			{
				b_Lighting = false;
				//set_LableBackColor(lbl_Lighting_Close, lbl_Lighting_Open, "DIFFERENCE");
			}


			sends = new JosystickSend();


			sends.UpDown_Value_Y = vscY.Value;//上下控制


			sends.Forward_Back_Value_R = 100;

			sends.Direc_Value_Z = 100;

			if (hscZ.Value <= 98 || hscZ.Value >= 102 || vscR.Value <= 98 || vscR.Value >= 102)
			{
				sends.Direc_Value_Z = hscZ.Value;
				sends.Forward_Back_Value_R = vscR.Value;

				/*if (ABS(sends.Direc_Value_Z - 100) > ABS(sends.Forward_Back_Value_R - 100))
				{
					sends.Forward_Back_Value_R = 100;
				}
				else
				{
					sends.Direc_Value_Z = 100;
				}*/
			}

			//对手柄方向的调整
			sends.Forward_Back_Value_R = 200 - sends.Forward_Back_Value_R;//前进
			sends.UpDown_Value_Y = 200 - sends.UpDown_Value_Y;//上下

			/*************对按键位置调整*************/
			//左侧方向和前进，右侧上下
			int Forward_Back_Value_R = sends.UpDown_Value_Y;
			int Direc_Value_Z = RL_priority(hscX.Value, hscZ.Value);
			int UpDown_Value_Y = sends.Forward_Back_Value_R;

			sends.Forward_Back_Value_R = Forward_Back_Value_R;
			sends.Side_Shift_Value_X = 100; //取消横滚
			sends.Direc_Value_Z =  Direc_Value_Z; //左右反向
			sends.UpDown_Value_Y = UpDown_Value_Y;
			/****************************************/

			/*************遥杆灵敏度曲线*************/
			sends.Forward_Back_Value_R = RockerCurve(sends.Forward_Back_Value_R);
			sends.Side_Shift_Value_X = RockerCurve(sends.Side_Shift_Value_X);
			sends.Direc_Value_Z = RockerCurve(sends.Direc_Value_Z);
			sends.UpDown_Value_Y = RockerCurve(sends.UpDown_Value_Y);
			/****************************************/

			//显示推力大小
			Forward_Back_Value_R_TextBox.Text = Convert.ToString(sends.Forward_Back_Value_R - 100) + "%";
			Direc_Value_Z_TextBox.Text = Convert.ToString(sends.Direc_Value_Z - 100) + "%";
			UpDown_Value_Y_TextBox.Text = Convert.ToString(sends.UpDown_Value_Y - 100) + "%";

			byte[] sendDate = SendTransmitData(sends, "CONHANDLE");
			NetSendData(sendDate);
		}

		int RockerCurve(int Linear_Input)
		{  //二次函数灵敏度曲线
			int Curve_Output = 100;

			if (Linear_Input > 100)
			{
				Curve_Output = (int)(0.01 * Linear_Input * Linear_Input - 2 * Linear_Input + 200);
			}
			if (Linear_Input < 100)
			{
				Curve_Output = (int)(-0.01 * Linear_Input * Linear_Input + 2 * Linear_Input);
			}
			return Curve_Output;
		}

		//左右摇杆优先级算法
		int RL_priority(int l_input , int r_input)
		{
			int priority = 100;

			if (trunmethod)
			{
				priority = l_input;
			}
			else
			{
				priority = r_input;
			}
			return priority;
		}

		public int ABS(int input)
		{
			if (input < 0)
			{
				input = -input;
			}
			return input;
		}

		public byte[] SendTransmitData(JosystickSend SendMotorData, string str_Type)
		{
			byte[] theData = new byte[40];
			int index = 0;
			theData[index++] = 254;
			theData[index++] = 254;
			theData[index++] = Convert.ToByte(isConModel);
			if ("CONHANDLE" == str_Type)
			{
				theData[index++] = Convert.ToByte(SendMotorData.Forward_Back_Value_R);
				theData[index++] = Convert.ToByte(SendMotorData.Direc_Value_Z);
				theData[index++] = Convert.ToByte(SendMotorData.Side_Shift_Value_X);
				theData[index++] = Convert.ToByte(SendMotorData.UpDown_Value_Y);
			}
			else
			{
				theData[index++] = Convert.ToByte(i_Up_Down);
				theData[index++] = Convert.ToByte(i_Left_Right_Turn);
				theData[index++] = Convert.ToByte(i_Left_Right_Shift);
				theData[index++] = Convert.ToByte(i_Floating_Dive);
			}
			theData[index++] = Convert.ToByte(i_CatchOrLaser);
			theData[index++] = Convert.ToByte(i_CatchOrLaser);
			theData[index++] = Convert.ToByte(i_CatchOrLaser);
			theData[index++] = Convert.ToByte(i_CatchOrLaser);
			theData[index++] = Convert.ToByte(b_Lighting);
			theData[index++] = Convert.ToByte(b_Lighting);
			for (int i = 0; i < 24; i++)
			{
				theData[index++] = 0;
			}
			byte sum = theData[0];
			for (int j = 1; j <= index - 1; j++)
			{
				sum = Convert.ToByte(sum ^ theData[j]);
			}
			theData[index++] = sum;
			theData[index++] = 253;
			theData[index++] = 253;
			return theData;
		}

		private void btn_ConHandle_Click(object sender, EventArgs e)
		{
			try
			{
				string type = btn_ConHandle.Text;
				if (type == "连 接")
				{
					if (cmb_Type.Text == "手柄操作")
					{
						tmr_Automatic.Enabled = false;
						joy = new Joystick(0);
						joy.Capture();
						if (!joy.IsCapture)
						{
							MessageBox.Show("手柄未找到");
							return;
						}
						grp_MotionControlOperation.Enabled = false;
						tmr_Handle.Enabled = true;
					}
					else if (cmb_Type.Text == "键鼠控制")
					{
						grp_MotionControlOperation.Enabled = true;
						tmr_Automatic.Enabled = true;
						tmr_Handle.Enabled = false;
					}
					btn_ConHandle.Text = "重 置";
					cmb_Type.Enabled = false;
					isCon = true;
				}
				else
				{
					isCon = false;
					btn_ConHandle.Text = "连 接";
					cmb_Type.Enabled = true;
					setColor();
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void lbl_LeftShift_Click(object sender, EventArgs e)
		{
			set_LeftShift();
		}

		private void lbl_RightShift_Click(object sender, EventArgs e)
		{
			set_RightShift();
		}

		private void lbl_TurnLeft_Click(object sender, EventArgs e)
		{
			set_TurnLeft();
		}

		private void lbl_TurnRight_Click(object sender, EventArgs e)
		{
			set_TurnRight();
		}

		private void lbl_Forward_Click(object sender, EventArgs e)
		{
			set_Forward();
		}

		private void lbl_BackOff_Click(object sender, EventArgs e)
		{
			set_BackOff();
		}

		private void lbl_Floating_Click(object sender, EventArgs e)
		{
			set_Floating();
		}

		private void lbl_Dive_Click(object sender, EventArgs e)
		{
			set_Dive();
		}

		private void lbl_Lighting_Click(object sender, EventArgs e)
		{
			set_OpenOrCloseLinghting();
		}

		private void lbl_Middle_WS_Click(object sender, EventArgs e)
		{
			set_All_Middle();
		}

		private void lbl_Course_Click(object sender, EventArgs e)
		{
			set_Course_Laser();
		}

		private void lbl_Laser_Click(object sender, EventArgs e)
		{
			set_Laser_Course();
		}

		private void set_Course_Laser()
		{
			i_CatchOrLaser = 1;
			set_LableBackColor(lbl_Course, lbl_Laser, "DIFFERENCE");
			set_Send_Data_ROV();
		}

		private void set_Laser_Course()
		{
			i_CatchOrLaser = 0;
			set_LableBackColor(lbl_Laser, lbl_Course, "DIFFERENCE");
			set_Send_Data_ROV();
		}

		private void set_LeftShift()
		{
			i_Left_Right_Shift = 0;
			set_LableBackColor(lbl_LeftShift, lbl_RightShift, "DIFFERENCE");
			set_Send_Data_ROV();
		}

		private void set_RightShift()
		{
			i_Left_Right_Shift = 200;
			set_LableBackColor(lbl_RightShift, lbl_LeftShift, "DIFFERENCE");
			set_Send_Data_ROV();
		}

		private void set_Forward()
		{
			set_MiddleValue();
			i_Up_Down = 200;
			set_LableBackColor(lbl_Forward, lbl_BackOff, "DIFFERENCE");
			set_Send_Data_ROV();
		}

		private void set_TurnLeft()
		{
			i_Left_Right_Turn = 0;
			set_LableBackColor(lbl_TurnLeft, lbl_TurnRight, "DIFFERENCE");
			set_Send_Data_ROV();
		}

		private void set_BackOff()
		{
			set_MiddleValue();
			i_Up_Down = 0;
			set_LableBackColor(lbl_BackOff, lbl_Forward, "DIFFERENCE");
			set_Send_Data_ROV();
		}

		private void set_TurnRight()
		{
			i_Left_Right_Turn = 200;
			set_LableBackColor(lbl_TurnRight, lbl_TurnLeft, "DIFFERENCE");
			set_Send_Data_ROV();
		}

		private void set_Floating()
		{
			i_Floating_Dive = 200;
			set_LableBackColor(lbl_Floating, lbl_Dive, "DIFFERENCE");
			set_Send_Data_ROV();
		}

		private void set_Dive()
		{
			i_Floating_Dive = 0;
			set_LableBackColor(lbl_Dive, lbl_Floating, "DIFFERENCE");
			set_Send_Data_ROV();
		}

		private void set_LableBackColor(Label lbl_1, Label lbl_2, string str_Type)
		{
			if (str_Type == "IDENTICAL")
			{
				lbl_1.BackColor = Color.Silver;
				lbl_2.BackColor = Color.Silver;
			}
			else
			{
				lbl_1.BackColor = Color.DodgerBlue;
				lbl_2.BackColor = Color.Silver;
			}
		}

		private void set_MiddleValue()
		{
			set_LableBackColor(lbl_TurnLeft, lbl_TurnRight, "IDENTICAL");
			set_LableBackColor(lbl_LeftShift, lbl_RightShift, "IDENTICAL");
		}

		private void set_OpenOrCloseLinghting()
		{
			if (!b_Lighting)
			{
				lbl_Lighting.BackColor = Color.DodgerBlue;
				b_Lighting = true;
				i_Lighting = 1;
			}
			else
			{
				lbl_Lighting.BackColor = Color.Silver;
				b_Lighting = false;
				i_Lighting = 0;
			}
			set_Send_Data_ROV();
		}

		private void tmr_Automatic_Tick(object sender, EventArgs e)
		{
			set_Send_Data_ROV();
		}

		private void set_Send_Data_ROV()
		{
			byte[] sendDate = SendTransmitData(null, null);
			NetSendData(sendDate);
		}

		private void rdoOpenLoop_CheckedChanged(object sender, EventArgs e)
		{
			if (rdoOpenLoop.Checked)
			{
				isConModel = 0;
			}
		}

		private void rdo_Depth_Orientation_CheckedChanged(object sender, EventArgs e)
		{
			if (rdo_Depth_Orientation.Checked)
			{
				isConModel = 1;
			}
		}

		private void rdo_Deep_CheckedChanged(object sender, EventArgs e)
		{
			if (rdo_Deep.Checked)
			{
				isConModel = 2;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_MainRov));
            this.grp_RobotState = new System.Windows.Forms.GroupBox();
            this.Roll_Pitch_Angle = new ROV.AttitudeIndicatorInstrumentControl();
            this.lbl_Heading_Angle_Number = new ROV.HeadingIndicatorInstrumentControl();
            this.lbl_Right_Cabin_Temperature = new System.Windows.Forms.Label();
            this.lbl_InCabin = new System.Windows.Forms.Label();
            this.lbl_Depth = new System.Windows.Forms.Label();
            this.picControlModule = new System.Windows.Forms.PictureBox();
            this.lbl_TDepth = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.grp_Operation = new System.Windows.Forms.GroupBox();
            this.cmb_Type = new System.Windows.Forms.ComboBox();
            this.btn_ConHandle = new System.Windows.Forms.Button();
            this.lblHandle = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txt_ConRobot = new System.Windows.Forms.TextBox();
            this.btn_ConRobot = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_Port = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grpType = new System.Windows.Forms.GroupBox();
            this.rdo_Deep = new System.Windows.Forms.RadioButton();
            this.rdo_Depth_Orientation = new System.Windows.Forms.RadioButton();
            this.rdoOpenLoop = new System.Windows.Forms.RadioButton();
            this.grp_MotionControlOperation = new System.Windows.Forms.GroupBox();
            this.lbl_Forward = new System.Windows.Forms.Label();
            this.lbl_TurnLeft = new System.Windows.Forms.Label();
            this.lbl_TurnRight = new System.Windows.Forms.Label();
            this.lbl_BackOff = new System.Windows.Forms.Label();
            this.lbl_Floating = new System.Windows.Forms.Label();
            this.lbl_Dive = new System.Windows.Forms.Label();
            this.lbl_LeftShift = new System.Windows.Forms.Label();
            this.lbl_RightShift = new System.Windows.Forms.Label();
            this.lbl_Laser = new System.Windows.Forms.Label();
            this.lbl_Course = new System.Windows.Forms.Label();
            this.lbl_Lighting = new System.Windows.Forms.Label();
            this.lbl_Middle_WS = new System.Windows.Forms.Label();
            this.grp_Handle = new System.Windows.Forms.GroupBox();
            this.TurnMethod_TextBox = new System.Windows.Forms.TextBox();
            this.TurnMethod = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Direc_Value_Z_TextBox = new System.Windows.Forms.TextBox();
            this.UpDown_Value_Y_TextBox = new System.Windows.Forms.TextBox();
            this.Forward_Back_Value_R_TextBox = new System.Windows.Forms.TextBox();
            this.vscY = new System.Windows.Forms.VScrollBar();
            this.vscR = new System.Windows.Forms.VScrollBar();
            this.hscX = new System.Windows.Forms.HScrollBar();
            this.lblRightShift = new System.Windows.Forms.Label();
            this.hscZ = new System.Windows.Forms.HScrollBar();
            this.tmr_Handle = new System.Windows.Forms.Timer(this.components);
            this.tmr_Automatic = new System.Windows.Forms.Timer(this.components);
            this.grp_RobotState.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picControlModule)).BeginInit();
            this.grp_Operation.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grpType.SuspendLayout();
            this.grp_MotionControlOperation.SuspendLayout();
            this.grp_Handle.SuspendLayout();
            this.SuspendLayout();
            // 
            // grp_RobotState
            // 
            this.grp_RobotState.Controls.Add(this.Roll_Pitch_Angle);
            this.grp_RobotState.Controls.Add(this.lbl_Heading_Angle_Number);
            this.grp_RobotState.Controls.Add(this.lbl_Right_Cabin_Temperature);
            this.grp_RobotState.Controls.Add(this.lbl_InCabin);
            this.grp_RobotState.Controls.Add(this.lbl_Depth);
            this.grp_RobotState.Controls.Add(this.picControlModule);
            this.grp_RobotState.Controls.Add(this.lbl_TDepth);
            this.grp_RobotState.Controls.Add(this.label3);
            this.grp_RobotState.Location = new System.Drawing.Point(12, 12);
            this.grp_RobotState.Name = "grp_RobotState";
            this.grp_RobotState.Size = new System.Drawing.Size(437, 160);
            this.grp_RobotState.TabIndex = 88;
            this.grp_RobotState.TabStop = false;
            this.grp_RobotState.Text = "机器人状态";
            // 
            // Roll_Pitch_Angle
            // 
            this.Roll_Pitch_Angle.Location = new System.Drawing.Point(143, 20);
            this.Roll_Pitch_Angle.Name = "Roll_Pitch_Angle";
            this.Roll_Pitch_Angle.Size = new System.Drawing.Size(131, 132);
            this.Roll_Pitch_Angle.TabIndex = 86;
            this.Roll_Pitch_Angle.Text = "attitudeIndicatorInstrumentControl1";
            // 
            // lbl_Heading_Angle_Number
            // 
            this.lbl_Heading_Angle_Number.Location = new System.Drawing.Point(6, 20);
            this.lbl_Heading_Angle_Number.Name = "lbl_Heading_Angle_Number";
            this.lbl_Heading_Angle_Number.Size = new System.Drawing.Size(131, 132);
            this.lbl_Heading_Angle_Number.TabIndex = 85;
            this.lbl_Heading_Angle_Number.Text = "headingIndicatorInstrumentControl1";
            // 
            // lbl_Right_Cabin_Temperature
            // 
            this.lbl_Right_Cabin_Temperature.AutoSize = true;
            this.lbl_Right_Cabin_Temperature.Font = new System.Drawing.Font("宋体", 10F);
            this.lbl_Right_Cabin_Temperature.Location = new System.Drawing.Point(371, 129);
            this.lbl_Right_Cabin_Temperature.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Right_Cabin_Temperature.Name = "lbl_Right_Cabin_Temperature";
            this.lbl_Right_Cabin_Temperature.Size = new System.Drawing.Size(28, 14);
            this.lbl_Right_Cabin_Temperature.TabIndex = 84;
            this.lbl_Right_Cabin_Temperature.Text = "0°";
            // 
            // lbl_InCabin
            // 
            this.lbl_InCabin.AutoSize = true;
            this.lbl_InCabin.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_InCabin.Location = new System.Drawing.Point(279, 129);
            this.lbl_InCabin.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_InCabin.Name = "lbl_InCabin";
            this.lbl_InCabin.Size = new System.Drawing.Size(87, 16);
            this.lbl_InCabin.TabIndex = 83;
            this.lbl_InCabin.Text = "舱内温度：";
            // 
            // lbl_Depth
            // 
            this.lbl_Depth.AutoSize = true;
            this.lbl_Depth.Font = new System.Drawing.Font("宋体", 10F);
            this.lbl_Depth.Location = new System.Drawing.Point(371, 96);
            this.lbl_Depth.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Depth.Name = "lbl_Depth";
            this.lbl_Depth.Size = new System.Drawing.Size(42, 14);
            this.lbl_Depth.TabIndex = 76;
            this.lbl_Depth.Text = "0.00m";
            this.lbl_Depth.Click += new System.EventHandler(this.lbl_Depth_Click);
            // 
            // picControlModule
            // 
            this.picControlModule.BackColor = System.Drawing.Color.Red;
            this.picControlModule.Location = new System.Drawing.Point(368, 31);
            this.picControlModule.Margin = new System.Windows.Forms.Padding(2);
            this.picControlModule.Name = "picControlModule";
            this.picControlModule.Size = new System.Drawing.Size(35, 38);
            this.picControlModule.TabIndex = 70;
            this.picControlModule.TabStop = false;
            // 
            // lbl_TDepth
            // 
            this.lbl_TDepth.AutoSize = true;
            this.lbl_TDepth.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_TDepth.Location = new System.Drawing.Point(279, 96);
            this.lbl_TDepth.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_TDepth.Name = "lbl_TDepth";
            this.lbl_TDepth.Size = new System.Drawing.Size(87, 16);
            this.lbl_TDepth.TabIndex = 75;
            this.lbl_TDepth.Text = "当前深度：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F);
            this.label3.Location = new System.Drawing.Point(279, 40);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 16);
            this.label3.TabIndex = 69;
            this.label3.Text = "漏水报警";
            // 
            // grp_Operation
            // 
            this.grp_Operation.Controls.Add(this.cmb_Type);
            this.grp_Operation.Controls.Add(this.btn_ConHandle);
            this.grp_Operation.Controls.Add(this.lblHandle);
            this.grp_Operation.Enabled = false;
            this.grp_Operation.Font = new System.Drawing.Font("宋体", 9F);
            this.grp_Operation.Location = new System.Drawing.Point(210, 234);
            this.grp_Operation.Name = "grp_Operation";
            this.grp_Operation.Size = new System.Drawing.Size(239, 50);
            this.grp_Operation.TabIndex = 89;
            this.grp_Operation.TabStop = false;
            this.grp_Operation.Text = "操作类型";
            // 
            // cmb_Type
            // 
            this.cmb_Type.FormattingEnabled = true;
            this.cmb_Type.Location = new System.Drawing.Point(55, 19);
            this.cmb_Type.Name = "cmb_Type";
            this.cmb_Type.Size = new System.Drawing.Size(102, 20);
            this.cmb_Type.TabIndex = 27;
            this.cmb_Type.SelectedIndexChanged += new System.EventHandler(this.cmb_Type_SelectedIndexChanged);
            // 
            // btn_ConHandle
            // 
            this.btn_ConHandle.Font = new System.Drawing.Font("宋体", 9F);
            this.btn_ConHandle.Location = new System.Drawing.Point(164, 15);
            this.btn_ConHandle.Name = "btn_ConHandle";
            this.btn_ConHandle.Size = new System.Drawing.Size(55, 27);
            this.btn_ConHandle.TabIndex = 26;
            this.btn_ConHandle.Text = "连 接";
            this.btn_ConHandle.UseVisualStyleBackColor = true;
            this.btn_ConHandle.Click += new System.EventHandler(this.btn_ConHandle_Click);
            // 
            // lblHandle
            // 
            this.lblHandle.AutoSize = true;
            this.lblHandle.Font = new System.Drawing.Font("宋体", 9F);
            this.lblHandle.Location = new System.Drawing.Point(6, 22);
            this.lblHandle.Name = "lblHandle";
            this.lblHandle.Size = new System.Drawing.Size(53, 12);
            this.lblHandle.TabIndex = 25;
            this.lblHandle.Text = "请选择：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txt_ConRobot);
            this.groupBox2.Controls.Add(this.btn_ConRobot);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txt_Port);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(11, 177);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(438, 52);
            this.groupBox2.TabIndex = 66;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "网络连接";
            // 
            // txt_ConRobot
            // 
            this.txt_ConRobot.Location = new System.Drawing.Point(69, 17);
            this.txt_ConRobot.Margin = new System.Windows.Forms.Padding(2);
            this.txt_ConRobot.Name = "txt_ConRobot";
            this.txt_ConRobot.Size = new System.Drawing.Size(100, 21);
            this.txt_ConRobot.TabIndex = 42;
            this.txt_ConRobot.Text = "192.168.1.252";
            // 
            // btn_ConRobot
            // 
            this.btn_ConRobot.Location = new System.Drawing.Point(354, 16);
            this.btn_ConRobot.Margin = new System.Windows.Forms.Padding(2);
            this.btn_ConRobot.Name = "btn_ConRobot";
            this.btn_ConRobot.Size = new System.Drawing.Size(49, 22);
            this.btn_ConRobot.TabIndex = 65;
            this.btn_ConRobot.TabStop = false;
            this.btn_ConRobot.Text = "开启";
            this.btn_ConRobot.UseVisualStyleBackColor = true;
            this.btn_ConRobot.Click += new System.EventHandler(this.btn_ConRobot_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10F);
            this.label4.Location = new System.Drawing.Point(11, 18);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 14);
            this.label4.TabIndex = 59;
            this.label4.Text = "IP地址:";
            // 
            // txt_Port
            // 
            this.txt_Port.Location = new System.Drawing.Point(246, 17);
            this.txt_Port.Margin = new System.Windows.Forms.Padding(2);
            this.txt_Port.Name = "txt_Port";
            this.txt_Port.Size = new System.Drawing.Size(52, 21);
            this.txt_Port.TabIndex = 63;
            this.txt_Port.Text = "1524";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10F);
            this.label5.Location = new System.Drawing.Point(186, 19);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 14);
            this.label5.TabIndex = 64;
            this.label5.Text = "端口号:";
            // 
            // grpType
            // 
            this.grpType.Controls.Add(this.rdo_Deep);
            this.grpType.Controls.Add(this.rdo_Depth_Orientation);
            this.grpType.Controls.Add(this.rdoOpenLoop);
            this.grpType.Font = new System.Drawing.Font("宋体", 9F);
            this.grpType.Location = new System.Drawing.Point(11, 234);
            this.grpType.Name = "grpType";
            this.grpType.Size = new System.Drawing.Size(193, 50);
            this.grpType.TabIndex = 91;
            this.grpType.TabStop = false;
            this.grpType.Text = "控制模式";
            // 
            // rdo_Deep
            // 
            this.rdo_Deep.AutoSize = true;
            this.rdo_Deep.Font = new System.Drawing.Font("宋体", 9F);
            this.rdo_Deep.Location = new System.Drawing.Point(144, 22);
            this.rdo_Deep.Name = "rdo_Deep";
            this.rdo_Deep.Size = new System.Drawing.Size(47, 16);
            this.rdo_Deep.TabIndex = 3;
            this.rdo_Deep.Tag = "2";
            this.rdo_Deep.Text = "定深";
            this.rdo_Deep.UseVisualStyleBackColor = true;
            this.rdo_Deep.CheckedChanged += new System.EventHandler(this.rdo_Deep_CheckedChanged);
            // 
            // rdo_Depth_Orientation
            // 
            this.rdo_Depth_Orientation.AutoSize = true;
            this.rdo_Depth_Orientation.Font = new System.Drawing.Font("宋体", 9F);
            this.rdo_Depth_Orientation.Location = new System.Drawing.Point(67, 22);
            this.rdo_Depth_Orientation.Name = "rdo_Depth_Orientation";
            this.rdo_Depth_Orientation.Size = new System.Drawing.Size(71, 16);
            this.rdo_Depth_Orientation.TabIndex = 2;
            this.rdo_Depth_Orientation.Tag = "1";
            this.rdo_Depth_Orientation.Text = "定深定向";
            this.rdo_Depth_Orientation.UseVisualStyleBackColor = true;
            this.rdo_Depth_Orientation.CheckedChanged += new System.EventHandler(this.rdo_Depth_Orientation_CheckedChanged);
            // 
            // rdoOpenLoop
            // 
            this.rdoOpenLoop.AutoSize = true;
            this.rdoOpenLoop.Checked = true;
            this.rdoOpenLoop.Font = new System.Drawing.Font("宋体", 9F);
            this.rdoOpenLoop.Location = new System.Drawing.Point(13, 22);
            this.rdoOpenLoop.Name = "rdoOpenLoop";
            this.rdoOpenLoop.Size = new System.Drawing.Size(47, 16);
            this.rdoOpenLoop.TabIndex = 0;
            this.rdoOpenLoop.TabStop = true;
            this.rdoOpenLoop.Tag = "0";
            this.rdoOpenLoop.Text = "开环";
            this.rdoOpenLoop.UseVisualStyleBackColor = true;
            this.rdoOpenLoop.CheckedChanged += new System.EventHandler(this.rdoOpenLoop_CheckedChanged);
            // 
            // grp_MotionControlOperation
            // 
            this.grp_MotionControlOperation.Controls.Add(this.lbl_Forward);
            this.grp_MotionControlOperation.Controls.Add(this.lbl_TurnLeft);
            this.grp_MotionControlOperation.Controls.Add(this.lbl_TurnRight);
            this.grp_MotionControlOperation.Controls.Add(this.lbl_BackOff);
            this.grp_MotionControlOperation.Controls.Add(this.lbl_Floating);
            this.grp_MotionControlOperation.Controls.Add(this.lbl_Dive);
            this.grp_MotionControlOperation.Controls.Add(this.lbl_LeftShift);
            this.grp_MotionControlOperation.Controls.Add(this.lbl_RightShift);
            this.grp_MotionControlOperation.Controls.Add(this.lbl_Laser);
            this.grp_MotionControlOperation.Controls.Add(this.lbl_Course);
            this.grp_MotionControlOperation.Controls.Add(this.lbl_Lighting);
            this.grp_MotionControlOperation.Controls.Add(this.lbl_Middle_WS);
            this.grp_MotionControlOperation.Enabled = false;
            this.grp_MotionControlOperation.Location = new System.Drawing.Point(12, 289);
            this.grp_MotionControlOperation.Margin = new System.Windows.Forms.Padding(2);
            this.grp_MotionControlOperation.Name = "grp_MotionControlOperation";
            this.grp_MotionControlOperation.Padding = new System.Windows.Forms.Padding(2);
            this.grp_MotionControlOperation.Size = new System.Drawing.Size(437, 118);
            this.grp_MotionControlOperation.TabIndex = 92;
            this.grp_MotionControlOperation.TabStop = false;
            this.grp_MotionControlOperation.Text = "运动控制操作";
            // 
            // lbl_Forward
            // 
            this.lbl_Forward.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_Forward.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_Forward.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_Forward.Location = new System.Drawing.Point(80, 20);
            this.lbl_Forward.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Forward.Name = "lbl_Forward";
            this.lbl_Forward.Size = new System.Drawing.Size(52, 40);
            this.lbl_Forward.TabIndex = 96;
            this.lbl_Forward.Text = " 前进(W)";
            this.lbl_Forward.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Forward.Click += new System.EventHandler(this.lbl_Forward_Click);
            // 
            // lbl_TurnLeft
            // 
            this.lbl_TurnLeft.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_TurnLeft.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_TurnLeft.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_TurnLeft.Location = new System.Drawing.Point(8, 64);
            this.lbl_TurnLeft.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_TurnLeft.Name = "lbl_TurnLeft";
            this.lbl_TurnLeft.Size = new System.Drawing.Size(52, 40);
            this.lbl_TurnLeft.TabIndex = 98;
            this.lbl_TurnLeft.Text = " 左转(A)";
            this.lbl_TurnLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_TurnLeft.Click += new System.EventHandler(this.lbl_TurnLeft_Click);
            // 
            // lbl_TurnRight
            // 
            this.lbl_TurnRight.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_TurnRight.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_TurnRight.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_TurnRight.Location = new System.Drawing.Point(152, 64);
            this.lbl_TurnRight.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_TurnRight.Name = "lbl_TurnRight";
            this.lbl_TurnRight.Size = new System.Drawing.Size(52, 40);
            this.lbl_TurnRight.TabIndex = 99;
            this.lbl_TurnRight.Text = " 右转(D)";
            this.lbl_TurnRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_TurnRight.Click += new System.EventHandler(this.lbl_TurnRight_Click);
            // 
            // lbl_BackOff
            // 
            this.lbl_BackOff.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_BackOff.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_BackOff.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_BackOff.Location = new System.Drawing.Point(80, 64);
            this.lbl_BackOff.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_BackOff.Name = "lbl_BackOff";
            this.lbl_BackOff.Size = new System.Drawing.Size(52, 40);
            this.lbl_BackOff.TabIndex = 97;
            this.lbl_BackOff.Text = " 后退(S)";
            this.lbl_BackOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_BackOff.Click += new System.EventHandler(this.lbl_BackOff_Click);
            // 
            // lbl_Floating
            // 
            this.lbl_Floating.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_Floating.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_Floating.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_Floating.Location = new System.Drawing.Point(224, 20);
            this.lbl_Floating.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Floating.Name = "lbl_Floating";
            this.lbl_Floating.Size = new System.Drawing.Size(52, 40);
            this.lbl_Floating.TabIndex = 100;
            this.lbl_Floating.Text = " 上浮 (I)";
            this.lbl_Floating.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Floating.Click += new System.EventHandler(this.lbl_Floating_Click);
            // 
            // lbl_Dive
            // 
            this.lbl_Dive.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_Dive.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_Dive.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_Dive.Location = new System.Drawing.Point(224, 64);
            this.lbl_Dive.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Dive.Name = "lbl_Dive";
            this.lbl_Dive.Size = new System.Drawing.Size(52, 40);
            this.lbl_Dive.TabIndex = 101;
            this.lbl_Dive.Text = " 下潜(K)";
            this.lbl_Dive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Dive.Click += new System.EventHandler(this.lbl_Dive_Click);
            // 
            // lbl_LeftShift
            // 
            this.lbl_LeftShift.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_LeftShift.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_LeftShift.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_LeftShift.Location = new System.Drawing.Point(8, 20);
            this.lbl_LeftShift.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_LeftShift.Name = "lbl_LeftShift";
            this.lbl_LeftShift.Size = new System.Drawing.Size(52, 40);
            this.lbl_LeftShift.TabIndex = 102;
            this.lbl_LeftShift.Text = " 左移(Q)";
            this.lbl_LeftShift.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_LeftShift.Click += new System.EventHandler(this.lbl_LeftShift_Click);
            // 
            // lbl_RightShift
            // 
            this.lbl_RightShift.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_RightShift.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_RightShift.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_RightShift.Location = new System.Drawing.Point(152, 20);
            this.lbl_RightShift.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_RightShift.Name = "lbl_RightShift";
            this.lbl_RightShift.Size = new System.Drawing.Size(52, 40);
            this.lbl_RightShift.TabIndex = 103;
            this.lbl_RightShift.Text = " 右移(E)";
            this.lbl_RightShift.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_RightShift.Click += new System.EventHandler(this.lbl_RightShift_Click);
            // 
            // lbl_Laser
            // 
            this.lbl_Laser.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_Laser.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_Laser.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_Laser.Location = new System.Drawing.Point(367, 64);
            this.lbl_Laser.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Laser.Name = "lbl_Laser";
            this.lbl_Laser.Size = new System.Drawing.Size(52, 40);
            this.lbl_Laser.TabIndex = 94;
            this.lbl_Laser.Text = "释放(L)";
            this.lbl_Laser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Laser.Click += new System.EventHandler(this.lbl_Laser_Click);
            // 
            // lbl_Course
            // 
            this.lbl_Course.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_Course.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_Course.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_Course.Location = new System.Drawing.Point(368, 20);
            this.lbl_Course.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Course.Name = "lbl_Course";
            this.lbl_Course.Size = new System.Drawing.Size(52, 40);
            this.lbl_Course.TabIndex = 95;
            this.lbl_Course.Text = "抓取(J)";
            this.lbl_Course.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Course.Click += new System.EventHandler(this.lbl_Course_Click);
            // 
            // lbl_Lighting
            // 
            this.lbl_Lighting.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_Lighting.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_Lighting.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_Lighting.Location = new System.Drawing.Point(296, 20);
            this.lbl_Lighting.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Lighting.Name = "lbl_Lighting";
            this.lbl_Lighting.Size = new System.Drawing.Size(52, 40);
            this.lbl_Lighting.TabIndex = 93;
            this.lbl_Lighting.Text = " 照明(O)";
            this.lbl_Lighting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Lighting.Click += new System.EventHandler(this.lbl_Lighting_Click);
            // 
            // lbl_Middle_WS
            // 
            this.lbl_Middle_WS.BackColor = System.Drawing.SystemColors.Highlight;
            this.lbl_Middle_WS.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_Middle_WS.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbl_Middle_WS.Location = new System.Drawing.Point(299, 64);
            this.lbl_Middle_WS.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_Middle_WS.Name = "lbl_Middle_WS";
            this.lbl_Middle_WS.Size = new System.Drawing.Size(52, 40);
            this.lbl_Middle_WS.TabIndex = 78;
            this.lbl_Middle_WS.Text = "归中(B)";
            this.lbl_Middle_WS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Middle_WS.Click += new System.EventHandler(this.lbl_Middle_WS_Click);
            // 
            // grp_Handle
            // 
            this.grp_Handle.Controls.Add(this.TurnMethod_TextBox);
            this.grp_Handle.Controls.Add(this.TurnMethod);
            this.grp_Handle.Controls.Add(this.label8);
            this.grp_Handle.Controls.Add(this.label7);
            this.grp_Handle.Controls.Add(this.label6);
            this.grp_Handle.Controls.Add(this.label2);
            this.grp_Handle.Controls.Add(this.label1);
            this.grp_Handle.Controls.Add(this.Direc_Value_Z_TextBox);
            this.grp_Handle.Controls.Add(this.UpDown_Value_Y_TextBox);
            this.grp_Handle.Controls.Add(this.Forward_Back_Value_R_TextBox);
            this.grp_Handle.Controls.Add(this.vscY);
            this.grp_Handle.Controls.Add(this.vscR);
            this.grp_Handle.Controls.Add(this.hscX);
            this.grp_Handle.Controls.Add(this.lblRightShift);
            this.grp_Handle.Controls.Add(this.hscZ);
            this.grp_Handle.Font = new System.Drawing.Font("宋体", 9F);
            this.grp_Handle.Location = new System.Drawing.Point(12, 411);
            this.grp_Handle.Margin = new System.Windows.Forms.Padding(2);
            this.grp_Handle.Name = "grp_Handle";
            this.grp_Handle.Padding = new System.Windows.Forms.Padding(2);
            this.grp_Handle.Size = new System.Drawing.Size(437, 144);
            this.grp_Handle.TabIndex = 110;
            this.grp_Handle.TabStop = false;
            this.grp_Handle.Text = "手柄操作";
            // 
            // TurnMethod_TextBox
            // 
            this.TurnMethod_TextBox.Location = new System.Drawing.Point(232, 108);
            this.TurnMethod_TextBox.Name = "TurnMethod_TextBox";
            this.TurnMethod_TextBox.Size = new System.Drawing.Size(50, 21);
            this.TurnMethod_TextBox.TabIndex = 50;
            this.TurnMethod_TextBox.Text = "左摇杆";
            // 
            // TurnMethod
            // 
            this.TurnMethod.AutoSize = true;
            this.TurnMethod.Location = new System.Drawing.Point(172, 111);
            this.TurnMethod.Name = "TurnMethod";
            this.TurnMethod.Size = new System.Drawing.Size(53, 12);
            this.TurnMethod.TabIndex = 49;
            this.TurnMethod.Text = "转向控制";
            this.TurnMethod.Click += new System.EventHandler(this.TurnMethod_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(384, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 48;
            this.label8.Text = "右摇杆";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 38);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 47;
            this.label7.Text = "左摇杆";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(172, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 46;
            this.label6.Text = "水平转矩";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(172, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 45;
            this.label2.Text = "上升推力";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(172, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 44;
            this.label1.Text = "前进推力";
            // 
            // Direc_Value_Z_TextBox
            // 
            this.Direc_Value_Z_TextBox.Location = new System.Drawing.Point(232, 73);
            this.Direc_Value_Z_TextBox.Name = "Direc_Value_Z_TextBox";
            this.Direc_Value_Z_TextBox.Size = new System.Drawing.Size(65, 21);
            this.Direc_Value_Z_TextBox.TabIndex = 43;
            // 
            // UpDown_Value_Y_TextBox
            // 
            this.UpDown_Value_Y_TextBox.Location = new System.Drawing.Point(232, 46);
            this.UpDown_Value_Y_TextBox.Name = "UpDown_Value_Y_TextBox";
            this.UpDown_Value_Y_TextBox.Size = new System.Drawing.Size(65, 21);
            this.UpDown_Value_Y_TextBox.TabIndex = 42;
            // 
            // Forward_Back_Value_R_TextBox
            // 
            this.Forward_Back_Value_R_TextBox.Location = new System.Drawing.Point(232, 19);
            this.Forward_Back_Value_R_TextBox.Name = "Forward_Back_Value_R_TextBox";
            this.Forward_Back_Value_R_TextBox.Size = new System.Drawing.Size(65, 21);
            this.Forward_Back_Value_R_TextBox.TabIndex = 41;
            // 
            // vscY
            // 
            this.vscY.Location = new System.Drawing.Point(68, 16);
            this.vscY.Maximum = 200;
            this.vscY.Name = "vscY";
            this.vscY.Size = new System.Drawing.Size(20, 96);
            this.vscY.TabIndex = 28;
            this.vscY.Value = 100;
            // 
            // vscR
            // 
            this.vscR.Location = new System.Drawing.Point(353, 16);
            this.vscR.Maximum = 200;
            this.vscR.Name = "vscR";
            this.vscR.Size = new System.Drawing.Size(20, 99);
            this.vscR.TabIndex = 33;
            this.vscR.Value = 100;
            // 
            // hscX
            // 
            this.hscX.Location = new System.Drawing.Point(20, 112);
            this.hscX.Maximum = 200;
            this.hscX.Name = "hscX";
            this.hscX.Size = new System.Drawing.Size(117, 20);
            this.hscX.TabIndex = 40;
            this.hscX.Value = 100;
            this.hscX.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hscX_Scroll);
            // 
            // lblRightShift
            // 
            this.lblRightShift.AutoSize = true;
            this.lblRightShift.Font = new System.Drawing.Font("宋体", 14F);
            this.lblRightShift.Location = new System.Drawing.Point(454, 126);
            this.lblRightShift.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRightShift.Name = "lblRightShift";
            this.lblRightShift.Size = new System.Drawing.Size(47, 19);
            this.lblRightShift.TabIndex = 31;
            this.lblRightShift.Text = "右转";
            // 
            // hscZ
            // 
            this.hscZ.Location = new System.Drawing.Point(307, 115);
            this.hscZ.Maximum = 200;
            this.hscZ.Name = "hscZ";
            this.hscZ.Size = new System.Drawing.Size(113, 20);
            this.hscZ.TabIndex = 29;
            this.hscZ.Value = 100;
            // 
            // tmr_Handle
            // 
            this.tmr_Handle.Tick += new System.EventHandler(this.tmr_Handle_Tick);
            // 
            // tmr_Automatic
            // 
            this.tmr_Automatic.Tick += new System.EventHandler(this.tmr_Automatic_Tick);
            // 
            // frm_MainRov
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 563);
            this.Controls.Add(this.grp_Handle);
            this.Controls.Add(this.grp_MotionControlOperation);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grpType);
            this.Controls.Add(this.grp_Operation);
            this.Controls.Add(this.grp_RobotState);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "frm_MainRov";
            this.Text = "Rov";
            this.Load += new System.EventHandler(this.frm_MainRov_Load);
            this.grp_RobotState.ResumeLayout(false);
            this.grp_RobotState.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picControlModule)).EndInit();
            this.grp_Operation.ResumeLayout(false);
            this.grp_Operation.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.grpType.ResumeLayout(false);
            this.grpType.PerformLayout();
            this.grp_MotionControlOperation.ResumeLayout(false);
            this.grp_Handle.ResumeLayout(false);
            this.grp_Handle.PerformLayout();
            this.ResumeLayout(false);

		}

        private void hscX_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void lbl_Depth_Click(object sender, EventArgs e)
        {

        }

        private void TurnMethod_Click(object sender, EventArgs e)
        {
			trunmethod = !trunmethod;
			if (trunmethod)
			{
				TurnMethod_TextBox.Text = "左摇杆";
			}
			else
			{
				TurnMethod_TextBox.Text = "右摇杆";
			}

		}
    }
}

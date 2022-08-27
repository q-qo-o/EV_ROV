using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ROV
{
	public class frm_DebugMode : Form
	{
		public string str_Path;

		private List<string> listItem;

		private IContainer components = null;

		private Panel panel1;

		private Label label3;

		private Label label2;

		private Label label1;

		private TextBox txt_Depth_P;

		private TextBox txt_Depth_I_L;

		private TextBox txt_Depth_I;

		private Label label4;

		private Label label5;

		private TextBox txt_Depth_P_L;

		private TextBox txt_Depth_D_L;

		private TextBox txt_Depth_D;

		private Label label6;

		private Label label7;

		private TextBox txt_Roll_D_L;

		private TextBox txt_Roll_D;

		private Label label22;

		private Label label23;

		private TextBox txt_Roll_I_L;

		private TextBox txt_Roll_I;

		private Label label24;

		private Label label25;

		private TextBox txt_Roll_P_L;

		private TextBox txt_Roll_P;

		private Label label26;

		private Label label27;

		private Label label28;

		private TextBox txt_Pitch_D_L;

		private TextBox txt_Pitch_D;

		private Label label15;

		private Label label16;

		private TextBox txt_Pitch_I_L;

		private TextBox txt_Pitch_I;

		private Label label17;

		private Label label18;

		private TextBox txt_Pitch_P_L;

		private TextBox txt_Pitch_P;

		private Label label19;

		private Label label20;

		private Label label21;

		private TextBox txt_Boat_D_L;

		private TextBox txt_Boat_D;

		private Label label8;

		private Label label9;

		private TextBox txt_Boat_I_L;

		private TextBox txt_Boat_I;

		private Label label10;

		private Label label11;

		private TextBox txt_Boat_P_L;

		private TextBox txt_Boat_P;

		private Label label12;

		private Label label13;

		private Label lbl_Boat;

		private TextBox txt_Max_Value1;

		private Label label30;

		private TextBox txt_Min_Value1;

		private Label label29;

		private Label label14;

		private TextBox txt_Max_Value2;

		private Label label31;

		private TextBox txt_Min_Value2;

		private Label label32;

		private Label label33;

		private Label label34;

		private Label label35;

		private TextBox txt_Throttle;

		private Label label36;

		private Label label37;

		private Label label38;

		private Button btn_Write;

		private Timer trm_SendPID;

		public event SendPidParameterInfo Send_InfoItems;

		public frm_DebugMode()
		{
			InitializeComponent();
		}

		private void frm_DebugMode_Load(object sender, EventArgs e)
		{
			listItem = new List<string>();
			str_Path = Assembly.GetExecutingAssembly().Location;
			str_Path = str_Path.Substring(0, str_Path.LastIndexOf('\\'));
			str_Path += "\\ParSet\\fParameter.xml";
			SetInitData();
		}

		private void frm_DebugMode_FormClosing(object sender, FormClosingEventArgs e)
		{
			trm_SendPID.Enabled = false;
		}

		private void btn_Write_Click(object sender, EventArgs e)
		{
			listItem.Clear();
			listItem.Add(txt_Depth_P.Text);
			listItem.Add(txt_Depth_P_L.Text);
			listItem.Add(txt_Depth_I.Text);
			listItem.Add(txt_Depth_I_L.Text);
			listItem.Add(txt_Depth_D.Text);
			listItem.Add(txt_Depth_D_L.Text);
			listItem.Add(txt_Boat_P.Text);
			listItem.Add(txt_Boat_P_L.Text);
			listItem.Add(txt_Boat_I.Text);
			listItem.Add(txt_Boat_I_L.Text);
			listItem.Add(txt_Boat_D.Text);
			listItem.Add(txt_Boat_D_L.Text);
			listItem.Add(txt_Pitch_P.Text);
			listItem.Add(txt_Pitch_P_L.Text);
			listItem.Add(txt_Pitch_I.Text);
			listItem.Add(txt_Pitch_I_L.Text);
			listItem.Add(txt_Pitch_D.Text);
			listItem.Add(txt_Pitch_D_L.Text);
			listItem.Add(txt_Roll_P.Text);
			listItem.Add(txt_Roll_P_L.Text);
			listItem.Add(txt_Roll_I.Text);
			listItem.Add(txt_Roll_I_L.Text);
			listItem.Add(txt_Roll_D.Text);
			listItem.Add(txt_Roll_D_L.Text);
			listItem.Add(txt_Min_Value1.Text);
			listItem.Add(txt_Max_Value1.Text);
			listItem.Add(txt_Min_Value2.Text);
			listItem.Add(txt_Max_Value2.Text);
			listItem.Add(txt_Throttle.Text);
			UpdateParameterRecord();
			trm_SendPID.Enabled = true;
		}

		private void trm_SendPID_Tick(object sender, EventArgs e)
		{
			this.Send_InfoItems(listItem);
		}

		private void SetInitData()
		{
			DataTable dtPar = GetShipInfos(str_Path);
			DataRow[] dr = dtPar.Select("id=1");
			txt_Depth_P.Text = dr[0]["depth_p"].ToString();
			txt_Depth_P_L.Text = dr[0]["depth_p_l"].ToString();
			txt_Depth_I.Text = dr[0]["depth_i"].ToString();
			txt_Depth_I_L.Text = dr[0]["depth_i_l"].ToString();
			txt_Depth_D.Text = dr[0]["depth_d"].ToString();
			txt_Depth_D_L.Text = dr[0]["depth_d_l"].ToString();
			txt_Boat_P.Text = dr[0]["boat_p"].ToString();
			txt_Boat_P_L.Text = dr[0]["boat_p_l"].ToString();
			txt_Boat_I.Text = dr[0]["boat_i"].ToString();
			txt_Boat_I_L.Text = dr[0]["boat_i_l"].ToString();
			txt_Boat_D.Text = dr[0]["boat_d"].ToString();
			txt_Boat_D_L.Text = dr[0]["boat_d_l"].ToString();
			txt_Pitch_P.Text = dr[0]["pitch_p"].ToString();
			txt_Pitch_P_L.Text = dr[0]["pitch_p_l"].ToString();
			txt_Pitch_I.Text = dr[0]["pitch_i"].ToString();
			txt_Pitch_I_L.Text = dr[0]["pitch_i_l"].ToString();
			txt_Pitch_D.Text = dr[0]["pitch_d"].ToString();
			txt_Pitch_D_L.Text = dr[0]["pitch_d_l"].ToString();
			txt_Roll_P.Text = dr[0]["roll_p"].ToString();
			txt_Roll_P_L.Text = dr[0]["roll_p_l"].ToString();
			txt_Roll_I.Text = dr[0]["roll_i"].ToString();
			txt_Roll_I_L.Text = dr[0]["roll_i_l"].ToString();
			txt_Roll_D.Text = dr[0]["roll_d"].ToString();
			txt_Roll_D_L.Text = dr[0]["roll_d_l"].ToString();
			txt_Min_Value1.Text = dr[0]["min_value1"].ToString();
			txt_Max_Value1.Text = dr[0]["max_value1"].ToString();
			txt_Min_Value2.Text = dr[0]["min_value2"].ToString();
			txt_Max_Value2.Text = dr[0]["max_value2"].ToString();
			txt_Throttle.Text = dr[0]["throttle"].ToString();
		}

		public static DataTable GetShipInfos(string str_Path)
		{
			DataTable dt = new DataTable();
			try
			{
				if (!File.Exists(str_Path))
				{
					throw new Exception("文件不存在!");
				}
				DataSet myds = new DataSet();
				myds.ReadXml(str_Path);
				return myds.Tables[0];
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return new DataTable();
			}
		}

		private void UpdateParameterRecord()
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(str_Path);
			XmlNodeList nodelist = xmlDoc.SelectSingleNode("Parameters").ChildNodes;
			foreach (XmlNode xn in nodelist)
			{
				XmlElement selectXe = (XmlElement)xn;
				selectXe.GetElementsByTagName("depth_p").Item(0).InnerText = txt_Depth_P.Text.ToString();
				selectXe.GetElementsByTagName("depth_p_l").Item(0).InnerText = txt_Depth_P_L.Text.ToString();
				selectXe.GetElementsByTagName("depth_i").Item(0).InnerText = txt_Depth_I.Text.ToString();
				selectXe.GetElementsByTagName("depth_i_l").Item(0).InnerText = txt_Depth_I_L.Text.ToString();
				selectXe.GetElementsByTagName("depth_d").Item(0).InnerText = txt_Depth_D.Text.ToString();
				selectXe.GetElementsByTagName("depth_d_l").Item(0).InnerText = txt_Depth_D_L.Text.ToString();
				selectXe.GetElementsByTagName("boat_p").Item(0).InnerText = txt_Boat_P.Text.ToString();
				selectXe.GetElementsByTagName("boat_p_l").Item(0).InnerText = txt_Boat_P_L.Text.ToString();
				selectXe.GetElementsByTagName("boat_i").Item(0).InnerText = txt_Boat_I.Text.ToString();
				selectXe.GetElementsByTagName("boat_i_l").Item(0).InnerText = txt_Boat_I_L.Text.ToString();
				selectXe.GetElementsByTagName("boat_d").Item(0).InnerText = txt_Boat_D.Text.ToString();
				selectXe.GetElementsByTagName("boat_d_l").Item(0).InnerText = txt_Boat_D_L.Text.ToString();
				selectXe.GetElementsByTagName("pitch_p").Item(0).InnerText = txt_Pitch_P.Text.ToString();
				selectXe.GetElementsByTagName("pitch_p_l").Item(0).InnerText = txt_Pitch_P_L.Text.ToString();
				selectXe.GetElementsByTagName("pitch_i").Item(0).InnerText = txt_Pitch_I.Text.ToString();
				selectXe.GetElementsByTagName("pitch_i_l").Item(0).InnerText = txt_Pitch_I_L.Text.ToString();
				selectXe.GetElementsByTagName("pitch_d").Item(0).InnerText = txt_Pitch_D.Text.ToString();
				selectXe.GetElementsByTagName("pitch_d_l").Item(0).InnerText = txt_Pitch_D_L.Text.ToString();
				selectXe.GetElementsByTagName("roll_p").Item(0).InnerText = txt_Roll_P.Text.ToString();
				selectXe.GetElementsByTagName("roll_p_l").Item(0).InnerText = txt_Roll_P_L.Text.ToString();
				selectXe.GetElementsByTagName("roll_i").Item(0).InnerText = txt_Roll_I.Text.ToString();
				selectXe.GetElementsByTagName("roll_i_l").Item(0).InnerText = txt_Roll_I_L.Text.ToString();
				selectXe.GetElementsByTagName("roll_d").Item(0).InnerText = txt_Roll_D.Text.ToString();
				selectXe.GetElementsByTagName("roll_d_l").Item(0).InnerText = txt_Roll_D_L.Text.ToString();
				selectXe.GetElementsByTagName("min_value1").Item(0).InnerText = txt_Min_Value1.Text.ToString();
				selectXe.GetElementsByTagName("max_value1").Item(0).InnerText = txt_Max_Value1.Text.ToString();
				selectXe.GetElementsByTagName("min_value2").Item(0).InnerText = txt_Min_Value2.Text.ToString();
				selectXe.GetElementsByTagName("max_value2").Item(0).InnerText = txt_Max_Value2.Text.ToString();
				selectXe.GetElementsByTagName("throttle").Item(0).InnerText = txt_Throttle.Text.ToString();
			}
			xmlDoc.Save(str_Path);
		}

		private void txt_Depth_P_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Depth_P_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Depth_I_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Depth_I_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Depth_D_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Depth_D_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Boat_P_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Boat_P_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Boat_I_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Boat_I_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Boat_D_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Boat_D_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Pitch_P_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Pitch_P_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Pitch_I_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Pitch_I_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Pitch_D_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Pitch_D_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Roll_P_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Roll_P_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Roll_I_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Roll_I_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Roll_D_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Roll_D_L_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Min_Value1_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Max_Value1_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Min_Value2_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Max_Value2_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void txt_Throttle_KeyPress(object sender, KeyPressEventArgs e)
		{
			KeyPressCheckInt(sender, e);
		}

		private void KeyPressCheckInt(object sender, KeyPressEventArgs e)
		{
			if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\r' && e.KeyChar != '\b')
			{
				e.Handled = true;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ROV.frm_DebugMode));
			this.panel1 = new System.Windows.Forms.Panel();
			this.btn_Write = new System.Windows.Forms.Button();
			this.label38 = new System.Windows.Forms.Label();
			this.label37 = new System.Windows.Forms.Label();
			this.txt_Throttle = new System.Windows.Forms.TextBox();
			this.label36 = new System.Windows.Forms.Label();
			this.label35 = new System.Windows.Forms.Label();
			this.label34 = new System.Windows.Forms.Label();
			this.txt_Max_Value2 = new System.Windows.Forms.TextBox();
			this.label31 = new System.Windows.Forms.Label();
			this.txt_Min_Value2 = new System.Windows.Forms.TextBox();
			this.label32 = new System.Windows.Forms.Label();
			this.label33 = new System.Windows.Forms.Label();
			this.txt_Max_Value1 = new System.Windows.Forms.TextBox();
			this.label30 = new System.Windows.Forms.Label();
			this.txt_Min_Value1 = new System.Windows.Forms.TextBox();
			this.label29 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.txt_Roll_D_L = new System.Windows.Forms.TextBox();
			this.txt_Roll_D = new System.Windows.Forms.TextBox();
			this.label22 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.txt_Roll_I_L = new System.Windows.Forms.TextBox();
			this.txt_Roll_I = new System.Windows.Forms.TextBox();
			this.label24 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.txt_Roll_P_L = new System.Windows.Forms.TextBox();
			this.txt_Roll_P = new System.Windows.Forms.TextBox();
			this.label26 = new System.Windows.Forms.Label();
			this.label27 = new System.Windows.Forms.Label();
			this.label28 = new System.Windows.Forms.Label();
			this.txt_Pitch_D_L = new System.Windows.Forms.TextBox();
			this.txt_Pitch_D = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.txt_Pitch_I_L = new System.Windows.Forms.TextBox();
			this.txt_Pitch_I = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.txt_Pitch_P_L = new System.Windows.Forms.TextBox();
			this.txt_Pitch_P = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.txt_Boat_D_L = new System.Windows.Forms.TextBox();
			this.txt_Boat_D = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.txt_Boat_I_L = new System.Windows.Forms.TextBox();
			this.txt_Boat_I = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.txt_Boat_P_L = new System.Windows.Forms.TextBox();
			this.txt_Boat_P = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.lbl_Boat = new System.Windows.Forms.Label();
			this.txt_Depth_D_L = new System.Windows.Forms.TextBox();
			this.txt_Depth_D = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.txt_Depth_I_L = new System.Windows.Forms.TextBox();
			this.txt_Depth_I = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.txt_Depth_P_L = new System.Windows.Forms.TextBox();
			this.txt_Depth_P = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.trm_SendPID = new System.Windows.Forms.Timer(this.components);
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.panel1.Controls.Add(this.btn_Write);
			this.panel1.Controls.Add(this.label38);
			this.panel1.Controls.Add(this.label37);
			this.panel1.Controls.Add(this.txt_Throttle);
			this.panel1.Controls.Add(this.label36);
			this.panel1.Controls.Add(this.label35);
			this.panel1.Controls.Add(this.label34);
			this.panel1.Controls.Add(this.txt_Max_Value2);
			this.panel1.Controls.Add(this.label31);
			this.panel1.Controls.Add(this.txt_Min_Value2);
			this.panel1.Controls.Add(this.label32);
			this.panel1.Controls.Add(this.label33);
			this.panel1.Controls.Add(this.txt_Max_Value1);
			this.panel1.Controls.Add(this.label30);
			this.panel1.Controls.Add(this.txt_Min_Value1);
			this.panel1.Controls.Add(this.label29);
			this.panel1.Controls.Add(this.label14);
			this.panel1.Controls.Add(this.txt_Roll_D_L);
			this.panel1.Controls.Add(this.txt_Roll_D);
			this.panel1.Controls.Add(this.label22);
			this.panel1.Controls.Add(this.label23);
			this.panel1.Controls.Add(this.txt_Roll_I_L);
			this.panel1.Controls.Add(this.txt_Roll_I);
			this.panel1.Controls.Add(this.label24);
			this.panel1.Controls.Add(this.label25);
			this.panel1.Controls.Add(this.txt_Roll_P_L);
			this.panel1.Controls.Add(this.txt_Roll_P);
			this.panel1.Controls.Add(this.label26);
			this.panel1.Controls.Add(this.label27);
			this.panel1.Controls.Add(this.label28);
			this.panel1.Controls.Add(this.txt_Pitch_D_L);
			this.panel1.Controls.Add(this.txt_Pitch_D);
			this.panel1.Controls.Add(this.label15);
			this.panel1.Controls.Add(this.label16);
			this.panel1.Controls.Add(this.txt_Pitch_I_L);
			this.panel1.Controls.Add(this.txt_Pitch_I);
			this.panel1.Controls.Add(this.label17);
			this.panel1.Controls.Add(this.label18);
			this.panel1.Controls.Add(this.txt_Pitch_P_L);
			this.panel1.Controls.Add(this.txt_Pitch_P);
			this.panel1.Controls.Add(this.label19);
			this.panel1.Controls.Add(this.label20);
			this.panel1.Controls.Add(this.label21);
			this.panel1.Controls.Add(this.txt_Boat_D_L);
			this.panel1.Controls.Add(this.txt_Boat_D);
			this.panel1.Controls.Add(this.label8);
			this.panel1.Controls.Add(this.label9);
			this.panel1.Controls.Add(this.txt_Boat_I_L);
			this.panel1.Controls.Add(this.txt_Boat_I);
			this.panel1.Controls.Add(this.label10);
			this.panel1.Controls.Add(this.label11);
			this.panel1.Controls.Add(this.txt_Boat_P_L);
			this.panel1.Controls.Add(this.txt_Boat_P);
			this.panel1.Controls.Add(this.label12);
			this.panel1.Controls.Add(this.label13);
			this.panel1.Controls.Add(this.lbl_Boat);
			this.panel1.Controls.Add(this.txt_Depth_D_L);
			this.panel1.Controls.Add(this.txt_Depth_D);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.label7);
			this.panel1.Controls.Add(this.txt_Depth_I_L);
			this.panel1.Controls.Add(this.txt_Depth_I);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.txt_Depth_P_L);
			this.panel1.Controls.Add(this.txt_Depth_P);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(804, 223);
			this.panel1.TabIndex = 0;
			this.btn_Write.Font = new System.Drawing.Font("宋体", 14f);
			this.btn_Write.Location = new System.Drawing.Point(695, 181);
			this.btn_Write.Name = "btn_Write";
			this.btn_Write.Size = new System.Drawing.Size(87, 37);
			this.btn_Write.TabIndex = 77;
			this.btn_Write.Text = "写入";
			this.btn_Write.UseVisualStyleBackColor = true;
			this.btn_Write.Click += new System.EventHandler(btn_Write_Click);
			this.label38.AutoSize = true;
			this.label38.Font = new System.Drawing.Font("宋体", 14f);
			this.label38.Location = new System.Drawing.Point(165, 190);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(202, 19);
			this.label38.TabIndex = 76;
			this.label38.Text = "取值范围0%—100%之间";
			this.label37.AutoSize = true;
			this.label37.Font = new System.Drawing.Font("宋体", 14f);
			this.label37.Location = new System.Drawing.Point(140, 192);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(19, 19);
			this.label37.TabIndex = 75;
			this.label37.Text = "%";
			this.txt_Throttle.Location = new System.Drawing.Point(79, 190);
			this.txt_Throttle.Name = "txt_Throttle";
			this.txt_Throttle.Size = new System.Drawing.Size(55, 21);
			this.txt_Throttle.TabIndex = 74;
			this.txt_Throttle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Throttle_KeyPress);
			this.label36.AutoSize = true;
			this.label36.Font = new System.Drawing.Font("宋体", 14f);
			this.label36.Location = new System.Drawing.Point(21, 190);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(47, 19);
			this.label36.TabIndex = 73;
			this.label36.Text = "油门";
			this.label35.AutoSize = true;
			this.label35.Font = new System.Drawing.Font("宋体", 14f);
			this.label35.Location = new System.Drawing.Point(428, 153);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(192, 19);
			this.label35.TabIndex = 72;
			this.label35.Text = "取值范围50—250之间";
			this.label34.AutoSize = true;
			this.label34.Font = new System.Drawing.Font("宋体", 14f);
			this.label34.Location = new System.Drawing.Point(428, 124);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(192, 19);
			this.label34.TabIndex = 71;
			this.label34.Text = "取值范围50—250之间";
			this.txt_Max_Value2.Location = new System.Drawing.Point(349, 153);
			this.txt_Max_Value2.Name = "txt_Max_Value2";
			this.txt_Max_Value2.Size = new System.Drawing.Size(55, 21);
			this.txt_Max_Value2.TabIndex = 70;
			this.txt_Max_Value2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Max_Value2_KeyPress);
			this.label31.AutoSize = true;
			this.label31.Font = new System.Drawing.Font("宋体", 14f);
			this.label31.Location = new System.Drawing.Point(258, 153);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(85, 19);
			this.label31.TabIndex = 69;
			this.label31.Text = "最大角度";
			this.txt_Min_Value2.Location = new System.Drawing.Point(178, 153);
			this.txt_Min_Value2.Name = "txt_Min_Value2";
			this.txt_Min_Value2.Size = new System.Drawing.Size(55, 21);
			this.txt_Min_Value2.TabIndex = 68;
			this.txt_Min_Value2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Min_Value2_KeyPress);
			this.label32.AutoSize = true;
			this.label32.Font = new System.Drawing.Font("宋体", 14f);
			this.label32.Location = new System.Drawing.Point(87, 153);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(85, 19);
			this.label32.TabIndex = 67;
			this.label32.Text = "最小角度";
			this.label33.AutoSize = true;
			this.label33.Font = new System.Drawing.Font("宋体", 14f);
			this.label33.Location = new System.Drawing.Point(21, 153);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(76, 19);
			this.label33.TabIndex = 66;
			this.label33.Text = "舵机2：";
			this.txt_Max_Value1.Location = new System.Drawing.Point(349, 124);
			this.txt_Max_Value1.Name = "txt_Max_Value1";
			this.txt_Max_Value1.Size = new System.Drawing.Size(55, 21);
			this.txt_Max_Value1.TabIndex = 65;
			this.txt_Max_Value1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Max_Value1_KeyPress);
			this.label30.AutoSize = true;
			this.label30.Font = new System.Drawing.Font("宋体", 14f);
			this.label30.Location = new System.Drawing.Point(258, 124);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(85, 19);
			this.label30.TabIndex = 64;
			this.label30.Text = "最大角度";
			this.txt_Min_Value1.Location = new System.Drawing.Point(178, 124);
			this.txt_Min_Value1.Name = "txt_Min_Value1";
			this.txt_Min_Value1.Size = new System.Drawing.Size(55, 21);
			this.txt_Min_Value1.TabIndex = 62;
			this.txt_Min_Value1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Min_Value1_KeyPress);
			this.label29.AutoSize = true;
			this.label29.Font = new System.Drawing.Font("宋体", 14f);
			this.label29.Location = new System.Drawing.Point(87, 124);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(85, 19);
			this.label29.TabIndex = 61;
			this.label29.Text = "最小角度";
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("宋体", 14f);
			this.label14.Location = new System.Drawing.Point(21, 124);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(76, 19);
			this.label14.TabIndex = 60;
			this.label14.Text = "舵机1：";
			this.txt_Roll_D_L.Location = new System.Drawing.Point(727, 93);
			this.txt_Roll_D_L.Name = "txt_Roll_D_L";
			this.txt_Roll_D_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Roll_D_L.TabIndex = 59;
			this.txt_Roll_D_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Roll_D_L_KeyPress);
			this.txt_Roll_D.Location = new System.Drawing.Point(581, 93);
			this.txt_Roll_D.Name = "txt_Roll_D";
			this.txt_Roll_D.Size = new System.Drawing.Size(55, 21);
			this.txt_Roll_D.TabIndex = 58;
			this.txt_Roll_D.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Roll_D_KeyPress);
			this.label22.AutoSize = true;
			this.label22.Font = new System.Drawing.Font("宋体", 14f);
			this.label22.Location = new System.Drawing.Point(642, 95);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(79, 19);
			this.label22.TabIndex = 57;
			this.label22.Text = "D-Limit";
			this.label23.AutoSize = true;
			this.label23.Font = new System.Drawing.Font("宋体", 14f);
			this.label23.Location = new System.Drawing.Point(556, 93);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(19, 19);
			this.label23.TabIndex = 56;
			this.label23.Tag = "";
			this.label23.Text = "D";
			this.txt_Roll_I_L.Location = new System.Drawing.Point(495, 93);
			this.txt_Roll_I_L.Name = "txt_Roll_I_L";
			this.txt_Roll_I_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Roll_I_L.TabIndex = 55;
			this.txt_Roll_I_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Roll_I_L_KeyPress);
			this.txt_Roll_I.Location = new System.Drawing.Point(349, 93);
			this.txt_Roll_I.Name = "txt_Roll_I";
			this.txt_Roll_I.Size = new System.Drawing.Size(55, 21);
			this.txt_Roll_I.TabIndex = 54;
			this.txt_Roll_I.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Roll_I_KeyPress);
			this.label24.AutoSize = true;
			this.label24.Font = new System.Drawing.Font("宋体", 14f);
			this.label24.Location = new System.Drawing.Point(410, 95);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(79, 19);
			this.label24.TabIndex = 53;
			this.label24.Text = "I-Limit";
			this.label25.AutoSize = true;
			this.label25.Font = new System.Drawing.Font("宋体", 14f);
			this.label25.Location = new System.Drawing.Point(324, 93);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(19, 19);
			this.label25.TabIndex = 52;
			this.label25.Text = "I";
			this.txt_Roll_P_L.Location = new System.Drawing.Point(248, 91);
			this.txt_Roll_P_L.Name = "txt_Roll_P_L";
			this.txt_Roll_P_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Roll_P_L.TabIndex = 51;
			this.txt_Roll_P_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Roll_P_L_KeyPress);
			this.txt_Roll_P.Location = new System.Drawing.Point(102, 91);
			this.txt_Roll_P.Name = "txt_Roll_P";
			this.txt_Roll_P.Size = new System.Drawing.Size(55, 21);
			this.txt_Roll_P.TabIndex = 50;
			this.txt_Roll_P.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Roll_P_KeyPress);
			this.label26.AutoSize = true;
			this.label26.Font = new System.Drawing.Font("宋体", 14f);
			this.label26.Location = new System.Drawing.Point(163, 93);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(79, 19);
			this.label26.TabIndex = 48;
			this.label26.Text = "P-Limit";
			this.label27.AutoSize = true;
			this.label27.Font = new System.Drawing.Font("宋体", 14f);
			this.label27.Location = new System.Drawing.Point(77, 91);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(19, 19);
			this.label27.TabIndex = 47;
			this.label27.Text = "P";
			this.label28.AutoSize = true;
			this.label28.Font = new System.Drawing.Font("宋体", 14f);
			this.label28.Location = new System.Drawing.Point(21, 90);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(66, 19);
			this.label28.TabIndex = 46;
			this.label28.Text = "横滚：";
			this.txt_Pitch_D_L.Location = new System.Drawing.Point(727, 69);
			this.txt_Pitch_D_L.Name = "txt_Pitch_D_L";
			this.txt_Pitch_D_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Pitch_D_L.TabIndex = 45;
			this.txt_Pitch_D_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Pitch_D_L_KeyPress);
			this.txt_Pitch_D.Location = new System.Drawing.Point(581, 69);
			this.txt_Pitch_D.Name = "txt_Pitch_D";
			this.txt_Pitch_D.Size = new System.Drawing.Size(55, 21);
			this.txt_Pitch_D.TabIndex = 44;
			this.txt_Pitch_D.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Pitch_D_KeyPress);
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("宋体", 14f);
			this.label15.Location = new System.Drawing.Point(642, 71);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(79, 19);
			this.label15.TabIndex = 43;
			this.label15.Text = "D-Limit";
			this.label16.AutoSize = true;
			this.label16.Font = new System.Drawing.Font("宋体", 14f);
			this.label16.Location = new System.Drawing.Point(556, 69);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(19, 19);
			this.label16.TabIndex = 42;
			this.label16.Tag = "";
			this.label16.Text = "D";
			this.txt_Pitch_I_L.Location = new System.Drawing.Point(495, 69);
			this.txt_Pitch_I_L.Name = "txt_Pitch_I_L";
			this.txt_Pitch_I_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Pitch_I_L.TabIndex = 41;
			this.txt_Pitch_I_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Pitch_I_L_KeyPress);
			this.txt_Pitch_I.Location = new System.Drawing.Point(349, 69);
			this.txt_Pitch_I.Name = "txt_Pitch_I";
			this.txt_Pitch_I.Size = new System.Drawing.Size(55, 21);
			this.txt_Pitch_I.TabIndex = 40;
			this.txt_Pitch_I.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Pitch_I_KeyPress);
			this.label17.AutoSize = true;
			this.label17.Font = new System.Drawing.Font("宋体", 14f);
			this.label17.Location = new System.Drawing.Point(410, 71);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(79, 19);
			this.label17.TabIndex = 39;
			this.label17.Text = "I-Limit";
			this.label18.AutoSize = true;
			this.label18.Font = new System.Drawing.Font("宋体", 14f);
			this.label18.Location = new System.Drawing.Point(324, 69);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(19, 19);
			this.label18.TabIndex = 38;
			this.label18.Text = "I";
			this.txt_Pitch_P_L.Location = new System.Drawing.Point(248, 67);
			this.txt_Pitch_P_L.Name = "txt_Pitch_P_L";
			this.txt_Pitch_P_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Pitch_P_L.TabIndex = 37;
			this.txt_Pitch_P_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Pitch_P_L_KeyPress);
			this.txt_Pitch_P.Location = new System.Drawing.Point(102, 67);
			this.txt_Pitch_P.Name = "txt_Pitch_P";
			this.txt_Pitch_P.Size = new System.Drawing.Size(55, 21);
			this.txt_Pitch_P.TabIndex = 36;
			this.txt_Pitch_P.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Pitch_P_KeyPress);
			this.label19.AutoSize = true;
			this.label19.Font = new System.Drawing.Font("宋体", 14f);
			this.label19.Location = new System.Drawing.Point(163, 69);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(79, 19);
			this.label19.TabIndex = 34;
			this.label19.Text = "P-Limit";
			this.label20.AutoSize = true;
			this.label20.Font = new System.Drawing.Font("宋体", 14f);
			this.label20.Location = new System.Drawing.Point(77, 67);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(19, 19);
			this.label20.TabIndex = 33;
			this.label20.Text = "P";
			this.label21.AutoSize = true;
			this.label21.Font = new System.Drawing.Font("宋体", 14f);
			this.label21.Location = new System.Drawing.Point(21, 66);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(66, 19);
			this.label21.TabIndex = 32;
			this.label21.Text = "俯仰：";
			this.txt_Boat_D_L.Location = new System.Drawing.Point(727, 45);
			this.txt_Boat_D_L.Name = "txt_Boat_D_L";
			this.txt_Boat_D_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Boat_D_L.TabIndex = 31;
			this.txt_Boat_D_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Boat_D_L_KeyPress);
			this.txt_Boat_D.Location = new System.Drawing.Point(581, 45);
			this.txt_Boat_D.Name = "txt_Boat_D";
			this.txt_Boat_D.Size = new System.Drawing.Size(55, 21);
			this.txt_Boat_D.TabIndex = 10;
			this.txt_Boat_D.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Boat_D_KeyPress);
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("宋体", 14f);
			this.label8.Location = new System.Drawing.Point(642, 47);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(79, 19);
			this.label8.TabIndex = 29;
			this.label8.Text = "D-Limit";
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("宋体", 14f);
			this.label9.Location = new System.Drawing.Point(556, 45);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(19, 19);
			this.label9.TabIndex = 28;
			this.label9.Tag = "";
			this.label9.Text = "D";
			this.txt_Boat_I_L.Location = new System.Drawing.Point(495, 45);
			this.txt_Boat_I_L.Name = "txt_Boat_I_L";
			this.txt_Boat_I_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Boat_I_L.TabIndex = 9;
			this.txt_Boat_I_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Boat_I_L_KeyPress);
			this.txt_Boat_I.Location = new System.Drawing.Point(349, 45);
			this.txt_Boat_I.Name = "txt_Boat_I";
			this.txt_Boat_I.Size = new System.Drawing.Size(55, 21);
			this.txt_Boat_I.TabIndex = 8;
			this.txt_Boat_I.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Boat_I_KeyPress);
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("宋体", 14f);
			this.label10.Location = new System.Drawing.Point(410, 47);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(79, 19);
			this.label10.TabIndex = 25;
			this.label10.Text = "I-Limit";
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("宋体", 14f);
			this.label11.Location = new System.Drawing.Point(324, 45);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(19, 19);
			this.label11.TabIndex = 24;
			this.label11.Text = "I";
			this.txt_Boat_P_L.Location = new System.Drawing.Point(248, 43);
			this.txt_Boat_P_L.Name = "txt_Boat_P_L";
			this.txt_Boat_P_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Boat_P_L.TabIndex = 7;
			this.txt_Boat_P_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Boat_P_L_KeyPress);
			this.txt_Boat_P.Location = new System.Drawing.Point(102, 43);
			this.txt_Boat_P.Name = "txt_Boat_P";
			this.txt_Boat_P.Size = new System.Drawing.Size(55, 21);
			this.txt_Boat_P.TabIndex = 6;
			this.txt_Boat_P.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Boat_P_KeyPress);
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("宋体", 14f);
			this.label12.Location = new System.Drawing.Point(163, 45);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(79, 19);
			this.label12.TabIndex = 20;
			this.label12.Text = "P-Limit";
			this.label13.AutoSize = true;
			this.label13.Font = new System.Drawing.Font("宋体", 14f);
			this.label13.Location = new System.Drawing.Point(77, 43);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(19, 19);
			this.label13.TabIndex = 19;
			this.label13.Text = "P";
			this.lbl_Boat.AutoSize = true;
			this.lbl_Boat.Font = new System.Drawing.Font("宋体", 14f);
			this.lbl_Boat.Location = new System.Drawing.Point(21, 42);
			this.lbl_Boat.Name = "lbl_Boat";
			this.lbl_Boat.Size = new System.Drawing.Size(66, 19);
			this.lbl_Boat.TabIndex = 18;
			this.lbl_Boat.Text = "航向：";
			this.txt_Depth_D_L.Location = new System.Drawing.Point(727, 21);
			this.txt_Depth_D_L.Name = "txt_Depth_D_L";
			this.txt_Depth_D_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Depth_D_L.TabIndex = 5;
			this.txt_Depth_D_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Depth_D_L_KeyPress);
			this.txt_Depth_D.Location = new System.Drawing.Point(581, 21);
			this.txt_Depth_D.Name = "txt_Depth_D";
			this.txt_Depth_D.Size = new System.Drawing.Size(55, 21);
			this.txt_Depth_D.TabIndex = 4;
			this.txt_Depth_D.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Depth_D_KeyPress);
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("宋体", 14f);
			this.label6.Location = new System.Drawing.Point(642, 23);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(79, 19);
			this.label6.TabIndex = 15;
			this.label6.Text = "D-Limit";
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("宋体", 14f);
			this.label7.Location = new System.Drawing.Point(556, 21);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(19, 19);
			this.label7.TabIndex = 14;
			this.label7.Tag = "";
			this.label7.Text = "D";
			this.txt_Depth_I_L.Location = new System.Drawing.Point(495, 21);
			this.txt_Depth_I_L.Name = "txt_Depth_I_L";
			this.txt_Depth_I_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Depth_I_L.TabIndex = 3;
			this.txt_Depth_I_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Depth_I_L_KeyPress);
			this.txt_Depth_I.Location = new System.Drawing.Point(349, 21);
			this.txt_Depth_I.Name = "txt_Depth_I";
			this.txt_Depth_I.Size = new System.Drawing.Size(55, 21);
			this.txt_Depth_I.TabIndex = 2;
			this.txt_Depth_I.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Depth_I_KeyPress);
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("宋体", 14f);
			this.label4.Location = new System.Drawing.Point(410, 23);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(79, 19);
			this.label4.TabIndex = 11;
			this.label4.Text = "I-Limit";
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("宋体", 14f);
			this.label5.Location = new System.Drawing.Point(324, 21);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(19, 19);
			this.label5.TabIndex = 10;
			this.label5.Text = "I";
			this.txt_Depth_P_L.Location = new System.Drawing.Point(248, 19);
			this.txt_Depth_P_L.Name = "txt_Depth_P_L";
			this.txt_Depth_P_L.Size = new System.Drawing.Size(55, 21);
			this.txt_Depth_P_L.TabIndex = 1;
			this.txt_Depth_P_L.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Depth_P_L_KeyPress);
			this.txt_Depth_P.Location = new System.Drawing.Point(102, 19);
			this.txt_Depth_P.Name = "txt_Depth_P";
			this.txt_Depth_P.Size = new System.Drawing.Size(55, 21);
			this.txt_Depth_P.TabIndex = 0;
			this.txt_Depth_P.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txt_Depth_P_KeyPress);
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("宋体", 14f);
			this.label3.Location = new System.Drawing.Point(163, 21);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(79, 19);
			this.label3.TabIndex = 2;
			this.label3.Text = "P-Limit";
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("宋体", 14f);
			this.label2.Location = new System.Drawing.Point(77, 19);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(19, 19);
			this.label2.TabIndex = 1;
			this.label2.Text = "P";
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("宋体", 14f);
			this.label1.Location = new System.Drawing.Point(21, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 19);
			this.label1.TabIndex = 0;
			this.label1.Text = "深度：";
			this.trm_SendPID.Tick += new System.EventHandler(trm_SendPID_Tick);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(804, 223);
			base.Controls.Add(this.panel1);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(820, 262);
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(820, 262);
			base.Name = "frm_DebugMode";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "调试模式";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frm_DebugMode_FormClosing);
			base.Load += new System.EventHandler(frm_DebugMode_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}

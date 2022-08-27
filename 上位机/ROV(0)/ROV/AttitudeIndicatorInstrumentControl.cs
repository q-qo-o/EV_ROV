using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ROV.AvionicsInstrumentsControls;

namespace ROV
{
	internal class AttitudeIndicatorInstrumentControl : InstrumentControl
	{
		private double PitchAngle = 0.0;

		private double RollAngle = 0.0;

		private Bitmap bmpCadran = new Bitmap(AvionicsInstrumentsControlsRessources.Horizon_Background);

		private Bitmap bmpBoule = new Bitmap(AvionicsInstrumentsControlsRessources.Horizon_GroundSky);

		private Bitmap bmpAvion = new Bitmap(AvionicsInstrumentsControlsRessources.Maquette_Avion);

		private Container components = null;

		public AttitudeIndicatorInstrumentControl()
		{
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			Point ptBoule = new Point(25, -210);
			Point ptRotation = new Point(150, 150);
			float scale = (float)base.Width / (float)bmpCadran.Width;
			bmpCadran.MakeTransparent(Color.Yellow);
			bmpAvion.MakeTransparent(Color.Yellow);
			RotateAndTranslate(pe, bmpBoule, RollAngle, 0.0, ptBoule, (int)(4.0 * PitchAngle), ptRotation, scale);
			Pen maskPen = new Pen(BackColor, 30f * scale);
			pe.Graphics.DrawRectangle(maskPen, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			pe.Graphics.DrawImage(bmpCadran, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			pe.Graphics.DrawImage(bmpAvion, (float)((0.5 * (double)bmpCadran.Width - 0.5 * (double)bmpAvion.Width) * (double)scale), (float)((0.5 * (double)bmpCadran.Height - 0.5 * (double)bmpAvion.Height) * (double)scale), (float)bmpAvion.Width * scale, (float)bmpAvion.Height * scale);
		}

		public void SetAttitudeIndicatorParameters(double aircraftPitchAngle, double aircraftRollAngle)
		{
			PitchAngle = aircraftPitchAngle;
			RollAngle = aircraftRollAngle * Math.PI / 180.0;
			Refresh();
		}
	}
}

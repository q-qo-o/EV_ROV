using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ROV.AvionicsInstrumentsControls;

namespace ROV
{
	internal class VerticalSpeedIndicatorInstrumentControl : InstrumentControl
	{
		private int verticalSpeed;

		private Bitmap bmpCadran = new Bitmap(AvionicsInstrumentsControlsRessources.VerticalSpeedIndicator_Background);

		private Bitmap bmpNeedle = new Bitmap(AvionicsInstrumentsControlsRessources.VerticalSpeedNeedle);

		private Container components = null;

		public VerticalSpeedIndicatorInstrumentControl()
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
			Point ptRotation = new Point(150, 150);
			Point ptimgNeedle = new Point(136, 39);
			bmpCadran.MakeTransparent(Color.Yellow);
			bmpNeedle.MakeTransparent(Color.Yellow);
			double alphaNeedle = InterpolPhyToAngle(verticalSpeed, -6000f, 6000f, 120f, 420f);
			float scale = (float)base.Width / (float)bmpCadran.Width;
			Pen maskPen = new Pen(BackColor, 30f * scale);
			pe.Graphics.DrawRectangle(maskPen, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			pe.Graphics.DrawImage(bmpCadran, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			RotateImage(pe, bmpNeedle, alphaNeedle, ptimgNeedle, ptRotation, scale);
		}

		public void SetVerticalSpeedIndicatorParameters(int aircraftVerticalSpeed)
		{
			verticalSpeed = aircraftVerticalSpeed;
			Refresh();
		}
	}
}

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ROV.AvionicsInstrumentsControls;

namespace ROV
{
	internal class AirSpeedIndicatorInstrumentControl : InstrumentControl
	{
		private int airSpeed;

		private Bitmap bmpCadran = new Bitmap(AvionicsInstrumentsControlsRessources.AirSpeedIndicator_Background);

		private Bitmap bmpNeedle = new Bitmap(AvionicsInstrumentsControlsRessources.AirSpeedNeedle);

		private Container components = null;

		public AirSpeedIndicatorInstrumentControl()
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
			double alphaNeedle = InterpolPhyToAngle(airSpeed, 0f, 800f, 180f, 468f);
			float scale = (float)base.Width / (float)bmpCadran.Width;
			Pen maskPen = new Pen(BackColor, 30f * scale);
			pe.Graphics.DrawRectangle(maskPen, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			pe.Graphics.DrawImage(bmpCadran, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			RotateImage(pe, bmpNeedle, alphaNeedle, ptimgNeedle, ptRotation, scale);
		}

		public void SetAirSpeedIndicatorParameters(int aircraftAirSpeed)
		{
			airSpeed = aircraftAirSpeed;
			Refresh();
		}
	}
}

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ROV.AvionicsInstrumentsControls;

namespace ROV
{
	internal class HeadingIndicatorInstrumentControl : InstrumentControl
	{
		private int Heading;

		private Bitmap bmpCadran = new Bitmap(AvionicsInstrumentsControlsRessources.HeadingIndicator_Background);

		private Bitmap bmpHedingWeel = new Bitmap(AvionicsInstrumentsControlsRessources.HeadingWeel);

		private Bitmap bmpAircaft = new Bitmap(AvionicsInstrumentsControlsRessources.HeadingIndicator_Aircraft);

		private Container components = null;

		public HeadingIndicatorInstrumentControl()
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
			Point ptImgAircraft = new Point(73, 41);
			Point ptImgHeadingWeel = new Point(13, 13);
			bmpCadran.MakeTransparent(Color.Yellow);
			bmpHedingWeel.MakeTransparent(Color.Yellow);
			bmpAircaft.MakeTransparent(Color.Yellow);
			double alphaHeadingWeel = InterpolPhyToAngle(Heading, 0f, 360f, 360f, 0f);
			float scale = (float)base.Width / (float)bmpCadran.Width;
			Pen maskPen = new Pen(BackColor, 30f * scale);
			pe.Graphics.DrawRectangle(maskPen, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			pe.Graphics.DrawImage(bmpCadran, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			RotateImage(pe, bmpAircaft, alphaHeadingWeel, ptImgAircraft, ptRotation, scale);
			pe.Graphics.DrawImage(bmpHedingWeel, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
		}

		public void SetHeadingIndicatorParameters(int aircraftHeading)
		{
			Heading = aircraftHeading;
			Refresh();
		}
	}
}

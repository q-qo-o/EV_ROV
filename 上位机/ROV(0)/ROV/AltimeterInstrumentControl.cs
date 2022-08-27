using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ROV.AvionicsInstrumentsControls;

namespace ROV
{
	internal class AltimeterInstrumentControl : InstrumentControl
	{
		private int altitude;

		private Bitmap bmpCadran = new Bitmap(AvionicsInstrumentsControlsRessources.Altimeter_Background);

		private Bitmap bmpSmallNeedle = new Bitmap(AvionicsInstrumentsControlsRessources.SmallNeedleAltimeter);

		private Bitmap bmpLongNeedle = new Bitmap(AvionicsInstrumentsControlsRessources.LongNeedleAltimeter);

		private Bitmap bmpScroll = new Bitmap(AvionicsInstrumentsControlsRessources.Bandeau_Drouleur);

		private Container components = null;

		public AltimeterInstrumentControl()
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
			Point ptCounter = new Point(35, 135);
			Point ptRotation = new Point(150, 150);
			Point ptimgNeedle = new Point(136, 39);
			bmpCadran.MakeTransparent(Color.Yellow);
			bmpLongNeedle.MakeTransparent(Color.Yellow);
			bmpSmallNeedle.MakeTransparent(Color.Yellow);
			double alphaSmallNeedle = InterpolPhyToAngle(altitude, 0f, 10000f, 0f, 359f);
			double alphaLongNeedle = InterpolPhyToAngle(altitude % 1000, 0f, 1000f, 0f, 359f);
			float scale = (float)base.Width / (float)bmpCadran.Width;
			ScrollCounter(pe, bmpScroll, 5, altitude, ptCounter, scale);
			Pen maskPen = new Pen(BackColor, 30f * scale);
			pe.Graphics.DrawRectangle(maskPen, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			pe.Graphics.DrawImage(bmpCadran, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			RotateImage(pe, bmpSmallNeedle, alphaSmallNeedle, ptimgNeedle, ptRotation, scale);
			RotateImage(pe, bmpLongNeedle, alphaLongNeedle, ptimgNeedle, ptRotation, scale);
		}

		public void SetAlimeterParameters(int aircraftAltitude)
		{
			altitude = aircraftAltitude;
			Refresh();
		}
	}
}

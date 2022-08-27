using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ROV.AvionicsInstrumentsControls;

namespace ROV
{
	internal class TurnCoordinatorInstrumentControl : InstrumentControl
	{
		private float TurnRate;

		private float TurnQuality;

		private Bitmap bmpCadran = new Bitmap(AvionicsInstrumentsControlsRessources.TurnCoordinator_Background);

		private Bitmap bmpBall = new Bitmap(AvionicsInstrumentsControlsRessources.TurnCoordinatorBall);

		private Bitmap bmpAircraft = new Bitmap(AvionicsInstrumentsControlsRessources.TurnCoordinatorAircraft);

		private Bitmap bmpMarks = new Bitmap(AvionicsInstrumentsControlsRessources.TurnCoordinatorMarks);

		private Container components = null;

		public TurnCoordinatorInstrumentControl()
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
			Point ptRotationAircraft = new Point(150, 150);
			Point ptImgAircraft = new Point(57, 114);
			Point ptRotationBall = new Point(150, -155);
			Point ptImgBall = new Point(136, 216);
			Point ptMarks = new Point(134, 216);
			bmpCadran.MakeTransparent(Color.Yellow);
			bmpBall.MakeTransparent(Color.Yellow);
			bmpAircraft.MakeTransparent(Color.Yellow);
			bmpMarks.MakeTransparent(Color.Yellow);
			double alphaAircraft = InterpolPhyToAngle(TurnRate, -6f, 6f, -30f, 30f);
			double alphaBall = InterpolPhyToAngle(TurnQuality, -10f, 10f, -11f, 11f);
			float scale = (float)base.Width / (float)bmpCadran.Width;
			Pen maskPen = new Pen(BackColor, 30f * scale);
			pe.Graphics.DrawRectangle(maskPen, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			pe.Graphics.DrawImage(bmpCadran, 0f, 0f, (float)bmpCadran.Width * scale, (float)bmpCadran.Height * scale);
			RotateImage(pe, bmpBall, alphaBall, ptImgBall, ptRotationBall, scale);
			RotateImage(pe, bmpAircraft, alphaAircraft, ptImgAircraft, ptRotationAircraft, scale);
			pe.Graphics.DrawImage(bmpMarks, (int)((float)ptMarks.X * scale), (int)((float)ptMarks.Y * scale), (float)bmpMarks.Width * scale, (float)bmpMarks.Height * scale);
		}

		public void SetTurnCoordinatorParameters(float aircraftTurnRate, float aircraftTurnQuality)
		{
			TurnRate = aircraftTurnRate;
			TurnQuality = aircraftTurnQuality;
			Refresh();
		}
	}
}

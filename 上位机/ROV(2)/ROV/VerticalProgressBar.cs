using System.Windows.Forms;

namespace ROV
{
	public class VerticalProgressBar : ProgressBar
	{
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.Style |= 4;
				return cp;
			}
		}
	}
}

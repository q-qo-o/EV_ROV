using System.Drawing;

namespace ROV
{
	internal struct InstrumentControlMarksDefinition
	{
		internal float minPhy;

		internal float minAngle;

		internal float maxPhy;

		internal float maxAngle;

		internal int numberOfDivisions;

		internal int fontSize;

		internal Color fontColor;

		internal InstumentMarkScaleStyle scaleStyle;

		public InstrumentControlMarksDefinition(float myMinPhy, float myMinAngle, float myMaxPhy, float myMaxAngle, int myNumberOfDivisions, int myFontSize, Color myFontColor, InstumentMarkScaleStyle myScaleStyle)
		{
			minPhy = myMinPhy;
			minAngle = myMinAngle;
			maxPhy = myMaxPhy;
			maxAngle = myMaxAngle;
			numberOfDivisions = myNumberOfDivisions;
			fontSize = myFontSize;
			fontColor = myFontColor;
			scaleStyle = myScaleStyle;
		}
	}
}

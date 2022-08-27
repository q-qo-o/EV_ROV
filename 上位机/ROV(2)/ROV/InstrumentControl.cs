using System;
using System.Drawing;
using System.Windows.Forms;

namespace ROV
{
	internal class InstrumentControl : Control
	{
		protected void RotateImage(PaintEventArgs pe, Image img, double alpha, Point ptImg, Point ptRot, float scaleFactor)
		{
			double beta = 0.0;
			double d = 0.0;
			float deltaX = 0f;
			float deltaY = 0f;
			if (ptImg != ptRot)
			{
				if (ptRot.X != 0)
				{
					beta = Math.Atan((double)ptRot.Y / (double)ptRot.X);
				}
				d = Math.Sqrt(ptRot.X * ptRot.X + ptRot.Y * ptRot.Y);
				deltaX = (float)(d * (Math.Cos(alpha - beta) - Math.Cos(alpha) * Math.Cos(alpha + beta) - Math.Sin(alpha) * Math.Sin(alpha + beta)));
				deltaY = (float)(d * (Math.Sin(beta - alpha) + Math.Sin(alpha) * Math.Cos(alpha + beta) - Math.Cos(alpha) * Math.Sin(alpha + beta)));
			}
			pe.Graphics.RotateTransform((float)(alpha * 180.0 / Math.PI));
			pe.Graphics.DrawImage(img, ((float)ptImg.X + deltaX) * scaleFactor, ((float)ptImg.Y + deltaY) * scaleFactor, (float)img.Width * scaleFactor, (float)img.Height * scaleFactor);
			pe.Graphics.RotateTransform((float)((0.0 - alpha) * 180.0 / Math.PI));
		}

		protected void TranslateImage(PaintEventArgs pe, Image img, int deltaPx, float alpha, Point ptImg, float scaleFactor)
		{
			float deltaX = (float)((double)deltaPx * Math.Sin(alpha));
			float deltaY = (float)((double)(-deltaPx) * Math.Cos(alpha));
			pe.Graphics.DrawImage(img, ((float)ptImg.X + deltaX) * scaleFactor, ((float)ptImg.Y + deltaY) * scaleFactor, (float)img.Width * scaleFactor, (float)img.Height * scaleFactor);
		}

		protected void RotateAndTranslate(PaintEventArgs pe, Image img, double alphaRot, double alphaTrs, Point ptImg, int deltaPx, Point ptRot, float scaleFactor)
		{
			double beta = 0.0;
			double d = 0.0;
			float deltaXRot = 0f;
			float deltaYRot = 0f;
			float deltaXTrs = 0f;
			float deltaYTrs = 0f;
			if (ptImg != ptRot)
			{
				if (ptRot.X != 0)
				{
					beta = Math.Atan((double)ptRot.Y / (double)ptRot.X);
				}
				d = Math.Sqrt(ptRot.X * ptRot.X + ptRot.Y * ptRot.Y);
				deltaXRot = (float)(d * (Math.Cos(alphaRot - beta) - Math.Cos(alphaRot) * Math.Cos(alphaRot + beta) - Math.Sin(alphaRot) * Math.Sin(alphaRot + beta)));
				deltaYRot = (float)(d * (Math.Sin(beta - alphaRot) + Math.Sin(alphaRot) * Math.Cos(alphaRot + beta) - Math.Cos(alphaRot) * Math.Sin(alphaRot + beta)));
			}
			deltaXTrs = (float)((double)deltaPx * Math.Sin(alphaTrs));
			deltaYTrs = (float)((double)(-deltaPx) * (0.0 - Math.Cos(alphaTrs)));
			pe.Graphics.RotateTransform((float)(alphaRot * 180.0 / Math.PI));
			pe.Graphics.DrawImage(img, ((float)ptImg.X + deltaXRot + deltaXTrs) * scaleFactor, ((float)ptImg.Y + deltaYRot + deltaYTrs) * scaleFactor, (float)img.Width * scaleFactor, (float)img.Height * scaleFactor);
			pe.Graphics.RotateTransform((float)((0.0 - alphaRot) * 180.0 / Math.PI));
		}

		protected void ScrollCounter(PaintEventArgs pe, Image imgBand, int nbOfDigits, int counterValue, Point ptImg, float scaleFactor)
		{
			int indexDigit = 0;
			int digitBoxHeight = imgBand.Height / 11;
			int digitBoxWidth = imgBand.Width;
			for (indexDigit = 0; indexDigit < nbOfDigits; indexDigit++)
			{
				int currentDigit = (int)((double)counterValue / Math.Pow(10.0, indexDigit) % 10.0);
				int prevDigit = ((indexDigit != 0) ? ((int)((double)counterValue / Math.Pow(10.0, indexDigit - 1) % 10.0)) : 0);
				int xOffset = digitBoxWidth * (nbOfDigits - indexDigit - 1);
				int yOffset;
				if (prevDigit == 9)
				{
					double fader = 0.33;
					yOffset = (int)(0.0 - (fader + (double)currentDigit) * (double)digitBoxHeight);
				}
				else
				{
					yOffset = -(currentDigit * digitBoxHeight);
				}
				pe.Graphics.DrawImage(imgBand, (float)(ptImg.X + xOffset) * scaleFactor, (float)(ptImg.Y + yOffset) * scaleFactor, (float)imgBand.Width * scaleFactor, (float)imgBand.Height * scaleFactor);
			}
		}

		protected void DisplayRoundMark(PaintEventArgs pe, Image imgMark, InstrumentControlMarksDefinition insControlMarksDefinition, Point ptImg, int radiusPx, bool displayText, float scaleFactor)
		{
			int textBoxHeight = (int)((double)insControlMarksDefinition.fontSize * 1.1 / (double)scaleFactor);
			Point textPoint = default(Point);
			Point rotatePoint = default(Point);
			Font markFont = new Font("Arial", insControlMarksDefinition.fontSize);
			SolidBrush markBrush = new SolidBrush(insControlMarksDefinition.fontColor);
			InstrumentControlMarkPoint[] markArray = new InstrumentControlMarkPoint[2 + insControlMarksDefinition.numberOfDivisions];
			markArray[0].value = insControlMarksDefinition.minPhy;
			markArray[0].angle = insControlMarksDefinition.minAngle;
			markArray[markArray.Length - 1].value = insControlMarksDefinition.maxPhy;
			markArray[markArray.Length - 1].angle = insControlMarksDefinition.maxAngle;
			for (int index = 1; index < insControlMarksDefinition.numberOfDivisions + 1; index++)
			{
				markArray[index].value = (insControlMarksDefinition.maxPhy - insControlMarksDefinition.minPhy) / (float)(insControlMarksDefinition.numberOfDivisions + 1) * (float)index + insControlMarksDefinition.minPhy;
				markArray[index].angle = (insControlMarksDefinition.maxAngle - insControlMarksDefinition.minAngle) / (float)(insControlMarksDefinition.numberOfDivisions + 1) * (float)index + insControlMarksDefinition.minAngle;
			}
			rotatePoint.X = (int)((float)(base.Width / 2) / scaleFactor);
			rotatePoint.Y = rotatePoint.X;
			InstrumentControlMarkPoint[] array = markArray;
			for (int i = 0; i < array.Length; i++)
			{
				InstrumentControlMarkPoint markPoint = array[i];
				double alphaRot = Math.PI / 2.0 - (double)markPoint.angle;
				int textBoxLength = (int)((double)(Convert.ToString(markPoint.value).Length * insControlMarksDefinition.fontSize) * 0.8 / (double)scaleFactor);
				int textPointRadiusPx = (int)((double)radiusPx - 1.2 * (double)imgMark.Height - 0.5 * (double)textBoxLength);
				textPoint.X = (int)(((double)textPointRadiusPx * Math.Cos(markPoint.angle) - 0.5 * (double)textBoxLength + (double)rotatePoint.X) * (double)scaleFactor);
				textPoint.Y = (int)(((double)(-textPointRadiusPx) * Math.Sin(markPoint.angle) - 0.5 * (double)textBoxHeight + (double)rotatePoint.Y) * (double)scaleFactor);
				RotateImage(pe, imgMark, alphaRot, ptImg, rotatePoint, scaleFactor);
				if (displayText)
				{
					pe.Graphics.DrawString(Convert.ToString(markPoint.value), markFont, markBrush, textPoint);
				}
			}
		}

		protected float InterpolPhyToAngle(float phyVal, float minPhy, float maxPhy, float minAngle, float maxAngle)
		{
			if (phyVal < minPhy)
			{
				return (float)((double)minAngle * Math.PI / 180.0);
			}
			if (phyVal > maxPhy)
			{
				return (float)((double)maxAngle * Math.PI / 180.0);
			}
			float a = (maxAngle - minAngle) / (maxPhy - minPhy);
			float b = (float)(0.5 * (double)(maxAngle + minAngle - a * (maxPhy + minPhy)));
			float y = a * phyVal + b;
			return (float)((double)y * Math.PI / 180.0);
		}

		protected Point FromCartRefToImgRef(Point cartPoint)
		{
			Point imgPoint = default(Point);
			imgPoint.X = cartPoint.X + base.Width / 2;
			imgPoint.Y = -cartPoint.Y + base.Height / 2;
			return imgPoint;
		}

		protected double FromDegToRad(double degAngle)
		{
			return degAngle * Math.PI / 180.0;
		}
	}
}

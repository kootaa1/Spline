using System;

namespace Spline
{
    public class Palitra
    {
        //OpenGL gl;
        const byte MaxPixelColorValue = 255;
        const double Eps = 0.01;
        public double red;
        public double blue;
        public double yellow;
        public double green;
        public double minblue;
        public bool isReadyToDraw;

        public Palitra()
        {
            //gl = openGL;
            isReadyToDraw = false;
        }

        public void setColorValues(double min, double max)
        {
            red = max;
            minblue = min;
            blue = min + (max - min) / 4;
            green = blue + (max - min) / 2;
            yellow = min + 3 * (max - min) / 4;
            isReadyToDraw = true;
        }
        public void ColorCalculate(double value, out byte r, out byte g, out byte b)
        {
            byte z;
            if (value <= minblue)
            {
                r = 0;
                g = 0;
                b = MaxPixelColorValue;
                return;
            }
            else if (value <= blue)
            {
                z = (byte)(MaxPixelColorValue * (Math.Abs(blue - minblue) - Math.Abs(value - minblue)) / Math.Abs(blue - minblue));
                r = 0;
                g = (byte)(MaxPixelColorValue - z);
                b = MaxPixelColorValue;
                return;

            }
            else if (value <= green - Eps)
            {
                b = (byte)(MaxPixelColorValue * (Math.Abs(green - blue) - Math.Abs(value - blue)) / Math.Abs(green - blue));
                r = 0;
                g = MaxPixelColorValue;
                //b = z;
                return;
            }
            else if (value <= yellow)
            {
                z = (byte)(MaxPixelColorValue * (Math.Abs(yellow - green) - Math.Abs(value - green)) / Math.Abs(yellow - green));
                r = (byte)(MaxPixelColorValue - z);
                g = MaxPixelColorValue;
                b = 0;
                return;
            }
            else
            {
                g = (byte)(MaxPixelColorValue * (Math.Abs(red - yellow) - Math.Abs(value - yellow)) / Math.Abs(red - yellow));
                r = MaxPixelColorValue;
                b = 0;
                return;
            }
        }
    }
}
using System;
namespace Spline
{
    public class Palitra
    {
        //OpenGL gl;
        double red;
        double blue;
        double yellow;
        double green;
        double minblue;

        public Palitra(double max, double min)
        {
            gl = openGL;
            red = max;
            minblue = min;
            blue = min + (max - min) / 4;
            green = blue + (max - min) / 2;
            yellow = min + 3 * (max - min) / 4;

        }

        void ColorCalculate(double value, out byte r, out byte g, out byte b)
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
                z = (byte)(MaxPixelColorValue * (abs(blue - minblue) - abs(value - minblue)) / abs(blue - minblue));
                r = 0;
                g = (byte)(MaxPixelColorValue - z);
                b = MaxPixelColorValue;
                return;

            }
            else if (value <= green - Eps)
            {
                b = (byte)(MaxPixelColorValue * (abs(green - blue) - abs(value - blue)) / abs(green - blue));
                r = 0;
                g = MaxPixelColorValue;
                //b = z;
                return;
            }
            else if (value <= yellow)
            {
                z = (byte)(MaxPixelColorValue * (abs(yellow - green) - abs(value - green)) / abs(yellow - green));
                r = (byte)(MaxPixelColorValue - z);
                g = MaxPixelColorValue;
                b = 0;
                return;
            }
            else
            {
                g = (byte)(MaxPixelColorValue * (abs(red - yellow) - abs(value - yellow)) / abs(red - yellow));
                r = MaxPixelColorValue;
                b = 0;
                return;
            }
        }
    }
}

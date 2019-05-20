using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

namespace Spline.sources
{
    public class Grid
    {
        public struct Point
        {
            public double x;
            public double y;

            public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }
        public Basis basis;
        public List<Point> points;
        public List<double> omega;
        public List<double> x, y, f;
        public int Qx, Qy, Qf;

        public Grid()
        {
            points = new List<Point>();
            omega = new List<double>();
            x = new List<double>();
            y = new List<double>();
            f = new List<double>();
        }

        public int getXSize()
        {
            return x.Count;
        }
        public int getYSize()
        {
            return x.Count;
        }
        public bool inputSpline(string path)
        {
            try
            {
                StreamReader sr = new StreamReader(path);
                string buf = sr.ReadLine();
                Qx = int.Parse(buf);
                buf = sr.ReadLine();
                string[] splitArray = buf.Split();
                foreach (string s in splitArray)
                {
                    x.Add(double.Parse(s, CultureInfo.InvariantCulture));
                }
                buf = sr.ReadLine();
                Qy = int.Parse(buf);
                buf = sr.ReadLine();
                splitArray = buf.Split();
                foreach (string s in splitArray)
                {
                    y.Add(double.Parse(s, CultureInfo.InvariantCulture));
                }
                buf = sr.ReadLine();
                Qf = int.Parse(buf);
                Array.Resize(ref splitArray, 3);
                for (int i = 0; i < Qf; i++)
                {
                    buf = sr.ReadLine();
                    splitArray = buf.Split();
                    points.Add(new Point(double.Parse(splitArray[0], CultureInfo.InvariantCulture),
                        double.Parse(splitArray[1], CultureInfo.InvariantCulture)));
                    f.Add(double.Parse(splitArray[2], CultureInfo.InvariantCulture));
                    omega.Add(1);
                } 
                return true;
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException || e is InvalidDataException
                    || e is FormatException || e is OverflowException)
                {
                    MessageBox.Show("Ошибка во входных данных, перепроверь. \n" + e.ToString());
                    return false;
                }
            }
            return false;
        }

        public int calculatePosition(int i, int j)
        {
            if (i >= x.Count || j >= y.Count || i < 0 || j < 0)
                return -1;
            int k = j * x.Count + i;
            if (k >= 0)
                return k;
            else
                return -1;
        }
    }
}
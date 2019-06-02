using Microsoft.Win32;
using SharpGL;
using SharpGL.SceneGraph;
using Spline.sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Spline
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FilePath { get; set; }
        public OpenGL gl, legend;
        public bool isReadyToDrawing;
        MyTask task;
        Palitra palitra;
        double[] xPoints, yPoints;
        double[,] z;
        double scaleX, scaleY, scaleZ;
        double translateX, translateY, translateZ;
        private bool isMiddleButton;
        private bool isRightButton;
        private double prevPositionX;
        private double prevPositionY;
        private bool needInit;
        private double xPointforCalculating;
        private double yPointforCalculating;
        private bool drowFunctionValue;
        private const int MAX_ANGLE = 360;
        private const int MIN_ANGLE = 0;
        double minZValue, maxZValue;
        private int xSplit, ySplit;
        //private bool isRotate;

        public MainWindow()
        {
            InitializeComponent();
            palitra = new Palitra();
            task = new MyTask();
            XRotate.Value = 300; YRotate.Value = 0; ZRotate.Value = 40;
            scaleX = 1; scaleY = 1; scaleZ = 1;
            XSplitting.Text = "30";
            xSplit = 30;
            YSplitting.Text = "30";
            ySplit = 30;
            if (App.Parameters != null)
            {
                needInit = true;
            }
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)// (e.Button == MouseButtons.Middle)
            {
                isMiddleButton = true;
                prevPositionX = e.GetPosition(null).X;
                prevPositionY = e.GetPosition(null).Y;
            }
            if (e.ChangedButton == MouseButton.Right)// == MouseButtons.Right)
            {
                isRightButton = true;
                prevPositionX = e.GetPosition(null).X;
                prevPositionY = e.GetPosition(null).Y;
            }
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (isMiddleButton)
            {
                translateX += e.GetPosition(null).X - prevPositionX;
                translateY += -(e.GetPosition(null).Y - prevPositionY);
                //OpenGLForm.Open
                prevPositionX = e.GetPosition(null).X;
                prevPositionY = e.GetPosition(null).Y;
            }
            if (isRightButton)
            {
                YRotate.Value += (float)(e.GetPosition(null).X - prevPositionX) / 5;
                XRotate.Value += (float)-(e.GetPosition(null).Y - prevPositionY) / 5;
                //Refresh();
                prevPositionX = e.GetPosition(null).X;
                prevPositionY = e.GetPosition(null).Y;
                //isRotate = true;
            }
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                isMiddleButton = false;
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                isRightButton = false;
            }
        }

        private void MyKeyDown(object sender, KeyEventArgs e)
        {
            float val = 5;
            if (e.Key == Key.W)
            {
                //isRotate = true;
                //rotateX += val;
                XRotate.Value += val;
            }
            if (e.Key == Key.S)
            {
                //isRotate = true;
                //rotateX -= val;
                XRotate.Value -= val;
            }
            if (e.Key == Key.A)
            {
                //isRotate = true;
                //rotateY += val;
                YRotate.Value += val;
            }
            if (e.Key == Key.D)
            {
                //isRotate = true;
                //rotateY -= val;
                YRotate.Value -= val;
            }
            if (e.Key == Key.Q)
            {
                //isRotate = true;
                ZRotate.Value += val;
            }
            if (e.Key == Key.E)
            {
                //isRotate = true;
                ZRotate.Value -= val;
            }
            if (e.Key == Key.F1)
                scaleZ -= val;
            if (e.Key == Key.F2)
                scaleZ += val;
        }

        private void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                scaleX *= 1.05f;
                scaleY *= 1.05f;
                scaleZ *= 1.05f;
            }
            else
            {
                scaleX *= 0.95f;
                scaleY *= 0.95f;
                scaleZ *= 0.95f;
            }
        }

        private void OpenGLForm_Initialized(object sender, OpenGLEventArgs args)
        {
            gl = OpenGLForm.OpenGL;
            //gl.Ortho(0, OpenGLForm.ActualHeight, 0, OpenGLForm.ActualWidth, 0, 1000);
            gl.ClearColor(0.8f, 0.8f, 0.8f, 0.8f);
        }

        private void OpenGLForm_Resized(object sender, OpenGLEventArgs args)
        {
            gl = OpenGLForm.OpenGL;
            //gl.Ortho(0, OpenGLForm.ActualHeight, 0, OpenGLForm.ActualWidth, 0, 1000);
            gl.ClearColor(0.8f, 0.8f, 0.8f, 0.8f);
            if (needInit)
            {
                loadAndPrepareData(App.Parameters[0]);
                needInit = false;
                translateY += OpenGLForm.ActualWidth / 2;
                translateX += OpenGLForm.ActualHeight / 2;
            }
        }

        private void XRotate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (XRotate.Value > MAX_ANGLE)
                XRotate.Value -= MAX_ANGLE;
            if (XRotate.Value < MIN_ANGLE)
                XRotate.Value += MAX_ANGLE;
        }
        private void YRotate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (YRotate.Value > MAX_ANGLE)
                YRotate.Value -= MAX_ANGLE;
            if (YRotate.Value < MIN_ANGLE)
                YRotate.Value += MAX_ANGLE;
        }
        private void ZRotate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ZRotate.Value > MAX_ANGLE)
                ZRotate.Value -= MAX_ANGLE;
            if (ZRotate.Value < MIN_ANGLE)
                ZRotate.Value += MAX_ANGLE;
        }

        private void OpenGLForm_Draw(object sender, OpenGLEventArgs args)
        {
            if (!isReadyToDrawing)
                return;
            byte[] r = new byte[4], g = new byte[4], b = new byte[4];

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_PROJECTION); gl.LoadIdentity();
            gl.Ortho(0, OpenGLForm.ActualHeight, 0, OpenGLForm.ActualWidth, -10000, 10000);
            gl.MatrixMode(OpenGL.GL_MODELVIEW); gl.LoadIdentity();
            gl.Enable(OpenGL.GL_DEPTH_TEST); gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Translate(translateX, translateY, translateZ);
            gl.Rotate((float)XRotate.Value, (float)YRotate.Value, (float)ZRotate.Value);
            gl.Scale(scaleX, scaleY, scaleZ);
            if (Axis.IsChecked)
            {
                gl.Begin(OpenGL.GL_LINES);
                gl.Color((byte)255, (byte)0, (byte)0);
                gl.Vertex(0, 0, 0);
                gl.Vertex(int.MaxValue - 1, 0, 0);//X
                gl.Color((byte)0, (byte)255, (byte)0);
                gl.Vertex(0, 0, 0);
                gl.Vertex(0, int.MaxValue - 1, 0);//Y
                gl.Color((byte)0, (byte)0, (byte)255);
                gl.Vertex(0, 0, 0);
                gl.Vertex(0, 0, int.MaxValue - 1);//Z
                gl.End();
                gl.LineWidth(1);
                gl.Begin(OpenGL.GL_LINES);
                gl.Color(1, 1, 1);
                for (int Coordinate = (int)xPoints[0]; Coordinate < xPoints[xPoints.GetLength(0) - 1] + 1; Coordinate++)
                {
                    gl.Vertex(Coordinate, (int)yPoints[0], minZValue);
                    gl.Vertex(Coordinate, (int)(yPoints[yPoints.GetLength(0) - 1] + 1), minZValue);
                }
                for (int Coordinate = (int)yPoints[0]; Coordinate < yPoints[yPoints.GetLength(0) - 1] + 1; Coordinate++)
                {
                    gl.Vertex((int)xPoints[0], Coordinate, minZValue);
                    gl.Vertex((int)(xPoints[xPoints.GetLength(0) - 1] + 1), Coordinate, minZValue);
                }
                gl.End();
                gl.PointSize(2);
                gl.Begin(OpenGL.GL_POINTS);
                for (int i = (int)minZValue; i < maxZValue; i++)
                {
                    gl.Vertex(0, 0, i);
                }
                gl.End();
            }
            if (drowFunctionValue)
            {
                gl.PointSize(8);
                gl.Begin(OpenGL.GL_POINTS);
                gl.Color((byte)255, (byte)255, (byte)255);
                gl.Vertex(xPointforCalculating, yPointforCalculating,
                    task.valueInPoint(xPointforCalculating, yPointforCalculating));
                gl.End();
            }
            if (SplinePoints.IsChecked)
            {
                gl.PointSize(8);
                gl.Begin(OpenGL.GL_POINTS);
                gl.Color((byte)0, (byte)0, (byte)0);
                //IntPtr quadric = gl.NewQuadric();
                for (int i = 0; i < task.grid.points.Count; i++)
                {
                    gl.Vertex(task.grid.points[i].x, task.grid.points[i].y, task.grid.f[i]);
                    //gl.Sphere(quadric,4,OpenGL.GL_QUADS,);
                }
                gl.End();
            }
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            for (int j = 0; j < yPoints.Length - 1; j++)
            {
                for (int i = 0; i < xPoints.Length - 1; i++)
                {
                    if (!Relief.IsChecked)
                        gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
                    else
                        gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
                    gl.Begin(OpenGL.GL_QUADS);
                    palitra.ColorCalculate(z[i, j], out r[0], out g[0], out b[0]);
                    gl.Color(r[0], g[0], b[0]);
                    gl.Vertex(xPoints[i], yPoints[j], z[i, j]);
                    palitra.ColorCalculate(z[i + 1, j], out r[1], out g[1], out b[1]);
                    gl.Color(r[1], g[1], b[1]);
                    gl.Vertex(xPoints[i + 1], yPoints[j], z[i + 1, j]);
                    palitra.ColorCalculate(z[i + 1, j + 1], out r[2], out g[2], out b[2]);
                    gl.Color(r[2], g[2], b[2]);
                    gl.Vertex(xPoints[i + 1], yPoints[j + 1], z[i + 1, j + 1]);
                    palitra.ColorCalculate(z[i, j + 1], out r[3], out g[3], out b[3]);
                    gl.Color(r[3], g[3], b[3]);
                    gl.Vertex(xPoints[i], yPoints[j + 1], z[i, j + 1]);
                    gl.End();
                    gl.Color((byte)255, (byte)255, (byte)255);
                    gl.LineWidth(1);
                    if (Relief.IsChecked)
                    {
                        gl.Begin(OpenGL.GL_QUADS);
                        gl.Vertex(xPoints[i], yPoints[j], 0);
                        gl.Vertex(xPoints[i + 1], yPoints[j], 0);
                        gl.Vertex(xPoints[i + 1], yPoints[j + 1], 0);
                        gl.Vertex(xPoints[i], yPoints[j + 1], 0);
                        gl.End();
                        gl.Begin(OpenGL.GL_LINES);
                        gl.Vertex(xPoints[i], yPoints[j], 0);
                        gl.Vertex(xPoints[i], yPoints[j], z[i, j]);
                        gl.Vertex(xPoints[i + 1], yPoints[j], 0);
                        gl.Vertex(xPoints[i + 1], yPoints[j], z[i + 1, j]);
                        gl.Vertex(xPoints[i + 1], yPoints[j + 1], 0);
                        gl.Vertex(xPoints[i + 1], yPoints[j + 1], z[i + 1, j + 1]);
                        gl.Vertex(xPoints[i], yPoints[j + 1], 0);
                        gl.Vertex(xPoints[i], yPoints[j + 1], z[i, j + 1]);
                        gl.End();
                    }
                    gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
                    if (Grid.IsChecked && !Relief.IsChecked)
                    {
                        gl.Begin(OpenGL.GL_QUADS);
                        gl.Color((byte)255, (byte)255, (byte)255);
                        gl.Vertex(xPoints[i], yPoints[j], z[i, j]);
                        gl.Vertex(xPoints[i + 1], yPoints[j], z[i + 1, j]);
                        gl.Vertex(xPoints[i + 1], yPoints[j + 1], z[i + 1, j + 1]);
                        gl.Vertex(xPoints[i], yPoints[j + 1], z[i, j + 1]);
                        gl.End();
                    }
                }
            }
            gl.Flush();
        }

        private void Resplit_Click(object sender, RoutedEventArgs e)
        {
            int prevXSplit = xSplit, prevYSplit = ySplit;
            if (!int.TryParse(XSplitting.Text, out xSplit) || !int.TryParse(YSplitting.Text, out ySplit))
            {
                MessageBox.Show("Вы ввели неверные значения в поля координат, исправьте и повторите.");
                return;
            }
            if (xSplit > 0 && ySplit > 0)
                calculateGridArrays(xSplit, ySplit);
            else
            {
                MessageBox.Show("Отрицательные значения для разбиения сетки недопустимы.");
                xSplit = prevXSplit;
                ySplit = prevYSplit;
                XSplitting.Text = xSplit.ToString();
                YSplitting.Text = ySplit.ToString();
                return;
            }
            calculateGridArrays(xSplit, ySplit);
        }

        private void OpenFileWasClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open Data File";
            //openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text documents (.txt)|*.txt";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var result = openFileDialog.ShowDialog();
            if (result.Value)
            {
                isReadyToDrawing = false;
                loadAndPrepareData(openFileDialog.FileName);
                isReadyToDrawing = true;
            }
        }

        private void PalitraInitalized(object sender, OpenGLEventArgs args)
        {
            legend = PalitraForm.OpenGL;
        }

        private void PalitraDraw(object sender, OpenGLEventArgs args)
        {
            if (!palitra.isReadyToDraw)
                return;
            byte r, g, b;
            legend.Disable(OpenGL.GL_DEPTH_TEST);
            legend.MatrixMode(OpenGL.GL_PROJECTION);
            legend.LoadIdentity();
            legend.Ortho(0, PalitraForm.Width, 0, PalitraForm.Height, 1, 0);
            legend.MatrixMode(OpenGL.GL_MODELVIEW);
            legend.LoadIdentity();
            legend.Begin(OpenGL.GL_QUADS);
            textBlock5.Text = String.Format("{0:0.###}", palitra.darkBlue);
            palitra.ColorCalculate(palitra.darkBlue, out r, out g, out b);
            legend.Color(r, g, b);
            legend.Vertex(0, 0);
            legend.Vertex(PalitraForm.Width, 0);
            textBlock4.Text = String.Format("{0:0.###}", palitra.blue);
            palitra.ColorCalculate(palitra.blue, out r, out g, out b);
            legend.Color(r, g, b);
            legend.Vertex(PalitraForm.Width, PalitraForm.Height / 4);
            legend.Vertex(0, PalitraForm.Height / 4);
            legend.Vertex(0, PalitraForm.Height / 4);
            legend.Vertex(PalitraForm.Width, PalitraForm.Height / 4);
            textBlock3.Text = String.Format("{0:0.###}", palitra.green);
            palitra.ColorCalculate(palitra.green, out r, out g, out b);
            legend.Color(r, g, b);
            legend.Vertex(PalitraForm.Width, PalitraForm.Height / 2);
            legend.Vertex(0, PalitraForm.Height / 2);
            legend.Vertex(0, PalitraForm.Height / 2);
            legend.Vertex(PalitraForm.Width, PalitraForm.Height / 2);
            textBlock2.Text = String.Format("{0:0.###}", palitra.yellow);
            palitra.ColorCalculate(palitra.yellow, out r, out g, out b);
            legend.Color(r, g, b);
            legend.Vertex(PalitraForm.Width, 3 * PalitraForm.Height / 4);
            legend.Vertex(0, 3 * PalitraForm.Height / 4);
            legend.Vertex(0, 3 * PalitraForm.Height / 4);
            legend.Vertex(PalitraForm.Width, 3 * PalitraForm.Height / 4);
            textBlock1.Text = String.Format("{0:0.###}", palitra.red);
            palitra.ColorCalculate(palitra.red, out r, out g, out b);
            legend.Color(r, g, b);
            legend.Vertex(PalitraForm.Width, PalitraForm.Height);
            legend.Vertex(0, PalitraForm.Height);
            legend.End();
        }

        void loadAndPrepareData(string filePath)
        {
            if (task.Make(filePath))
            {
                calculateGridArrays(xSplit, ySplit);

                if (xPoints[xPoints.Length - 1] - xPoints[0] < yPoints[yPoints.Length - 1] - yPoints[0])
                {
                    if (OpenGLForm.ActualWidth < OpenGLForm.ActualHeight)
                    {
                        scaleX = OpenGLForm.ActualWidth / (xPoints[xPoints.Length - 1] - xPoints[0]);
                        scaleY = scaleX;
                    }
                    else
                    {
                        scaleX = OpenGLForm.ActualHeight / (xPoints[xPoints.Length - 1] - xPoints[0]);
                        scaleY = scaleX;
                    }
                }
                else
                {
                    if (OpenGLForm.Width < OpenGLForm.Height)
                    {
                        scaleX = OpenGLForm.ActualWidth / (yPoints[yPoints.Length - 1] - yPoints[0]);
                        scaleY = scaleX;
                    }
                    else
                    {
                        scaleX = OpenGLForm.ActualHeight / (yPoints[yPoints.Length - 1] - yPoints[0]);
                        scaleY = scaleX;
                    }
                }
                translateX = xPoints[0];
                translateY = yPoints[0];
            }
        }
        private void CalculateFunctionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(textBoxX.Text, out xPointforCalculating) ||
            !double.TryParse(textBoxX.Text, out yPointforCalculating))
            {
                MessageBox.Show("Вы ввели неверные значения в поля координат, исправьте и повторите.");
                drowFunctionValue = false;
                resultTextBlock.Text = "Результат";
                return;
            }
            if (xPointforCalculating < xPoints[0] || xPointforCalculating > xPoints[xPoints.Length - 1] ||
                yPointforCalculating < yPoints[0] || yPointforCalculating > yPoints[yPoints.Length - 1])
            {
                MessageBox.Show("Введенные координаты находятся за пределами рассчетной области.");
                drowFunctionValue = false;
                resultTextBlock.Text = "Результат";
                return;
            }
            drowFunctionValue = true;
            resultTextBlock.Text = String.Format("{0:0.###}", task.valueInPoint(xPointforCalculating, yPointforCalculating));
        }

        public void calculateGridArrays(int countOfXPoints, int countOfYPoints)
        {
            isReadyToDrawing = false;
            //Array.Resize(ref xPoints, countOfXPoints);
            xPoints = new double[countOfXPoints];
            //Array.Resize(ref yPoints, countOfYPoints);
            yPoints = new double[countOfYPoints];
            double hx = (task.grid.x[task.grid.x.Count - 1] - task.grid.x[0]) / countOfXPoints;
            for (int i = 0; i < countOfXPoints; i++)
            {
                xPoints[i] = task.grid.x[0] + hx * i;
            }
            double hy = (task.grid.y[task.grid.y.Count - 1] - task.grid.y[0]) / countOfYPoints;
            for (int i = 0; i < countOfYPoints; i++)
            {
                yPoints[i] = task.grid.y[0] + hy * i;
            }
            z = new double[xPoints.Length, yPoints.Length];
            maxZValue = task.valueInPoint(xPoints[0], yPoints[0]);
            minZValue = maxZValue;
            for (int i = 0; i < countOfXPoints; i++)
            {
                for (int j = 0; j < countOfYPoints; j++)
                {
                    z[i, j] = task.valueInPoint(xPoints[i], yPoints[j]);
                    if (z[i, j] > maxZValue) maxZValue = z[i, j];
                    if (z[i, j] < minZValue) minZValue = z[i, j];
                }
            }
            palitra.setColorValues(minZValue, maxZValue);
            isReadyToDrawing = true;
            scaleZ = maxZValue - minZValue;
        }
    }
}

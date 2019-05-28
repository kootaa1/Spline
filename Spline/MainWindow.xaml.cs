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
        float rotateX, rotateY, rotateZ;
        double translateX, translateY, translateZ;
        private bool isMiddleButton;
        private bool isRightButton;
        private double prevPositionX;
        private double prevPositionY;
        private bool needInit;
        private double xPointforCalculating;
        private double yPointforCalculating;
        private bool drowFunctionValue;

        public MainWindow()
        {
            InitializeComponent();
            palitra = new Palitra();
            task = new MyTask();
            rotateX = 0; rotateY = 0; rotateZ = 0;
            translateX = 0;
            translateY = 0;
            translateZ = 0;
            scaleX = 1; scaleY = 1; scaleZ = 1;
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
                rotateY += (float)(e.GetPosition(null).X - prevPositionX) / 5;
                rotateX += (float)-(e.GetPosition(null).Y - prevPositionY) / 5;
                //Refresh();
                prevPositionX = e.GetPosition(null).X;
                prevPositionY = e.GetPosition(null).Y;
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
                rotateX += val;
            if (e.Key == Key.S)
                rotateX -= val;
            if (e.Key == Key.A)
                rotateY += val;
            if (e.Key == Key.D)
                rotateY -= val;
            if (e.Key == Key.Q)
                rotateZ += val;
            if (e.Key == Key.E)
                rotateZ -= val;
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
            Console.WriteLine("OpenGLForm_Initialized");
        }
        public int DS_Count(string s)
        {
            string substr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString();
            int count = (s.Length - s.Replace(substr, "").Length) / substr.Length;
            return count;
        }
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !((Char.IsDigit(e.Text, 0) ||
                ((e.Text == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString())
                && (DS_Count(((TextBox)sender).Text)< 1))));
        }

        private void OpenGLForm_Resized(object sender, OpenGLEventArgs args)
        {
            gl = OpenGLForm.OpenGL;
            //gl.Ortho(0, OpenGLForm.ActualHeight, 0, OpenGLForm.ActualWidth, 0, 1000);
            gl.ClearColor(0.8f, 0.8f, 0.8f, 0.8f);
            Console.WriteLine("OpenGLForm_Resized");
            if (needInit)
            {
                loadAndPrepareData(App.Parameters[0]);
                needInit = false;
                translateY += OpenGLForm.ActualWidth / 2;
                translateX += OpenGLForm.ActualHeight / 2;
            }
        }
        private void OpenGLForm_Draw(object sender, OpenGLEventArgs args)
        {
            Console.WriteLine("OpenGLForm_Draw");
            if (!isReadyToDrawing)
                return;
            byte[] r = new byte[4], g = new byte[4], b = new byte[4];

            //legend.Disable(OpenGL.GL_DEPTH_TEST);
            //legend.MatrixMode(OpenGL.GL_PROJECTION);
            //legend.LoadIdentity();
            //legend.Ortho(0, PalitraForm.Width, 0, PalitraForm.Height, 1, 0);
            //legend.MatrixMode(OpenGL.GL_MODELVIEW);
            //legend.LoadIdentity();

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_PROJECTION); gl.LoadIdentity();
            gl.Ortho(0, OpenGLForm.ActualHeight, 0, OpenGLForm.ActualWidth, -10000, 10000);
            gl.MatrixMode(OpenGL.GL_MODELVIEW); gl.LoadIdentity();
            gl.Enable(OpenGL.GL_DEPTH_TEST); gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Translate(translateX, translateY, translateZ);
            gl.Rotate(rotateX, rotateY, rotateZ);
            gl.Scale(scaleX, scaleY, scaleZ);
            gl.LineWidth(2);

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

            gl.ShadeModel(OpenGL.GL_SMOOTH);
            for (int j = 0; j < yPoints.Length - 1; j++)
            {
                for (int i = 0; i < xPoints.Length - 1; i++)
                {
                    gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
                    gl.Begin(OpenGL.GL_QUADS);
                    //double[] value = new double[4];
                    //value[0] = task.valueInPoint(xPoints[i], yPoints[j]);
                    palitra.ColorCalculate(z[i, j], out r[0], out g[0], out b[0]);
                    gl.Color(r[0], g[0], b[0]);
                    gl.Vertex(xPoints[i], yPoints[j], z[i, j]);
                    //value[1] = task.valueInPoint(xPoints[i + 1], yPoints[j]);
                    palitra.ColorCalculate(z[i + 1, j], out r[1], out g[1], out b[1]);
                    gl.Color(r[1], g[1], b[1]);
                    gl.Vertex(xPoints[i + 1], yPoints[j], z[i + 1, j]);
                    //value[2] = task.valueInPoint(xPoints[i], yPoints[j + 1]);
                    palitra.ColorCalculate(z[i + 1, j + 1], out r[2], out g[2], out b[2]);
                    gl.Color(r[2], g[2], b[2]);
                    gl.Vertex(xPoints[i + 1], yPoints[j + 1], z[i + 1, j + 1]);
                    //value[3] = task.valueInPoint(xPoints[i + 1], yPoints[j + 1]);
                    palitra.ColorCalculate(z[i, j + 1], out r[3], out g[3], out b[3]);
                    gl.Color(r[3], g[3], b[3]);
                    gl.Vertex(xPoints[i], yPoints[j + 1], z[i, j + 1]);
                    gl.End();
                    gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Color((byte)255, (byte)255, (byte)255);
                    gl.Vertex(xPoints[i], yPoints[j], z[i, j]);
                    gl.Vertex(xPoints[i + 1], yPoints[j], z[i + 1, j]);
                    gl.Vertex(xPoints[i + 1], yPoints[j + 1], z[i + 1, j + 1]);
                    gl.Vertex(xPoints[i], yPoints[j + 1], z[i, j + 1]);
                    gl.End();
                }
            }

            if(drowFunctionValue)
            {
                gl.PointSize(8);
                gl.Begin(OpenGL.GL_POINTS);
                gl.Color((byte)255, (byte)255, (byte)255);
                gl.Vertex(xPointforCalculating, yPointforCalculating,
                    task.valueInPoint(xPointforCalculating, yPointforCalculating));
                gl.End();
            }

            if (DrawPoints.IsChecked.Value)
            {
                gl.PointSize(8);
                gl.Begin(OpenGL.GL_POINTS);
                gl.Color((byte)0, (byte)0, (byte)0);
                for (int i = 0; i < task.grid.points.Count; i++)
                {
                    gl.Vertex(task.grid.points[i].x, task.grid.points[i].y, task.grid.f[i]);
                }
                gl.End();
            }
            gl.Flush();
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
                calculateGridArrays(30, 30);

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
                translateY = xPoints[0];
            }
        }
        private void CalculateFunctionBtn_Click(object sender, RoutedEventArgs e)
        {
            double.TryParse(textBoxX.Text, out xPointforCalculating);
            double.TryParse(textBoxX.Text, out yPointforCalculating);
            drowFunctionValue = true;
            resultTextBlock.Text = String.Format("{0:0.###}", task.valueInPoint(xPointforCalculating,yPointforCalculating));
        }
        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open Data File";
            //openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text documents (.txt)|*.txt";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var result = openFileDialog.ShowDialog();
            if (result.Value)
            {
                loadAndPrepareData(openFileDialog.FileName);
            }
        }

        public void calculateGridArrays(int countOfXPoints, int countOfYPoints)
        {
            isReadyToDrawing = false;
            Array.Resize(ref xPoints, countOfXPoints);
            Array.Resize(ref yPoints, countOfYPoints);
            double min, max;
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
            max = task.valueInPoint(xPoints[0], yPoints[0]);
            min = max;
            for (int j = 0; j < countOfXPoints; j++)
            {
                for (int i = 0; i < countOfYPoints; i++)
                {
                    z[i, j] = task.valueInPoint(xPoints[i], yPoints[j]);
                    if (z[i, j] > max) max = z[i, j];
                    if (z[i, j] < min) min = z[i, j];
                }
            }
            palitra.setColorValues(min, max);
            isReadyToDrawing = true;
        }
    }
}

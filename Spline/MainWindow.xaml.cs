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
        double scaleX = 1, scaleY = 1, scaleZ = 1;
        float rotateX = 1, rotateY = 1, rotateZ = 1;
        double translateX = 1, translateY = 1, translateZ = 1;
        public MainWindow()
        {
            InitializeComponent();
            palitra = new Palitra();
            task = new MyTask();
            isReadyToDrawing = false;
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
            textBlock5.Text = palitra.minblue.ToString();
            palitra.ColorCalculate(palitra.minblue, out r, out g, out b);
            legend.Color(r, g, b);
            legend.Vertex(0, 0);
            legend.Vertex(PalitraForm.Width, 0);
            textBlock4.Text = palitra.blue.ToString();
            palitra.ColorCalculate(palitra.blue, out r, out g, out b);
            legend.Color(r, g, b);
            legend.Vertex(PalitraForm.Width, PalitraForm.Height / 4);
            legend.Vertex(0, PalitraForm.Height / 4);
            legend.Vertex(0, PalitraForm.Height / 4);
            legend.Vertex(PalitraForm.Width, PalitraForm.Height / 4);
            textBlock3.Text = palitra.green.ToString();
            palitra.ColorCalculate(palitra.green, out r, out g, out b);
            legend.Color(r, g, b);
            legend.Vertex(PalitraForm.Width, PalitraForm.Height / 2);
            legend.Vertex(0, PalitraForm.Height / 2);
            legend.Vertex(0, PalitraForm.Height / 2);
            legend.Vertex(PalitraForm.Width, PalitraForm.Height / 2);
            textBlock2.Text = palitra.yellow.ToString();
            palitra.ColorCalculate(palitra.yellow, out r, out g, out b);
            legend.Vertex(PalitraForm.Width, 3 * PalitraForm.Height / 4);
            legend.Vertex(0, 3 * PalitraForm.Height / 4);
            legend.Vertex(0, 3 * PalitraForm.Height / 4);
            legend.Vertex(PalitraForm.Width, 3 * PalitraForm.Height / 4);
            textBlock1.Text = palitra.yellow.ToString();
            palitra.ColorCalculate(palitra.red, out r, out g, out b);
            legend.Vertex(PalitraForm.Width, PalitraForm.Height);
            legend.Vertex(0, PalitraForm.Height);
            legend.End();
        }

        private void PalitraInitalized(object sender, OpenGLEventArgs args)
        {
            legend = PalitraForm.OpenGL;
        }
        private void OpenGLForm_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            gl = OpenGLForm.OpenGL;
        }

        private void OpenGLForm_Resized(object sender, OpenGLEventArgs args)
        {

        }

        private void OpenGLForm_Draw(object sender, OpenGLEventArgs args)
        {
            if (!isReadyToDrawing)
                return;
            byte r, g, b;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_PROJECTION); gl.LoadIdentity();
            gl.Ortho(0, OpenGLForm.Height, 0, OpenGLForm.Width, -1000, 1000);
            gl.MatrixMode(OpenGL.GL_MODELVIEW); gl.LoadIdentity();
            gl.Enable(OpenGL.GL_DEPTH_TEST); gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ClearColor(0.8f, 0.8f, 0.8f, 0.8f);
            gl.Rotate(rotateX, rotateY, rotateZ);
            gl.Translate(translateX, translateY, translateZ);
            gl.Scale(scaleX, scaleY, scaleZ);
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
            gl.Begin(OpenGL.GL_QUADS);
            for (int j = 0; j < yPoints.Length - 1; j++)
            {
                for (int i = 0; i < xPoints.Length - 1; i++)
                {
                    double value = task.valueInPoint(xPoints[i], yPoints[j]);
                    palitra.ColorCalculate(value, out r, out g, out b);
                    gl.Color(r, g, b);
                    gl.Vertex(xPoints[i], yPoints[j], value);
                    value = task.valueInPoint(xPoints[i + 1], yPoints[j]);
                    palitra.ColorCalculate(value, out r, out g, out b);
                    gl.Color(r, g, b);
                    gl.Vertex(xPoints[i], yPoints[j], value);
                    value = task.valueInPoint(xPoints[i], yPoints[j + 1]);
                    palitra.ColorCalculate(value, out r, out g, out b);
                    gl.Color(r, g, b);
                    gl.Vertex(xPoints[i], yPoints[j], value);
                    value = task.valueInPoint(xPoints[i + 1], yPoints[j + 1]);
                    palitra.ColorCalculate(value, out r, out g, out b);
                    gl.Color(r, g, b);
                    gl.Vertex(xPoints[i], yPoints[j], value);
                }
            }
            gl.End();
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
                FilePath = openFileDialog.FileName;
                if (task.Make(FilePath))
                    calculateGridArrays(30, 30);
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


        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {

        }
    }
}

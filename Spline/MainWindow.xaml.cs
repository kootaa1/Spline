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
        public OpenGL gl;
        public bool isDataLoaded = false;
        MyTask task;
        public MainWindow()
        {
            InitializeComponent();
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
                task = new MyTask();
                task.Make(FilePath);
            }
        }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {

        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {

        }

        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {

        }
        public void Draw()
        {
            gl = OpenGlControl1.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_PROJECTION); gl.LoadIdentity();
            gl.Ortho(0, OpenGlControl1.Height, 0, OpenGlControl1.Width, -1000, 1000);
            gl.MatrixMode(OpenGL.GL_MODELVIEW); gl.LoadIdentity();
            gl.Enable(OpenGL.GL_DEPTH_TEST); gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ClearColor(0.8f, 0.8f, 0.8f, 0.8f);
        }
    }
}

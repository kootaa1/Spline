using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Spline
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static List<string> Parameters = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            Parameters = new List<string>();
            foreach(var item in e.Args)
            {
                Parameters.Add(item);
            }
            base.OnStartup(e);
        }
    }
}

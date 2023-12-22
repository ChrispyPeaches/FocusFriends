using FocusApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp
{
    class App : Application
    {
        public App(AppShell shell)
        {
            Resources = new AppStyles();

            MainPage = shell;
        }
    }
}

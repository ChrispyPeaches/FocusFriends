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
        public App()
        {
            Resources = new AppStyles();

            MainPage = new AppShell();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp.Resources
{
    internal class Consts
    {
        public const string DatabaseFileName = "focus.db";
        public static readonly string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
    }
}

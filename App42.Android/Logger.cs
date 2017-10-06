using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Util;

namespace App42
{
    static class Logger
    {
        private const string Tag = "JGDH.App42";

        public static void I(string message)
        {
            Log.Info(Tag, message);
        }

        public static void E(string message)
        {
            Log.Error(Tag, message);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meteo
{
    public static class Util
    {
        //public static Dictionary<Color, List<Point>> mapCR = new Dictionary<Color, List<Point>>();
        public static List<String> spektrumRadar = new List<string>() { "fffcfcfc","ffa00000", "fffc0000", "fffc5800", "fffc8400", "fffcb000", "ffe0dc00", "ff9cdc00", "ff34d800", "ff00bc00", "ff00a000", "ff006cc0", "ff0000fc", "ff3000a8", "ff380070" };
        public static List<String> spektrumSrazky = new List<string>() { "ffffb200", "ffe3df00" };
        public static bool Develop = true;
         
        public static void ShowLoading(string message)
        {
            if (View.FormLoader != null && !View.FormLoader.IsDisposed)
                return;
            View.FormLoader = new FormLoader(message);
            View.FormLoader.TopMost = true;
            View.FormLoader.StartPosition = FormStartPosition.CenterScreen;
            View.FormLoader.Show();
            View.FormLoader.Refresh();
            Thread.Sleep(500);
            Application.Idle += OnLoaded;
        }

        private static void OnLoaded(object sender, EventArgs e)
        {
            Application.Idle -= OnLoaded;
            View.FormLoader.Close();
        }
        
        public static void l(object obj)
        {
            Console.WriteLine(obj);
        }


    }
}

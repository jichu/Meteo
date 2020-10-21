using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Meteo
{
    public static class Preloader
    {
        internal static void ShowAuto(string message, string info = "", bool selfClose = true)
        {
            if (View.FormMain.Preloader)
            {
                UserControlLoader.Instance.UpdateInfo(info);
                return;
            }
            View.FormMain.ShowControlLoader(message);
            if (selfClose)
            {
                Thread.Sleep(100);
                Application.Idle += OnLoaded;
            }
            /*
            if (View.FormLoader != null && !View.FormLoader.IsDisposed)
            {
                View.FormLoader.UpdateInfo(info);
                return;
            }
            View.FormLoader = new FormLoader(message, info);
            View.FormLoader.TopMost=true;
            View.FormLoader.Show();
            View.FormLoader.Refresh();
            if (selfClose)
            {
                Thread.Sleep(100);
                Application.Idle += OnLoaded;
            }*/
        }

        internal static void Show(string message, string info = "")
        {
            ShowAuto(message,info,false);
        }

        internal static void Hide()
        {
            if (View.FormMain.Preloader)
            {
                Application.Idle -= OnLoaded;
                View.FormMain.HideControlLoader();
            }
            /*
            if (View.FormLoader != null && !View.FormLoader.IsDisposed)
            {
                Application.Idle -= OnLoaded;
                View.FormLoader.Close();
            }
            */
        }

        internal static void Log(string info = "")
        {
                UserControlLoader.Instance.UpdateInfo(info);
            /*
            if (View.FormMain.Preloader)
            else
                Show("Zpracování...", info);

            /*
            if (View.FormLoader != null && !View.FormLoader.IsDisposed)
                View.FormLoader.UpdateInfo(info);
            else
                Show("Zpracování...", info);
            */
        }

        private static void OnLoaded(object sender, EventArgs e)
        {
            Application.Idle -= OnLoaded;
            View.FormMain.HideControlLoader();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloaderImagesModels
{
    internal static class Util
    {
        internal static string logFile= "log.txt";

        internal static FormMain form { get; set; }

        internal static void l(object obj, Dictionary<string, object> logOptions = null)
        {
            string ExceptionText = "Exception";
            char logMessageDelimiter = '|';
            Dictionary<string, object> options = new Dictionary<string, object>
            {
                { "toFile", false },
                { "filename", "log.txt" },
                { "clearFile", false},
                { "messageBoxIcon", MessageBoxIcon.Warning}
            };

            if (logOptions != null)
                foreach (var item in logOptions)
                {
                    if (options.ContainsKey(item.Key)) options[item.Key] = item.Value;
                }

            Console.WriteLine(obj);

            form.BeginInvoke((Action)(() =>
            {
                form.richTextBoxOutput.AppendText(obj.ToString() + Environment.NewLine);
                form.status.Text = obj.ToString();
            }));

            if (obj.GetType().ToString().Contains(ExceptionText) || (bool)options["toFile"])
            {
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter((string)options["filename"], !((bool)options["clearFile"])))
                    {
                        file.WriteLine(obj);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            if (obj.ToString().Contains(logMessageDelimiter))
                MessageBox.Show(obj.ToString().Split(logMessageDelimiter)[0], obj.ToString().Split(logMessageDelimiter)[1], MessageBoxButtons.OK, (MessageBoxIcon)options["messageBoxIcon"]);
        }

        internal static void ProcessReady(string msg=null)
        {
            if (msg == null)
            {
                form.BeginInvoke((Action)(() =>
                {
                    form.richTextBoxOutput.Text = "Prosím čekejte..."+Environment.NewLine;
                    form.status.Text = "Ready";
                }));
            }
            else
                l(msg);
        }

        internal static void LabelTimeupdate(int item)
        {
            form.labelTimeupdate.Text += item.ToString()+"h ";
        }
    }
}

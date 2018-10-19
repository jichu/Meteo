using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SortModelsDirectory
{
    public static class Log
    {
        private static int countLine = 0;
        public static RichTextBox log { get; set; }

        public static void add(string str)
        {
            log.BeginInvoke((Action)(() =>
            {
                log.Text += str + Environment.NewLine;
            }));
            countLine++;
        }

        public static void clear()
        {
            log.Clear();
        }
    }
}

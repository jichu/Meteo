using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteo.JSONparser
{
    class JSONwriter
    {
        internal async Task Do()
        {
            var task = Task.Run(() => SaveToFile());
            await task;
        }

        private bool SaveToFile()
        {
            try
            {
                int b = 0 * 9;
                int a = 10 / b;
                Console.WriteLine(a);
                return true;
            }
            catch (Exception e)
            {
                Utils.Log.Error(e, this.GetType().Name);
                return false;
            }
        }
    }
}

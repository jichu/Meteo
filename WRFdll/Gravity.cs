using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WRFdll
{
    internal class Gravity
    {
        public Point Sum { get; set; }
        public Point Counts { get; set; }
        public Point Average { get; set; }
        public Point NearestStrartPoint { get; set; }
        public Gravity(Point sum, Point counts)
        {
            Sum = sum;
            Counts = counts;
            Calc();
        }

        private void Calc()
        {
            if(Counts.X!=0 && Counts.Y!=0)
                Average = new Point(
                    (int)Math.Round((float)Sum.X/(float)Counts.X, MidpointRounding.AwayFromZero), 
                    (int)Math.Round((float)Sum.Y / (float)Counts.Y, MidpointRounding.AwayFromZero)                  
                    );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace WRF
{
    internal class MagicWandForF0ckWRFmodel
    {
        private List<Bitmap> Shapes=new List<Bitmap>();
        private Dictionary<int,List<Point>> DictLines = new Dictionary<int, List<Point>>();
        private string colorKey = "ff000000";

        public bool ShowMetaOutputs { get; set; }

        public MagicWandForF0ckWRFmodel()
        {
            ShowMetaOutputs = true;
            Bitmap bmpNew = new Bitmap(Map.MapSource.Width, Map.MapSource.Height);
            bmpNew = PreprocessDoFilterMask(bmpNew,Map.MapMask);
            if (ShowMetaOutputs)
                Show(bmpNew, "PreprocessDoFilterMask");

            GenerateLines();

            //bmpNew = BreakeToShapes(bmpNew);
        }

        private void GenerateLines(int size=100, int angleStep=5)
        {
            // IV. kv
            for (int i = 0; i < 90; i+=angleStep)
            {
                DictLines.Add(i, DrawLine(i,size));
            }

            Bitmap bmp = new Bitmap(size, size);
            foreach (var item in DictLines)
            {
                foreach (var points in item.Value)
                    bmp.SetPixel(points.X, points.Y, Color.Black);
            }
            Show(bmp);
            /*

            if (!dictionary.TryGetValue(shape.Name, out bm))
            {
                bm = (Bitmap)Properties.Resources.ResourceManager.GetObject(shape.Name);
                dictionary.Add(shape.Name, bm);
            }*/

            // III. kv
            /*
            Dictionary<int, List<Point>> DictLinesNew = new Dictionary<int, List<Point>>();
            int step = angleStep;
            foreach (var item in DictLines)
            {
                List<Point> line = new List<Point>();
                foreach (var points in item.Value)
                {
                    line.Add(new Point(-points.X, points.Y));
                }
                DictLinesNew.Add(90 + step, line);
                step += angleStep;
            }

            bmp = new Bitmap(size, size);
            foreach (var item in DictLinesNew)
                foreach (var points in item.Value)
                    bmp.SetPixel(points.X, points.Y, Color.Black);
            Show(bmp);
            */
        }

        private Bitmap PreprocessDoFilterMask(Bitmap bmp, Bitmap mask, int cutTop=60, int cutBotton=100)
        {
            for (int x = 0; x < mask.Width; x++)
            {
                for (int y = 0; y < mask.Height; y++)
                {
                    bmp.SetPixel(x, y, Color.White);
                    if (bmp.Height > cutTop)
                        if (y < cutTop) continue;
                    if (y > bmp.Height-cutBotton) continue;
                    if (Map.MapSource.GetPixel(x, y).Name == colorKey)
                        bmp.SetPixel(x, y, Color.Black);
                    // use mask
                    if (mask.GetPixel(x,y).Name == colorKey)
                        bmp.SetPixel(x, y, Color.White);
                }
            }
            return bmp;
        }

        int tmpMaxX = 0;
        int tmpMaxY = 0;
        int tmpMinX = 0;
        int tmpMinY = 0;
        List<Point> cachePoint=new List<Point>();
        private Bitmap BreakeToShapes(Bitmap bmp)
        {
            tmpMaxX = 0;
            tmpMaxY = 0;
            tmpMinX = bmp.Width+1;
            tmpMinY = bmp.Height+1;
            cachePoint.Clear();
            Bitmap bmpShape = new Bitmap(bmp.Width,bmp.Height);
            Point startPoint = FindFirstPoint(bmp);
            bmp = ColorChange(bmp,startPoint);
            FindAroundPoint(bmp,startPoint);
            CreateShape(tmpMaxX - startPoint.X + 1, tmpMaxY - startPoint.Y + 1);
            Console.WriteLine(tmpMinX);
            Console.WriteLine(tmpMinY);
            Console.WriteLine(tmpMaxX-startPoint.X+1);
            Console.WriteLine(tmpMaxY-startPoint.Y+1);
            return bmp;
        }

        private void CreateShape(int w, int h)
        {
            Bitmap tmp = new Bitmap(w,h);
            foreach (Point point in cachePoint)
            {
                Console.WriteLine(point);
                //tmp.SetPixel(point.X-tmpMinX, point.Y-tmpMaxY, Color.Black);
            }
            Show(tmp);
        }

        private void FindAroundPoint(Bitmap bmp, Point stasrtPoint)
        {
            for (int x = -1; x <=1 ; x++)
            {
                for (int y = -1; y <=1; y++)
                {
                    if (bmp.GetPixel(stasrtPoint.X + x, stasrtPoint.Y + y).Name == colorKey)
                    {
                        bmp = ColorChange(bmp, new Point(stasrtPoint.X + x, stasrtPoint.Y + y));
                        if (tmpMaxX <= stasrtPoint.X + x)
                            tmpMaxX = stasrtPoint.X + x;
                        if (tmpMaxY <= stasrtPoint.Y + y)
                            tmpMaxY = stasrtPoint.Y + y;
                        if (tmpMinX >= stasrtPoint.X + x)
                            tmpMinX = stasrtPoint.X + x;
                        if (tmpMinY >= stasrtPoint.Y + y)
                            tmpMinY = stasrtPoint.Y + y;
                        cachePoint.Add(new Point(stasrtPoint.X + x, stasrtPoint.Y+y));
                        FindAroundPoint(bmp, new Point(stasrtPoint.X + x, stasrtPoint.Y + y));
                    }
                }
            }
        }

        private Bitmap ColorChange(Bitmap bmp, Point xy)
        {
            if(bmp.Width>=xy.X && bmp.Height>=xy.Y)
                bmp.SetPixel(xy.X,xy.Y, Color.Red);
            return bmp;
        }

        private Point FindFirstPoint(Bitmap bmp)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    if (bmp.GetPixel(x, y).Name == colorKey)
                        return new Point(x, y);
                }
            }
            return new Point();
        }

        private double Radian(int angle) { return (Math.PI / 180.0) * angle; }

        private List<Point> DrawLine(int angle=30, int length = 100)
        {
            Bitmap bmp = new Bitmap(length, length,PixelFormat.Format32bppPArgb);
            var myPen = new Pen(Color.Black);
            int x = 0;
            int y = 0;
            double x2 = x + (Math.Cos(Radian(angle)) * length);
            double y2 = y + (Math.Sin(Radian(angle)) * length);
            int intX1 = Convert.ToInt32(x);
            int intY1 = Convert.ToInt32(y);
            int intX2 = Convert.ToInt32(x2);
            int intY2 = Convert.ToInt32(y2);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.DrawLine(myPen, new Point(intX1, intY1), new Point(intX2, intY2));
            }
            List<Point> line = new List<Point>();
            for (x = 0; x < bmp.Width; x++)
            {
                for (y = 0; y < bmp.Height; y++)
                {
                    if (bmp.GetPixel(x, y).Name == "ff000000")
                        line.Add(new Point(x, y));
                }
            }
            return line;
        }

        private void Show(Bitmap bmpNew,string title="")
        {
            new FormTemplate(title, bmpNew).Show();
        }
    }

}
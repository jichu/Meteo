using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;

namespace WRF
{
    internal class MagicWandForF0ckWRFmodel
    {
        public int AroundStartPoint { get; set; }

        private List<Bitmap> Shapes=new List<Bitmap>();
        private Dictionary<int,List<Point>> DictLines = new Dictionary<int, List<Point>>();
        private List<Point> StartPoints = new List<Point>();
        private List<Point> StartPointsAround = new List<Point>();
        private string colorKey = "ff000000";
        private Stopwatch watch;
        private long watchTimeAll=0;

        public bool ShowMetaOutputs { get; set; }

        public MagicWandForF0ckWRFmodel()
        {
            AroundStartPoint = 10;
            ShowMetaOutputs = true;
            
            Bitmap bmpNew = new Bitmap(Map.MapSource.Width, Map.MapSource.Height);
            bmpNew = PreprocessDoFilterMask(bmpNew,Map.MapMask);

            StartWatch();
            GenerateAroundCircle();
            StopWatch("GenerateAroundCircle()");

            StartWatch();
            GenerateGrid(bmpNew);
            StopWatch("GenerateGrid()");

            StartWatch();
            GenerateLines();
            StopWatch("GenerateLines()");
            /*
            Thread t = new Thread(() => ProcessDoWand(bmpNew));
            t.Start();
            */
            Console.WriteLine($"Celkový čas zpracování {watchTimeAll}ms");
        }
        
        private void StartWatch()
        {
            watch = Stopwatch.StartNew();
        }
        private void StopWatch(string msg ="")
        {
            watch.Stop();
            watchTimeAll += watch.ElapsedMilliseconds;
            Console.WriteLine($"{msg} v čase {watch.ElapsedMilliseconds}ms");
        }

        private Bitmap PreprocessDoFilterMask(Bitmap bmp, Bitmap mask, int cutTop = 0, int cutBotton = 0)
        {
            for (int x = 0; x < mask.Width; x++)
            {
                for (int y = 0; y < mask.Height; y++)
                {
                    bmp.SetPixel(x, y, Color.White);
                    if (bmp.Height > cutTop)
                        if (y < cutTop) continue;
                    if (y > bmp.Height - cutBotton) continue;
                    if (Map.MapSource.GetPixel(x, y).Name == colorKey)
                        bmp.SetPixel(x, y, Color.Black);
                    // use mask
                    if (mask.GetPixel(x, y).Name == colorKey)
                        bmp.SetPixel(x, y, Color.White);
                }
            }
            if (ShowMetaOutputs)
                Show(bmp, "PreprocessDoFilterMask");
            return bmp;
        }

        private void GenerateLines(int angleStep=5, int size = 100, bool showPampelishka=true)
        {
            DictLines.Clear();
            // IV. kv
            int angle = 0;
            for (angle = 0; angle <= 90; angle+=angleStep)
            {
                DictLines.Add(angle, DrawLine(angle,size));
            }

            // III. kv
            angle = 90;
            int count = DictLines.Count;
            for (int i = count-1; i >= 0; i--)
            {
                List<Point> line = new List<Point>();
                foreach (var points in DictLines.ElementAt(i).Value)
                {
                    line.Add(new Point(-points.X, points.Y));
                }
                if (!DictLines.ContainsKey(angle))
                    DictLines.Add(angle, line);
                angle += angleStep;
            }

            // II. kv & I. kv
            angle = 270;
            count = DictLines.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                List<Point> line = new List<Point>();
                foreach (var points in DictLines.ElementAt(i).Value)
                {
                    line.Add(new Point(points.X, -points.Y));
                }
                if (!DictLines.ContainsKey(angle))
                    DictLines.Add(angle, line);
                angle += angleStep;
            }

            if (showPampelishka)
            {
                int c = 240;
                Bitmap bmp = new Bitmap(size * 2, size * 2);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.DarkGreen);
                foreach (var item in DictLines)
                {
                    foreach (var points in item.Value)
                        bmp.SetPixel(size + points.X, size + points.Y, Color.FromArgb(255, 255, 255, c));
                    if (c > 0) c-=(int)255/DictLines.Count;
                }
                Show(bmp);
            }
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

        private void GenerateAroundCircle()
        {
            Bitmap bmp = new Bitmap(AroundStartPoint, AroundStartPoint);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.FillEllipse(new SolidBrush(Color.Black), new Rectangle(0, 0, AroundStartPoint, AroundStartPoint));
            }
            StartPointsAround.Clear();
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    if (bmp.GetPixel(x, y).Name == "ff000000")
                        StartPointsAround.Add(new Point(x- AroundStartPoint/2, y-AroundStartPoint/2));
                }
            }
        }
        private void GenerateGrid(Bitmap bmp)
        {
            StartPoints.Clear();
            Point count=new Point(25,17);
            Point boxSize = new Point(36, 38);
            Point offset = new Point(76, 93);
            for (int y = 0; y < count.Y; y++)
            {
                for (int x = 0; x < count.X; x++)
                {
                    Point space = new Point(boxSize.X* x+offset.X, boxSize.Y *y+offset.Y);
                    if (space.X<=bmp.Width && space.Y <= bmp.Height)
                    {
                        AddPointToList(new Point(space.X, space.Y));
                    }
                }
            }
            if (ShowMetaOutputs)
            {
                Bitmap b = new Bitmap(bmp);
                foreach (var p in StartPoints)
                    b.SetPixel(p.X, p.Y, Color.Red);
                Show(b, "GenerateGrid startPoint");
            }
        }

        private void AddPointToList(Point point)
        {
            //StartPoints.Add(new Point(point.X, point.Y));
            AddPointAroundToList(new Point(point.X, point.Y)); 
        }

        private void AddPointAroundToList(Point point)
        {
            foreach (Point around in StartPointsAround)
            {
                int x = point.X - around.X;
                int y = point.Y - around.Y;
                if (x >= 0 && y >= 0)
                        StartPoints.Add(new Point(x, y));
            }
        }

        private void Show(Bitmap bmpNew,string title="")
        {
            new FormTemplate(title, bmpNew).Show();
        }

        // zpracovani WRF
        private void ProcessDoWand(Bitmap bmp)
        {
            try
            {
                foreach (Point startPoint in StartPoints)
                {
                    Thread t = new Thread(() => LookUpWind(bmp, startPoint));
                    t.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LookUpWind(Bitmap bmp, Point startPoint)
        {
            if (bmp.GetPixel(startPoint.X, startPoint.Y).Name == "ff000000")
                Console.WriteLine(1);
        }


        /*
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
        /*
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
        */
    }

}
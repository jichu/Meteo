using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WRF
{
    internal class MagicWandForF0ckWRFmodel
    {
        public int AroundStartPoint { get; set; }

        private List<string> BitmapMatrix = new List<string>();
        private Dictionary<int,List<Point>> DictLines = new Dictionary<int, List<Point>>();
        private List<Point> StartPoints = new List<Point>();
        private List<Point> StartPointsAround = new List<Point>();
        private List<NamePoint> NamePoints = new List<NamePoint>();
        private Point gridCount = new Point(25, 16);
        private Point gridBoxSize = new Point(36, 38);
        private Point gridOffset = new Point(76, 93);
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


            ProcessDoWand(bmpNew);

            Console.WriteLine($"Celkový čas zpracování {watchTimeAll}ms, startovacích bodů {StartPoints.Count}");
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
                    {
                        bmp.SetPixel(x, y, Color.Black);
                        BitmapMatrix.Add($"{x},{y}");
                    }
                    // use mask
                    if (mask.GetPixel(x, y).Name == colorKey)
                        bmp.SetPixel(x, y, Color.White);
                }
            }
            if (ShowMetaOutputs)
                Show(bmp, "PreprocessDoFilterMask");
            return bmp;
        }

        private void GenerateLines(int angleStep=1, int size = 37, bool showPampelishka=true)
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
            for (int i = count - 1; i > 0; i--)
            {
                List<Point> line = new List<Point>();
                foreach (var points in DictLines.ElementAt(i).Value)
                {
                    line.Add(new Point(points.X, -points.Y));
                }
                if (!DictLines.ContainsKey(angle))
                    DictLines.Add(angle-89, line);
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

        private List<Point> DrawLine(int angle=30, int length = 38)
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
            for (int y = 0; y < gridCount.Y; y++)
            {
                for (int x = 0; x < gridCount.X; x++)
                {
                    Point space = new Point(gridBoxSize.X* x+gridOffset.X, gridBoxSize.Y *y+gridOffset.Y);
                    if (space.X<=bmp.Width && space.Y <= bmp.Height)
                    {
                        AddPointToList(bmp, new Point(space.X, space.Y));
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

        private void AddPointToList(Bitmap bmp, Point point)
        {
            if (bmp.GetPixel(point.X, point.Y).Name == "ff000000")
                StartPoints.Add(new Point(point.X, point.Y));
            else
                AddPointAroundToList(bmp, new Point(point.X, point.Y));
        }

        private void AddPointAroundToList(Bitmap bmp, Point point)
        {
            foreach (Point around in StartPointsAround)
            {
                int x = point.X - around.X;
                int y = point.Y - around.Y;
                if (x >= 0 && y >= 0)
                    if (bmp.GetPixel(x, y).Name == "ff000000")
                    {
                        StartPoints.Add(new Point(x, y));
                        break;
                    }
            }
        }

        private void Show(Bitmap bmpNew,string title="")
        {
            new FormTemplate(title, bmpNew).Show();
        }

        // zpracovani WRF
        private void ProcessDoWand(Bitmap bmp,bool sync =true)
        {
            if (sync)
            {
                StartWatch();
                foreach (Point startPoint in StartPoints)
                {
                    LookUpWind(bmp, startPoint, sync);
                }
                StopWatch("ProcessDoWand()");
                ShowOutput(bmp);
            }
            else
            {
                Task t = Task.Run(() => ProcessDoWandAsync(bmp));
            }
        }

        private void ShowOutput(Bitmap bmp)
        {
            Bitmap b = new Bitmap(bmp);
            foreach (var np in NamePoints)
            {
                foreach (var lines in DictLines)
                {
                    if (lines.Key == np.Angle)
                    {
                        foreach (var p in lines.Value)
                        {
                            b.SetPixel(p.X+np.StartPoint.X, p.Y+np.StartPoint.Y, Color.Green);
                        }
                        break;
                    }
                }
                using (Graphics g = Graphics.FromImage(b))
                {
                    using (Font font = new Font("Times New Roman", 12, FontStyle.Regular, GraphicsUnit.Pixel))
                    {
                        PointF pointF1 = new PointF(np.StartPoint.X-20, np.StartPoint.Y-20);
                        g.DrawString(np.Angle.ToString()+ "°", font, Brushes.DarkBlue, pointF1);
                    }

                }

            }
            Show(b, "Výstup");
        }

        private void ProcessDoWandAsync(Bitmap bmp)
        {
            try
            {
                StartWatch();
                List<Task<int>> tasks = new List<Task<int>>();
                int i = 0;
                foreach (Point startPoint in StartPoints)
                {
                    tasks.Add(Task.Run(() => LookUpWind(bmp, startPoint, false)));
                    //Thread t = new Thread(() => LookUpWind(bmp, startPoint));
                    //t.Start();
                    if (i > 1000)
                        break;
                    i++;
                }
                Task.WaitAll(tasks.ToArray());
                StopWatch("ProcessDoWandAsyc()");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static object locker = new object();
        private int LookUpWind(Bitmap bmp, Point startPoint,  bool sync=false)
        {
            int max = 0;
            int angle=-1;
            foreach (var lines in DictLines)
            {
                int accord = 0;
                foreach (var point in lines.Value)
                {
                    int x = startPoint.X + point.X;
                    int y = startPoint.Y + point.Y;
                    if (x >= 0 && y >= 0)
                    {
                        if (sync)
                            if (bmp.GetPixel(x, y).Name == "ff000000")
                                accord++;

                        if (!sync)
                            if (BitmapMatrix.Contains($"{x},{y}"))
                              accord++;
                    }
                }
                if (max < accord)
                {
                    max = accord;
                    angle = lines.Key;
                }
            }
            //Console.WriteLine($"úhel:{angle} shoda: {max}");
            if(max>10)
            NamePoints.Add(new NamePoint()
            {
                Angle = angle,
                StartPoint=startPoint
            });
            return angle;
        }

    }

}
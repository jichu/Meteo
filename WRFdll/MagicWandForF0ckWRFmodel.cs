using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WRFdll
{
    internal class MagicWandForF0ckWRFmodel
    {
        public int AroundStartPoint { get; set; } = 20;
        public int AngleStep { get; set; } = 5;
        public int LineLength { get; set; } = 34;
        public bool ShowMetaOutputs { get; set; } = true;

        private List<string> BitmapMatrix = new List<string>();
        private Dictionary<int,List<Point>> DictLines = new Dictionary<int, List<Point>>();
        private List<Point> StartPoints = new List<Point>();
        private List<List<Point>> StartPointsAround = new List<List<Point>>();
        private List<NamePoint> NamePoints = new List<NamePoint>();
        private Point gridCount = new Point(12, 8);
        private Point gridBoxSize = new Point(36, 38);
        private Point gridOffset = new Point(76, 93);
        private string colorKey = "ff000000";
        private Stopwatch watch;
        private long watchTimeAll=0;
        private Bitmap bmpNew;
        
        public MagicWandForF0ckWRFmodel()
        {
            
            bmpNew = new Bitmap(WRF.MapSource.Width, WRF.MapSource.Height);
            bmpNew = PreprocessDoFilterMask(bmpNew,WRF.MapMask);

            StartWatch();
            GenerateAroundCircles();
            StopWatch("GenerateAroundCircles()");
            
            StartWatch();
            GenerateGrids(bmpNew,WRF.MapMask);
            StopWatch("GenerateGrids()");

            StartWatch();
            GenerateLines(AngleStep,LineLength);
            StopWatch("GenerateLines()");
            
            Console.WriteLine($"Celkový čas zpracování {watchTimeAll}ms, startovacích bodů {StartPoints.Count}");
        }

        public Dictionary<string,string> Do()
        {
            ProcessDoWand(bmpNew);

            StartWatch();
            Dictionary<string, string> d =ProcessDoAssignORP();
            StopWatch("ProcessDoAssignORP()");

            if(ShowMetaOutputs)
                ShowOutput(bmpNew);

            return d;
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
                    // use mask
                    if (mask.GetPixel(x, y).Name == colorKey || mask.GetPixel(x, y).Name == "ffff0000")
                        WRF.MapSource.SetPixel(x, y, Color.White);

                    if (WRF.MapSource.GetPixel(x, y).Name == colorKey)
                    {
                        bmp.SetPixel(x, y, Color.Black);
                        if (mask.GetPixel(x, y).Name != colorKey)
                            BitmapMatrix.Add($"{x},{y}");
                    }
                }
            }
            if (ShowMetaOutputs)
                Show(bmp, "PreprocessDoFilterMask");
            return bmp;
        }

        private void GenerateLines(int angleStep=5, int size = 34, bool showPampelishka=true)
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
            if (ShowMetaOutputs)
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

        private void GenerateAroundCircles()
        {
            StartPointsAround.Clear();
            for (int i = 0; i < AroundStartPoint; i+=3)
            {
                int size = i;
                Bitmap bmp = new Bitmap(size + 1, size + 1);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    g.DrawEllipse(new Pen(Color.Black), new Rectangle(0, 0, size, size));
                }
                List<Point> circle = new List<Point>();
                for (int x = 0; x < bmp.Width; x++)
                    for (int y = 0; y < bmp.Height; y++)
                        if (bmp.GetPixel(x, y).Name == "ff000000")
                            circle.Add(new Point(x - size / 2, y - size / 2));
                StartPointsAround.Add(circle);
            }
        }

        private void GenerateGrids(Bitmap bmpNew, Bitmap mask)
        {
            Point tmp_gridOffset = gridOffset;
            for (int i = 1; i <= 2; i++)
            {
                GenerateGrid(bmpNew,mask);
                gridOffset = new Point(gridOffset.X + gridBoxSize.X * gridCount.X, gridOffset.Y);
            }
            gridOffset = new Point(tmp_gridOffset.X, gridOffset.Y + gridBoxSize.Y * gridCount.Y+3);

            for (int i = 1; i <= 2; i++)
            {
                GenerateGrid(bmpNew,mask);
                gridOffset = new Point(gridOffset.X + gridBoxSize.X * gridCount.X, gridOffset.Y);
            }
            if (ShowMetaOutputs)
            {
                Bitmap b = new Bitmap(bmpNew);
                foreach (var p in StartPoints)
                    b.SetPixel(p.X, p.Y, Color.Red);
                Show(b, "GenerateGrid startPoint");
            }
        }

        private void GenerateGrid(Bitmap bmp,Bitmap mask)
        {
            for (int y = 0; y < gridCount.Y; y++)
            {
                for (int x = 0; x < gridCount.X; x++)
                {
                    Point space = new Point(gridBoxSize.X* x+gridOffset.X, gridBoxSize.Y *y+gridOffset.Y);
                    if (space.X<=bmp.Width && space.Y <= bmp.Height)
                    {
                        if (mask.GetPixel(space.X, space.Y).Name != "ffff0000")
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
            if(!StartPoints.Contains(new Point(point.X, point.Y)))
            StartPoints.Add(new Point(point.X, point.Y));
        }

        private void Show(Bitmap bmpNew,string title="")
        {
            new FormTemplate(title, bmpNew).Show();
        }

        // zpracovani WRF
        private void ProcessDoWand(Bitmap bmp,bool sync =false)
        {
            if (sync)
            {
                StartWatch();
                foreach (Point startPoint in StartPoints)
                {
                    LookUpStartWind(bmp, startPoint, sync);
                }
                StopWatch("ProcessDoWand()");
            }
            else
            {
                StartWatch();
                List<Task<bool>> tasks = new List<Task<bool>>();
                tasks.Add(Task.Run(() => ProcessDoWandAsync(bmp)));
                Task.WaitAll(tasks.ToArray());
                StopWatch("ProcessDoWandAsyc()");
            }
        }

        private bool ProcessDoWandAsync(Bitmap bmp)
        {

            List<Task<int>> tasks = new List<Task<int>>();
            try
            {
                int i = 0;
                foreach (Point startPoint in StartPoints)
                {
                    tasks.Add(Task.Run(() => LookUpStartWind(bmp, startPoint, false)));
                    if (i > 1002)
                    {
                        Task.WaitAll(tasks.ToArray());
                        tasks.Clear();
                        i=0;
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            };

            Task.WaitAll(tasks.ToArray());
            return true;
        }

        private int LookUpStartWind(Bitmap bmp, Point startPoint,  bool sync=false)
        {
            List<Point> curStartPoints = new List<Point>();
            if (IsBlack(bmp,startPoint.X, startPoint.Y))
                curStartPoints.Add(startPoint);
            else
                curStartPoints = LookUpPointsAround(bmp, startPoint);

         //   Console.WriteLine($"startovacích bodů pro jeden směr: {curStartPoints.Count}");

            if (curStartPoints.Count > 0)
            {
                int angleWin = -1;
                int maxAgree = 0;

                foreach (var sp in curStartPoints)
                {
                    int max = 0;
                    int angle = -1;
                    if (LookUpWind(bmp, curStartPoints[0], out max, out angle))
                    {
                        if (max > maxAgree)
                        {
                            maxAgree = max;
                            angleWin = angle;
                        }
                    }
                }
                if(angleWin!=-1)
                NamePoints.Add(new NamePoint()
                {
                    Angle = angleWin,
                    StartPoint = startPoint
                });
            }

            return 0;
        }

        private bool LookUpWind(Bitmap bmp, Point startPoint, out int max, out int angle, bool sync = false)
        {
            max = 0;
            angle=-1;
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
                        {
                            if (bmp.GetPixel(x, y).Name == "ff000000")
                                accord++;
                        }

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
            if (max >= 8) return true;
            else return false;
        }

        private List<Point> LookUpPointsAround(Bitmap bmp, Point startPoint)
        {
            List<Point> curStartPoints = new List<Point>();
            bool found = false;
            int r = 0;//AroundStartPoint/4;
            foreach (var circles in StartPointsAround)
            {
                if (found) break;
                foreach (var circle in circles)
                {
                    int x = circle.X-r + startPoint.X;
                    int y = circle.Y-r + startPoint.Y;
                    if (IsBlack(bmp, x, y))
                    {
                        curStartPoints.Add(new Point(x, y));
                        found = true;
                        break;
                    }
                    /*
                    if (!IsBlack(bmp, x, y))
                        bmp.SetPixel(x,y, Color.Green);
                        */
                }
            }

            return curStartPoints;
        }


        private void ShowOutput(Bitmap bmp)
        {
            Bitmap b = new Bitmap(bmp);
            Console.WriteLine(bmp.Height);
            foreach (var np in NamePoints)
            {
                foreach (var lines in DictLines)
                {
                    if (lines.Key == np.Angle)
                    {
                        foreach (var p in lines.Value)
                        {
                            b.SetPixel(p.X + np.StartPoint.X, p.Y + np.StartPoint.Y, Color.Red);
                        }
                        break;
                    }
                }

                string compass = GetCompass(np.Angle);
                using (Graphics g = Graphics.FromImage(b))
                {
                    using (Font font = new Font("Times New Roman", 12, FontStyle.Regular, GraphicsUnit.Pixel))
                    {
                        PointF pointF1 = new PointF(np.StartPoint.X, np.StartPoint.Y);
                        g.DrawString(compass, font, Brushes.DarkBlue, pointF1);
                    }

                }

            }
            Show(b, "Výstup");
        }

        private string GetCompass(int angle)
        {
            string compass = "";
            if (angle > 337.5 || angle < 22.5) compass = "V";
            if (angle > 22.5 && angle < 67.5) compass = "JV";
            if (angle > 67.5 && angle < 112.5) compass = "J";
            if (angle > 112.5 && angle < 157.5) compass = "JZ";
            if (angle > 157.5 && angle < 202.5) compass = "Z";
            if (angle > 202.5 && angle < 247.5) compass = "SZ";
            if (angle > 247.5 && angle < 292.5) compass = "S";
            if (angle > 292.5 && angle < 337.5) compass = "SV";
            return compass;
        }

        private bool IsBlack(Bitmap bmp, int x, int y)
        {
            return (BitmapMatrix.Contains($"{x},{y}"));
                //                return bmp.GetPixel(x, y).Name == "ff000000";
        }


        // prirazeni vetru k ORP
        private Dictionary<string, string> ProcessDoAssignORP()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            Dictionary<string, LineRegion> regions = new Dictionary<string, LineRegion>();
            foreach (var np in NamePoints)
            {
                Dictionary<string, int> DicColorCounter = new Dictionary<string, int>();
                foreach (var wind in DictLines[np.Angle])
                {
                   
                    
                    string color =WRF.MapMaskORP.GetPixel(np.StartPoint.X + wind.X, np.StartPoint.Y + wind.Y).Name;
                    
                    if (color == "ffffffff" || color == "ff000000")
                        continue;
                    if (!DicColorCounter.ContainsKey(color))
                        DicColorCounter.Add(color, 1);
                    else
                        DicColorCounter[color]++;
                }

                if(DicColorCounter.Count>0)
                {
                    foreach (var cc in DicColorCounter)
                    {
                     //   Console.WriteLine($"{cc.Key}: {cc.Value}x angle: {np.Angle}");
                        if (!regions.ContainsKey(cc.Key))
                            regions.Add(cc.Key, new LineRegion()
                            {
                                Angle = np.Angle,
                                AngleArea = cc.Value
                            });
                        else
                        {
                            if (regions[cc.Key].AngleArea < cc.Value)
                                regions[cc.Key].AngleArea = cc.Value;
                            /*if (regions[cc.Key].AngleArea == cc.Value)
                                Console.WriteLine($"HA, co teď? {cc.Key} ({Region.ORP["#" + cc.Key.Substring(2, 6)]}): {cc.Value}x angle: {np.Angle} a {regions[cc.Key].AngleArea}");*/
                        }
                    }
                }
            }

            Console.WriteLine($"Počet detekovaných ORP: {regions.Count}");

            foreach (var r in regions)
            {
                if (Region.ORP.ContainsKey("#" + r.Key.Substring(2, 6)))
                {
                    //Console.WriteLine($"{Region.ORP["#" + r.Key.Substring(2, 6)]} směr: {GetCompass(r.Value.Angle)}   ({"#" + r.Key.Substring(2, 6)} {r.Value.AngleArea}x úhel: {r.Value.Angle}))");
                    ret.Add(Region.ORP["#" + r.Key.Substring(2, 6)], GetCompass(r.Value.Angle));
                }
            }

            return ret;
        }
    }

}
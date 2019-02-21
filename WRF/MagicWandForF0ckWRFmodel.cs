using System;
using System.Collections.Generic;
using System.Drawing;

namespace WRF
{
    internal class MagicWandForF0ckWRFmodel
    {
        private List<Bitmap> Shapes=new List<Bitmap>();
        private string colorKey = "ff000000";

        public bool ShowMetaOutputs { get; set; }

        public MagicWandForF0ckWRFmodel()
        {
            ShowMetaOutputs = true;

            Bitmap bmpNew = new Bitmap(Map.MapSource.Width, Map.MapSource.Height);
            bmpNew = PreprocessDoFilterMask(bmpNew,Map.MapMask);
            if (ShowMetaOutputs)
                Show(bmpNew, "PreprocessDoFilterMask");
            bmpNew = BreakeToShapes(bmpNew);
        }
        
        private Bitmap PreprocessDoFilterMask(Bitmap bmp, Bitmap mask, int cutTop=0, int cutBotton=0)
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
            /*
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                }
            }*/
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

        private void Show(Bitmap bmpNew,string title="")
        {
            new FormTemplate(title, bmpNew).Show();
        }
    }

}
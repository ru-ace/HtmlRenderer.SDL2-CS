using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using SDL2;

namespace AcentricPixels.HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class GraphicsPathAdapter : RGraphicsPath
    {

        internal struct PathItem
        {
            public int x;
            public int y;

            public bool arc;
            public int arc_x;
            public int arc_y;
            public int arc_cx;
            public int arc_cy;
            public int arc_r;
            public int arc_startAngle;
            public Corner corner;
            public SDL.SDL_Point ToSDL() { return new SDL.SDL_Point { x = x, y = y }; }
        }
        internal readonly List<PathItem> pathItems = new List<PathItem>();

        internal int? x1 = null, y1 = null, x2 = null, y2 = null;

        internal SDL.SDL_Rect rect { get { return new SDL.SDL_Rect { x = x1.Value, y = y1.Value, w = x2.Value - x1.Value, h = y2.Value - y1.Value }; } }


        public override void ArcTo(double x, double y, double size, Corner corner)
        {
            if (pathItems.Count == 0)
                AddPoint(x, y);
            var lastPoint = pathItems.Last();
            var arc_item = new PathItem { arc = true, x = (int)x, y = (int)y, corner = corner, arc_r = (int)size };
            arc_item.arc_x = (int)(x - ((corner == Corner.TopRight || corner == Corner.TopLeft) ? size : 0));// (int)(Math.Min(x, lastPoint.x) - (corner == Corner.TopRight || corner == Corner.BottomRight ? size : 0));
            arc_item.arc_y = (int)(y - ((corner == Corner.TopRight || corner == Corner.BottomRight) ? size : 0));//(int)(Math.Min(y, lastPoint.y) - (corner == Corner.BottomLeft || corner == Corner.BottomRight ? size : 0));
            arc_item.arc_cx = arc_item.arc_x;
            arc_item.arc_cy = arc_item.arc_y;

            switch (corner)
            {
                case Corner.TopLeft: arc_item.arc_startAngle = 180; arc_item.arc_cx += arc_item.arc_r; arc_item.arc_cy += arc_item.arc_r; break;
                case Corner.BottomLeft: arc_item.arc_startAngle = 90; arc_item.arc_cx += arc_item.arc_r; break;
                case Corner.TopRight: arc_item.arc_startAngle = 270; arc_item.arc_cy += arc_item.arc_r; break;
                case Corner.BottomRight: arc_item.arc_startAngle = 0; break;
            }

            pathItems.Add(arc_item);
        }

        internal List<SDL.SDL_Rect> GetSDLRects()
        {
            PathItem ctl = new PathItem(), ctr = new PathItem(), cbl = new PathItem(), cbr = new PathItem();
            List<SDL.SDL_Rect> rects = new List<SDL.SDL_Rect>();

            for (int i = 1; i < pathItems.Count; i++)
            {
                var pi0 = pathItems[i - 1];
                var pi1 = pathItems[i];
                if (pi1.arc)
                {
                    rects.AddRange(GetArcSDLRects(pi1));
                    switch (pi1.corner)
                    {
                        case RGraphicsPath.Corner.TopLeft: ctl = pi1; break;
                        case RGraphicsPath.Corner.TopRight: ctr = pi1; break;
                        case RGraphicsPath.Corner.BottomLeft: cbl = pi1; break;
                        case RGraphicsPath.Corner.BottomRight: cbr = pi1; break;
                    }

                }
            }
            int xl = ctl.arc_cx >= cbl.arc_cx ? ctl.arc_cx : cbl.arc_cx;
            int xr = ctr.arc_cx <= cbr.arc_cx ? ctr.arc_cx : cbr.arc_cx;
            int yt = ctl.arc_y;
            int yb = cbl.arc_y + cbl.arc_r;
            rects.Add(new SDL.SDL_Rect { x = xl, y = yt, w = xr - xl, h = yb - yt });
            if (ctl.arc_r == cbl.arc_r)
                rects.Add(new SDL.SDL_Rect { x = ctl.arc_x, y = ctl.arc_cy, w = ctl.arc_r, h = cbl.arc_cy - ctl.arc_cy });
            else
            {
                if (ctl.arc_r < cbl.arc_r)
                {
                    rects.Add(new SDL.SDL_Rect { x = ctl.arc_x, y = ctl.arc_cy, w = ctl.arc_r, h = cbl.arc_cy - ctl.arc_cy });
                    rects.Add(new SDL.SDL_Rect { x = ctl.arc_cx, y = ctl.arc_y, w = cbl.arc_r - ctl.arc_r, h = cbl.arc_cy - ctl.arc_y });

                }
                else
                {
                    rects.Add(new SDL.SDL_Rect { x = ctl.arc_x, y = ctl.arc_cy, w = cbl.arc_r, h = cbl.arc_cy - ctl.arc_cy });
                    rects.Add(new SDL.SDL_Rect { x = cbl.arc_cx, y = ctl.arc_cy, w = ctl.arc_r - cbl.arc_r, h = cbl.arc_cy - ctl.arc_cy + cbl.arc_r });
                }
            }
            if (ctr.arc_r == cbr.arc_r)
                rects.Add(new SDL.SDL_Rect { x = ctr.arc_x, y = ctr.arc_cy, w = ctr.arc_r, h = cbr.arc_cy - ctr.arc_cy });
            else
            {
                if (ctr.arc_r < cbr.arc_r)
                {
                    rects.Add(new SDL.SDL_Rect { x = cbr.arc_cx, y = ctr.arc_y, w = cbr.arc_r - ctr.arc_r, h = cbr.arc_cy - ctr.arc_y });
                    rects.Add(new SDL.SDL_Rect { x = ctr.arc_cx, y = ctr.arc_cy, w = ctr.arc_r, h = cbr.arc_cy - ctr.arc_cy });
                }
                else
                {
                    rects.Add(new SDL.SDL_Rect { x = ctr.arc_cx, y = ctr.arc_cy, w = ctr.arc_r - cbr.arc_r, h = cbr.arc_cy - ctr.arc_cy + cbr.arc_r });
                    rects.Add(new SDL.SDL_Rect { x = cbr.arc_x, y = ctr.arc_cy, w = cbr.arc_r, h = cbr.arc_cy - ctr.arc_cy });
                }
            }
            return rects;
        }

        private List<SDL.SDL_Rect> GetArcSDLRects(PathItem corner)
        {
            List<SDL.SDL_Rect> rects = new List<SDL.SDL_Rect>();
            Double step = 90f / ((double)corner.arc_r * Math.PI / 2f);


            int last_y = 10000000;
            for (double angle = 0; angle < 90; angle += step)
            {
                double r_angle = Math.PI * ((double)corner.arc_startAngle + angle) / 180f;
                int y = corner.arc_cy + (int)((double)corner.arc_r * Math.Sin(r_angle));
                int x = corner.arc_cx + (int)((double)corner.arc_r * Math.Cos(r_angle));
                int x1 = 0, x2 = 0;
                if (last_y != y)
                {
                    switch (corner.corner)
                    {
                        case RGraphicsPath.Corner.TopLeft:
                        case RGraphicsPath.Corner.BottomLeft:
                            x1 = x;
                            x2 = corner.arc_cx;
                            break;
                        case RGraphicsPath.Corner.TopRight:
                        case RGraphicsPath.Corner.BottomRight:
                            x1 = corner.arc_cx;
                            x2 = x;
                            break;
                    }


                }

                rects.Add(new SDL.SDL_Rect { x = x1, y = y, w = x2 - x1, h = 1 });
                last_y = y;
            }
            return rects;
        }
        internal List<SDL.SDL_Point> GetArcSDPoints(PenAdapter pen, PathItem corner)
        {
            List<SDL.SDL_Point> points = new List<SDL.SDL_Point>();

            Double step = 90f / ((double)corner.arc_r * Math.PI / 2f);
            //points.Add(new SDL.SDL_Point { x = corner.arc_cx, y = corner.arc_cy });

            for (double angle = 0; angle <= 90; angle += step)
            {
                points.Add(GetArcSDLPointAtAngle(corner, angle));
                switch (pen.dashStyle)
                {
                    case RDashStyle.Solid:
                        break;
                    case RDashStyle.Dot:
                        angle += step * 1.5f;
                        break;
                    case RDashStyle.Dash:
                        angle += step;
                        points.Add(GetArcSDLPointAtAngle(corner, angle));
                        angle += step * 2f;
                        break;
                }

            }
            return points;
        }

        private SDL.SDL_Point GetArcSDLPointAtAngle(PathItem corner, double angle)
        {
            int r = corner.arc_r;
            int cx = corner.arc_cx;
            int cy = corner.arc_cy;

            switch (corner.corner)
            {
                case RGraphicsPath.Corner.TopLeft:
                    break;
                case RGraphicsPath.Corner.BottomLeft:
                    r++;
                    break;
                case RGraphicsPath.Corner.TopRight:
                    break;
                case RGraphicsPath.Corner.BottomRight:
                    r++;

                    break;
            }
            double r_angle = Math.PI * ((double)corner.arc_startAngle + angle) / 180f;
            int x = cx + (int)(r * Math.Cos(r_angle));
            int y = cy + (int)(r * Math.Sin(r_angle));
            return new SDL.SDL_Point { x = x, y = y };

        }



        public override void Dispose()
        {
            pathItems.Clear();
        }

        public override void LineTo(double x, double y)
        {
            AddPoint(x, y);
        }

        public override void Start(double x, double y)
        {
            AddPoint(x, y);
        }

        private void AddPoint(double x, double y)
        {
            if (!x1.HasValue || (x1.HasValue && x < x1)) x1 = (int)x;
            if (!x2.HasValue || (x2.HasValue && x > x2)) x2 = (int)x;
            if (!y1.HasValue || (y1.HasValue && y < y1)) y1 = (int)y;
            if (!y2.HasValue || (y2.HasValue && y > y2)) y2 = (int)y;
            pathItems.Add(new PathItem { arc = false, x = (int)x, y = (int)y });
        }

    }
}

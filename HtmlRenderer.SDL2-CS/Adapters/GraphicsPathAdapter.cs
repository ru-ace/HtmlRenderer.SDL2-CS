﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using SDL2;

namespace HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class GraphicsPathAdapter : RGraphicsPath
    {

        internal struct PathItem
        {
            public int x;
            public int y;

            public bool arc;
            public int arc_top;
            public int arc_left;
            public int arc_r;
            public Corner corner;
            public SDL.SDL_Point ToSDL() { return new SDL.SDL_Point { x = x, y = y }; }
        }
        internal readonly List<PathItem> pathItems = new List<PathItem>();

        public override void ArcTo(double x, double y, double size, Corner corner)
        {
            if (pathItems.Count == 0)
                AddPoint(x, y);
            var lastPoint = pathItems.Last();
            var arc_item = new PathItem { arc = true, x = (int)x, y = (int)y, corner = corner, arc_r = (int)size };
            arc_item.arc_top = (int)(Math.Min(x, lastPoint.x) - (corner == Corner.TopRight || corner == Corner.BottomRight ? size : 0));
            arc_item.arc_left = (int)(Math.Min(y, lastPoint.y) - (corner == Corner.BottomLeft || corner == Corner.BottomRight ? size : 0));
            pathItems.Add(arc_item);
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
            pathItems.Add(new PathItem { arc = false, x = (int)x, y = (int)y });
        }

        /// <summary>
        /// Get arc start angle for the given corner.
        /// </summary>
        private static int GetStartAngle(Corner corner)
        {
            int startAngle;
            switch (corner)
            {
                case Corner.TopLeft:
                    startAngle = 180;
                    break;
                case Corner.TopRight:
                    startAngle = 270;
                    break;
                case Corner.BottomLeft:
                    startAngle = 90;
                    break;
                case Corner.BottomRight:
                    startAngle = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("corner");
            }
            return startAngle;
        }
    }
}

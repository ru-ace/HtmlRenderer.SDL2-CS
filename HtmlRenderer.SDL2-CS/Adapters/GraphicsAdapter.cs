﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using SDL2;
using HtmlRenderer.SDL2_CS.Utils;

namespace HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class GraphicsAdapter : RGraphics
    {
        private readonly IntPtr _renderer = IntPtr.Zero;


        public GraphicsAdapter(RAdapter adapter, RRect initialClip) : base(adapter, initialClip)
        {
            _renderer = SDL2Adapter.Instance.Renderer;
            if (_renderer == IntPtr.Zero)
                throw new Exception("Graphics.GraphicsAdapter(): SDL2Adapter.Instance.Renderer not set");
        }

        public override void Dispose()
        {
            //throw new NotImplementedException();
        }

        public override void DrawImage(RImage image, RRect destRect, RRect srcRect)
        {
            image.ToImageA().Draw(_renderer, destRect, srcRect);
        }

        public override void DrawImage(RImage image, RRect destRect)
        {
            image.ToImageA().Draw(_renderer, destRect);
        }


        public override void DrawLine(RPen pen, double x1, double y1, double x2, double y2)
        {

            if (y1 == y2)
                DrawLineH(pen, (int)y1, (int)x1, (int)x2);
            else if (x1 == x2)
                DrawLineV(pen, (int)x1, (int)y1, (int)y2);
            else
            {
                pen.ToPenA().color.SetToSDLRenderer();
                if (SDL.SDL_RenderDrawLine(_renderer, (int)x1, (int)y1, (int)x2, (int)y2) < 0)
                    Helpers.ShowSDLError("Graphics.DrawLine:Unable to SDL_RenderDrawLine!");
            }
        }

        private void DrawLineH(RPen rpen, int y1, int x1, int x2)
        {
            PenAdapter pen = rpen.ToPenA();
            pen.color.SetToSDLRenderer();
            int width = (int)pen.width;
            int w0, w1;
            w0 = w1 = width / 2;
            //w1 -= (width != 1 && width % 2 == 1) ? 1 : 0;

            List<SDL.SDL_Rect> rects = new List<SDL.SDL_Rect>();
            int length = (int)(x2 - x1 + 1);

            switch (pen.dashStyle)
            {
                case RDashStyle.Solid:
                    rects.Add(new SDL.SDL_Rect { x = x1, y = y1 - w0, w = length, h = width });
                    break;
                case RDashStyle.Dot:
                    for (int x = (int)x1; x <= x2 - width + 1; x += width * 2)
                        rects.Add(new SDL.SDL_Rect { x = x, y = y1 - w0, w = width, h = width });
                    if (length % (width * 2) < width)
                        rects.Add(new SDL.SDL_Rect { x = x2 - length % (width * 2) + 1, y = y1 - w0, w = length % width, h = width });
                    break;
                case RDashStyle.Dash:
                    for (int x = (int)x1; x <= x2 - width * 2 + 1; x += width * 3)
                        rects.Add(new SDL.SDL_Rect { x = x, y = y1 - w0, w = width * 2, h = width });
                    if (length % (width * 3) < width * 2)
                        rects.Add(new SDL.SDL_Rect { x = x2 - length % (width * 3) + 1, y = y1 - w0, w = length % (width * 3) + 1, h = width });
                    break;
            }

            SDL.SDL_RenderFillRects(_renderer, rects.ToArray(), rects.Count);

        }
        private void DrawLineV(RPen rpen, int x1, int y1, int y2)
        {
            PenAdapter pen = rpen.ToPenA();
            pen.color.SetToSDLRenderer();
            int width = (int)pen.width;
            int w0, w1;
            w0 = w1 = width / 2;
            //w1 -= (width != 1 && width % 2 == 1) ? 1 : 0;

            List<SDL.SDL_Rect> rects = new List<SDL.SDL_Rect>();
            int length = (int)(y2 - y1 + 1);

            switch (pen.dashStyle)
            {
                case RDashStyle.Solid:
                    rects.Add(new SDL.SDL_Rect { x = x1 - w0, y = y1, w = width, h = length });
                    break;
                case RDashStyle.Dot:
                    for (int y = y1; y <= y2 - width + 1; y += width * 2)
                        rects.Add(new SDL.SDL_Rect { x = x1 - w0, y = y, w = width, h = width });
                    if (length % (width * 2) < width)
                        rects.Add(new SDL.SDL_Rect { x = x1 - w0, y = y2 - length % (width * 2) + 1, w = width, h = length % width });
                    break;
                case RDashStyle.Dash:
                    for (int y = y1; y <= y2 - width * 2 + 1; y += width * 3)
                        rects.Add(new SDL.SDL_Rect { x = x1 - w0, y = y, w = width, h = width * 2 });
                    if (length % (width * 3) < width * 2)
                        rects.Add(new SDL.SDL_Rect { x = x1 - w0, y = y2 - length % (width * 3) + 1, w = width, h = length % (width * 3) });
                    break;
            }

            SDL.SDL_RenderFillRects(_renderer, rects.ToArray(), rects.Count);

        }

        public override void DrawPath(RPen pen, RGraphicsPath path)
        {
            //Console.WriteLine("Graphics.DrawPath P");
            //TODO Use pen and realize arc 
            pen.ToPenA().color.SetToSDLRenderer();
            var pathA = path.ToPathA();

            SDL.SDL_Point[] sdl_points = new SDL.SDL_Point[path.ToPathA().pathItems.Count];
            for (int i = 1; i < sdl_points.Length; i++)
            {
                if (pathA.pathItems[i].arc)
                {
                    //this.DrawRectangle()
                }
                else
                {
                    this.DrawLine(pen, pathA.pathItems[i - 1].x, pathA.pathItems[i - 1].y, pathA.pathItems[i].x, pathA.pathItems[i].y);
                }
                //sdl_points[i] = pathA.pathItems[i].ToSDL();
                //Console.WriteLine("  {0} {1}", i, pathA.pathItems[i].arc ? "arc" : "line");
            }
            //if (SDL.SDL_RenderDrawLines(_renderer, sdl_points, sdl_points.Length) < 0)
            //Helpers.ShowSDLError("Graphics.DrawPolygon:Unable to SDL_RenderDrawLines!");

        }

        public override void DrawPath(RBrush brush, RGraphicsPath path)
        {
            //Console.WriteLine("Graphics.DrawPath B");
            //TODO Use brush and draw arc 
            var b = brush.ToBrushA();
            var p = path.ToPathA();
            if (b.isImage)
            {

            }
            else
            {

                GraphicsPathAdapter.PathItem ctl = new GraphicsPathAdapter.PathItem(), ctr = new GraphicsPathAdapter.PathItem(), cbl = new GraphicsPathAdapter.PathItem(), cbr = new GraphicsPathAdapter.PathItem();
                List<SDL.SDL_Rect> rects = new List<SDL.SDL_Rect>();
                b.color.SetToSDLRenderer();
                for (int i = 1; i < p.pathItems.Count; i++)
                {
                    var pi0 = p.pathItems[i - 1];
                    var pi1 = p.pathItems[i];
                    if (pi1.arc)
                    {
                        rects.AddRange(DrawArc(pi1));
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


                SDL.SDL_RenderFillRects(_renderer, rects.ToArray(), rects.Count);
            }
        }


        private List<SDL.SDL_Rect> DrawArc(GraphicsPathAdapter.PathItem corner)
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
            //SDL.SDL_RenderFillRects(_renderer, rects.ToArray(), rects.Count);
            //var rect = new SDL.SDL_Rect { x = coner.arc_x, y = coner.arc_y, w = coner.arc_r, h = coner.arc_r };
            //SDL.SDL_RenderDrawRect(_renderer, ref rect);
        }


        public override void DrawPolygon(RBrush brush, RPoint[] points)
        {
            Console.WriteLine("Graphics.DrawPolygon");
            //TODO Use brush
            brush.ToBrushA().color.SetToSDLRenderer();
            SDL.SDL_Point[] sdl_points = new SDL.SDL_Point[points.Length];
            for (int i = 0; i < sdl_points.Length; i++)
                sdl_points[i] = points[i].ToSDL();

            if (SDL.SDL_RenderDrawLines(_renderer, sdl_points, sdl_points.Length) < 0)
                Helpers.ShowSDLError("Graphics.DrawPolygon:Unable to SDL_RenderDrawLines!");
        }

        public override void DrawRectangle(RPen pen, double x, double y, double width, double height)
        {
            //TODO Use pen 
            pen.ToPenA().color.SetToSDLRenderer();
            var rect = new SDL.SDL_Rect { x = (int)x, y = (int)y, w = (int)width, h = (int)height };
            if (SDL.SDL_RenderDrawRect(_renderer, ref rect) < 0)
                Helpers.ShowSDLError("Graphics.DrawRectangle:Unable to SDL_RenderDrawRect!");
        }

        public override void DrawRectangle(RBrush brush, double x, double y, double width, double height)
        {

            //Console.WriteLine("Graphics.DrawRectangle x:{0} y:{1} w:{2} h:{3}", x, y, width, height);
            var brushA = brush.ToBrushA();
            var rect = new SDL.SDL_Rect { x = (int)x, y = (int)y, w = (int)width, h = (int)height };
            if (brushA.isImage)
            {   //Fill with texture


                double xs = (width / brushA.image_dstRect.Width + (width % brushA.image_dstRect.Width == 0 ? 0 : 1f) + (brushA.image_translateTransformLocation.X == x ? 0 : 1f));
                double ys = (height / brushA.image_dstRect.Height + (height % brushA.image_dstRect.Height == 0 ? 0 : 1) + (brushA.image_translateTransformLocation.Y == y ? 0 : 1f));
                for (int xi = 0; xi < xs; xi++)
                {
                    for (int yi = 0; yi < ys; yi++)
                    {
                        double dst_x = (x + xi * brushA.image_dstRect.Width) + (x == brushA.image_translateTransformLocation.X ? 0 : brushA.image_translateTransformLocation.X);
                        double dst_y = (y + yi * brushA.image_dstRect.Height) + (y == brushA.image_translateTransformLocation.Y ? 0 : brushA.image_translateTransformLocation.Y);
                        var dst_rrect = new RRect { X = dst_x, Y = dst_y, Width = brushA.image_dstRect.Width, Height = brushA.image_dstRect.Height };
                        brushA.image.Draw(_renderer, dst_rrect);
                    }
                }
            }
            else
            {
                //Fill with solid color
                brushA.color.SetToSDLRenderer();
                if (SDL.SDL_RenderFillRect(_renderer, ref rect) < 0)
                    Helpers.ShowSDLError("Graphics.DrawRectangle:Unable to SDL_RenderFillRect!");
            }
        }


        /// <summary>
        /// Draw the given string using the given font and foreground color at given location.
        /// </summary>
        /// <param name="str">the string to draw</param>
        /// <param name="font">the font to use to draw the string</param>
        /// <param name="color">the text color to set</param>
        /// <param name="point">the location to start string draw (top-left)</param>
        /// <param name="size">used to know the size of the rendered text for transparent text support</param>
        /// <param name="rtl">is to render the string right-to-left (true - RTL, false - LTR)</param>
        public override void DrawString(string str, RFont font, RColor color, RPoint point, RSize size, bool rtl)
        {
            var textSurface = SDL_ttf.TTF_RenderUTF8_Blended(font.ToTTF_Font(), str, color.ToSDL());
            if (textSurface.ShowSDLError("Graphics.DrawString: Unable to TTF_RenderUTF8_Blended!"))
                return;

            var texture_text = SDL.SDL_CreateTextureFromSurface(_renderer, textSurface);
            if (texture_text.ShowSDLError("Graphics.DrawString: Unable to CreateTextureFromSurface!"))
            {
                SDL.SDL_FreeSurface(textSurface);
                return;
            }

            var dst_rect = textSurface.As<SDL.SDL_Surface>().ToSDL_Rect().UpdatedByRPoint(point);

            if (SDL.SDL_RenderCopy(_renderer, texture_text, IntPtr.Zero, ref dst_rect) < 0)
                Helpers.ShowSDLError("Graphics.DrawString:Unable to SDL_RenderCopy!");

            SDL.SDL_DestroyTexture(texture_text);
            SDL.SDL_FreeSurface(textSurface);

        }

        public override RGraphicsPath GetGraphicsPath()
        {
            return new GraphicsPathAdapter();
        }


        public override RSize MeasureString(string str, RFont font)
        {

            SDL_ttf.TTF_SizeUTF8(font.ToTTF_Font(), str, out int w, out int h);
            return new RSize(w, h);

        }



        // int SDL_RenderSetClipRect(SDL_Renderer*   renderer, const SDL_Rect* rect)
        // SDL_RenderGetClipRect(SDL_Renderer* renderer, SDL_Rect* rect)
        // SDL_bool SDL_RenderIsClipEnabled(SDL_Renderer* renderer)

        public override void PopClip()
        {
            _clipStack.Pop();
            var sdl_rect = _clipStack.Peek().ToSDL();
            SDL.SDL_RenderSetClipRect(_renderer, ref sdl_rect);
        }

        public override void PushClip(RRect rect)
        {
            _clipStack.Push(rect);
            var sdl_rect = rect.ToSDL();
            SDL.SDL_RenderSetClipRect(_renderer, ref sdl_rect);

        }


        public override RBrush GetTextureBrush(RImage image, RRect dstRect, RPoint translateTransformLocation)
        {
            return new BrushAdapter(image, dstRect, translateTransformLocation);
        }

        // TODO low priority 


        /// <summary>
        /// Measure the width of string under max width restriction calculating the number of characters that can fit and the width those characters take.<br/>
        /// Not relevant for platforms that don't render HTML on UI element.
        /// </summary>
        /// <param name="str">the string to measure</param>
        /// <param name="font">the font to measure string with</param>
        /// <param name="maxWidth">the max width to calculate fit characters</param>
        /// <param name="charFit">the number of characters that will fit under <see cref="maxWidth"/> restriction</param>
        /// <param name="charFitWidth">the width that only the characters that fit into max width take</param>
        public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
        {
            //TODO low priority (there is no need for it - used for text selection)       
            Console.WriteLine("Graphics.MeasureString(): NotImplemented");
            throw new NotSupportedException();
        }
        public override void ReturnPreviousSmoothingMode(object prevMode)
        {
            //Console.WriteLine("Graphics.ReturnPreviousSmoothingMode(): NotImplemented");
            //there is no need for it
            //throw new NotImplementedException();
        }

        public override object SetAntiAliasSmoothingMode()
        {
            //Console.WriteLine("Graphics.SetAntiAliasSmoothingMode(): NotImplemented");
            //there is no need for it
            //throw new NotImplementedException();
            return null;
        }
        public override void PushClipExclude(RRect rect)
        {   // pdfSharp is not implement this
            // Seem not used 
            Console.WriteLine("Graphics.PushClipExclude(): NotImplemented");
            //throw new NotImplementedException();
        }
    }
}

using System;
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
            throw new NotImplementedException();
        }

        public override void DrawImage(RImage image, RRect destRect, RRect srcRect)
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(RImage image, RRect destRect)
        {
            throw new NotImplementedException();
        }

        public override void DrawLine(RPen pen, double x1, double y1, double x2, double y2)
        {

            //TODO Use pen
            pen.ToPenA().color.SetToSDLRenderer();
            if (SDL.SDL_RenderDrawLine(_renderer, (int)x1, (int)y1, (int)x2, (int)y2) < 0)
                Helpers.ShowSDLError("Graphics.DrawString:Unable to SDL_RenderDrawLine!");

        }

        public override void DrawPath(RPen pen, RGraphicsPath path)
        {
            //TODO Use pen and realize arc 
            pen.ToPenA().color.SetToSDLRenderer();

            SDL.SDL_Point[] sdl_points = new SDL.SDL_Point[path.ToPathA().pathItems.Count];
            for (int i = 0; i < sdl_points.Length; i++)
                sdl_points[i] = path.ToPathA().pathItems[i].ToSDL();

            if (SDL.SDL_RenderDrawLines(_renderer, sdl_points, sdl_points.Length) < 0)
                Helpers.ShowSDLError("Graphics.DrawPolygon:Unable to SDL_RenderDrawLines!");

        }

        public override void DrawPath(RBrush brush, RGraphicsPath path)
        {
            //TODO Use brush and realize arc 
            brush.ToBrushA().color.SetToSDLRenderer();

            SDL.SDL_Point[] sdl_points = new SDL.SDL_Point[path.ToPathA().pathItems.Count];
            for (int i = 0; i < sdl_points.Length; i++)
                sdl_points[i] = path.ToPathA().pathItems[i].ToSDL();

            if (SDL.SDL_RenderDrawLines(_renderer, sdl_points, sdl_points.Length) < 0)
                Helpers.ShowSDLError("Graphics.DrawPolygon:Unable to SDL_RenderDrawLines!");

        }

        public override void DrawPolygon(RBrush brush, RPoint[] points)
        {
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
                Helpers.ShowSDLError("Graphics.DrawString:Unable to SDL_RenderCopy!");
        }

        public override void DrawRectangle(RBrush brush, double x, double y, double width, double height)
        {
            //TODO Use brush 
            brush.ToBrushA().color.SetToSDLRenderer();
            var rect = new SDL.SDL_Rect { x = (int)x, y = (int)y, w = (int)width, h = (int)height };
            if (SDL.SDL_RenderDrawRect(_renderer, ref rect) < 0)
                Helpers.ShowSDLError("Graphics.DrawString:Unable to SDL_RenderCopy!");
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

        public override RBrush GetTextureBrush(RImage image, RRect dstRect, RPoint translateTransformLocation)
        {
            throw new NotImplementedException();
        }

        public override void PopClip()
        {
            throw new NotImplementedException();
        }

        public override void PushClip(RRect rect)
        {
            throw new NotImplementedException();
        }

        public override void PushClipExclude(RRect rect)
        {
            throw new NotImplementedException();
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
            throw new NotSupportedException();
        }
        public override void ReturnPreviousSmoothingMode(object prevMode)
        {
            //there is no need for it
            throw new NotImplementedException();
        }

        public override object SetAntiAliasSmoothingMode()
        {
            //there is no need for it
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using SDL2;
using HtmlRenderer.SDL2_CS.Utils;
using System.Runtime.InteropServices;

namespace HtmlRenderer.SDL2_CS.Adapters
{
    public sealed class SDL2Adapter : RAdapter
    {

        private IntPtr _renderer = IntPtr.Zero;

        private static SDL2Adapter _instance = null;

        private SDL2Adapter()
        {
            UpdateFontFamilyList();
        }

        public static SDL2Adapter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SDL2Adapter();
                return _instance;
            }
        }



        public RRect GetRendererRect(bool window = true)
        {

            if (SDL.SDL_RenderIsClipEnabled(_renderer) == SDL.SDL_bool.SDL_TRUE && !window)
            {

                SDL.SDL_RenderGetClipRect(_renderer, out SDL.SDL_Rect rect);
                return rect.ToRRect();
            }
            else
            {
                SDL.SDL_GetRendererOutputSize(_renderer, out int width, out int height);
                return new RRect(0, 0, (double)width, (double)height);
            }

            // SDL_RenderGetClipRect(SDL_Renderer* renderer, SDL_Rect* rect)
            // SDL_bool SDL_RenderIsClipEnabled(SDL_Renderer* renderer)
        }

        public void UpdateFontFamilyList()
        {
            foreach (var fontfamily_name in Utils.FontManager.Instance.Families)
                AddFontFamily(new FontFamilyAdapter(fontfamily_name));
        }


        public IntPtr Renderer
        {
            set { _renderer = value; }
            get { return _renderer; }
        }


        protected override RFont CreateFontInt(string family, double size, RFontStyle style)
        {
            return new FontAdapter(family, size, style);
        }

        protected override RFont CreateFontInt(RFontFamily family, double size, RFontStyle style)
        {
            return new FontAdapter(family.Name, size, style);
        }


        protected override RPen CreatePen(RColor color)
        {
            return new PenAdapter(color);
        }

        protected override RBrush CreateSolidBrush(RColor color)
        {
            return new BrushAdapter(color);
        }

        protected override RColor GetColorInt(string colorName)
        {
            return Utils.Color.FromKnownColor(colorName);
        }
        protected override RImage ConvertImageInt(object image)
        {
            return new ImageAdapter(image);
        }



        protected override RImage ImageFromStreamInt(Stream memoryStream)
        {
            Console.WriteLine("ImageFromStreamInt(): NotImplemented");
            throw new NotImplementedException();
        }
        protected override RBrush CreateLinearGradientBrush(RRect rect, RColor color1, RColor color2, double angle)
        {
            Console.WriteLine("CreateLinearGradientBrush(): NotImplemented");
            throw new NotImplementedException();
        }


    }
}

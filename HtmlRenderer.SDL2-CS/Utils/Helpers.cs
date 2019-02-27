using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using SDL2;


namespace HtmlRenderer.SDL2_CS.Utils
{
    internal static class Helpers
    {
        public static RRect ToRRect(this SDL.SDL_Rect rect)
        {
            return new RRect(rect.x, rect.y, rect.w, rect.h);
        }
        public static SDL.SDL_Rect ToSDL_Rect(this RRect rect)
        {
            return new SDL.SDL_Rect { x = (int)rect.X, y = (int)rect.Y, w = (int)rect.Width, h = (int)rect.Height };
        }


        public static RColor ToSDL_Color(this SDL.SDL_Color color)
        {
            return RColor.FromArgb(color.a, color.r, color.g, color.b);
        }
        public static SDL.SDL_Color ToSDL_Color(this RColor color)
        {
            return new SDL.SDL_Color { a = color.A, r = color.R, g = color.G, b = color.B };
        }
        /*
        public static SDL.SDL_Rect ToSDL_Rect(this IntPtr ptr)
        {

        }
        */
    }
}

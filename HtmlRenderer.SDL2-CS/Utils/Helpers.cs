using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Adapters;
using SDL2;
using System.Runtime.InteropServices;
using HtmlRenderer.SDL2_CS;

namespace HtmlRenderer.SDL2_CS.Utils
{
    public static class Helpers
    {
        public static RRect ToRRect(this SDL.SDL_Rect rect)
        {
            return new RRect(rect.x, rect.y, rect.w, rect.h);
        }


        public static SDL.SDL_Rect ToSDL(this RRect rect)
        {
            return new SDL.SDL_Rect { x = (int)rect.X, y = (int)rect.Y, w = (int)rect.Width, h = (int)rect.Height };
        }

        public static SDL.SDL_Rect UpdatedByRPoint(this SDL.SDL_Rect rect, RPoint point)
        {
            rect.x = (int)point.X;
            rect.y = (int)point.Y;
            return rect;
        }
        public static SDL.SDL_Rect UpdatedByRSize(this SDL.SDL_Rect rect, RSize size)
        {
            rect.w = (int)size.Width;
            rect.h = (int)size.Height;
            return rect;
        }

        public static IntPtr GetTTF_Font(this RFont font)
        {
            var fa = font as Adapters.FontAdapter;
            return fa.Font;
        }


        public static SDL.SDL_Point ToSDL(this RPoint point)
        {
            return new SDL.SDL_Point { x = (int)point.X, y = (int)point.Y };
        }


        public static RColor ToSDL_Color(this SDL.SDL_Color color)
        {
            return RColor.FromArgb(color.a, color.r, color.g, color.b);
        }
        public static SDL.SDL_Color ToSDL(this RColor color)
        {
            return new SDL.SDL_Color { a = color.A, r = color.R, g = color.G, b = color.B };
        }

        public static SDL.SDL_Rect ToSDL_Rect(this SDL.SDL_Surface sdl_surface)
        {
            return new SDL.SDL_Rect { x = 0, y = 0, w = sdl_surface.w, h = sdl_surface.h };
        }
        public static T As<T>(this IntPtr ptr)
        {
            return Marshal.PtrToStructure<T>(ptr);
        }
        public static bool ShowSDLError(this IntPtr ptr, string error_text)
        {
            if (ptr == IntPtr.Zero)
                ShowSDLError(error_text);
            return ptr == IntPtr.Zero;
        }
        public static void ShowSDLError(string error_text)
        {
            Console.WriteLine(error_text + " SDL_ttf Error: {0}", SDL.SDL_GetError());
        }

    }
}

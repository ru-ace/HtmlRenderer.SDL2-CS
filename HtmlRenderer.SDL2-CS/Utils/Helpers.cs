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

        public static IntPtr ToTTF_Font(this RFont font)
        {
            return (font as Adapters.FontAdapter).Font;
        }

        internal static Adapters.PenAdapter ToPenA(this RPen pen)
        {
            return pen as Adapters.PenAdapter;
        }

        internal static Adapters.BrushAdapter ToBrushA(this RBrush brush)
        {
            return brush as Adapters.BrushAdapter;
        }
        internal static Adapters.GraphicsPathAdapter ToPathA(this RGraphicsPath path)
        {
            return path as Adapters.GraphicsPathAdapter;
        }
        internal static Adapters.ImageAdapter ToImageA(this RImage image)
        {
            return image as Adapters.ImageAdapter;
        }

        public static SDL.SDL_Point ToSDL(this RPoint point)
        {
            return new SDL.SDL_Point { x = (int)point.X, y = (int)point.Y };
        }

        public static void SetToSDLRenderer(this SDL.SDL_Color color)
        {
            SDL.SDL_SetRenderDrawColor(Adapters.SDL2Adapter.Instance.Renderer, color.r, color.g, color.b, color.a);
        }

        public static void SetToSDLRenderer(this RColor color)
        {
            SDL.SDL_SetRenderDrawColor(Adapters.SDL2Adapter.Instance.Renderer, color.R, color.G, color.B, color.A);
        }

        public static RColor ToRColor(this SDL.SDL_Color color)
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
        public static RPoint ToRPoint(this SDL.SDL_MouseButtonEvent e)
        {
            return new RPoint(e.x, e.y);
        }
        public static RPoint ToRPoint(this SDL.SDL_MouseMotionEvent e)
        {
            return new RPoint(e.x, e.y);
        }
        public static RMouseEvent ToRMouseEvent(this SDL.SDL_MouseButtonEvent e)
        {
            return new RMouseEvent((e.button & SDL.SDL_BUTTON_LEFT) != 0);
        }

        public static RKeyEvent ToRKeyEvent(this SDL.SDL_KeyboardEvent e)
        {
            return new RKeyEvent((e.keysym.mod & SDL.SDL_Keymod.KMOD_CTRL) != 0, e.keysym.sym == SDL.SDL_Keycode.SDLK_a, e.keysym.sym == SDL.SDL_Keycode.SDLK_c);
        }
        public static RSize ToRSize(this RRect rect)
        {
            return new RSize(rect.Width - rect.X, rect.Height - rect.Y);
        }

        public static void ShowSDLError(string error_text)
        {
            Console.WriteLine(error_text + " SDL_ttf Error: {0}", SDL.SDL_GetError());
        }

    }
}

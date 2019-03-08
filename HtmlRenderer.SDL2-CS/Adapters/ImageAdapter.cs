using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using SDL2;
using AcentricPixels.HtmlRenderer.SDL2_CS.Utils;
using System.Runtime.InteropServices;

namespace AcentricPixels.HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class ImageAdapter : RImage
    {
        private IntPtr _surface;
        private IntPtr _texture = IntPtr.Zero;
        private int _width = -1;
        private int _height = -1;

        public ImageAdapter(object image)
        {
            if (image.GetType() == typeof(IntPtr))
            {
                _surface = (IntPtr)image;
                var surface = _surface.As<SDL.SDL_Surface>();
                _width = surface.w;
                _height = surface.h;
                Console.WriteLine("Texture w:{0}, h:{1}", _width, _height);
            }
        }

        public override double Width { get { return (double)_width; } }

        public override double Height { get { return (double)_height; } }

        public IntPtr GetTexture(IntPtr renderer, bool create_texture)
        {
            IntPtr texture = IntPtr.Zero;
            if (create_texture || _texture == IntPtr.Zero)
                texture = SDL.SDL_CreateTextureFromSurface(renderer, _surface);

            if (!create_texture && _texture == IntPtr.Zero)
                _texture = texture;

            if (!create_texture && texture == IntPtr.Zero)
                texture = _texture;
            return texture;
        }

        public void Draw(IntPtr renderer, RRect destRect, RRect srcRect, bool create_texture = false)
        {
            var dst_rect = destRect.ToSDL();
            var src_rect = srcRect.ToSDL();
            var texture = GetTexture(renderer, create_texture);
            if (SDL.SDL_RenderCopy(renderer, texture, ref src_rect, ref dst_rect) < 0)
                Helpers.ShowSDLError("Image.Draw(D,S):Unable to SDL_RenderCopy!");

            if (create_texture)
                SDL.SDL_DestroyTexture(texture);
        }

        public void Draw(IntPtr renderer, RRect destRect, bool create_texture = false)
        {
            var dst_rect = destRect.ToSDL();
            var texture = GetTexture(renderer, create_texture);

            if (SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref dst_rect) < 0)
                Helpers.ShowSDLError("Image.Draw(D):Unable to SDL_RenderCopy!");

            if (create_texture)
                SDL.SDL_DestroyTexture(texture);
        }


        public override void Dispose()
        {
            if (_texture != IntPtr.Zero)
                SDL.SDL_DestroyTexture(_texture);
        }
    }
}

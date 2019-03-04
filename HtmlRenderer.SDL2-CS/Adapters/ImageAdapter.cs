using System;
using System.Collections.Generic;
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
    internal sealed class ImageAdapter : RImage
    {
        private IntPtr _texture;
        private int _width = -1;
        private int _height = -1;

        public ImageAdapter(object image)
        {
            if (image.GetType() == typeof(IntPtr))
            {
                _texture = (IntPtr)image;
                var null_ptr = IntPtr.Zero;
                SDL.SDL_QueryTexture(_texture, out uint format, out int access, out _width, out _height);
                Console.WriteLine("Texture w:{0}, h:{1}", _width, _height);
            }
        }

        public override double Width { get { return (double)_width; } }

        public override double Height { get { return (double)_height; } }

        public void Draw(IntPtr renderer, RRect destRect, RRect srcRect)
        {
            var dst_rect = destRect.ToSDL();
            var src_rect = srcRect.ToSDL();
            if (SDL.SDL_RenderCopy(renderer, _texture, ref src_rect, ref dst_rect) < 0)
                Helpers.ShowSDLError("Image.Draw(D,S):Unable to SDL_RenderCopy!");
        }

        public void Draw(IntPtr renderer, RRect destRect)
        {
            var dst_rect = destRect.ToSDL();
            if (SDL.SDL_RenderCopy(renderer, _texture, IntPtr.Zero, ref dst_rect) < 0)
                Helpers.ShowSDLError("Image.Draw(D):Unable to SDL_RenderCopy!");
        }


        public override void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}

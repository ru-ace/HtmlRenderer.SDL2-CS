using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using HtmlRenderer.SDL2_CS.Adapters;
using HtmlRenderer.SDL2_CS.Utils;
using SDL2;


namespace HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class BrushAdapter : RBrush
    {
        internal RColor color = RColor.FromArgb(255, 0, 0, 0);

        internal bool isImage = false;

        internal ImageAdapter image;
        internal RRect image_dstRect;
        internal RPoint image_translateTransformLocation;


        public BrushAdapter(RImage image, RRect dstRect, RPoint translateTransformLocation)
        {
            isImage = true;
            this.image = image.ToImageA();
            image_dstRect = dstRect;
            image_translateTransformLocation = translateTransformLocation;
        }
        public BrushAdapter(RColor color)
        {
            this.color = color;
            isImage = false;
        }

        internal IntPtr RenderTexture(IntPtr renderer, SDL.SDL_Rect rect)
        {
            double x = rect.x;
            double y = rect.y;

            IntPtr surface, xrenderer;
            bool software_renderer = false;
            if (renderer == IntPtr.Zero)
            {
                surface = SDL.SDL_CreateRGBSurfaceWithFormat(0, rect.w, rect.h, 32, SDL.SDL_PIXELFORMAT_RGBA8888);
                xrenderer = SDL.SDL_CreateSoftwareRenderer(surface);
                software_renderer = true;
            }
            else
            {
                surface = IntPtr.Zero;
                xrenderer = renderer;
                software_renderer = false;
            }
            double img_width = image.Width;
            double img_height = image.Height;
            double xs = (image_dstRect.Width / img_width);
            double ys = (image_dstRect.Height / img_height);
            var xtexture = image.GetTexture(xrenderer, software_renderer);
            for (int xi = 0; xi < xs; xi++)
            {
                for (int yi = 0; yi < ys; yi++)
                {
                    int dst_x = (int)(image_dstRect.X + xi * img_width) - (software_renderer ? rect.x : 0);
                    int dst_y = (int)(image_dstRect.Y + yi * img_height) - (software_renderer ? rect.y : 0);
                    var dst_rect = new SDL.SDL_Rect { x = dst_x, y = dst_y, w = (int)img_width, h = (int)img_height };
                    if (SDL.SDL_RenderCopy(xrenderer, xtexture, IntPtr.Zero, ref dst_rect) < 0)
                        Helpers.ShowSDLError("BrushAdapter.RenderTexture:Unable to SDL_RenderCopy!");
                }
            }
            if (software_renderer)
            {
                IntPtr texture = SDL.SDL_CreateTextureFromSurface(SDL2Adapter.Instance.Renderer, surface);
                SDL.SDL_DestroyRenderer(renderer);
                SDL.SDL_FreeSurface(surface);
                return texture;
            }
            else
            {
                return IntPtr.Zero;
            }

        }

        public override void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}

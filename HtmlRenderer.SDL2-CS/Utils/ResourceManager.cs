using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Parse;
using TheArtOfDev.HtmlRenderer.Core.Utils;
using HtmlRenderer.SDL2_CS.Adapters;
using SDL2;

namespace HtmlRenderer.SDL2_CS.Utils
{
    public sealed class ResourceManager
    {
        private static ResourceManager _instance = null;
        private static Dictionary<string, IntPtr> _imageTexture = new Dictionary<string, IntPtr>();

        private ResourceManager()
        {

        }
        public static void OnImageLoad(object sender, HtmlImageLoadEventArgs args)
        {

            Console.WriteLine(args.Src);
            if (!_imageTexture.ContainsKey(args.Src))
            {
                var img_surface = SDL_image.IMG_Load(args.Src);
                if (img_surface.ShowSDLError("OnImageLoad: Unable to IMG_Load!"))
                    return;

                var texture_text = SDL.SDL_CreateTextureFromSurface(SDL2Adapter.Instance.Renderer, img_surface);
                if (texture_text.ShowSDLError("OnImageLoad: Unable to CreateTextureFromSurface!"))
                {
                    SDL.SDL_FreeSurface(img_surface);
                    return;
                }
                SDL.SDL_FreeSurface(img_surface);
                _imageTexture[args.Src] = texture_text;
            }
            args.Handled = true;
            //foreach (var kv in args.Attributes)
            //Console.WriteLine("{0}: {1}", kv.Key, kv.Value);

            args.Callback(_imageTexture[args.Src]);



            Console.WriteLine(args.Src);

        }
        public static ResourceManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ResourceManager();
                return _instance;
            }
        }

        public static void Quit()
        {
            foreach (var kv in _imageTexture)
            {
                SDL.SDL_DestroyTexture(kv.Value);
            }

        }


    }
}

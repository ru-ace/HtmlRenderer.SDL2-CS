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
    internal sealed class ResourceManager
    {
        private static ResourceManager _instance = null;
        private static Dictionary<string, IntPtr> _imageSurface = new Dictionary<string, IntPtr>();

        private ResourceManager()
        {

        }
        public static void OnImageLoad(object sender, HtmlImageLoadEventArgs args)
        {

            Console.WriteLine(args.Src);
            if (!_imageSurface.ContainsKey(args.Src))
            {

                var img_surface = SDL_image.IMG_Load(args.Src);
                if (img_surface.ShowSDLError("OnImageLoad: Unable to IMG_Load!"))
                    return;

                _imageSurface[args.Src] = img_surface;
            }
            args.Handled = true;
            //foreach (var kv in args.Attributes)
            //Console.WriteLine("{0}: {1}", kv.Key, kv.Value);

            args.Callback(_imageSurface[args.Src]);
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
            foreach (var kv in _imageSurface)
            {
                SDL.SDL_FreeSurface(kv.Value);
            }

        }


    }
}

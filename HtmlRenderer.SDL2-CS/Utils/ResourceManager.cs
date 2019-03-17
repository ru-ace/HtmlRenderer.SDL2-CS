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
using AcentricPixels.HtmlRenderer.SDL2_CS.Adapters;
using SDL2;

namespace AcentricPixels.HtmlRenderer.SDL2_CS.Utils
{
    internal sealed class ResourceManager
    {

        private static Dictionary<string, IntPtr> _imageSurface = new Dictionary<string, IntPtr>();
        internal static string directory = "";
        private static Dictionary<int, IntPtr> _cursor = new Dictionary<int, IntPtr>();

        private ResourceManager()
        {

        }
        public static void OnImageLoad(object sender, HtmlImageLoadEventArgs args)
        {

            Console.WriteLine(directory + args.Src);
            if (!_imageSurface.ContainsKey(args.Src))
            {

                var img_surface = SDL_image.IMG_Load(directory + args.Src);
                if (img_surface.ShowSDLError("OnImageLoad: Unable to IMG_Load!"))
                    return;

                _imageSurface[args.Src] = img_surface;
            }
            args.Handled = true;
            //foreach (var kv in args.Attributes)
            //Console.WriteLine("{0}: {1}", kv.Key, kv.Value);

            args.Callback(_imageSurface[args.Src]);
        }

        public static IntPtr GetSDLCursor(SDL.SDL_SystemCursor system_cursor)
        {
            int cursor_id = (int)system_cursor;
            if (!_cursor.ContainsKey(cursor_id))
                _cursor[cursor_id] = SDL.SDL_CreateSystemCursor(system_cursor);

            return _cursor[cursor_id];
        }

        public static void Quit()
        {
            foreach (var kv in _imageSurface)
                SDL.SDL_FreeSurface(kv.Value);

            foreach (var kv in _cursor)
                SDL.SDL_FreeCursor(kv.Value);

        }


    }
}

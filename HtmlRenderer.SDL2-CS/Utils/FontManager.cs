using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using SDL2;
using System.IO;


namespace HtmlRenderer.SDL2_CS.Utils
{
    public sealed class FontManager
    {
        private static FontManager _instance = null;

        private readonly List<IntPtr> font_RWops = new List<IntPtr>();
        private readonly List<IntPtr> font_mem = new List<IntPtr>();

        private FontManager()
        {
            if (SDL_ttf.TTF_WasInit() == 0)
            {
                throw new Exception("You need call SDL_ttf.Init() before using FontManager");
            }
        }

        public static FontManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FontManager();
                return _instance;
            }
        }

        public void RegisterFontsFromDir(string directory)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            FileInfo[] Files = dir.GetFiles("*.ttf");
            foreach (FileInfo file in Files)
            {
                Console.WriteLine(file);
                RegisterFontFromFile(file.FullName);
            }

        }

        public void RegisterFontFromFile(string filename)
        {

            RegisterFontFromBytes(File.ReadAllBytes(filename));
        }

        public void RegisterFontFromBytes(byte[] bytes)
        {
            int size = Marshal.SizeOf(bytes[0]) * bytes.Length;
            IntPtr mem = Marshal.AllocHGlobal(size);
            font_mem.Add(mem);
            Marshal.Copy(bytes, 0, mem, size);
            IntPtr RWops_font = SDL.SDL_RWFromMem(mem, size);
            font_RWops.Add(RWops_font);
            CollectFontInfo(font_RWops.Count - 1);
        }

        public void CollectFontInfo(int font_RWops_id)
        {
            IntPtr font = SDL_ttf.TTF_OpenFontRW(font_RWops[font_RWops_id], 0, 16);
            if (font == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load font! SDL_ttf Error: {0}", SDL.SDL_GetError());
            }
            else
            {
                long fontfaces = SDL_ttf.TTF_FontFaces(font);
                string fontface_familyname = SDL_ttf.TTF_FontFaceFamilyName(font);
                string fontface_stylename = SDL_ttf.TTF_FontFaceStyleName(font);
                int fontface_mono = SDL_ttf.TTF_FontFaceIsFixedWidth(font);

                Console.WriteLine("  TTF_FontFaces: {0}", fontfaces);
                Console.WriteLine("  TTF_FontFaceFamilyName: {0}", fontface_familyname);
                Console.WriteLine("  TTF_FontFaceStyleName: {0}", fontface_stylename);
                Console.WriteLine("  TTF_FontFaceIsFixedWidth: {0}", fontface_mono);
            }
            /*
             3.3.14 TTF_FontFaces Get the number of faces in a font
 3.3.15 TTF_FontFaceIsFixedWidth Get whether font is monospaced or not
 3.3.16 TTF_FontFaceFamilyName Get current font face family name string
 3.3.17 TTF_FontFaceStyleName
     */
        }

        public void Clear()
        {
            for (int i = 0; i < font_mem.Count; i++)
            {
                Marshal.FreeHGlobal(font_mem[i]);
            }
        }

    }
}

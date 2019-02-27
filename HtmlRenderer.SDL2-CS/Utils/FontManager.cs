using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using SDL2;
using System.IO;
using System.Security.Cryptography;

namespace HtmlRenderer.SDL2_CS.Utils
{
    public sealed class FontManager
    {
        private static FontManager _instance = null;

        internal struct Font
        {
            public Font(IntPtr RWops, IntPtr mem, string hash, long index = 0) { this.RWops = RWops; this.mem = mem; this.hash = hash; this.index = index; }
            public IntPtr RWops;
            public IntPtr mem;
            public long index;
            public string hash;
        }

        private readonly List<Font> _fonts = new List<Font>();

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


            string hash = "";
            using (HashAlgorithm sha = new SHA1CryptoServiceProvider())
            {
                byte[] sha1_hash = sha.ComputeHash(bytes);
                hash = BitConverter.ToString(sha1_hash).Replace("-", string.Empty);
            }

            for (int i = 0; i < _fonts.Count; i++)
            {
                if (_fonts[i].hash == hash)
                {
                    //Console.WriteLine("  Already Loaded");
                    return;
                }

            }

            try
            {

                int size = Marshal.SizeOf(bytes[0]) * bytes.Length;
                IntPtr mem = Marshal.AllocHGlobal(size);
                Marshal.Copy(bytes, 0, mem, size);
                IntPtr RWops = SDL.SDL_RWFromMem(mem, size);
                _fonts.Add(new Font(RWops, mem, hash, 0));
                CollectFontInfo(_fonts.Count - 1);
            }
            catch
            {

            }
        }

        public void CollectFontInfo(int font_id)
        {
            IntPtr font = SDL_ttf.TTF_OpenFontRW(_fonts[font_id].RWops, 0, 16);
            if (font == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load font! SDL_ttf Error: {0}", SDL.SDL_GetError());
            }
            else
            {
                long fontfaces = SDL_ttf.TTF_FontFaces(font);
                string fontface_familyname = SDL_ttf.TTF_FontFaceFamilyName(font);
                string fontface_stylename = SDL_ttf.TTF_FontFaceStyleName(font);
                bool fontface_mono = SDL_ttf.TTF_FontFaceIsFixedWidth(font) > 0;
                Console.WriteLine("  Hash:{0}", _fonts[font_id].hash);
                Console.WriteLine("  TTF_FontFaces: {0}", fontfaces);
                Console.WriteLine("  TTF_FontFaceFamilyName: {0}", fontface_familyname);
                Console.WriteLine("  TTF_FontFaceStyleName: {0}", fontface_stylename);
                Console.WriteLine("  TTF_FontFaceIsFixedWidth: {0}", fontface_mono);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _fonts.Count; i++)
            {
                Marshal.FreeHGlobal(_fonts[i].mem);
            }
        }

    }
}

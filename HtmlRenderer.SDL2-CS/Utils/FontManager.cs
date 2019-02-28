using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using SDL2;
using System.IO;
using System.Security.Cryptography;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace HtmlRenderer.SDL2_CS.Utils
{
    public sealed class FontManager
    {
        //Singleton 
        private static FontManager _instance = null;
        public bool UseRWops = false;
        public bool UseFontCache = true;
        internal class Font
        {
            public Font(IntPtr RWops, IntPtr mem, string hash, string filename)
            {
                this.RWops = RWops;
                this.mem = mem;
                this.hash = hash;
                this.filename = filename;

                index = 0;
                fontFamilyId = -1;
                fontStyle = -1;
                mono = false;
            }

            public string filename = "";
            public IntPtr RWops;
            public IntPtr mem;
            public int index;
            public string hash;


            //Font description
            public int fontFamilyId;
            public int fontStyle;
            public bool mono;

            public Font Clone() { return new Font(RWops, mem, hash, filename); }

            public override string ToString()
            {
                string line = "";
                line += String.Format("fontFamilyId:{0} fontStyle:{1}", fontFamilyId, fontStyle);
                return line;
            }

        }



        private readonly List<Font> _fonts = new List<Font>();
        private readonly List<string> _fontFamily = new List<string>();
        //_fontCache[fontFamilyId][fontStyle][fontPtSize]
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, IntPtr>>> _fontCache = new Dictionary<int, Dictionary<int, Dictionary<int, IntPtr>>>();

        //value from TheArtOfDev.HtmlRenderer.Core.Utils.CssConstants.FontSize        
        private const double _defaultFontSize = 11f;
        private int _defaultFontFamilyId = -1;

        private readonly Dictionary<string, int> _defaultsFontFamilyId = new Dictionary<string, int>
        {
            { "serif" , -1},
            { "sans-serif" , -1},
            { "monospace" , -1}
        };


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

        public void SetDefaultsFontFamily(string serif, string sans_serif = "", string monospace = "")
        {

            foreach (var familyname in _defaultsFontFamilyId.Keys.ToList())
            {
                if (GetFontFamilyId(familyname) < 0)
                {
                    string value = "";
                    switch (familyname)
                    {
                        case "serif": value = serif; break;
                        case "sans-serif": value = sans_serif; break;
                        case "monospace": value = monospace; break;
                    }
                    if (value != "")
                        _defaultsFontFamilyId[familyname] = GetFontFamilyId(value);

                }
            }
            _defaultFontFamilyId = GetFontFamilyId("serif");
            if (_defaultFontFamilyId == -1)
                throw new Exception("FontFamilyName [" + serif + "] for serif not found.");

            foreach (var kv in _defaultsFontFamilyId)
                if (kv.Value == -1)
                    _defaultsFontFamilyId[kv.Key] = _defaultFontFamilyId;

        }

        public IntPtr GetTTF_Font(string familyname, double size, RFontStyle style)
        {
            if (_defaultFontFamilyId == -1)
                throw new Exception("Run FontManager.Instance.SetDefaultsFontFamily() after calling FontManager.Instance.RegisterFont* methods.");


            int fontfamily_id = GetFontFamilyId(familyname);
            int style_id = (int)style;
            int size_id = (int)Math.Round(_defaultFontSize * size);
            if (!UseFontCache)
                return OpenTTF_Font(fontfamily_id, size_id, style_id);

            //fontfamily
            if (!_fontCache.ContainsKey(fontfamily_id))
                _fontCache[fontfamily_id] = new Dictionary<int, Dictionary<int, IntPtr>>();
            var fc_family = _fontCache[fontfamily_id];

            //style
            if (!fc_family.ContainsKey(style_id))
                fc_family[style_id] = new Dictionary<int, IntPtr>();
            var fc_style = fc_family[style_id];

            //size
            if (!fc_style.ContainsKey(size_id))
                fc_style[size_id] = OpenTTF_Font(fontfamily_id, size_id, style_id);


            return fc_style[size_id];

        }

        private IntPtr OpenTTF_Font(int fontfamily_id, int size_id, int style_id)
        {
            int font_id = FindBestMatchFontId(fontfamily_id, style_id);
            Console.WriteLine("Font:" + font_id + " size=" + size_id + " style=" + style_id + " file=" + _fonts[font_id].filename);

            IntPtr font = IntPtr.Zero;
            if (UseRWops)
                font = SDL_ttf.TTF_OpenFontIndexRW(_fonts[font_id].RWops, 0, size_id, _fonts[font_id].index);
            else
                font = SDL_ttf.TTF_OpenFontIndex(_fonts[font_id].filename, size_id, _fonts[font_id].index);

            font.ShowSDLError("Failed to load font!");

            SDL_ttf.TTF_SetFontStyle(font, style_id);
            //SDL_ttf.TTF_SetFontHinting(font, )
            return font;
        }

        private int FindBestMatchFontId(int fontfamily_id, int style_id)
        {
            int real_fontStyle = style_id & 3;
            int best_font_id = -1;
            for (int font_id = 0; font_id < _fonts.Count; font_id++)
            {
                if (_fonts[font_id].fontFamilyId != fontfamily_id) continue;

                if (_fonts[font_id].fontStyle == real_fontStyle)
                    return font_id;

                if (best_font_id < 0 && _fonts[font_id].fontStyle == 0) best_font_id = font_id;

                if (real_fontStyle == 3 && _fonts[font_id].fontStyle > 0) best_font_id = font_id;
            }
            return best_font_id;
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

            Console.WriteLine("Registered FontFamily:");
            foreach (var fontname in _fontFamily)
                Console.WriteLine("  " + fontname);

        }

        public void RegisterFontFromFile(string filename)
        {

            RegisterFontFromBytes(filename, File.ReadAllBytes(filename));
        }

        public void RegisterFontFromBytes(string filename, byte[] bytes)
        {
            string hash = "";
            using (HashAlgorithm sha = new SHA1CryptoServiceProvider())
            {
                byte[] sha1_hash = sha.ComputeHash(bytes);
                hash = BitConverter.ToString(sha1_hash).Replace("-", string.Empty);
            }

            for (int i = 0; i < _fonts.Count; i++)
                if (_fonts[i].hash == hash)
                    return;

            try
            {
                IntPtr RWops = IntPtr.Zero;
                IntPtr mem = IntPtr.Zero;
                if (UseRWops)
                {
                    int size = Marshal.SizeOf(bytes[0]) * bytes.Length;
                    mem = Marshal.AllocHGlobal(size);
                    Marshal.Copy(bytes, 0, mem, size);
                    RWops = SDL.SDL_RWFromMem(mem, size);
                    //IntPtr RWops = SDL_RWFromConstMem(mem, size);
                    if (RWops == IntPtr.Zero)
                        throw new Exception("SDL_RWFromMem error: " + SDL.SDL_GetError());
                }

                _fonts.Add(new Font(RWops, mem, hash, filename));
                CollectFontInfo(_fonts.Count - 1);
            }
            catch (Exception e)
            {
                Console.WriteLine("RegisterFontFromBytes Exception: {0}", e.Message);
            }
        }

        private int GetFontFamilyId(string familyname, bool create_if_absent = false)
        {
            string familyname_l = familyname.ToLower();
            int index = _fontFamily.IndexOf(familyname_l);

            if (index == -1 && _defaultsFontFamilyId.ContainsKey(familyname_l))
                index = _defaultsFontFamilyId[familyname_l];

            if (index == -1 && create_if_absent)
            {
                _fontFamily.Add(familyname_l);
                index = _fontFamily.Count - 1;
                if (_defaultsFontFamilyId.ContainsKey(familyname_l))
                    _defaultsFontFamilyId[familyname_l] = index;
            }
            if (index == -1)
                index = _defaultFontFamilyId;

            return index;
        }

        public void CollectFontInfo(int font_id)
        {
            IntPtr font = IntPtr.Zero;
            if (UseRWops)
                font = SDL_ttf.TTF_OpenFontIndexRW(_fonts[font_id].RWops, 0, 16, _fonts[font_id].index);
            else
                font = SDL_ttf.TTF_OpenFontIndex(_fonts[font_id].filename, 16, _fonts[font_id].index);

            //IntPtr font = SDL_ttf.TTF_OpenFontRW(_fonts[font_id].RWops, 0, 16);
            if (font == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load font! SDL_ttf Error: {0}", SDL.SDL_GetError());
            }
            else
            {
                int fontfaces = (int)SDL_ttf.TTF_FontFaces(font);
                string fontface_familyname = SDL_ttf.TTF_FontFaceFamilyName(font);
                string fontface_stylename = SDL_ttf.TTF_FontFaceStyleName(font);
                bool fontface_mono = (SDL_ttf.TTF_FontFaceIsFixedWidth(font) > 0);
                int font_style = SDL_ttf.TTF_GetFontStyle(font);

                _fonts[font_id].fontFamilyId = GetFontFamilyId(fontface_familyname, true);
                _fonts[font_id].fontStyle = font_style;
                _fonts[font_id].mono = fontface_mono;

                /*
                Console.WriteLine("  Hash:{0}", _fonts[font_id].hash);
                Console.WriteLine("  TTF_FontFaces: {0}", fontfaces);
                Console.WriteLine("  TTF_FontFaceIsFixedWidth: {0}", fontface_mono);

                Console.WriteLine("  TTF_FontFaceFamilyName: {0}", fontface_familyname);
                Console.WriteLine("  TTF_FontFaceStyleName: {0}", fontface_stylename);
                Console.WriteLine("  TTF_GetFontStyle: {0}", font_style);
                */

                if (_fonts[font_id].index == 0 && fontfaces > 1)
                {
                    for (int index = 1; index < fontfaces; index++)
                    {
                        //Console.WriteLine("         index: {0}", index);
                        Font subfont = _fonts[font_id].Clone();
                        subfont.index = index;
                        _fonts.Add(subfont);
                        CollectFontInfo(_fonts.Count - 1);
                    }

                }
                SDL_ttf.TTF_CloseFont(font);
            }
        }

        public void ClearFontCache()
        {
            if (UseFontCache)
                foreach (var family in _fontCache)
                    foreach (var style in _fontCache[family.Key])
                        foreach (var size in _fontCache[family.Key][style.Key])
                            SDL_ttf.TTF_CloseFont(size.Value);

            _fontCache.Clear();
        }

        public void Quit()
        {
            if (UseRWops)
                for (int i = 0; i < _fonts.Count; i++)
                    if (_fonts[i].index == 0)
                        Marshal.FreeHGlobal(_fonts[i].mem);

            _fonts.Clear();
        }
        /* mem refers to a void*, IntPtr to an SDL_RWops* */
        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SDL_RWFromConstMem(IntPtr mem, int size);
    }
}

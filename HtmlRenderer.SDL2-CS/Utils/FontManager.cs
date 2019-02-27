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

        internal class Font
        {
            public Font(IntPtr RWops, IntPtr mem, string hash)
            {
                this.RWops = RWops;
                this.mem = mem;
                this.hash = hash;

                index = 0;
                fontFamilyId = -1;
                fontStyle = -1;
                mono = false;
            }

            public IntPtr RWops;
            public IntPtr mem;
            public int index;
            public string hash;


            //Font description
            public int fontFamilyId;
            public int fontStyle;
            public bool mono;

            public Font Clone() { return new Font(RWops, mem, hash); }

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
            foreach (var kv in _defaultsFontFamilyId)
            {
                if (GetFontFamilyId(kv.Key) < 0)
                {
                    string value = "";
                    switch (kv.Key)
                    {
                        case "serif": value = serif; break;
                        case "sans-serif": value = sans_serif; break;
                        case "monospace": value = monospace; break;
                    }
                    if (value != "")
                        _defaultsFontFamilyId[kv.Key] = GetFontFamilyId(value);

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

            //fontfamily
            int fontfamily_id = GetFontFamilyId(familyname);



            if (fontfamily_id == -1) return IntPtr.Zero;
            if (!_fontCache.ContainsKey(fontfamily_id))
                _fontCache[fontfamily_id] = new Dictionary<int, Dictionary<int, IntPtr>>();
            var fc_family = _fontCache[fontfamily_id];

            //style
            var style_id = (int)style;
            if (!fc_family.ContainsKey(style_id))
                fc_family[style_id] = new Dictionary<int, IntPtr>();
            var fc_style = fc_family[style_id];

            //size
            int size_id = (int)Math.Round(_defaultFontSize * size);
            if (!fc_style.ContainsKey(size_id))
                fc_style[size_id] = OpenTTF_Font(fontfamily_id,);

            return fc_style[size_id];

        }

        private IntPtr OpenTTF_Font(int fontfamily_id, int style_id, int size_id)
        {
            int font_id = FindBestMatchFontId(fontfamily_id, style_id);
            IntPtr font = SDL_ttf.TTF_OpenFontIndexRW(_fonts[font_id].RWops, 0, size_id, _fonts[font_id].index);
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

                if (_fonts[font_id].fontStyle == 0 && best_font_id < 0) best_font_id = font_id;

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
                _fonts.Add(new Font(RWops, mem, hash));
                CollectFontInfo(_fonts.Count - 1);
            }
            catch
            {

            }
        }

        private int GetFontFamilyId(string familyname, bool create_if_absent = false)
        {
            string familyname_l = familyname.ToLower();
            int index = _fontFamily.IndexOf(familyname);

            if (index == -1 && _defaultsFontFamilyId.ContainsKey(familyname_l))
                index = _defaultsFontFamilyId[familyname_l];

            if (index == -1 && create_if_absent)
            {
                _fontFamily.Add(familyname_l);
                index = _fontFamily.Count - 1;
                if (_defaultsFontFamilyId.ContainsKey(familyname_l))
                {
                    _defaultsFontFamilyId[familyname_l] = index;
                }
            }
            if (index == -1)
                index = _defaultFontFamilyId;

            return index;
        }

        public void CollectFontInfo(int font_id)
        {
            IntPtr font = SDL_ttf.TTF_OpenFontIndexRW(_fonts[font_id].RWops, 0, 16, _fonts[font_id].index);
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

                /*
                Console.WriteLine("  Hash:{0}", _fonts[font_id].hash);
                Console.WriteLine("  TTF_FontFaces: {0}", fontfaces);
                Console.WriteLine("  TTF_FontFaceIsFixedWidth: {0}", fontface_mono);

                Console.WriteLine("  TTF_FontFaceFamilyName: {0}", fontface_familyname);
                Console.WriteLine("  TTF_FontFaceStyleName: {0}", fontface_stylename);
                Console.WriteLine("  TTF_GetFontStyle: {0}", font_style);
                */

                _fonts[font_id].fontFamilyId = GetFontFamilyId(fontface_familyname);
                _fonts[font_id].fontStyle = font_style;
                _fonts[font_id].mono = fontface_mono;

                if (_fonts[font_id].index == 0 && fontfaces > 1)
                {
                    for (int index = 1; index < fontfaces; index++)
                    {
                        Font subfont = _fonts[font_id].Clone();
                        subfont.index = index;
                        _fonts.Add(subfont);
                        CollectFontInfo(_fonts.Count - 1);
                    }

                }
                SDL_ttf.TTF_CloseFont(font);
            }
        }

        public void Quit()
        {
            for (int i = 0; i < _fonts.Count; i++)
                if (_fonts[i].index == 0)
                    Marshal.FreeHGlobal(_fonts[i].mem);

        }

    }
}

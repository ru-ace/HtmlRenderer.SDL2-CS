using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using SDL2;
using System.IO;
using System.Security.Cryptography;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace AcentricPixels.HtmlRenderer.SDL2_CS.Utils
{
    internal sealed class FontManager : IDisposable
    {
        //Singleton 
        private static FontManager _instance = null;
        public bool UseRWops = true;
        public bool CreateRWopsCacheOnFontRegister = false;
        public bool UseFontCache = true;
        internal class Font : IDisposable
        {


            public string filename = "";
            private IntPtr _RWops = IntPtr.Zero;
            private IntPtr _mem = IntPtr.Zero;
            private int _mem_size = -1;
            private Font _parent = null;

            public Font(string filename)
            {

                this.filename = filename;
                byte[] bytes = File.ReadAllBytes(filename);

                using (HashAlgorithm sha = new SHA1CryptoServiceProvider())
                {
                    byte[] sha1_hash = sha.ComputeHash(bytes);
                    hash = BitConverter.ToString(sha1_hash).Replace("-", string.Empty);
                }

                _RWops = IntPtr.Zero;
                _mem = IntPtr.Zero;
                if (Instance.UseRWops && Instance.CreateRWopsCacheOnFontRegister)
                    CreateRWops();
            }

            private Font(Font parent)
            {
                _parent = parent;
                hash = _parent.hash;
                filename = _parent.filename;

                index = 0;
                fontFamilyId = -1;
                fontStyle = -1;
                mono = false;
            }



            private void CreateRWops()
            {
                if (_mem == IntPtr.Zero)
                {
                    byte[] bytes = File.ReadAllBytes(filename);
                    _mem_size = Marshal.SizeOf(bytes[0]) * bytes.Length;
                    _mem = Marshal.AllocHGlobal(_mem_size);
                    Marshal.Copy(bytes, 0, _mem, _mem_size);
                }
                _RWops = SDL.SDL_RWFromMem(_mem, _mem_size);
                if (_RWops.ShowSDLError("FontManager.Font.RWops: SDL_RWFromMem failed!"))
                    throw new Exception("SDL_RWFromMem error: " + SDL.SDL_GetError());
            }
            public IntPtr RWops
            {
                get
                {
                    if (_parent != null)
                        return _parent.RWops;

                    //! About this solution and commented "if" statement:
                    //! https://bugzilla.libsdl.org/show_bug.cgi?id=4524
                    //! https://bugzilla.libsdl.org/show_bug.cgi?id=4526

                    //if (_RWops == IntPtr.Zero)
                    CreateRWops();
                    return _RWops;
                }
            }


            public int index = 0;
            public string hash = "";


            //Font description
            public int fontFamilyId = -1;
            public int fontStyle = -1;
            public bool mono = false;

            public Font Clone() { return new Font(this); }

            public void Dispose()
            {
                if (_mem != IntPtr.Zero)
                    Marshal.FreeHGlobal(_mem);
            }

            public override string ToString()
            {
                string line = "";
                line += String.Format("fontFamilyId:{0} fontStyle:{1}", fontFamilyId, fontStyle);
                return line;
            }

        }



        private readonly List<Font> _fonts = new List<Font>();
        private readonly List<string> _fontFamily = new List<string>();
        private readonly List<string> _fontFamilyForAdapter = new List<string>();

        public string[] Families { get { return _fontFamilyForAdapter.ToArray(); } }

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
                    {
                        _defaultsFontFamilyId[familyname] = GetFontFamilyId(value);
                        if (!_fontFamilyForAdapter.Contains(familyname)) _fontFamilyForAdapter.Add(familyname);
                    }
                }
            }
            _defaultFontFamilyId = GetFontFamilyId("serif");
            if (_defaultFontFamilyId == -1)
                throw new Exception("FontFamilyName [" + serif + "] for serif not found.");

            foreach (var familyname in _defaultsFontFamilyId.Keys.ToList())
                if (_defaultsFontFamilyId[familyname] == -1)
                    _defaultsFontFamilyId[familyname] = _defaultFontFamilyId;

        }

        public IntPtr GetTTF_Font(string familyname, double size, RFontStyle style)
        {
            if (_defaultFontFamilyId == -1)
                throw new Exception("Run FontManager.Instance.SetDefaultsFontFamily() after calling FontManager.Instance.RegisterFont* methods.");


            int fontfamily_id = GetFontFamilyId(familyname);
            int style_id = (int)style;
            int size_id = (int)size;// Math.Round(_defaultFontSize * size);
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
            //Console.WriteLine("Font:" + font_id + " size=" + size_id + " style=" + style_id + " file=" + _fonts[font_id].filename);

            IntPtr font = IntPtr.Zero;
            if (UseRWops)
                font = SDL_ttf.TTF_OpenFontIndexRW(_fonts[font_id].RWops, 1, size_id, _fonts[font_id].index);
            else
                font = SDL_ttf.TTF_OpenFontIndex(_fonts[font_id].filename, size_id, _fonts[font_id].index);

            if (!font.ShowSDLError("Failed to load font!"))
            {
                SDL_ttf.TTF_SetFontStyle(font, style_id);
                //SDL_ttf.TTF_SetFontHinting(font, )
            }

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

        public void RegisterFontsDir(string directory)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            FileInfo[] ttf_files = dir.GetFiles("*.ttf");
            foreach (FileInfo file in ttf_files)
            {
                //Console.WriteLine(file);
                RegisterFontFile(file.FullName);
            }
            FileInfo[] fon_files = dir.GetFiles("*.fon");
            foreach (FileInfo file in fon_files)
            {
                //Console.WriteLine(file);
                RegisterFontFile(file.FullName);
            }

            Console.WriteLine("Registered FontFamily:");
            foreach (var fontname in _fontFamily)
                Console.WriteLine("  " + fontname);

        }

        public void RegisterFontFile(string filename)
        {
            Font font = new Font(filename);
            for (int i = 0; i < _fonts.Count; i++)
                if (_fonts[i].hash == font.hash)
                    return;

            if (CollectFontInfo(font))
                _fonts.Add(font);
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
                _fontFamilyForAdapter.Add(familyname_l);
                if (familyname != familyname_l) _fontFamilyForAdapter.Add(familyname);

                index = _fontFamily.Count - 1;
                if (_defaultsFontFamilyId.ContainsKey(familyname_l))
                    _defaultsFontFamilyId[familyname_l] = index;
            }
            if (index == -1)
                index = _defaultFontFamilyId;

            return index;
        }

        private bool CollectFontInfo(Font ifont)
        {
            IntPtr font = IntPtr.Zero;
            if (UseRWops && CreateRWopsCacheOnFontRegister)
                font = SDL_ttf.TTF_OpenFontIndexRW(ifont.RWops, 1, 16, ifont.index);
            else
                font = SDL_ttf.TTF_OpenFontIndex(ifont.filename, 16, ifont.index);

            if (font.ShowSDLError("FontManager.CollectFontInfo(): Failed to load font!"))
            {
                return false;
            }
            else
            {
                int fontfaces = (int)SDL_ttf.TTF_FontFaces(font);
                string fontface_familyname = SDL_ttf.TTF_FontFaceFamilyName(font);
                string fontface_stylename = SDL_ttf.TTF_FontFaceStyleName(font);
                bool fontface_mono = (SDL_ttf.TTF_FontFaceIsFixedWidth(font) > 0);
                int font_style = SDL_ttf.TTF_GetFontStyle(font);

                ifont.fontFamilyId = GetFontFamilyId(fontface_familyname, true);
                ifont.fontStyle = font_style;
                ifont.mono = fontface_mono;

                /*
                Console.WriteLine("  Hash:{0}", _fonts[font_id].hash);
                Console.WriteLine("  TTF_FontFaces: {0}", fontfaces);
                Console.WriteLine("  TTF_FontFaceIsFixedWidth: {0}", fontface_mono);

                Console.WriteLine("  TTF_FontFaceFamilyName: {0}", fontface_familyname);
                Console.WriteLine("  TTF_FontFaceStyleName: {0}", fontface_stylename);
                Console.WriteLine("  TTF_GetFontStyle: {0}", font_style);
                */

                if (ifont.index == 0 && fontfaces > 1)
                {
                    for (int index = 1; index < fontfaces; index++)
                    {
                        //Console.WriteLine("         index: {0}", index);
                        Font sub_ifont = ifont.Clone();
                        sub_ifont.index = index;
                        if (CollectFontInfo(sub_ifont))
                            _fonts.Add(sub_ifont);
                    }

                }
                SDL_ttf.TTF_CloseFont(font);
                return true;
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

        public void ClearFontRegister()
        {
            if (UseRWops)
                for (int i = 0; i < _fonts.Count; i++)
                    _fonts[i].Dispose();

            _fonts.Clear();
            _fontFamily.Clear();
            _fontFamilyForAdapter.Clear();

            _defaultFontFamilyId = -1;
            foreach (var familyname in _defaultsFontFamilyId.Keys.ToList())
                _defaultsFontFamilyId[familyname] = -1;

        }
        public void Dispose()
        {
            ClearFontCache();
            ClearFontRegister();
        }

    }
}

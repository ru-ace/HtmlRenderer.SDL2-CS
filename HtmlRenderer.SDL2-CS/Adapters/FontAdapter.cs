using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using SDL2;

namespace HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class FontAdapter : RFont

    {
        /// <summary>
        /// Cached font whitespace width.
        /// </summary>
        private double _whitespaceWidth = -1;
        /// <summary>
        /// the vertical offset of the font underline location from the top of the font.
        /// </summary>
        private double _underlineOffset = -1;

        /// <summary>
        /// IntPtr TTF_Font
        /// </summary>
        private IntPtr readonly _font = IntPtr.Zero;

        private double _size = -1;
        /// <summary>
        /// Cached font height.
        /// </summary>
        private double _height = -1;


        //private RFontStyle _style;


        public FontAdapter(RFontFamily family, double size, RFontStyle style)
        {
            _font = Utils.FontManager.Instance.GetTTF_Font(family.Name, size, style);
        }

        public IntPtr Font { get { return _font; } }

        public override double Size { get { return _size; } }

        public override double LeftPadding { get { return Height / 6f; } }

        public override double Height
        {
            get
            {
                if (_height < 0 && _font != IntPtr.Zero)
                    _height = (double)SDL_ttf.TTF_FontLineSkip(_font);
                return Height;
            }
        }

        public override double UnderlineOffset
        {
            get
            {
                if (_underlineOffset < 0 && _font != IntPtr.Zero)
                    _underlineOffset = Height - (double)SDL_ttf.TTF_FontDescent(_font);
                return _underlineOffset;
            }
        }


        public override double GetWhitespaceWidth(RGraphics graphics)
        {
            if (_whitespaceWidth < 0)
                _whitespaceWidth = graphics.MeasureString(" ", this).Width;
            return _whitespaceWidth;
        }

    }
}

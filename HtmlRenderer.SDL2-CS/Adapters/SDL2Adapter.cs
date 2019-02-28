using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using SDL2;


namespace HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class SDL2Adapter : RAdapter
    {

        IntPtr _renderer = IntPtr.Zero;

        private static SDL2Adapter _instance = null;

        private SDL2Adapter()
        {

        }

        public static SDL2Adapter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SDL2Adapter();
                return _instance;
            }
        }

        public IntPtr Renderer
        {
            set { _renderer = value; }
            get { return _renderer; }
        }

        protected override RImage ConvertImageInt(object image)
        {
            throw new NotImplementedException();
        }

        protected override RFont CreateFontInt(string family, double size, RFontStyle style)
        {
            return new FontAdapter(family, size, style);
        }

        protected override RFont CreateFontInt(RFontFamily family, double size, RFontStyle style)
        {
            return new FontAdapter(family.Name, size, style);
        }

        protected override RBrush CreateLinearGradientBrush(RRect rect, RColor color1, RColor color2, double angle)
        {
            throw new NotImplementedException();
        }

        protected override RPen CreatePen(RColor color)
        {
            throw new NotImplementedException();
        }

        protected override RBrush CreateSolidBrush(RColor color)
        {
            throw new NotImplementedException();
        }

        protected override RColor GetColorInt(string colorName)
        {
            return Utils.Color.FromKnownColor(colorName);
        }

        protected override RImage ImageFromStreamInt(Stream memoryStream)
        {
            throw new NotImplementedException();
        }
    }
}

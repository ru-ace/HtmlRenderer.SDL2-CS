using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace AcentricPixels.HtmlRenderer.SDL2_CS.Adapters
{

    internal sealed class FontFamilyAdapter : RFontFamily
    {
        private readonly string _fontfamily_name;
        public FontFamilyAdapter(string fontfamily_name)
        {
            _fontfamily_name = fontfamily_name;
        }

        public override string Name
        {
            get { return _fontfamily_name; }
        }
    }
}

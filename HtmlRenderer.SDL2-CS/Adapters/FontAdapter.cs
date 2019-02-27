using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class FontAdapter : RFont

    {
        public override double Size => throw new NotImplementedException();

        public override double Height => throw new NotImplementedException();

        public override double UnderlineOffset => throw new NotImplementedException();

        public override double LeftPadding => throw new NotImplementedException();

        public override double GetWhitespaceWidth(RGraphics graphics)
        {
            throw new NotImplementedException();
        }
    }
}

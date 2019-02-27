using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class ImageAdapter : RImage
    {
        public override double Width => throw new NotImplementedException();

        public override double Height => throw new NotImplementedException();

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

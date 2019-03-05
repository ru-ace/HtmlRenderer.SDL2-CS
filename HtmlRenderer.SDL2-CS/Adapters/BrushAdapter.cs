using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;


namespace HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class BrushAdapter : RBrush
    {
        internal RColor color = RColor.FromArgb(255, 0, 0, 0);

        public BrushAdapter(RColor color)
        {
            this.color = color;
        }

        public override void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}

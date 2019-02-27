using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class PenAdapter : RPen
    {
        public override double Width { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override RDashStyle DashStyle { set => throw new NotImplementedException(); }
    }
}

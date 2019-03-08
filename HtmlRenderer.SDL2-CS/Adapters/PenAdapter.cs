using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace AcentricPixels.HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class PenAdapter : RPen
    {
        internal RColor color = RColor.FromArgb(255, 0, 0, 0);
        internal double width = 1;
        internal RDashStyle dashStyle = RDashStyle.Solid;
        // dotted = Dot
        // dashed = Dash
        // double = Solid
        // groove = Solid
        // ridge = Solid
        // inset = DrarPolygon
        // outset = DrawPolygon

        public PenAdapter(RColor color)
        {
            this.color = color;
        }

        public override double Width { get { return width; } set { width = value; } }
        public override RDashStyle DashStyle { set { dashStyle = value; } }
    }
}

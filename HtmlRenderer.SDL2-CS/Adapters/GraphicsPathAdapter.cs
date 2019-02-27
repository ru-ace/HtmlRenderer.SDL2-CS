using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class GraphicsPathAdapter : RGraphicsPath
    {
        public override void ArcTo(double x, double y, double size, Corner corner)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void LineTo(double x, double y)
        {
            throw new NotImplementedException();
        }

        public override void Start(double x, double y)
        {
            throw new NotImplementedException();
        }
    }
}

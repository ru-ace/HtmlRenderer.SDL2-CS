using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using AcentricPixels.HtmlRenderer.SDL2_CS.Adapters;
using AcentricPixels.HtmlRenderer.SDL2_CS.Utils;

namespace AcentricPixels.HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class ControlAdapter : RControl
    {
        public ControlAdapter(RAdapter adapter) : base(adapter)
        {

        }

        public override bool LeftMouseButton => throw new NotImplementedException();

        public override bool RightMouseButton => throw new NotImplementedException();

        public override RPoint MouseLocation => throw new NotImplementedException();

        public override void DoDragDropCopy(object dragDropData)
        {
            throw new NotImplementedException();
        }

        public override void Invalidate()
        {
            throw new NotImplementedException();
        }

        public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
        {
            throw new NotImplementedException();
        }

        public override void SetCursorDefault()
        {
            throw new NotImplementedException();
        }

        public override void SetCursorHand()
        {
            throw new NotImplementedException();
        }

        public override void SetCursorIBeam()
        {
            throw new NotImplementedException();
        }
    }
}

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
    internal sealed class GraphicsAdapter : RGraphics
    {
        public GraphicsAdapter(RAdapter adapter, RRect initialClip) : base(adapter, initialClip)
        {
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(RImage image, RRect destRect, RRect srcRect)
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(RImage image, RRect destRect)
        {
            throw new NotImplementedException();
        }

        public override void DrawLine(RPen pen, double x1, double y1, double x2, double y2)
        {
            throw new NotImplementedException();
        }

        public override void DrawPath(RPen pen, RGraphicsPath path)
        {
            throw new NotImplementedException();
        }

        public override void DrawPath(RBrush brush, RGraphicsPath path)
        {
            throw new NotImplementedException();
        }

        public override void DrawPolygon(RBrush brush, RPoint[] points)
        {
            throw new NotImplementedException();
        }

        public override void DrawRectangle(RPen pen, double x, double y, double width, double height)
        {
            throw new NotImplementedException();
        }

        public override void DrawRectangle(RBrush brush, double x, double y, double width, double height)
        {
            throw new NotImplementedException();
        }

        public override void DrawString(string str, RFont font, RColor color, RPoint point, RSize size, bool rtl)
        {
            throw new NotImplementedException();
        }

        public override RGraphicsPath GetGraphicsPath()
        {
            throw new NotImplementedException();
        }

        public override RBrush GetTextureBrush(RImage image, RRect dstRect, RPoint translateTransformLocation)
        {
            throw new NotImplementedException();
        }

        public override RSize MeasureString(string str, RFont font)
        {
            throw new NotImplementedException();
        }

        public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
        {
            throw new NotImplementedException();
        }

        public override void PopClip()
        {
            throw new NotImplementedException();
        }

        public override void PushClip(RRect rect)
        {
            throw new NotImplementedException();
        }

        public override void PushClipExclude(RRect rect)
        {
            throw new NotImplementedException();
        }

        public override void ReturnPreviousSmoothingMode(object prevMode)
        {
            throw new NotImplementedException();
        }

        public override object SetAntiAliasSmoothingMode()
        {
            throw new NotImplementedException();
        }
    }
}

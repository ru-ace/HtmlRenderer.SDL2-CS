using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using HtmlRenderer.SDL2_CS.Adapters;
using HtmlRenderer.SDL2_CS.Utils;


namespace HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class BrushAdapter : RBrush
    {
        internal RColor color = RColor.FromArgb(255, 0, 0, 0);

        internal bool isImage = false;

        internal ImageAdapter image;
        internal RRect image_dstRect;
        internal RPoint image_translateTransformLocation;


        public BrushAdapter(RImage image, RRect dstRect, RPoint translateTransformLocation)
        {
            isImage = true;
            this.image = image.ToImageA();
            image_dstRect = dstRect;
            image_translateTransformLocation = translateTransformLocation;
        }
        public BrushAdapter(RColor color)
        {
            this.color = color;
            isImage = false;
        }

        public override void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using AcentricPixels.HtmlRenderer.SDL2_CS.Adapters;
using AcentricPixels.HtmlRenderer.SDL2_CS.Utils;
using SDL2;
namespace AcentricPixels.HtmlRenderer.SDL2_CS.Adapters
{
    internal sealed class ControlAdapter : RControl
    {
        public ControlAdapter(RAdapter adapter) : base(adapter) { }

        public ControlAdapter(RAdapter adapter, RPoint mouse_location) : base(adapter)
        {
            _mouseLocation = mouse_location;
        }
        public ControlAdapter(RAdapter adapter, RPoint mouse_location, SDL.SDL_MouseButtonEvent mouse_event) : base(adapter)
        {
            _mouseLocation = mouse_location;
            _leftMouseButton = (mouse_event.button & SDL.SDL_BUTTON_LEFT) > 0;
            _rightMouseButton = (mouse_event.button & SDL.SDL_BUTTON_RIGHT) > 0;
        }


        private bool _leftMouseButton = false;
        private bool _rightMouseButton = false;
        private RPoint _mouseLocation;

        public override bool LeftMouseButton { get { return _leftMouseButton; } }

        public override bool RightMouseButton { get { return _rightMouseButton; } }

        public override RPoint MouseLocation { get { return _mouseLocation; } }




        public override void DoDragDropCopy(object dragDropData)
        {
            Console.WriteLine("ControlAdapter.DoDragDropCopy Not Implemented.");
            //throw new NotImplementedException();
        }

        public override void Invalidate()
        {
            Console.WriteLine("ControlAdapter.Invalidate Not Implemented.");

            //throw new NotImplementedException();
        }

        public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
        {
            Console.WriteLine("ControlAdapter.MeasureString Not Implemented.");

            throw new NotImplementedException();
        }

        private static int _cursor = -1;
        private SDL.SDL_SystemCursor cursor
        {
            set
            {
                if (_cursor == (int)value) return;
                _cursor = (int)value;
                SDL.SDL_SetCursor(ResourceManager.GetSDLCursor(value));
            }
        }

        public override void SetCursorDefault()
        {
            cursor = SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_ARROW;
        }

        public override void SetCursorHand()
        {
            cursor = SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_HAND;
        }

        public override void SetCursorIBeam()
        {
            cursor = SDL.SDL_SystemCursor.SDL_SYSTEM_CURSOR_IBEAM;
        }
    }
}

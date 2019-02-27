using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlRenderer.SDL2_CS.Utils
{
    internal sealed class ResourceManager
    {
        private static ResourceManager _instance = null;

        private ResourceManager()
        {
        }

        public static ResourceManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ResourceManager();
                return _instance;
            }
        }

    }
}

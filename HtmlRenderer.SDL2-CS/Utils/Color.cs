using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
//using System.Drawing; // for GenerateColorCode

namespace AcentricPixels.HtmlRenderer.SDL2_CS.Utils
{
    internal static class Color
    {
        /*
        static public string GenerateColorCode()
        {
            string code = "";
            string format = "\"{0}\", RColor.FromArgb({1}, {2}, {3}, {4})";
            foreach (KnownColor kc in Enum.GetValues(typeof(KnownColor)))
            {
                Color color = Color.FromKnownColor(kc);
                code += "{" + String.Format(format, color.Name, color.A, color.R, color.G, color.B) + "},\n";
            }
            Console.WriteLine(code);
            Console.ReadLine();            
        }
        */
        /* 
        public static void remake()
        {
            string code = "";
            string format = "\"{0}\", RColor.FromArgb({1}, {2}, {3}, {4})";
            foreach (var kv in _knownColor)
            {
                var color = kv.Value;
                var name = kv.Key.ToLower();
                code += "{" + System.String.Format(format, name, color.A, color.R, color.G, color.B) + "},\n";
            }
            System.Console.WriteLine(code);
            System.Console.ReadLine();
        }
        */

        public static RColor FromKnownColor(string colorName)
        {
            string color = colorName.ToLower();
            return _knownColor.ContainsKey(color) ? _knownColor[color] : RColor.Empty;
        }

        #region _knownColor Dic Declarations
        private static readonly Dictionary<string, RColor> _knownColor = new Dictionary<string, RColor> {
            {"activeborder", RColor.FromArgb(255, 180, 180, 180)},
            {"activecaption", RColor.FromArgb(255, 153, 180, 209)},
            {"activecaptiontext", RColor.FromArgb(255, 0, 0, 0)},
            {"appworkspace", RColor.FromArgb(255, 171, 171, 171)},
            {"control", RColor.FromArgb(255, 240, 240, 240)},
            {"controldark", RColor.FromArgb(255, 160, 160, 160)},
            {"controldarkdark", RColor.FromArgb(255, 105, 105, 105)},
            {"controllight", RColor.FromArgb(255, 227, 227, 227)},
            {"controllightlight", RColor.FromArgb(255, 255, 255, 255)},
            {"controltext", RColor.FromArgb(255, 0, 0, 0)},
            {"desktop", RColor.FromArgb(255, 0, 0, 0)},
            {"graytext", RColor.FromArgb(255, 109, 109, 109)},
            {"highlight", RColor.FromArgb(255, 0, 120, 215)},
            {"highlighttext", RColor.FromArgb(255, 255, 255, 255)},
            {"hottrack", RColor.FromArgb(255, 0, 102, 204)},
            {"inactiveborder", RColor.FromArgb(255, 244, 247, 252)},
            {"inactivecaption", RColor.FromArgb(255, 191, 205, 219)},
            {"inactivecaptiontext", RColor.FromArgb(255, 0, 0, 0)},
            {"info", RColor.FromArgb(255, 255, 255, 225)},
            {"infotext", RColor.FromArgb(255, 0, 0, 0)},
            {"menu", RColor.FromArgb(255, 240, 240, 240)},
            {"menutext", RColor.FromArgb(255, 0, 0, 0)},
            {"scrollbar", RColor.FromArgb(255, 200, 200, 200)},
            {"window", RColor.FromArgb(255, 255, 255, 255)},
            {"windowframe", RColor.FromArgb(255, 100, 100, 100)},
            {"windowtext", RColor.FromArgb(255, 0, 0, 0)},
            {"transparent", RColor.FromArgb(0, 255, 255, 255)},
            {"aliceblue", RColor.FromArgb(255, 240, 248, 255)},
            {"antiquewhite", RColor.FromArgb(255, 250, 235, 215)},
            {"aqua", RColor.FromArgb(255, 0, 255, 255)},
            {"aquamarine", RColor.FromArgb(255, 127, 255, 212)},
            {"azure", RColor.FromArgb(255, 240, 255, 255)},
            {"beige", RColor.FromArgb(255, 245, 245, 220)},
            {"bisque", RColor.FromArgb(255, 255, 228, 196)},
            {"black", RColor.FromArgb(255, 0, 0, 0)},
            {"blanchedalmond", RColor.FromArgb(255, 255, 235, 205)},
            {"blue", RColor.FromArgb(255, 0, 0, 255)},
            {"blueviolet", RColor.FromArgb(255, 138, 43, 226)},
            {"brown", RColor.FromArgb(255, 165, 42, 42)},
            {"burlywood", RColor.FromArgb(255, 222, 184, 135)},
            {"cadetblue", RColor.FromArgb(255, 95, 158, 160)},
            {"chartreuse", RColor.FromArgb(255, 127, 255, 0)},
            {"chocolate", RColor.FromArgb(255, 210, 105, 30)},
            {"coral", RColor.FromArgb(255, 255, 127, 80)},
            {"cornflowerblue", RColor.FromArgb(255, 100, 149, 237)},
            {"cornsilk", RColor.FromArgb(255, 255, 248, 220)},
            {"crimson", RColor.FromArgb(255, 220, 20, 60)},
            {"cyan", RColor.FromArgb(255, 0, 255, 255)},
            {"darkblue", RColor.FromArgb(255, 0, 0, 139)},
            {"darkcyan", RColor.FromArgb(255, 0, 139, 139)},
            {"darkgoldenrod", RColor.FromArgb(255, 184, 134, 11)},
            {"darkgray", RColor.FromArgb(255, 169, 169, 169)},
            {"darkgreen", RColor.FromArgb(255, 0, 100, 0)},
            {"darkkhaki", RColor.FromArgb(255, 189, 183, 107)},
            {"darkmagenta", RColor.FromArgb(255, 139, 0, 139)},
            {"darkolivegreen", RColor.FromArgb(255, 85, 107, 47)},
            {"darkorange", RColor.FromArgb(255, 255, 140, 0)},
            {"darkorchid", RColor.FromArgb(255, 153, 50, 204)},
            {"darkred", RColor.FromArgb(255, 139, 0, 0)},
            {"darksalmon", RColor.FromArgb(255, 233, 150, 122)},
            {"darkseagreen", RColor.FromArgb(255, 143, 188, 139)},
            {"darkslateblue", RColor.FromArgb(255, 72, 61, 139)},
            {"darkslategray", RColor.FromArgb(255, 47, 79, 79)},
            {"darkturquoise", RColor.FromArgb(255, 0, 206, 209)},
            {"darkviolet", RColor.FromArgb(255, 148, 0, 211)},
            {"deeppink", RColor.FromArgb(255, 255, 20, 147)},
            {"deepskyblue", RColor.FromArgb(255, 0, 191, 255)},
            {"dimgray", RColor.FromArgb(255, 105, 105, 105)},
            {"dodgerblue", RColor.FromArgb(255, 30, 144, 255)},
            {"firebrick", RColor.FromArgb(255, 178, 34, 34)},
            {"floralwhite", RColor.FromArgb(255, 255, 250, 240)},
            {"forestgreen", RColor.FromArgb(255, 34, 139, 34)},
            {"fuchsia", RColor.FromArgb(255, 255, 0, 255)},
            {"gainsboro", RColor.FromArgb(255, 220, 220, 220)},
            {"ghostwhite", RColor.FromArgb(255, 248, 248, 255)},
            {"gold", RColor.FromArgb(255, 255, 215, 0)},
            {"goldenrod", RColor.FromArgb(255, 218, 165, 32)},
            {"gray", RColor.FromArgb(255, 128, 128, 128)},
            {"green", RColor.FromArgb(255, 0, 128, 0)},
            {"greenyellow", RColor.FromArgb(255, 173, 255, 47)},
            {"honeydew", RColor.FromArgb(255, 240, 255, 240)},
            {"hotpink", RColor.FromArgb(255, 255, 105, 180)},
            {"indianred", RColor.FromArgb(255, 205, 92, 92)},
            {"indigo", RColor.FromArgb(255, 75, 0, 130)},
            {"ivory", RColor.FromArgb(255, 255, 255, 240)},
            {"khaki", RColor.FromArgb(255, 240, 230, 140)},
            {"lavender", RColor.FromArgb(255, 230, 230, 250)},
            {"lavenderblush", RColor.FromArgb(255, 255, 240, 245)},
            {"lawngreen", RColor.FromArgb(255, 124, 252, 0)},
            {"lemonchiffon", RColor.FromArgb(255, 255, 250, 205)},
            {"lightblue", RColor.FromArgb(255, 173, 216, 230)},
            {"lightcoral", RColor.FromArgb(255, 240, 128, 128)},
            {"lightcyan", RColor.FromArgb(255, 224, 255, 255)},
            {"lightgoldenrodyellow", RColor.FromArgb(255, 250, 250, 210)},
            {"lightgray", RColor.FromArgb(255, 211, 211, 211)},
            {"lightgreen", RColor.FromArgb(255, 144, 238, 144)},
            {"lightpink", RColor.FromArgb(255, 255, 182, 193)},
            {"lightsalmon", RColor.FromArgb(255, 255, 160, 122)},
            {"lightseagreen", RColor.FromArgb(255, 32, 178, 170)},
            {"lightskyblue", RColor.FromArgb(255, 135, 206, 250)},
            {"lightslategray", RColor.FromArgb(255, 119, 136, 153)},
            {"lightsteelblue", RColor.FromArgb(255, 176, 196, 222)},
            {"lightyellow", RColor.FromArgb(255, 255, 255, 224)},
            {"lime", RColor.FromArgb(255, 0, 255, 0)},
            {"limegreen", RColor.FromArgb(255, 50, 205, 50)},
            {"linen", RColor.FromArgb(255, 250, 240, 230)},
            {"magenta", RColor.FromArgb(255, 255, 0, 255)},
            {"maroon", RColor.FromArgb(255, 128, 0, 0)},
            {"mediumaquamarine", RColor.FromArgb(255, 102, 205, 170)},
            {"mediumblue", RColor.FromArgb(255, 0, 0, 205)},
            {"mediumorchid", RColor.FromArgb(255, 186, 85, 211)},
            {"mediumpurple", RColor.FromArgb(255, 147, 112, 219)},
            {"mediumseagreen", RColor.FromArgb(255, 60, 179, 113)},
            {"mediumslateblue", RColor.FromArgb(255, 123, 104, 238)},
            {"mediumspringgreen", RColor.FromArgb(255, 0, 250, 154)},
            {"mediumturquoise", RColor.FromArgb(255, 72, 209, 204)},
            {"mediumvioletred", RColor.FromArgb(255, 199, 21, 133)},
            {"midnightblue", RColor.FromArgb(255, 25, 25, 112)},
            {"mintcream", RColor.FromArgb(255, 245, 255, 250)},
            {"mistyrose", RColor.FromArgb(255, 255, 228, 225)},
            {"moccasin", RColor.FromArgb(255, 255, 228, 181)},
            {"navajowhite", RColor.FromArgb(255, 255, 222, 173)},
            {"navy", RColor.FromArgb(255, 0, 0, 128)},
            {"oldlace", RColor.FromArgb(255, 253, 245, 230)},
            {"olive", RColor.FromArgb(255, 128, 128, 0)},
            {"olivedrab", RColor.FromArgb(255, 107, 142, 35)},
            {"orange", RColor.FromArgb(255, 255, 165, 0)},
            {"orangered", RColor.FromArgb(255, 255, 69, 0)},
            {"orchid", RColor.FromArgb(255, 218, 112, 214)},
            {"palegoldenrod", RColor.FromArgb(255, 238, 232, 170)},
            {"palegreen", RColor.FromArgb(255, 152, 251, 152)},
            {"paleturquoise", RColor.FromArgb(255, 175, 238, 238)},
            {"palevioletred", RColor.FromArgb(255, 219, 112, 147)},
            {"papayawhip", RColor.FromArgb(255, 255, 239, 213)},
            {"peachpuff", RColor.FromArgb(255, 255, 218, 185)},
            {"peru", RColor.FromArgb(255, 205, 133, 63)},
            {"pink", RColor.FromArgb(255, 255, 192, 203)},
            {"plum", RColor.FromArgb(255, 221, 160, 221)},
            {"powderblue", RColor.FromArgb(255, 176, 224, 230)},
            {"purple", RColor.FromArgb(255, 128, 0, 128)},
            {"red", RColor.FromArgb(255, 255, 0, 0)},
            {"rosybrown", RColor.FromArgb(255, 188, 143, 143)},
            {"royalblue", RColor.FromArgb(255, 65, 105, 225)},
            {"saddlebrown", RColor.FromArgb(255, 139, 69, 19)},
            {"salmon", RColor.FromArgb(255, 250, 128, 114)},
            {"sandybrown", RColor.FromArgb(255, 244, 164, 96)},
            {"seagreen", RColor.FromArgb(255, 46, 139, 87)},
            {"seashell", RColor.FromArgb(255, 255, 245, 238)},
            {"sienna", RColor.FromArgb(255, 160, 82, 45)},
            {"silver", RColor.FromArgb(255, 192, 192, 192)},
            {"skyblue", RColor.FromArgb(255, 135, 206, 235)},
            {"slateblue", RColor.FromArgb(255, 106, 90, 205)},
            {"slategray", RColor.FromArgb(255, 112, 128, 144)},
            {"snow", RColor.FromArgb(255, 255, 250, 250)},
            {"springgreen", RColor.FromArgb(255, 0, 255, 127)},
            {"steelblue", RColor.FromArgb(255, 70, 130, 180)},
            {"tan", RColor.FromArgb(255, 210, 180, 140)},
            {"teal", RColor.FromArgb(255, 0, 128, 128)},
            {"thistle", RColor.FromArgb(255, 216, 191, 216)},
            {"tomato", RColor.FromArgb(255, 255, 99, 71)},
            {"turquoise", RColor.FromArgb(255, 64, 224, 208)},
            {"violet", RColor.FromArgb(255, 238, 130, 238)},
            {"wheat", RColor.FromArgb(255, 245, 222, 179)},
            {"white", RColor.FromArgb(255, 255, 255, 255)},
            {"whitesmoke", RColor.FromArgb(255, 245, 245, 245)},
            {"yellow", RColor.FromArgb(255, 255, 255, 0)},
            {"yellowgreen", RColor.FromArgb(255, 154, 205, 50)},
            {"buttonface", RColor.FromArgb(255, 240, 240, 240)},
            {"buttonhighlight", RColor.FromArgb(255, 255, 255, 255)},
            {"buttonshadow", RColor.FromArgb(255, 160, 160, 160)},
            {"gradientactivecaption", RColor.FromArgb(255, 185, 209, 234)},
            {"gradientinactivecaption", RColor.FromArgb(255, 215, 228, 242)},
            {"menubar", RColor.FromArgb(255, 240, 240, 240)},
            {"menuhighlight", RColor.FromArgb(255, 51, 153, 255)},

        };
        #endregion

    }
}

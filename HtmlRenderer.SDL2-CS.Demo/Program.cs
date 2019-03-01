using System;
using SDL2;
using HtmlRenderer.SDL2_CS.Utils;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using System.Runtime.InteropServices;

using System.Collections.Generic;
//using System.Drawing; // for GenerateColorCode
namespace HtmlRenderer.SDL2_CS.Demo
{
    class Program
    {
        /*
        static string GenerateColorCode()
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
        private static void InitSDL2()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) < 0)
                Console.WriteLine("SDL could not initialize! SDL_Error: {0}", SDL.SDL_GetError());
            else
                Console.WriteLine("SDL initialized!");

            // OPTIONAL: init SDL_image
            var imgFlags = SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_WEBP;
            if ((SDL_image.IMG_Init(imgFlags) > 0 & imgFlags > 0) == false)
                Console.WriteLine("SDL_image could not initialize! SDL_image Error: {0}", SDL.SDL_GetError());
            else
                Console.WriteLine("SDL_image initialized!");

            // OPTIONAL: init SDL_ttf

            if (SDL_ttf.TTF_Init() == -1)
                Console.WriteLine("SDL_ttf could not initialize! SDL_image Error: {0}", SDL.SDL_GetError());
            else
                Console.WriteLine("SDL_ttf initialized!");

            // OPTIONAL: init SDL_mixer
            if (SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048) < 0)
                Console.WriteLine("SDL_mixer could not initialize! SDL_image Error: {0}", SDL.SDL_GetError());
            else
                Console.WriteLine("SDL_mixer initialized!");

            if (SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "1") == SDL.SDL_bool.SDL_FALSE)
                Console.WriteLine("Warning: Linear texture filtering not enabled!");

        }



        private static void QuitSDL2()
        {
            SDL_ttf.TTF_Quit();
            SDL_image.IMG_Quit();
            SDL_mixer.Mix_Quit();
            SDL.SDL_Quit();
        }

        private static void TestFM(IntPtr renderer, string font_familyname, double font_size_em)
        {

            var textColor = new SDL.SDL_Color { a = 255, r = 0, g = 0, b = 100 };

            SDL.SDL_GetRendererOutputSize(renderer, out int width, out int height);

            Console.WriteLine("Renderer width={0}, height={1}", width, height);

            FontManager fm = FontManager.Instance;
            int y = 0;

            string[] real_style_name_short = { "R--", "-B-", "--I", "-BI" };

            string[] real_style_name = { "Regular|Обычный", "Bold|Жирный", "Italic|Наклонный", "Bold+Italic|Наклонный+Жирный" };
            for (int style_id = 0; style_id <= 15; style_id++)
            {
                //if (!(style_id == 2 || style_id == 6)) continue;
                int real_style_id = style_id & 3;
                string rs_short = real_style_name_short[real_style_id];

                string text_addon = "";

                if ((style_id & (int)RFontStyle.Underline) > 0)
                {
                    text_addon += "  Underline|Подчеркнутый";
                    rs_short += "U";
                }
                else
                {
                    rs_short += "-";
                }

                if ((style_id & (int)RFontStyle.Strikeout) > 0)
                {
                    text_addon += "  Strikeout|Перечеркнутый";
                    rs_short += "S";
                }
                else
                {
                    rs_short += "-";
                }

                string text_line = "[" + rs_short + "] " + style_id.ToString() + ": " + real_style_name[real_style_id] + text_addon;
                Console.WriteLine("Rendering {0}: \"" + text_line + "\"", y);
                //text_line = "TESTываыва";
                IntPtr font = fm.GetTTF_Font(font_familyname, font_size_em, (RFontStyle)style_id);
                font.ShowSDLError("GetTTF_Font return bad font");

                var textSurface = SDL_ttf.TTF_RenderUTF8_Blended(font, text_line, textColor);
                textSurface.ShowSDLError("Unable to render text surface!");

                var texture_text = SDL.SDL_CreateTextureFromSurface(renderer, textSurface);
                texture_text.ShowSDLError("Unable to create text texture!");


                var dst_rect = textSurface.As<SDL.SDL_Surface>().ToSDL_Rect();
                dst_rect.y = y;

                //SDL.SDL_RenderCopyEx(renderer, texture_text, IntPtr.Zero, ref dst_rect, 0, IntPtr.Zero, SDL.SDL_RendererFlip.SDL_FLIP_NONE);

                SDL.SDL_RenderCopy(renderer, texture_text, IntPtr.Zero, ref dst_rect);


                y += dst_rect.h;

                SDL.SDL_DestroyTexture(texture_text);
                SDL.SDL_FreeSurface(textSurface);

            }



        }

        static int RenderText(IntPtr renderer, IntPtr font, string text_line, int y, int style)
        {
            var textColor = new SDL.SDL_Color { a = 0, r = 100, g = 0, b = 0 };

            SDL_ttf.TTF_SetFontStyle(font, style);
            var textSurface = SDL_ttf.TTF_RenderUTF8_Blended(font, text_line, textColor);
            textSurface.ShowSDLError("Unable to render text surface!");

            var texture_text = SDL.SDL_CreateTextureFromSurface(renderer, textSurface);

            var dst_rect = textSurface.As<SDL.SDL_Surface>().ToSDL_Rect();
            dst_rect.y = y;
            int h = dst_rect.h;
            SDL.SDL_RenderCopy(renderer, texture_text, IntPtr.Zero, ref dst_rect);

            SDL.SDL_DestroyTexture(texture_text);
            SDL.SDL_FreeSurface(textSurface);
            return h;
        }
        static void PT_Serif_Italic_Bug(IntPtr renderer)
        {
            int ptsize = (int)Math.Round(1.5f * 11f);
            string filename = @"fonts/PT Serif_Italic.ttf";
            byte[] bytes = System.IO.File.ReadAllBytes(filename);
            int size = Marshal.SizeOf(bytes[0]) * bytes.Length;
            IntPtr mem = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, mem, size);
            IntPtr RWops = SDL.SDL_RWFromMem(mem, size);
            RWops.ShowSDLError("SDL_RWFromMem():");

            int y = 0;
            IntPtr font_2 = SDL_ttf.TTF_OpenFontIndexRW(RWops, 0, ptsize, 0);
            //IntPtr font_2 = SDL_ttf.TTF_OpenFontIndex(filename, ptsize, 0);
            font_2.ShowSDLError("Failed to load font!");

            y += RenderText(renderer, font_2, "й", y, 2);

            IntPtr font_6 = SDL_ttf.TTF_OpenFontIndexRW(RWops, 0, ptsize, 0);
            //IntPtr font_6 = SDL_ttf.TTF_OpenFontIndex(filename, ptsize, 0);
            font_6.ShowSDLError("Failed to load font!");

            y += RenderText(renderer, font_6, "Hello", y, 6);

        }


        static void Main(string[] args)
        {

            SDL2.SDL2_CS_libs_bundle.Init();
            InitSDL2();

            var window = SDL.SDL_CreateWindow("HtmlRenderer.SDL2-CS.Demo", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED,
                                            640, 480, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            if (!window.ShowSDLError("Window could not be created!"))
                Console.WriteLine("Window created!");

            var renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            renderer.ShowSDLError("Renderer could not be created!");

            SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);
            SDL.SDL_RenderClear(renderer);


            FontManager fm = FontManager.Instance;
            fm.UseRWops = true;
            fm.CreateRWopsCacheOnFontRegister = true;
            fm.UseFontCache = true;
            fm.RegisterFontsDir("fonts");
            fm.SetDefaultsFontFamily(serif: "PT Serif", sans_serif: "PT Sans", monospace: "PT Mono");

            //fm.RegisterFontsDir(@"C:\Windows\Fonts\");
            //fm.SetDefaultsFontFamily(serif: "Segoe UI", sans_serif: "Arial", monospace: "Lucida Console");
            //fm.SetDefaultsFontFamily(serif: "ms serif", sans_serif: "ms san serif", monospace: "Lucida Console");

            var watch = System.Diagnostics.Stopwatch.StartNew();

            TestFM(renderer, "", 1.5f);
            //TestFM(renderer, "", 1.5f);
            //TestFM(renderer, "", 1.5f);
            //TestFM(renderer, "", 1.5f);
            //TestFM(renderer, "", 0.5f);
            //TestFM(renderer, "", 1f);
            //TestFM(renderer, "", 2f);

            //PT_Serif_Italic_Bug(renderer);

            watch.Stop();
            Console.WriteLine("Render time:{0}", watch.ElapsedMilliseconds);

            SDL.SDL_RenderPresent(renderer);

            bool exit = false;
            while (!exit)
            {
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
                {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT)
                        exit = true;
                }

                SDL.SDL_Delay(50);
            }
            fm.Dispose();
            QuitSDL2();


        }
    }
}

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
            SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);
            SDL.SDL_RenderClear(renderer);
            var textColor = new SDL.SDL_Color();

            FontManager fm = FontManager.Instance;
            int y = 0;

            string[] real_style_name_short = { "R--", "-B-", "--I", "-BI" };

            string[] real_style_name = { "Regular|Обычный", "Bold|Жирный", "Italic|Наклонный", "Bold+Italic|Наклонный+Жирный" };
            for (int style_id = 0; style_id < 16; style_id++)
            {

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
                Console.WriteLine("Rendering " + text_line + "");

                IntPtr font = fm.GetTTF_Font(font_familyname, font_size_em, (RFontStyle)style_id);
                if (font == IntPtr.Zero)
                    Console.WriteLine("GetTTF_Font return bad font");

                var textSurface = SDL_ttf.TTF_RenderUTF8_Blended(font, text_line, textColor);
                if (textSurface == IntPtr.Zero)
                    Console.WriteLine("Unable to render text surface! SDL_ttf Error: {0}", SDL.SDL_GetError());

                var texture_text = SDL.SDL_CreateTextureFromSurface(renderer, textSurface);
                var s_text = Marshal.PtrToStructure<SDL.SDL_Surface>(textSurface);
                var dst_rect = new SDL.SDL_Rect { x = 0, y = y, w = s_text.w, h = s_text.h };
                SDL.SDL_RenderCopyEx(renderer, texture_text, IntPtr.Zero, ref dst_rect, 0, IntPtr.Zero, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
                y += s_text.h;

                SDL.SDL_DestroyTexture(texture_text);
                SDL.SDL_FreeSurface(textSurface);

            }
            SDL.SDL_RenderPresent(renderer);

        }




        static void Main(string[] args)
        {

            SDL2.SDL2_CS_libs_bundle.Init();
            InitSDL2();

            var window = SDL.SDL_CreateWindow("HtmlRenderer.SDL2-CS.Demo", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED,
                                            640, 480, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            if (window == IntPtr.Zero)
                Console.WriteLine("Window could not be created! SDL_Error: {0}", SDL.SDL_GetError());
            else
                Console.WriteLine("Window created!");
            var renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (renderer == IntPtr.Zero)
                Console.WriteLine("Renderer could not be created! SDL Error: {0}", SDL.SDL_GetError());




            bool exit = false;

            FontManager fm = FontManager.Instance;
            fm.RegisterFontsFromDir("fonts");
            fm.SetDefaultsFontFamily(serif: "PT Serif", sans_serif: "PT Sans", monospace: "PT Mono");
            TestFM(renderer, "", 1.5);



            while (!exit)
            {
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
                {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT)
                        exit = true;
                }

                SDL.SDL_Delay(50);
            }
            fm.ClearFontCache();
            fm.Quit();
            QuitSDL2();


        }
    }
}

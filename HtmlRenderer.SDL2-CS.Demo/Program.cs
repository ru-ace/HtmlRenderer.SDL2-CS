using System;
using System.Collections.Generic;
//using System.Drawing; // for GenerateColorCode
using SDL2;
using HtmlRenderer.SDL2_CS.Utils;

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
            bool exit = false;

            FontManager fm = FontManager.Instance;
            fm.RegisterFontsFromDir("fonts");
            fm.RegisterFontsFromDir("fonts");

            while (!exit)
            {
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
                {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT)
                        exit = true;
                }

                SDL.SDL_Delay(50);
            }

            fm.Clear();
            QuitSDL2();


        }
    }
}

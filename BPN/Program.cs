﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using Gwen;
using Gwen.Controls;
using Gwen.Skin;
using System.Diagnostics;
using System.Timers;
using System.Threading;

//Book Publishing Nation v0.1
namespace BPN
{
    class Program
    {
        private static RenderWindow window;
        private static Vector2i screenSize = new Vector2i(600, 400);
        public static Vector2i ScreenSize { get { return screenSize; } }

        private static Canvas _canvas;
        public static Canvas _Canvas { get { return _canvas; } }
        private static Gwen.Renderer.SFML guiRenderer;
        private static Gwen.Skin.TexturedBase skin;
        private static Gwen.Font guiFont;
        public static Gwen.Font GuiFont { get { return guiFont; } }
        private static Gwen.Font guiFontLarge;
        public static Gwen.Font GuiFontLarge { get { return guiFontLarge; } }
        private static Gwen.Input.SFML input;
        private static TabControl tab;
        public static TabControl Tab { get { return tab; } }
        private static Canvas bookPage;
        public static Canvas BookPage { get { return bookPage; } }
        private static Canvas aboutPage;
        public static Canvas AboutPage { get { return aboutPage; } }
        private static TabButton bookTabButton;
        public static TabButton BookTabButton { get { return bookTabButton; } }
        private static TabButton aboutTabButton;
        public static TabButton AboutTabButton { get { return aboutTabButton; } }



        private static int thisFrameFps =0, lastFrameFps=0;

        static void Main(string[] args)
        {
            Initialize();
            GameLoop();
        }

        private static void GameLoop()
        {
            while (window.IsOpened())
            {
                window.DispatchEvents();
                Update();
                Draw();
                
            }

            window_Closed(null, new EventArgs());
        }

        private static void Update()
        {
            #region Memory & FPS
            float memory = (float)Math.Round(GC.GetTotalMemory(true) / 1024F / 1024F, 2);
            thisFrameFps = (int)window.GetFrameTime();
            //thisFrameFps = (int)(1 / window.GetFrameTime());
            window.SetTitle("BPN | Memory: " + memory + "MB | FPS: " + Math.Round(thisFrameFps*0.7+lastFrameFps*0.3));
            lastFrameFps = thisFrameFps;
            #endregion

            Book.Update();

            GameManager.Update();

        }

        private static void Draw()
        {
            //Defaults to black color
            window.Clear();
            window.SaveGLStates();
            #region render gui
            guiRenderer.Begin();
            guiRenderer.End();
            #endregion
            _canvas.RenderCanvas();
            window.RestoreGLStates();
            window.Display();
        }

        private static void Initialize()
        {
            #region setup debug
            TextWriterTraceListener debugger = new TextWriterTraceListener(Console.Out);
            Debug.Listeners.Add(debugger);
            #endregion

            Debug.WriteLine("Initializing Game");

            #region window setup
            window = new RenderWindow(new VideoMode((uint)screenSize.X, (uint)screenSize.Y, 32), "BPN", Styles.Close);
            window.SetFramerateLimit(30); 
            window.KeyReleased += new EventHandler<KeyEventArgs>(window_KeyReleased);
            window.Closed += new EventHandler(window_Closed);
            window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(window_MouseButtonReleased);
            window.MouseMoved += new EventHandler<MouseMoveEventArgs>(window_MouseMoved);
            window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(window_MouseButtonPressed);
            window.EnableVerticalSync(true);
            #endregion

            #region Gui Setup
            guiRenderer = new Gwen.Renderer.SFML(window);
            guiRenderer.Initialize();
            skin = new TexturedBase(guiRenderer, @"Data/DefaultSkin.png");
            guiFont = new Gwen.Font("Data/OpenSans.ttf", 10);
            guiFontLarge = new Gwen.Font("Data/OpenSans.ttf", 14);
            guiRenderer.LoadFont(guiFont);
            guiRenderer.LoadFont(guiFontLarge);
            skin.SetDefaultFont(guiFont.FaceName);

            _canvas = new Canvas(skin);
            _canvas.SetSize(screenSize.X, screenSize.Y);
            _canvas.ShouldDrawBackground = true;
            _canvas.BackgroundColor = System.Drawing.Color.FromArgb(255, 150, 170, 170);
            _canvas.KeyboardInputEnabled = true;

            #region book page
            bookPage = new Canvas(skin);
            bookPage.SetBounds(4, 4, screenSize.X - 8, screenSize.Y - 8);
            bookPage.MouseInputEnabled = true;
            
            bookTabButton = new TabButton(_canvas);
            bookTabButton.SetBounds(4, 4, bookTabButton.Parent.Width-4,bookTabButton.Parent.Height-4);
            bookTabButton.Text = "Books";
            bookTabButton.Page = bookPage;
            bookTabButton.Font = guiFontLarge;
            bookTabButton.MouseInputEnabled = true;
            bookTabButton.IsTabable = false;
            
            #endregion

            #region about page
            aboutPage = new Canvas(skin);
            aboutPage.SetBounds(4, 4, screenSize.X-8, screenSize.Y-8);
            aboutTabButton = new TabButton(_canvas);
            aboutTabButton.SetBounds(4, 4, aboutTabButton.Parent.Width - 4, aboutTabButton.Parent.Height - 4);
            aboutTabButton.Text = "About BPN";
            aboutTabButton.Page = aboutPage;
            aboutTabButton.Font = guiFontLarge;
            #endregion

            tab = new TabControl(_canvas);
            tab.SetBounds(0, 24, screenSize.X, screenSize.Y-54);
            tab.AddPage(bookTabButton);
            tab.AddPage(aboutTabButton);
            tab.MouseInputEnabled = true;

            input = new Gwen.Input.SFML();
            input.Initialize(_canvas);
            #endregion

            #region Name Generator Initialization
            BookNameGenerator.Initialize();
            PersonNameGenerator.Initialize();
            //Have a few books ready for review at beginning of the game
            for (int i = 0; i < 3; i++)
            { Book b = new Book(); }
            #endregion

            GameManager.Initialize();

            guiRenderer.Initialize();

            Book.Initialize();

            Debug.WriteLine("Initialization Complete");
        }

        public static void Exit()
        {
            window_Closed(null, null);
        }

        static void window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            input.ProcessMessage(e);
        }

        static void window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            input.ProcessMessage(e);
        }

        static void window_Closed(object sender, EventArgs e)
        {
            /*
                _canvas.Dispose();
                skin.Dispose();
                guiRenderer.Dispose();
                window.Close();
                //window.Dispose();
                Debug.WriteLine("Exiting");

                Environment.Exit(0);
            */
            try
            {
                Debug.WriteLine("Exiting");
                window.Close();
            }
            catch (Exception f)
            { Console.WriteLine(f); }
        }

        static void window_KeyReleased(object sender, KeyEventArgs e)
        {
            
            switch (e.Code)
            {
                case Keyboard.Key.Escape: { Environment.Exit(0); break; }
                case Keyboard.Key.Space: 
                    { 
                        if (GameManager.GameSpeed == GameManager.GameSpeeds.Paused)
                        {
                            GameManager.GameSpeed = GameManager.GameSpeeds.Normal;
                            GameManager.PausedText.Hide();
                        }
                        else
                        {
                            GameManager.GameSpeed = GameManager.GameSpeeds.Paused;
                            GameManager.PausedText.Show();
                        }

                        break;
                    }
            }

            input.ProcessMessage(e);
            
        }

        static void window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            input.ProcessMessage(e);
        }
        
        }
}

using System;
using System.Collections.Generic;
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
using SFML.System;

//Book Publishing Nation v0.1
namespace BPN
{
    class Program
    {
        private static RenderWindow window;
        private static Vector2i screenSize = new Vector2i(512, 400);
        public static Vector2i ScreenSize { get { return screenSize; } }
        private static Random randomizer = new Random();
        public static Random Randomizer { get { return randomizer; } }

        private static Canvas _canvas;
        public static Canvas _Canvas { get { return _canvas; } }
        private static Gwen.Renderer.SFML guiRenderer;
        private static Gwen.Skin.TexturedBase skin;
        private static Gwen.Font guiFont;
        public static Gwen.Font GuiFont { get { return guiFont; } }
        private static Gwen.Font guiFontLarge;
        public static Gwen.Font GuiFontLarge { get { return guiFontLarge; } }
        private static Gwen.Input.SFML input;
        private static Button bookPageButton;
        public static Button BookPageButton { get { return bookPageButton; } }
        private static Button aboutPageButton;
        public static Button AboutPageButton { get { return aboutPageButton; } }

        private static Button activePageButton;
        public static Button ActivePageButton { get { return activePageButton; } }

        private static System.Drawing.Rectangle smallPageBounds;
        
        static void Main(string[] args)
        {
            Initialize();
            GameLoop();
        }

        private static void GameLoop()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                Update();
                Draw();
            }

            window_Closed(null, new EventArgs());
        }

        private static void Update()
        {
            Book.Update();

            GameManager.Update();
        }

        private static void Draw()
        {
            //Defaults to black color
            window.Clear();
            window.PushGLStates();
            #region render gui
            guiRenderer.Begin();
            guiRenderer.End();
            #endregion
            _canvas.RenderCanvas();
            window.PopGLStates();
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
            window.MouseWheelScrolled += Window_MouseWheelScrolled;
            window.SetVerticalSyncEnabled(true);
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

            input = new Gwen.Input.SFML();
            input.Initialize(_canvas);
            #endregion

            #region Side Buttons
            aboutPageButton = new Button(_canvas);
            aboutPageButton.Text = "About";
            aboutPageButton.SetPos(4, 2*screenSize.Y/3-8);
            aboutPageButton.OnDown += new Gwen.Controls.Base.ControlCallback(aboutPageButton_OnDown);

            bookPageButton = new Button(_canvas);
            bookPageButton.Text = "Books";
            bookPageButton.SetPos(4, 32);
            bookPageButton.OnDown += new Gwen.Controls.Base.ControlCallback(bookPageButton_OnDown);

            activePageButton = bookPageButton;
            #endregion

            int leftBound = 80;
            smallPageBounds = new System.Drawing.Rectangle(leftBound,leftBound,screenSize.X-leftBound*2,screenSize.Y -leftBound*2);

            #region Name Generator Initialization
            BookNameGenerator.Initialize();
            PersonNameGenerator.Initialize();
            //Have a few books ready for review at beginning of the game
            for (int i = 0; i < 8; i++)
            { Book b = new Book(); }
            #endregion

            GameManager.Initialize();

            guiRenderer.Initialize();

            Notifications.Initialize();

            Book.Initialize();

            Debug.WriteLine("Initialization Complete");
        }

        private static void Window_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            input.ProcessMessage(e);
        }

        static void bookPageButton_OnDown(Gwen.Controls.Base control)
        {
            GameManager.ChangePage(GameManager.Page.Books);
        }

        static void aboutPageButton_OnDown(Gwen.Controls.Base control)
        {
            GameManager.ChangePage(GameManager.Page.About);
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

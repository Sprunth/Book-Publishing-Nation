using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using Gwen.ControlsInternal;

namespace BPN
{
    using P = Program;
    class GameManager
    {
        private static DateTime gameTime;
        public static String GameTimeString { get { return "Year " + gameTime.Year + " Month " + gameTime.Month + " Day " + gameTime.Day; } }
        public static void NextDay() { gameTime = gameTime.AddDays(1); }

        private static int frameCounter = 0;
        private static int counterMax = 60;
        public enum GameSpeeds {Paused,Normal,Fast};
        private static GameSpeeds gameSpeed = GameSpeeds.Normal;
        public static GameSpeeds GameSpeed 
        { 
            get {return gameSpeed;}
            set 
            {
                gameSpeed = value;
                switch (gameSpeed)
                {
                    case GameSpeeds.Paused: { break; }
                    case GameSpeeds.Normal: { counterMax = 60; break; }
                    case GameSpeeds.Fast: { counterMax = 30; break; }
                }
            }
        }

        private static Gwen.ControlsInternal.Text gameTimeText;
        public static Gwen.ControlsInternal.Text GameTimeText { get { return gameTimeText; } }
        private static Gwen.ControlsInternal.Text pausedText;
        public static Gwen.ControlsInternal.Text PausedText { get { return pausedText; } }

        private static Gwen.ControlsInternal.Text aboutText;
        public static Gwen.ControlsInternal.Text AboutText { get { return aboutText; } }

        private static Gwen.Controls.MenuStrip menuStrip;
        public static Gwen.Controls.MenuStrip MenuStrip { get { return menuStrip; } }
        public static List<Gwen.Controls.MenuItem> menuStripItems = new List<Gwen.Controls.MenuItem>();
        private static Gwen.Controls.MenuItem exitButton;

        public static void Initialize()
        {
            menuStrip = new Gwen.Controls.MenuStrip(P._Canvas);
            menuStripItems.Add(menuStrip.AddItem("File"));
            exitButton = menuStripItems[0].Menu.AddItem("Exit");
            exitButton.OnDown += new Gwen.Controls.Base.ControlCallback(exitButton_OnDown);

            gameTime = new DateTime(2000, 1, 1);

            gameTimeText = new Gwen.ControlsInternal.Text(P._Canvas);
            gameTimeText.String = GameTimeString;
            gameTimeText.SetPos(10, P.ScreenSize.Y - 20);
            gameTimeText.TextColor = System.Drawing.Color.Black;

            pausedText = new Gwen.ControlsInternal.Text(P._Canvas);
            pausedText.String = "PAUSED";
            pausedText.Font = P.GuiFontLarge;
            pausedText.SetPos(P.ScreenSize.X - pausedText.Length * P.GuiFont.Size, P.ScreenSize.Y - 20);
            pausedText.Hide();

            aboutText = new Gwen.ControlsInternal.Text(P.AboutPage);
            aboutText.String = "Book Publishing Nation was created by Dylan Wang";
            aboutText.SetPos(aboutText.Parent.Width / 2, aboutText.Parent.Height / 2);
        }

        static void exitButton_OnDown(Gwen.Controls.Base control)
        {
            Environment.Exit(0);
        }


        public static void Update()
        {
            if (gameSpeed != GameSpeeds.Paused)
            {
                frameCounter++;
                if (frameCounter > counterMax)
                { 
                    NextDay();
                    gameTimeText.String = GameTimeString;
                    frameCounter = 0;
                }
            }
        }

    }
}

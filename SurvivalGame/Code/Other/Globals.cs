using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    enum TextureName
    {
        None = 0,
        Rectangle,
        Circle,
        PistolItem,
        SwordItem,
        RPG,
        GrassyBackground,
        Map,
        Sparkles,
        PineTree,
        Axe,
        AxeItem,
        PineTreeOnFire,
        fire
    }
    enum SpriteFontName
    {
        None,
        Aerial16
    }
    enum MouseKey
    {
        LeftButton,
        RightButton,
        MiddleButton,
        XButton1,
        XButton2
    }
    class Globals
    {
        public static Random rand = new Random();

        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static Dictionary<SpriteFontName, SpriteFont> SpriteFonts = new Dictionary<SpriteFontName, SpriteFont>();

        public static GraphicsDeviceManager graphics { get; set; }
        public static SpriteBatch spriteBatch { get; set; }
        //public static GraphicsDevice GraphicsDevice { get; set; }

        public static List<Drawing> Drawings = new List<Drawing>();
        public static List<DrawingText> DrawingTexts = new List<DrawingText>();
        public static List<IUpdate> Updatables = new List<IUpdate>();

        public static bool IsUserWriting = false;

        public static List<Keys> PressedKeyboardKeys = new List<Keys>();
        public static List<Keys> NewKeyboardKeys = new List<Keys>();

        public static List<MouseKey> PressedMouseKeys = new List<MouseKey>();
        public static List<MouseKey> NewMouseKeys = new List<MouseKey>();

        public static Command Command;
        public static HUD HUD;
        public static MouseCursor MouseCursor;
        public static Shop shop;
        public static MainMenu MainMenu;
        public static Map Map;

        public static Dictionary<(int x, int y), Room> Rooms = new Dictionary<(int x, int y), Room>();
        public static (int x, int y) activeRoomCoords { get; set; } = (0,0);

    }
}

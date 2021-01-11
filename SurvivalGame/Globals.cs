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
        Circle
    }
    enum SpriteFontName
    {
        None,
        Chat
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
        public static Dictionary<TextureName, Texture2D> Textures = new Dictionary<TextureName, Texture2D>();
        public static Dictionary<SpriteFontName, SpriteFont> SpriteFonts = new Dictionary<SpriteFontName, SpriteFont>();

        public static bool IsTextBoxActive = false;

        public static List<Keys> PressedKeyboardKeys = new List<Keys>();
        public static List<Keys> NewKeyboardKeys = new List<Keys>();

        public static List<MouseKey> PressedMouseKeys = new List<MouseKey>();
        public static List<MouseKey> NewMouseKeys = new List<MouseKey>();
    }
}

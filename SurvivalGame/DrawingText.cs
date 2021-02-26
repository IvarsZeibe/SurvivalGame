using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class DrawingText
    {
        public DrawingText()
        {
            Globals.DrawingTexts.Add(this);
        }
        public DrawingText(SpriteFontName spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 scale, float layerDepth = 0.5f, bool isDrawn = false)
        {
            this.SpriteFont = spriteFont;
            this.Text = text;
            this.Position = position;
            this.Color = color;
            this.Rotation = rotation;
            this.Scale = scale;
            this.LayerDepth = layerDepth;
            this.IsDrawn = isDrawn;
            Globals.DrawingTexts.Add(this);
        }
        public SpriteFontName SpriteFont { get; set; } = SpriteFontName.None;
        public StringBuilder Text { get; set; } = new StringBuilder();
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        public float Rotation { get; set; } = 0f;
        public Vector2 Scale { get; } = new Vector2(1, 1);
        //private Vector2 scale = new Vector2(1, 1);
        //public Vector2 Scale
        //{
        //    get => scale;
        //    set
        //    {
        //        float textureWidth = Globals.SpriteFonts[SpriteFont].MeasureString(Text).X / Text.Length;
        //        float textureHeight = Globals.SpriteFonts[SpriteFont].MeasureString(Text).Y;
        //        scale = new Vector2(value.X / textureWidth, value.Y / textureHeight);
        //    }
        //}
        public float LayerDepth { get; set; } = 0.2f;
        public bool IsDrawn { get; set; } = false;
        public bool IsDead { get; set; } = false;
    }
}

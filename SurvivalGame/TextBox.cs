using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class TextBox : IDrawingText
    {
        public SpriteFontName SpriteFont { get; set; }
        public StringBuilder Text { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        private Vector2 scale = new Vector2(1,1);
        public Vector2 Scale
        {
            get => scale;
            set
            {
                scale = new Vector2(value.X / (Globals.SpriteFonts[SpriteFont].MeasureString(Text).X / Text.Length), value.Y / Globals.SpriteFonts[SpriteFont].MeasureString(Text).Y);
            }
        }
        public float LayerDepth { get; set; }

        public bool IsActive { get; set; }
        private bool isFirstTime = true;
        private float timeSinceLastCharacter = 0f;
        private readonly float HeldCharacterCooldown = 0.5f;
        private Keys LastCharacter;
        private Index index = ^0;

        private TextPointer Pointer;
        public TextBox()
        {
            EntityTracker.DrawingsText.Add(this);
        }
        public void Input(GameTime gameTime)
        {
            if (Globals.NewKeyboardKeys.Count > 0)
            {
                LastCharacter = Globals.NewKeyboardKeys[Globals.NewKeyboardKeys.Count - 1];
                timeSinceLastCharacter = 0f;
            }
            if (Globals.NewKeyboardKeys.Count > 0 || 
                (timeSinceLastCharacter > HeldCharacterCooldown && Globals.PressedKeyboardKeys.Contains(LastCharacter)))
            {
                switch (LastCharacter)
                {
                    case Keys.Enter:
                        if (!isFirstTime)
                        {
                            IsActive = false;
                            Pointer.IsActive = false;
                        }
                        break;
                    case Keys.Back:
                        if (index.GetOffset(Text.Length) > 0)
                        {
                            Text.Remove(index.GetOffset(Text.Length) - 1, 1);
                        }
                        break;
                    case Keys.Delete:
                        if (index.Value > 0)
                        {
                            Text.Remove(index.GetOffset(Text.Length), 1);
                            index = ^(index.Value - 1);
                        }
                        break;
                    case Keys.Left:
                        if (index.Value < Text.Length)
                            index = ^(index.Value + 1);
                        break;
                    case Keys.Right:
                        if (index.Value > 0)
                            index = ^(index.Value - 1);
                        break;
                    case Keys n when (int)n >= 65 && (int)n <= 90:
                        if (!Globals.PressedKeyboardKeys.Contains(Keys.RightShift))
                            Text.Insert(index.GetOffset(Text.Length), n.ToString().ToLower());
                        else
                            Text.Insert(index.GetOffset(Text.Length), n.ToString());
                        break;
                    case Keys n when (int)n >= 48 && (int)n <= 57:
                        Text.Insert(index.GetOffset(Text.Length), n.ToString()[1]);
                        break;
                }

            }
            timeSinceLastCharacter += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (isFirstTime)
            {
                Pointer = new TextPointer()
                {
                    SpriteFont = this.SpriteFont,
                    ParentPosition = this.Position,
                    Color = this.Color,
                    Rotation = this.Rotation,
                    Scale = this.Scale,
                    LayerDepth = this.LayerDepth
                };
                isFirstTime = false;
            }
            else
                Pointer.Update(gameTime, index.GetOffset(Text.Length), Text);
        }
    }
    class TextPointer : IDrawingText
    {
        public SpriteFontName SpriteFont { get; set; }
        public StringBuilder Text { get; set; }
        public Vector2 ParentPosition { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = new Vector2(1, 1);
        public float LayerDepth { get; set; }
        float TimeSinceBlink { get; set; } = 0f;
        private readonly float BlinkCooldown = 0.5f;
        bool visiable = true;
        public bool IsActive { get; set; } = true;
        public void Update(GameTime gameTime, Index index, StringBuilder text)
        {
            TimeSinceBlink += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Text = new StringBuilder("|");
            string txt = text.ToString();
            Position = new Vector2(ParentPosition.X + Globals.SpriteFonts[SpriteFont].MeasureString(txt[0..index]).X - 3, ParentPosition.Y - 2);

            if (TimeSinceBlink > BlinkCooldown)
            {
                if (visiable)
                {
                    EntityTracker.DrawingsText.Remove(this);
                    visiable = false;
                }
                else
                {
                    EntityTracker.DrawingsText.Add(this);
                    visiable = true;
                }
                TimeSinceBlink = 0f;
            }
        }
    }
}

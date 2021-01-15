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
        private bool isFirstTime = true;
        private float timeSinceLastCharacter = 0f;
        private readonly float HeldCharacterCooldown = 0.5f;
        private Keys LastCharacter;
        private Index index = ^0;
        private TextPointer Pointer;


        public SpriteFontName SpriteFont { get; set; }
        public StringBuilder Text { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        private Vector2 scale = new Vector2(1, 1);
        public Vector2 Scale
        {
            get => scale;
            set
            {
                float textureWidth = Globals.SpriteFonts[SpriteFont].MeasureString(Text).X;
                float textureHeight = Globals.SpriteFonts[SpriteFont].MeasureString(Text).Y;
                scale = new Vector2(value.X / (textureWidth / Text.Length), value.Y / textureHeight);
            }
        }
        public float LayerDepth { get; set; }
        public bool IsDead { get; set; } = false;

        private bool isDrawn = false;
        public bool IsDrawn
        {
            get => isDrawn;
            set
            {
                if (value && !isDrawn)
                {
                    EntityTracker.DrawingsText.Add(this);
                    isDrawn = true;
                }
                else if (!value && isDrawn)
                {
                    EntityTracker.DrawingsText.Remove(this);
                    isDrawn = false;
                }
            }
        }
        public TextBox()
        {
            IsDrawn = true;
        }
        public bool Input(GameTime gameTime, Chat chat)
        {
            if (Globals.NewKeyboardKeys.Count > 0)
            {
                LastCharacter = Globals.NewKeyboardKeys[^1];
                timeSinceLastCharacter = 0f;
            }
            if (Globals.NewKeyboardKeys.Count > 0 ||
                (timeSinceLastCharacter > HeldCharacterCooldown && Globals.PressedKeyboardKeys.Contains(LastCharacter)))
            {
                switch (LastCharacter)
                {
                    case Keys.Enter:
                        //IsDrawn = false;
                        Pointer.IsDrawn = false;
                        Pointer = null;
                        return false;
                    case Keys.T:
                        if (!isFirstTime)
                        {
                            IsDrawn = false;
                            Pointer.IsDrawn = false;
                            chat.IsDrawn = false;
                            return false;
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
                    case Keys.Space:
                        Text.Insert(index.GetOffset(Text.Length), " ");
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

            return true;
        }

        public float GetHeight()
        {
            return Globals.SpriteFonts[SpriteFont].MeasureString(Text).Y;
        }
        public float GetWidth()
        {
            return Globals.SpriteFonts[SpriteFont].MeasureString(Text).X;
        }
    }


    class TextPointer : IDrawingText
    {
        private readonly float BlinkCooldown = 0.5f;
        float TimeSinceBlink { get; set; } = 0f;

        public SpriteFontName SpriteFont { get; set; }
        public StringBuilder Text { get; set; } = new StringBuilder("|");
        public Vector2 ParentPosition { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = new Vector2(1, 1);
        public float LayerDepth { get; set; }
        private bool isDrawn = false;
        public bool IsDrawn
        {
            get => isDrawn;
            set
            {
                if (value && !isDrawn)
                {
                    EntityTracker.DrawingsText.Add(this);
                    isDrawn = true;
                }
                else if (!value && isDrawn)
                {
                    EntityTracker.DrawingsText.Remove(this);
                    isDrawn = false;
                }
            }
        }
        public void Update(GameTime gameTime, Index index, StringBuilder text)
        {
            float textWidthTillPointer = Globals.SpriteFonts[SpriteFont].MeasureString(text.ToString(0, index.GetOffset(text.Length))).X;
            Position = new Vector2(ParentPosition.X + (textWidthTillPointer - 3) * Scale.X, ParentPosition.Y - 2);

            TimeSinceBlink += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TimeSinceBlink > BlinkCooldown)
            {
                IsDrawn = !IsDrawn;
                TimeSinceBlink = 0f;
            }
        }
    }
}

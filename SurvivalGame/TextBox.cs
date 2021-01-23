using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class TextBox
    {
        private float timeSinceOpend = 0f;
        private readonly float ClosingCooldown = 0.1f;

        private float timeSinceLastCharacter = 0f;
        private readonly float HeldCharacterCooldown = 0.5f;



        private Keys LastCharacter;
        private Index index = ^0;
        private TextPointer Pointer;
        public DrawingText DrawingText { get; set; }

        public bool Input(GameTime gameTime)
        {
            timeSinceOpend += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceOpend > ClosingCooldown)
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
                            Globals.DrawingTexts.Remove(Pointer.DrawingText);
                            Pointer = null;
                            return false;
                        case Keys.OemTilde:
                            Globals.DrawingTexts.Remove(Pointer.DrawingText);
                            Pointer = null;
                            Globals.DrawingTexts.Remove(DrawingText);
                            Globals.IsUserWriting = false;
                            return false;
                        case Keys.Back:
                            if (index.GetOffset(DrawingText.Text.Length) > 0)
                            {
                                DrawingText.Text.Remove(index.GetOffset(DrawingText.Text.Length) - 1, 1);
                            }
                            break;
                        case Keys.Delete:
                            if (index.Value > 0)
                            {
                                DrawingText.Text.Remove(index.GetOffset(DrawingText.Text.Length), 1);
                                index = ^(index.Value - 1);
                            }
                            break;
                        case Keys.Left:
                            if (index.Value < DrawingText.Text.Length)
                                index = ^(index.Value + 1);
                            break;
                        case Keys.Right:
                            if (index.Value > 0)
                                index = ^(index.Value - 1);
                            break;
                        case Keys.Space:
                            DrawingText.Text.Insert(index.GetOffset(DrawingText.Text.Length), " ");
                            break;
                        case Keys n when (int)n >= 65 && (int)n <= 90:
                            if (!Globals.PressedKeyboardKeys.Contains(Keys.RightShift))
                                DrawingText.Text.Insert(index.GetOffset(DrawingText.Text.Length), n.ToString().ToLower());
                            else
                                DrawingText.Text.Insert(index.GetOffset(DrawingText.Text.Length), n.ToString());
                            break;
                        case Keys n when (int)n >= 48 && (int)n <= 57:
                            DrawingText.Text.Insert(index.GetOffset(DrawingText.Text.Length), n.ToString()[1]);
                            break;
                    }

                }
            }
            timeSinceLastCharacter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Pointer == null)
            {
                Pointer = new TextPointer()
                {
                    ParentPosition = DrawingText.Position,
                    DrawingText = new DrawingText()
                    {
                        SpriteFont = DrawingText.SpriteFont,
                        Text = new StringBuilder("|", 1),
                        Color = DrawingText.Color,
                        Rotation = DrawingText.Rotation,
                        LayerDepth = DrawingText.LayerDepth,
                        IsDrawn = false
                    }
                };
            }
            else
                Pointer.Update(gameTime, index.GetOffset(DrawingText.Text.Length), DrawingText.Text);

            return true;
        }

        public float GetHeight()
        {
            return Globals.SpriteFonts[DrawingText.SpriteFont].MeasureString(DrawingText.Text).Y;
        }
        public float GetWidth()
        {
            return Globals.SpriteFonts[DrawingText.SpriteFont].MeasureString(DrawingText.Text).X;
        }
    }


    class TextPointer
    {
        private readonly float BlinkCooldown = 0.5f;
        float TimeSinceBlink { get; set; } = 0f;
        public DrawingText DrawingText { get; set; }
        public Vector2 ParentPosition { get; set; }
        public void Update(GameTime gameTime, Index index, StringBuilder text)
        {
            float textWidthTillPointer = Globals.SpriteFonts[DrawingText.SpriteFont].MeasureString(text.ToString(0, index.GetOffset(text.Length))).X;
            DrawingText.Position = new Vector2(ParentPosition.X + (textWidthTillPointer - 3) * DrawingText.Scale.X, ParentPosition.Y - 2);

            TimeSinceBlink += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TimeSinceBlink > BlinkCooldown)
            {
                DrawingText.IsDrawn = !DrawingText.IsDrawn;
                TimeSinceBlink = 0f;
            }
        }
    }
}

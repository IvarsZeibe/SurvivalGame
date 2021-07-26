using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class EditorTextInput : UIElement
    {
        public StringBuilder text = new StringBuilder();
        public string placeholder = "";
        public bool error = false;
        public int fontHeight = -1;
        public EditorTextInput(int x, int y, int width, int height, bool topLeft = false)
        {
            Hitbox = new Rect(x, y, width, height, topLeft);
            CreateClickAction();
        }
        void CreateClickAction()
        {
            clickAction = () =>
            {
                if (Globals.NewMouseKeys.Contains(MouseKey.LeftButton))
                {
                    foreach (var UIElement in Globals.Editor.UIElements.Values)
                        UIElement.LoseFocus();
                    selected = true;
                    error = false;
                }
            };
        }
        public void SetBackgroundColor(Color color)
        {
            this.color = color;
        }
        private Keys LastCharacter;
        private Index index = ^0;

        private float timeSinceLastCharacter = 0f;
        private readonly float HeldCharacterCooldown = 0.5f;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (selected)
            {
                TimeSinceBlink += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (TimeSinceBlink > BlinkCooldown)
                {
                    pointerDrawn = !pointerDrawn;
                    TimeSinceBlink = 0f;
                }
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
                            LoseFocus();
                            break;
                        case Keys.OemTilde:
                            text.Insert(index.GetOffset(text.Length), "`");
                            break;
                        case Keys.Back:
                            if (index.GetOffset(text.Length) > 0)
                            {
                                text.Remove(index.GetOffset(text.Length) - 1, 1);
                            }
                            break;
                        case Keys.Delete:
                            if (index.Value > 0)
                            {
                                text.Remove(index.GetOffset(text.Length), 1);
                                index = ^(index.Value - 1);
                            }
                            break;
                        case Keys.Left:
                            if (index.Value < text.Length)
                                index = ^(index.Value + 1);
                            break;
                        case Keys.Right:
                            if (index.Value > 0)
                                index = ^(index.Value - 1);
                            break;
                        case Keys.Space:
                            text.Insert(index.GetOffset(text.Length), " ");
                            break;
                        case Keys n when (int)n >= 65 && (int)n <= 90:
                            if (!Globals.PressedKeyboardKeys.Contains(Keys.RightShift))
                                text.Insert(index.GetOffset(text.Length), n.ToString().ToLower());
                            else
                                text.Insert(index.GetOffset(text.Length), n.ToString());
                            break;
                        case Keys n when (int)n >= 48 && (int)n <= 57:
                            text.Insert(index.GetOffset(text.Length), n.ToString()[1]);
                            break;
                        case Keys.OemQuestion:
                            if (!Globals.PressedKeyboardKeys.Contains(Keys.RightShift))
                                text.Insert(index.GetOffset(text.Length), '/');
                            else
                                text.Insert(index.GetOffset(text.Length), '?');
                            break;
                        case Keys.OemPeriod:
                            if (!Globals.PressedKeyboardKeys.Contains(Keys.RightShift))
                                text.Insert(index.GetOffset(text.Length), '.');
                            else
                                text.Insert(index.GetOffset(text.Length), '>');
                            break;
                        case Keys.OemComma:
                            if (!Globals.PressedKeyboardKeys.Contains(Keys.RightShift))
                                text.Insert(index.GetOffset(text.Length), ',');
                            else
                                text.Insert(index.GetOffset(text.Length), '<');
                            break;
                    }

                }
                timeSinceLastCharacter += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
                pointerDrawn = false;
        }
        private readonly float BlinkCooldown = 0.5f;
        float TimeSinceBlink { get; set; } = 0f;
        bool pointerDrawn = true;
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            string txt;
            Color textColor;
            if (text.Length > 0)
            {
                txt = text.ToString();
                textColor = Color.Black;
            }
            else
            {
                txt = placeholder;
                textColor = Color.Gray;
            }
            if (error)
            {
                textColor = Color.Red;
            }

            var font = Globals.SpriteFonts[SpriteFontName.Aerial16];
            var pos = new Vector2(Hitbox.GetPosVector().X - font.MeasureString(txt).X * 0.5f, Hitbox.GetPosVector().Y - font.MeasureString(txt).Y * 0.5f);
            if(txt == "")
            {
                txt = " ";
                pos = new Vector2(Hitbox.GetPosVector().X - font.MeasureString(txt).X * 0.5f, Hitbox.GetPosVector().Y - font.MeasureString(txt).Y * 0.5f);
                txt = "";
            }
            Vector2 Scale;
            if (fontHeight == -1)
                Scale = Vector2.One;
            else
                Scale = new Vector2(1, fontHeight / font.MeasureString(txt).Y);
            spriteBatch.DrawString(font, txt, pos, textColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, layerDepth - 0.01f);

            if (pointerDrawn)
            {
                float textWidthTillPointer = font.MeasureString(text.ToString(0, index.GetOffset(text.Length))).X;
                var position = new Vector2(pos.X + (textWidthTillPointer - 3) * /*DrawingText.Scale.X*/1, pos.Y - 2);
                spriteBatch.DrawString(font, "|", position, Color.Black, 0f, Vector2.Zero, Scale, SpriteEffects.None, layerDepth - 0.01f);
            }
        }
    }
}

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
    class EditorTextInput : EditorWindow
    {
        public StringBuilder text = new StringBuilder();
        public string placeholder = "";
        public bool error = false;
        public int fontHeight = -1;
        public EditorTextInput(int x, int y, int width, int height, bool topLeft = false) : base(x, y, width, height, topLeft)
        {
            //Hitbox = new Rect(x, y, width, height, topLeft);
        }
        protected override void OnClick(EventArgs e)
        {
            error = false;
            base.OnClick(e);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (selected)
            {
                WriteText(gameTime);
            }
            else
                pointerDrawn = false;
        }
        private readonly float BlinkCooldown = 0.5f;
        float TimeSinceBlink { get; set; } = 0f;
        bool pointerDrawn = true;
        float xOffset = 0;
        protected override void DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            base.DrawOnRenderTarget(spriteBatch);
            var hitbox = Hitbox;
            Hitbox = new Rect(0, 0, Hitbox.Width, Hitbox.Height, true);

            string txt = placeholder;
            Color textColor = Color.Gray;
            if (text.Length > 0)
            {
                txt = text.ToString();
                textColor = Color.Black;
            }
            if (error)
                textColor = Color.Red;

            var font = Globals.SpriteFonts[SpriteFontName.Aerial16];
            var scale = GetScale(font);
            if (!selected)
                txt = GetShortenedText(font, scale, txt);

            Vector2 pos;
            if (txt == "")
            {
                pos = new Vector2(Hitbox.GetPosVector().X - font.MeasureString(" ").X * 0.5f * scale, Hitbox.GetPosVector().Y - font.MeasureString(" ").Y * 0.5f * scale);
            }
            else
            {
                pos = new Vector2(Hitbox.GetPosVector().X - font.MeasureString(txt).X * 0.5f * scale, Hitbox.GetPosVector().Y - font.MeasureString(txt).Y * 0.5f * scale);
            }
            if (pos.X < Hitbox.Left)
            {
                pos.X = (float)Hitbox.Left;
            }

            float textWidthTillPointer = font.MeasureString(text.ToString(0, index.GetOffset(text.Length))).X * scale;
            var position = new Vector2(pos.X + (textWidthTillPointer - 3) * /*DrawingText.Scale.X*/1, pos.Y - 2);
            if (selected)
            {
                float letterAverageWidth = font.MeasureString("a").X * scale;
                while (position.X + xOffset > Hitbox.Right)
                    xOffset -= letterAverageWidth;
                while (position.X + xOffset < Hitbox.Left)
                    xOffset += letterAverageWidth;
            }
            else
                xOffset = 0;

            spriteBatch.DrawString(font, txt, pos + new Vector2(xOffset, 0), textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.01f);
            if (pointerDrawn)
            {
                spriteBatch.DrawString(font, "|", position + new Vector2(xOffset, 0), Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.01f);
            }

            Hitbox = hitbox;
        }
        float GetScale(SpriteFont font)
        {
            float Scale = 1f;
            if(fontHeight == -1)
            {
                if(font.MeasureString("|").Y > Hitbox.Height)
                {
                    Scale = Hitbox.Height / font.MeasureString("|").Y;
                }
            }
            else
            {
                Scale = fontHeight / font.MeasureString("|").Y;
            }
            return Scale;
        }
        string GetShortenedText(SpriteFont font, float Scale, string shortText)
        {
            string endText = "...";
            float letterAverageWidth = font.MeasureString("a").X * Scale;
            float textWidth = font.MeasureString(shortText).X * Scale;
            if (textWidth > Hitbox.Width)
            {
                int letterCountToRemove = (int)Math.Ceiling((textWidth + font.MeasureString(endText).X * Scale - Hitbox.Width) / letterAverageWidth);
                shortText = shortText.Remove(text.Length - letterCountToRemove - 1, letterCountToRemove);
                shortText += endText;
                while(font.MeasureString(shortText).X > Hitbox.Width)
                {
                    if (shortText.Length > 4)
                        shortText = shortText.Remove(shortText.Length - 4, 1);
                    else
                        break;
                }
            }
            return shortText;
        }
        private Keys LastCharacter;
        private Index index = ^0;

        private float timeSinceLastCharacter = 0f;
        private readonly float HeldCharacterCooldown = 0.5f;
        void WriteText(GameTime gameTime)
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
                        selected = false;
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
                    case Keys.OemPipe:
                        if (!Globals.PressedKeyboardKeys.Contains(Keys.RightShift))
                            text.Insert(index.GetOffset(text.Length), '\\');
                        else
                            text.Insert(index.GetOffset(text.Length), '|');
                        break;
                }

            }
            timeSinceLastCharacter += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}

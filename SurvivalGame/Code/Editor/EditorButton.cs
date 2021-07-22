using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class EditorButton : UIElement
    {
        public Color defaultColor = Color.White;
        public Color hoverColor = new Color(200, 200, 200);
        public Color holdingColor = Color.Gray;
        public Color releaseColor = Color.Red;
        public string text = "";
        public bool Selectable = false;
        public EditorButton()
        {
            Hitbox = new Rect(10, 10, 300, 50, true);
        }
        protected override void OnDefault()
        {
            color = defaultColor;
        }
        protected override void OnHover()
        {
            color = new Color(200, 200, 200);
        }
        protected override void ClickEvent(GameTime gameTime)
        {
            if (Globals.NewMouseKeys.Contains(MouseKey.LeftButton))
            {
                holding = (true, MouseKey.LeftButton);
                if (Selectable)
                {
                    foreach (var UIElement in Globals.Editor.UIElements.Values)
                        UIElement.LoseFocus();
                    selected = true;
                }
                clickAction();
            }
        }
        protected override void OnHolding()
        {
            color = Color.Gray;
        }
        protected override void OnRelease()
        {
            color = Color.Red;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (text != "")
            {
                var font = Globals.SpriteFonts[SpriteFontName.Aerial16];
                var pos = Hitbox.GetPosVector() - font.MeasureString(text) * 0.5f;
                spriteBatch.DrawString(font, text, pos, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth - 0.01f);
            }
        }
    }
}
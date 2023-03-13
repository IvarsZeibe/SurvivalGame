using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class EditorButton : UIElement
    {
        public Color hoverColor = new Color(200, 200, 200);
        public Color holdingColor = Color.Gray;
        public Color releaseColor = Color.Red;
        public string text = "";
        //public bool Selectable = false;
        public EditorButton()
        {
            Hitbox = new Rect(10, 10, 300, 50, true);
            defaultColor = Color.White;
        }
        protected override void SetDefault()
        {
            base.SetDefault();
        }
        protected override void OnHover(EventArgs e)
        {
            color = new Color(200, 200, 200);

            base.OnHover(e);
        }
        protected override void OnClick(EventArgs e)
        {
            //if (Selectable)
            //{
            //    //foreach (var UIElement in Globals.Editor.UIElements.Values)
            //    //{
            //    //    if(UIElement != this)
            //    //        UIElement.LoseFocus();
            //    //}
            //    selected = true;
            //}

            base.OnClick(e);
        }
        protected override void OnHeld(EventArgs e)
        {
            color = Color.Gray;
            base.OnHeld(e);
        }
        protected override void OnRelease(EventArgs e)
        {
            color = Color.Red;
            base.OnRelease(e);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (text != "")
            {
                var font = Globals.SpriteFonts[SpriteFontName.Aerial16];
                var pos = Hitbox.GetPosVector() - font.MeasureString(text) * 0.5f;
                spriteBatch.DrawString(font, text, pos, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth - 0.001f);
            }
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class EditorBox : UIElement
    {
        public string text = "";
        public EditorBox(int x, int y, int width, int height, string text = "", bool topLeft = false)
        {
            this.text = text;
            Hitbox = new Rect(x, y, width, height, topLeft);
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

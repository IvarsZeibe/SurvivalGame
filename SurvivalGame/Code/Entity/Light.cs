using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Light
    {
        string texture;
        // top left
        Vector2 position;
        Color color;
        Vector2 size;
        public Light(Vector2 pos, Vector2 size, Color color, string texture = "light2")
        {
            this.texture = texture;
            position = pos - size * 0.5f;
            this.size = size;
            this.color = color;

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(size.X / Globals.Textures[texture].Width, size.Y / Globals.Textures[texture].Height);
            Globals.spriteBatch.Draw(Globals.Textures[texture], position, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.1f);
        }
    }
}

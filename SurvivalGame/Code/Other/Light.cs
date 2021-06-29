using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Light
    {
        public string texture { get; set; }
        // top left
        public Vector2 position { get; set; }
        public bool relativePosition { get; set; }
        public Color color { get; set; }
        public Vector2 size { get; set; }
        Light() { }
        public Light(Vector2 pos, Vector2 size, Color color, bool relativePosition = false, string texture = "light2")
        {
            this.texture = texture;
            this.relativePosition = relativePosition;
            position = pos - size * 0.5f;
            this.size = size;
            this.color = color;
        }
        public void Draw(SpriteBatch spriteBatch, Entity entity = null)
        {
            Vector2 pos = position;
            if (entity != null)
                pos += entity.Hitbox.GetPosVector();
            Vector2 scale = new Vector2(size.X / Globals.Textures[texture].Width, size.Y / Globals.Textures[texture].Height);
            Globals.spriteBatch.Draw(Globals.Textures[texture], pos, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.1f);
        }
    }
}

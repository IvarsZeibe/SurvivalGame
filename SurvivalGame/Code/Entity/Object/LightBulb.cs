using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class LightBulb : Entity
    {
        LightBulb() { }
        public LightBulb(Vector2 pos, Vector2 size, Color color) : base(false)
        {
            Hitbox = new Rect(pos.X, pos.Y, (int)Math.Round(size.X), (int)Math.Round(size.Y));
            Hitbox.Active = false;
            Collision = false;
            Lights.Add(new Light(Hitbox.GetPosVector(), Hitbox.GetScaleVector(), color));
            //Drawing = new Drawing("light", Hitbox.GetTopLeftPosVector(), color, 0f, size, 0.20f - (float)pos.Y / 100000);
            //Drawings.Add("base", Drawing);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateAnimations(gameTime);
        }
    }
}

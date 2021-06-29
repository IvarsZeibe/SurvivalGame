using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Stone : Entity
    {
        public Hitbox drawingHitbox { get; set; }
        Stone() { }
        public Stone(Vector2 pos) : base(false)
        {
            drawingHitbox = new Rect(pos.X, pos.Y, 90, 90);
            Hitbox = new Rect(pos.X + 10, pos.Y + 46, 80, 20);

            Collision = true;
            Mass = 30;
            int randInt = Globals.rand.Next(150, 235);
            Drawing = new Drawing("stone", drawingHitbox.GetTopLeftPosVector(), new Color(randInt, randInt, randInt), 0f, new Vector2(100, 100), 0.30f - (float)pos.Y / 100000);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}

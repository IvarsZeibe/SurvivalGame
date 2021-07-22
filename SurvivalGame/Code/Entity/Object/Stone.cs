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
            Hitbox = new Rect(pos.X, pos.Y, 80, 20);
            Collision = true;
            Mass = 30;
            int randInt = Globals.rand.Next(150, 235);
            Drawing = new Drawing("stone", new Vector2(pos.X, pos.Y), new Color(randInt, randInt, randInt), 0f, new Vector2(100, 100), 0.30f - (float)pos.Y / 100000);
            Drawing.Offset = new Vector2(-55, -91);
        }
        public override void SetDafaultValues()
        {
            Hitbox = new Rect(0, 0, 80, 20);
            Collision = true;
            Mass = 30;
            int randInt = Globals.rand.Next(150, 235);
            Drawing = new Drawing("stone", Vector2.Zero, new Color(randInt, randInt, randInt), 0f, new Vector2(100, 100), 0.30f - (float)0 / 100000, false);
            Drawing.Offset = new Vector2(-55, -91);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Drawing.Coord = Hitbox.GetPosVector();
        }
        protected override void CreateDefaultProperties()
        {
            properties = new Dictionary<string, Action<object>>();
            properties.Add("width", new Action<object>(width => Hitbox.Width = Convert.ToInt32(width)));
            properties.Add("height", new Action<object>(height => Hitbox.Height = Convert.ToInt32(height)));
            properties.Add("x", new Action<object>(x => Hitbox.X = Convert.ToDouble(x)));
            properties.Add("y", new Action<object>(y => Hitbox.Y = Convert.ToDouble(y)));
        }
    }
}

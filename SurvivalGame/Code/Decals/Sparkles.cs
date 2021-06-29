using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Sparkles : IUpdate
    {
        public Drawing Drawing { get; set; }
        public float Lifespan { get; set; } = 0.25f;
        public float timeAlive { get; set; } = 0;
        Sparkles()
        {
            Globals.Updatables.Add(this);
            if (Drawing.IsDrawn)
                Globals.Drawings.Add(Drawing);
        }
        public Sparkles(Vector2 _position)
        {
            Vector2 size = new Vector2(20, 20);
            Vector2 position = _position - size * 0.5f;
            Drawing = new Drawing(TextureName.Sparkles, position, Color.White, 0f, size, layerDepth: 0.2f);
            Globals.Updatables.Add(this);
        }

        public bool UpdateEnabled { get; set; } = true;
        public bool IsDead { get; set; } = false;

        public void Update(GameTime gameTime)
        {
            timeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Drawing.Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Drawing.Scale = new Vector2(Drawing.GetWidth(), Drawing.GetHeight()) + new Vector2(10,10) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeAlive > Lifespan)
            {
                IsDead = true;
                Globals.Drawings.Remove(Drawing);
            }
        }
    }
}

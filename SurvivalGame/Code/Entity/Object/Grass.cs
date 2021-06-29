using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Grass : Entity
    {
        Grass() { }
        public Grass(Vector2 pos) : base(false)
        {
            Hitbox = new Rect(pos.X, pos.Y, 50, 50);
            Hitbox.Active = false;
            Collision = false;
            Drawing = new Drawing("grass", Hitbox.GetTopLeftPosVector(), new Color(0, Globals.rand.Next(100, 140), 0), 0f, new Vector2(50, 50), 0.40f - (float)pos.Y / 100000);
            Drawing.originPercentage = new Vector2(0.5f, 1f);
            //Drawing.Offset = new Vector2(0.5f, 1f) * new Vector2(50, 50);
            var animation = new ShakeAnimation(Drawing);
            //animation.isLoop = true;
            Animations.Add("wind", animation);
            //Animations["wind"].Start();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateAnimations(gameTime);
        }
        protected override void UpdateAnimations(GameTime gameTime)
        {
            base.UpdateAnimations(gameTime);
            if (Animations["wind"].Inactive)
            {
                if (Globals.getActiveRoom.windXCoord - 50 < X && Globals.getActiveRoom.windXCoord + 50 > X)
                {
                    Animations["wind"] = new ShakeAnimation(Drawing, Globals.rand.Next(4,15)/10);
                    Animations["wind"].Start();
                }
            }
        }
    }
}

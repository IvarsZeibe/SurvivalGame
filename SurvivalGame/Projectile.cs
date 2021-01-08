using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Projectile : Entity
    {
        public int Damage { get; set; }
        int Range { get; set; }
        List<double> RelativCoord { get; set; }
        List<double> StartingCoord { get; set; }
        public Projectile(Texture2D texture, float speed, Vector2 source, Vector2 target, int damage)
        {
            Collision = false;
            StartingCoord = new List<double>() { source.X, source.Y };
            RelativCoord = new List<double>() { 0, 0 };
            Range = 300;

            Texture = texture;
            Speed = speed;
            Hitbox = new Rect(source.X, source.Y, 10, 2);
            Damage = damage;

            double yEdge = (source.Y - target.Y);
            double xEdge = (source.X - target.X);
            Rotation = (float)Math.Atan2(yEdge, xEdge);

            Vector2 relativeMouse = new Vector2((float)xEdge, (float)yEdge);
            XMovement = -relativeMouse.X / ((Math.Abs(relativeMouse.X) + Math.Abs(relativeMouse.Y)) * Speed);
            YMovement = -relativeMouse.Y / ((Math.Abs(relativeMouse.X) + Math.Abs(relativeMouse.Y)) * Speed);
        }
        public override void Update(GameTime gameTime)
        {
            Move(XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
            Move(YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);

            if (XMovement > 0)
            {
                RelativCoord[0] = X - StartingCoord[0];
            }
            else RelativCoord[0] = StartingCoord[0] - X;
            if (YMovement > 0)
            {
                RelativCoord[1] = Y - StartingCoord[1];
            }
            else RelativCoord[1] = StartingCoord[1] - Y;

            if (RelativCoord[0] * RelativCoord[0] + RelativCoord[1] * RelativCoord[1] > Range * Range)
            {
                isDead = true;
            }

            foreach (var wall in EntityTracker.Walls)
            {
                if (CollidesWith(wall) && wall.Collision)
                {
                    isDead = true;
                }
            }
        }
    }
}

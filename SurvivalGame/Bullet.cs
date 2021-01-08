using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Bullet : Entity
    {

        // Not used



        int Damage { get; set; }
        int Range { get; set; }
        List<double> RelativCoord { get; set; }
        List<double> StartingCoord { get; set; }
        public Bullet(Texture2D texture, float speed, float x, float y, float rotation, Vector2 relativeMouse)
        {
            Mass = 1f;
            Collision = false;
            StartingCoord = new List<double>() { x, y };
            Hitbox = new Rect(x, y, 10, 2);
            Rotation = rotation;
            RelativCoord = new List<double>() { 0, 0 };
            Speed = 500f;
            Range = 300;
            Texture = texture;
            XMovement = -relativeMouse.X / ((Math.Abs(relativeMouse.X) + Math.Abs(relativeMouse.Y)) * Speed);
            YMovement = -relativeMouse.Y / ((Math.Abs(relativeMouse.X) + Math.Abs(relativeMouse.Y)) * Speed);
        }
        public void Update(GameTime gameTime, List<Entity> deadEntities)
        {
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
                deadEntities.Add(this);
            }
        }
    }
}

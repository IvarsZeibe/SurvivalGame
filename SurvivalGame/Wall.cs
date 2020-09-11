using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Wall : Entity
    {
        bool ghost;
        float timeAlive;
        public Wall(Texture2D texture, double x, double y, bool collision = true)
        {
            Random rand = new Random();
            //Force = 5;
            Mass = 9;
            //Speed = 1;
            Texture = texture;
            //X = (int)(x / 50) * 50;
            //Y = (int)(y / 50) * 50;
            //Size = new Point(50, 50);
            X = x;
            Y = y;
            Size = new Point(rand.Next(18, 20), rand.Next(18, 20));
            Collision = collision;
            ghost = !collision;
        }
        public void Update(GameTime gameTime, List<Entity> deadEntities)
        {
            Force = Mass * Speed;
            timeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(ghost && timeAlive > 0.1)
            {
                deadEntities.Add(this);
            }
            Center = new Vector2((float)X + Size.X / 2, (float)Y + Size.Y / 2);
            Rect = new Rectangle((int)X, (int)Y, Size.X, Size.Y);
        }
    }
}

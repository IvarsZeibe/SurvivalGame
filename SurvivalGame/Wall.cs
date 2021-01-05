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

        //Point Size;
        bool ghost;
        float timeAlive;
        public Wall(Texture2D texture, double x, double y, bool collision = true)
        {
            Random rand = new Random();
            //Force = 5;
            Mass = 19;
            //Speed = 1;
            Texture = texture;
            //X = (int)(x / 50) * 50;
            //Y = (int)(y / 50) * 50;
            //Size = new Point(50, 50);
            //X = x;
            //Y = y;
            //Size = new Point(rand.Next(18, 20), rand.Next(18, 20));
            Collision = collision;
            ghost = !collision;
            Hitbox = new Rect(x, y, 50, 50);

        }
        public void Update(GameTime gameTime)
        {
            Force = Mass * Speed;
            timeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(ghost && timeAlive > 0.1)
            {
                isDead = true;
            }
            //Center = new Vector2((float)X + Hitbox.Width / 2, (float)Y + Hitbox.Height / 2);
            //Rect = new Rectangle((int)X, (int)Y, Hitbox.Width, Hitbox.Height);
        }
    }
}

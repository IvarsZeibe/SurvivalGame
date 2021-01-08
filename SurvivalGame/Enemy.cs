using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Enemy : Entity
    {
        public Enemy(Texture2D texture, float x, float y, int width = 20, int height = 0, int speed = 100, bool collision = true)
        {
            if(height == 0)
                Hitbox = new Circle(x, y, width);
            else
                Hitbox = new Rect(x, y, width, height);
            Mass = 5;
            Collision = collision;
            Speed = speed;
            Health = 300;
            Texture = texture;

        }
        public void Movement(double xedge, double yedge)
        {
            XMovement = -xedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed);
            YMovement = -yedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed);

        }
        public override bool DamageEntity(int damage, string source)
        {
            Health -= damage;
            if (Health / 100 != (Health + damage) / 100) 
            {
                Hitbox.Width = (int)(Hitbox.Width * 0.8);
                Hitbox.Height = (int)(Hitbox.Height * 0.8);
            }
            if (Health <= 0)
            {
                isDead = true;
            }
            return true;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Enemy : Entity
    {
        int Health { get; set; }
        float TimeSinceDamageRecieved;
        float DamageRecievedCooldown = 0.2f;
        public Enemy(Texture2D texture, double x, double y, int sizex = 20, int sizey = 30, int speed = 100, bool collision = true)
        {
            Mass = 5;
            //Force = 2;
            Collision = collision;
            Speed = speed;
            Health = 3;
            X = x;
            Y = y;
            Size = new Point(sizex, sizey);
            Texture = texture;
        }
        public void Update(GameTime gameTime)
        {
            //Force = Mass * Speed;
            TimeSinceDamageRecieved += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Center = new Vector2((float)X + Size.X / 2, (float)Y + Size.Y / 2);
            Rect = new Rectangle((int)X, (int)Y, Size.X, Size.Y);
        }
        public void Movement(double xedge, double yedge)
        {
            XMovement = -xedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed);
            YMovement = -yedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed);
        }
        public void DamageHealth(List<Entity> deadEntities)
        {
            if(TimeSinceDamageRecieved > DamageRecievedCooldown)
            {
                Health--;
                Size = new Point((int)(Size.X * 0.8), (int)(Size.Y * 0.8));
                if (Health <= 0)
                {
                    deadEntities.Add(this);
                }
                TimeSinceDamageRecieved = 0f;
            }
        }
    }
}

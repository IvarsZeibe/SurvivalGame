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
        //int maxHealth;
        //Point startSize;
        float TimeSinceDamageRecieved;
        float DamageRecievedCooldown = 0.2f;
        public Enemy(Texture2D texture, double x, double y, int width = 20, int height = 30, int speed = 100, bool collision = true)
        {
            isDead = false;
            Mass = 5;
            Collision = collision;
            Speed = speed;
            Health = 300;
            //maxHealth = 300;
            X = x;
            Y = y;
            //Size = new Point(sizex, sizey);
            //startSize = Size;
            Texture = texture;
            Hitbox = new Rect(X, Y, 20, 30);
            //Hitbox = new Circle(X, Y, 20);

        }
        public void Update(GameTime gameTime)
        {
            TimeSinceDamageRecieved += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Center = new Vector2((float)X + Size.X / 2, (float)Y + Size.Y / 2);
            //Rect = new Rectangle((int)X, (int)Y, Size.X, Size.Y);
            //Hitbox = new Rect(X, Y, Size.X, Size.Y);
        }
        public void Movement(double xedge, double yedge)
        {
            XMovement = -xedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed);
            YMovement = -yedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed);
        }
        //public void DamageHealth(List<Entity> deadEntities, int amount = 100)
        //{
        //    if(TimeSinceDamageRecieved > DamageRecievedCooldown)
        //    {
        //        Health = Health - amount;
        //        if(Health / 100 != (Health + amount) / 100)
        //            Size = new Point((int)(Size.X * 0.8), (int)(Size.Y * 0.8));
        //        if (Health <= 0)
        //        {
        //            deadEntities.Add(this);
        //        }
        //        TimeSinceDamageRecieved = 0f;
        //    }
        //}
        public bool DamageEntity(int damage, string source)
        {
            //if (TimeSinceDamageRecieved > DamageRecievedCooldown)
            {
                Health -= damage;
                if (Health / 100 != (Health + damage) / 100) 
                {
                    //Size = new Point((int)(Size.X * 0.8), (int)(Size.Y * 0.8));
                    Hitbox.Width = (int)(Hitbox.Width * 0.8);
                    Hitbox.Height = (int)(Hitbox.Height * 0.8);
                }
                //Size = new Point((int)(startSize.X * (Health / (double)maxHealth)), (int)(startSize.Y * (Health / (double)maxHealth)));
                if (Health <= 0)
                {
                    isDead = true;
                }
                TimeSinceDamageRecieved = 0f;
                return true;
            }
            //else return false;
        }
    }
}

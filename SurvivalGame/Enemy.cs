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
        private Entity Target { get; set; }
        public Enemy(Texture2D texture, float x, float y, int width, int height, int speed, bool collision, Entity target)
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
            Target = target;
        }
        public override void Update(GameTime gameTime)
        {
            Movement();
            Move(XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
            Move(YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);

            foreach (var projectile in EntityTracker.Projectiles)
            {
                if (CollidesWith(projectile))
                {
                    DamageEntity(projectile.Damage, "Projectile");
                    projectile.isDead = true;
                }
            }
            foreach (var sword in EntityTracker.Swords)
            {
                if (sword.CollidesWith(this) && !sword.hitEntities.Contains(this))
                {
                    DamageEntity(sword.Damage, "Sword");
                    sword.hitEntities.Add(this);
                }
            }
        }
        private void Movement()
        {
            if (Target == null)
            {
                XMovement = 0;
                YMovement = 0;
            }
            else
            {
                double xedge = Hitbox.X - Target.Hitbox.X;
                double yedge = Hitbox.Y - Target.Hitbox.Y;
                XMovement = -xedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed);
                YMovement = -yedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed);
            }

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

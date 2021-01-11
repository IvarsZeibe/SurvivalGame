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
        public enum Type
        {
            RandomaRectangle,
            RandomCircle,
            Custom
        }
        public enum Weapon
        {
            Pistol,
            //Minigun,
            Sword
        }
        private float PrimaryRateOfFire;
        private float PrimaryCooldown = 0f;
        private float SecondaryRateOfFire;
        private float SecondaryCooldown = 0f;
        public Weapon Primary { get; set; } = Weapon.Pistol;
        public Weapon Secondary { get; set; } = Weapon.Sword;
        private Entity Target { get; set; }

        public Enemy() { }
        public Enemy(Texture2D texture, float x, float y, int width, int height, int speed, bool collision, Entity target, Color? color)
        {
            this.Texture = texture;
            if (height == 0)
                this.Hitbox = new Circle(x, y, width);
            else
                 this.Hitbox = new Rect(x, y, width, height);
            this.Mass = 5;
            this.Collision = collision;
            this.Speed = speed;
            this.Health = 300;
            this.Target = target;
            if (color != null)
                Color = (Color)color;
        }
        //public Enemy(Texture2D texture, float x, float y, int width, int height, bool collision, int health, int mass, Color color, float speed = 0f, float rotation = 0f, float layerDepth = 0.5f)
        //{
        //    this.Texture = texture;
        //    this.Hitbox = new Rect(x, y, width, height);
        //    this.Collision = collision;
        //    this.Health = health;
        //    this.Mass = mass;
        //    this.Color = color;
        //    this.Speed = speed;
        //    this.Rotation = rotation;
        //    this.LayerDepth = layerDepth;
        //}
        //public Enemy(Texture2D texture, float x, float y, int diameter, bool collision, int health, int mass, Color color, float speed = 0f, float rotation = 0f, float layerDepth = 0.5f)
        //{
        //    this.Texture = texture;
        //    this.Hitbox = new Circle(x, y, diameter);
        //    this.Collision = collision;
        //    this.Health = health;
        //    this.Mass = mass;
        //    this.Color = color;
        //    this.Speed = speed;
        //    this.Rotation = rotation;
        //    this.LayerDepth = layerDepth;
        //}
        public override void Update(GameTime gameTime)
        {
            PrimaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            SecondaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Movement();
            Move(XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
            Move(YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);

            double distanceFromTarget = Math.Sqrt((X - Target.X) * (X - Target.X) + (Y - Target.Y) * (Y - Target.Y));
            if (distanceFromTarget > 100)
                PrimaryAttack();
            else
                SecondaryAttack();

            foreach (var projectile in EntityTracker.GetEntities<Projectile>())
            {
                if (CollidesWith(projectile) && !projectile.immuneEntities.Contains(this))
                {
                    DamageEntity(projectile.Damage, "Projectile");
                    projectile.immuneEntities.Add(this);
                    projectile.IsDead = true;
                }
            }
            foreach (var sword in EntityTracker.GetEntities<Sword>())
            {
                if (sword.CollidesWith(this) && !sword.immuneEntities.Contains(this))
                {
                    DamageEntity(sword.Damage, "Sword");
                    sword.immuneEntities.Add(this);
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
                IsDead = true;
            }
            return true;
        }
        public void PrimaryAttack()
        {
            switch (Primary)
            {
                case Weapon.Pistol:
                    PrimaryRateOfFire = 0.3f;
                    if (PrimaryCooldown > PrimaryRateOfFire)
                    {
                        EntityTracker.Add.Projectile(Game1.textures["Rectangle"], 500f, new Vector2(X, Y), new Vector2(Target.X, Target.Y), 200).immuneEntities.Add(this);
                        //p.immuneEntities.Add(this);
                        //Random rand = new Random();
                        //p.SetPrecision((float)(rand.NextDouble() + 0.5));
                        //p.Precision = 0.1f;

                        PrimaryCooldown = 0;
                    }
                    break;
                //case Weapon.Minigun:
                //    PrimaryRateOfFire = 0.1f;
                //    if (PrimaryCooldown > PrimaryRateOfFire)
                //    {
                //        MouseState mstate = Mouse.GetState();
                //        EntityTracker.Add.Projectile(Game1.textures["Flame"], 500f, new Vector2(X, Y), new Vector2(mstate.X, mstate.Y), 200);

                //        PrimaryCooldown = 0;
                //    }
                //    break;
            }
        }
        public void SecondaryAttack()
        {
            switch (Secondary)
            {
                case Weapon.Sword:
                    SecondaryRateOfFire = 0.3f;
                    if (SecondaryCooldown > SecondaryRateOfFire)
                    {
                        double yEdge = (Y - Target.Y);
                        double xEdge = (X - Target.X);
                        EntityTracker.Add.Sword(Game1.textures["Rectangle"], this, (float)Math.Atan2(yEdge, xEdge)).immuneEntities.Add(this);

                        SecondaryCooldown = 0;
                    }
                    break;
            }
        }
    }
}

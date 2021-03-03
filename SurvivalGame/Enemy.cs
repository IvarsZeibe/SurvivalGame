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
        protected float PrimaryRateOfFire;
        protected float PrimaryCooldown = 0f;
        private float SecondaryRateOfFire;
        private float SecondaryCooldown = 0f;
        private int defaultWidth;
        private int defaultHeight;
        private int minSize = 10;
        public Enemy() { }
        public Enemy(TextureName texture, float x, float y, int width = 20, int height = 0, int speed = 100, bool collision = true, Entity target = null, Color? color = null)
        {
            if (height == 0)
                this.Hitbox = new Circle(x, y, width);
            else
                this.Hitbox = new Rect(x, y, width, height);
            defaultWidth = width;
            defaultHeight = height;
            this.Mass = 5;
            this.Collision = collision;
            this.Speed = speed;
            this.MaxHealth = 100;
            this.Health = 100;
            this.Target = target;
            CreateHealthBar();
            Drawing = new Drawing(texture, new Vector2(x, y), color ?? Color.White, 0, new Vector2(width, height), 0.4f, true);
            
        }
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
        public Weapon Primary { get; set; } = Weapon.Pistol;
        public Weapon Secondary { get; set; } = Weapon.Sword;
        protected Entity Target { get; set; }
        public override void Update(GameTime gameTime)
        {
            PrimaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            SecondaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Movement();
            Move(XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
            Move(YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);

            if (!Target.IsDead)
            {
                double distanceFromTarget = Math.Sqrt((X - Target.X) * (X - Target.X) + (Y - Target.Y) * (Y - Target.Y));
                if (distanceFromTarget > 100)
                    PrimaryAttack();
                else
                    SecondaryAttack();
            }

            foreach (var projectile in EntityTracker.GetEntities<Projectile>())
            {
                if (CollidesWith(projectile) && !projectile.immuneEntities.Contains(this))
                {
                    DamageSelf(projectile.Damage, "Projectile");
                    projectile.immuneEntities.Add(this);
                    projectile.Kill();
                }
            }
            foreach (var sword in EntityTracker.GetEntities<Sword>())
            {
                if (sword.CollidesWith(this) && !sword.immuneEntities.Contains(this))
                {
                    DamageSelf(sword.Damage, "Sword");
                    sword.immuneEntities.Add(this);
                }
            }

            Drawing.Position = new Vector2((float)Hitbox.Left, (float)Hitbox.Top);
            Drawing.Scale = new Vector2(Hitbox.Width, Hitbox.Height);
        }
        private void CreateHealthBar()
        {
            new HealthBar(this);
        }
        private void Movement()
        {
            if (Target.IsDead)
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
        public override bool DamageSelf(int damage, string source)
        {
            Health -= damage;
            if (Hitbox is Circle)
                Hitbox.Width = (int)((defaultWidth - minSize) * ((float)Health / MaxHealth)) + minSize;
            else
            {
                Hitbox.Width = (int)((defaultWidth - minSize) * ((float)Health / MaxHealth)) + minSize;
                Hitbox.Height = (int)((defaultHeight - minSize) * ((float)Health / MaxHealth)) + minSize;
            }
            if (Health <= 0)
            {
                Kill();
            }
            return true;
        }
        public void PrimaryAttack()
        {
            switch (Primary)
            {
                case Weapon.Pistol:
                    PrimaryRateOfFire = 0.6f;
                    if (PrimaryCooldown > PrimaryRateOfFire)
                    {
                        new Projectile(TextureName.Rectangle, 500f, new Vector2(X, Y), new Vector2(Target.X, Target.Y), 10).immuneEntities.Add(this);
                        //p.immuneEntities.Add(this);
                        //Random rand = new Random();
                        //p.SetPrecision((float)(rand.NextDouble() + 0.5));
                        //p.Precision = 0.1f;

                        PrimaryCooldown = 0;
                    }
                    break;
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
                        new Sword(TextureName.Rectangle, this, (float)Math.Atan2(yEdge, xEdge), 5).immuneEntities.Add(this);

                        SecondaryCooldown = 0;
                    }
                    break;
            }
        }
    }
}

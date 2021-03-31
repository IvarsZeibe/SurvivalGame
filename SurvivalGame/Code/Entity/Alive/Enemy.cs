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
        protected float PrimaryCooldown = 0f;
        private int defaultWidth;
        private int defaultHeight;
        private int minSize = 10;
        private int detectionRange = 400;
        private int followRange = 800;
        private bool aggresive = true;
        Inventory Inventory = new Inventory(3);
        private HealthBar HealthBar;

        public Enemy(TextureName texture, float x, float y, int width = 20, int height = 0, int speed = 100, bool collision = true, Entity target = null, Color? color = null, bool addToRoom = true) : base(addToRoom)
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
            HealthBar = new HealthBar(this);
            Drawing = new Drawing(texture, new Vector2(x, y), color ?? Color.White, 0, new Vector2(width, height), 0.4f, true);
            Inventory.Add(new Pistol());
            Inventory.Add(new SwordItem());
        }
        public override void Update(GameTime gameTime)
        {
            PrimaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Movement();
            Move(XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
            Move(YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);

            TryToAttack();

            foreach (var projectile in EntityTracker.GetEntities<Projectile>())
            {
                if (CollidesWith(projectile) && !projectile.immuneEntities.Contains(this))
                {
                    DamageSelf(projectile.Damage, projectile.owner);
                    projectile.immuneEntities.Add(this);
                    projectile.Kill();
                }
            }
            foreach (var sword in EntityTracker.GetEntities<Sword>())
            {
                if (sword.CollidesWith(this) && !sword.immuneEntities.Contains(this))
                {
                    DamageSelf(sword.Damage, sword.owner);
                    sword.immuneEntities.Add(this);
                }
            }

            Drawing.Position = new Vector2((float)Hitbox.Left, (float)Hitbox.Top);
            Drawing.Scale = new Vector2(Hitbox.Width, Hitbox.Height);
        }
        private void Movement()
        {
            CheckForTarget();
            if(!Target.IsDead)
            {
                double xedge = Hitbox.X - Target.Hitbox.X;
                double yedge = Hitbox.Y - Target.Hitbox.Y;
                XMovement = -xedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed);
                YMovement = -yedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed);
                if (Math.Abs(xedge) < Hitbox.Width)
                    XMovement = 0;
                if (Math.Abs(yedge) < Hitbox.Height)
                    YMovement = 0;
            }

        }
        private void TryToAttack()
        {
            if (aggresive && !Target.IsDead)
            {
                double distanceFromTarget = Math.Sqrt((X - Target.X) * (X - Target.X) + (Y - Target.Y) * (Y - Target.Y));
                if (distanceFromTarget > 100)
                {
                    if (PrimaryCooldown > Inventory.Get(0).Cooldown)
                    {
                        Inventory.Get(0).OnPrimaryUse(this);
                        if (Inventory.Get(0).Successful)
                            PrimaryCooldown = 0;
                    }
                }
                else
                {
                    if (PrimaryCooldown > Inventory.Get(1).Cooldown)
                    {
                        Inventory.Get(1).OnPrimaryUse(this);
                        if (Inventory.Get(1).Successful)
                            PrimaryCooldown = 0;
                    }
                }

            }
        }
        protected void CheckForTarget()
        {
            if(Target.IsDead)
            {
                foreach(var target in EntityTracker.GetEntities<Player>())
                {
                    if(Hitbox.Distance(target.Hitbox) < detectionRange)
                    {
                        Target = target;
                        aggresive = true;
                        return;
                    }
                }
                Random rand = new Random();
                if (rand.Next(0) == 1)
                    Target = new NoBrainEntity(X, Y);
                else
                {
                    float xPosition = X + rand.Next(-followRange, followRange);
                    float yPosition = Y + rand.Next(-followRange, followRange);

                    if (xPosition < 0)
                        xPosition = X + followRange;
                    else if (xPosition > Globals.graphics.PreferredBackBufferWidth)
                        xPosition = X - followRange;
                    if (yPosition < 0)
                        yPosition = Y + followRange;
                    else if (yPosition > Globals.graphics.PreferredBackBufferHeight)
                        yPosition = Y - followRange;

                    Target = new NoBrainEntity(xPosition, yPosition);
                }
                aggresive = false;
            }
            else
            {
                if (Hitbox.Distance(Target.Hitbox) > followRange)
                {
                    Target = new NoBrainEntity(X, Y);
                    aggresive = false;
                }

            }
        }
        public override bool DamageSelf(int damage, Entity source, DamageType damageType = DamageType.Unknown)
        {
            if(source is Player)
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
                if(source is Player) { }
                    Globals.HUD.points += 1;
                Globals.HUD.EnemiesLeft -= 1;
                Kill();
            }
            return true;
        }
        public override void Load()
        {
            HealthBar.Load();
            Drawing.IsDrawn = true;
        }
        public override void UnLoad()
        {
            HealthBar.UnLoad();
            Drawing.IsDrawn = false;
        }
    }
}

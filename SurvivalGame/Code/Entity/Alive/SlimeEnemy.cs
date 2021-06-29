using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class SlimeEnemy : Entity
    {
        public int Damage { get; set; }
        public float AttackCooldown { get; set; } = 0.1f;
        public float AttackCooldownLeft { get; set; } = 0f;
        public bool aggresive { get; set; } = true;

        public double JumpSpeedMultiplier { get; set; } = 1;
        public double JumpProgress { get; set; } = 0;
        public double BaseJumpCooldown { get; set; } = 1.5;
        double JumpCooldownRandomness { get => (Globals.rand.NextDouble() - 0.5) * 2; }
        public double JumpCooldownLeft { get; set; } = 0;
        public float yOffset { get; set; } = 0f;

        public double knockbackX { get; set; } = 0;
        public double knockbackY { get; set; } = 0;

        public int detectionRange { get; set; } = 800;
        public int followRange { get; set; } = 800;

        public Hitbox AttackArea { get; set; }
        private Drawing Shadow { 
            get => Drawings["Shadow"];
            set => Drawings["Shadow"] = value;
        }
        public HealthBar HealthBar { get; set; }

        SlimeEnemy() { }
        public SlimeEnemy(float x, float y, Entity target, bool addToRoom = true) : base(addToRoom)
        {
            this.Hitbox = new Circle(x, y, 20);
            this.AttackArea = new Circle(X, y, 21);
            this.Mass = 3;
            this.Collision = true;
            this.Speed = 400;
            this.Health = 100;
            this.MaxHealth = Health;
            Damage = 1;
            this.Target = target;
            HealthBar = new HealthBar(this);
            Drawing = new Drawing(TextureName.Circle, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), Color.LightGreen, 0, new Vector2(Hitbox.Width, Hitbox.Height), 0.4f, true);
            Shadow = new Drawing(TextureName.Rectangle, new Vector2((float)Hitbox.Left, (float)Hitbox.Bottom + 1), Color.Black, 0, new Vector2(Hitbox.Width, 1f), 0.9f, true);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Jump(gameTime);
            HealthBar.Update(this);

            UpdateMovement(gameTime);
            Move(XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
            Move(YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);

            this.AttackArea.X = Hitbox.X;
            this.AttackArea.Y = Hitbox.Y;

            TryToAttack(gameTime);

            Drawing.Position = new Vector2((float)Hitbox.Left, (float)Hitbox.Top + yOffset);
            Shadow.Position = new Vector2((float)Hitbox.Left, (float)Hitbox.Bottom + 1);
            Drawing.Scale = new Vector2(Hitbox.Width, Hitbox.Height);
            Shadow.Scale = new Vector2(Hitbox.Width, 1f);
        }

        private void Jump(GameTime gameTime)
        {
            if(JumpCooldownLeft <= 0)
            {
                JumpProgress += gameTime.ElapsedGameTime.TotalSeconds / 1;
                if(JumpProgress <= 0.5)
                {
                    JumpSpeedMultiplier = JumpProgress;
                    yOffset = (float)JumpProgress * -10;
                }
                else if(JumpProgress <= 1)
                {
                    JumpSpeedMultiplier = 1 - JumpProgress;
                    yOffset = (float)(1 - JumpProgress) * -10;
                }
                else
                {
                    JumpSpeedMultiplier = 0;
                    JumpCooldownLeft = BaseJumpCooldown + JumpCooldownRandomness;
                    JumpProgress = 0;
                }
                
            }
            else
            {
                JumpCooldownLeft -= gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        private void UpdateMovement(GameTime gameTime)
        {
            CheckForTarget();
            double xedge = Hitbox.X - Target.Hitbox.X;
            double yedge = Hitbox.Y - Target.Hitbox.Y;
            XMovement = (-xedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed) * JumpSpeedMultiplier);
            YMovement = (-yedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed) * JumpSpeedMultiplier);
            if (Math.Abs(xedge) < Hitbox.Width)
                XMovement = 0;
            if (Math.Abs(yedge) < Hitbox.Height)
                YMovement = 0;

            if (Math.Abs(knockbackX) > 1)
                knockbackX -= knockbackX / Math.Abs(knockbackX) * 10;
            else
                knockbackX = 0;
            if (Math.Abs(knockbackY) > 1)
                knockbackY -= knockbackY / Math.Abs(knockbackY) * 10;
            else
                knockbackY = 0;
            XMovement += knockbackX;
            YMovement += knockbackY;

        }
        protected void CheckForTarget()
        {
            if(Target is NoBrainEntity)
            {
                foreach (var target in EntityTracker.GetEntities<Player>())
                {
                    if (Hitbox.Distance(target.Hitbox) < detectionRange)
                    {
                        Target = target;
                        aggresive = true;
                        return;
                    }
                }
            }
            if (Target.IsDead)
            {
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
        private void TryToAttack(GameTime gameTime)
        {
            AttackCooldownLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (AttackArea.CollisionDetect(Target.Hitbox) != Vector2.Zero && AttackCooldownLeft <= 0f)
            {
                Target.DamageSelf(Damage, this);
                AttackCooldownLeft = AttackCooldown;
            }
        }
        public override bool DamageSelf(int damage, Entity source, DamageType damageType = DamageType.Unknown)
        {
            if (source != null)
            {
                if (source.owner is Player)
                {
                    if(source is Projectile)
                    {
                        var effects = source.Effects.FindAll(eff => eff.IsPassable);
                        effects.ForEach(eff => eff.IsActive = true);
                        effects.ForEach(eff => eff.IsPassable = false);
                        foreach (var effect in effects)
                        {
                            AddEffect(effect);
                        }
                    }
                    Health -= damage;
                }
            }
            else
            {
                if(damageType == DamageType.Fire)
                {
                    Health -= damage;
                }
            }
            if (Health <= 0)
            {
                if (source != null && source.owner is Player)
                    Globals.HUD.points += 1;
                Globals.HUD.EnemiesLeft -= 1;
                Kill();
            }
            return true;
        }

        public override void Kill()
        {
            base.Kill();
            Globals.Drawings.Remove(Shadow);
            HealthBar.UnLoad();
        }
        public override void Load()
        {
            HealthBar.Load();
            Shadow.Enable();
            base.Load();
        }
        public override void UnLoad()
        {
            HealthBar.UnLoad();
            Shadow.Disable();
            base.UnLoad();
        }
    }
}

﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class SlimeEnemy : Entity
    {
        private int Damage;
        private float AttackCooldown = 0.1f;
        private float AttackCooldownLeft = 0f;
        private bool aggresive = true;

        private double JumpSpeedMultiplier = 1;
        private double JumpProgress = 0;
        private double JumpCooldown = 1.5;
        private double JumpCooldownLeft = 0;
        private float yOffset = 0f;

        private int detectionRange = 400;
        private int followRange = 800;

        private Entity Target { get; set; }
        private Hitbox AttackArea;
        private Drawing Shadow;

        public SlimeEnemy(float x, float y, Entity target)
        {
            this.Hitbox = new Circle(x, y, 20);
            this.AttackArea = new Circle(X, y, 21);
            this.Mass = 3;
            this.Collision = true;
            this.Speed = 400;
            this.Health = 100;
            this.MaxHealth = Health;
            Damage = 30;
            this.Target = target;
            new HealthBar(this);
            Drawing = new Drawing(TextureName.Circle, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), Color.LightGreen, 0, new Vector2(Hitbox.Width, Hitbox.Height), 0.4f, true);
            Shadow = new Drawing(TextureName.Rectangle, new Vector2((float)Hitbox.Left, (float)Hitbox.Bottom + 1), Color.Black, 0, new Vector2(Hitbox.Width, 1f), 0.9f, true);
        }
        public override void Update(GameTime gameTime)
        {
            Jump(gameTime);

            UpdateMovement();
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
                    JumpCooldownLeft = JumpCooldown;
                    JumpProgress = 0;
                }
                
            }
            else
            {
                JumpCooldownLeft -= gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        private void UpdateMovement()
        {
            CheckForTarget();
            if(!IsDead)
            {
                double xedge = Hitbox.X - Target.Hitbox.X;
                double yedge = Hitbox.Y - Target.Hitbox.Y;
                XMovement = (-xedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed) * JumpSpeedMultiplier);
                YMovement = (-yedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed) * JumpSpeedMultiplier);
                if (Math.Abs(xedge) < Hitbox.Width)
                    XMovement = 0;
                if (Math.Abs(yedge) < Hitbox.Height)
                    YMovement = 0;
            }

        }
        protected void CheckForTarget()
        {
            if (Target.IsDead)
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
        public override bool DamageSelf(int damage, Entity source)
        {
            if(source is Player)
                Health -= damage;
            if (Health <= 0)
            {
                if (source is Player)
                    Globals.HUD.points += 1;
                Globals.HUD.enemiesLeft -= 1;
                Kill();
            }
            return true;
        }

        public override void Kill()
        {
            base.Kill();
            Globals.Drawings.Remove(Shadow);
        }
    }
}

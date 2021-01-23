using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Player : Entity
    {
        private float PrimaryRateOfFire;
        private float PrimaryCooldown = 0f;
        private float SecondaryRateOfFire;
        private float SecondaryCooldown = 0f;

        private readonly float startingRadius;
        private readonly int minRadius = 30;
        public Player(/*Texture2D texture*/)
        {
            this.Mass = 10;
            this.Collision = true;
            this.Speed = 1 / 200f;
            //this.Texture = texture;
            this.Hitbox = new Circle(100, 100, 100);
            startingRadius = 100;
            //this.LayerDepth = 0.1f;
            this.MaxHealth = 2000;
            this.Health = 2000;
            CreateHealthBar();
            Drawing = new Drawing(TextureName.Circle, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), Color.Red, 0f,
                new Vector2(100, 100), 0.1f, true);
            Hotbar = new Hotbar(this);
            Hotbar.Add(new Pistol(), 0);
            Hotbar.Add(new Pistol(10, 0.1f, "mini"), 1);
            //Primary = Hotbar.Get(0);
            Hotbar.Selected = 0;
        }
        public enum Weapon
        {
            Pistol,
            Minigun,
            Sword
        }

        public IItem Primary { get => Hotbar.Get(Hotbar.Selected); }
        public Weapon Secondary { get; set; } = Weapon.Sword;

        public Hotbar Hotbar;

        public override void Update(GameTime gameTime)
        {
            PrimaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            SecondaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var projectile in EntityTracker.GetEntities<Projectile>())
            {
                if (CollidesWith(projectile) && !projectile.immuneEntities.Contains(this))
                {
                    DamageEntity(projectile.Damage, "Projectile");
                    projectile.immuneEntities.Add(this);
                    projectile.Kill();
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
            Drawing.Position = new Vector2((float)Hitbox.Left, (float)Hitbox.Top);
            if (IsDead)
                Globals.Drawings.Remove(Drawing);
        }
        public void PrimaryAttack()
        {
            if (PrimaryCooldown > Primary.Cooldown)
            {
                Primary.OnUse(this);
                PrimaryCooldown = 0;
            }
            //switch (Primary)
            //{
            //    case Weapon.Pistol:
            //        PrimaryRateOfFire = 0.3f;
            //        if (PrimaryCooldown > PrimaryRateOfFire)
            //        {
            //            MouseState mstate = Mouse.GetState();
            //            EntityTracker.Add.Projectile(TextureName.Rectangle, 500f, new Vector2(X, Y), new Vector2(mstate.X, mstate.Y), 20).immuneEntities.Add(this);

            //            PrimaryCooldown = 0;
            //        }
            //        break;
            //    case Weapon.Minigun:
            //        PrimaryRateOfFire = 0.1f;
            //        if (PrimaryCooldown > PrimaryRateOfFire)
            //        {
            //            MouseState mstate = Mouse.GetState();
            //            EntityTracker.Add.Projectile(TextureName.Rectangle, 500f, new Vector2(X, Y), new Vector2(mstate.X, mstate.Y), 4).immuneEntities.Add(this);

            //            PrimaryCooldown = 0;
            //        }
            //        break;
            //}
        }
        public void SecondaryAttack()
        {
            switch (Secondary)
            {
                case Weapon.Sword:
                    SecondaryRateOfFire = 0.3f;
                    if (SecondaryCooldown > SecondaryRateOfFire)
                    {
                        MouseState mstate = Mouse.GetState();
                        double yEdge = (Y - mstate.Y);
                        double xEdge = (X - mstate.X);
                        EntityTracker.Add.Sword(TextureName.Rectangle, this, (float)Math.Atan2(yEdge, xEdge), 30).immuneEntities.Add(this);

                        SecondaryCooldown = 0;
                    }
                    break;
            }
        }
        public override bool DamageEntity(int damage, string source)
        {
            Health -= damage;
            //Hitbox.Width = (int)((startingRadius - minRadius) * ((float)Health / MaxHealth)) + minRadius;
            if (Health <= 0)
            {
                Kill();
            }
            return true;
        }
        private void CreateHealthBar()
        {
            new HealthBar(this);
        }
    }
}

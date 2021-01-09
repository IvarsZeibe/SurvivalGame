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
        public enum Weapon
        {
            Pistol,
            Minigun,
            Sword
        }
        private float PrimaryRateOfFire;
        private float PrimaryCooldown = 0f;
        private float SecondaryRateOfFire;
        private float SecondaryCooldown = 0f;

        public Weapon Primary { get; set; } = Weapon.Pistol;
        public Weapon Secondary { get; set; } = Weapon.Sword;
        public Player(Texture2D texture)
        {
            this.Mass = 10;
            this.Collision = true;
            this.Speed = 1 / 200f;
            this.Texture = texture;
            this.Hitbox = new Circle(100, 100, 100);
            this.LayerDepth = 0.1f;
            this.Health = 30000;
        }
        public override void Update(GameTime gameTime)
        {
            PrimaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            SecondaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var projectile in EntityTracker.Projectiles)
            {
                if (CollidesWith(projectile) && !projectile.immuneEntities.Contains(this))
                {
                    DamageEntity(projectile.Damage, "Projectile");
                    projectile.immuneEntities.Add(this);
                    projectile.IsDead = true;
                }
            }
            foreach (var sword in EntityTracker.Swords)
            {
                if (sword.CollidesWith(this) && !sword.immuneEntities.Contains(this))
                {
                    DamageEntity(sword.Damage, "Sword");
                    sword.immuneEntities.Add(this);
                }
            }
        }
        public void PrimaryAttack()
        {
            switch (Primary)
            {
                case Weapon.Pistol:
                    PrimaryRateOfFire = 0.3f;
                    if (PrimaryCooldown > PrimaryRateOfFire)
                    {
                        MouseState mstate = Mouse.GetState();
                        EntityTracker.Add.Projectile(Game1.textures["Rectangle"], 500f, new Vector2(X, Y), new Vector2(mstate.X, mstate.Y), 200).immuneEntities.Add(this);

                        PrimaryCooldown = 0;
                    }
                    break;
                case Weapon.Minigun:
                    PrimaryRateOfFire = 0.1f;
                    if (PrimaryCooldown > PrimaryRateOfFire)
                    {
                        MouseState mstate = Mouse.GetState();
                        EntityTracker.Add.Projectile(Game1.textures["Rectangle"], 500f, new Vector2(X, Y), new Vector2(mstate.X, mstate.Y), 200).immuneEntities.Add(this);

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
                        MouseState mstate = Mouse.GetState();
                        double yEdge = (Y - mstate.Y);
                        double xEdge = (X - mstate.X);
                        EntityTracker.Add.Sword(Game1.textures["Rectangle"], this, (float)Math.Atan2(yEdge, xEdge)).immuneEntities.Add(this);

                        SecondaryCooldown = 0;
                    }
                    break;
            }
        }
        public override bool DamageEntity(int damage, string source)
        {
            Health -= damage;
            if (Health / 1000 != (Health + damage) / 1000)
            {
                Hitbox.Width = Hitbox.Width - 2;
            }
            if (Health <= 0)
            {
                IsDead = true;
            }
            return true;
        }
    }
}

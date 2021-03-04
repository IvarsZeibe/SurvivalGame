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
        private float PrimaryCooldown = 0f;
        private float SecondaryCooldown = 0f;

        private readonly int radius = 40;

        private readonly float startingRadius;
        //private readonly int minRadius = 30;
        public Player()
        {
            this.Mass = 10;
            this.Collision = true;
            this.Speed = 1 / 200f;
            this.Hitbox = new Circle(Globals.graphics.PreferredBackBufferWidth / 2 - radius/2, Globals.graphics.PreferredBackBufferHeight / 2 - radius / 2, radius);
            this.MaxHealth = 2000;
            this.Health = 2000;
            CreateHealthBar();
            Drawing = new Drawing(TextureName.Circle, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), Color.Red, 0f,
                new Vector2(radius, radius), 0.4f, true);
            Hotbar = Globals.HUD.hotbar;
            Hotbar.Add(new Pistol(), 0);
            Hotbar.Add(new Pistol(5, 0.1f, "mini"), 1);
            Hotbar.Add(new SwordItem(), 2);
            Hotbar.Add(new BlockItem(), 3);
            Hotbar.Selected = 0;
        }
        public IItem EquipedItem { get => Hotbar.Get(Hotbar.Selected); }
        public HUD.Hotbar Hotbar;

        public override void Update(GameTime gameTime)
        {
            PrimaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            SecondaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
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
            if (IsDead)
                Globals.Drawings.Remove(Drawing);
        }
        public void UsePrimary()
        {
            if (PrimaryCooldown > EquipedItem.Cooldown)
            {
                EquipedItem.OnPrimaryUse(this);
                if (EquipedItem.Successful)
                    PrimaryCooldown = 0;
            }
        }
        public void UseSecondary()
        {
            if (SecondaryCooldown > EquipedItem.Cooldown)
            {
                EquipedItem.OnSecondaryUse(this);
                if (EquipedItem.Successful)
                    SecondaryCooldown = 0;
            }
        }
        public override bool DamageSelf(int damage, Entity source)
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

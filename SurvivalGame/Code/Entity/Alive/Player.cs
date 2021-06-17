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
        private HealthBar HealthBar;

        public Player()
        {
            this.Mass = 10;
            this.Collision = true;
            this.Speed = 1 / 200f;
            this.Hitbox = new Circle(Globals.graphics.PreferredBackBufferWidth / 2 - radius/2, Globals.graphics.PreferredBackBufferHeight / 2 - radius / 2, radius);
            this.MaxHealth = 1000;
            this.Health = 1000;
            Target = Globals.MouseCursor;
            HealthBar = new HealthBar(this);
            Drawing = new Drawing(TextureName.Circle, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), Color.Red, 0f,
                new Vector2(radius, radius), 0.4f);
            Drawings.Add("base", Drawing);

            Hotbar = Globals.HUD.hotbar;
            Hotbar.Selected = 0;

        }
        public IItem EquipedItem { get => Hotbar.Get(Hotbar.Selected); }
        public Hotbar Hotbar;

        public override void Update(GameTime gameTime)
        {
            PrimaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            SecondaryCooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var projectile in EntityTracker.GetEntities<Projectile>())
            {
                if (CollidesWith(projectile) && !projectile.immuneEntities.Contains(this))
                {
                    DamageSelf(projectile.Damage, projectile);
                    projectile.immuneEntities.Add(this);
                    projectile.Kill();
                }
            }
            foreach (var sword in EntityTracker.GetEntities<Sword>())
            {
                if (sword.CollidesWith(this) && !sword.immuneEntities.Contains(this))
                {
                    DamageSelf(sword.Damage, sword);
                    sword.immuneEntities.Add(this);
                }
            }
            Drawing.Position = new Vector2((float)Hitbox.Left, (float)Hitbox.Top);
            Drawing.LayerDepth = 0.4f - (float)Y / 100000;
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
        public override bool DamageSelf(int damage, Entity source, DamageType damageType = DamageType.Unknown)
        {
            Health -= damage;
            //Hitbox.Width = (int)((startingRadius - minRadius) * ((float)Health / MaxHealth)) + minRadius;
            if (Health <= 0)
            {
                Kill();
            }
            return true;
        }
        public override void Load()
        {
            base.Load();
            HealthBar.Load();
        }
        public override void UnLoad()
        {
            base.UnLoad();
            HealthBar.UnLoad();
        }
        public void Revive()
        {
            IsDead = false;
            Globals.Drawings.Add(Drawing);
            EntityTracker.Entities.Add(this);
            Health = MaxHealth;
            HealthBar.IsDead = false;
            Globals.Drawings.Add(HealthBar.Drawing);
            HealthBar.UpdateEnabled = true;
            Globals.Updatables.Add(HealthBar);
        }
    }
}

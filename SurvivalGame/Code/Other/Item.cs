using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    interface IItem
    {
        public string Name { get; set; }
        //public float Damage { get; }
        public float Cooldown { get; set; }
        public TextureName TextureName { get; }
        public Color Color {get;set; }
        public bool Successful { get; set; }
        public void OnPrimaryUse(Entity owner) { }
        public void OnSecondaryUse(Entity owner) { }
        public Hitbox Hitbox { get; set; }
    }
    class EmptyItem : IItem
    {
        public string Name { get; set; } = "Air";
        public float Damage { get; } = 0f;
        public float Cooldown { get; set; } = 0f;
        public TextureName TextureName { get; } = TextureName.Rectangle;
        public Color Color { get; set; } = Color.Transparent;
        public bool Successful { get; set; } = true;
        public Hitbox Hitbox { get; set; } = new Rect(0, 0, 0, 0);

    }
    class Pistol : IItem
    {
        public Pistol(float damage = 20f, float cooldown = 0.3f, string name = "pistol", Color? color = null, float bulletVelocity = 500f)
        {
            Damage = damage;
            Cooldown = cooldown;
            this.bulletVelocity = bulletVelocity;
            Name = name;
            color ??= Color.White;
            Color = (Color)color;
        }
        float bulletVelocity;
        public string Name { get; set; }
        public float Damage { get; }
        public float Cooldown { get; set; }
        public TextureName TextureName { get; } = TextureName.PistolItem;
        public Color Color { get; set; } = Color.Black;
        public bool Successful { get; set; } = true;
        public Hitbox Hitbox { get; set; }
        public void OnPrimaryUse(Entity owner)
        {
            MouseState mstate = Mouse.GetState();
            new Projectile(owner, TextureName.Rectangle, bulletVelocity, new Vector2(owner.X, owner.Y), new Vector2(owner.Target.X, owner.Target.Y), (int)Damage).immuneEntities.Add(owner);
        }
    }
    class SwordItem : IItem
    {
        public SwordItem(float damage = 30f, float cooldown = 0.3f, string name = "sword", Color? color = null, float knockbackStrenght = 100)
        {
            Damage = damage;
            Cooldown = cooldown;
            Name = name;
            color ??= Color.White;
            Color = (Color)color;
            this.knockbackStrenght = knockbackStrenght;
        }
        public float knockbackStrenght { get; set; }
        public string Name { get; set; }
        public float Damage { get; }
        public float Cooldown { get; set; }
        public TextureName TextureName { get; } = TextureName.SwordItem;
        public Color Color { get; set; } = Color.Black;
        public bool Successful { get; set; } = true;
        public Hitbox Hitbox { get; set; }
        public void OnPrimaryUse(Entity owner)
        {
            MouseState mstate = Mouse.GetState();
            double yEdge = (owner.Y - owner.Target.Y);
            double xEdge = (owner.X - owner.Target.X);
            new Sword(TextureName.Rectangle, owner, (float)Math.Atan2(yEdge, xEdge), (int)Damage, knockbackStrenght).immuneEntities.Add(owner);
        }
    }
    class BlockItem : IItem
    {
        public BlockItem(float cooldown = 0.3f, string name = "block", Color? color = null)
        {
            Cooldown = cooldown;
            Name = name;
            color ??= Color.SaddleBrown;
            Color = (Color)color;
        }
        public string Name { get; set; }
        public float Cooldown { get; set; }
        public TextureName TextureName { get; } = TextureName.Rectangle;
        public Color Color { get; set; } = Color.Black;
        public bool Successful { get; set; }
        public Hitbox Hitbox { get; set; }
        public void OnPrimaryUse(Entity owner)
        {
            MakeWall();
        }
        public void OnSecondaryUse(Entity owner)
        {
            DeleteWall();
        }
        private MouseState mstate { get => Mouse.GetState(); }
        void MakeWall()
        {
            var wall = new Wall(TextureName.Rectangle, mstate.X, mstate.Y);

            bool suitableSpot = true;
            foreach (var entity in EntityTracker.Entities)
            {
                if (entity is MouseCursor)
                    continue;
                if (wall.CollidesWith(entity) && entity != wall)
                {
                    suitableSpot = false;
                }
            }
            if (!suitableSpot)
            {
                wall.Kill();
                MakeGhost();
                Successful = false;
            }
            else
            {
                Successful = true;
            }
        }
        void MakeGhost()
        {
            var wallGhost = new Wall(TextureName.Rectangle, mstate.X, mstate.Y, false);

            bool intersects = false;
            foreach (var w in EntityTracker.GetEntities<Wall>())
            {
                if (!w.Collision && wallGhost.CollidesWith(w) && w != wallGhost)
                {
                    intersects = true;
                }
            }
            if (intersects)
            {
                wallGhost.Kill();
                EntityTracker.Entities.Remove(wallGhost);
            }
        }
        void DeleteWall()
        {
            foreach (var wall in EntityTracker.GetEntities<Wall>())
            {
                if (wall.Collision && wall.CollidesWith(Globals.MouseCursor))
                {
                    wall.Kill();
                    break;
                }
            }
            Successful = true;
        } 
    }
    class RPG : IItem
    {
        public RPG(int _damage = 60, bool _trackEnemy = true, float speed = 500f, int _range = 800, float cooldown = 1f, string name = "rpg", Color? color = null)
        {
            range = _range;
            Damage = _damage;
            trackEnemy = _trackEnemy;
            Cooldown = cooldown;
            this.bulletVelocity = speed;
            Name = name;
            color ??= Color.White;
            Color = (Color)color;
        }
        bool trackEnemy;
        float bulletVelocity;
        int range;
        public string Name { get; set; }
        public int Damage { get; }
        public float Cooldown { get; set; }
        public TextureName TextureName { get; } = TextureName.RPG;
        public Color Color { get; set; } = Color.Black;
        public bool Successful { get; set; } = true;
        public Hitbox Hitbox { get; set; }
        public void OnPrimaryUse(Entity owner)
        {
            new Missile(owner, new Vector2(owner.X, owner.Y), trackEnemy, owner.Target, Damage, bulletVelocity, range).ImmuneEntities.Add(owner);
        }
    }
    class Shotgun : IItem
    {
        public Shotgun(float damage = 20f, float cooldown = 0.3f, string name = "shotgun", int bulletCount = 3, float spread = 0, float bulletVelocity = 500f)
        {
            Damage = damage;
            Cooldown = cooldown;
            this.bulletVelocity = bulletVelocity;
            Name = name;

        }
        float bulletVelocity;
        public string Name { get; set; }
        public float Damage { get; }
        public float Cooldown { get; set; }
        public TextureName TextureName { get; } = TextureName.PistolItem;
        public Color Color { get; set; } = Color.Black;
        public bool Successful { get; set; } = true;
        public Hitbox Hitbox { get; set; }
        public void OnPrimaryUse(Entity owner)
        {
            MouseState mstate = Mouse.GetState();
            new Projectile(owner, TextureName.Rectangle, bulletVelocity, new Vector2(owner.X, owner.Y), new Vector2(owner.Target.X, owner.Target.Y), (int)Damage).immuneEntities.Add(owner);
            new Projectile(owner, TextureName.Rectangle, bulletVelocity, new Vector2(owner.X, owner.Y), new Vector2(owner.Target.X, owner.Target.Y), (int)Damage, 0.4f).immuneEntities.Add(owner);
            new Projectile(owner, TextureName.Rectangle, bulletVelocity, new Vector2(owner.X, owner.Y), new Vector2(owner.Target.X, owner.Target.Y), (int)Damage, -0.4f).immuneEntities.Add(owner);
        }
    }
    class AxeItem : IItem
    {
        public AxeItem(float cooldown = 0.3f, string name = "axe", Color? color = null)
        {
            Cooldown = cooldown;
            Name = name;
            color ??= Color.White;
            Color = (Color)color;
        }
        public string Name { get; set; }
        public float Damage { get; }
        public float Cooldown { get; set; }
        public TextureName TextureName { get; } = TextureName.AxeItem;
        public Color Color { get; set; } = Color.Black;
        public bool Successful { get; set; } = true;
        public Hitbox Hitbox { get; set; }
        public void OnPrimaryUse(Entity owner)
        {
            MouseState mstate = Mouse.GetState();
            double yEdge = (owner.Y - owner.Target.Y);
            double xEdge = (owner.X - owner.Target.X);
            new Axe(owner, owner.Target.Hitbox.GetPosVector());
        }
    }
    class FlamethrowerItem : IItem
    {
        public FlamethrowerItem(float damage = 5f, float cooldown = 0.02f, string name = "flamethrower", float bulletVelocity = 250f, int bulletSpeedVariation = 70, int spread = 7)
        {
            Damage = damage;
            Cooldown = cooldown;
            this.bulletVelocity = bulletVelocity;
            this.bulletSpeedVariation = bulletSpeedVariation;
            this.spread = spread;
            Name = name;

        }
        float bulletVelocity;
        int bulletSpeedVariation;
        int spread;
        int range = 200;
        public string Name { get; set; }
        public float Damage { get; }
        public float Cooldown { get; set; }
        public TextureName TextureName { get; } = TextureName.PistolItem;
        public Color Color { get; set; } = Color.Black;
        public bool Successful { get; set; } = true;
        public Hitbox Hitbox { get; set; }
        public void OnPrimaryUse(Entity owner)
        {
            MouseState mstate = Mouse.GetState();
            float angle = Globals.rand.Next(-spread, spread) / 100f * (float)Math.PI;
            var projectile = new Projectile(owner, TextureName.Rectangle, bulletVelocity + Globals.rand.Next(-bulletSpeedVariation, bulletSpeedVariation), new Vector2(owner.X, owner.Y), new Vector2(owner.Target.X, owner.Target.Y), (int)Damage, angle, range);
            projectile.immuneEntities.Add(owner);
            projectile.Drawing.Color = Color.Orange;
            projectile.effects.Add(new OnFire(1, 10));
        }
    }
}

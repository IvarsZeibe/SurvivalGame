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
        public Pistol(float damage = 20f, float cooldown = 0.3f, string name = "pistol", Color? color = null)
        {
            Damage = damage;
            Cooldown = cooldown;
            Name = name;
            color ??= Color.White;
            Color = (Color)color;
        }
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
            new Projectile(TextureName.Rectangle, 500f, new Vector2(owner.X, owner.Y), new Vector2(mstate.X, mstate.Y), (int)Damage).immuneEntities.Add(owner);
        }
    }
    class SwordItem : IItem
    {
        public SwordItem(float damage = 30f, float cooldown = 0.3f, string name = "sword", Color? color = null)
        {
            Damage = damage;
            Cooldown = cooldown;
            Name = name;
            color ??= Color.White;
            Color = (Color)color;
        }
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
            double yEdge = (owner.Y - mstate.Y);
            double xEdge = (owner.X - mstate.X);
            new Sword(TextureName.Rectangle, owner, (float)Math.Atan2(yEdge, xEdge), (int)Damage).immuneEntities.Add(owner);
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
                if (wall.Collision && wall.CollidesWith(EntityTracker.GetEntities<MouseCursor>()[0]))
                {
                    wall.Kill();
                    break;
                }
            }
            Successful = true;
        }
    }
}

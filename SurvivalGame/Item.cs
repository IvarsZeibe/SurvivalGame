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
        public float Damage { get; }
        public float Cooldown { get; set; }
        public TextureName TextureName { get; }
        public Color Color {get;set;}
        public void OnUse(Entity owner) { }
    }
    class EmptyItem : IItem
    {
        public string Name { get; set; } = "Air";
        public float Damage { get; } = 0f;
        public float Cooldown { get; set; } = 0f;
        public TextureName TextureName { get; } = TextureName.Rectangle;
        public Color Color { get; set; } = Color.Transparent;
        public void OnUse(Entity owner)
        {
        }

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
        public void OnUse(Entity owner)
        {
            MouseState mstate = Mouse.GetState();
            EntityTracker.Add.Projectile(TextureName.Rectangle, 500f, new Vector2(owner.X, owner.Y), new Vector2(mstate.X, mstate.Y), (int)Damage).immuneEntities.Add(owner);
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
        public void OnUse(Entity owner)
        {
            MouseState mstate = Mouse.GetState();
            double yEdge = (owner.Y - mstate.Y);
            double xEdge = (owner.X - mstate.X);
            EntityTracker.Add.Sword(TextureName.Rectangle, owner, (float)Math.Atan2(yEdge, xEdge), (int)Damage).immuneEntities.Add(owner);
        }
    }
}

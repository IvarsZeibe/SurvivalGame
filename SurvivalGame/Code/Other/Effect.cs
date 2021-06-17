using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    abstract class Effect
    {
        public virtual void Apply(GameTime gameTime) { }
        public string Name { get; set; }
        public int Strength { get; set; }
        public float Duration { get; set; }
        public Entity Owner { get; set; }
        public virtual void Remove() { }
        public Vector2 flameSize = new Vector2(0.5f, 1f);
    }
    class OnFire : Effect
    {
        Drawing drawing { get; set; }
        float attackRate = 0.5f;
        float sinceAttack = 0f;
        public OnFire(int strength, int duration, Entity Owner = null)
        {
            Name = "OnFire";
            Strength = strength;
            Duration = duration;
            this.Owner = Owner;
        }

        public override void Apply(GameTime gameTime)
        {
            if (Owner is null)
                return;
            if (!Owner.Drawings.ContainsKey(Name))
            {
                Vector2 scale = new Vector2(Owner.Drawings["base"].GetWidth(), Owner.Drawings["base"].GetHeight()) * flameSize;
                drawing = new Drawing(TextureName.fire, Vector2.Zero, Color.White, 0f, scale, Owner.Drawings["base"].LayerDepth - 0.01f, false) 
                {
                    originPercentage = new Vector2(0.5f, 0.5f)
                };
                Owner.Drawings.Add(Name, drawing);
                drawing.Enable();
                Owner.Animations.Add("fire", new FireAnimation(drawing));
            }
            else
            {
                drawing = Owner.Drawings[Name];
                drawing.Enable();
            }

            Duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(sinceAttack >= attackRate)
            {
                Owner.DamageSelf(Strength, null, DamageType.Fire);
                sinceAttack = 0f;
            }
            else
                sinceAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
            var i = Vector2.Transform(new Vector2(0, 1), Matrix.CreateRotationZ(3.14f/2));
            Vector2 rotationOffset = (Vector2.Transform(new Vector2(0, -0.5f), Matrix.CreateRotationZ(Owner.Drawings["base"].Rotation))) * Owner.Drawings["base"].Size;
            drawing.Coord = Owner.Drawings["base"].Position + Owner.Drawings["base"].Size * 0.5f - (Owner.Drawings["base"].originPercentage - new Vector2(0, 0.5f)) * Owner.Drawings["base"].Size + rotationOffset;
            Animation animation;
            Owner.Animations.TryGetValue("fire", out animation);
            if (animation != null)
                drawing.Rotation = Owner.Drawings["base"].Rotation + (Owner.Animations["fire"] as FireAnimation).rotation;
            else
                drawing.Rotation = Owner.Drawings["base"].Rotation;
        }
        public override void Remove()
        {
            Owner.ActiveEffects.Remove(this);
            Globals.Drawings.Remove(drawing);
            Owner.Drawings.Remove(Name);
            Owner.Animations.Remove("fire");
        }
    }
}

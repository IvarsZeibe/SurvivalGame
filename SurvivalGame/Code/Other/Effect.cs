using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    interface IEffect
    {
        public void Apply(GameTime gameTime);
        public string Name { get; }
        public int Strength { get; set; }
        public float Duration { get; set; }
        public Entity Owner { get; set; }
        public void Remove();
    }
    class OnFire : IEffect
    {
        public Entity Owner { get; set; }
        public string Name { get; } = "OnFire";
        public int Strength { get; set; }
        public float Duration { get; set; }
        Drawing drawing { get; set; } = new Drawing(TextureName.fire, Vector2.Zero, Color.White, 0f, new Vector2(10, 10), 0.39f, false) {originPercentage = new Vector2(0.5f, 0.5f) };
        float attackRate = 0.5f;
        float sinceAttack = 0f;
        public OnFire(int strength, int duration, Entity Owner = null)
        {
            Strength = strength;
            Duration = duration;
            this.Owner = Owner;
            Globals.Drawings.Remove(drawing);
            //if (owner.Drawings.ContainsKey(Name))
            //{
            //    Globals.Drawings.Remove(owner.Drawings[Name]);
            //    owner.Drawings[Name] = drawing;
            //}
            //else
            //    owner.Drawings.Add(Name, drawing);
        }

        public void Apply(GameTime gameTime)
        {
            if (Owner is null)
                return;
            if (!Owner.Drawings.ContainsKey(Name))
            {
                drawing.Scale = new Vector2(Owner.Drawings["base"].GetWidth(), Owner.Drawings["base"].GetHeight()) * new Vector2(0.6f, 1.2f);
                Owner.Drawings.Add(Name, drawing);
            }
            else
            {
                drawing = Owner.Drawings[Name];
            }
            if (!Globals.Drawings.Contains(drawing))
            {
                Globals.Drawings.Add(drawing);
            }

            Duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(sinceAttack >= attackRate)
            {
                Owner.DamageSelf(Strength, null, DamageType.Fire);
                sinceAttack = 0f;
            }
            else
                sinceAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;

            drawing.Coord = Owner.Drawings["base"].Position + new Vector2(Owner.Drawings["base"].GetWidth(), Owner.Drawings["base"].GetHeight()) * 0.5f;
            drawing.Rotation = Owner.Drawings["base"].Rotation;
            //drawing.Coord = Owner.Hitbox.GetTopLeftPosVector();
            drawing.IsDrawn = true;
        }
        public void Remove()
        {
            Owner.ActiveEffects.Remove(this);
            Globals.Drawings.Remove(drawing);
            Owner.Drawings.Remove(Name);
        }
    }
}

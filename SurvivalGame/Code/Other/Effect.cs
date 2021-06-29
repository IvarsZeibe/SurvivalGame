using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    abstract class Effect
    {
        public virtual void Apply(GameTime gameTime, Entity Owner = null) { }
        public string Name { get; set; }
        public int Strength { get; set; }
        public float Duration { get; set; }
        public bool IsPassable { get; set; }
        public bool IsActive { get; set; }
        public bool IsDead { get; set; } = false;
        public Drawing Drawing { get; set; }
        public Animation Animation { get; set; }
        public virtual void UnLoad() { }
        public virtual void Kill() { }
        public Vector2 Size { get; set; } = new Vector2(0.5f, 1f);
    }
    class OnFire : Effect
    {
        public float attackRate { get; set; } = 0.5f;
        public float sinceAttack { get; set; } = 0f;
        OnFire() { }
        public OnFire(int strength, int duration, bool isActive = true)
        {
            Name = "OnFire";
            Strength = strength;
            Duration = duration;
            IsPassable = true;
            IsActive = isActive;
            Drawing = new Drawing(TextureName.fire, Vector2.Zero, Color.White, 0f, Vector2.Zero, 0.3f, false)
            {
                originPercentage = new Vector2(0.5f, 0.5f)
            };
            Animation = new FireAnimation(Drawing);
        }

        public override void Apply(GameTime gameTime, Entity Owner = null)
        {
            Duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(Duration <= 0)
            {
                Kill();
                return;
            }
            bool flag = true;
            foreach(var effect in Owner.Effects)
            {
                if(effect.GetType() == GetType() && effect.Strength >= Strength
                    // Checks if it is not the same effect and allows 1 to execute if 2 effects have the same 
                    && Owner.Effects.IndexOf(effect) < Owner.Effects.IndexOf(this))
                {
                    if (!effect.IsDead)
                    {
                        flag = false;
                    }
                    else
                    {
                        Animation = effect.Animation;
                        Animation.Owner = Drawing;
                    }
                }
            }
            if (flag)
            {
                //foreach (var eff in Owner.Effects)
                //{
                //    var j = eff.GetType() == GetType();
                //    var k = eff.Strength >= Strength;
                //    var n = Owner.Effects.IndexOf(eff);
                //    var m = Owner.Effects.IndexOf(this);
                //    var l = n < m;
                //    if(j && k && l)
                //    {

                //    }
                //}
                Drawing.Scale = Owner.Drawings["base"].Size * Size;
                Drawing.LayerDepth = Owner.Drawings["base"].LayerDepth - 0.01f;
                Drawing.Enable();
                Animation.Update(gameTime);

                if (sinceAttack >= attackRate)
                {
                    Owner.DamageSelf(Strength, null, DamageType.Fire);
                    sinceAttack = 0f;
                }
                else
                    sinceAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
                var i = Vector2.Transform(new Vector2(0, 1), Matrix.CreateRotationZ(3.14f / 2));
                Vector2 rotationOffset = (Vector2.Transform(new Vector2(0, -0.5f), Matrix.CreateRotationZ(Owner.Drawings["base"].Rotation))) * Owner.Drawings["base"].Size;
                Drawing.Coord = Owner.Drawings["base"].Position + Owner.Drawings["base"].Size * 0.5f - (Owner.Drawings["base"].originPercentage - new Vector2(0, 0.5f)) * Owner.Drawings["base"].Size + rotationOffset;
                Drawing.Rotation = Owner.Drawings["base"].Rotation + (Animation as FireAnimation).rotation;
            }
            else
            {
                Drawing.Disable();
            }
            //if (Owner is null)
            //    return;
            //if (!Owner.Drawings.ContainsKey(Name))
            //{
            //    Vector2 scale = new Vector2(Owner.Drawings["base"].GetWidth(), Owner.Drawings["base"].GetHeight()) * flameSize;
            //    drawing = new Drawing(TextureName.fire, Vector2.Zero, Color.White, 0f, scale, Owner.Drawings["base"].LayerDepth - 0.01f, false) 
            //    {
            //        originPercentage = new Vector2(0.5f, 0.5f)
            //    };
            //    Owner.Drawings.Add(Name, drawing);
            //    drawing.Enable();
            //    Owner.Animations.Add("fire", new FireAnimation(drawing));
            //}
            //else
            //{
            //    drawing = Owner.Drawings[Name];
            //    drawing.Enable();
            //}

            //Duration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if(sinceAttack >= attackRate)
            //{
            //    Owner.DamageSelf(Strength, null, DamageType.Fire);
            //    sinceAttack = 0f;
            //}
            //else
            //    sinceAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //var i = Vector2.Transform(new Vector2(0, 1), Matrix.CreateRotationZ(3.14f/2));
            //Vector2 rotationOffset = (Vector2.Transform(new Vector2(0, -0.5f), Matrix.CreateRotationZ(Owner.Drawings["base"].Rotation))) * Owner.Drawings["base"].Size;
            //drawing.Coord = Owner.Drawings["base"].Position + Owner.Drawings["base"].Size * 0.5f - (Owner.Drawings["base"].originPercentage - new Vector2(0, 0.5f)) * Owner.Drawings["base"].Size + rotationOffset;
            //Animation animation;
            //Owner.Animations.TryGetValue("fire", out animation);
            //if (animation != null)
            //    drawing.Rotation = Owner.Drawings["base"].Rotation + (Owner.Animations["fire"] as FireAnimation).rotation;
            //else
            //    drawing.Rotation = Owner.Drawings["base"].Rotation;
        }
        public override void UnLoad()
        {
            Drawing.Disable();
        }
        public override void Kill()
        {
            //Owner.ActiveEffects.Remove(this);
            //Globals.Drawings.Remove(drawing);
            //Owner.Drawings.Remove(Name);
            //Owner.Animations.Remove("fire");
            UnLoad();
            IsDead = true;
        }
    }
}

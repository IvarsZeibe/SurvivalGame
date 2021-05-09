﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Tree : Entity
    {
        float drawingRotation = 0f;
        Color color = Color.White;
        public Tree(double x, double y) : base(false)
        {
            Collision = true;
            Mass = 19;
            Health = 100;
            Vector2 origin = new Vector2(0.5f, 1f);
            Hitbox = new Rect(x, y, 30, 30);
            Vector2 drawingSize = new Vector2(150, 150);
            Drawing = new Drawing(TextureName.PineTree, Hitbox.GetTopLeftPosVector(), color, drawingRotation, drawingSize, 0.3f - (float)y / 100000);
            {
                Drawing.originPercentage = origin;
                Drawing.Offset = -new Vector2(62, 115) + drawingSize * origin;
            }
            Drawings.Add("base", Drawing);
            Drawing drawing = new Drawing(TextureName.PineTreeOnFire, Vector2.Zero, Color.White, 0f, new Vector2(150, 150), Drawing.LayerDepth - 0.01f)
            {
                originPercentage = new Vector2(1f, 1.5f)
            };
            Drawings.Add("OnFire", drawing);
            Animations.Add("chopShake", new ShakeAnimation(Drawing));
            Animations.Add("despawn", new DisappearingAnimation(Drawing));
            Animations.Add("stayStill", new StillnessAnimation(Drawing));
            Animations.Add("fall", new FallAnimation(Drawing));
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateAnimations(gameTime);
            Drawing.Rotation = drawingRotation;
            Drawing.Color = color;
        }
        public override bool DamageSelf(int damage, Entity source, DamageType damageType = DamageType.Unknown)
        {
            if (source is Projectile)
            {
                foreach (var effect in (source as Projectile).effects)
                {
                    effect.Owner = this;
                }
                ActiveEffects.AddRange((source as Projectile).effects);
            }
            if (damageType == DamageType.Axe)
            {
                Health -= damage;

            }
            else if (damageType == DamageType.Fire)
            {
                if (damageType == DamageType.Fire)
                {
                    Health -= damage * 3;
                }
            }
            else
                return false;

            if (Health > 0)
            {
                if (Animations["chopShake"].Inactive)
                {
                    Animations["chopShake"].Start();
                }
            }
            else
            {
                Hitbox.Active = false;
                if (Animations["fall"].Inactive)
                {

                    if (source is null)
                    {
                        Kill();
                    }
                    else if (Hitbox.X > source.Hitbox.X)
                    {
                        Animations["fall"].Start();
                    }
                    else
                    {
                        (Animations["fall"] as FallAnimation).direction = Direction.Left;
                        Animations["fall"].Start();
                    }
                }

            }
            return true;
        }
        void UpdateAnimations(GameTime gameTime)
        {
            foreach(var animation in Animations)
            {
                animation.Value.Update(gameTime);
            }
            drawingRotation = Drawing.Rotation;
            color = Drawing.Color;
            if (Animations["fall"].Progress == 1 && Animations["stayStill"].Inactive)
            {
                Animations["stayStill"].Start();
            }
            if(Animations["stayStill"].Progress == 1)
            {
                Animations["despawn"].Start();
            }
        }
    }
}
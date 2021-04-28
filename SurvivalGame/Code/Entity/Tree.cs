using Microsoft.Xna.Framework;
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
            Drawing = new Drawing(TextureName.PineTree, Hitbox.GetTopLeftPosVector() - new Vector2(62, 115) + drawingSize * origin, color, drawingRotation, drawingSize, 0.3f - (float)y / 100000);
            {
                Drawing.originPercentage = origin;
            }
        }
        public override void Update(GameTime gameTime)
        {
            UpdateAnimations(gameTime);
            Drawing.Rotation = drawingRotation;
            Drawing.Color = color;
        }
        public override bool DamageSelf(int damage, Entity source, DamageType damageType = DamageType.Unknown)
        {
            if(damageType == DamageType.Axe)
            {
                Health -= damage;
                if(Health > 0)
                {
                    shakeLeft = SHAKE_LENGTH;
                }
                else
                {
                    Hitbox.Active = false;
                    if (Hitbox.X > source.Hitbox.X)
                    {
                        fallLeft = FALL_LENGHT;
                        fallDirection = Direction.Right;
                    }
                    else
                    {
                        fallLeft = FALL_LENGHT;
                        fallDirection = Direction.Left;
                    }
                }
                return true;
            }
            return false;
        }
        float stayFallenLeft = 0f;
        float STAY_FALLEN_LENGTH = 2f;
        void UpdateAnimations(GameTime gameTime)
        {
            if(shakeLeft > 0)
            {
                shakeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                AnimateShake();
            }
            if(fallLeft > 0)
            {
                fallLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                AnimateFall();
                if (fallLeft <= 0)
                    stayFallenLeft = STAY_FALLEN_LENGTH;
            }
            if(disappearingLeft > 0)
            {
                disappearingLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                AnimateDisappearing();
            }
            if(stayFallenLeft > 0)
            {
                stayFallenLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (stayFallenLeft <= 0)
                    disappearingLeft = DISAPPEARING_LENGTH;
            }
        }
        const float SHAKE_LENGTH = 0.6f;
        float shakeLeft = 0f;
        void AnimateShake()
        {
            float shakeCount = 2;
            int segments = 4;
            float animationLength = SHAKE_LENGTH / shakeCount;
            float animationPart = (shakeLeft % animationLength);
            int segment = (int)Math.Floor(animationPart / (animationLength / segments));
            float progress = 1 - animationPart / animationLength;
            if (progress < 0.25f)
                drawingRotation = progress * 0.3f;
            else if (progress < 0.5f)
                drawingRotation = (0.5f - progress) * 0.3f;
            else if (progress < 0.75f)
                drawingRotation = -(progress - 0.5f) * 0.3f;
            else if (progress < 1f)
                drawingRotation = -(1 - progress) * 0.3f;
        }
        const float FALL_LENGHT = 1.5f;
        float fallLeft = 0f;
        Direction fallDirection;
        void AnimateFall()
        {
            float progress = 1 - (fallLeft / FALL_LENGHT);
            if (fallDirection == Direction.Right)
                drawingRotation = 1.57f * progress * progress;
            else
                drawingRotation = -1.57f * progress * progress;
        }
        const float DISAPPEARING_LENGTH = 1f;
        float disappearingLeft = 0f;
        void AnimateDisappearing()
        {
            float progress = 1 - disappearingLeft / DISAPPEARING_LENGTH;
            color.A = Convert.ToByte(255 - 255 * progress);
            color.R = Convert.ToByte(255 - 255 * progress);
            color.G = Convert.ToByte(255 - 255 * progress);
            color.B = Convert.ToByte(255 - 255 * progress);
            if (progress >= 1)
                Kill();
        }
    }
}

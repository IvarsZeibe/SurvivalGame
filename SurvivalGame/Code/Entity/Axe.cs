using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Axe : Entity
    {
        float rotation = 0f;
        readonly float startRotation;
        const float LIFE_SPAN = 0.2f;
        float timeAlive = 0;
        int damage = 10;
        public Axe(Entity owner, Vector2 targetPos)
        {
            this.owner = owner;
            Hitbox = new Circle(owner.X, owner.Y, 30);
            float yEdge = owner.Y - targetPos.Y;
            float xEdge = owner.X - targetPos.X;
            startRotation = (float)Math.Atan2(yEdge, xEdge) - (float)Math.PI/3;
            rotation = startRotation;
            Vector2 direction = Vector2.Normalize(new Vector2(xEdge, yEdge));
            Vector2 positionOffset = -direction * Hitbox.Width;
            Hitbox.X += positionOffset.X;
            Hitbox.Y += positionOffset.Y;
            Drawing = new Drawing(TextureName.Axe, GetNewDrawingPosition(), Color.White, startRotation, new Vector2(15, Hitbox.Width*1.5f))
            {
                originPercentage = new Vector2(0.5f, 1f)
            };
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            rotation = (timeAlive / LIFE_SPAN) * -1.34f + startRotation;
            foreach (var entity in EntityTracker.Entities)
            {
                if(entity is Tree)
                {
                    if (Hitbox.CollidesWith(entity.Hitbox))
                    {
                        entity.DamageSelf(damage, this, DamageType.Axe);
                    }
                }
            }

            Drawing.Rotation = rotation;
            Drawing.Position = GetNewDrawingPosition();

            Hitbox.Active = false;
            timeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(timeAlive > LIFE_SPAN)
                Kill();
        }
        Vector2 GetNewDrawingPosition()
        {
            return owner.Hitbox.GetPosVector();
        }
    }
}

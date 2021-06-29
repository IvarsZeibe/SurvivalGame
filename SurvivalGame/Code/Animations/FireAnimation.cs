using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class FireAnimation : Animation
    {
        public Vector2 sizeChange { get; set; } = Vector2.Zero;
        public Animation shake { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public float rotation { get => (shake as ShakeAnimation).rotation; }
        FireAnimation() { }
        public FireAnimation(Drawing owner, float length = 2f) : base(owner, length) 
        {
            isLoop = true;
            shake = new ShakeAnimation(owner, strength: 0.6f);
            shake.isLoop = true;
            shake.Start();
            Start();
        }
        public override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;
            if (TotalProgress < 1)
            {
                shake.Update(gameTime);
                sinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Owner.Rotation -= rotationChange;
                //rotationChange = 0f;

                //if (Progress < 0.5f)
                //    rotationChange = 1.57f * Progress;
                //else
                //    rotationChange = -1.57f * Progress;

                //Owner.Rotation += rotationChange;
                Owner.Scale = Owner.Size - sizeChange;
                if (Progress < 0.25f)
                    sizeChange = new Vector2(20, 20) * Progress;
                else if (Progress < 0.5f)
                    sizeChange = new Vector2(20, 20) * (0.5f - Progress);
                else if (Progress < 0.75f)
                    sizeChange = new Vector2(10, 10) * -(Progress - 0.5f);
                else if (Progress < 1f)
                    sizeChange = new Vector2(10, 10) * -(1 - Progress);
                Owner.Scale = Owner.Size + sizeChange;

                if (Progress == 1 && iteration < iterationCount - 1)
                {
                    iteration++;
                    Progress = 0;
                }
                if(TotalProgress == 1 && isLoop)
                {
                    TotalProgress = 0;
                }
            }
            else
            {
                Inactive = true;
                IsActive = false;
            }
        }
    }
}

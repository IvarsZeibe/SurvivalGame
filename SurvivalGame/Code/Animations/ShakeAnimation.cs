using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class ShakeAnimation : Animation
    {
        float rotationChanges = 0f;
        public float rotation { get => rotationChanges; }
        float strength;
        public ShakeAnimation(Drawing owner, float shake_length = 0.6f, float strength = 0.3f) : base(owner, shake_length) 
        {
            iterationCount = 2;
            this.strength = strength;
        }
        public override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;
            if (TotalProgress < 1)
            {
                sinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Owner.Rotation -= rotationChanges;
                rotationChanges = 0f;
                if (Progress < 0.25f)
                    rotationChanges = Progress * strength;
                else if (Progress < 0.5f)
                    rotationChanges = (0.5f - Progress) * strength;
                else if (Progress < 0.75f)
                    rotationChanges = -(Progress - 0.5f) * strength;
                else if (Progress < 1f)
                    rotationChanges = -(1 - Progress) * strength;

                Owner.Rotation += rotationChanges;
                if (Progress == 1 && iteration < iterationCount - 1)
                {
                    iteration++;
                    Progress = 0;
                }
                if (TotalProgress == 1 && isLoop)
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
        public override void Reset()
        {
            base.Reset();
            Owner.Rotation -= rotationChanges;
            rotationChanges = 0;
        }
    }
}

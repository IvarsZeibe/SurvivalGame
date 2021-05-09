using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class ShakeAnimation : Animation
    {
        float rotationChanges = 0f;
        public ShakeAnimation(Drawing owner, float shake_length = 0.6f) : base(owner, shake_length) 
        {
            iterationCount = 2;
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
                    rotationChanges = Progress * 0.3f;
                else if (Progress < 0.5f)
                    rotationChanges = (0.5f - Progress) * 0.3f;
                else if (Progress < 0.75f)
                    rotationChanges = -(Progress - 0.5f) * 0.3f;
                else if (Progress < 1f)
                    rotationChanges = -(1 - Progress) * 0.3f;

                Owner.Rotation += rotationChanges;
                if (Progress == 1 && iteration < iterationCount - 1)
                {
                    iteration++;
                    Progress = 0;
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

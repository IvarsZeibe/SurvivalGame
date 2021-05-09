using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class StillnessAnimation : Animation
    {
        public StillnessAnimation(Drawing owner, float iterationLength = 2f) : base(owner, iterationLength) { }
        public override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;
            if (TotalProgress < 1)
            {
                sinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;

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
    }
}

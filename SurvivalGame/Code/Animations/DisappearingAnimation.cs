using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class DisappearingAnimation : Animation
    {
        Vector4 colorChange = Vector4.Zero;
        public DisappearingAnimation(Drawing owner, float iterationLength = 2f) : base(owner, iterationLength) { }
        public override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;
            if (TotalProgress < 1)
            {
                sinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Owner.Color = new Color(Owner.Color.ToVector4() - colorChange);
                colorChange = new Vector4(
                    -Owner.Color.R * Progress / 255,
                    -Owner.Color.G * Progress / 255,
                    -Owner.Color.B * Progress / 255,
                    -Owner.Color.A * Progress / 255);
                Owner.Color = new Color(Owner.Color.ToVector4() + colorChange);

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
            Owner.Color = new Color(Owner.Color.ToVector4() - colorChange);
            colorChange = Vector4.Zero;
        }
    }
}

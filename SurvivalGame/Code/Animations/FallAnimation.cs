using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class FallAnimation : Animation
    {
        public float rotationChanges { get; set; } = 0f;
        public Direction direction { get; set; } = Direction.Right;
        FallAnimation() { }
        public FallAnimation(Drawing owner, float shake_length = 1.5f) : base(owner, shake_length) { }
        public override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;
            if (TotalProgress < 1)
            {
                sinceStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Owner.Rotation -= rotationChanges;
                rotationChanges = 0f;

                //float progress = 1 - (fallLeft / FALL_LENGHT);
                if (direction == Direction.Right)
                    rotationChanges = 1.57f * Progress * Progress;
                else
                    rotationChanges = -1.57f * Progress * Progress;

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

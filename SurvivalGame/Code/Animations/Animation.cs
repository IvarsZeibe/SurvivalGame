using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    abstract class Animation
    {
        public Animation(Drawing owner, float length)
        {
            Owner = owner;
            LENGTH = length;
        }

        protected readonly float LENGTH;
        protected float sinceStart = 0f;
        public float TotalLength { get => LENGTH * iterationCount; }
        public int iterationCount = 1;
        public int iteration = 0;
        public bool isLoop = false;
        public Drawing Owner { get; set; }
        public float Progress
        {
            get
            {
                if (sinceStart / LENGTH - iteration < 1)
                    return sinceStart % LENGTH / LENGTH;
                else
                    return 1;
            }
            set => sinceStart = LENGTH * (value + iteration);
        }
        public float TotalProgress
        {
            get 
            {
                if (sinceStart / TotalLength < 1)
                    return sinceStart / TotalLength;
                else
                    return 1;
            }
            set
            {
                sinceStart = value * TotalLength;
                iteration = (int)(TotalLength / LENGTH) - 1;
            }
        }
        public bool Inactive = true;
        public bool IsActive { get; protected set; }

        public void Pause()
        {
            IsActive = false;
        }
        public void Continue()
        {
            IsActive = true;
        }
        public void Start()
        {
            IsActive = true;
            Reset();
            Inactive = false;
        }
        public void Stop()
        {
            Inactive = true;
            IsActive = false;
            TotalProgress = 0;
        }
        public virtual void Reset()
        {
            TotalProgress = 0;
        }
        public virtual void Update(GameTime gameTime) { }
    }
}

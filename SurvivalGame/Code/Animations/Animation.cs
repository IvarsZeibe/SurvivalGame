using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    abstract class Animation
    {
        protected Animation() { }
        public Animation(Drawing owner, float length)
        {
            Owner = owner;
            LENGTH = length;
        }

        public float LENGTH { get; set; }
        public float sinceStart { get; set; } = 0f;
        [System.Text.Json.Serialization.JsonIgnore]
        public float TotalLength { get => LENGTH * iterationCount; }
        public int iterationCount { get; set; } = 1;
        public int iteration { get; set; } = 0;
        public bool isLoop { get; set; } = false;
        public Drawing Owner { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
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
        [System.Text.Json.Serialization.JsonIgnore]
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
        public bool Inactive { get; set; } = true;
        public bool IsActive { get; set; }

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

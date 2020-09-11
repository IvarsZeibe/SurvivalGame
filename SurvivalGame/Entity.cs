using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Entity
    {
        public bool Collision { get; set; }
        float speed;
        public float Speed { get => speed; set { speed = 1 / value; } }
        public Vector2 Center { get; set; }
        public float Force { get; set; }
        public float Mass { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double XMovement { get; set; }
        public double YMovement { get; set; }
        public Point Size { get; set; }
        public Rectangle Rect { get; set; }
        public Texture2D Texture { get; set; }
        public void Update()
        {
            Force = Mass/* * (Speed+1)*/;
            Center = new Vector2((float)X + Size.X / 2, (float)Y + Size.Y / 2);
            Rect = new Rectangle((int)X, (int)Y, Size.X, Size.Y);
        }
    }
}

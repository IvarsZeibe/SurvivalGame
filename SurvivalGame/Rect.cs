using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Rect : Hitbox
    {
        //public int Width;
        //public int Height;
        //public override int Width { get; set; }
        //public override int Height { get; set; }

        public override double Left
        {
            get => X - Width / 2f;
            set => X = value + Width / 2f;
        }
        public override double Right
        {
            get => X + Width / 2f;
            set => X = value - Width / 2f;
        }
        public override double Top
        {
            get => Y - Height / 2f;
            set => Y = value + Height / 2f;
        }
        public override double Bottom
        {
            get => Y + Height / 2f;
            set => Y = value - Height / 2f;
        }
        public Rect(double x, double y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}

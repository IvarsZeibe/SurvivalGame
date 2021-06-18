using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Circle : Hitbox
    {
        public int Diameter;
        public override int Width
        {
            get => Diameter;
            set => Diameter = value;
        }
        public override int Height
        {
            get => Diameter;
            set => Diameter = value;
        }
        public override double Left
        {
            get => X - Diameter / 2f;
            set => X = value + Diameter / 2f;
        }
        public override double Top
        {
            get => Y - Diameter / 2f;
            set => Y = value + Diameter / 2f;
        }
        public override double Right
        {
            get => X + Diameter / 2f;
            set => X = value - Diameter / 2f;
        }
        public override double Bottom
        {
            get => Y + Diameter / 2f;
            set => Y = value - Diameter / 2f;
        }
        Circle() { }
        public Circle(double x, double y, int diameter)
        {
            X = x;
            Y = y;
            Diameter = diameter;
        }
    }
}

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
        public Circle(double x, double y, int diameter)
        {
            X = x;
            Y = y;
            Diameter = diameter;
        }
    }
}

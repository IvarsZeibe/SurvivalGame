using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Hitbox
    {
        public double X { get; set; }
        public double Y { get; set; }
        public virtual double Left { get; set; }
        public virtual double Top { get; set; }
        public virtual double Right { get; set; }
        public virtual double Bottom { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public Vector2 CollisionDetect(Hitbox h2)
        {
            if (this is Rect)
            {
                if (h2 is Rect)
                    return CollisionRectRect(this as Rect, h2 as Rect);
                else
                    return CollisionRecCircle(this as Rect, h2 as Circle);
            }
            else
            {
                if (h2 is Circle)
                    return CollisionCircleCircle(this as Circle, h2 as Circle);
                else
                    return CollisionRecCircle(h2 as Rect, this as Circle);
            }
        }

        Vector2 CollisionRectRect(Rect h1, Rect h2)
        {
            double distanceX = Math.Round(Math.Abs(h1.X - h2.X), 2);
            double distanceY = Math.Round(Math.Abs(h1.Y - h2.Y), 2);

            if (distanceX >= (h1.Width + h2.Width) / 2f)
            {
                return Vector2.Zero;
            }
            else if (distanceY >= (h1.Height + h2.Height) / 2f)
            {
                return Vector2.Zero;
            }
            else
                return new Vector2((float)((h1.Width + h2.Width) / 2f - distanceX), (float)((h1.Height + h2.Height) / 2f - distanceY));
        }
        Vector2 CollisionRecCircle(Rect h1, Circle h2)
        {
            double distanceX = Math.Round(Math.Abs(h1.X - h2.X), 2);
            double distanceY = Math.Round(Math.Abs(h1.Y - h2.Y), 2);

            double halfWidth = h1.Width / 2f;
            double halfHeight = h1.Height / 2f;
            double radius = h2.Diameter / 2f;

            if (distanceX >= halfWidth + radius)
            {
                return Vector2.Zero;
            }
            else if (distanceY >= halfHeight + radius)
            {
                return Vector2.Zero;
            }
            else
            {
                float cornerX = (float)(Math.Sqrt(radius * radius - (distanceY - halfHeight) * (distanceY - halfHeight)) - (distanceX - halfWidth));
                if (cornerX < 0)
                {
                    cornerX = 0;
                }
                float cornerY = (float)(Math.Sqrt(radius * radius - (distanceX - halfWidth) * (distanceX - halfWidth)) - (distanceY - halfHeight));
                if (cornerY < 0)
                {
                    cornerY = 0;
                }

                float edgeX = (float)(radius + halfWidth - distanceX);
                float edgeY = (float)(radius + halfHeight - distanceY);

                return new Vector2(
                    distanceY > halfHeight ? cornerX : edgeX,
                    distanceX > halfWidth ? cornerY : edgeY);
            }
        }
        Vector2 CollisionCircleCircle(Circle h1, Circle h2)
        {
            double distanceX = Math.Round(Math.Abs(h1.X - h2.X), 2);
            double distanceY = Math.Round(Math.Abs(h1.Y - h2.Y), 2);

            double radius1 = h1.Diameter / 2f;
            double radius2 = h2.Diameter / 2f;

            double minimalDistance = radius1 + radius2;

            if (distanceX * distanceX + distanceY * distanceY >= minimalDistance * minimalDistance)
            {
                return Vector2.Zero;
            }
            else
            {
                double minimalDistanceX = Math.Sqrt(minimalDistance * minimalDistance - distanceY * distanceY);
                double minimalDistanceY = Math.Sqrt(minimalDistance * minimalDistance - distanceX * distanceX);

                float offsetX = (float)(minimalDistanceX - distanceX);
                float offsetY = (float)(minimalDistanceY - distanceY);

                return new Vector2(offsetX, offsetY);
            }
        }
        public static Rect operator +(Hitbox h1, Hitbox h2)
        {
            Rect result = new Rect(h1.X + h2.X, h1.Y + h2.Y, h1.Width + h2.Width, h1.Height + h2.Height);
            return result;
        }
    }
}

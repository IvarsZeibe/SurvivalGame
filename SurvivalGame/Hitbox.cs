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
        //public bool Collision { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public virtual double Left { get; set; }
        public virtual double Top { get; set; }
        public virtual double Right { get; set; }
        public virtual double Bottom { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        // Collsion using center coords
        // h1 = self hitbox
        // h2 = other hitbox
        public Vector2 CollisionDetect(Hitbox h1, Hitbox h2)
        {
            if (h1 is Rect)
            {
                if (h2 is Rect)
                    return CollisionRectRect(h1 as Rect, h2 as Rect);
                else
                    return CollisionRecCircle(h1 as Rect, h2 as Circle);
            }
            else
            {
                if (h2 is Circle)
                    return CollisionCircleCircle(h1 as Circle, h2 as Circle);
                else
                    return CollisionRecCircle(h2 as Rect, h1 as Circle);
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
                float offsetX = (float)(minimalDistanceX - distanceX);
                //if (offsetX < 0)
                //{
                //    offsetX = 0;
                //}
                double minimalDistanceY = Math.Sqrt(minimalDistance * minimalDistance - distanceX * distanceX);
                float offsetY = (float)(minimalDistanceY - distanceY);
                //if (offsetY < 0)
                //{
                //    offsetY = 0;
                //}

                //loat edgeX = (float)(radius2 + radius1 - distanceX);
                //float edgeY = (float)(radius2 + radius1 - distanceY);

                // return new Vector2(
                //distanceY > radius1 ? offsetX : edgeX,
                //distanceX > radius1 ? offsetY : edgeY);
                return new Vector2(offsetX, offsetY);
            }
            //double distanceX = Math.Abs(h1.X - h2.X);
            //double distanceY = Math.Abs(h1.Y - h2.Y);

            //if (distanceX * distanceX + distanceY * distanceY >= (h1.Diameter * h1.Diameter + h2.Diameter * h2.Diameter) / 2f)
            //{
            //    return Vector2.Zero;
            //}
            //else
            //{
            //    double minimalX = Math.Sqrt(h1.Diameter * h1.Diameter + h2.Diameter * h2.Diameter - distanceY * distanceY);
            //    double minimalY = Math.Sqrt(h1.Diameter * h1.Diameter + h2.Diameter * h2.Diameter - distanceX * distanceX);

            //    return new Vector2((float)(minimalX - distanceX), (float)(minimalY - distanceY));
            //}
        }
    }
}

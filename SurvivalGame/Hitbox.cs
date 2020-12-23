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
            double distanceX = Math.Abs(h1.X - h2.X);
            double distanceY = Math.Abs(h1.Y - h2.Y);

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
            double distanceX = Math.Abs(h1.X - h2.X);
            double distanceY = Math.Abs(h1.Y - h2.Y);

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
            //else if (Math.Sqrt(distanceX * distanceX + distanceY * distanceY) >= Math.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight) + radius)
            //{
            //    return Vector2.Zero;
            //}
            else
            {
                double requiredDistance = Math.Sqrt(halfWidth * halfWidth + halfHeight + halfHeight) + radius;
                //Console.WriteLine($"\nX: {radius + halfWidth - distanceX},{Math.Sqrt(requiredDistance * requiredDistance - distanceY * distanceY)}\n" +
                //    $"Y: {radius + halfHeight - distanceY},{Math.Sqrt(requiredDistance * requiredDistance - distanceX * distanceX)}");

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

                //if (distanceY > halfHeight)
                    //return new Vector2((float)cornerX, (float)cornerY);
                //else
                //    return new Vector2((float)(radius + halfWidth - distanceX), (float)(radius + halfHeight - distanceY));
                //return new Vector2
                //    (
                //    (float)Math.Min(radius + halfWidth - distanceX, cornerX),
                //    (float)Math.Min(radius + halfHeight - distanceY, cornerY)
                //    );
            }

            //Console.WriteLine($"{Math.Sqrt(distanceX * distanceX + distanceY * distanceY)}  { (Math.Sqrt(h1.Width * h1.Width + h1.Height * h1.Height) + Math.Sqrt(h2.Diameter * h2.Diameter)) / 2f}");

                //if (Math.Sqrt(distanceX * distanceX + distanceY * distanceY) < (Math.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight) + radius))
                //{
                //    //return overlapCorner

                //    double requiredDistance = Math.Sqrt(halfWidth * halfWidth + halfHeight + halfHeight) + radius;
                //    //double realDistance = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

                //    return new Vector2((float)Math.Sqrt(requiredDistance * requiredDistance - distanceY * distanceY), (float)Math.Sqrt(requiredDistance * requiredDistance - distanceX * distanceX));
                //}
                //else if (distanceX < radius + halfWidth && distanceY < radius + halfHeight)
                //{
                //    return new Vector2((float)(radius + halfWidth - distanceX), (float)(radius + halfHeight - radius));
                //}
                //else return Vector2.Zero;




                //if (distanceX >= (h1.Width + h2.Diameter) / 2f)
                //{
                //    return Vector2.Zero;
                //}
                //else if (distanceY >= (h1.Height + h2.Diameter) / 2f)
                //{
                //    return Vector2.Zero;
                //}
                //else if (Math.Sqrt(distanceX * distanceX + distanceY * distanceY) >= (Math.Sqrt(h1.Width * h1.Width + h1.Height * h1.Height) + Math.Sqrt(h2.Diameter * h2.Diameter)) / 2f)
                //{
                //    return Vector2.Zero;
                //}
                //else if (distanceX >= (h1.Width + h2.Diameter) / 2f && distanceY >= (h1.Height + h2.Diameter) / 2f)
                //{
                //    double minimalX = Math.Sqrt(Math.Pow(Math.Sqrt(halfH1Width * halfH1Width + halfH1Height * halfH1Height) + h2.Diameter / 2, 2) - distanceY * distanceY);
                //    double minimalY = Math.Sqrt(Math.Pow(Math.Sqrt(halfH1Width * halfH1Width + halfH1Height * halfH1Height) + h2.Diameter / 2, 2) - distanceX * distanceX);

                //    return new Vector2((float)(minimalX - distanceX), (float)(minimalY - distanceY));
                //}
                //else
                //{
                //    return new Vector2((float)((h1.Width + h2.Diameter) / 2f - distanceX), (float)((h1.Height + h2.Diameter) / 2f - distanceY));
                //}
        }
        Vector2 CollisionCircleCircle(Circle h1, Circle h2)
        {
                double distanceX = Math.Abs(h1.X - h2.X);
                double distanceY = Math.Abs(h1.Y - h2.Y);

            if (distanceX * distanceX + distanceY * distanceY >= (h1.Diameter * h1.Diameter + h2.Diameter * h2.Diameter) / 2f)
            {
                return Vector2.Zero;
            }
            else
            {
                double minimalX = Math.Sqrt(h1.Diameter * h1.Diameter + h2.Diameter * h2.Diameter - distanceY * distanceY);
                double minimalY = Math.Sqrt(h1.Diameter * h1.Diameter + h2.Diameter * h2.Diameter - distanceX * distanceX);

                return new Vector2((float)(minimalX - distanceX), (float)(minimalY - distanceY));
            }
            }
    }
}

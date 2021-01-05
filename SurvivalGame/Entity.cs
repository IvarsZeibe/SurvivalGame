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
        //public Point Size { get; set; }
        public Hitbox Hitbox { get; set; }
        public Rectangle Rect { get; set; }
        public Texture2D Texture { get; set; }
        public void Update()
        {
            //Hitbox.Collision = Collision;
            Force = Mass/* * (Speed+1)*/;
            X = Hitbox.X;
            Y = Hitbox.Y;
            //Center = new Vector2((float)X + Size.X / 2, (float)Y + Size.Y / 2);
            //Rect = new Rectangle((int)X, (int)Y, Size.X, Size.Y);
            //Hitbox.X = X;
            //Hitbox.Y = Y;
        }
        public int Health { get; set; }
        public bool DamageEntity(int damage, string source)
        {
            Health -= damage;
            return true;
        }
        public bool isDead = false;


        public float Move(double movement, bool xDirection, List<Entity> entities, List<(Entity movedEntity, float distanceMoved)> movedEntities = null, float movementDecrease = 0f, float mass = 0)
        {
            bool first = false;
            float thisDecrease = 0f;

            //Moves this entity
            if (xDirection)
            {
                Hitbox.X += movement;
            }
            else
            {
                Hitbox.Y += movement;
            }

            if(movedEntities == null)
            {
                movedEntities = new List<(Entity movedEntity, float distanceMoved)>();
                first = true;
                mass = Mass;
            }

            //Save self and self distance moved sum in a list
            bool flag = false;
            for(int i = 0; i < movedEntities.Count; i++)
            {
                if (movedEntities[i].movedEntity == this)
                {
                    if(movement > 0)
                        movedEntities[i] = (this, movedEntities[i].distanceMoved + (float)movement);
                    else
                        movedEntities[i] = (this, movedEntities[i].distanceMoved - (float)movement);

                    flag = true;
                    break;
                }
            }
            if(!flag)
                movedEntities.Add((this, (float)movement));

            //pushing
            if (Collision)
            {
                foreach (Entity entity in entities)
                {
                    if (entity.Collision && entity != this)
                    {
                        float intersection = 0;
                        if (xDirection)
                        {
                            intersection = Hitbox.CollisionDetect(Hitbox, entity.Hitbox).X;
                        }
                        else
                        {
                            intersection = Hitbox.CollisionDetect(Hitbox, entity.Hitbox).Y;
                        }

                        if (intersection != 0)
                        {
                            if (entity.Mass < mass)
                            {
                                Collision = false;
                                if (movement < 0)
                                    intersection *= -1;
                                float potencialDecrease = entity.Move(intersection, xDirection, entities, movedEntities, movementDecrease, mass);
                                if (potencialDecrease > thisDecrease)
                                    thisDecrease = potencialDecrease;
                                Collision = true;
                            }
                            else
                            {
                                if (intersection > thisDecrease)
                                    thisDecrease = intersection;
                            }
                        }
                            

                        //// breaks if moves too far into other entity
                        //if (movement < 0 && intersection != 0)
                        //    movement += intersection;
                        //else
                        //    movement -= intersection;
                    }
                }
            }

            movementDecrease += thisDecrease;

            if (first)
            {

                if (movement < 0)
                    movementDecrease *= -1;

                for (int i = 0; i < movedEntities.Count; i++)
                {
                    var tupl = movedEntities[i];
                    if (Math.Abs(tupl.distanceMoved) < Math.Abs(movementDecrease))
                        movementDecrease = tupl.distanceMoved;
                    if (xDirection)
                    {
                        tupl.movedEntity.Hitbox.X -= movementDecrease;
                    }
                    else
                    {
                        tupl.movedEntity.Hitbox.Y -= movementDecrease;
                    }
                    tupl.movedEntity.Update();

                }
                return 0f;

            }
            else
            {
                return movementDecrease;
            }
        }
    }
}

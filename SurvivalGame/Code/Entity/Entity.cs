using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    enum Direction { Left, Right, Up, Down};
    enum DamageType
    {
        Unknown,
        Sword,
        Projectile,
        Axe,
        Fire
    }
    abstract class Entity
    {
        Entity() { }
        public Entity(bool addToRoom = true)
        {
            if (addToRoom)
            {
                IsLoaded = true;
                Globals.Rooms[Globals.activeRoomCoords].Entities.Add(this);
            }
            else
            {
                IsLoaded = false;
            }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public Drawing Drawing {
            get => Drawings["base"];
            set => Drawings["base"] = value;
        }
        public Dictionary<string, Drawing> Drawings { get; set; } = new Dictionary<string, Drawing>();
        public List<Light> Lights { get; set; } = new List<Light>();
        public Entity owner { get; set; } = null;
        public Hitbox Hitbox { get; set; }
        public bool Collision { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Mass { get; set; }
        public double XMovement { get; set; }
        public double YMovement { get; set; }
        public Vector2 RecievedKnockback { get; set; } = Vector2.Zero;
        public bool IsDead { get; set; } = false;
        public bool IsLoaded { get; set; }
        public virtual void Load()
        {
            IsLoaded = true;
            foreach (var Drawing in Drawings)
            {
                Drawing.Value.Enable();
            }
        }
        public virtual void UnLoad()
        {
            IsLoaded = false;
            foreach (var Drawing in Drawings)
            {
                Drawing.Value.Disable();
            }
            Effects.ForEach(eff => eff.UnLoad());
        }
        public Entity Target { get; set; }

        private float speed;
        [System.Text.Json.Serialization.JsonIgnore]
        public float Speed
        {
            get => speed;
            set { speed = 1 / value; }
        }
        public float DirectSpeed
        {
            get => speed;
            set { speed = value; }
        }
        public float X
        {
            get => (float)Hitbox.X;
            set => Hitbox.X = value;
        }

        public float Y
        {
            get => (float)Hitbox.Y;
            set => Hitbox.Y = value;
        }
        public virtual void Update(GameTime gameTime) 
        {
            ApplyEffects(gameTime);
            UpdateAnimations(gameTime);
        }
        public virtual bool DamageSelf(int damage, Entity source, DamageType damageType = DamageType.Unknown)
        {
            Health -= damage;
            return true;
        }
        public virtual void Kill()
        {
            UnLoad();
            IsLoaded = true;
            IsDead = true;
        }
        public List<Effect> Effects { get; set; } = new List<Effect>();
        public virtual void ApplyEffects(GameTime gameTime)
        {
            foreach (var effect in Effects.FindAll(eff => eff.IsActive))
            {
                effect.Apply(gameTime, this);
            }
            Effects = Effects.FindAll(effect => !effect.IsDead);
        }
        public bool AddEffect(Effect effect)
        {
            if (Effects.Exists(eff => eff.GetType() == effect.GetType()))
            {
                if (Effects.Exists(eff => eff.Strength >= effect.Strength && eff.Duration >= effect.Duration))
                {
                    Trace.WriteLine(1);
                    return false;
                }
                var efect = Effects.Find(eff => eff.Strength <= effect.Strength && eff.Duration <= effect.Duration);
                if (efect != null)
                {
                    Trace.WriteLine(2);
                    effect.Animation = efect.Animation;
                    effect.Animation.Owner = effect.Drawing;
                    efect.Duration = 0;
                }
            }
            Effects.Add(effect);
            return true;
        }
        public Dictionary<string, Animation> Animations { get; set; } = new Dictionary<string, Animation>();
        protected virtual void UpdateAnimations(GameTime gameTime)
        {
            foreach (var animation in Animations)
            {
                animation.Value.Update(gameTime);
            }
        }
        public bool CollidesWith(Entity entity)
        {
            return Hitbox.CollisionDetect(entity.Hitbox) != Vector2.Zero;
        }
        public float Move(double movement, bool xDirection, List<(Entity movedEntity, float maxMovement)> movedEntities = null, float movementDecreaseTotal = 0f, float mass = 0)
        {
            bool first = false;
            float movementDecrease = 0f;

            movement = Math.Round(movement, 2);

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
                    if (movement > 0)
                        movedEntities[i] = (this, movedEntities[i].maxMovement + (float)movement);
                    else
                        movedEntities[i] = (this, movedEntities[i].maxMovement + (float)movement);

                    flag = true;
                    break;
                }
            }
            if(!flag)
                movedEntities.Add((this, (float)movement));

            //pushing
            if (Collision)
            {
                foreach (Entity entity in EntityTracker.Entities)
                {
                    if (entity.Collision && entity != this)
                    {
                        float intersection = 0;
                        if (xDirection)
                        {
                            intersection = Hitbox.CollisionDetect(entity.Hitbox).X;
                        }
                        else
                        {
                            intersection = Hitbox.CollisionDetect(entity.Hitbox).Y;
                        }

                        if (intersection != 0)
                        {
                            if (entity.Mass <= mass)
                            {
                                Collision = false;
                                if (movement < 0)
                                    intersection *= -1;
                                float potencialDecrease = entity.Move(intersection, xDirection, movedEntities, movementDecreaseTotal, mass);
                                if (potencialDecrease > movementDecrease)
                                    movementDecrease = potencialDecrease;
                                Collision = true;
                            }
                            else
                            {
                                if (intersection > movementDecrease)
                                    movementDecrease = intersection;
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

            movementDecreaseTotal += movementDecrease;

            if (first)
            {

                if (movement < 0)
                    movementDecreaseTotal *= -1;

                for (int i = 0; i < movedEntities.Count; i++)
                {
                    float movmentDecrease = movementDecreaseTotal;
                    var tupl = movedEntities[i];
                    if (Math.Abs(tupl.maxMovement) < Math.Abs(movmentDecrease))
                        movmentDecrease = tupl.maxMovement;
                    if (xDirection)
                    {
                        tupl.movedEntity.Hitbox.X -= movmentDecrease;
                        tupl.movedEntity.Hitbox.X = Math.Round(tupl.movedEntity.Hitbox.X, 2);
                    }
                    else
                    {
                        tupl.movedEntity.Hitbox.Y -= movmentDecrease;
                        tupl.movedEntity.Hitbox.Y = Math.Round(tupl.movedEntity.Hitbox.Y, 2);
                    }
                }
                return 0f;
            }
            else
            {
                return movementDecreaseTotal;
            }

        }

    }
}

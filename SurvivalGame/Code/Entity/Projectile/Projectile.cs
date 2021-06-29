using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Projectile : Entity
    {
        public List<Entity> immuneEntities { get; set; } = new List<Entity>();
        public int Damage { get; set; }
        public int Range { get; set; }
        public Vector2 StartingCoord { get; set; }
        public Vector2 direction { get; set; } = Vector2.Zero;
        //public List<Effect> effects { get; set; } = new List<Effect>();
        Projectile() { }
        public Projectile(Entity _owner, TextureName texture, float speed, Vector2 source, Vector2 target, int damage, float angleRad = 0, int range = 600)
        {
            owner = _owner;
            //this.Texture = texture;
            this.Hitbox = new Circle(source.X, source.Y, 2);
            this.Collision = false;
            this.Range = range;
            this.Speed = speed;
            this.StartingCoord = new Vector2(X, Y);
            this.Damage = damage;
            Drawing = new Drawing(texture, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), Color.Yellow, 0, new Vector2(10, 2), 0.5f, true);
            Movement(target);
            //direction += directionModifier ?? Vector2.Zero;
            //direction = Vector2.Transform(direction, Matrix.CreateRotationX(angle));
            direction = Vector2.Transform(direction, Matrix.CreateRotationZ(angleRad));
            Drawing.Rotation += angleRad;
        }
        public override void Update(GameTime gameTime)
        {
            Move(direction.X * (1/Speed) * gameTime.ElapsedGameTime.TotalSeconds, true);
            Move(direction.Y * (1/Speed) * gameTime.ElapsedGameTime.TotalSeconds, false);

            if((StartingCoord.X - X ) * (StartingCoord.X - X) + (StartingCoord.Y - Y) * (StartingCoord.Y - Y) > Range * Range)
                Kill();

            foreach(var enemy in EntityTracker.Entities)
            {
                if (enemy is Wall)
                {
                    if (Hitbox.CollidesWith(enemy.Hitbox) && !immuneEntities.Contains(enemy))
                    {
                        Kill();
                        new Sparkles(new Vector2(X, Y));
                    }
                }
                else if (!(enemy is Projectile) && !(enemy is MouseCursor))
                {
                    if (Hitbox.CollidesWith(enemy.Hitbox) && !immuneEntities.Contains(enemy))
                    {
                        enemy.DamageSelf(Damage, this);
                        immuneEntities.Add(this);
                        Kill();
                        new Sparkles(new Vector2(X, Y));
                    }
                }
            }
            Drawing.Position = new Vector2((float)Hitbox.Left, (float)Hitbox.Top);
        }
        private void Movement(Vector2 target)
        {
            float yEdge = (target.Y - Y)/* * precision*/;
            float xEdge = (target.X - X)/* * precision*/;
            Drawing.Rotation = (float)Math.Atan2(yEdge, xEdge);

            direction = Vector2.Normalize(new Vector2(xEdge, yEdge));
            //Vector2 relativeMouse = new Vector2((float)xEdge, (float)yEdge);
            //XMovement = -relativeMouse.X / ((Math.Abs(relativeMouse.X) + Math.Abs(relativeMouse.Y)) * Speed);
            //YMovement = -relativeMouse.Y / ((Math.Abs(relativeMouse.X) + Math.Abs(relativeMouse.Y)) * Speed);
        }
    }
}

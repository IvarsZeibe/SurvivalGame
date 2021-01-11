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
        //Vector2 Target { get; set; }
        //private float precision = 1f;
        //public void SetPrecision(float precision)
        //{
        //    this.precision = precision;
        //    Movement(Target);
        //}
        public List<Entity> immuneEntities = new List<Entity>();
        public int Damage { get; set; }
        int Range { get; set; }
        //List<double> RelativCoord { get; set; }
        //List<double> StartingCoord { get; set; }
        Vector2 StartingCoord { get; set; }
        public Projectile(Texture2D texture, float speed, Vector2 source, Vector2 target, int damage)
        {
            this.Texture = texture;
            this.Hitbox = new Rect(source.X, source.Y, 10, 2);
            this.Collision = false;
            this.Range = 300;
            this.Speed = speed;
            this.StartingCoord = new Vector2(X, Y);
            this.Damage = damage;
            this.LayerDepth = 0.8f;
            //this.Target = target;
            Movement(target);
        }
        //public Projectile(Texture2D texture, float x, float y, int diameter, bool collision, int health, int mass, Color color, float speed = 0f, float rotation = 0f, float layerDepth = 0.5f)
        //{
        //    this.Texture = texture;
        //    this.Hitbox = new Circle(x, y, diameter);
        //    this.Collision = collision;
        //    this.Health = health;
        //    this.Mass = mass;
        //    this.Color = color;
        //    this.Speed = speed;
        //    this.Rotation = rotation;
        //    this.LayerDepth = layerDepth;
        //}
        public override void Update(GameTime gameTime)
        {
            Move(XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
            Move(YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);

            if((StartingCoord.X - X ) * (StartingCoord.X - X) + (StartingCoord.Y - Y) * (StartingCoord.Y - Y) > Range * Range)
                IsDead = true;

            foreach (var wall in EntityTracker.GetEntities<Wall>())
            {
                if (CollidesWith(wall) && wall.Collision)
                {
                    IsDead = true;
                }
            }
        }
        private void Movement(Vector2 target)
        {
            double yEdge = (Y - target.Y)/* * precision*/;
            double xEdge = (X - target.X)/* * precision*/;
            this.Rotation = (float)Math.Atan2(yEdge, xEdge);

            Vector2 relativeMouse = new Vector2((float)xEdge, (float)yEdge);
            XMovement = -relativeMouse.X / ((Math.Abs(relativeMouse.X) + Math.Abs(relativeMouse.Y)) * Speed);
            YMovement = -relativeMouse.Y / ((Math.Abs(relativeMouse.X) + Math.Abs(relativeMouse.Y)) * Speed);
        }
    }
}

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
        public Projectile(TextureName texture, float speed, Vector2 source, Vector2 target, int damage)
        {
            //this.Texture = texture;
            this.Hitbox = new Rect(source.X, source.Y, 10, 2);
            this.Collision = false;
            this.Range = 300;
            this.Speed = speed;
            this.StartingCoord = new Vector2(X, Y);
            this.Damage = damage;
            Drawing = new Drawing(texture, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), Color.Yellow, 0, new Vector2(10, 2), isDrawn: true);
            Movement(target);
        }
        public override void Update(GameTime gameTime)
        {
            Move(XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
            Move(YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);

            if((StartingCoord.X - X ) * (StartingCoord.X - X) + (StartingCoord.Y - Y) * (StartingCoord.Y - Y) > Range * Range)
                Kill();

            foreach (var wall in EntityTracker.GetEntities<Wall>())
            {
                if (CollidesWith(wall) && wall.Collision)
                {
                    Kill();
                }
            }
            Drawing.Position = new Vector2((float)Hitbox.Left, (float)Hitbox.Top);
        }
        private void Movement(Vector2 target)
        {
            double yEdge = (Y - target.Y)/* * precision*/;
            double xEdge = (X - target.X)/* * precision*/;
            Drawing.Rotation = (float)Math.Atan2(yEdge, xEdge);

            Vector2 relativeMouse = new Vector2((float)xEdge, (float)yEdge);
            XMovement = -relativeMouse.X / ((Math.Abs(relativeMouse.X) + Math.Abs(relativeMouse.Y)) * Speed);
            YMovement = -relativeMouse.Y / ((Math.Abs(relativeMouse.X) + Math.Abs(relativeMouse.Y)) * Speed);
        }
    }
}

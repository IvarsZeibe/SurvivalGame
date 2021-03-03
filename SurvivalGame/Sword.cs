using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Sword : Entity
    {
        public List<Entity> immuneEntities = new List<Entity>();
        float StartingRotation { get; set; }
        Direction Direction { get; set; }
        Entity Owner { get; set; }
        float timeAlive;
        float timeTillDeath = 0.2f;
        public int Damage { get; set; }
        public Sword(TextureName texture, Entity owner, float rotation, int damage)
        {
            Damage = damage;
            Collision = false;
            Mass = 1;
            Hitbox = new Circle(owner.Hitbox.X, owner.Hitbox.Y, (owner.Hitbox.Width + owner.Hitbox.Height) / 2);
            Owner = owner;

            if(rotation >= -0.8 && rotation < 0.8)
            {
                StartingRotation = -2.4f;
                Direction = Direction.Left;
            }
            else if (rotation >= 0.8 && rotation < 2.4)
            {
                StartingRotation = -0.8f;
                Direction = Direction.Down;
            }
            else if (rotation >= -2.4 && rotation < -0.8)
            {
                StartingRotation = 2.4f;
                Direction = Direction.Up;
            }
            else if (rotation >= 2.4 || rotation < -2.4)
            {
                StartingRotation = 0.8f;
                Direction = Direction.Right;
            }
            UpdateCoord();

            Drawing = new Drawing(texture, new Vector2((float)Hitbox.X, (float)Hitbox.Y), Color.White, StartingRotation,
                new Vector2((float)Hitbox.Width, 2f), 0.35f, true);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateSize();
            timeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeAlive > timeTillDeath)
            {
                Kill();
            }
            UpdateCoord();
            Drawing.Position = new Vector2((float)Hitbox.X, (float)Hitbox.Y);
            Drawing.Rotation = timeAlive * -1.8f / timeTillDeath + StartingRotation;
            foreach (var slime in EntityTracker.GetEntities<SlimeEnemy>())
            {
                if(Hitbox.CollisionDetect(slime.Hitbox) != Vector2.Zero && !immuneEntities.Contains(slime))
                {
                    slime.DamageSelf(Damage, this);
                    immuneEntities.Add(slime);
                }
            }
        }
        private void UpdateCoord()
        {
            switch (Direction)
            {
                case Direction.Right:
                    Hitbox.Left = Owner.Hitbox.X;
                    Hitbox.Y = Owner.Hitbox.Y;
                    break;
                case Direction.Up:
                    Hitbox.Top = Owner.Hitbox.Y;
                    Hitbox.X = Owner.Hitbox.X;
                    break;
                case Direction.Down:
                    Hitbox.Bottom = Owner.Hitbox.Y;
                    Hitbox.X = Owner.Hitbox.X;
                    break;
                case Direction.Left:
                    Hitbox.Right = Owner.Hitbox.X;
                    Hitbox.Y = Owner.Hitbox.Y;
                    break;
            }
        }
        private void UpdateSize()
        {
            (Hitbox as Circle).Diameter = (Owner.Hitbox.Width + Owner.Hitbox.Height) / 2;
        }
    }
}

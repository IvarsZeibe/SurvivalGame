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
        float timeAlive;
        float timeTillDeath = 0.2f;
        public int Damage { get; set; }
        private float sizeRatio = 0.8f;
        private float KnockbackStrenght;
        // rotation right = 0, up = -1.57, left = 3.14 or -3.14, bottom = 1.57
        public Sword(TextureName texture, Entity owner, float rotation, int damage, float knockbackStrenght)
        {
            Damage = damage;
            Collision = false;
            Mass = 1;
            Hitbox = new Circle(owner.Hitbox.X, owner.Hitbox.Y, (int)((owner.Hitbox.Width + owner.Hitbox.Height) * sizeRatio));
            this.owner = owner;
            KnockbackStrenght = knockbackStrenght;

            if (rotation >= -0.8 && rotation < 0.8)
            {
                StartingRotation = -1.57f;
                Direction = Direction.Left;
            }
            else if (rotation >= 0.8 && rotation < 2.4)
            {
                StartingRotation = 0f;
                Direction = Direction.Down;
            }
            else if (rotation >= -2.4 && rotation < -0.8)
            {
                StartingRotation = 3.14f;
                Direction = Direction.Up;
            }
            else if (rotation >= 2.4 || rotation < -2.4)
            {
                StartingRotation = 1.57f;
                Direction = Direction.Right;
            }
            UpdateCoord();

            Drawing = new Drawing(texture, new Vector2((float)Hitbox.X, (float)Hitbox.Y), Color.White, StartingRotation,
                new Vector2((float)(owner.Hitbox.Width + owner.Hitbox.Height) * sizeRatio / 2, 2f), 0.35f, true);
            // radius = (owner.Hitbox.Width + owner.Hitbox.Height) / 4;
            //Drawing = new Drawing(TextureName.Circle, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), Color.White, 0f,
            //    new Vector2(radius, radius), 0.4f, true);
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
            Drawing.Rotation = timeAlive * -3.14f / timeTillDeath + StartingRotation;
            foreach (var slime in EntityTracker.GetEntities<SlimeEnemy>())
            {
                if(Hitbox.CollisionDetect(slime.Hitbox) != Vector2.Zero && !immuneEntities.Contains(slime))
                {
                    slime.DamageSelf(Damage, base.owner);
                    immuneEntities.Add(slime);
                    switch (Direction)
                    {
                        case Direction.Left:
                            slime.knockbackX = -KnockbackStrenght;
                            break;
                        case Direction.Right:
                            slime.knockbackX = KnockbackStrenght;
                            break;
                        case Direction.Up:
                            slime.knockbackY = KnockbackStrenght;
                            break;
                        case Direction.Down:
                            slime.knockbackY = -KnockbackStrenght;
                            break;
                    }
                }
            }
            foreach(var entity in EntityTracker.Entities)
            {
                if(!(entity is SlimeEnemy))
                {
                    if (Hitbox.CollidesWith(entity.Hitbox) && !immuneEntities.Contains(entity))
                    {
                        entity.DamageSelf(Damage, owner, DamageType.Projectile);
                        immuneEntities.Add(entity);
                        switch (Direction)
                        {
                            case Direction.Left:
                                entity.RecievedKnockback += new Vector2(-KnockbackStrenght, 0);
                                break;
                            case Direction.Right:
                                entity.RecievedKnockback += new Vector2(KnockbackStrenght, 0);
                                break;
                            case Direction.Up:
                                entity.RecievedKnockback += new Vector2(0, -KnockbackStrenght);
                                break;
                            case Direction.Down:
                                entity.RecievedKnockback += new Vector2(0, KnockbackStrenght);
                                break;
                        }
                    }
                }
            }
        }
        private void UpdateCoord()
        {
            switch (Direction)
            {
                case Direction.Right:
                    Hitbox.X = owner.Hitbox.Right;
                    Hitbox.Y = owner.Hitbox.Y;
                    break;
                case Direction.Up:
                    Hitbox.Y = owner.Hitbox.Bottom;
                    Hitbox.X = owner.Hitbox.X;
                    break;
                case Direction.Down:
                    Hitbox.Y = owner.Hitbox.Top;
                    Hitbox.X = owner.Hitbox.X;
                    break;
                case Direction.Left:
                    Hitbox.X = owner.Hitbox.Left;
                    Hitbox.Y = owner.Hitbox.Y;
                    break;
            }
        }
        private void UpdateSize()
        {
            (Hitbox as Circle).Diameter = (int)((owner.Hitbox.Width + owner.Hitbox.Height) * sizeRatio);
        }
    }
}

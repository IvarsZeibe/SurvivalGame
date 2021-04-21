using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Missile : Entity
    {
        public List<Entity> ImmuneEntities = new List<Entity>();

        Vector2 startingCoord;
        int damage;
        int range;
        float explosionProgress = -1f;
        int maxWidth = 200;
        int width = 7;
        int height = 3;
        public Missile(Entity _owner, Vector2 source, bool trackEnemy, Entity _target, int _damage, float speed, int _range)
        {
            this.Collision = false;


            owner = _owner;
            ImmuneEntities.Add(owner);
            ImmuneEntities.Add(this);
            this.Hitbox = new Circle(source.X, source.Y, 2);
            this.startingCoord = new Vector2(source.X, source.Y);
            if (trackEnemy)
                this.Target = _target;
            else
                this.Target = new NoBrainEntity(_target.X, _target.Y);
            this.damage = _damage;
            this.Speed = speed;
            this.range = _range;

            Drawing = new Drawing(TextureName.Rectangle, Hitbox.GetTopLeftPosVector(), Color.DarkKhaki, 0, new Vector2(width, height), 0.5f, true);
        }
        public override void Update(GameTime gameTime)
        {
            if(explosionProgress == -1f)
            {
                UpdateMovement(gameTime);
                Move(XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
                Move(YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);

                if (XMovement == 0 && YMovement == 0)
                {
                    BeginExplosion();
                }
            }
            else
            {
                UpdateExplosion(gameTime);
            }
            Drawing.Position = Hitbox.GetTopLeftPosVector();

            DamageEntities();

        }

        private void UpdateMovement(GameTime gameTime)
        {
            double xEdge = Hitbox.X - Target.Hitbox.X;
            double yEdge = Hitbox.Y - Target.Hitbox.Y;
            Drawing.Rotation = (float)Math.Atan2(yEdge, xEdge);
            XMovement = -xEdge / ((Math.Abs(xEdge) + Math.Abs(yEdge)) * Speed);
            YMovement = -yEdge / ((Math.Abs(xEdge) + Math.Abs(yEdge)) * Speed);
            if (Math.Abs(xEdge) < Math.Abs(XMovement) * gameTime.ElapsedGameTime.TotalSeconds)
                XMovement = 0;
            if (Math.Abs(yEdge) < Math.Abs(YMovement) * gameTime.ElapsedGameTime.TotalSeconds)
                YMovement = 0;
        }
        private void BeginExplosion()
        {
            Drawing.Texture = TextureName.Circle;
            Drawing.Rotation = 0;
            explosionProgress = 0;
        }
        private void UpdateExplosion(GameTime gameTime)
        {
            explosionProgress += (float)gameTime.ElapsedGameTime.TotalSeconds / 0.2f;
            if (explosionProgress < 0.6)
            {
                Hitbox.Width = (int)(maxWidth * explosionProgress);
            }
            else if(explosionProgress < 1)
            {
                Hitbox.Width = (int)(maxWidth * (0.6 - (explosionProgress - 0.6)));
            }
            else
            {
                Kill();
            }
            Drawing.Scale = Hitbox.GetScaleVector();
        }
        private void DamageEntities()
        {
            foreach (var entity in EntityTracker.GetEntities<Entity>())
            {
                if (!ImmuneEntities.Contains(entity) && entity.Hitbox.CollidesWith(Hitbox))
                {
                    entity.DamageSelf(damage, this);
                    ImmuneEntities.Add(entity);
                    if (explosionProgress == -1)
                        BeginExplosion();
                }
            }
        }
    }
}

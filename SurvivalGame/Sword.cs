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
        //public override Rectangle Drawing
        //{
        //    get => new Rectangle((int)Owner.Hitbox.X, (int)Owner.Hitbox.Y, Hitbox.Width, 2);
        //    //get => new Rectangle((int)Hitbox.Left, (int)Hitbox.Top, Hitbox.Width, Hitbox.Height);
        //}
        public Sword(TextureName texture, Entity owner, float rotation, int damage)
        {
            Damage = damage;
            Collision = false;
            Mass = 1;
            Hitbox = new Circle(owner.Hitbox.X, owner.Hitbox.Y, (owner.Hitbox.Width + owner.Hitbox.Height) / 2);
            //Hitbox = new Rect(player.Hitbox.X, player.Hitbox.Y, 150, 150);
            //Texture = texture;
            Owner = owner;

            //right
            if(rotation >= -0.8 && rotation < 0.8)
            {
                StartingRotation = -2.4f;
                Direction = Direction.Left;
            }
            //down
            else if (rotation >= 0.8 && rotation < 2.4)
            {
                StartingRotation = -0.8f;
                Direction = Direction.Down;
            }
            //up
            else if (rotation >= -2.4 && rotation < -0.8)
            {
                StartingRotation = 2.4f;
                Direction = Direction.Up;
            }
            //right
            else if (rotation >= 2.4 || rotation < -2.4)
            {
                StartingRotation = 0.8f;
                Direction = Direction.Right;
            }
            //Rotation = StartingRotation;
            UpdateCoord();

            Drawing = new Drawing(texture, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), Color.White, StartingRotation,
                new Vector2((float)Hitbox.Width, 2f), 0.5f, true);
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
        }
        private void UpdateCoord()
        {
            switch (Direction)
            {
                case Direction.Right:
                    Hitbox.Left = Owner.Hitbox.X;
                    break;
                case Direction.Up:
                    Hitbox.Top = Owner.Hitbox.Y;
                    break;
                case Direction.Down:
                    Hitbox.Bottom = Owner.Hitbox.Y;
                    break;
                case Direction.Left:
                    Hitbox.Right = Owner.Hitbox.X;
                    break;
            }
        }
        private void UpdateSize()
        {
            (Hitbox as Circle).Diameter = (Owner.Hitbox.Width + Owner.Hitbox.Height) / 2;
        }
    }
}

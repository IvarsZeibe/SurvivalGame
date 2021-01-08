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
        public List<Entity> hitEntities = new List<Entity>();
        float StartingRotation { get; set; }
        Direction Direction { get; set; }
        Entity Player { get; set; }
        float timeAlive;
        float timeTillDeath = 0.2f;
        public int Damage { get; set; }
        public override Rectangle Drawing
        {
            get => new Rectangle((int)Player.Hitbox.X, (int)Player.Hitbox.Y, 150, 2);
            //get => new Rectangle((int)Hitbox.Left, (int)Hitbox.Top, Hitbox.Width, Hitbox.Width);
        }
        public Sword(Texture2D texture, Entity player, float rotation)
        {
            Damage = 100;
            Collision = false;
            Mass = 1f;
            Hitbox = new Circle(player.Hitbox.X, player.Hitbox.Y, 150);
            //Hitbox = new Rect(player.Hitbox.X, player.Hitbox.Y, 150, 150);
            Texture = texture;
            Player = player;

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
            Rotation = StartingRotation;
            UpdateCoord();
        }

        public void Update(GameTime gameTime)
        {
            timeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Rotation = timeAlive * -1.8f / timeTillDeath + StartingRotation;
            if (timeAlive > timeTillDeath)
            {
                isDead = true;
            }
            UpdateCoord();
        }
        private void UpdateCoord()
        {
            switch (Direction)
            {
                case Direction.Right:
                    Hitbox.Left = Player.Hitbox.X;
                    break;
                case Direction.Up:
                    Hitbox.Top = Player.Hitbox.Y;
                    break;
                case Direction.Down:
                    Hitbox.Bottom = Player.Hitbox.Y;
                    break;
                case Direction.Left:
                    Hitbox.Right = Player.Hitbox.X;
                    break;
            }

        }
    }
}

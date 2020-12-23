using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Sword:Entity
    {
        public List<Entity> hitEntities = new List<Entity>();
        public Rectangle DrawRect;
        Point Hitbox { get; set; }
        float StartingRotation { get; set; }
        public float Rotation { get; set; }
        int Direction { get; set; }
        Entity Player { get; set; }
        float timeAlive;
        float timeTillDeath = 0.2f;
        public int Damage { get; set; }
        public Sword(Texture2D texture, Entity player, float rotation)
        {
            Damage = 100;
            Collision = false;
            Mass = 1f;
            Hitbox = new Point(80, 60);
            Texture = texture;
            Size = new Point(80, 2);
            Player = player;
            //right
            if(rotation >= -0.8 && rotation < 0.8)
            {
                StartingRotation = -2.4f;
                Direction = 0;
            }
            //down
            else if (rotation >= 0.8 && rotation < 2.4)
            {
                StartingRotation = -0.8f;
                Direction = 1;
            }
            //up
            else if (rotation >= -2.4 && rotation < -0.8)
            {
                StartingRotation = 2.4f;
                Direction = 2;
            }
            //right
            else if (rotation >= 2.4 || rotation < -2.4)
            {
                StartingRotation = 0.8f;
                Direction = 3;
            }
            Rotation = StartingRotation;
        }
        public void Update(GameTime gameTime)
        {
            //Force = Mass * Speed;
            timeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;

            X = Player.Center.X;
            Y = Player.Center.Y;
            Rotation = timeAlive * -1.8f / timeTillDeath + StartingRotation;
            DrawRect = new Rectangle((int)X, (int)Y, Size.X, Size.Y);
            if (timeAlive > timeTillDeath)
            {
                isDead = true;
            }
            switch (Direction)
            {
                case 3:
                    Rect = new Rectangle(Player.Rect.Right, (int)Y - Hitbox.X / 2, Hitbox.Y, Size.X);
                    break;
                case 2:
                    Rect = new Rectangle((int)X - Hitbox.X / 2, Player.Rect.Bottom, Hitbox.X, Hitbox.Y);
                    break;
                case 1:
                    Rect = new Rectangle((int)X - Hitbox.X / 2, Player.Rect.Top - Hitbox.Y, Hitbox.X, Hitbox.Y);
                    break;
                case 0:
                    Rect = new Rectangle(Player.Rect.Left - Hitbox.Y, (int)Y - Hitbox.X / 2, Hitbox.Y, Size.X);
                    break;
            }
            ////right
            //else if (Rotation >= -0.8 && Rotation < 0.8)
            //{
            //    Rect = new Rectangle(Player.Rect.Right, (int)Y - Hitbox.X / 2, Hitbox.Y, Hitbox.X);
            //}
            ////down
            //else if (Rotation >= 0.8 && Rotation < 2.4)
            //{
            //    Rect = new Rectangle((int)X - Hitbox.X / 2, Player.Rect.Bottom, Hitbox.X, Hitbox.Y);
            //}
            ////up
            //else if (Rotation >= -2.4 && Rotation < -0.8)
            //{
            //    Rect = new Rectangle((int)X - Hitbox.X / 2, Player.Rect.Top - Hitbox.Y, Hitbox.X, Hitbox.Y);
            //}
            ////left
            //else if (Rotation >= 2.4 || Rotation < -2.4)
            //{
            //    Rect = new Rectangle(Player.Rect.Left - Hitbox.Y, (int)Y - Hitbox.X / 2, Hitbox.Y, Size.X);
            //}
        }
    }
}

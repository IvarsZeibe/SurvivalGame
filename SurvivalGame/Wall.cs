using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Wall : Entity
    {

        bool ghost;
        float timeAlive;
        public Wall(Texture2D texture, double x, double y, bool collision = true)
        {
            Mass = 19;
            Texture = texture;
            Collision = collision;
            ghost = !collision;
            Hitbox = new Rect(x, y, 50, 50);

        }
        public override void Update(GameTime gameTime)
        {
            timeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(ghost && timeAlive > 0.1)
            {
                isDead = true;
            }
        }
    }
}

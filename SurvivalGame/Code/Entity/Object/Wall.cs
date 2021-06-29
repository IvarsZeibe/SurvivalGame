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

        public bool ghost { get; set; }
        public float timeAlive { get; set; }
        Wall() { }
        public Wall(TextureName texture, double x, double y, bool collision = true)
        {
            Mass = 19;
            this.Collision = collision;
            this.ghost = !collision;
            Hitbox = new Rect(x, y, 50, 50);
            Color color = Color.SaddleBrown;
            if (ghost)
                color = Color.SandyBrown;
            Drawing = new Drawing(texture, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), color, 0f, new Vector2(50, 50), isDrawn: true);

        }
        public override void Update(GameTime gameTime)
        {
            timeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(ghost && timeAlive > 0.1)
            {
                Kill();
            }
        }
    }
}

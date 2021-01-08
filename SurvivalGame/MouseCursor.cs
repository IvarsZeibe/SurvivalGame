using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class MouseCursor : Entity
    {
        public MouseCursor(Texture2D texture)
        {
            LayerDepth = 0f;
            Collision = false;
            Texture = texture;
            Hitbox = new Rect(0, 0, 3, 3);
        }
        public void Update(MouseState mstate)
        {
            Hitbox.X = mstate.X;
            Hitbox.Y = mstate.Y;
        }
    }
}

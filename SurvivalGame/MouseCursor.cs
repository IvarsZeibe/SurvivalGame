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
        Point Size;
        public MouseCursor(Texture2D texture)
        {
            Collision = false;
            Size = new Point(3, 3);
            Texture = texture;
        }
        public void Update(MouseState mstate)
        {
            X = mstate.X;
            Y = mstate.Y;

            Center = new Vector2((float)X + Size.X / 2, (float)Y + Size.Y / 2);
            Rect = new Rectangle((int)X, (int)Y, Size.X, Size.Y);
        }
    }
}

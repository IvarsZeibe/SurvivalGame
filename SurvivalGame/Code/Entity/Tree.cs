using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Tree : Entity
    {
        public Tree(double x, double y) : base(false)
        {
            Collision = true;
            Mass = 19;
            //Texture2D texture = Globals.Textures[TextureName.PineTree.ToString()];
            Hitbox = new Rect(x, y, 30, 30);
            Drawing = new Drawing(TextureName.PineTree, Hitbox.GetTopLeftPosVector() - new Vector2(60, 120), Color.White, 0f, new Vector2(150, 150), 0.3f - (float)y / 100000);
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    interface IDrawing
    {
        TextureName Texture { get; set; }
        Vector2 Position { get; set; }
        Color Color { get; set; }
        float Rotation { get; set; }
        Vector2 Scale { get; set; }
        float LayerDepth { get; set; }
        public void Update(GameTime gameTime);
    }
}

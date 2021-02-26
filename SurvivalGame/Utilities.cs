using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Utilities
    {
        public static Texture2D CreateTexture(Color color, GraphicsDevice GraphicsDevice)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] data = new Color[1] { color };
            texture.SetData(data);
            return texture;
        }
        public static List<Drawing> CreateEmptyRectDrawings(Vector2 position, Color color, Vector2 scale/* 0, new Vector2(Drawing.Scale.X / 10, Drawing.Scale.Y), 0.2f*/)
        {
            List<Drawing> borders = new List<Drawing>();
            borders.Add(new Drawing(TextureName.Rectangle, new Vector2(position.X, position.Y), color, 0f, new Vector2(scale.X, 1), 0.2f));
            borders.Add(new Drawing(TextureName.Rectangle, new Vector2(position.X, position.Y + scale.Y), color, 0f, new Vector2(scale.X, 1), 0.2f));
            borders.Add(new Drawing(TextureName.Rectangle, new Vector2(position.X, position.Y), color, 0f, new Vector2(1, scale.Y), 0.2f));
            borders.Add(new Drawing(TextureName.Rectangle, new Vector2(position.X + scale.X, position.Y), color, 0f, new Vector2(1, scale.Y), 0.2f));
            return borders;
            //Texture2D texture = CreateTexture(color);
            //Color[] data = new Color[1] { color };
            //texture.SetData(data);
            //return texture;
        }
    }
}

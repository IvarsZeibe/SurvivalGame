using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Drawing
    {
        //public Drawing()
        //{
        //    Globals.Drawings.Add(this);
        //}
        public Drawing(TextureName texture, Vector2 position, Color color, float rotation, Vector2 scale, float layerDepth = 0.5f, bool isDrawn = true)
        {
            this.Texture = texture;
            this.Coord = position;
            this.Color = color;
            this.Rotation = rotation;
            if(scale != Vector2.Zero)
                this.Scale = scale;
            this.LayerDepth = layerDepth;
            this.IsDrawn = isDrawn;

            Globals.Drawings.Add(this);
        }
        //public Drawing(TextureName texture, Vector2 position, Color color, float rotation, Point scale, float layerDepth = 0.5f, bool isDrawn = false)
        //{
        //    this.Texture = texture;
        //    this.Position = position;
        //    this.Color = color;
        //    this.Rotation = rotation;
        //    this.Scale = new Vector2((float)scale.X / Globals.Textures[texture].Width, (float)scale.Y / Globals.Textures[texture].Height);
        //    this.LayerDepth = layerDepth;
        //    this.IsDrawn = isDrawn;
        //    Globals.Drawings.Add(this);
        //}
        public Vector2 originPercentage = Vector2.Zero;
        public Vector2 Origin
        {
            get => new Vector2(Globals.Textures[Texture.ToString()].Width, Globals.Textures[Texture.ToString()].Height) * originPercentage;
        }
        public TextureName Texture { get; set; } = TextureName.Rectangle;
        public Vector2 Coord { get; set; } = Vector2.Zero;
        public Vector2 Position 
        {
            get => Coord + Offset;
            set => Coord = value - Offset; 
        }
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        public float Rotation { get; set; } = 0f;
        public Vector2 scale = new Vector2(1,1);
        public Vector2 Scale
        {
            get => scale;
            set
            {
                //if (Texture == TextureName.Rectangle)
                //    scale = value;
                //else
                    scale = new Vector2((float)value.X / Globals.Textures[Texture.ToString()].Width, (float)value.Y / Globals.Textures[Texture.ToString()].Height);
            }
        }
        public float LayerDepth { get; set; } = 0.5f;
        public bool IsDrawn { get; set; } = true;
        //public bool IsDead { get; set; } = false;
        public float GetWidth()
        {
            return Globals.Textures[Texture.ToString()].Width * scale.X;
        }
        public float GetHeight()
        {
            return Globals.Textures[Texture.ToString()].Height * scale.Y;
        }
    }
}

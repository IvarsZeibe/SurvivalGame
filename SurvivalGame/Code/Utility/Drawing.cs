using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Drawing
    {
        Drawing() { }
        public Drawing(TextureName texture, Vector2 position, Color color, float rotation, Vector2 scale, float layerDepth = 0.5f, bool isDrawn = true)
        {
            this.Texture = texture;
            this.Coord = position;
            this.Color = color;
            this.Rotation = rotation;
            if (scale != Vector2.Zero)
                this.Scale = scale;
            this.LayerDepth = layerDepth;
            if (isDrawn)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
        public Drawing(string texture, Vector2 position, Color color, float rotation, Vector2 scale, float layerDepth = 0.5f, bool isDrawn = true)
        {
            this.TextureStr = texture;
            this.Coord = position;
            this.Color = color;
            this.Rotation = rotation;
            if (scale != Vector2.Zero)
                this.Scale = scale;
            this.LayerDepth = layerDepth;
            if (isDrawn)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
        public Vector2 originPercentage { get; set; } = Vector2.Zero;
        [System.Text.Json.Serialization.JsonIgnore]
        public Vector2 Origin
        {
            get => new Vector2(Globals.Textures[Texture.ToString()].Width, Globals.Textures[Texture.ToString()].Height) * originPercentage;
        }
        public TextureName Texture { get; set; } = TextureName.Rectangle;
        public string TextureStr { get; set; } = "none";
        public Vector2 Coord { get; set; } = Vector2.Zero;
        [System.Text.Json.Serialization.JsonIgnore]
        public Vector2 Position
        {
            get => Coord + Offset;
            set => Coord = value - Offset;
        }
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        public float Rotation { get; set; } = 0f;
        public Vector2 scale { get; set; } = new Vector2(1, 1);
        [System.Text.Json.Serialization.JsonIgnore]
        public Vector2 Scale
        {
            get => scale;
            set
            {
                if(TextureStr == "none")
                    scale = new Vector2((float)value.X / Globals.Textures[Texture.ToString()].Width, (float)value.Y / Globals.Textures[Texture.ToString()].Height);
                else
                    scale = new Vector2((float)value.X / Globals.Textures[TextureStr].Width, (float)value.Y / Globals.Textures[TextureStr].Height);
            }
        }
        public float LayerDepth { get; set; } = 0.5f;
        bool isDrawn { get; set; } = false;
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsDrawn { get => isDrawn; }
        public float GetWidth()
        {
            return Globals.Textures[Texture.ToString()].Width * scale.X;
        }
        public float GetHeight()
        {
            return Globals.Textures[Texture.ToString()].Height * scale.Y;
        }
        public Vector2 Size { get => new Vector2(GetWidth(), GetHeight()); }
        public void Enable()
        {
            if (!IsDrawn)
            {
                if (Globals.Drawings.Contains(this))
                    throw new Exception("Drawing already is drawn");
                Globals.Drawings.Add(this);
                isDrawn = true;
            }
        }
        public void Disable()
        {
            if (IsDrawn)
            {
                Globals.Drawings.Remove(this);
                isDrawn = false;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Chat : IDrawing
    {
        public TextureName Texture { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        private Vector2 scale = new Vector2(1,1);
        public Vector2 Scale 
        {
            get => scale;
            set
            {
                if (Texture == TextureName.Rectangle)
                    scale = value;
                else
                    scale = new Vector2(Globals.Textures[Texture].Width / value.X, Globals.Textures[Texture].Width / value.X);
            }
        }
        public float LayerDepth { get; set; }
        TextBox TextBox { get; set; }
        public Chat(GraphicsDeviceManager graphics)
        {
            EntityTracker.Drawings.Add(this);

            this.Texture = TextureName.Rectangle;
            this.Position = new Vector2(100, graphics.PreferredBackBufferHeight - 100);
            this.Color = new Color(0,0,0,100);
            this.Rotation = 0f;
            this.Scale = new Vector2(500, 50);
            this.LayerDepth = 0.2f;
            this.Texture = TextureName.Rectangle;
            this.TextBox = new TextBox()
            {
                SpriteFont = SpriteFontName.Chat,
                Text = new StringBuilder("You can write here"),
                Position = this.Position,
                Color = Color.White,
                Rotation = 0f,
                //Scale = new Vector2(8,20),
                LayerDepth = this.LayerDepth - 0.1f,
                IsActive = true
            };
        }
        public void Update(GameTime gameTime)
        {
            if (TextBox.IsActive)
            {
                TextBox.Input(gameTime);
                //EntityTracker.AreEntitiesActive = false;
                Globals.IsTextBoxActive = true;
            }
            else
                Globals.IsTextBoxActive = false;
                //EntityTracker.AreEntitiesActive = true;


        }
        //public StringBuilder Text { get; set; } = new StringBuilder();
        //public SpriteFont Font { get; set; }
        //public Vector2 TextPosition { get; set; }

        //UI(Texture2D texture, GraphicsDeviceManager graphics)
        //{
        //    this.Texture = texture;
        //    this.Hitbox = new Rect(10, graphics.PreferredBackBufferHeight - 10, 500, 20);
        //    //    this.Collision = collision;
        //    //    this.Health = health;
        //    //    this.Mass = mass;
        //    Color color = Color.Black;
        //    color.A = 50;
        //    this.Color = color;
        //    //    this.Speed = speed;
        //    this.Rotation = 0;
        //    this.LayerDepth = 0.1f;
        //}

    }
}

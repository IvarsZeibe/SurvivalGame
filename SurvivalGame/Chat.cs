using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Chat : IDrawing, IUpdate
    {
        private Vector2 TextPosition { get; set; }
        TextBox ActiveText { get; set; }
        List<TextBox> WrittenText { get; set; } = new List<TextBox>();

        private bool updateEnabled = false;
        public bool UpdateEnabled
        {
            get => updateEnabled;
            set
            {
                if (value && !updateEnabled)
                {
                    EntityTracker.ObjectsWithUpdate.Add(this);
                    updateEnabled = true;
                }
                else if (!value && updateEnabled)
                {
                    EntityTracker.ObjectsWithUpdate.Remove(this);
                    updateEnabled = false;
                }
            }
        }

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
        private bool isDrawn = false;
        public bool IsDrawn
        {
            get => isDrawn;
            set
            {
                if (value && !isDrawn)
                {
                    EntityTracker.Drawings.Add(this);
                    isDrawn = true;
                    foreach(var line in WrittenText)
                    {
                        line.IsDrawn = true;
                    }
                    NewLine();
                }
                else if (!value && isDrawn)
                {
                    EntityTracker.Drawings.Remove(this);
                    isDrawn = false;
                    foreach (var line in WrittenText)
                    {
                        line.IsDrawn = false;
                    }
                }
            }
        }

        public Chat(GraphicsDeviceManager graphics)
        {
            this.Texture = TextureName.Rectangle;
            this.Scale = new Vector2(500, 170);
            this.Position = new Vector2(100, graphics.PreferredBackBufferHeight - 400);
            this.TextPosition = new Vector2(this.Position.X, this.Position.Y + GetHeight());
            this.Color = new Color(0,0,0,100);
            this.Rotation = 0f;
            this.LayerDepth = 0.2f;

            EntityTracker.ObjectsWithUpdate.Add(this);
            
        }
        public void NewLine()
        {
            UpdateEnabled = true;
            if (ActiveText != null && ActiveText.Text.Length > 0)
            {
                WrittenText.Add(ActiveText);
                if(WrittenText.Count > 6)
                {
                    WrittenText[0].IsDrawn = false;
                    WrittenText.RemoveAt(0);
                }
                foreach(var line in WrittenText)
                {
                    line.Position = new Vector2(line.Position.X, line.Position.Y - line.GetHeight());
                }
            }
            this.ActiveText = new TextBox()
            {
                SpriteFont = SpriteFontName.Chat,
                Text = new StringBuilder("You can write here"),
                Position = new Vector2(this.TextPosition.X, this.TextPosition.Y - Globals.SpriteFonts[SpriteFontName.Chat].MeasureString("A").Y),
                Color = Color.White,
                Rotation = 0f,
                //Scale = new Vector2(80,150),
                LayerDepth = this.LayerDepth - 0.1f,
                IsDrawn = true
            };
        }
        public void Update(GameTime gameTime)
        {
            if (ActiveText != null)
            {
                if(ActiveText.Input(gameTime, this))
                    Globals.IsUserWriting = true;
                else
                {
                    if (this.isDrawn)
                    {
                        NewLine();
                    }
                    else
                    {
                        ActiveText = null;
                        Globals.IsUserWriting = false;

                        UpdateEnabled = false;
                    }
                }
            }
            else
                Globals.IsUserWriting = false;
        }

        private float GetWidth()
        {
            return (Globals.Textures[this.Texture].Width * Scale.X);
        }
        private float GetHeight()
        {
            return (Globals.Textures[this.Texture].Height * Scale.Y);
        }
    }
}

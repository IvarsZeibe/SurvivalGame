﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Chat : IUpdate
    {
        private Vector2 TextPosition { get; set; }
        TextBox ActiveText { get; set; }
        List<TextBox> WrittenText { get; set; } = new List<TextBox>();

        public bool UpdateEnabled { get; set; } = true;
        private Vector2 Position { get; set; }
        public Drawing Drawing { get; set; }
        public bool IsDead { get; set; } = false;
        public Chat(GraphicsDeviceManager graphics)
        {
            this.Position = new Vector2(100, graphics.PreferredBackBufferHeight - 400);
            this.UpdateEnabled = false;
            Globals.Updatables.Add(this);

            this.Drawing = new Drawing(
                TextureName.Rectangle,
                Position/*new Vector2(100, graphics.PreferredBackBufferHeight - 400)*/,
                new Color(0, 0, 0, 100),
                0f,
                new Vector2(500, 170),
                0.35f,
                false
            );
            this.TextPosition = new Vector2(this.Position.X, this.Position.Y + GetHeight());
        }
        public void Update(GameTime gameTime)
        {
            if (ActiveText != null)
            {
                if (ActiveText.Input(gameTime))
                    Globals.IsUserWriting = true;
                else
                {
                    if (Globals.IsUserWriting)
                    {
                        NewLine();
                    }
                    else
                    {
                        ActiveText = null;
                        foreach (var line in WrittenText)
                        {
                            line.DrawingText.IsDrawn = false;
                        }
                        Drawing.Disable();
                        UpdateEnabled = false;
                    }
                }
            }
            else
                Globals.IsUserWriting = false;
        }
        public void NewLine()
        {
            UpdateEnabled = true;
            Drawing.Enable();
            if (ActiveText != null)
            {
                if (ActiveText.DrawingText.Text.Length > 0)
                {
                    if(ActiveText.DrawingText.Text[0] == '/')
                    {
                        ActiveText.DrawingText.Text = Globals.Command.CreateCommand(ActiveText.DrawingText.Text);
                    }
                    WrittenText.Add(ActiveText);
                    if (WrittenText.Count > 6)
                    {
                        Globals.DrawingTexts.Remove(WrittenText[0].DrawingText);
                        WrittenText.RemoveAt(0);
                    }
                    foreach (var line in WrittenText)
                    {
                        line.DrawingText.Position = new Vector2(line.DrawingText.Position.X, line.DrawingText.Position.Y - line.GetHeight());
                    }
                }
                else
                    Globals.DrawingTexts.Remove(ActiveText.DrawingText);
            }
            foreach(var line in WrittenText)
            {
                line.DrawingText.IsDrawn = true;
            }
            this.ActiveText = new TextBox()
            {
                DrawingText = new DrawingText()
                {
                    SpriteFont = SpriteFontName.Aerial16,
                    Text = new StringBuilder(""),
                    Position = new Vector2(this.TextPosition.X, this.TextPosition.Y - Globals.SpriteFonts[SpriteFontName.Aerial16].MeasureString("A").Y),
                    Color = Color.White,
                    Rotation = 0f,
                    LayerDepth = 0.2f,
                    IsDrawn = true
                }
            };
        }

        private float GetWidth()
        {
            return (Globals.Textures[Drawing.Texture.ToString()].Width);
        }
        private float GetHeight()
        {
            return (Globals.Textures[Drawing.Texture.ToString()].Height * Drawing.Scale.Y);
        }
    }
}
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Button
    {
        public bool IsActive { get; set; } = true;
        public Hitbox Hitbox { get; set; }
        private Drawing Background;
        private DrawingText Text;
        public Button(Hitbox hitbox, Color backgroundColor)
        {
            Hitbox = hitbox;
            Background = new Drawing(
                TextureName.Rectangle, Hitbox.GetTopLeftPosVector(),
                backgroundColor, 0f, Hitbox.GetScaleVector(), 0.1f);
        }
        public Button(Hitbox hitbox, Color backgroundColor, StringBuilder text, Color textColor)
        {
            Hitbox = hitbox;
            Background = new Drawing(
                TextureName.Rectangle, Hitbox.GetTopLeftPosVector(),
                backgroundColor, 0f, Hitbox.GetScaleVector(), 0.1f);
            Text = new DrawingText(SpriteFontName.Aerial16, text, new Vector2((float)Hitbox.X, (float)Hitbox.Y), textColor, 0f, new Vector2(1,1), 0.09f, true);
        }
        public void Activate()
        {
            IsActive = true;
            Background.IsDrawn = true;
            if (!(Text is null))
                Text.IsDrawn = true;
        }
        public void Deactivate()
        {
            IsActive = false;
            Background.IsDrawn = false;
            if (!(Text is null))
                Text.IsDrawn = false;
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class HealthBar : IUpdate
    {
        private readonly Vector2 maxScale;
        public HealthBar(Entity owner)
        {
            this.Owner = owner;
            this.UpdateEnabled = true;
            Globals.Updatables.Add(this);
            maxScale = new Vector2(50, 3f);
            this.Drawing = new Drawing 
            (
                TextureName.Rectangle,
                new Vector2((float)Owner.Hitbox.X - (maxScale.X / 2), (float)Owner.Hitbox.Top - 20f),
                Color.Red,
                0f,
                maxScale,
                0.1f,
                true
            );
            //Drawing.Position = Drawing.Position - new Vector2(Drawing.Scale.X / 2, 0);
            //this.DrawingText = new DrawingText
            //    (
            //        SpriteFontName.Aerial16,
            //        new StringBuilder(Owner.Health),
            //        Drawing.Position,
            //        Color.White,
            //        0f,
            //        new Vector2(1, 1),
            //        0.05f,
            //        true
            //    );
        }
        public bool UpdateEnabled { get; set; }
        private Entity Owner { get; set; }
        public Drawing Drawing { get; set; }
        public DrawingText DrawingText { get; set; }
        public bool IsDead { get; set; } = false;

        public void Update(GameTime gameTime)
        {
            if (Owner.IsDead)
            {
                IsDead = true;
                Globals.Drawings.Remove(Drawing);
                //Drawing.IsDead = true;
                //DrawingText.IsDead = true;
            }
            else
            {
                this.Drawing.Scale = new Vector2((float)Owner.Health / Owner.MaxHealth * 50, 3f);
                this.Drawing.Position = new Vector2((float)Owner.Hitbox.X - maxScale.X / 2, (float)Owner.Hitbox.Top - 20f);
                //this.DrawingText.Position = this.Drawing.Position;
                //this.DrawingText.Text.Clear();
                //this.DrawingText.Text.Append(Owner.Health);
            }
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class HealthBar
    {
        public Vector2 maxScale { get; set; }
        HealthBar() { }
        public HealthBar(Entity owner, bool isDrawn = true)
        {
            //this.Owner = owner;
            maxScale = new Vector2(50, 3f);
            this.Drawing = new Drawing 
            (
                TextureName.Rectangle,
                new Vector2((float)owner.Hitbox.X - (maxScale.X / 2), (float)owner.Hitbox.Top - 20f),
                Color.Red,
                0f,
                maxScale,
                0.36f,
                isDrawn
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
        //public Entity Owner { get; set; }
        public Drawing Drawing { get; set; }
        public DrawingText DrawingText { get; set; }
        public void Load() 
        {
            Drawing.Enable();
        }
        public void UnLoad()
        {
            Drawing.Disable();
        }

        public void Update(Entity Owner)
        {
            //if (Owner.IsDead)
            //{
            //    Globals.Drawings.Remove(Drawing);
            //    IsDead = true;
            //    UpdateEnabled = false;
            //    //Drawing.IsDead = true;
            //    //DrawingText.IsDead = true;
            //}
            //else
            //{
                this.Drawing.Scale = new Vector2((float)Owner.Health / Owner.MaxHealth * 50, 3f);
                this.Drawing.Position = new Vector2((float)Owner.Hitbox.X - maxScale.X / 2, (float)Owner.Hitbox.Top - 20f);
                //this.DrawingText.Position = this.Drawing.Position;
                //this.DrawingText.Text.Clear();
                //this.DrawingText.Text.Append(Owner.Health);
            //}
        }
    }
}

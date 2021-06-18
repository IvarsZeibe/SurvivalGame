using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    abstract class UIElement
    {
        protected Color color = Color.White;
        protected string textureName = "Rectangle";
        public (bool, MouseKey) holding = (false, MouseKey.LeftButton);
        public Hitbox Hitbox;
        public virtual void Update(GameTime gameTime)
        {
            OnDefault();
            if (Globals.MouseCursor.Hitbox.CollidesWith(Hitbox))
            {
                OnHover();
            }
            if (holding.Item1)
            {
                OnHolding();
            }
            if (holding.Item1 && !Globals.PressedMouseKeys.Contains(holding.Item2))
            {
                holding.Item1 = false;
                holding.Item2 = MouseKey.LeftButton;
                OnRelease();
            }
            if (Globals.MouseCursor.Hitbox.CollidesWith(Hitbox))
            {
                if (Globals.NewMouseKeys.Count > 0)
                {
                    ClickEvent();
                }
            }
        }
        protected virtual void OnDefault() { }
        protected virtual void OnHolding() { }
        protected virtual void OnRelease() { }
        protected virtual void OnHover() { }
        protected virtual void ClickEvent() { }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Globals.Textures[textureName], Hitbox.ToRectangle(), null, color, 0f, Vector2.Zero, SpriteEffects.None, 0.201f);
        }
    }
}

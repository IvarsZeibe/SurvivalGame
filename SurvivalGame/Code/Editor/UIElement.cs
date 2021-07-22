using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    abstract class UIElement
    {
        protected Color color = new Color(240, 240, 240);
        public float layerDepth = 0.201f;
        public string textureName = "Rectangle";
        public int borderWidth = 0;
        public Color borderColor = new Color(240, 240, 240);
        public (bool, MouseKey) holding = (false, MouseKey.LeftButton);
        public bool selected = false;
        public Hitbox Hitbox;
        public virtual void Update(GameTime gameTime)
        {
            OnDefault();
            if (Globals.MouseCursor.Hitbox.CollidesWith(Hitbox) && AdditionalClickAndHoverCheck())
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
            if (Globals.MouseCursor.Hitbox.CollidesWith(Hitbox) && AdditionalClickAndHoverCheck())
            {
                if (Globals.NewMouseKeys.Count > 0)
                {
                    ClickEvent(gameTime);
                }
            }
        }
        protected virtual void OnDefault() { }
        protected virtual void OnHolding() { }
        protected virtual void OnRelease() { }
        protected virtual void OnHover() { }
        public Func<bool> AdditionalClickAndHoverCheck = new Func<bool>(() => true);
        protected virtual void ClickEvent(GameTime gameTime) { }
        public Action clickAction = () => { };
        public virtual void LoseFocus()
        {
            if (selected)
            {
                OnLostFocus();
                lostFocusAction();
            }
            selected = false;
        }
        protected virtual void OnLostFocus() { }
        public Action lostFocusAction = () => { };
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Globals.Textures[textureName], Hitbox.ToRectangle(), null, color, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
            if(borderWidth > 0)
                spriteBatch.Draw(Globals.Textures[textureName], (Hitbox + new Rect(0, 0, 2 * borderWidth, 2 * borderWidth)).ToRectangle(), null, borderColor, 0f, Vector2.Zero, SpriteEffects.None, layerDepth + 0.001f);
        }
    }
}
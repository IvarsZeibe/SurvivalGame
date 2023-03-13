using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    abstract class UIElement
    {
        protected Color color;
        public Color defaultColor = new Color(240, 240, 240);
        public float layerDepth = 0.201f;
        public string textureName = "Rectangle";
        public int DefaultBorderWidth = 0;
        public int borderWidth;
        public Color borderColor = new Color(240, 240, 240);
        protected bool selected = false;
        public Hitbox Hitbox;
        public virtual void Update(GameTime gameTime)
        {
            SetDefault();
            isHoverd = Globals.MouseCursor.Hitbox.CollidesWith(Hitbox) && AdditionalClickAndHoverCheck();
            if (isHoverd)
            {
                if (layerDepth <= Globals.Editor.HoveredElementAction.layerDepth)
                {
                    Globals.Editor.HoveredElementAction.action = OnHover;
                    Globals.Editor.HoveredElementAction.args = new EventArgs();
                    Globals.Editor.HoveredElementAction.layerDepth = layerDepth;
                }
            }
            if (isPressed)
            {
                if (Globals.PressedMouseKeys.Contains(MouseKey.LeftButton))
                {
                    Globals.Editor.HoveredElementAction.action = OnHeld;
                    Globals.Editor.HoveredElementAction.args = new EventArgs();
                    Globals.Editor.HoveredElementAction.layerDepth = layerDepth;
                }
                else
                {
                    Globals.Editor.HoveredElementAction.action = OnRelease;
                    Globals.Editor.HoveredElementAction.args = new EventArgs();
                    Globals.Editor.HoveredElementAction.layerDepth = layerDepth;

                }
            }
            if (Globals.NewMouseKeys.Contains(MouseKey.LeftButton))
            {
                if (isHoverd)
                {
                    if (layerDepth <= Globals.Editor.ClickedElementAction.layerDepth)
                    {
                        Globals.Editor.ClickedElementAction.action = OnClick;
                        Globals.Editor.ClickedElementAction.args = new EventArgs();
                        Globals.Editor.ClickedElementAction.layerDepth = layerDepth;
                    }
                }

            }
            if (selected)
                OnFocus(new EventArgs());
        }
        public Func<bool> AdditionalClickAndHoverCheck = new Func<bool>(() => true);
        protected virtual void SetDefault() 
        {
            color = defaultColor;
            borderWidth = DefaultBorderWidth;
        }
        protected virtual void OnHeld(EventArgs e)
        {
            EventHandler handler = Holding;
            handler?.Invoke(this, e);
        }
        public event EventHandler Holding;
        protected virtual void OnRelease(EventArgs e)
        {
            isPressed = false;
            EventHandler handler = Release;
            handler?.Invoke(this, e);
        }
        public event EventHandler Release;
        bool isHoverd = false;
        protected virtual void OnHover(EventArgs e)
        {
            EventHandler handler = Hover;
            handler?.Invoke(this, e);
        }
        public event EventHandler Hover;
        bool isPressed = false;
        protected virtual void OnClick(EventArgs e)
        {
            isPressed = true;
            foreach (var element in Globals.Editor.UIElements)
            {
                if (element.Value != this)
                    element.Value.Unfocus();
            }
            selected = true;
            EventHandler handler = Click;
            handler?.Invoke(this, e);
        }
        public event EventHandler Click;
        protected virtual void OnFocus(EventArgs e)
        {
            EventHandler handler = Focus;
            handler?.Invoke(this, e);
        }
        public event EventHandler Focus;
        public virtual void Unfocus()
        {
            selected = false;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Globals.Textures[textureName], Hitbox.ToRectangle(), null, color, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
            if(borderWidth > 0)
                spriteBatch.Draw(Globals.Textures[textureName], (Hitbox + new Rect(0, 0, 2 * borderWidth, 2 * borderWidth)).ToRectangle(), null, borderColor, 0f, Vector2.Zero, SpriteEffects.None, layerDepth + 0.001f);
        }
        public virtual void PrepareDraw(SpriteBatch spriteBatch) { }
    }
}
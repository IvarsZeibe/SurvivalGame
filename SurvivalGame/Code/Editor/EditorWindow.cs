using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class EditorWindow : UIElement
    {
        protected Dictionary<string, UIElement> elements = new Dictionary<string, UIElement>();
        protected RenderTarget2D drawing;
        public EditorWindow(int x, int y, int width, int height, bool topLeft = false)
        {
            Hitbox = new Rect(x, y, width, height, topLeft);
            drawing = new RenderTarget2D(Globals.graphics.GraphicsDevice, width, height);
        }
        public override void Update(GameTime gameTime)
        {
            foreach(var element in elements.Values)
            {
                element.Update(gameTime);  
            }
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(drawing, Hitbox.GetTopLeftPosVector(), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, layerDepth);
            //base.Draw(spriteBatch);
            spriteBatch.Draw(drawing, Hitbox.ToRectangle(), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
            if (borderWidth > 0)
                spriteBatch.Draw(Globals.Textures["Rectangle"], (Hitbox + new Rect(0, 0, 2 * borderWidth, 2 * borderWidth)).ToRectangle(), null, borderColor, 0f, Vector2.Zero, SpriteEffects.None, layerDepth + 0.001f);

        }
        public override void PrepareDraw(SpriteBatch spriteBatch)
        {
            if (drawing != null)
            {
                foreach (var element in elements.Values)
                {
                    element.PrepareDraw(spriteBatch);
                }
                Globals.graphics.GraphicsDevice.SetRenderTarget(drawing);
                Globals.graphics.GraphicsDevice.Clear(color);
                spriteBatch.Begin();

                DrawOnRenderTarget(spriteBatch);

                spriteBatch.End();
                Globals.graphics.GraphicsDevice.SetRenderTarget(null);
            }
        }
        protected virtual void DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            foreach (var element in elements.Values)
            {
                element.Hitbox.AddToCoords(-Hitbox.Left, -Hitbox.Top);
                element.Draw(spriteBatch);
                element.Hitbox.AddToCoords(Hitbox.Left, Hitbox.Top);
            }
        }
        public override void Unfocus()
        {
            foreach (var element in elements.Values)
                element.Unfocus();
            base.Unfocus();
        }
        public void AddElement(UIElement element, string name)
        {
            element.Focus += (object sender, EventArgs e) => { selected = true; };
            element.layerDepth = layerDepth - 0.001f;
            elements.Add(name, element);
        }
    }
}

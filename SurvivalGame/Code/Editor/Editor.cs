using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Editor
    {
        List<UIElement> UIElements = new List<UIElement>();
        public bool IsActive = false;
        public Editor()
        {
            UIElements.Add(new EditorButton());
            UIElements.Add(new RoomInProgress());
        }
        public void Update(GameTime gameTime)
        {
            foreach(var element in UIElements)
            {
                element.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(var el in UIElements)
            {
                el.Draw(spriteBatch);
            }
        }
    }
}

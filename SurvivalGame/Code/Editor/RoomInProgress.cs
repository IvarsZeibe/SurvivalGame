using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class RoomInProgress : UIElement
    {
        public RoomInProgress()
        {
            var windowsWidth = Globals.graphics.PreferredBackBufferWidth;
            var windowsHeight = Globals.graphics.PreferredBackBufferHeight;
            Hitbox = new Rect(windowsWidth / 2, windowsHeight / 2, windowsWidth - 320, windowsHeight - 200);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}

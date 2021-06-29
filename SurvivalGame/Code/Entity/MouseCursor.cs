using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class MouseCursor : Entity
    {
        public CursorSlot CursorSlot { get; set; }
        MouseCursor() { }
        public MouseCursor(bool manual = true) : base(false)
        {
            Hitbox = new Rect(0, 0, 3, 3);
            Drawings.Add("base", new Drawing(TextureName.Rectangle, new Vector2(0, 0), Color.White, 0f, new Vector2(3, 3), 0.01f, true));
            CursorSlot = new CursorSlot(this);

        }
        public override void Update(GameTime gameTime)
        {
            MouseState mstate = Mouse.GetState();
            Hitbox.X = mstate.X;
            Hitbox.Y = mstate.Y;
            Drawing.Position = new Vector2((float)Hitbox.Left, (float)Hitbox.Top);
            CursorSlot.Update(this);
        }
        public override void Load()
        {
            IsLoaded = true;
            foreach (var Drawing in Drawings)
            {
                Drawing.Value.Enable();
            }
            CursorSlot.Load();
        }
        public override void UnLoad()
        {
            IsLoaded = false;
            foreach (var Drawing in Drawings)
            {
                Drawing.Value.Disable();
            }
            CursorSlot.Unload();
        }

    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class CursorSlot
    {
        private int padding = 3;
        private readonly int width = 50;
        private readonly int height = 50;
        private const int slotCountHorizontal = 1;
        CursorSlot() { }
        public CursorSlot(MouseCursor Owner, bool isDrawn = false)
        {
            //Owner = owner;
            Drawing = new Drawing
                (
                    TextureName.Rectangle,
                    new Vector2((float)Owner.Hitbox.X, (float)Owner.Hitbox.Y),
                    new Color(0, 0, 0, 100),
                    0f,
                    new Vector2(width, height),
                    0.35f,
                    isDrawn
                );
            for (int i = 0; i < slotCountHorizontal; i++)
            {
                ItemDrawings.Add(null);
            }
            //SelectedItemFrame = new Drawing(TextureName.Rectangle, Vector2.Zero, Color.Red, 0, new Vector2(Drawing.Scale.X / 10, Drawing.Scale.Y), 0.2f);
        }
        //private MouseCursor Owner { get; }
        public Drawing Drawing { get; set; }
        public List<Drawing> ItemDrawings { get; set; } = new List<Drawing>();
        public Inventory Inventory { get; set; } = new Inventory(slotCountHorizontal);
        public IItem Add(IItem item, int index = 0)
        {
            IItem oldItem = Get(index);
            if (Inventory.Add(item, index))
            {
                Globals.Drawings.Remove(ItemDrawings[0]);
                float height = Drawing.Scale.Y - 2 * padding;
                float width = Drawing.Scale.X / slotCountHorizontal - 2 * padding;
                ItemDrawings[index] = new Drawing(item.TextureName, Drawing.Position + new Vector2((width + padding * 2) * index + padding, padding), item.Color, 0f, new Vector2(width, height), 0.3f);
            }
            return oldItem;
        }
        public bool Remove(IItem item)
        {
            return Inventory.Remove(item);
        }
        public IItem Get(int index)
        {
            return Inventory.Get(index);
        }
        public void Update(MouseCursor Owner)
        {
            Vector2 newPosition = new Vector2((float)Owner.Hitbox.X, (float)Owner.Hitbox.Y);
            foreach(var i in ItemDrawings)
            {
                if(i != null)
                    i.Position = i.Position - Drawing.Position + newPosition;
            }
            Drawing.Position = newPosition;
        }
        public void Unload()
        {
            foreach(var drawing in ItemDrawings)
            {
                if(drawing != null)
                    drawing.Disable();
            }
        }
        public void Load()
        {
            foreach (var drawing in ItemDrawings)
            {
                if (drawing != null)
                    drawing.Enable();
            }
        }
    }
}

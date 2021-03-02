using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Hotbar
    {
        private int padding = 3;
        public Hotbar(Entity owner, bool isDrawn = true)
        {
            Owner = owner;
            Drawing = new Drawing
                (
                    TextureName.Rectangle,
                    new Vector2(400, Globals.graphics.PreferredBackBufferHeight - 100),
                    new Color(0, 0, 0, 100),
                    0f,
                    new Vector2(300,30),
                    0.35f,
                    isDrawn
                );
            for (int i = 0; i < 10; i++)
            {
                ItemDrawings.Add(null);
            }
            //SelectedItemFrame = new Drawing(TextureName.Rectangle, Vector2.Zero, Color.Red, 0, new Vector2(Drawing.Scale.X / 10, Drawing.Scale.Y), 0.2f);
        }
        private Entity Owner { get; }
        public Drawing Drawing { get; set; }
        public List<Drawing> ItemDrawings { get; } = new List<Drawing>();
        public List<Drawing> SelectedItemBorder { get; set; } = new List<Drawing>();
        private int selected;
        public int Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                foreach (var drawing in SelectedItemBorder)
                    Globals.Drawings.Remove(drawing);
                float height = Drawing.Scale.Y;
                float width = Drawing.Scale.X / 10;
                SelectedItemBorder = Utilities.CreateEmptyRectDrawings(Drawing.Position + new Vector2(width * selected, 0), Color.Black, new Vector2(width, height));
            }
        }
        public Inventory Inventory { get; set; } = new Inventory(10);
        public bool Add(IItem item, int index)
        {
            //return Inventory.Add(item);
            if (Inventory.Add(item, index))
            {
                float height = Drawing.Scale.Y - 2*padding;
                float width = Drawing.Scale.X / 10 - 2*padding;
                ItemDrawings[index] = new Drawing(item.TextureName, Drawing.Position + new Vector2((width + padding * 2) * index + padding, padding), item.Color, 0f, new Vector2(width, height), 0.3f);
                return true;
            }
            return false;
        }
        public bool Remove(IItem item)
        {
            return Inventory.Remove(item);
        }
        public IItem Get(int index)
        {
            return Inventory.Get(index);
        }
    }
}

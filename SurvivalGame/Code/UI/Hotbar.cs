using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Hotbar
    {
        private int padding = 3;
        private readonly int width = 500;
        private readonly int height = 50;
        public const int slotCountHorizontal = 10;
        public Hotbar(/*Entity owner,*/ bool isDrawn = true)
        {
            //Owner = owner;
            Drawing = new Drawing
                (
                    TextureName.Rectangle,
                    new Vector2(Globals.graphics.PreferredBackBufferWidth / 2 - width / 2, Globals.graphics.PreferredBackBufferHeight - 100),
                    new Color(0, 0, 0, 100),
                    0f,
                    new Vector2(width, height),
                    0.11f,
                    isDrawn
                );
            for (int i = 0; i < 10; i++)
            {
                ItemDrawings.Add(null);
                Inventory.Add(new EmptyItem(), i);
                int height = (int)Drawing.Scale.Y;
                int width = (int)Drawing.Scale.X / slotCountHorizontal;
                Vector2 position = Drawing.Position + new Vector2(width * i, 0);
                Inventory.Get(i).Hitbox = new Rect(position.X, position.Y, width, height);
            }
            //SelectedItemFrame = new Drawing(TextureName.Rectangle, Vector2.Zero, Color.Red, 0, new Vector2(Drawing.Scale.X / 10, Drawing.Scale.Y), 0.2f);
        }
        public Hitbox Hitbox { get; set; }
        //private Entity Owner { get; }
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
                float width = Drawing.Scale.X / slotCountHorizontal;
                SelectedItemBorder = Utilities.CreateEmptyRectDrawings(Drawing.Position + new Vector2(width * selected, 0), Color.Black, new Vector2(width, height), isActive, depth: 0.1f);
            }
        }
        public Inventory Inventory { get; set; } = new Inventory(slotCountHorizontal);

        public bool Add(IItem item, int index = -1)
        {
            if (index == -1)
            {
                for (int i = 0; i < Inventory.SlotMax; i++)
                {
                    if (Inventory.Get(i) is EmptyItem)
                    {
                        index = i;
                        break;
                    }
                }
                if (index == -1)
                    return false;
            }
            if (Inventory.Add(item, index))
            {
                if (ItemDrawings[index] != null)
                    Globals.Drawings.Remove(ItemDrawings[index]);
                float height = Drawing.Scale.Y - 2 * padding;
                float width = Drawing.Scale.X / slotCountHorizontal - 2 * padding;
                Vector2 position = Drawing.Position + new Vector2((width + padding * 2) * index + padding, padding);
                ItemDrawings[index] = new Drawing(item.TextureName, position, item.Color, 0f, new Vector2(width, height), 0.09f, isActive);
                item.Hitbox = new Rect(Drawing.Position.X + Drawing.Scale.X / 10 * index + Drawing.Scale.X / 20, Drawing.Position.Y + Drawing.Scale.Y / 2, (int)Drawing.Scale.X / 10, (int)Drawing.Scale.Y);
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
        private bool isActive = true;
        private bool IsActive
        {
            get => isActive;
        }
        public void Activate()
        {
            isActive = true;
            Drawing.Enable();
            foreach(var drawing in ItemDrawings)
            {
                if (!(drawing is null))
                    drawing.Enable();
            }
            foreach(var border in SelectedItemBorder)
            {
                border.Enable();
            }
        }
        public void Deactivate()
        {
            isActive = false;
            Drawing.Disable();
            foreach (var drawing in ItemDrawings)
            {
                if(!(drawing is null))
                    drawing.Disable();
            }
            foreach (var border in SelectedItemBorder)
            {
                border.Disable();
            }
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Shop
    {
        private bool isActive = false;
        public bool IsActive { get => isActive; }
        public Inventory itemsForSale = new Inventory(6);
        int rowLenght = 2;

        Drawing background;
        Hitbox backgroundHitbox;
        Color backgroundColor = new Color(0, 0, 0, 100);

        Drawing closeButton;
        Hitbox closeButtonHitbox;
        Color closeButtonColor = new Color(255, 0, 0, 100);

        DrawingText title;

        //List<Drawing> itemSlotDrawings = new List<Drawing>();
        //List<Drawing> itemDrawings = new List<Drawing>();
        //List<Hitbox> itemSlotHitboxes = new List<Hitbox>();
        //List<int> prices = new List<int>();
        //List<Drawing> priceCoinDrawings = new List<Drawing>();
        //List<DrawingText> priceTextDrawings = new List<DrawingText>();
        List<ShopSlot> slots = new List<ShopSlot>();

        public Shop()
        {
            backgroundHitbox = new Rect(100, 100, 200, 600, true);
            background = new Drawing(
                TextureName.Rectangle,
                new Vector2((float)backgroundHitbox.Left, (float)backgroundHitbox.Top),
                backgroundColor,
                0f,
                new Vector2((float)backgroundHitbox.Width, (float)backgroundHitbox.Height),
                0.1f,
                false);

            closeButtonHitbox = new Rect(backgroundHitbox.Left + 5, backgroundHitbox.Top + 5, 10, 10, true);
            closeButton = new Drawing(
                TextureName.Rectangle,
                closeButtonHitbox.GetTopLeftPosVector(),
                closeButtonColor,
                0f,
                closeButtonHitbox.GetScaleVector(),
                0.09f,
                false);

            title = new DrawingText(
                SpriteFontName.Aerial16,
                new StringBuilder("Shop"),
                new Vector2((float)backgroundHitbox.Left + 75, (float)backgroundHitbox.Top + 30),
                Color.White,
                0f,
                new Vector2(1,1),
                0.09f,
                false);
            CreateItemSlots();
        }
        public void Open()
        {
            isActive = true;
            background.IsDrawn = true;
            closeButton.IsDrawn = true;
            title.IsDrawn = true;
            foreach(var slot in slots)
            {
                slot.itemSlotDrawing.IsDrawn = true;
                slot.itemDrawing.IsDrawn = true;
                slot.priceTextDrawing.IsDrawn = true;
                slot.priceCoinDrawing.IsDrawn = true;
            }
        }
        public void Close()
        {
            isActive = false;
            background.IsDrawn = false;
            closeButton.IsDrawn = false;
            title.IsDrawn = false;
            foreach (var slot in slots)
            {
                slot.itemSlotDrawing.IsDrawn = false;
                slot.itemDrawing.IsDrawn = false;
                slot.priceTextDrawing.IsDrawn = false;
                slot.priceCoinDrawing.IsDrawn = false;
            }
        }
        public void CheckLeftClickEvent() 
        {
            if (isActive)
            {
                if (Globals.MouseCursor.Hitbox.CollidesWith(closeButtonHitbox))
                {
                    Close();
                }
                for (int i = 0; i < slots.Count; i++)
                {
                    var slot = slots[i];
                    if (Globals.MouseCursor.Hitbox.CollidesWith(slot.itemSlotHitbox))
                    {
                        if (Globals.HUD.points >= slot.price)
                        {
                            Globals.HUD.points -= slot.price;
                            Globals.HUD.hotbar.Add(itemsForSale.Get(i));
                            RemoveItemAt(i);
                        }
                    }
                }
            }
        }
        public void CheckRightClickEvent()
        {
            //for(int i = 0; i < itemSlotHitboxes.Count; i++)
            //{
            //    if (Globals.MouseCursor.Hitbox.CollidesWith(itemSlotHitboxes[i]))
            //    {
            //        RemoveItemAt(i);
            //    }
            //}
        }
        void CreateItemSlots()
        {
            int width = 50;
            int offsetFromLeft = 50;
            int height = 50;
            int offsetFromTop = 200;
            int spacing = 5;
            for (int y = 0; y < itemsForSale.SlotMax / rowLenght; y++)
            {
                for(int x = 0; x < rowLenght; x++)
                {
                    var hitbox = new Rect(backgroundHitbox.Left + offsetFromLeft + x*(width + spacing),
                        backgroundHitbox.Top + offsetFromTop + y*(height + spacing), width, height, true);
                    slots.Add(new ShopSlot(hitbox));
                }
            }
        }

        public void AddItemForSale(IItem item, int price)
        {
            itemsForSale.Add(item);
            int index = itemsForSale.GetIndex(item);
            var slot = slots[index];
            slot.ChangeItem(item.TextureName, price);
            //Vector2 scale = slot.itemDrawing.Scale;
            //slot.itemDrawing.Texture = item.TextureName;
            //slot.itemDrawing.Color = Color.White;
            //slot.itemDrawing.Scale = scale;
            //slot.price = price;
            //slot.priceTextDrawing.Text = new StringBuilder(price.ToString());
            //slot.priceTextDrawing.Position = slot.priceTextDrawing.Position + new Vector2(slot.priceTextDrawing.GetWidth(), 5);
            //slot.priceTextDrawing.Color = Color.Yellow;
        }
        void RemoveItem(IItem item)
        {
            int index = itemsForSale.GetIndex(item);
            RemoveItemAt(index);
        }
        void RemoveItemAt(int index)
        {
            itemsForSale.RemoveAt(index);
            slots[index].Clear();
            //slots[index].itemDrawing.Texture = TextureName.Rectangle;
            //slots[index].itemDrawing.Color = Color.Transparent;
            //slots[index].priceTextDrawing.Text = new StringBuilder("");
            //slots[index].priceCoinDrawing.Color = Color.Transparent;
        }
    }
}

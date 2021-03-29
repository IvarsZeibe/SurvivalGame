using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class ShopSlot
    {
        public Drawing itemSlotDrawing;
        public Drawing itemDrawing;
        public Hitbox itemSlotHitbox;
        public int price = 0;
        public Drawing priceCoinDrawing;
        public DrawingText priceTextDrawing;
        public ShopSlot(Hitbox hitbox)
        {

            itemSlotHitbox = hitbox;
            itemSlotDrawing = new Drawing(
                TextureName.Rectangle,
                hitbox.GetTopLeftPosVector(),
                Color.Gray,
                0f,
                hitbox.GetScaleVector(),
                0.09f,
                false);
            itemDrawing = new Drawing(
                TextureName.Rectangle,
                hitbox.GetTopLeftPosVector(),
                Color.Transparent,
                0f,
                hitbox.GetScaleVector(),
                0.08f,
                false);
            var text = new DrawingText(
                SpriteFontName.Aerial16,
                new StringBuilder(""),
                new Vector2((float)hitbox.Right, (float)hitbox.Bottom - 20),
                Color.White,
                0f,
                new Vector2(1, 1),
                0.08f);
            priceTextDrawing = text;
            priceCoinDrawing = new Drawing(
                TextureName.Circle,
                new Vector2(0, 0),
                Color.Transparent,
                0f,
                new Vector2(10, 10),
                0.08f,
                false);
        }
        public void ChangeItem(TextureName texture, int price)
        {
            itemDrawing.Texture = texture;
            itemDrawing.Color = Color.White;
            itemDrawing.Scale = itemSlotHitbox.GetScaleVector();
            this.price = price;

            float oldTextWidth = priceTextDrawing.GetWidth();
            priceTextDrawing.Text = new StringBuilder(price.ToString());
            priceTextDrawing.Position = priceTextDrawing.Position + new Vector2(oldTextWidth - priceTextDrawing.GetWidth(), 0);
            priceCoinDrawing.Color = Color.Yellow;
            priceCoinDrawing.Position = new Vector2(priceTextDrawing.Position.X - priceCoinDrawing.GetWidth(), (float)itemSlotHitbox.Bottom - priceCoinDrawing.GetHeight());
        }
        public void Clear()
        {
            itemDrawing.Color = Color.Transparent;
            priceTextDrawing.Text = new StringBuilder("");
            priceCoinDrawing.Color = Color.Transparent;
            this.price = 0;
        }
    }
}

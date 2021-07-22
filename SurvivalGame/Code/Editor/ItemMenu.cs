using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class ItemMenu : UIElement
    {
        EditorBox Title;
        List<Item> items = new List<Item>();
        int activeItemIndex = -1;
        public ItemMenu()
        {
            int width = 140;
            int height = 300;
            int x = Globals.graphics.PreferredBackBufferWidth - width / 2 - 10;
            int y = height / 2 + 100;
            Hitbox = new Rect(x, y, width, height);

            Title = new EditorBox(x, (int)Hitbox.Top + 10 + 20, 100, 20, "Items");

            AddItem(typeof(Stone));
            AddItem(typeof(Tree));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Title.Draw(spriteBatch);
            foreach (var item in items)
                item.box.Draw(spriteBatch);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (var item in items)
                item.box.Update(gameTime);

        }
        public override void LoseFocus()
        {
            if (selected)
                OnLostFocus();
            selected = false;

            foreach (var item in items)
            {
                item.box.LoseFocus();
            }
        }
        void AddItem(Type t)
        {
            EditorButton button = new EditorButton();
            button.layerDepth = layerDepth - 0.01f;
            var item = new Item(t, button, true);
            if (items.Count == 0)
                button.Hitbox = new Rect((int)Hitbox.Left + 5, (int)Title.Hitbox.Bottom + 5, 20, 20, true);
            else
                button.Hitbox = new Rect((int)Hitbox.Left + 5, (int)items[items.Count - 1].box.Hitbox.Bottom + 5, 20, 20, true);
            button.borderColor = Color.Black;
            //button.Selectable = true;
            button.clickAction = () =>
            {
                SetActiveItem(item);
                (Globals.Editor.UIElements["editedRoom"] as EditedRoom).SetActiveItem(null);
                //button.borderWidth = 1;
            };
            //button.lostFocusAction = () => { button.borderWidth = 0; };

            Entity entity = Activator.CreateInstance(item.type, true) as Entity;
            entity.SetDafaultValues();
            item.entity = entity;
            if (entity.Drawing.TextureStr != "none")
                button.textureName = entity.Drawing.TextureStr;
            else
                button.textureName = entity.Drawing.Texture.ToString();

            items.Add(item);
        }
        public Item GetActiveItem()
        {
            if (items.Count < activeItemIndex || activeItemIndex == -1)
                return null;
            return items[activeItemIndex];
        }
        public bool SetActiveItem(Item item)
        {
            if(item is null)
            {
                if (activeItemIndex != -1)
                    items[activeItemIndex].box.borderWidth = 0;
                activeItemIndex = -1;
                return true;
            }
            if (!items.Contains(item))
                return false;
            if(activeItemIndex != -1)
                items[activeItemIndex].box.borderWidth = 0;
            activeItemIndex = items.IndexOf(item);
            items[activeItemIndex].box.borderWidth = 2;
            return true;
        }
    }
    class Item
    {
        public Type type;
        public EditorButton box;
        public Entity entity;
        public bool isSpawner;
        public bool clicked = false;
        public Item(Type t, EditorButton b, bool spawner)
        {
            this.isSpawner = spawner;
            type = t;
            box = b;
        }
    }
}

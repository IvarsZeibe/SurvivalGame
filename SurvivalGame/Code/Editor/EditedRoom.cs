using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class EditedRoom : UIElement
    {
        float scale = 0.75f;
        public Room room;
        List<Item> entitiesAsItems = new List<Item>();
        int activeItemIndex = -1;

        public EditedRoom()
        {
            var windowsWidth = Globals.graphics.PreferredBackBufferWidth;
            var windowsHeight = Globals.graphics.PreferredBackBufferHeight;
            //var windowsWidth = 1280;
            //var windowsHeight = 800;
            Hitbox = new Rect(windowsWidth / 2, windowsHeight / 2, (int)(windowsWidth * scale), (int)(windowsHeight * scale));
            borderWidth = 2;
            ResetRoom();
            CreateClickAction();
        }
        public Item GetActiveItem()
        {
            if (entitiesAsItems.Count < activeItemIndex || activeItemIndex == -1)
                return null;
            return entitiesAsItems[activeItemIndex];
        }
        public bool SetActiveItem(Item item)
        {
            if (item is null)
            {
                if (activeItemIndex != -1)
                    entitiesAsItems[activeItemIndex].box.borderWidth = 0;
                activeItemIndex = -1;
                return true;
            }
            if (!entitiesAsItems.Contains(item))
                return false;
            if (activeItemIndex != -1)
                entitiesAsItems[activeItemIndex].box.borderWidth = 0;
            activeItemIndex = entitiesAsItems.IndexOf(item);
            entitiesAsItems[activeItemIndex].box.borderWidth = 2;
            return true;
        }
        void CreateClickAction()
        {
            clickAction = () =>
            {
                var itemMenu = Globals.Editor.UIElements["itemMenu"] as ItemMenu;
                var item = itemMenu.GetActiveItem();
                if (item != null && item.isSpawner)
                {
                    var entity = SaveManager.Clone(item.entity);
                    entity.Hitbox.X = (Globals.MouseCursor.X - Hitbox.Left) / scale;
                    entity.Hitbox.Y = (Globals.MouseCursor.Y - Hitbox.Top) / scale;
                    AddItemNoCloning(entity);
                    room.Entities.Add(entity);
                }
            };
        }
        void AddItemNoCloning(Entity entity)
        {
            var itemMenu = Globals.Editor.UIElements["itemMenu"] as ItemMenu;
            EditorButton button = new EditorButton();
            var item = new Item(button, false);
            item.entity = entity;
            //button.defaultColor = Color.White;
            //button.hoverColor = Color.White;
            //button.holdingColor = Color.White;
            //button.releaseColor = Color.White;
            button.Hitbox = new Rect((item.entity.Hitbox.X) * scale + Hitbox.Left, (item.entity.Hitbox.Y) * scale + Hitbox.Top,
                (int)(item.entity.Drawing.GetWidth() * scale), (int)(item.entity.Drawing.GetHeight() * scale));
            button.borderColor = Color.Black;
            button.clickAction += () =>
            {
                itemMenu.SetActiveItem(null);
                SetActiveItem(item);
            };
            button.layerDepth = layerDepth - 0.005f - (float)(button.Hitbox.Y / 1000000);

            if (item.entity.Drawing.TextureStr != "none")
                button.textureName = item.entity.Drawing.TextureStr;
            else
                button.textureName = item.entity.Drawing.Texture.ToString();
            button.defaultColor = item.entity.Drawing.Color;

            entitiesAsItems.Add(item);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            DrawDrawing(spriteBatch, room.background, layerDepth - 0.001f);
            foreach(var item in entitiesAsItems)
            {
                item.box.Draw(spriteBatch);
            }

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (var item in entitiesAsItems)
            {
                item.box.Update(gameTime);
                item.box.Hitbox = new Rect((item.entity.Hitbox.X) * scale + Hitbox.Left, (item.entity.Hitbox.Y) * scale + Hitbox.Top,
                    (int)(item.entity.Drawing.GetWidth() * scale), (int)(item.entity.Drawing.GetHeight() * scale));
            }
        }
        void DrawDrawing(SpriteBatch spriteBatch, Drawing drawing, float layerDepth = -1) 
        {
            Vector2 pos = (drawing.Position * scale) + Hitbox.GetTopLeftPosVector();
            Vector2 drawingScale = drawing.Scale * scale;
            if (layerDepth == -1)
                layerDepth = drawing.LayerDepth;
            if (drawing.TextureStr == "none")
                spriteBatch.Draw(Globals.Textures[drawing.Texture.ToString()], pos, null, drawing.Color, drawing.Rotation, drawing.Origin, drawingScale, SpriteEffects.None, layerDepth);
            else
            {
                var i = Globals.Textures[drawing.TextureStr];
                spriteBatch.Draw(Globals.Textures[drawing.TextureStr], pos, null, drawing.Color, drawing.Rotation, drawing.Origin, drawingScale, SpriteEffects.None, layerDepth);
            }
        }
        public void FinishRoom()
        {
            if (Globals.Rooms.ContainsKey(room.Coords))
                Globals.Rooms[room.Coords] = room;
            else
                Globals.Rooms.Add(room.Coords, room);
            ResetRoom();
            Globals.HUD.Activate();
            Globals.Editor.IsActive = false;
            Globals.getActiveRoom.Load();
        }
        public void Load()
        {
            var roomCoordInput = Globals.Editor.UIElements["RoomCoordInput"] as EditorTextInput;
            try
            {
                var cords = roomCoordInput.text.ToString().Split(" ");
                var coords = (Convert.ToInt32(cords[0]), Convert.ToInt32(cords[1]));
                room = SaveManager.Clone(Globals.Rooms[coords]);
                foreach(var entity in room.Entities)
                {
                    AddItemNoCloning(entity);
                }
            }
            catch { roomCoordInput.error = true; }
        }
        public void ResetRoom()
        {
            room = new Room();
            room.background = new Drawing(TextureName.Rectangle, Vector2.Zero, Color.Pink, 0f,
                new Vector2(Globals.graphics.PreferredBackBufferWidth, Globals.graphics.PreferredBackBufferHeight), 1f, false);

            entitiesAsItems.Clear();
            activeItemIndex = -1;
        }
    }
}
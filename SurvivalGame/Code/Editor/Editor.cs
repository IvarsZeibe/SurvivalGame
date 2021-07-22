using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SurvivalGame
{
    class Editor
    {
        public Color BackgroundColor = Color.LightGray;
        public Dictionary<string, UIElement> UIElements = new Dictionary<string, UIElement>();
        public bool IsActive = false;
        public Editor()
        {
            UIElements.Add("editedRoom", new EditedRoom());
            UIElements.Add("Title", new EditorBox(Globals.graphics.PreferredBackBufferWidth / 2, 30, 90, 50, "Editor"));
            UIElements.Add("RoomNameInput", new EditorTextInput(50, 100, 90, 50) { placeholder = "Name" });
            UIElements.Add("RoomCoordInput", new EditorTextInput(50, 160, 90, 50) { placeholder = "Coords" });
            UIElements.Add("finishButton", new EditorButton()
            {
                text = "Save and Play",
                Hitbox = new Rect(Globals.graphics.PreferredBackBufferWidth - 210, Globals.graphics.PreferredBackBufferHeight - 50, 400, 50),
                clickAction = () =>
                {
                    var editedRoom = (UIElements["editedRoom"] as EditedRoom);
                    if (Save())
                    {
                        editedRoom.FinisheRoom();
                    }
                }
            });
            UIElements.Add("loadButton", new EditorButton()
            {
                text = "Load Room",
                Hitbox = new Rect(Globals.graphics.PreferredBackBufferWidth - 620, Globals.graphics.PreferredBackBufferHeight - 50, 400, 50),
                clickAction = () =>
                {
                    (UIElements["editedRoom"] as EditedRoom).Load();
                    
                }
            });
            UIElements.Add("itemMenu", new ItemMenu());
            UIElements.Add("itemPropertiesWindow", new ItemPropertiesWindow());
        }
        public void Update(GameTime gameTime)
        {
            foreach (var element in UIElements.Values)
            {
                element.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var el in UIElements.Values)
            {
                el.Draw(spriteBatch);
            }
        }
        bool Save()
        {
            bool flag = true;
            var room = (UIElements["editedRoom"] as EditedRoom).room;

            var roomNameInput = UIElements["RoomNameInput"] as EditorTextInput;
            if (roomNameInput.text.Length > 0)
            {
                room.Name = roomNameInput.text.ToString();
                roomNameInput.error = false;
            }
            else
            {
                roomNameInput.error = true;
                flag = false;
            }

            var roomCoordInput = UIElements["RoomCoordInput"] as EditorTextInput;
            try
            {
                var coords = roomCoordInput.text.ToString().Split(" ");
                room.Coords = (Convert.ToInt32(coords[0]), Convert.ToInt32(coords[1]));
                roomCoordInput.error = false;
            }
            catch 
            {
                roomCoordInput.error = true;
                flag = false;
            }
            return flag;
        }
    }
}
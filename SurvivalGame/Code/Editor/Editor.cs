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
        public (Action<EventArgs> action, EventArgs args, float layerDepth) ClickedElementAction;
        public (Action<EventArgs> action, EventArgs args, float layerDepth) HoveredElementAction;
        public (Action<EventArgs> action, EventArgs args) ReleasedElementAction;
        public (Action<EventArgs> action, EventArgs args) HeldElementAction;
        public Editor()
        {
            UIElements.Add("editedRoom", new EditedRoom());
            UIElements.Add("Title", new EditorBox(Globals.graphics.PreferredBackBufferWidth / 2, 30, 90, 50, "Editor"));
            UIElements.Add("RoomNameInput", new EditorTextInput(50, 100, 90, 50) { placeholder = "Name" });
            UIElements.Add("RoomCoordInput", new EditorTextInput(50, 160, 90, 50) { placeholder = "Coords" });
            var resetButton = new EditorButton();
            resetButton.Click += (object sender, EventArgs e) => { (UIElements["editedRoom"] as EditedRoom).ResetRoom(); };
            resetButton.text = "Reset";
            resetButton.Hitbox = new Rect(50, 220, 90, 50);
            UIElements.Add("ResetButton", resetButton);

            var finishButton = new EditorButton();
            finishButton.text = "Save and Play";
            finishButton.Hitbox = new Rect(Globals.graphics.PreferredBackBufferWidth - 210, Globals.graphics.PreferredBackBufferHeight - 50, 400, 50);
            finishButton.Click += (object sender, EventArgs e) =>
            {
                var editedRoom = (UIElements["editedRoom"] as EditedRoom);
                if (Save())
                {
                    editedRoom.FinishRoom();
                }
            };
            UIElements.Add("finishButton", finishButton);

            var loadButton = new EditorButton();
            loadButton.text = "Load Room";
            loadButton.Hitbox = new Rect(Globals.graphics.PreferredBackBufferWidth - 620, Globals.graphics.PreferredBackBufferHeight - 50, 400, 50);
            loadButton.Click += (object sender, EventArgs e) =>
            {
                (UIElements["editedRoom"] as EditedRoom).Load();
            };
            UIElements.Add("loadButton", loadButton);

            UIElements.Add("itemMenu", new ItemMenu());
            UIElements.Add("itemPropertiesWindow", new ItemPropertiesWindow());

            //var test = new EditorWindow(10, 10, 200, 200, true);
            //test.layerDepth = 0.1f;
            //test.AddElement(new EditorButton() { Hitbox = new Rect(0, 20, 50, 50, true), defaultColor = Color.Orange }, "a");
            //test.AddElement(new EditorWindow(80, 20, 50, 50, true) { defaultColor = Color.Yellow }, "b");
            //UIElements.Add("t", test);
        }
        public void Update(GameTime gameTime)
        {
            ClickedElementAction = (null, null, 1);
            HoveredElementAction = (null, null, 1);
            HeldElementAction = (null, null);
            ReleasedElementAction = (null, null);
            foreach (var element in UIElements.Values)
            {
                element.Update(gameTime);
            }
            HoveredElementAction.action?.Invoke(HoveredElementAction.args);
            HeldElementAction.action?.Invoke(HeldElementAction.args);
            ReleasedElementAction.action?.Invoke(ReleasedElementAction.args);
            ClickedElementAction.action?.Invoke(ClickedElementAction.args);
            //if (HoveredElementAction.action != null)
            //{
            //    if (!(HoveredElementAction.action.Target as UIElement).holding.Item1)
            //        HoveredElementAction.action(HoveredElementAction.args);
            //}
        }
        public void PrepareDraw(SpriteBatch spriteBatch)
        {
            foreach (var el in UIElements.Values)
            {
                el.PrepareDraw(spriteBatch);
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
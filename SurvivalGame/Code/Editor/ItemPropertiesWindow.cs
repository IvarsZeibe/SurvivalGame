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
    class ItemPropertiesWindow : UIElement
    {
        EditorBox Title;
        //EditorButton SaveButton;
        List<(EditorBox, EditorTextInput)> properties = new List<(EditorBox, EditorTextInput)>();
        Item item = null;
        RenderTarget2D propertiesDrawing;
        double ScrollY = 0;
        public ItemPropertiesWindow()
        {
            int width = 140;
            int height = 300 - 35;
            int x = Globals.graphics.PreferredBackBufferWidth - width / 2 - 10;
            int y = height / 2 + 405;
            Hitbox = new Rect(x, y, width, height);
            Title = new EditorBox(x, (int)Hitbox.Top + 30, 100, 20, "Properties");
            Title.AdditionalClickAndHoverCheck = new Func<bool>(() => Globals.MouseCursor.Hitbox.CollidesWith(Hitbox));
            //Title = new EditorBox(0, 20, width, 20, "Properties", true);

            propertiesDrawing = new RenderTarget2D(Globals.graphics.GraphicsDevice, width, height);

            //SaveButton = new EditorButton();
            //SaveButton.Hitbox.Height = 30;
            //SaveButton.Hitbox.Width = Hitbox.Width;
            //SaveButton.Hitbox.Top = Hitbox.Bottom - 1;
            //SaveButton.Hitbox.Left = Hitbox.Left;
            //SaveButton.text = "Save";
            //SaveButton.clickAction = () =>
            //{
            //    Entity entity = Activator.CreateInstance(ac);
            //    Room room = (Globals.Editor.UIElements["editedRoom"] as EditedRoom).room;
            //    room.Entities.Add(entity);
            //};
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(propertiesDrawing, Hitbox.GetTopLeftPosVector(), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, layerDepth);
            //SaveButton.Draw(spriteBatch);
        }
        void DrawRenderTarget()
        {
            var spriteBatch = Globals.spriteBatch;
            Globals.graphics.GraphicsDevice.SetRenderTarget(propertiesDrawing);
            Globals.graphics.GraphicsDevice.Clear(new Color(240, 240, 240));
            spriteBatch.Begin();
            //base.Draw(spriteBatch);
            Title.Hitbox.AddToCoords(-Hitbox.Left, -Hitbox.Top);
            Title.Draw(spriteBatch);
            Title.Hitbox.AddToCoords(Hitbox.Left, Hitbox.Top);
            foreach (var property in properties)
            {
                property.Item1.Hitbox.X -= Hitbox.Left;
                property.Item1.Hitbox.Y -= Hitbox.Top;
                property.Item1.Draw(spriteBatch);
                property.Item1.Hitbox.X += Hitbox.Left;
                property.Item1.Hitbox.Y += Hitbox.Top;
                property.Item2.Hitbox.X -= Hitbox.Left;
                property.Item2.Hitbox.Y -= Hitbox.Top;
                property.Item2.Draw(spriteBatch);
                property.Item2.Hitbox.X += Hitbox.Left;
                property.Item2.Hitbox.Y += Hitbox.Top;
            }
            spriteBatch.End();
            Globals.graphics.GraphicsDevice.SetRenderTarget(null);
            ///
            /// Hitbox in wrong place
            ///
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var activeItem = (Globals.Editor.UIElements["itemMenu"] as ItemMenu).GetActiveItem();
            if (activeItem is null)
                activeItem = (Globals.Editor.UIElements["editedRoom"] as EditedRoom).GetActiveItem();
            if (item != activeItem)
            {
                item = activeItem;
                if(item != null)
                    RefreshProperties();
            }
            foreach(var property in properties)
            {
                property.Item1.Update(gameTime);
                property.Item2.Update(gameTime);
            }
            //SaveButton.Update(gameTime);
            CheckForScroll();
            DrawRenderTarget();
        }
        bool CheckForScroll()
        {
            var mstate = Mouse.GetState();
            if (Globals.MouseCursor.Hitbox.CollidesWith(Hitbox) && Globals.ScrollWheel != 0)
            {
                OnScroll();
                return true;
            }
            return false;
        }
        void OnScroll()
        {
            var value = Globals.ScrollWheel / 3;
            if (Title.Hitbox.Top - 20 + value > Hitbox.Top)
                value = (int)(Hitbox.Top + 20 - Title.Hitbox.Top);
            Title.Hitbox.Y += value;
            foreach (var property in properties)
            {
                property.Item1.Hitbox.Y += value;
                property.Item2.Hitbox.Y += value;
            }
            ScrollY += value;
        }
        public override void LoseFocus()
        {
            if (selected)
                OnLostFocus();
            selected = false;

            foreach (var property in properties)
            {
                property.Item1.LoseFocus();
                property.Item2.LoseFocus();
            }
        }
        void RefreshProperties()
        {
            properties.Clear();
            foreach (var name in item.entity.GetPropertiesNames())
            {
                AddProperty(name);
            }
        }
        void AddProperty(string name)
        {
            EditorBox box;
            if (properties.Count == 0)
                //box = new EditorBox(0, (int)Title.Hitbox.Bottom + 5, 60, 16, name, true);
                box = new EditorBox((int)Hitbox.Left, (int)Title.Hitbox.Bottom + 5, 60, 16, name, true);
            else
                //box = new EditorBox(0, (int)properties[properties.Count - 1].Item1.Hitbox.Bottom + 5, 60, 16, name, true);
                box = new EditorBox((int)Hitbox.Left, (int)properties[properties.Count - 1].Item1.Hitbox.Bottom + 6, 60, 16, name, true);

            var input = new EditorTextInput((int)box.Hitbox.Right + 5, (int)box.Hitbox.Top, 60, 20, true);
            input.SetBackgroundColor(Color.White);
            input.clickAction = () => {
                foreach (var property in properties)
                {
                    property.Item2.LoseFocus();
                }
            };
            input.layerDepth = layerDepth - 0.01f;
            box.layerDepth = layerDepth - 0.01f;
            box.AdditionalClickAndHoverCheck = new Func<bool>(() => Globals.MouseCursor.Hitbox.CollidesWith(Hitbox));
            input.AdditionalClickAndHoverCheck = new Func<bool>(() => Globals.MouseCursor.Hitbox.CollidesWith(Hitbox));
            properties.Add((box, input));
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalGame
{
    class Input
    {
        Game1 Game1;
        Player player { get => EntityTracker.GetEntity<Player>(); }
        Chat chat;

        int OldScrollWheel;

        Vector2 oldMousePos;
        Vector2 mouseMovment;
        public Input(Game1 game1, Player player, Chat chat)
        {
            Game1 = game1;
            //this.player = player;
            this.chat = chat;

        }
        public void Update(GameTime gameTime)
        {
            UpdateKeys();
            OnKeyDown(gameTime);
            OnKeyUp(gameTime);
        }
        void UpdateKeys()
        {
            UpdateKeyboardKeys();
            UpdateMouseKeys();
        }
        void UpdateKeyboardKeys()
        {
            List<Keys> oldList = new List<Keys>(Globals.PressedKeyboardKeys);
            Globals.PressedKeyboardKeys = new List<Keys>(Keyboard.GetState().GetPressedKeys());
            Globals.NewKeyboardKeys = Globals.PressedKeyboardKeys.Except(oldList).ToList();
        }
        void UpdateMouseKeys()
        {
            List<MouseKey> oldList = new List<MouseKey>(Globals.PressedMouseKeys);
            Globals.PressedMouseKeys.Clear();

            MouseState mstate = Mouse.GetState();
            if ((int)mstate.LeftButton == 1)
            {
                Globals.PressedMouseKeys.Add(MouseKey.LeftButton);
            }
            if ((int)mstate.RightButton == 1)
            {
                Globals.PressedMouseKeys.Add(MouseKey.RightButton);
            }
            if ((int)mstate.MiddleButton == 1)
            {
                Globals.PressedMouseKeys.Add(MouseKey.MiddleButton);
            }
            if ((int)mstate.XButton1 == 1)
            {
                Globals.PressedMouseKeys.Add(MouseKey.XButton1);
            }
            if ((int)mstate.XButton2 == 1)
            {
                Globals.PressedMouseKeys.Add(MouseKey.XButton2);
            }

            Globals.NewMouseKeys = Globals.PressedMouseKeys.Except(oldList).ToList();

            Globals.ScrollWheel = mstate.ScrollWheelValue - OldScrollWheel;
            OldScrollWheel = mstate.ScrollWheelValue;

            mouseMovment = oldMousePos - new Vector2(mstate.Position.X, mstate.Position.Y);
            oldMousePos = new Vector2(mstate.Position.X, mstate.Position.Y);


        }

        void OnKeyDown(GameTime gameTime)
        {
            if (!Globals.IsUserWriting && Game1.IsActive)
            {
                foreach (var key in Globals.PressedKeyboardKeys)
                {
                    if (!Globals.MainMenu.IsActive && !Globals.Editor.IsActive)
                    {
                        switch (key)
                        {
                            case Keys.D:
                                if (player != null)
                                    player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
                                break;
                            case Keys.A:
                                if (player != null)
                                    player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
                                break;
                            case Keys.S:
                                if (player != null)
                                    player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
                                break;
                            case Keys.W:
                                if (player != null)
                                    player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
                                break;
                            case Keys.Space:
                                if (player != null)
                                {
                                    player.UsePrimary();
                                }
                                break;
                        }
                    }
                }
                foreach (var key in Globals.NewKeyboardKeys)
                {
                    if (!Globals.MainMenu.IsActive && !Globals.Editor.IsActive)
                    {
                        switch (key)
                        {
                            case Keys.D1:
                                if(player != null)
                                    player.Hotbar.Selected = 0;
                                break;
                            case Keys.D2:
                                if (player != null)
                                    player.Hotbar.Selected = 1;
                                break;
                            case Keys.D3:
                                if (player != null)
                                    player.Hotbar.Selected = 2;
                                break;
                            case Keys.D4:
                                if (player != null)
                                    player.Hotbar.Selected = 3;
                                break;
                            case Keys.D5:
                                if (player != null)
                                    player.Hotbar.Selected = 4;
                                break;
                            case Keys.D6:
                                if (player != null)
                                    player.Hotbar.Selected = 5;
                                break;
                            case Keys.D7:
                                if (player != null)
                                    player.Hotbar.Selected = 6;
                                break;
                            case Keys.D8:
                                if (player != null)
                                    player.Hotbar.Selected = 7;
                                break;
                            case Keys.D9:
                                if (player != null)
                                    player.Hotbar.Selected = 8;
                                break;
                            case Keys.D0:
                                if (player != null)
                                    player.Hotbar.Selected = 9;
                                break;
                            case Keys.T:
                                chat.NewLine();
                                break;
                            case Keys.B:
                                if (Globals.shop.IsActive)
                                    Globals.shop.Close();
                                else
                                    Globals.shop.Open();
                                break;
                            case Keys.M:
                                if (Globals.Map.IsActive)
                                    Globals.Map.Close();
                                else
                                    Globals.Map.Open();
                                break;
                        }
                    }
                    switch (key)
                    {
                        case Keys.Escape:
                            if (Globals.MainMenu.IsActive)
                                Globals.MainMenu.Deactivate();
                            else
                                Globals.MainMenu.Activate();
                            break;
                        case Keys.F11:
                            Globals.graphics.ToggleFullScreen();
                            break;
                        case Keys.F1:
                            SaveManager.Save();
                            break;
                        case Keys.F2:
                            SaveManager.Load();
                            break;
                    }
                }
                foreach (var button in Globals.PressedMouseKeys)
                {
                    if (!Globals.MainMenu.IsActive && !Globals.Editor.IsActive)
                    {
                        switch (button)
                        {
                            case MouseKey.RightButton:
                                if (player != null)
                                    player.UseSecondary();
                                break;
                            case MouseKey.LeftButton:
                                if(Globals.Map.BeingDragged)
                                    Globals.Map.Drag(mouseMovment);
                                break;
                        }
                    }
                }
                foreach (var button in Globals.NewMouseKeys)
                {
                    if (!Globals.MainMenu.IsActive && !Globals.Editor.IsActive)
                    {
                        switch (button)
                        {
                            case MouseKey.LeftButton:
                                if (player != null)
                                {
                                    for (int i = 0; i < player.Hotbar.Inventory.SlotMax; i++)
                                    {
                                        if (Globals.MouseCursor.Hitbox.CollidesWith(player.Hotbar.Get(i).Hitbox))
                                        {
                                            player.Hotbar.Add(Globals.MouseCursor.CursorSlot.Add(player.Hotbar.Get(i)), i);
                                            break;
                                        }
                                    }
                                }
                                Globals.shop.CheckLeftClickEvent();
                                if (Globals.Map.Hitbox.CollidesWith(Globals.MouseCursor.Hitbox) && Globals.Map.IsActive)
                                    Globals.Map.BeingDragged = true;
                                break;
                            case MouseKey.RightButton:
                                Globals.shop.CheckRightClickEvent();
                                break;
                            case MouseKey.MiddleButton:
                                var light = new LightBulb(Globals.MouseCursor.Hitbox.GetPosVector(), new Vector2(500, 500), new Color(150, 100, 0));
                                Globals.getActiveRoom.Entities.Add(light);
                                break;
                        }
                    }
                    switch (button)
                    {
                        case MouseKey.LeftButton:
                            Globals.MainMenu.CheckClickEvent();
                            break;
                    }
                }
            }
            if (Globals.ScrollWheel != 0) 
            {
                if (Globals.MouseCursor.Hitbox.CollidesWith(Globals.Map.Hitbox))
                {
                    Globals.Map.Zoom(Globals.ScrollWheel);
                }
            }
        }
        void OnKeyUp(GameTime gameTime)
        {
            if (!Globals.PressedMouseKeys.Contains(MouseKey.LeftButton))
                Globals.Map.BeingDragged = false;
        }
    }
}

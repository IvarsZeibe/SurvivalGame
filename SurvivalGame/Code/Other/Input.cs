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
        Player player;
        Chat chat;
        public Input(Game1 game1, Player player, Chat chat)
        {
            Game1 = game1;
            this.player = player;
            this.chat = chat;

        }
        public void Update(GameTime gameTime)
        {
            UpdateKeys();
            OnKeyDown(gameTime);
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
        }

        void OnKeyDown(GameTime gameTime)
        {
            if (!Globals.IsUserWriting && Game1.IsActive)
            {
                foreach (var key in Globals.PressedKeyboardKeys)
                {
                    if (!Globals.MainMenu.IsActive)
                    {
                        switch (key)
                        {
                            case Keys.D:
                                if (!player.IsDead)
                                    player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
                                break;
                            case Keys.A:
                                if (!player.IsDead)
                                    player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
                                break;
                            case Keys.S:
                                if (!player.IsDead)
                                    player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
                                break;
                            case Keys.W:
                                if (!player.IsDead)
                                    player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
                                break;
                            case Keys.Space:
                                if (!player.IsDead)
                                {
                                    player.UsePrimary();
                                }
                                break;
                        }
                    }
                }
                foreach (var key in Globals.NewKeyboardKeys)
                {
                    if (!Globals.MainMenu.IsActive)
                    {
                        switch (key)
                        {
                            case Keys.D1:
                                player.Hotbar.Selected = 0;
                                break;
                            case Keys.D2:
                                player.Hotbar.Selected = 1;
                                break;
                            case Keys.D3:
                                player.Hotbar.Selected = 2;
                                break;
                            case Keys.D4:
                                player.Hotbar.Selected = 3;
                                break;
                            case Keys.D5:
                                player.Hotbar.Selected = 4;
                                break;
                            case Keys.D6:
                                player.Hotbar.Selected = 5;
                                break;
                            case Keys.D7:
                                player.Hotbar.Selected = 6;
                                break;
                            case Keys.D8:
                                player.Hotbar.Selected = 7;
                                break;
                            case Keys.D9:
                                player.Hotbar.Selected = 8;
                                break;
                            case Keys.D0:
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
                    }
                    switch (key)
                    {
                        case Keys.F11:
                            Globals.graphics.ToggleFullScreen();
                            break;
                    }
                }
                foreach (var button in Globals.PressedMouseKeys)
                {
                    if (!Globals.MainMenu.IsActive)
                    {
                        switch (button)
                        {
                            case MouseKey.RightButton:
                                if (!player.IsDead)
                                    player.UseSecondary();
                                break;
                        }
                    }
                }
                foreach (var button in Globals.NewMouseKeys)
                {
                    if (!Globals.MainMenu.IsActive)
                    {
                        switch (button)
                        {
                            case MouseKey.LeftButton:
                                for (int i = 0; i < player.Hotbar.Inventory.SlotMax; i++)
                                {
                                    if (Globals.MouseCursor.Hitbox.CollidesWith(player.Hotbar.Get(i).Hitbox))
                                    {
                                        player.Hotbar.Add(Globals.MouseCursor.CursorSlot.Add(player.Hotbar.Get(i)), i);
                                        break;
                                    }
                                }
                                Globals.shop.CheckLeftClickEvent();
                                break;
                            case MouseKey.RightButton:
                                Globals.shop.CheckRightClickEvent();
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
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace SurvivalGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseState mstate;

        Random rand = new Random();

        Player player;
        Chat chat;
        MouseCursor mouseCursor;

        public float enemySpawnRate = 2f;
        float timeSinceEnemySpawn = 999999f;
        float wallPlacementCooldown = 0.3f;
        float timeSinceWallPlacement = 999999f;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1200;
            Globals.graphics = graphics;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();



            //Globals.Textures.Add(TextureName.Circle.ToString(), Content.Load<Texture2D>("Circle"));
            Globals.SpriteFonts.Add(SpriteFontName.Aerial16, this.Content.Load<SpriteFont>("Chat"));

            //player = EntityTracker.Add.Player();
            player = new Player();
            mouseCursor = new MouseCursor();
            chat = new Chat(graphics);
            Globals.Command = new Command(this);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here

            void addTexture(string fileName)
            {
                Globals.Textures.Add(fileName, Content.Load<Texture2D>(fileName));
            }
            Globals.Textures.Add(TextureName.Rectangle.ToString(), Utilities.CreateTexture(Color.White, GraphicsDevice));
            addTexture("Circle");
            addTexture("PistolItem");
            addTexture("SwordItem");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            mstate = Mouse.GetState();
            // TODO: Add your update logic here

            UpdateKeys();
            OnKeyDown(gameTime);

            Globals.Command.DoCommand(this);

            timeSinceWallPlacement += (float)gameTime.ElapsedGameTime.TotalSeconds;

            EntityTracker.UpdateEntities(gameTime);

            Globals.Updatables = Globals.Updatables.Where(o => !o.IsDead).ToList();
            foreach (var updatable in Globals.Updatables)
            {
                if (updatable.UpdateEnabled)
                    updatable.Update(gameTime);
            }
            SpawnEnemy(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.ForestGreen);
            // TODO: Add your drawing code here

            spriteBatch.Begin(SpriteSortMode.BackToFront);

            foreach (var drawing in Globals.Drawings)
            {
                if (drawing.IsDrawn)
                    spriteBatch.Draw(Globals.Textures[drawing.Texture.ToString()], drawing.Position, null, drawing.Color, drawing.Rotation, Vector2.Zero, drawing.Scale, SpriteEffects.None, drawing.LayerDepth);
            }
            foreach (var drawingText in Globals.DrawingTexts)
            {
                if (drawingText.IsDrawn)
                    spriteBatch.DrawString(Globals.SpriteFonts[drawingText.SpriteFont], drawingText.Text, drawingText.Position, drawingText.Color, drawingText.Rotation, Vector2.Zero, drawingText.Scale, SpriteEffects.None, drawingText.LayerDepth);
            }

            spriteBatch.End();
            base.Draw(gameTime);
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
            if((int)mstate.LeftButton == 1)
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
            if (!Globals.IsUserWriting && this.IsActive)
            {
                foreach(var key in Globals.PressedKeyboardKeys)
                {
                    switch (key)
                    {
                        case Keys.D:
                            if(!player.IsDead)
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
                foreach(var key in Globals.NewKeyboardKeys)
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
                        case Keys.F11:
                            graphics.ToggleFullScreen();
                            break;
                        case Keys.T:
                            chat.NewLine();
                            break;
                    }
                }
                foreach(var button in Globals.PressedMouseKeys)
                {
                    switch (button)
                    {
                        case MouseKey.RightButton:
                            if (!player.IsDead)
                                player.UseSecondary();
                            break;
                    }
                }
                foreach(var button in Globals.NewMouseKeys)
                {
                    switch (button)
                    {
                        case MouseKey.LeftButton:
                            for (int i = 0; i < player.Hotbar.Inventory.SlotMax; i++)
                            {
                                if (mouseCursor.Hitbox.CollisionDetect(player.Hotbar.Get(i).Hitbox) != Vector2.Zero)
                                {
                                    player.Hotbar.Add(mouseCursor.CursorSlot.Add(player.Hotbar.Get(i)), i);
                                    break;
                                }
                            }
                            break;
                    }
                }
            }
        }
        void SpawnEnemy(GameTime gameTime)
        {
            timeSinceEnemySpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (1 / timeSinceEnemySpawn < enemySpawnRate && EntityTracker.GetEntities<Enemy>().Count < 20)
            {
                if (rand.Next(4) < 1)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Enemy enemy;
                        bool suitableSpot = true;
                        if (rand.Next(2) == 1)
                            enemy = new Enemy(TextureName.Circle,
                                rand.Next(0, Globals.graphics.PreferredBackBufferWidth),
                                rand.Next(0, Globals.graphics.PreferredBackBufferHeight),
                                rand.Next(15, 25), target: player, color: Color.DarkSlateGray);
                        else
                            enemy = new Enemy(TextureName.Rectangle,
                                rand.Next(0, Globals.graphics.PreferredBackBufferWidth),
                                rand.Next(0, Globals.graphics.PreferredBackBufferHeight),
                                rand.Next(15, 25), rand.Next(25, 35), target: player, color: Color.DarkGray);


                        foreach (var entity in EntityTracker.Entities)
                        {
                            if (enemy != entity && enemy.CollidesWith(entity))
                            {
                                suitableSpot = false;
                                break;
                            }
                        }
                        if (enemy.Hitbox.Distance(player.Hitbox) < 200)
                            suitableSpot = false;
                        if (!suitableSpot)
                        {
                            EntityTracker.Entities.Remove(enemy);
                            enemy.Kill();
                        }
                        else
                            break;
                    }
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        SlimeEnemy slime;
                        bool suitableSpot = true;
                        slime = new SlimeEnemy(
                            rand.Next(0, Globals.graphics.PreferredBackBufferWidth),
                            rand.Next(0, Globals.graphics.PreferredBackBufferHeight),
                            player);
                        foreach (var e in EntityTracker.Entities)
                        {
                            if (slime != e && slime.CollidesWith(e))
                            {
                                suitableSpot = false;
                                break;
                            }
                        }
                        if (slime.Hitbox.Distance(player.Hitbox) < 200)
                            suitableSpot = false;
                        if (!suitableSpot)
                        {
                            EntityTracker.Entities.Remove(slime);
                            slime.Kill();
                        }
                        else
                            break;
                    }
                }

                timeSinceEnemySpawn = 0f;
            }
        }
    }
}

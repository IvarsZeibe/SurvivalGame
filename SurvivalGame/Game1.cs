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

        float enemySpawnRate = 2f;
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
            Globals.Textures.Add(TextureName.Rectangle, Utilities.CreateTexture(Color.White, GraphicsDevice));
            Globals.Textures.Add(TextureName.Circle, Content.Load<Texture2D>("Circle"));
            Globals.SpriteFonts.Add(SpriteFontName.Aerial16, this.Content.Load<SpriteFont>("Chat"));

            player = EntityTracker.Add.Player();
            mouseCursor = EntityTracker.Add.MouseCursor();
            chat = new Chat(graphics);

            base.Initialize();
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
                    spriteBatch.Draw(Globals.Textures[drawing.Texture], drawing.Position, null, drawing.Color, drawing.Rotation, Vector2.Zero, drawing.Scale, SpriteEffects.None, drawing.LayerDepth);
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
        //Texture2D CreateTexture(Color color)
        //{
        //    Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
        //    Color[] data = new Color[1] { color };
        //    texture.SetData(data);
        //    return texture;
        //}
        void OnKeyDown(/*List<string> keysPressed,*/ GameTime gameTime)
        {
            if (!Globals.IsUserWriting)
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
                                player.PrimaryAttack();
                            break;

                        case Keys.V:
                            if (!player.IsDead)
                                player.SecondaryAttack();
                            break;
                    }
                }
                foreach(var key in Globals.NewKeyboardKeys)
                {
                    switch (key)
                    {
                        case Keys.D1:
                            //if (player.Primary.Name == "pistol")
                            //{
                            //    player.Primary.Cooldown = 0.1f;
                            //    player.Primary.Name = "mini";
                            //}
                            //else
                            //{
                            //    player.Primary.Cooldown = 0.3f;
                            //    player.Primary.Name = "pistol";
                            //}
                            //player.Primary = player.Hotbar.Get(0);
                            player.Hotbar.Selected = 0;
                            break;
                        case Keys.D2:
                            //player.Primary = player.Hotbar.Get(1);
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
                            //chat.IsDrawn = true; ;
                            //EntityTracker.ObjectsWithUpdate.Add(chat);
                            chat.NewLine();
                            break;
                    }
                }
                foreach(var button in Globals.PressedMouseKeys)
                {
                    switch (button)
                    {
                        case MouseKey.RightButton:
                            DeleteWall();
                            break;
                    }
                }
                foreach(var button in Globals.NewMouseKeys)
                {
                    switch (button)
                    {
                        case MouseKey.LeftButton:
                            MakeWall();
                            break;
                    }
                }
            }
        }

        void SpawnEnemy(GameTime gameTime)
        {
            timeSinceEnemySpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceEnemySpawn > enemySpawnRate && EntityTracker.GetEntities<Enemy>().Count < 10 )
            {
                int i = 0;
                while (i < 10)
                {
                    Enemy enemy;
                    bool suitableSpot = true;
                    if(rand.Next(2) == 1)
                        enemy = EntityTracker.Add.Enemy(TextureName.Circle, rand.Next(0, 500), rand.Next(0, 500), rand.Next(15, 25), target: player, color: Color.DarkSlateGray);
                    else
                        enemy = EntityTracker.Add.Enemy(TextureName.Rectangle, rand.Next(0, 500), rand.Next(0, 500), rand.Next(15, 25), rand.Next(25, 35), target: player, color: Color.DarkGray);


                    foreach (var entity in EntityTracker.Entities)
                    {
                        if (enemy != entity && enemy.CollidesWith(entity))
                        {
                            suitableSpot = false;
                            break;
                        }
                    }
                    if (!suitableSpot)
                    {
                        //EntityTracker.Entities.Remove(enemy);
                        enemy.Kill();
                    }
                    else
                        break;
                    i++;
                }
                timeSinceEnemySpawn = 0f;
            }
        }
        void MakeWall()
        {
            if (timeSinceWallPlacement > wallPlacementCooldown)
            {
                var wall = EntityTracker.Add.Wall(TextureName.Rectangle, mstate.X, mstate.Y);

                bool suitableSpot = true;
                foreach (var entity in EntityTracker.Entities)
                {
                    if (entity is MouseCursor)
                        continue;
                    if (wall.CollidesWith(entity) && entity != wall)
                    {
                        suitableSpot = false;
                    }
                }
                if (!suitableSpot)
                {
                    wall.Kill();
                    MakeGhost();
                }
                else
                    timeSinceWallPlacement = 0f;
            }
        }
        void MakeGhost()
        {
            var wallGhost = EntityTracker.Add.Wall(TextureName.Rectangle, mstate.X, mstate.Y, false);

            bool intersects = false;
            foreach (var w in EntityTracker.GetEntities<Wall>())
            {
                if (!w.Collision && wallGhost.CollidesWith(w) && w != wallGhost)
                {
                    intersects = true;
                }
            }
            if (intersects)
            {
                EntityTracker.Entities.Remove(wallGhost);
            }
        }
        void DeleteWall()
        {
            foreach (var wall in EntityTracker.GetEntities<Wall>())
            {
                if (wall.Collision && wall.CollidesWith(mouseCursor))
                {
                    wall.Kill();
                    break;
                }
            }
        }
    }
}

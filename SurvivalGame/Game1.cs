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
        /*public static */GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState kstate;
        MouseState mstate;

        Random rand = new Random();
        Point defaultRes = new Point(800, 480);

        //List<string> mouseKeyHistory = new List<string>();
        public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        Player player;
        //Enemy enemy;
        MouseCursor mouseCursor;

        float enemySpawnRate = 1f;
        float timeSinceEnemySpawn = 999999f;
        float wallPlacementCooldown = 0.3f;
        float timeSinceWallPlacement = 999999f;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1200;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //textures.Add("Player", CreateTexture(Color.Red));
            //textures.Add("Enemy", CreateTexture(Color.White));
            //textures.Add("MouseCursor", CreateTexture(Color.White));
            //textures.Add("Bullet", CreateTexture(Color.Orange));
            //textures.Add("Flame", CreateTexture(Color.DarkOrange));
            //textures.Add("Wall", CreateTexture(Color.SaddleBrown));
            //textures.Add("Sword", CreateTexture(Color.White));
            //var color = Color.SaddleBrown;
            //color.A = 100;
            //textures.Add("WallGhost", CreateTexture(color));

            //textures.Add("Player", this.Content.Load<Texture2D>("Untitled"));

            textures.Add("Circle", this.Content.Load<Texture2D>("Circle"));
            textures.Add("Rectangle", CreateTexture(Color.White));
            Globals.Textures.Add(TextureName.Rectangle, CreateTexture(Color.White));
            Globals.SpriteFonts.Add(SpriteFontName.Chat, this.Content.Load<SpriteFont>("Chat"));

            player = EntityTracker.Add.Player(textures["Circle"]);
            mouseCursor = EntityTracker.Add.MouseCursor(textures["Rectangle"]);

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
            kstate = Keyboard.GetState();
            mstate = Mouse.GetState();
            // TODO: Add your update logic here

            UpdateKeys();


            Utility.DetectKeyReleased(kstate, mstate);
            foreach (var key in Utility.DetectKeyPressed(kstate, mstate))
            {

                Utility.newKeys.Add(key);
            }

            OnKeyDown(gameTime);
            OnKeyUp(Utility.newKeys, gameTime);

            timeSinceWallPlacement += (float)gameTime.ElapsedGameTime.TotalSeconds;

            EntityTracker.UpdateEntities(gameTime);

            timeSinceEnemySpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
            SpawnEnemy(gameTime);

            foreach(var i in Utility.DetectKeyPressed(kstate, mstate))
            {
                Utility.keyHistory.Add(i);
            }
            Utility.newKeys.Clear();
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

            foreach(var entity in EntityTracker.Entities)
            {
                spriteBatch.Draw(entity.Texture, entity.Drawing, null, entity.Color, entity.Rotation, Vector2.Zero, SpriteEffects.None, entity.LayerDepth);
            }
            foreach(var drawing in EntityTracker.Drawings)
            {
                spriteBatch.Draw(Globals.Textures[drawing.Texture], drawing.Position, null, drawing.Color, drawing.Rotation, Vector2.Zero, drawing.Scale, SpriteEffects.None, drawing.LayerDepth);
            }
            foreach(var text in EntityTracker.DrawingsText)
            {
                spriteBatch.DrawString(Globals.SpriteFonts[text.SpriteFont], text.Text, text.Position, text.Color, text.Rotation, Vector2.Zero, text.Scale, SpriteEffects.None, text.LayerDepth);
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

        Texture2D CreateTexture(Color color)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] data = new Color[1] { color };
            texture.SetData(data);
            return texture;
        }
        void OnKeyDown(/*List<string> keysPressed,*/ GameTime gameTime)
        {
            if (!Globals.IsTextBoxActive)
            {
                foreach(var key in Globals.PressedKeyboardKeys)
                {
                    switch (key)
                    {
                        case Keys.D:
                            player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
                            break;
                        case Keys.A:
                            player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
                            break;

                        case Keys.S:
                            player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
                            break;

                        case Keys.W:
                            player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
                            break;

                        case Keys.Space:
                            player.PrimaryAttack();
                            break;

                        case Keys.V:
                            player.SecondaryAttack();
                            break;
                    }
                }
                foreach(var key in Globals.NewKeyboardKeys)
                {
                    switch (key)
                    {
                        case Keys.D1:
                            if (player.Primary == Player.Weapon.Pistol)
                                player.Primary = Player.Weapon.Minigun;
                            else
                                player.Primary = Player.Weapon.Pistol;
                            break;
                        case Keys.F11:
                            graphics.ToggleFullScreen();
                            break;
                        case Keys.Enter:
                            new Chat(graphics);
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
                //foreach (string key in Utility.keyHistory)
                //{
                //    switch (key)
                //    {
                //        case "LeftButton":
                //            if (keysPressed.Contains("LeftButton"))
                //                MakeWall();
                //            break;

                //        case "RightButton":
                //            DeleteWall();
                //            break;

                //        case "D1":
                //            if (keysPressed.Contains("D1"))
                //            {
                //                if (player.Primary == Player.Weapon.Pistol)
                //                    player.Primary = Player.Weapon.Minigun;
                //                else
                //                    player.Primary = Player.Weapon.Pistol;
                //            }
                //            break;

                //        case "F11":
                //            if (keysPressed.Contains("F11"))
                //                graphics.ToggleFullScreen();
                //            break;

                //        case "D":
                //            player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
                //            break;

                //        case "A":
                //            player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
                //            break;

                //        case "S":
                //            player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
                //            break;

                //        case "W":
                //            player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
                //            break;

                //        case "Space":
                //            player.PrimaryAttack();
                //            break;

                //        case "V":
                //            player.SecondaryAttack();
                //            break;
                //        case "Enter":
                //            if (keysPressed.Contains("Enter"))
                //            {
                //                new Chat(graphics);
                //                //new TextBox(CreateTexture(Color.Black), null, graphics);
                //            }
                //            break;
                //        default:
                //            break;
                //    }
                //}
            }
        }
        void OnKeyUp(List<string> keysReleased, GameTime gameTime)
        {

        }
        //List<string> DetectKeyPressed(KeyboardState kstate, MouseState mstate)
        //{
        //    List<string> keysPressed = new List<string>();
        //    foreach (Keys key in kstate.GetPressedKeys())
        //    {
        //        if (!keyHistory.Contains(key.ToString()))
        //        {
        //            keysPressed.Add(key.ToString());
        //            keyHistory.Add(key.ToString());
        //        }
        //    }
        //    if (mstate.LeftButton.ToString() == "Pressed")
        //    {
        //        if (!keyHistory.Contains("LeftButton"))
        //        {
        //            keysPressed.Add("LeftButton");
        //            keyHistory.Add("LeftButton");
        //        }
        //    }
        //    if (mstate.RightButton.ToString() == "Pressed")
        //    {
        //        if (!keyHistory.Contains("RightButton"))
        //        {
        //            keysPressed.Add("RightButton");
        //            keyHistory.Add("RightButton");
        //        }
        //    }
        //    return keysPressed;
        //}
        //List<string> DetectKeyReleased(KeyboardState kstate, MouseState mstate)
        //{
        //    List<string> keysReleased = new List<string>();
        //    foreach (string key in keyHistory)
        //    {
        //        bool keyFound = false;
        //        foreach (Keys k in kstate.GetPressedKeys())
        //        {
        //            if (key == k.ToString())
        //            {
        //                keyFound = true;
        //            }
        //        }
        //        if (key == "LeftButton")
        //        {
        //            keyFound = false;
        //            if (!(mstate.LeftButton.ToString() == "Released"))
        //            {
        //                keyFound = true;
        //            }
        //        }
        //        if (key == "RightButton")
        //        {
        //            keyFound = false;
        //            if (!(mstate.RightButton.ToString() == "Released"))
        //            {
        //                keyFound = true;
        //            }
        //        }
        //        if (!keyFound)
        //        {
        //            keysReleased.Add(key);
        //        }
        //    }

        //    foreach (string key in keysReleased)
        //    {
        //        if (keyHistory.Contains(key))
        //        {
        //            keyHistory.Remove(key);
        //        }
        //    }
        //    return keysReleased;
        //}

        void SpawnEnemy(GameTime gameTime)
        {
            if (timeSinceEnemySpawn > enemySpawnRate && EntityTracker.GetEntities<Enemy>().Count < 100 )
            {
                int i = 0;
                while (i < 10)
                {
                    Enemy enemy;
                    bool suitableSpot = true;
                    if(rand.Next(2) == 1)
                        enemy = EntityTracker.Add.Enemy(textures["Circle"], rand.Next(0, 500), rand.Next(0, 500), rand.Next(15, 25), target: player, color: Color.DarkSlateGray);
                    else
                        enemy = EntityTracker.Add.Enemy(textures["Rectangle"], rand.Next(0, 500), rand.Next(0, 500), rand.Next(15, 25), rand.Next(25, 35), target: player, color: Color.DarkGray);


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
                        EntityTracker.Entities.Remove(enemy);
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
            bool success = true;
            if (timeSinceWallPlacement > wallPlacementCooldown)
            {
                var wall = EntityTracker.Add.Wall(textures["Rectangle"], mstate.X, mstate.Y);

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
                    EntityTracker.Entities.Remove(wall);

                    timeSinceWallPlacement = 0f;

                    success = false;
                }
            }
            if (!success)
            {
                MakeGhost();
            }
        }
        void MakeGhost()
        {
            var wallGhost = EntityTracker.Add.Wall(textures["Rectangle"], mstate.X, mstate.Y, false);

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
            //Wall wall = null;
            foreach (var wall in EntityTracker.GetEntities<Wall>())
            {
                if (wall.Collision && wall.CollidesWith(mouseCursor))
                {
                    //targetFound = true;
                    //wall = w;
                    wall.IsDead = true;
                    break;
                }
            }
            //if(targetFound)
            //    walls.Remove(wall);
            //if (targetFound)
            //{
            //    entities.Remove(wall);
            //    walls.Remove(wall);
            //}
        }
    }
}

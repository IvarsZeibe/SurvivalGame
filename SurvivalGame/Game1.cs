﻿using Microsoft.Xna.Framework;
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
        KeyboardState kstate;
        MouseState mstate;

        Random rand = new Random();
        Point defaultRes = new Point(800, 480);

        List<string> keyHistory = new List<string>();
        //List<string> mouseKeyHistory = new List<string>();
        Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        Player player;
        //Enemy enemy;
        MouseCursor mouseCursor;

        float rateOfFire = 0.2f;
        float timeSinceLastShot = 9999999f;
        int bulletType = 0;
        float swordCooldown = 0.3f;
        float timeSinceSwordAttack = 9999999f;
        float enemySpawnRate = .1f;
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
            textures.Add("Enemy", CreateTexture(Color.Black));
            textures.Add("MouseCursor", CreateTexture(Color.White));
            textures.Add("Bullet", CreateTexture(Color.Orange));
            textures.Add("Flame", CreateTexture(Color.DarkOrange));
            textures.Add("Wall", CreateTexture(Color.SaddleBrown));
            textures.Add("Sword", CreateTexture(Color.White));
            var color = Color.SaddleBrown;
            color.A = 100;
            textures.Add("WallGhost", CreateTexture(color));

            textures.Add("Player", this.Content.Load<Texture2D>("Untitled"));

            textures.Add("Circle", this.Content.Load<Texture2D>("Circle"));

            player = EntityTracker.Add.Player(textures["Player"]);
            mouseCursor = EntityTracker.Add.MouseCursor(textures["MouseCursor"]);

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
            OnKeyDown(DetectKeyPressed(kstate, mstate), gameTime);
            OnKeyUp(DetectKeyReleased(kstate, mstate), gameTime);

            timeSinceWallPlacement += (float)gameTime.ElapsedGameTime.TotalSeconds;

            EntityTracker.UpdateEntities(gameTime);

            timeSinceEnemySpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
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

            foreach(var entity in EntityTracker.Entities)
            {
                spriteBatch.Draw(entity.Texture, entity.Drawing, null, Color.White, entity.Rotation, Vector2.Zero, SpriteEffects.None, entity.LayerDepth);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }



        Texture2D CreateTexture(Color color)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] data = new Color[1] { color };
            texture.SetData(data);
            return texture;

        }
        void OnKeyDown(List<string> keysPressed, GameTime gameTime)
        {
            ///keysPressed = new key
            ///keyHistory = all pressed keys
            if (keysPressed.Contains("LeftButton"))
            {
                MakeWall();
            }
            if (keyHistory.Contains("RightButton"))
            {
                DeleteWall();
            }
            if (keysPressed.Contains(Keys.D1.ToString()))
            {
                if (bulletType == 0)
                {
                    bulletType = 1;
                    rateOfFire = 0.02f;
                }
                else
                {
                    bulletType = 0;
                    rateOfFire = 0.2f;
                }
            }
            if (keysPressed.Contains(Keys.F11.ToString()))
            {
                graphics.ToggleFullScreen();
            }
            if (keyHistory.Contains(Keys.D.ToString()))
            {
                player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
            }
            else if (keyHistory.Contains(Keys.A.ToString()))
            {
                player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
            }
            if (keyHistory.Contains(Keys.S.ToString()))
            {
                player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
            }
            else if (keyHistory.Contains(Keys.W.ToString()))
            {
                player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
            }
            timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyHistory.Contains(Keys.Space.ToString()))
            {
                if (bulletType == 0 && timeSinceLastShot > rateOfFire)
                {

                    Projectile projectile = EntityTracker.Add.Projectile(textures["Bullet"], 500f, new Vector2(player.X, player.Y), new Vector2(mouseCursor.X, mouseCursor.Y), 100);

                    timeSinceLastShot = 0f;
                }
                else if (bulletType == 1 && timeSinceLastShot > rateOfFire)
                {
                    Projectile projectile = EntityTracker.Add.Projectile(textures["Flame"], 500f, new Vector2(player.X, player.Y), new Vector2(mouseCursor.X, mouseCursor.Y), 200);

                    timeSinceLastShot = 0f;
                }
            }
            timeSinceSwordAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyHistory.Contains(Keys.V.ToString()) && timeSinceSwordAttack > swordCooldown)
            {
                timeSinceSwordAttack = 0f;
                double yEdge = (player.Y - mouseCursor.Y);
                double xEdge = (player.X - mouseCursor.X);

                Sword sword = EntityTracker.Add.Sword(textures["Sword"], player, (float)Math.Atan2(yEdge, xEdge));
            }
        }
        void OnKeyUp(List<string> keysReleased, GameTime gameTime)
        {

        }
        List<string> DetectKeyPressed(KeyboardState kstate, MouseState mstate)
        {
            List<string> keysPressed = new List<string>();
            foreach (Keys key in kstate.GetPressedKeys())
            {
                if (!keyHistory.Contains(key.ToString()))
                {
                    keysPressed.Add(key.ToString());
                    keyHistory.Add(key.ToString());
                }
            }
            if (mstate.LeftButton.ToString() == "Pressed")
            {
                if (!keyHistory.Contains("LeftButton"))
                {
                    keysPressed.Add("LeftButton");
                    keyHistory.Add("LeftButton");
                }
            }
            if (mstate.RightButton.ToString() == "Pressed")
            {
                if (!keyHistory.Contains("RightButton"))
                {
                    keysPressed.Add("RightButton");
                    keyHistory.Add("RightButton");
                }
            }
            return keysPressed;
        }
        List<string> DetectKeyReleased(KeyboardState kstate, MouseState mstate)
        {
            List<string> keysReleased = new List<string>();
            foreach (string key in keyHistory)
            {
                bool keyFound = false;
                foreach (Keys k in kstate.GetPressedKeys())
                {
                    if (key == k.ToString())
                    {
                        keyFound = true;
                    }
                }
                if (key == "LeftButton")
                {
                    keyFound = false;
                    if (!(mstate.LeftButton.ToString() == "Released"))
                    {
                        keyFound = true;
                    }
                }
                if (key == "RightButton")
                {
                    keyFound = false;
                    if (!(mstate.RightButton.ToString() == "Released"))
                    {
                        keyFound = true;
                    }
                }
                if (!keyFound)
                {
                    keysReleased.Add(key);
                }
            }

            foreach (string key in keysReleased)
            {
                if (keyHistory.Contains(key))
                {
                    keyHistory.Remove(key);
                }
            }
            return keysReleased;
        }

        void SpawnEnemy(GameTime gameTime)
        {
            if (timeSinceEnemySpawn > enemySpawnRate && EntityTracker.Enemies.Count < 100 )
            {
                int i = 0;
                while (i < 10)
                {
                    Enemy enemy;
                    bool suitableSpot = true;
                    if(rand.Next(2) == 1)
                        enemy = EntityTracker.Add.Enemy(textures["Circle"], rand.Next(0, 500), rand.Next(0, 500), rand.Next(15, 25), target: player);
                    else
                        enemy = EntityTracker.Add.Enemy(textures["Enemy"], rand.Next(0, 500), rand.Next(0, 500), rand.Next(15, 25), rand.Next(25, 35), target: player);


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
                        break;
                    }
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
                var wall = EntityTracker.Add.Wall(textures["Wall"], mstate.X, mstate.Y);

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
            var wallGhost = EntityTracker.Add.Wall(textures["WallGhost"], mstate.X, mstate.Y, false);

            bool intersects = false;
            foreach (var w in EntityTracker.Walls)
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
            foreach (var wall in EntityTracker.Walls)
            {
                if (wall.Collision && wall.CollidesWith(mouseCursor))
                {
                    //targetFound = true;
                    //wall = w;
                    wall.isDead = true;
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

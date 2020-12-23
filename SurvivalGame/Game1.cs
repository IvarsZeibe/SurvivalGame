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

        List<Entity> entities = new List<Entity>();
        //List<Bullet> bullets = new List<Bullet>();
        List<Projectile> projectiles = new List<Projectile>();
        List<Enemy> enemies = new List<Enemy>();
        List<Wall> walls = new List<Wall>();
        List<Sword> swords = new List<Sword>();

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

            player = new Player(textures["Player"]);
            entities.Add(player);
            //for(int i = 0; i < 10; i++)
            //{
            //    enemies.Add(new Enemy(textures["Enemy"], rand.Next(0, 500), rand.Next(0, 500)));
            //}
            mouseCursor = new MouseCursor(textures["MouseCursor"]);
            //entities.Add(mouseCursor);

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

            //if (mstate.LeftButton.ToString() == "Pressed")
            //{
            timeSinceWallPlacement += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //}


            mouseCursor.Update(mstate);

            List<Entity> deadEntities = new List<Entity>();
            UpdateEntities(gameTime, deadEntities);

            KillDeadEntities(deadEntities);

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
            spriteBatch.Begin();

            foreach (var projectile in projectiles)
            {
                spriteBatch.Draw(projectile.Texture, projectile.Rect, null, Color.White, projectile.Rotation, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
            }

            //spriteBatch.Draw(player.Texture, player.Rect, Color.White
            spriteBatch.Draw(player.Texture, new Rectangle((int)(player.Hitbox.X - (player.Size.X / 2f)), (int)(player.Hitbox.Y - (player.Size.Y / 2f)), player.Size.X, player.Size.Y), Color.White);
            //spriteBatch.Draw(player.Texture, new Vector2((float)player.X, (float)player.Y), null, Color.White, 0, Vector2.Zero, new Vector2(player.Size.X / player.Texture.Width, player.Size.Y / player.Texture.Height), SpriteEffects.None, 0);

            foreach (var enemy in enemies)
            {
                //spriteBatch.Draw(enemy.Texture, enemy.Rect, Color.White);
                spriteBatch.Draw(enemy.Texture, new Rectangle((int)(enemy.Hitbox.X - (enemy.Rect.Width / 2f)), (int)(enemy.Hitbox.Y - (enemy.Rect.Height / 2f)), enemy.Rect.Width, enemy.Rect.Height), Color.White);
                //spriteBatch.Draw(enemy.Texture, new Rectangle((int)enemy.X + (enemy.Size.X / 2), (int)enemy.Y + (enemy.Size.Y / 2), enemy.Size.X, enemy.Size.Y), Color.White);
            }
            foreach (var wall in walls)
            {
                spriteBatch.Draw(wall.Texture, wall.Rect, Color.White);
            }
            foreach (var sword in swords)
            {
                spriteBatch.Draw(sword.Texture, sword.DrawRect, null, Color.White, sword.Rotation, new Vector2(0f, 0f), SpriteEffects.None, 0);
                //spriteBatch.Draw(sword.Texture, sword.Rect, Color.White);
            }

            spriteBatch.Draw(mouseCursor.Texture, mouseCursor.Rect, Color.White);


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
            //keysPressed = new key
            //keyHistory = all pressed keys
            if (keysPressed.Contains("LeftButton"))
            {
                //MakeWall();
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
                //MoveV5(player, player.Speed * gameTime.ElapsedGameTime.TotalSeconds, 'x');
                MoveV6(player, 'x', player.Speed * gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (keyHistory.Contains(Keys.A.ToString()))
            {
                //MoveV5(player, -player.Speed * gameTime.ElapsedGameTime.TotalSeconds, 'x');
                MoveV6(player, 'x', -player.Speed * gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (keyHistory.Contains(Keys.S.ToString()))
            {
                //MoveV5(player, player.Speed * gameTime.ElapsedGameTime.TotalSeconds, 'y');
                MoveV6(player, 'y', player.Speed * gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (keyHistory.Contains(Keys.W.ToString()))
            {
                //MoveV5(player, -player.Speed * gameTime.ElapsedGameTime.TotalSeconds, 'y');
                MoveV6(player, 'y', -player.Speed * gameTime.ElapsedGameTime.TotalSeconds);
            }
            timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyHistory.Contains(Keys.Space.ToString()))
            {
                if (bulletType == 0 && timeSinceLastShot > rateOfFire)
                {

                    Projectile projectile = new Projectile(textures["Bullet"], 500f, player.Center, mouseCursor.Center, 100);
                    projectiles.Add(projectile);
                    entities.Add(projectile);

                    timeSinceLastShot = 0f;
                }
                else if (bulletType == 1 && timeSinceLastShot > rateOfFire)
                {
                    Projectile projectile = new Projectile(textures["Flame"], 300f, player.Center, mouseCursor.Center, 20);
                    projectiles.Add(projectile);
                    entities.Add(projectile);

                    timeSinceLastShot = 0f;
                }
            }
            //if (keyHistory.Contains(Keys.Space.ToString()) && timeSinceLastShot > rateOfFire)
            //{
            //    timeSinceLastShot = 0f;
            //    double yEdge = (player.Center.Y - mouseCursor.Center.Y);
            //    double xEdge = (player.Center.X - mouseCursor.Center.X);
            //    Bullet bullet = new Bullet(textures["Bullet"], 500f, player.Center.X, player.Center.Y, (float)Math.Atan2(yEdge, xEdge), new Vector2((float)xEdge, (float)yEdge));
            //    bullets.Add(bullet);
            //    entities.Add(bullet);
            //}
            timeSinceSwordAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyHistory.Contains(Keys.V.ToString()) && timeSinceSwordAttack > swordCooldown)
            {
                timeSinceSwordAttack = 0f;
                double yEdge = (player.Center.Y - mouseCursor.Center.Y);
                double xEdge = (player.Center.X - mouseCursor.Center.X);
                Sword sword = new Sword(textures["Sword"], player, (float)Math.Atan2(yEdge, xEdge));
                swords.Add(sword);
                entities.Add(sword);
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

        void UpdateEntities(GameTime gameTime, List<Entity> deadEntities)
        {
            foreach (var ent in entities)
            {
                if (ent is Player)
                {
                    player.Update();
                }
                else if (ent is Enemy)
                {
                    Enemy enemy = ent as Enemy;
                    enemy.Update(gameTime);
                    enemy.Movement(enemy.Hitbox.X - player.Hitbox.X, enemy.Hitbox.Y - player.Hitbox.Y);
                    //MoveV5(enemy, enemy.XMovement * gameTime.ElapsedGameTime.TotalSeconds, 'x');

                   // MoveV5(enemy, enemy.YMovement * gameTime.ElapsedGameTime.TotalSeconds, 'y');
                    foreach (var projectile in projectiles)
                    {
                        if (enemy.Rect.Intersects(projectile.Rect))
                        {
                            enemy.DamageEntity(projectile.Damage, "Projectile");
                            projectile.isDead = true;
                        }
                    }
                    foreach (var sword in swords)
                    {
                        if (enemy.Rect.Intersects(sword.Rect) && !sword.hitEntities.Contains(enemy))
                        {
                            enemy.DamageEntity(sword.Damage, "Sword");
                            sword.hitEntities.Add(enemy);
                        }
                    }
                    enemy.Update();
                }
                else if (ent is Projectile)
                {
                    Projectile projectile = ent as Projectile;
                    //MoveV5(projectile, projectile.XMovement * gameTime.ElapsedGameTime.TotalSeconds, 'x');
                    //MoveV5(projectile, projectile.YMovement * gameTime.ElapsedGameTime.TotalSeconds, 'y');
                    projectile.Update(gameTime);
                    foreach (var wall in walls)
                    {
                        if (projectile.Detect(wall) && wall.Collision)
                        {
                            projectile.isDead = true;
                        }
                    }
                }
                else if (ent is Sword)
                {
                    Sword sword = ent as Sword;
                    sword.Update(gameTime);
                    //foreach (var enemy in enemies)
                    //{
                    //    if (sword.Rect.Intersects(enemy.Rect))
                    //    {
                    //        enemy.Health--;
                    //        enemy.Size = new Point((int)(enemy.Size.X * 0.8), (int)(enemy.Size.Y * 0.8));
                    //    }
                    //}
                }
                else if (ent is Wall)
                {
                    Wall wall = ent as Wall;
                    wall.Update(gameTime);
                }


                if (ent.isDead)
                {
                    deadEntities.Add(ent);
                }
            }
        }

        void KillDeadEntities(List<Entity> deadEntities)
        {
            foreach (var entity in deadEntities)
            {
                entities.Remove(entity);
                if (entity is Projectile)
                {
                    projectiles.Remove(entity as Projectile);
                }
                if (entity is Enemy)
                {
                    enemies.Remove(entity as Enemy);
                }
                if (entity is Wall)
                {
                    walls.Remove(entity as Wall);
                }
                if (entity is Sword)
                {
                    swords.Remove(entity as Sword);
                }
            }
        }

        void SpawnEnemy(GameTime gameTime)
        {
            if (timeSinceEnemySpawn > enemySpawnRate && enemies.Count < 1)
            {
                int i = 0;
                while (i < 10)
                {
                    bool suitableSpot = true;
                    var enemy = new Enemy(textures["Enemy"], rand.Next(0, 500), rand.Next(0, 500), rand.Next(15, 25), rand.Next(25, 35));
                    enemy.Update();
                    foreach (var entity in entities)
                    {
                        if (enemy != entity && enemy.Rect.Intersects(entity.Rect))
                        {
                            suitableSpot = false;
                            break;
                        }
                    }
                    if (suitableSpot)
                    {
                        enemies.Add(enemy);
                        entities.Add(enemy);
                        break;
                    }
                    i++;
                }
                timeSinceEnemySpawn = 0f;
            }
        }
        void MakeWall()
        {
            //var enemy = new Enemy(textures["Enemy"], mstate.X, mstate.Y);
            //enemy.Update();
            //foreach (var e in enemies)
            //{
            //    if (enemy.Rect.Intersects(e.Rect) && e.Collision)
            //    {
            //        enemy.Texture = textures["Player"];
            //        enemy.Collision = false;
            //    }
            //}
            //enemies.Add(enemy);
            //entities.Add(enemy);

            bool success = false;
            if (timeSinceWallPlacement > wallPlacementCooldown)
            {
                var wall = new Wall(textures["Wall"], mstate.X, mstate.Y);
                wall.Update();
                bool suitableSpot = true;
                foreach (var w in walls)
                {
                    if (wall.Rect.Intersects(w.Rect))
                    {
                        suitableSpot = false;
                    }
                }
                if (suitableSpot)
                {
                    success = true;
                    entities.Add(wall);
                    walls.Add(wall);
                    timeSinceWallPlacement = 0f;
                }
            }
            if (!success)
            {
                MakeGhost();
            }
        }
        void MakeGhost()
        {
            var wallGhost = new Wall(textures["WallGhost"], mstate.X, mstate.Y, false);
            wallGhost.Update();
            bool intersects = false;
            foreach (var w in walls)
            {
                if (!w.Collision && wallGhost.Rect.Intersects(w.Rect))
                {
                    intersects = true;
                }
            }
            if (!intersects)
            {
                entities.Add(wallGhost);
                walls.Add(wallGhost);
            }
        }
        void DeleteWall()
        {
            Wall wall = null;
            bool targetFound = false;
            foreach (var w in walls)
            {
                if (w.Collision && w.Rect.Contains(mstate.X, mstate.Y))
                {
                    targetFound = true;
                    wall = w;
                }
            }
            if (targetFound)
            {
                entities.Remove(wall);
                walls.Remove(wall);
            }
        }

        //double MoveV5(Entity pusher, double movement, char xORy, List<(Entity, int)> pushedEntities = null, int parentId = -1, double mass = -1)
        //{
        //    if(mass == -1)
        //    {
        //        mass = pusher.Mass;
        //    }
        //    bool firstime = false;
        //    if (pushedEntities == null)
        //    {
        //        pushedEntities = new List<(Entity, int)>() { (pusher, -1) };
        //        firstime = true;
        //    }
        //    else //remove duplicate or destroy this duplicate
        //    {
        //        for(int i = 0; i < pushedEntities.Count; i++)
        //        {
        //            var tuple = pushedEntities[i];
        //            if(tuple.Item1 == pusher)
        //            {
        //                if (xORy == 'x')
        //                {
        //                    if (movement >= 0)
        //                    {
        //                        if (pushedEntities[parentId].Item1.X + pushedEntities[parentId].Item1.Size.X > pushedEntities[tuple.Item2].Item1.X + pushedEntities[tuple.Item2].Item1.Size.X)
        //                        {
        //                            DestroyFamilyV3(pushedEntities, pushedEntities.IndexOf(tuple));
        //                        }
        //                        else
        //                            return movement;
        //                    }
        //                    else
        //                    {
        //                        if (pushedEntities[parentId].Item1.X < pushedEntities[tuple.Item2].Item1.X)
        //                        {
        //                            DestroyFamilyV3(pushedEntities, pushedEntities.IndexOf(tuple));
        //                        }
        //                        else
        //                            return movement;
        //                    }
        //                }
        //                else
        //                {
        //                    if (movement >= 0)
        //                    {
        //                        if (pushedEntities[parentId].Item1.Y + pushedEntities[parentId].Item1.Size.Y > pushedEntities[tuple.Item2].Item1.Y + pushedEntities[tuple.Item2].Item1.Size.Y)
        //                        {
        //                            DestroyFamilyV3(pushedEntities, pushedEntities.IndexOf(tuple));
        //                        }
        //                        else
        //                            return movement;
        //                    }
        //                    else
        //                    {

        //                        if (pushedEntities[parentId].Item1.Y < pushedEntities[tuple.Item2].Item1.Y)
        //                        {
        //                            DestroyFamilyV3(pushedEntities, pushedEntities.IndexOf(tuple));
        //                        }
        //                        else
        //                            return movement;
        //                    }
        //                }
        //            }
        //        }
        //        pushedEntities.Add((pusher, parentId));
        //    }
        //    int selfId = pushedEntities.Count - 1;

        //    if (xORy == 'x') //move in x axis
        //    {
        //        double oldX = pusher.X;
        //        foreach (var entity in entities)
        //        {
        //            pusher.X = oldX;
        //            pusher.X += movement;
        //            pusher.Update();

        //            //if (pusher.Rect.Intersects(entity.Rect) && pusher != entity && entity.Collision && pusher.Collision)
        //            if(pusher != entity && entity.Collision && pusher.Collision && pusher.Hitbox.CollisionDetect(pusher.Hitbox, entity.Hitbox))
        //            {
        //                if(mass > entity.Mass) //can push
        //                {
        //                    pusher.X = oldX;
        //                    pusher.Update();
        //                    pusher.Collision = false;
        //                    movement = MoveV5(entity, movement, xORy, pushedEntities, selfId, mass);
        //                    pusher.Collision = true;
        //                }
        //                else //cant push
        //                {
        //                    if (movement >= 0)
        //                    {
        //                        movement = entity.Rect.Left - (oldX + pusher.Size.X);
        //                        if (movement < 0)
        //                            movement = 0;
        //                    }
        //                    else
        //                    {
        //                        movement = entity.Rect.Right - (oldX);
        //                        if (movement > 0)
        //                            movement = 0;
        //                    }
        //                }
        //            }
        //        }
        //        pusher.X = oldX;
        //        pusher.Update();
        //    }
        //    else // move in y axis
        //    {
        //        double oldY = pusher.Y;
        //        foreach (var entity in entities)
        //        {
        //            pusher.Y = oldY;
        //            pusher.Y += movement;
        //            pusher.Update();

        //            if (pusher != entity && entity.Collision && pusher.Collision && pusher.Hitbox.CollisionDetect(pusher.Hitbox, entity.Hitbox))
        //            {
        //                if (mass > entity.Mass) // can push
        //                {
        //                    pusher.Y = oldY;
        //                    pusher.Update();
        //                    pusher.Collision = false;
        //                    movement = MoveV5(entity, movement, xORy, pushedEntities, selfId, mass);
        //                    pusher.Collision = true;
        //                }
        //                else //cant push
        //                {
        //                    if (movement >= 0)
        //                    {
        //                        movement = entity.Rect.Top - (oldY + pusher.Size.Y);
        //                        if (movement < 0)
        //                            movement = 0;
        //                    }
        //                    else
        //                    {
        //                        movement = entity.Rect.Bottom - (oldY);
        //                        if (movement > 0)
        //                            movement = 0;
        //                    }
        //                }
        //            }
        //        }
        //        pusher.Y = oldY;
        //        pusher.Update();
        //    }

        //    if (firstime) //real push
        //    {

        //        var pushedEntitiesSorted = new List<(Entity, int)>();
        //        for (int i = 0; i < pushedEntities.Count; i++)
        //        {
        //            if (pushedEntities[i].Item1 != null)
        //            {
        //                pushedEntitiesSorted.Add(pushedEntities[i]);
        //            }

        //        }
        //        pushedEntitiesSorted = pushedEntitiesSorted.OrderBy(x => x.Item2).ToList();

        //        for(int i = pushedEntitiesSorted.Count/3; i > 0; i--) //weakens push
        //        {
        //            movement *= 0.7;
        //        }

        //        if (xORy == 'x')
        //        {
        //            pusher.X += movement;
        //        }
        //        else
        //            pusher.Y += movement;
        //        pusher.Update();

        //        for (int i = 0; i < pushedEntitiesSorted.Count; i++)
        //        {
        //            var pushed = pushedEntitiesSorted[i];
        //            if(pushed.Item1 != pusher && pushedEntities[pushed.Item2].Item1 != null && pushed.Item1 != null)
        //            {
        //                if (pushed.Item1.Rect.Intersects(pushedEntities[pushed.Item2].Item1.Rect))
        //                {
        //                    if (xORy == 'x')
        //                    {
        //                        if(movement <= 0)
        //                        {
        //                            //pushed.Item1.X = pushedEntities[pushed.Item2].Item1.X - pushed.Item1.Size.X;
        //                            pushed.Item1.X += movement;
        //                            pushed.Item1.Update();
        //                        }
        //                        else
        //                        {
        //                            //pushed.Item1.X = pushedEntities[pushed.Item2].Item1.X + pushedEntities[pushed.Item2].Item1.Size.X;
        //                            pushed.Item1.X += movement;
        //                            pushed.Item1.Update();
        //                        }
                            
        //                    }
        //                    else
        //                    {
        //                        if (movement <= 0)
        //                        {
        //                            //pushed.Item1.Y = pushedEntities[pushed.Item2].Item1.Y - pushed.Item1.Size.Y;
        //                            pushed.Item1.Y += movement;
        //                            pushed.Item1.Update();
        //                        }
        //                        else
        //                        {
        //                            //pushed.Item1.Y = pushedEntities[pushed.Item2].Item1.Y + pushedEntities[pushed.Item2].Item1.Size.Y;
        //                            pushed.Item1.Y += movement;
        //                            pushed.Item1.Update();
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    pushedEntities[i] = (null, pushed.Item2);
        //                }
        //            }
        //        }
        //    }
        //    return movement;

        //}

        void DestroyFamilyV3(List<(Entity, int)> pushedEntities, int id)
        {
            for (int i = 0; i < pushedEntities.Count; i++)
            {
                if (pushedEntities[i].Item2 == id)
                {
                    DestroyFamilyV3(pushedEntities, i);
                    pushedEntities[i] = (null, pushedEntities[i].Item2);
                }
            }
            pushedEntities[id] = (null, pushedEntities[id].Item2);
            return;
        }
        void MoveV6(Entity pusher, char xORy, double movement, List<(Entity, double)> pushedEntties = null, bool firstTime = true)
        {
            if (pusher.Collision)
            {
                if(pushedEntties == null)
                {
                    pushedEntties = new List<(Entity, double)>() { (pusher, movement) };
                }
                int index = pushedEntties.Count - 1;

                foreach(var i in pushedEntties)
                {
                    if(i.Item1 == pusher)
                    {
                        if(i.Item2 < movement)
                        {
                            index = pushedEntties.IndexOf(i);
                        }
                    }
                }
                if(index != pushedEntties.Count - 1)
                    pushedEntties.Add((pusher, movement));


                double oldX = pusher.Hitbox.X;
                double oldY = pusher.Hitbox.Y;

                if (xORy == 'x')
                {
                    pusher.Hitbox.X += movement;
                    foreach (Entity entity in entities)
                    {
                        if (entity.Collision && entity != pusher)
                        {
                            double intersect = pusher.Hitbox.CollisionDetect(pusher.Hitbox, entity.Hitbox).X;
                            Console.WriteLine("Value: " + intersect);

                            if (movement < 0 && intersect != 0)
                            {
                                //if (entity.Hitbox is Rect)
                                //    movement += intersect;
                                //else
                                //    movement += intersect;

                                //if (movement > 0)
                                //    movement = 0;
                                movement += intersect;
                            }
                            else
                                movement -= intersect;
                            //pusher.Hitbox.X = oldX;
                            //if (intersect != 0)
                            //{
                            //    if (false) // too big mass
                            //    {
                            //        movement -= intersect;
                            //    }
                            //    else
                            //    {
                            //        pusher.Collision = false;
                            //        movement -= MoveV6(entity, xORy, intersect, pushedEntties, false);
                            //        pusher.Collision = true;
                            //    }
                            //}
                        }
                    }
                    pusher.Hitbox.X = Math.Round(oldX + movement, 5);
                }
                else
                {
                    pusher.Hitbox.Y += movement;
                    foreach (Entity entity in entities)
                    {
                        if (entity.Collision && entity != pusher)
                        {
                            double intersect = pusher.Hitbox.CollisionDetect(pusher.Hitbox, entity.Hitbox).Y;
                            if (movement < 0 && intersect != 0)
                            {
                                //if (entity.Hitbox is Rect)
                                //    movement -= intersect;
                                //else
                                //    movement -= intersect;

                                //if (movement > 0)
                                //    movement = 0;
                                movement += intersect;
                            }
                            else
                                movement -= intersect;

                            //pusher.Hitbox.Y = oldY;
                            //if (intersect != 0)
                            //{
                            //    if (false) // too big mass
                            //    {
                            //        movement -= intersect;
                            //    }
                            //    else
                            //    {
                            //        pusher.Collision = false;
                            //        movement -= MoveV6(entity, xORy, intersect, pushedEntties, false);
                            //        pusher.Collision = true;
                            //    }
                            //}
                        }
                    }
                    pusher.Hitbox.Y = Math.Round(oldY + movement, 5);
                }
                pusher.Update();
                //if (firstTime)
                //{
                //    foreach
                //}

                
            }
        }



    }
}

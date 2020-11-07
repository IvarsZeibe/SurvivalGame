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
        List<Bullet> bullets = new List<Bullet>();
        List<Enemy> enemies = new List<Enemy>();
        List<Wall> walls = new List<Wall>();
        List<Sword> swords = new List<Sword>();

        float rateOfFire = 0.2f;
        float timeSinceLastShot = 9999999f;
        float swordCooldown = 0.3f;
        float timeSinceSwordAttack = 9999999f;
        float enemySpawnRate = 1.1f;
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
            textures.Add("Player", CreateTexture(Color.Red));
            textures.Add("Enemy", CreateTexture(Color.Black));
            textures.Add("MouseCursor", CreateTexture(Color.White));
            textures.Add("Bullet", CreateTexture(Color.Orange));
            textures.Add("Wall", CreateTexture(Color.SaddleBrown));
            textures.Add("Sword", CreateTexture(Color.White));
            var color = Color.SaddleBrown;
            color.A = 100;
            textures.Add("WallGhost", CreateTexture(color));

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

            foreach (var bullet in bullets)
            {
                spriteBatch.Draw(bullet.Texture, bullet.Rect, null, Color.White, bullet.Rotation, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
            }

            spriteBatch.Draw(player.Texture, player.Rect, Color.White);

            foreach (var enemy in enemies)
            {
                spriteBatch.Draw(enemy.Texture, enemy.Rect, Color.White);
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
                MakeWall();
            }
            if (keyHistory.Contains("RightButton"))
            {
                DeleteWall();
            }
            if (keysPressed.Contains(Keys.F11.ToString()))
            {
                graphics.ToggleFullScreen();
            }
            if (keyHistory.Contains(Keys.D.ToString()))
            {
                MoveV5(player, player.Speed * gameTime.ElapsedGameTime.TotalSeconds, 'x');
            }
            if (keyHistory.Contains(Keys.A.ToString()))
            {
                MoveV5(player, -player.Speed * gameTime.ElapsedGameTime.TotalSeconds, 'x');
            }
            if (keyHistory.Contains(Keys.S.ToString()))
            {
                MoveV5(player, player.Speed * gameTime.ElapsedGameTime.TotalSeconds, 'y');
            }
            if (keyHistory.Contains(Keys.W.ToString()))
            {
                MoveV5(player, -player.Speed * gameTime.ElapsedGameTime.TotalSeconds, 'y');
            }
            timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyHistory.Contains(Keys.Space.ToString()) && timeSinceLastShot > rateOfFire)
            {
                timeSinceLastShot = 0f;
                double yEdge = (player.Center.Y - mouseCursor.Center.Y);
                double xEdge = (player.Center.X - mouseCursor.Center.X);
                Bullet bullet = new Bullet(textures["Bullet"], player.Center.X, player.Center.Y, (float)Math.Atan2(yEdge, xEdge), new Vector2((float)xEdge, (float)yEdge));
                bullets.Add(bullet);
                entities.Add(bullet);
            }
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
                if (ent.GetType().Equals(typeof(Player)))
                {
                    player.Update();
                }
                else if (ent.GetType().Equals(typeof(Enemy)))
                {
                    foreach (var enemy in enemies)
                    {
                        if (enemy.Equals(ent))
                        {
                            enemy.Update(gameTime);
                            enemy.Movement(enemy.X - player.X, enemy.Y - player.Y);
                            MoveV5(enemy, enemy.XMovement * gameTime.ElapsedGameTime.TotalSeconds, 'x');

                            MoveV5(enemy, enemy.YMovement * gameTime.ElapsedGameTime.TotalSeconds, 'y');
                            foreach (var bullet in bullets)
                            {
                                if (enemy.Rect.Intersects(bullet.Rect))
                                {
                                    enemy.DamageHealth(deadEntities);
                                    deadEntities.Add(bullet);
                                }
                            }
                            foreach (var sword in swords)
                            {
                                if (enemy.Rect.Intersects(sword.Rect))
                                {
                                    enemy.DamageHealth(deadEntities);

                                }
                            }
                            enemy.Update();
                        }
                    }
                }
                else if (ent.GetType().Equals(typeof(Bullet)))
                {
                    foreach (var bullet in bullets)
                    {
                        if (bullet.Equals(ent))
                        {
                            MoveV5(bullet, bullet.XMovement * gameTime.ElapsedGameTime.TotalSeconds, 'x');
                            MoveV5(bullet, bullet.YMovement * gameTime.ElapsedGameTime.TotalSeconds, 'y');
                            bullet.Update(gameTime, deadEntities);
                            foreach (var wall in walls)
                            {
                                if (bullet.Detect(wall) && wall.Collision)
                                {
                                    deadEntities.Add(bullet);
                                }
                            }
                        }
                    }
                }
                else if (ent.GetType().Equals(typeof(Sword)))
                {
                    foreach (var sword in swords)
                    {
                        if (sword.Equals(ent))
                        {
                            sword.Update(gameTime, deadEntities);
                            //foreach (var enemy in enemies)
                            //{
                            //    if (sword.Rect.Intersects(enemy.Rect))
                            //    {
                            //        enemy.Health--;
                            //        enemy.Size = new Point((int)(enemy.Size.X * 0.8), (int)(enemy.Size.Y * 0.8));
                            //    }
                            //}
                        }
                    }
                }
                else if (ent.GetType().Equals(typeof(Wall)))
                {
                    foreach (var wall in walls)
                    {
                        if (wall.Equals(ent))
                        {
                            wall.Update(gameTime, deadEntities);
                        }
                    }
                }
            }
        }

        void KillDeadEntities(List<Entity> deadEntities)
        {
            foreach (var entity in deadEntities)
            {
                entities.Remove(entity);
                if (entity.GetType().Equals(typeof(Bullet)))
                {
                    Bullet bullet = null;
                    foreach (var b in bullets)
                    {
                        if (b.Equals(entity))
                        {
                            bullet = b;
                        }
                    }
                    bullets.Remove(bullet);
                }
                if (entity.GetType().Equals(typeof(Enemy)))
                {
                    Enemy enemy = null;
                    foreach (var e in enemies)
                    {
                        if (e.Equals(entity))
                        {
                            enemy = e;
                        }
                    }
                    enemies.Remove(enemy);
                }
                if (entity.GetType().Equals(typeof(Wall)))
                {
                    Wall wall = null;
                    foreach (var w in walls)
                    {
                        if (w.Equals(entity))
                        {
                            wall = w;
                        }
                    }
                    walls.Remove(wall);
                }
                if (entity.GetType().Equals(typeof(Sword)))
                {
                    Sword sword = null;
                    foreach (var s in swords)
                    {
                        if (s.Equals(entity))
                        {
                            sword = s;
                        }
                    }
                    swords.Remove(sword);
                }
            }
        }

        void SpawnEnemy(GameTime gameTime)
        {
            if (timeSinceEnemySpawn > enemySpawnRate && enemies.Count < 100)
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

        double MoveV5(Entity pusher, double movement, char xORy, List<(Entity, int)> pushedEntities = null, int parentId = -1, double mass = -1)
        {
            if(mass == -1)
            {
                mass = pusher.Mass;
            }
            bool firstime = false;
            if (pushedEntities == null)
            {
                pushedEntities = new List<(Entity, int)>() { (pusher, -1) };
                firstime = true;
            }
            else
            {
                for(int i = 0; i < pushedEntities.Count; i++)
                {
                    var tuple = pushedEntities[i];
                    if(tuple.Item1 == pusher)
                    {
                        if (xORy == 'x')
                        {
                            if (movement >= 0)
                            {
                                if (pushedEntities[parentId].Item1.X + pushedEntities[parentId].Item1.Size.X > pushedEntities[tuple.Item2].Item1.X + pushedEntities[tuple.Item2].Item1.Size.X)
                                {
                                    DestroyFamilyV3(pushedEntities, pushedEntities.IndexOf(tuple));
                                }
                                else
                                    return movement;
                            }
                            else
                            {
                                if (pushedEntities[parentId].Item1.X < pushedEntities[tuple.Item2].Item1.X)
                                {
                                    DestroyFamilyV3(pushedEntities, pushedEntities.IndexOf(tuple));
                                }
                                else
                                    return movement;
                            }
                        }
                        else
                        {
                            if (movement >= 0)
                            {
                                if (pushedEntities[parentId].Item1.Y + pushedEntities[parentId].Item1.Size.Y > pushedEntities[tuple.Item2].Item1.Y + pushedEntities[tuple.Item2].Item1.Size.Y)
                                {
                                    DestroyFamilyV3(pushedEntities, pushedEntities.IndexOf(tuple));
                                }
                                else
                                    return movement;
                            }
                            else
                            {

                                if (pushedEntities[parentId].Item1.Y < pushedEntities[tuple.Item2].Item1.Y)
                                {
                                    DestroyFamilyV3(pushedEntities, pushedEntities.IndexOf(tuple));
                                }
                                else
                                    return movement;
                            }
                        }
                    }
                }
                pushedEntities.Add((pusher, parentId));

            }
            int selfId = pushedEntities.Count - 1;

            if (xORy == 'x')
            {
                double oldX = pusher.X;
                foreach (var entity in entities)
                {
                    pusher.X = oldX;
                    pusher.X += movement;
                    pusher.Update();

                    if (pusher.Rect.Intersects(entity.Rect) && pusher != entity && entity.Collision && pusher.Collision)
                    {
                        if(mass > entity.Mass)
                        {
                            pusher.X = oldX;
                            pusher.Update();
                            pusher.Collision = false;
                            movement = MoveV5(entity, movement, xORy, pushedEntities, selfId, mass);
                            pusher.Collision = true;
                        }
                        else
                        {
                            if (movement >= 0)
                            {
                                movement = entity.Rect.Left - (oldX + pusher.Size.X);
                                if (movement < 0)
                                    movement = 0;
                            }
                            else
                            {
                                movement = entity.Rect.Right - (oldX);
                                if (movement > 0)
                                    movement = 0;
                            }
                        }
                    }
                }
                pusher.X = oldX;
                pusher.Update();
            }
            else
            {
                double oldY = pusher.Y;
                foreach (var entity in entities)
                {
                    pusher.Y = oldY;
                    pusher.Y += movement;
                    pusher.Update();

                    if (pusher.Rect.Intersects(entity.Rect) && pusher != entity && entity.Collision && pusher.Collision)
                    {
                        if (mass > entity.Mass)
                        {
                            pusher.Y = oldY;
                            pusher.Update();
                            pusher.Collision = false;
                            movement = MoveV5(entity, movement, xORy, pushedEntities, selfId, mass);
                            pusher.Collision = true;
                        }
                        else
                        {
                            if (movement >= 0)
                            {
                                movement = entity.Rect.Top - (oldY + pusher.Size.Y);
                                if (movement < 0)
                                    movement = 0;
                            }
                            else
                            {
                                movement = entity.Rect.Bottom - (oldY);
                                if (movement > 0)
                                    movement = 0;
                            }
                        }
                    }
                }
                pusher.Y = oldY;
                pusher.Update();
            }

            if (firstime)
            {

                var pushedEntitiesSorted = new List<(Entity, int)>();
                for (int i = 0; i < pushedEntities.Count; i++)
                {
                    if (pushedEntities[i].Item1 != null)
                    {
                        pushedEntitiesSorted.Add(pushedEntities[i]);
                    }

                }
                pushedEntitiesSorted = pushedEntitiesSorted.OrderBy(x => x.Item2).ToList();

                for(int i = pushedEntitiesSorted.Count/3; i > 0; i--)
                {
                    movement *= 0.7;
                }

                if (xORy == 'x')
                {
                    pusher.X += movement;
                }
                else
                    pusher.Y += movement;
                pusher.Update();

                for (int i = 0; i < pushedEntitiesSorted.Count; i++)
                {
                    var pushed = pushedEntitiesSorted[i];
                    //if (pushed.Item1 == null)
                    //    break;
                    if(pushed.Item1 != pusher && pushedEntities[pushed.Item2].Item1 != null && pushed.Item1 != null)
                    {
                        if (pushed.Item1.Rect.Intersects(pushedEntities[pushed.Item2].Item1.Rect))
                        {
                            if (xORy == 'x')
                            {
                                if(movement <= 0)
                                {
                                    pushed.Item1.X = pushedEntities[pushed.Item2].Item1.X - pushed.Item1.Size.X;
                                    pushed.Item1.Update();
                                }
                                else
                                {
                                    pushed.Item1.X = pushedEntities[pushed.Item2].Item1.X + pushedEntities[pushed.Item2].Item1.Size.X;
                                    pushed.Item1.Update();
                                }
                            
                            }
                            else
                            {
                                if (movement <= 0)
                                {
                                    pushed.Item1.Y = pushedEntities[pushed.Item2].Item1.Y - pushed.Item1.Size.Y;
                                    pushed.Item1.Update();
                                }
                                else
                                {
                                    pushed.Item1.Y = pushedEntities[pushed.Item2].Item1.Y + pushedEntities[pushed.Item2].Item1.Size.Y;
                                    pushed.Item1.Update();
                                }
                            }
                        }
                        else
                        {
                            pushedEntities[i] = (null, pushed.Item2);
                        }
                    }
                }
            }
            return movement;

        }

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
    }
}

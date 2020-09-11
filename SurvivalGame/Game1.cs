using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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
                //Move(1, gameTime, player, player.Speed);
                //MoveV3(player, 1, player.Speed * gameTime.ElapsedGameTime.TotalSeconds);
                MoveV4(player, player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
            }
            if (keyHistory.Contains(Keys.A.ToString()))
            {
                //Move(2, gameTime, player, player.Speed);
                //MoveV3(player, 2, player.Speed * gameTime.ElapsedGameTime.TotalSeconds);
                MoveV4(player, -player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true);
            }
            if (keyHistory.Contains(Keys.S.ToString()))
            {
                //Move(3, gameTime, player, player.Speed);
                //MoveV3(player, 3, player.Speed * gameTime.ElapsedGameTime.TotalSeconds);
                MoveV4(player, player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
            }
            if (keyHistory.Contains(Keys.W.ToString()))
            {
                //Move(4, gameTime, player, player.Speed);
                //MoveV3(player, 4, player.Speed * gameTime.ElapsedGameTime.TotalSeconds);
                MoveV4(player, -player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false);
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
                            MoveV4(enemy, enemy.XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);

                            MoveV4(enemy, enemy.YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);
                            //if (enemy.XMovement > 0)
                            //{
                            //    //Move(1, gameTime, enemy, enemy.XMovement);
                            //    //MoveV2(enemy, 1, enemy.XMovement * gameTime.ElapsedGameTime.TotalSeconds);
                            //    MoveV3(enemy, 1, enemy.XMovement * gameTime.ElapsedGameTime.TotalSeconds);
                            //}
                            //else
                            //{
                            //    //Move(2, gameTime, enemy, -enemy.XMovement);
                            //    //MoveV2(enemy, 2, -enemy.XMovement * gameTime.ElapsedGameTime.TotalSeconds);
                            //    MoveV3(enemy, 2, -enemy.XMovement * gameTime.ElapsedGameTime.TotalSeconds);
                            //}
                            //if (enemy.YMovement > 0)
                            //{
                            //    //Move(3, gameTime, enemy, enemy.YMovement);
                            //    //MoveV2(enemy, 3, enemy.YMovement * gameTime.ElapsedGameTime.TotalSeconds);

                            //    MoveV3(enemy, 3, enemy.YMovement * gameTime.ElapsedGameTime.TotalSeconds);
                            //}
                            //else
                            //{
                            //    //Move(4, gameTime, enemy, -enemy.YMovement); 
                            //    //MoveV2(enemy, 4, -enemy.YMovement * gameTime.ElapsedGameTime.TotalSeconds);

                            //    MoveV3(enemy, 4, -enemy.YMovement * gameTime.ElapsedGameTime.TotalSeconds);
                            //}
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

                            //if (bullet.XMovement > 0)
                            //{
                            //    MoveV3(bullet, 1, bullet.XMovement * gameTime.ElapsedGameTime.TotalSeconds);
                            //}
                            //else MoveV3(bullet, 2, -bullet.XMovement * gameTime.ElapsedGameTime.TotalSeconds);
                            //if (bullet.YMovement > 0)
                            //{
                            //    MoveV3(bullet, 3, bullet.YMovement * gameTime.ElapsedGameTime.TotalSeconds);
                            //}
                            //else MoveV3(bullet, 4, -bullet.YMovement * gameTime.ElapsedGameTime.TotalSeconds);
                            MoveV4(bullet, bullet.XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
                            MoveV4(bullet, bullet.YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);
                            bullet.Update(gameTime, deadEntities);
                            //foreach (var enemy in enemies)
                            //{
                            //    if (bullet.Detect(enemy))
                            //    {
                            //        enemy.Health--;
                            //        enemy.Size = new Point((int)(enemy.Size.X * 0.8), (int)(enemy.Size.Y * 0.8));
                            //        deadEntities.Add(bullet);
                            //    }
                            //}
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
            if (timeSinceEnemySpawn > enemySpawnRate)
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

        List<Tuple<List<double>, Entity>> MoveV3(Entity entity, int direction, double movement/* speed*time */, float? force = null, List<Tuple<List<double>, Entity>> v = null, bool firsTime = true, int? id = 0)
        {
            if (firsTime)
            {
                v = new List<Tuple<List<double>, Entity>>() { Tuple.Create(new List<double>() { 0, movement }, entity) };
            }
            else
            {
                v.Add(Tuple.Create(new List<double>() { (double)id, movement }, entity));
            }

            int ownID = v.Count - 1;
            for (int i = 0; i < v.Count; i++)
            {
                if (v[i].Item2 == entity && i != ownID)
                {
                    ownID = i;
                    break;
                }
            }
            List<double> coords = new List<double>();
            List<double> distances = new List<double>();
            List<double> maths = new List<double>();
            double old;
            if (force == null)
            {
                force = entity.Mass;
            }

            if (!firsTime && ownID != v.Count - 1)
            {
                var s = new Tuple<List<double>, Entity>(v[ownID].Item1, null);
                switch (direction)
                {
                    case 1:
                        if (v[(int)v[ownID].Item1[0]].Item2.Rect.Right > v[(int)id].Item2.Rect.Right - movement)
                        {
                            v[v.Count - 1] = s;
                            return v;
                        }
                        else
                        {
                            DestroyFamily(v, ownID);
                            v[ownID] = s;

                            ownID = v.Count - 1;
                        }
                        break;
                    case 2:
                        if (v[(int)v[ownID].Item1[0]].Item2.Rect.Left < v[(int)id].Item2.Rect.Left + movement)
                        {
                            v[v.Count - 1] = s;
                            return v;
                        }
                        else
                        {
                            DestroyFamily(v, ownID);
                            v[ownID] = s;

                            ownID = v.Count - 1;
                        }
                        break;
                    case 3:
                        if (v[(int)v[ownID].Item1[0]].Item2.Rect.Bottom > v[(int)id].Item2.Rect.Bottom - movement)
                        {
                            v[v.Count - 1] = s;
                            return v;
                        }
                        else
                        {
                            DestroyFamily(v, ownID);
                            v[ownID] = s;

                            ownID = v.Count - 1;
                        }
                        break;
                    case 4:
                        if (v[(int)v[ownID].Item1[0]].Item2.Rect.Top < v[(int)id].Item2.Rect.Top + movement)
                        {
                            v[v.Count - 1] = s;
                            return v;
                        }
                        else
                        {
                            DestroyFamily(v, ownID);
                            v[ownID] = s;

                            ownID = v.Count - 1;
                        }
                        break;
                }
            }
            switch (direction)
            {
                case 1:
                    old = entity.X;
                    entity.X += movement;
                    coords.Add(entity.X);
                    distances.Add(movement);
                    entity.Update();
                    foreach (var e in entities)
                    {
                        if (entity.Collision && e != entity && e.Collision && e.Rect.Intersects(entity.Rect))
                        {
                            if (force > e.Mass)
                            {
                                if (firsTime)
                                {
                                    entity.Collision = false;
                                    int l = v.Count - 1;
                                    double math = -(e.Rect.Left - entity.Rect.Right);
                                    maths.Add(math);
                                    if (math < 0)
                                        math = 0;
                                    v = (MoveV3(e, 1, math, 999, v, firsTime: false, 0));
                                    entity.Collision = true;
                                    for (int k = l; k < v.Count; k++)
                                    {
                                        double math2 = movement - math;
                                        if (math2 < 0) math2 = 0;
                                        distances.Add(v[k].Item1[1] + math2);
                                        coords.Add(Math.Floor(old + v[k].Item1[1] + math2));

                                    }
                                }
                                else
                                {
                                    entity.Collision = false;
                                    v = (MoveV3(e, 1, movement, 999, v, firsTime: false, ownID));
                                    entity.Collision = true;
                                }
                            }
                            else
                            {
                                coords.Add(e.Rect.Left - entity.Size.X);
                                distances.Add(e.Rect.Left - (old + entity.Size.X));
                            }
                        }
                    }
                    entity.X = old;
                    break;
                case 2:
                    old = entity.X;
                    entity.X -= movement;
                    coords.Add(entity.X);
                    distances.Add(movement);
                    entity.Update();
                    foreach (var e in entities)
                    {
                        if (entity.Collision && e != entity && e.Collision && e.Rect.Intersects(entity.Rect))
                        {
                            if (force > e.Mass)
                            {
                                if (firsTime)
                                {
                                    entity.Collision = false;
                                    int l = v.Count - 1;
                                    double math = -(entity.Rect.Left - e.Rect.Right);
                                    maths.Add(math);
                                    v = MoveV3(e, 2, math, 999, v, firsTime: false, 0);
                                    entity.Collision = true;
                                    for (int k = l; k < v.Count; k++)
                                    {
                                        double math2 = movement - math;
                                        if (math2 < 0) math2 = 0;
                                        distances.Add(v[k].Item1[1] + math2);
                                        coords.Add(Math.Ceiling(old - v[k].Item1[1] + math2));
                                    }
                                }
                                else
                                {
                                    entity.Collision = false;
                                    v = (MoveV3(e, 2, movement, 999, v, firsTime: false, ownID));
                                    entity.Collision = true;
                                }
                            }
                            else
                            {
                                coords.Add(e.Rect.Right);
                                distances.Add(old - e.Rect.Right);
                            }
                        }
                    }
                    entity.X = old;
                    break;
                case 3:
                    old = entity.Y;
                    entity.Y += movement;
                    coords.Add(entity.Y);
                    distances.Add(movement);
                    entity.Update();
                    foreach (var e in entities)
                    {
                        if (entity.Collision && e != entity && e.Collision && e.Rect.Intersects(entity.Rect))
                        {
                            if (force > e.Mass)
                            {
                                if (firsTime)
                                {
                                    int l = v.Count - 1;
                                    entity.Collision = false;
                                    double math = -(e.Rect.Top - entity.Rect.Bottom);
                                    maths.Add(math);
                                    if (math < 0) math = 0;
                                    v = (MoveV3(e, 3, math, 999, v, firsTime: false, 0));
                                    entity.Collision = true;
                                    for (int k = l; k < v.Count; k++)
                                    {
                                        double math2 = movement - math;
                                        if (math2 < 0) math2 = 0;
                                        distances.Add(v[k].Item1[1] + math2);
                                        coords.Add(old + v[k].Item1[1] + math2);

                                    }
                                }
                                else
                                {
                                    entity.Collision = false;
                                    v = (MoveV3(e, 3, movement, 999, v, firsTime: false, ownID));
                                    entity.Collision = true;
                                }
                            }
                            else
                            {
                                coords.Add(e.Rect.Top - entity.Size.Y);
                                distances.Add(e.Rect.Top - (old + entity.Size.Y));
                            }
                        }
                    }
                    entity.Y = old;
                    break;
                case 4:
                    old = entity.Y;
                    entity.Y -= movement;
                    coords.Add(entity.Y);
                    distances.Add(movement);
                    entity.Update();
                    foreach (var e in entities)
                    {
                        if (entity.Collision && e != entity && e.Collision && e.Rect.Intersects(entity.Rect))
                        {
                            if (force > e.Mass)
                            {
                                if (firsTime)
                                {
                                    int l = v.Count - 1;
                                    entity.Collision = false;
                                    double math = -(entity.Rect.Top - e.Rect.Bottom);
                                    maths.Add(math);
                                    if (math < 0) math = 0;
                                    v = MoveV3(e, 4, math, 999, v, firsTime: false, 0);
                                    entity.Collision = true;
                                    for (int k = l; k < v.Count; k++)
                                    {
                                        double math2 = movement - math;
                                        if (math2 < 0) math2 = 0;
                                        distances.Add(v[k].Item1[1] + math2);
                                        coords.Add(Math.Ceiling(old - v[k].Item1[1] + math2));

                                    }
                                }
                                else
                                {
                                    entity.Collision = false;
                                    v = (MoveV3(e, 4, movement, 999, v, firsTime: false, ownID));
                                    entity.Collision = true;
                                }
                            }
                            else
                            {
                                coords.Add(e.Rect.Bottom);
                                distances.Add(old - e.Rect.Bottom);
                            }
                        }
                    }
                    entity.Y = old;
                    break;
            }

            if (firsTime)
            {
                if (direction < 3)
                {
                      entity.X = coords[distances.IndexOf(distances.Min())];
                }
                else
                {
                      entity.Y = coords[distances.IndexOf(distances.Min())];
                }
                entity.Update();
                foreach (var i in v)
                {
                    if (i.Item2 != null && i.Item2 != entity)
                    {
                        switch (direction)
                        {
                            case 1:
                                if (v[(int)i.Item1[0]].Item2 != null)
                                {
                                    i.Item2.X = v[(int)i.Item1[0]].Item2.Rect.Right;
                                    i.Item2.Update();
                                }
                                break;
                            case 2:
                                if (v[(int)i.Item1[0]].Item2 != null)
                                {
                                    i.Item2.X = v[(int)i.Item1[0]].Item2.Rect.Left - i.Item2.Size.X;
                                    i.Item2.Update();
                                }
                                break;
                            case 3:
                                if (v[(int)i.Item1[0]].Item2 != null)
                                {
                                    i.Item2.Y = v[(int)i.Item1[0]].Item2.Rect.Bottom;
                                    i.Item2.Update();
                                }
                                break;
                            case 4:
                                if (v[(int)i.Item1[0]].Item2 != null)
                                {
                                    i.Item2.Y = v[(int)i.Item1[0]].Item2.Rect.Top - i.Item2.Size.Y;
                                    i.Item2.Update();
                                }
                                break;
                        }
                    }
                }
                return null;
            }
            else
            {
                entity.Update();
                v[ownID] = Tuple.Create(new List<double>() { (double)id, distances.Min() }, entity);
                return v;
            }
        }
        double findID(List<Tuple<List<double>, Entity>> v, int id)
        {
            if (v[id].Item2 == null)
            {
                return findID(v, (int)v[id].Item1[0]);
            }
            else
                return id;
        }
        void DestroyFamily(List<Tuple<List<double>, Entity>> v, int id)
        {
            for (int i = 0; i < v.Count; i++)
            {
                if (v[i].Item1[0] == id)
                {
                    DestroyFamily(v, i);
                    //v.Remove(v[i]);
                    v[i] = new Tuple<List<double>, Entity>(v[id].Item1, null);
                }
            }
            return;
        }
        void DestroyFamilyV2(List<(Entity, (int, double))> family, int id)
        {
            for (int i = 0; i < family.Count; i++)
            {
                if (family[i].Item2.Item1 == id)
                {
                    DestroyFamilyV2(family, i);
                    //v.Remove(v[i]);
                    family[i] = (null, family[i].Item2);
                }
            }
            family[id] = (null, family[id].Item2);
            return;
        }
        (double, float) MoveV4(Entity entity, double movement, bool xAxis, List<(Entity, (int, double))> family = null, bool firstTime = true, int parentId = 0, float mass = 0f)
        {
            if(family == null)
            {
                family = new List<(Entity, (int, double))>();
            }
            double distanceFromParent = 0;
            if (!firstTime)
            {
                if (xAxis)
                {
                    if (movement > 0)
                        distanceFromParent = entity.X - (family[parentId].Item1.X + family[parentId].Item1.Size.X/* - movement*/);
                    else
                        distanceFromParent = entity.X + entity.Size.X - (family[parentId].Item1.X/* - movement*/);
                }
                else
                {
                    if (movement > 0)
                        distanceFromParent = entity.Y - (family[parentId].Item1.Y + family[parentId].Item1.Size.Y/* - movement*/);
                    else
                        distanceFromParent = entity.Y + entity.Size.Y - (family[parentId].Item1.Y/* - movement*/);
                }
                if(distanceFromParent != 0)
                    Console.WriteLine(distanceFromParent);
            }
            (Entity, (int, double)) member = (entity, (parentId, distanceFromParent));
            for (int i = 0; i < family.Count; i++)
            {
                if(family[i].Item1 == entity)
                {
                    if (xAxis)
                    {
                        if (movement > 0)
                        {
                            //if (entity.X > family[i].Item1.X)
                            if (family[family[i].Item2.Item1].Item1.X + family[family[i].Item2.Item1].Item1.Size.X < family[parentId].Item1.X + family[parentId].Item1.Size.X)
                                DestroyFamilyV2(family, i);
                            else
                                return (movement, mass);
                        }
                        else
                        {
                            //if (entity.X < family[i].Item1.X)
                            if (family[family[i].Item2.Item1].Item1.X > family[parentId].Item1.X)
                                DestroyFamilyV2(family, i);
                            else
                                return (movement, mass);
                        }
                    }
                    else
                    {
                        if (movement > 0)
                        {
                            //if (entity.Y > family[i].Item1.Y)
                            if (family[family[i].Item2.Item1].Item1.Y + family[family[i].Item2.Item1].Item1.Size.Y < family[parentId].Item1.Y + family[parentId].Item1.Size.Y)
                                DestroyFamilyV2(family, i);
                            else
                                return (movement, mass);
                        }
                        else
                        {
                            //if (entity.Y < family[i].Item1.Y)
                            if (family[family[i].Item2.Item1].Item1.Y > family[parentId].Item1.Y)
                                DestroyFamilyV2(family, i);
                            else
                                return (movement, mass);
                        }
                    }
                }
            }
            //me, parentId, distanceTillParent
            family.Add(member);
            if (mass == 0)
            {
                mass = entity.Mass;
            }
            else
            {
                ////var old = movement;
                //movement += -(movement * (entity.Mass / (mass + entity.Mass)));
                ////movement *= entity.Mass / (mass + entity.Mass);
                //mass -= entity.Mass;
                ////if (old < 0 && movement > 0 || old > 0 && movement < 0)
                //if (mass < entity.Mass)
                //    return (movement, mass);
                //mass -= entity.Mass;
                //float minMovement = 0.1f;
                //if (mass < 0)
                //{
                //    mass = 0;
                //}
                //if(mass / (mass + entity.Mass) > minMovement)
                //{
                //    movement *= mass / (mass + entity.Mass);
                //}
                //else
                //  movement *= minMovement;

                double old = movement;
                movement *= mass / (mass + entity.Mass);
                //if(Math.Abs(movement) < 0.1 && old > 0)
                //{
                //    movement = 0.1;
                //}


            }
            int ownId = family.Count - 1;
            //List<>
            if (xAxis)
            {
                double old = entity.X;
                entity.X += movement;
                entity.Update();
                if (entity.Collision)
                {
                    foreach (var e in entities)
                    {
                        //e.Update();
                        if (e != entity && e.Collision && e.Rect.Intersects(entity.Rect))
                        {
                            if(mass > e.Mass)
                            {
                                entity.Collision = false;
                                entity.X = old;
                                entity.Update();
                                e.X = Math.Floor(e.X) + Math.Abs(entity.X % 1);
                                var tuple = MoveV4(e, movement, xAxis/*true*/, family, false, ownId, mass);
                                movement = tuple.Item1;
                                mass = tuple.Item2;
                                entity.X += movement;
                                entity.Update();
                                entity.Collision = true;
                            }
                            else
                            {
                                if(movement > 0)
                                {
                                    //if(movement - (entity.X + entity.Size.X - e.X) < movement)
                                    //  movement -= entity.X + entity.Size.X - e.X;
                                    //if(e.Rect.X - (old + entity.Size.X) < movement)
                                    //  movement = e.Rect.X - (old + entity.Size.X);

                                    if (e.X - (old + entity.Size.X) < movement)
                                        movement = e.X - (old + entity.Size.X);
                                    if (movement < 0)
                                        movement = 0;
                                }
                                else if(movement < 0)
                                {
                                    //if(movement + (e.X + e.Size.X - entity.X) > movement)
                                    //    movement += e.X + e.Size.X - entity.X;
                                    //if(old - (e.Rect.X + e.Size.X) > movement)
                                    //  movement = old - (e.Rect.X + e.Size.X);

                                    if ((e.X + e.Size.X) - old > movement)
                                        movement = (e.X + e.Size.X) - old;
                                    if (movement > 0)
                                        movement = 0;
                                }
                            }
                        }
                    }
                }
                entity.X = old;
            }
            else
            {
                double old = entity.Y;
                entity.Y += movement;
                entity.Update();
                if (entity.Collision)
                {
                    foreach (var e in entities)
                    {
                        //e.Update();
                        if (e != entity && e.Collision && e.Rect.Intersects(entity.Rect))
                        {
                            if (mass > e.Mass)
                            {
                                entity.Collision = false;
                                entity.Y = old;
                                entity.Update();
                                e.Y = Math.Floor(e.Y) + Math.Abs(entity.Y % 1);
                                var tuple = MoveV4(e, movement, xAxis/*false*/, family, false, ownId, mass);
                                movement = tuple.Item1;
                                mass = tuple.Item2;
                                entity.Y += movement;
                                entity.Update();
                                entity.Collision = true;
                            }
                            else
                            {
                                if (movement > 0)
                                {
                                    //if (movement - (entity.Y + entity.Size.Y - e.Y) < movement)
                                    //    movement -= entity.Y + entity.Size.Y - e.Y;
                                    //if (e.Rect.Y - (old + entity.Size.Y) < movement)
                                    //movement = e.Rect.Y - (old + entity.Size.Y);
                                    if (e.Y - (old + entity.Size.Y) < movement)
                                        movement = e.Y - (old + entity.Size.Y);
                                    if (movement < 0)
                                        movement = 0;
                                }
                                else if (movement < 0)
                                {
                                    //if (movement + (e.Y + e.Size.Y - entity.Y) > movement)
                                    //    movement += e.Y + e.Size.Y - entity.Y;
                                    //if (old - (e.Y + e.Size.Y) > movement)
                                    //movement = old - (e.Y + e.Size.Y);
                                    if ((e.Y + e.Size.Y) - old > movement)
                                        movement = (e.Y + e.Size.Y) - old;
                                    if (movement > 0)
                                        movement = 0;
                                }
                            }
                        }
                    }
                }
                entity.Y = old;
            }

            if (firstTime && movement != 0)
            {
                if (xAxis)
                    entity.X += movement;
                else
                    entity.Y += movement;
                entity.Update();

                for (int i = 1; i < family.Count; i++)
                {
                    if (family[i].Item1 != null && family[i].Item2.Item2 <= Math.Abs(movement))
                    {
                    //    bool closeEnough = false;
                    //    if (movement > 0 && family[i].Item2.Item2 <= movement)
                    //        closeEnough = true;
                    //    else if (movement < 0 && family[i].Item2.Item2 >= movement)

                        if (xAxis)
                        {
                            if (movement > 0)
                                family[i].Item1.X = family[family[i].Item2.Item1].Item1.X + family[family[i].Item2.Item1].Item1.Size.X;
                            else
                                family[i].Item1.X = family[family[i].Item2.Item1].Item1.X - family[i].Item1.Size.X;
                        }
                        else
                        {
                            if (movement > 0)
                                family[i].Item1.Y = family[family[i].Item2.Item1].Item1.Y + family[family[i].Item2.Item1].Item1.Size.Y;
                            else
                                family[i].Item1.Y = family[family[i].Item2.Item1].Item1.Y - family[i].Item1.Size.Y;
                        }
                        family[i].Item1.Update();
                    }
                }
            }
            return (movement, mass);

        }
    }
}

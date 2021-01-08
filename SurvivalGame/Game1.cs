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

            textures.Add("Circle", this.Content.Load<Texture2D>("Circle"));

            player = new Player(textures["Player"]);
            entities.Add(player);
            //for(int i = 0; i < 10; i++)
            //{
            //    enemies.Add(new Enemy(textures["Enemy"], rand.Next(0, 500), rand.Next(0, 500)));
            //}
            mouseCursor = new MouseCursor(textures["MouseCursor"]);
            entities.Add(mouseCursor);

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

            spriteBatch.Begin(SpriteSortMode.BackToFront);

            foreach(var entity in entities)
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
                player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true, entities);
            }
            else if (keyHistory.Contains(Keys.A.ToString()))
            {
                player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, true, entities);
            }
            if (keyHistory.Contains(Keys.S.ToString()))
            {
                player.Move(player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false, entities);
            }
            else if (keyHistory.Contains(Keys.W.ToString()))
            {
                player.Move(-player.Speed * gameTime.ElapsedGameTime.TotalSeconds, false, entities);
            }
            timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyHistory.Contains(Keys.Space.ToString()))
            {
                if (bulletType == 0 && timeSinceLastShot > rateOfFire)
                {

                    Projectile projectile = new Projectile(textures["Bullet"], 500f, new Vector2(player.X, player.Y), new Vector2(mouseCursor.X, mouseCursor.Y), 100);
                    projectiles.Add(projectile);
                    entities.Add(projectile);

                    timeSinceLastShot = 0f;
                }
                else if (bulletType == 1 && timeSinceLastShot > rateOfFire)
                {
                    Projectile projectile = new Projectile(textures["Flame"], 500f, new Vector2(player.X, player.Y), new Vector2(mouseCursor.X, mouseCursor.Y), 200);
                    projectiles.Add(projectile);
                    entities.Add(projectile);

                    timeSinceLastShot = 0f;
                }
            }
            timeSinceSwordAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyHistory.Contains(Keys.V.ToString()) && timeSinceSwordAttack > swordCooldown)
            {
                timeSinceSwordAttack = 0f;
                double yEdge = (player.Y - mouseCursor.Y);
                double xEdge = (player.X - mouseCursor.X);
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
                    enemy.Update();
                    enemy.Movement(enemy.Hitbox.X - player.Hitbox.X, enemy.Hitbox.Y - player.Hitbox.Y);
                    //MoveV5(enemy, enemy.XMovement * gameTime.ElapsedGameTime.TotalSeconds, 'x');

                    // MoveV5(enemy, enemy.YMovement * gameTime.ElapsedGameTime.TotalSeconds, 'y');

                    enemy.Move(enemy.XMovement * gameTime.ElapsedGameTime.TotalSeconds, true, entities);
                    enemy.Move(enemy.YMovement * gameTime.ElapsedGameTime.TotalSeconds, false, entities);

                    foreach (var projectile in projectiles)
                    {
                        if (enemy.Hitbox.CollisionDetect(projectile.Hitbox) != Vector2.Zero)
                        {
                            enemy.DamageEntity(projectile.Damage, "Projectile");
                            projectile.isDead = true;
                        }
                    }
                    foreach (var sword in swords)
                    {
                        if(sword.Hitbox.CollisionDetect(enemy.Hitbox) != Vector2.Zero && !sword.hitEntities.Contains(enemy))
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
                    projectile.Move(projectile.XMovement * gameTime.ElapsedGameTime.TotalSeconds, true, entities);
                    projectile.Move(projectile.YMovement * gameTime.ElapsedGameTime.TotalSeconds, false, entities);
                    projectile.Update(gameTime);
                    foreach (var wall in walls)
                    {
                        if (projectile.Hitbox.CollisionDetect(wall.Hitbox) != Vector2.Zero && wall.Collision)
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
            if (timeSinceEnemySpawn > enemySpawnRate && enemies.Count < 100 )
            {
                int i = 0;
                while (i < 10)
                {
                    Enemy enemy;
                    bool suitableSpot = true;
                    if(rand.Next(2) == 1)
                        enemy = new Enemy(textures["Circle"], rand.Next(0, 500), rand.Next(0, 500), rand.Next(15, 25));
                    else
                        enemy = new Enemy(textures["Enemy"], rand.Next(0, 500), rand.Next(0, 500), rand.Next(15, 25), rand.Next(25, 35));

                    enemy.Update();
                    foreach (var entity in entities)
                    {
                        if (enemy != entity && enemy.Hitbox.CollisionDetect(entity.Hitbox) != Vector2.Zero)
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
            bool success = false;
            if (timeSinceWallPlacement > wallPlacementCooldown)
            {
                var wall = new Wall(textures["Wall"], mstate.X, mstate.Y);
                wall.Update();
                bool suitableSpot = true;
                foreach (var w in entities)
                {
                    if (w is MouseCursor)
                        continue;
                    if (wall.Hitbox.CollisionDetect(w.Hitbox) != Vector2.Zero)
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
                if (!w.Collision && wallGhost.Hitbox.CollisionDetect(w.Hitbox) != Vector2.Zero)
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
                if (w.Collision && w.Hitbox.CollisionDetect(mouseCursor.Hitbox) != Vector2.Zero)
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
    }
}

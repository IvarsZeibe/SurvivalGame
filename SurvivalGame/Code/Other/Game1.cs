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
        //GraphicsDeviceManager graphics;
        //SpriteBatch spriteBatch;

        Player player { get => EntityTracker.GetEntity<Player>(); }
        Chat chat;
        DefaultLevels levels;
        Input input;

        const float RESPAWN_COOLDOWN = 2000f;
        float timeTillRespawn = RESPAWN_COOLDOWN;



        public Game1()
        {
            Globals.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Globals.graphics.PreferredBackBufferWidth = 1280;
            Globals.graphics.PreferredBackBufferHeight = 800;
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

            //player = EntityTracker.Add.Player();
            var room = new Room((0, 0), "Spawn", new Color(0, 220, 0), TextureName.GrassyBackground);
            Globals.MouseCursor = new MouseCursor();
            Globals.HUD = new HUD();
            new Player();
            chat = new Chat(Globals.graphics);
            Globals.shop = new Shop();
            Globals.Command = new Command(this);
            Globals.MainMenu = new MainMenu();
            input = new Input(this, player, chat);
            Globals.Map = new Map();
            //room.Entities.Add(new Enemy(
            //        TextureName.Rectangle, 
            //        Globals.rand.Next(0, Globals.graphics.PreferredBackBufferWidth),
            //        Globals.rand.Next(0, Globals.graphics.PreferredBackBufferHeight), height: 60, speed: 0,
            //        target: player, color: Color.DarkSlateGray, addToRoom: false));
            Globals.lightMap = new LightMap();
            Globals.HUD.hotbar.Add(new SwordItem());
            Globals.HUD.hotbar.Add(new Pistol());
            Globals.HUD.hotbar.Add(new FlamethrowerItem());
            Globals.HUD.hotbar.Add(new AxeItem());
            //Globals.shop.AddItemForSale(new Pistol(50, 1.5f, "sniper", bulletVelocity: 1500f), 3); 
            Globals.shop.AddItemForSale(new Pistol(), 3);
            Globals.shop.AddItemForSale(new Pistol(10, 0.1f, "mini"), 5);
            Globals.shop.AddItemForSale(new SwordItem(40, knockbackStrenght: 5), 3);
            Globals.shop.AddItemForSale(new RPG(_trackEnemy: false), 5);
            Globals.shop.AddItemForSale(new RPG(), 15);
            Globals.shop.AddItemForSale(new Shotgun(), 15);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here

            void addTexture(string fileName)
            {
                Globals.Textures.Add(fileName, Content.Load<Texture2D>(fileName));
            }
            Globals.Textures.Add(TextureName.Rectangle.ToString(), Utilities.CreateTexture(Color.White, GraphicsDevice));
            addTexture("Circle");
            addTexture("PistolItem");
            addTexture("SwordItem");
            addTexture("RPG");
            addTexture("GrassyBackground");
            addTexture("Sparkles");
            addTexture("PineTree");
            addTexture("PineTreeOnFire");
            addTexture("AxeItem");
            addTexture("Axe");
            addTexture("fire");
            addTexture("grass");
            addTexture("stone");
            addTexture("light");
            addTexture("light2");
            Globals.SpriteFonts.Add(SpriteFontName.Aerial16, this.Content.Load<SpriteFont>("Chat"));
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
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();
            //mstate = Mouse.GetState();
            // TODO: Add your update logic here

            input.Update(gameTime);
            //UpdateKeys();
            //OnKeyDown(gameTime);
            if (!Globals.MainMenu.IsActive)
            {
                //if(EntityTracker.GetEntities<Player>().Count > 0)
                //    player = EntityTracker.GetEntities<Player>()[0];
                if(player != null)
                    CheckForRoomChange();
                Globals.Command.DoCommand(this);
                Globals.HUD.Update(gameTime);
                //levels.Update(gameTime);
                Globals.Rooms[Globals.activeRoomCoords].Update(gameTime);
                EntityTracker.UpdateEntities(gameTime);
                UpdateUpdatables(gameTime);
                TryToRespawnPlayer(gameTime);
        }
            Globals.MouseCursor.Update(gameTime);



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Globals.Rooms[Globals.activeRoomCoords].BackgroundColor);
            // TODO: Add your drawing code here

            Globals.lightMap.Update(Globals.spriteBatch, gameTime);
            Globals.spriteBatch.Begin(SpriteSortMode.BackToFront);

            Globals.lightMap.Draw(Globals.spriteBatch);
            foreach (var drawing in Globals.Drawings)
            {
                if (drawing.IsDrawn)
                {
                    if (drawing.TextureStr == "none")
                        Globals.spriteBatch.Draw(Globals.Textures[drawing.Texture.ToString()], drawing.Position, null, drawing.Color, drawing.Rotation, drawing.Origin, drawing.Scale, SpriteEffects.None, drawing.LayerDepth);
                    else
                    {
                        var i = Globals.Textures[drawing.TextureStr];
                        Globals.spriteBatch.Draw(Globals.Textures[drawing.TextureStr], drawing.Position, null, drawing.Color, drawing.Rotation, drawing.Origin, drawing.Scale, SpriteEffects.None, drawing.LayerDepth);
                    }
                }
            }
            foreach (var drawingText in Globals.DrawingTexts)
            {
                if (drawingText.IsDrawn)
                    Globals.spriteBatch.DrawString(Globals.SpriteFonts[drawingText.SpriteFont], drawingText.Text, drawingText.Position, drawingText.Color, drawingText.Rotation, Vector2.Zero, drawingText.Scale, SpriteEffects.None, drawingText.LayerDepth);
            }

            Globals.spriteBatch.End();
            base.Draw(gameTime);
        }


        void UpdateUpdatables(GameTime gameTime)
        {
            Globals.Updatables = Globals.Updatables.Where(o => !o.IsDead).ToList();
            for (int i = 0; i < Globals.Updatables.Count; i++)
            {
                if (Globals.Updatables[i].UpdateEnabled)
                    Globals.Updatables[i].Update(gameTime);
            }
        }

        void TryToRespawnPlayer(GameTime gameTime)
        {
            if (player is null)
            {
                if (timeTillRespawn < 0)
                {
                    new Player();
                    timeTillRespawn = RESPAWN_COOLDOWN;
                }
                else
                {
                    timeTillRespawn -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
        }

        void CheckForRoomChange()
        {
            if (Globals.Rooms[Globals.activeRoomCoords].CanLeave)
            {
                var player = this.player;
                Vector2 oldPlayerPos;
                var oldRoom = Globals.Rooms[Globals.activeRoomCoords];
                (int x, int y) newRoomCoords;
                if (player.Y < 0)
                {
                    player.Y = Globals.graphics.PreferredBackBufferHeight;
                    newRoomCoords = (Globals.activeRoomCoords.x, Globals.activeRoomCoords.y + 1);
                    oldPlayerPos = new Vector2(player.X, 0);
                }
                else if (player.Y > Globals.graphics.PreferredBackBufferHeight)
                {
                    player.Y = 0;
                    newRoomCoords = (Globals.activeRoomCoords.x, Globals.activeRoomCoords.y - 1);
                    oldPlayerPos = new Vector2(player.X, Globals.graphics.PreferredBackBufferHeight);
                }
                else if (player.X > Globals.graphics.PreferredBackBufferWidth)
                {
                    player.X = 0;
                    newRoomCoords = (Globals.activeRoomCoords.x + 1, Globals.activeRoomCoords.y);
                    oldPlayerPos = new Vector2(Globals.graphics.PreferredBackBufferWidth, player.Y);
                }
                else if (player.X < 0)
                {
                    player.X = Globals.graphics.PreferredBackBufferWidth;
                    newRoomCoords = (Globals.activeRoomCoords.x - 1, Globals.activeRoomCoords.y);
                    oldPlayerPos = new Vector2(0, player.Y);
                }
                else
                    return;

                if (!Globals.Rooms.ContainsKey(newRoomCoords))
                    if (Math.Abs(newRoomCoords.x) + Math.Abs(newRoomCoords.y) <= 10)
                    {
                        switch (Globals.rand.Next(0,8))
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                RoomMaker.SlimeRoom(newRoomCoords);
                                break;
                            case 4:
                            case 5:
                            case 6:
                                RoomMaker.ShooterRoom(newRoomCoords);
                                break;
                            case 7:
                                RoomMaker.BossRoom(newRoomCoords);
                                break;
                            default:
                                RoomMaker.RandomEmptyRoom(newRoomCoords);
                                break;
                        }
                    }
                    else
                    {
                        RoomMaker.RandomEmptyRoom(newRoomCoords);
                    }
                Globals.activeRoomCoords = newRoomCoords;

                oldRoom.Entities.Remove(player);
                oldRoom.Entities.Remove(Globals.MouseCursor);
                Globals.Rooms[Globals.activeRoomCoords].Load();
                Globals.Rooms[Globals.activeRoomCoords].Entities.Add(player);
                Globals.Rooms[Globals.activeRoomCoords].Entities.Add(Globals.MouseCursor);
                oldRoom.UnLoad();
                bool collisionDetected = false;
                foreach (var entity in EntityTracker.Entities)
                {
                    if (entity.CollidesWith(player) && entity != player)
                    {
                        collisionDetected = true;
                        break;
                    }
                }
                if (collisionDetected)
                {
                    var generatedRoom = Globals.Rooms[Globals.activeRoomCoords];
                    Globals.activeRoomCoords = oldRoom.Coords;

                    player.X = oldPlayerPos.X;
                    player.Y = oldPlayerPos.Y;
                    generatedRoom.Entities.Remove(player);
                    generatedRoom.Entities.Remove(Globals.MouseCursor);
                    generatedRoom.UnLoad();
                    oldRoom.Load();
                    oldRoom.Entities.Add(player);
                    oldRoom.Entities.Add(Globals.MouseCursor);
                }

                Globals.Map.Update();
            }
            else
            {
                if (player.Y < 0)
                {
                    player.Y = 0;
                }
                else if (player.Y > Globals.graphics.PreferredBackBufferHeight)
                {
                    player.Y = Globals.graphics.PreferredBackBufferHeight;
                }
                else if (player.X > Globals.graphics.PreferredBackBufferWidth)
                {
                    player.X = Globals.graphics.PreferredBackBufferWidth;
                }
                else if (player.X < 0)
                {
                    player.X = 0;
                }
            }
        }
    }

}

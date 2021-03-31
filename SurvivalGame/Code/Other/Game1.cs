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

        Player player;
        Chat chat;
        DefaultLevels levels;
        Input input;

        float respawnCooldown = 10f;
        float timeTillRespawn = -100f;



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
            Globals.SpriteFonts.Add(SpriteFontName.Aerial16, this.Content.Load<SpriteFont>("Chat"));

            //player = EntityTracker.Add.Player();
            var room = new Room((0, 0), "Spawn", new Color(0, 220, 0), TextureName.GrassyBackground);
            Globals.MouseCursor = new MouseCursor();
            Globals.HUD = new HUD();
            player = new Player();
            chat = new Chat(Globals.graphics);
            Globals.shop = new Shop();
            Globals.Command = new Command(this);
            //levels = new DefaultLevels(this);
            Globals.MainMenu = new MainMenu();
            input = new Input(this, player, chat);
            Globals.Map = new Map();

            Globals.HUD.hotbar.Add(new SwordItem());
            Globals.shop.AddItemForSale(new Pistol(50, 1.5f, "sniper", bulletVelocity: 1500f), 3);
            Globals.shop.AddItemForSale(new Pistol(10, 0.1f, "mini"), 5);
            Globals.shop.AddItemForSale(new SwordItem(40, knockbackStrenght: 5), 3);
            Globals.shop.AddItemForSale(new RPG(_trackEnemy: false), 5);
            Globals.shop.AddItemForSale(new RPG(), 15);
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
                Globals.Command.DoCommand(this);
                Globals.HUD.Update(gameTime);
                //levels.Update(gameTime);
                Globals.Rooms[Globals.activeRoomCoords].Update(gameTime);
                EntityTracker.UpdateEntities(gameTime);
                UpdateUpdatables(gameTime);
                TryToRespawnPlayer(gameTime);
                CheckForRoomChange();

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

            Globals.spriteBatch.Begin(SpriteSortMode.BackToFront);

            foreach (var drawing in Globals.Drawings)
            {
                if (drawing.IsDrawn)
                {
                    Globals.spriteBatch.Draw(Globals.Textures[drawing.Texture.ToString()], drawing.Position, null, drawing.Color, drawing.Rotation, Vector2.Zero, drawing.Scale, SpriteEffects.None, drawing.LayerDepth);
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
            if (player.IsDead)
            {
                if (timeTillRespawn <= -100)
                {
                    timeTillRespawn = respawnCooldown;
                }
                else if (timeTillRespawn < 0)
                {
                    player = new Player();
                    timeTillRespawn = -100;
                }
                else
                {
                    timeTillRespawn -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }

        void CheckForRoomChange()
        {
            if (Globals.Rooms[Globals.activeRoomCoords].LevelsActive)
            {
                var oldRoom = Globals.Rooms[Globals.activeRoomCoords];
                (int x, int y) newRoomCoords;
                if (player.Y < 0)
                {
                    player.Y = Globals.graphics.PreferredBackBufferHeight;
                    newRoomCoords = (Globals.activeRoomCoords.x, Globals.activeRoomCoords.y + 1);
                }
                else if (player.Y > Globals.graphics.PreferredBackBufferHeight)
                {
                    player.Y = 0;
                    newRoomCoords = (Globals.activeRoomCoords.x, Globals.activeRoomCoords.y - 1);
                }
                else if (player.X > Globals.graphics.PreferredBackBufferWidth)
                {
                    player.X = 0;
                    newRoomCoords = (Globals.activeRoomCoords.x + 1, Globals.activeRoomCoords.y);
                }
                else if (player.X < 0)
                {
                    player.X = Globals.graphics.PreferredBackBufferWidth;
                    newRoomCoords = (Globals.activeRoomCoords.x - 1, Globals.activeRoomCoords.y);
                }
                else
                    return;

                if (!Globals.Rooms.ContainsKey(newRoomCoords))
                    if (Math.Abs(newRoomCoords.x) + Math.Abs(newRoomCoords.y) <= 10)
                    {
                        switch (Globals.rand.Next(0,3))
                        {
                            case 0:
                                RoomMaker.SlimeRoom(newRoomCoords);
                                break;
                            case 1:
                                RoomMaker.ShooterRoom(newRoomCoords);
                                break;
                            case 2:
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

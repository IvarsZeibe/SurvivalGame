using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Map
    {
        public bool IsActive = false;
        public Hitbox Hitbox;

        public bool BeingDragged = false;
        Vector2 totalOffsetFromCenter = new Vector2(0, 0);

        RenderTarget2D renderTarget2D;

        Vector2 TileSize = new Vector2(50, 50);
        List<List<Tile>> Tiles = new List<List<Tile>>();

        Drawing drawing;
        List<Drawing> border;

        public Map()
        {
            Hitbox = new Rect(0, 0, 200, 200);
            Hitbox.Right = Globals.graphics.PreferredBackBufferWidth - 5;
            Hitbox.Top = 5;

            renderTarget2D = new RenderTarget2D(Globals.graphics.GraphicsDevice, Hitbox.Width, Hitbox.Height);
            Update();
            drawing = new Drawing(TextureName.Map, Hitbox.GetTopLeftPosVector(), Color.White, 0f, new Vector2(0,0), 0.05f, false);
            border = Utilities.CreateEmptyRectDrawings(drawing, Color.Black, 3, false);
        }
        public void Update()
        {
            (int x, int y) centerRoom = GetCenterRoom();
            Vector2 tileOffset = GetOffset();


            Tiles.Clear();
            float widthDivision = (float)Hitbox.Width / TileSize.X;
            int extraWidth = 2;
            if (widthDivision % 1 == 0)
                widthDivision -= 0.01f;
            if (Math.Ceiling(widthDivision) % 2 == 0)
                extraWidth++;
            widthDivision += extraWidth;
            int extraHeight = 2;
            float heightDivision = (float)Hitbox.Height / TileSize.Y;
            if (heightDivision % 1 == 0)
                heightDivision -= 0.01f;
            if (Math.Ceiling(heightDivision) % 2 == 0)
                extraHeight++;
            heightDivision += extraHeight;
            for (int y = 0; y < Math.Ceiling(heightDivision); y++)
            {
                Tiles.Add(new List<Tile>());
                for (int x = 0; x < Math.Ceiling(widthDivision); x++)
                {
                    Tile tile;
                    Vector2 position = new Vector2(
                        (x * TileSize.X) - ((1 + extraWidth - widthDivision % 1) * TileSize.X) / 2,
                        (y * TileSize.Y) - ((1 + extraHeight - heightDivision % 1) * TileSize.Y) / 2) + tileOffset;
                    (int, int) roomCoords = 
                        (centerRoom.x + (x - (int)Math.Floor(Math.Ceiling(widthDivision) / 2)),
                        centerRoom.y - (y - (int)Math.Floor(Math.Ceiling(heightDivision) / 2)));
                    if (Globals.Rooms.ContainsKey(roomCoords))
                    {
                        var room = Globals.Rooms[(roomCoords)];
                        tile = new Tile(position, TileSize, room.background.Texture, room.background.Color);
                    }
                    else
                        tile = new Tile(position, TileSize);
                    Tiles[y].Add(tile);
                }
            }
            Draw();
        }
        void Draw()
        {
            Globals.graphics.GraphicsDevice.SetRenderTarget(renderTarget2D);
            Globals.graphics.GraphicsDevice.Clear(Color.White);
            Globals.spriteBatch.Begin();
            foreach (var tileLine in Tiles)
            {
                foreach (var tile in tileLine)
                {
                    Vector2 size = tile.Size / new Vector2(Globals.Textures[tile.Texture.ToString()].Width, Globals.Textures[tile.Texture.ToString()].Height);
                    Globals.spriteBatch.Draw(Globals.Textures[tile.Texture.ToString()], tile.Coord, null, tile.Color, 0f, Vector2.Zero, size, SpriteEffects.None, 1f);
                }
            }
            Globals.spriteBatch.End();
            Globals.Textures["Map"] = renderTarget2D;
            Globals.graphics.GraphicsDevice.SetRenderTarget(null);
        }
        public void Open()
        {
            totalOffsetFromCenter = Vector2.Zero;
            Update();
            drawing.IsDrawn = true;
            foreach(var drawing in border)
            {
                drawing.IsDrawn = true;
            }
            IsActive = true;
        }
        public void Close()
        {
            drawing.IsDrawn = false;
            foreach (var drawing in border)
            {
                drawing.IsDrawn = false;
            }
            IsActive = false;
            BeingDragged = false;
        }
        public void Zoom(int scrollWheel)
        {
            if (IsActive)
            {
                scrollWheel /= 30;
                TileSize += new Vector2(scrollWheel, scrollWheel);
                if (TileSize.X < 0 || TileSize.Y < 0)
                    TileSize -= new Vector2(scrollWheel, scrollWheel);
                Update();
            }
        }
        public void Drag(Vector2 mouseMovement)
        {
            totalOffsetFromCenter -= mouseMovement;
            Update();
        }
        (int x, int y) GetCenterRoom()
        {
            Vector2 division = (totalOffsetFromCenter + new Vector2(0.5f, 0.5f) * TileSize) / TileSize;
            return (Globals.activeRoomCoords.x - (int)Math.Floor(division.X), Globals.activeRoomCoords.y + (int)Math.Floor(division.Y));
        }
        Vector2 GetOffset()
        {
            float offsetX = totalOffsetFromCenter.X % TileSize.X;
            if (offsetX >= 0.5f * TileSize.X)
                offsetX -= TileSize.X;
            else if (offsetX < -0.5f * TileSize.X)
                offsetX += TileSize.X;

            float offsetY = totalOffsetFromCenter.Y % TileSize.Y;
            if (offsetY >= 0.5f * TileSize.Y)
                offsetY -= TileSize.Y;
            else if (offsetY < -0.5f * TileSize.Y)
                offsetY += TileSize.Y;
            return new Vector2(offsetX, offsetY);
        }
        class Tile
        {
            public Vector2 Coord;
            public Vector2 Size;
            public TextureName Texture = TextureName.Rectangle;
            public Color Color = Color.LightGray;
            public Tile(Vector2 coord, Vector2 size) 
            {
                Coord = coord;
                Size = size;
            }
            public Tile(Vector2 coord, Vector2 size, TextureName texture, Color color)
            {
                Coord = coord;
                Size = size;
                Texture = texture;
                Color = color;
            }

        }
    }
}

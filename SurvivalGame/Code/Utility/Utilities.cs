using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SurvivalGame
{
    class Utilities
    {
        public static Texture2D CreateTexture(Color color, GraphicsDevice GraphicsDevice)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] data = new Color[1] { color };
            texture.SetData(data);
            return texture;
        }
        public static List<Drawing> CreateEmptyRectDrawings(Vector2 position, Color color, Vector2 scale, bool isDrawn = true, float depth = 0.2f)
        {
            List<Drawing> borders = new List<Drawing>();
            borders.Add(new Drawing(TextureName.Rectangle, new Vector2(position.X, position.Y), color, 0f, new Vector2(scale.X, 1), depth, isDrawn));
            borders.Add(new Drawing(TextureName.Rectangle, new Vector2(position.X, position.Y + scale.Y - 1), color, 0f, new Vector2(scale.X, 1), depth, isDrawn));
            borders.Add(new Drawing(TextureName.Rectangle, new Vector2(position.X, position.Y), color, 0f, new Vector2(1, scale.Y), depth, isDrawn));
            borders.Add(new Drawing(TextureName.Rectangle, new Vector2(position.X + scale.X - 1, position.Y), color, 0f, new Vector2(1, scale.Y), depth, isDrawn));
            return borders;
        }
        public static List<Drawing> CreateEmptyRectDrawings(Drawing drawing, Color color, int thickness = 1, bool isDrawn = true)
        {
            List<Drawing> borders = new List<Drawing>();
            Vector2 scale = new Vector2(drawing.GetWidth(), drawing.GetHeight());
            borders.Add(new Drawing(TextureName.Rectangle, drawing.Position, color, 0f, new Vector2(1, scale.Y), drawing.LayerDepth - 0.01f, isDrawn));
            borders.Add(new Drawing(TextureName.Rectangle, drawing.Position + new Vector2(scale.X - 1, 0), color, 0f, new Vector2(1, scale.Y), drawing.LayerDepth - 0.01f, isDrawn));
            borders.Add(new Drawing(TextureName.Rectangle, drawing.Position, color, 0f, new Vector2(scale.X, 1), drawing.LayerDepth - 0.01f, isDrawn));
            borders.Add(new Drawing(TextureName.Rectangle, drawing.Position + new Vector2(0, scale.Y - 1), color, 0f, new Vector2(scale.X, 1), drawing.LayerDepth - 0.01f, isDrawn));
            return borders;
        }

        public static Vector2 LinearVectorDamping(Vector2 vector, Vector2 damper)
        {
            Vector2 oldVector = vector;

            if (vector.X > 0)
                damper.X *= -1;
            if (vector.Y > 0)
                damper.Y *= -1;

            vector += damper;

            if ((vector.X <= 0) != (oldVector.X <= 0))
                vector.X = 0;
            if ((vector.Y <= 0) != (oldVector.Y <= 0))
                vector.Y = 0;

            return vector;
        }
        public static void SaveGame()
        {
            using StreamWriter file = new StreamWriter("C:\\Users\\Ivars\\source\\repos\\SurvivalGame\\SurvivalGame\\Code\\mapData.json");
            var serializeOptions = new JsonSerializerOptions { WriteIndented = true };
            serializeOptions.Converters.Add(new saveCovertor());
            foreach (var room in Globals.Rooms)
            {
                var roomForSave = new Room(room.Value.Coords, room.Value.Name, room.Value.BackgroundColor, addToRooms: false);
                roomForSave.background = room.Value.background;
                roomForSave.Entities.Clear();
                string jsonString = JsonSerializer.Serialize(roomForSave, serializeOptions);
                file.WriteLine(jsonString);
                Trace.WriteLine(jsonString);
            }

            Trace.WriteLine(JsonSerializer.Serialize((2, 3), serializeOptions));
        }
        public static void LoadGame()
        {
            //using StreamWriter file = new StreamWriter("C:\\Users\\Ivars\\source\\repos\\SurvivalGame\\SurvivalGame\\Code\\mapData.json");
            string jsonString = File.ReadAllText("C:\\Users\\Ivars\\source\\repos\\SurvivalGame\\SurvivalGame\\Code\\mapData.json");
            //List<Entities> Globals.Rooms.Clear();
            var serializeOptions = new JsonSerializerOptions { WriteIndented = true };
            serializeOptions.Converters.Add(new saveCovertor());
            //foreach (var line in jsonString)
            //{
            Room room = JsonSerializer.Deserialize<Room>(jsonString, serializeOptions);
                if(Globals.Rooms[room.Coords] != null)
                    Globals.Rooms[room.Coords].UnLoad();
                Globals.Rooms[room.Coords] = room;
            room.Load();

            //}
        }
    }
    public class saveCovertor : JsonConverter<>
    {
        public override object Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            Trace.WriteLine(reader.TokenType);
            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            object objectToWrite,
            JsonSerializerOptions options) 
        {

        }
    }
}

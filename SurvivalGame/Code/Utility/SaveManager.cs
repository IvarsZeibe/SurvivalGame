using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Reflection;

namespace SurvivalGame
{
    /// <summary>
    /// Doesnt work
    /// Some classes have private parameterless constructors
    /// </summary>
    static class SaveManager
    {
        static JsonSerializerOptions Options { get; set; }
        static MyReferenceHandler myReferenceHandler = new MyReferenceHandler();
        static SaveManager()
        {
            Options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                ReferenceHandler = myReferenceHandler
            };
            Options.Converters.Add(new TupleIntIntConverter());
            Options.Converters.Add(new Vector2Converter());
            Options.Converters.Add(new InheritanceFactory());
        }
        class SaveFile
        {
            public List<Room> rooms { get; set; } = new List<Room>(Globals.Rooms.Values);
            public Hotbar hotbar { get; set; } = Globals.HUD.hotbar;
            public MouseCursor mouse { get; set; } = Globals.MouseCursor;
            public int points { get; set; } = Globals.HUD.points;

        }
        public static void Save()
        {
            var data = JsonSerializer.Serialize(new SaveFile(), Options);
            File.WriteAllText(@"C:\Users\Ivars\source\repos\SurvivalGame\SurvivalGame\Code\data.txt", data);
            myReferenceHandler.Reset();
        }
        public static void Load()
        {
            var data = File.ReadAllText(@"C:\Users\Ivars\source\repos\SurvivalGame\SurvivalGame\Code\data.txt");
            var save = JsonSerializer.Deserialize<SaveFile>(data, Options);
            myReferenceHandler.Reset();

            Globals.getActiveRoom.UnLoad();
            Globals.HUD.hotbar.Deactivate();
            Globals.MouseCursor.UnLoad();

            Globals.MouseCursor = save.mouse;
            Globals.HUD.hotbar = save.hotbar;

            Globals.Rooms.Clear();
            foreach (var room in save.rooms)
            {
                Globals.Rooms.Add(room.Coords, room);
                if (room.isActive)
                    Globals.activeRoomCoords = room.Coords;
            }
            Globals.getActiveRoom.Load();

            Globals.MouseCursor.Load();
            Globals.HUD.hotbar.Activate();
            Globals.HUD.points = save.points;

            Globals.Map.Update();
        }
        public static Entity Clone(Entity entity) 
        {
            var data = JsonSerializer.Serialize(entity, Options);
            Entity ent = JsonSerializer.Deserialize<Entity>(data, Options);
            myReferenceHandler.Reset();
            return ent;
        }
        public static Room Clone(Room room)
        {
            var data = JsonSerializer.Serialize(room, Options);
            File.WriteAllText(@"C:\Users\Ivars\source\repos\SurvivalGame\SurvivalGame\Code\tempData.txt", data);
            myReferenceHandler.Reset();
            var dat = File.ReadAllText(@"C:\Users\Ivars\source\repos\SurvivalGame\SurvivalGame\Code\tempData.txt");
            Room rom = JsonSerializer.Deserialize<Room>(dat, Options);
            myReferenceHandler.Reset();
            return rom;
        }

    }
}

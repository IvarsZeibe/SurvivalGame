using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Command : IUpdate
    {
        private List<string> CommandList { get; set; } = new List<string>();
        public bool UpdateEnabled { get; set; } = true;
        public bool IsDead { get; set; } = false;
        private delegate void doCommand();
        private doCommand handler = delegate { };
        private Game1 Game1;
        public Command(Game1 game1)
        {
            this.Game1 = game1;
        }
        public StringBuilder CreateCommand(StringBuilder command)
        {
            CommandList.Clear();
            string commandText = command.ToString().Remove(0, 1).Trim().ToLower();
            foreach(var word in commandText.Split(' '))
            {
                if (word == " ")
                    continue;

                CommandList.Add(word);
            }

            try
            {
                if (CommandList[0] == "setdata")
                {
                    if(CommandList[1] == "world")
                    {
                        if (CommandList[2] == "spawnrate")
                        {
                            handler = delegate {
                                foreach(var i in Globals.Updatables)
                                {
                                    if(i is Spawner)
                                    {
                                        (i as Spawner).cooldown = (float)Convert.ToDouble(CommandList[3]);
                                    }
                                }
                            };
                        }
                        else
                            return new StringBuilder("Invalid command");
                    }
                    else if (CommandList[1] == "player")
                    {
                        if (CommandList[2] == "speed")
                        {
                            handler = delegate {
                                EntityTracker.GetEntities<Player>()[0].Speed = 1 / (float)Convert.ToDouble(CommandList[3]); 
                            };
                        }
                        else
                            return new StringBuilder("Invalid command");
                    }
                    else
                        return new StringBuilder("Invalid command");
                }
                else
                    return new StringBuilder("Invalid command");
            }
            catch(Exception e)
            {
                return new StringBuilder("Invalid command");
            }
            return command;
        }

        public void DoCommand(Game1 g1)
        {
            this.Game1 = g1;
            handler();
            handler = delegate { };
            g1 = this.Game1;
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

    }
}

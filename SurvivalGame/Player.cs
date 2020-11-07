﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Player : Entity
    {
        //public int speed = 200;
        public Player(Texture2D texture)
        {
            Mass = 10;
            //Force = 10;
            Collision = true;
            Speed = 1/200f;
            X = 100;
            Y = 100;
            Size = new Point(25, 50);
            Texture = texture;
        }


    }
}
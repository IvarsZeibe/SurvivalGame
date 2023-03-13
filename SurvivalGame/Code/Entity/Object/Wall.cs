using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Wall : Entity
    {

        public bool ghost { get; set; }
        public float timeAlive { get; set; }
        Wall() { }
        public Wall(TextureName texture, double x, double y, bool collision = true)
        {
            Mass = 19;
            this.Collision = collision;
            this.ghost = !collision;
            Hitbox = new Rect(x, y, 50, 50);
            Color color = Color.SaddleBrown;
            if (ghost)
                color = Color.SandyBrown;
            Drawing = new Drawing(texture, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), color, 0f, new Vector2(50, 50), isDrawn: true);

        }
        public override void SetDafaultValues()
        {
            base.SetDafaultValues();
            Mass = 19;
            this.Collision = true;
            this.ghost = false;
            Hitbox = new Rect(0, 0, 50, 50);
            Color color = Color.SaddleBrown;
            if (ghost)
                color = Color.SandyBrown;
            Drawing = new Drawing(TextureName.Rectangle, new Vector2((float)Hitbox.Left, (float)Hitbox.Top), color, 0f, new Vector2(50, 50), isDrawn: true);
        }
        public override void Update(GameTime gameTime)
        {
            timeAlive += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(ghost && timeAlive > 0.1)
            {
                Kill();
            }
        }
        protected override void CreateDefaultProperties()
        {
            base.CreateDefaultProperties();
            Properties.Add("width", new VariableReference(() => { return Hitbox.Width; }, (object o) => { Hitbox.Width = Convert.ToInt32(o); Drawing.Scale = Hitbox.GetScaleVector(); }));
            Properties.Add("height", new VariableReference(() => { return Hitbox.Height; }, (object o) => { Hitbox.Height = Convert.ToInt32(o); Drawing.Scale = Hitbox.GetScaleVector(); }));
        }
    }
}

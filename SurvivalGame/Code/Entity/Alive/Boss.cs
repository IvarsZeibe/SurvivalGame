using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Boss : Entity
    {
        float attackLenght = 5f;
        float retreatLenght = 5f;

        string currentPhase = "peaceful";
        float timeSincePhaseChange = 0f;
        float timeSinceLastAttack = 0f;
        float angryFor = 0f;
        Inventory Inventory = new Inventory(3);
        int activeWeapon = 0;
        HealthBar HealthBar;
        Vector2 direction = Vector2.Zero;
        public Boss() : base(false)
        {
            MaxHealth = 2000;
            Health = MaxHealth;
            Hitbox = new Circle(Globals.graphics.PreferredBackBufferWidth / 2, Globals.graphics.PreferredBackBufferHeight / 2, 40);
            Collision = true;
            Speed = 1f / 150;
            Target = null;
            Drawing = new Drawing(TextureName.Circle, Hitbox.GetTopLeftPosVector(), Color.DarkGray, 0f, Hitbox.GetScaleVector());
            HealthBar = new HealthBar(this);

            Inventory.Add(new SwordItem(50, 0.5f, knockbackStrenght: 5));
            Inventory.Add(new Pistol());
            Inventory.Add(new Shotgun());
            Drawings.Add("base", Drawing);
        }
        public override void Update(GameTime gameTime)
        {
            timeSincePhaseChange += (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeSinceLastAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
            angryFor -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            PhaseChanger();
            CheckForTarget();

            MoveSelf(gameTime);
            //Move(XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
            //Move(YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);

            ///Linear decrease
            Vector2 oldKnockback = RecievedKnockback;
            if (RecievedKnockback != Vector2.Zero)
                RecievedKnockback += -Vector2.Normalize(RecievedKnockback) * 150 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (RecievedKnockback.X > 0 && oldKnockback.X <= 0)
                RecievedKnockback = new Vector2(0, RecievedKnockback.Y);
            if (RecievedKnockback.Y > 0 && oldKnockback.Y <= 0)
                RecievedKnockback = new Vector2(RecievedKnockback.X, 0);
            ///Smooth decrease
            //RecievedKnockback *= (float)Math.Pow(0.4f, (float)gameTime.ElapsedGameTime.TotalSeconds);

            Attack();

            Drawing.Position = Hitbox.GetTopLeftPosVector();
        }
        void PhaseChanger()
        {
            if (Target is null || Hitbox.Distance(Target.Hitbox) >= 450)
            {
                currentPhase = "peaceful";
                Drawing.Color = Color.DarkGray;
            }
            switch (currentPhase)
            {
                case "attack":
                    if(timeSincePhaseChange > attackLenght)
                    {
                        currentPhase = "retreat";
                        timeSincePhaseChange = 0;
                        Drawing.Color = Color.Black;
                        //activeWeapon = 1;
                    }
                    break;
                case "retreat":
                    if(timeSincePhaseChange > retreatLenght)
                    {
                        currentPhase = "attack";
                        timeSincePhaseChange = 0;
                        Drawing.Color = new Color(40, 0, 0);
                        //activeWeapon = 0;
                    }
                    break;
                case "peaceful":
                    if(!(Target is null) && Hitbox.Distance(Target.Hitbox) < 450)
                    {
                        currentPhase = "attack";
                        Globals.Rooms[Globals.activeRoomCoords].CanLeave = false;
                        timeSincePhaseChange = 0;
                        Drawing.Color = new Color(40, 0, 0);
                    }
                    break;
            }
            if(angryFor > 0)
            {
                currentPhase = "attack";
                Drawing.Color = new Color(40, 0, 0);
                timeSincePhaseChange = 0;
            }
        }
        void MoveSelf(GameTime gameTime)
        {
            Vector2 track = Vector2.Zero;
            if (currentPhase != "peaceful" && !(Target is null) && !Target.IsDead)
            {
                track = new Vector2((float)(Target.Hitbox.X - Hitbox.X), (float)(Target.Hitbox.Y - Hitbox.Y));
                if (track == Vector2.Zero)
                    direction = Vector2.Zero;
                else
                    direction = Vector2.Normalize(track);
                if (currentPhase == "retreat")
                {
                    direction = -direction;
                    if (Hitbox.Distance(Target.Hitbox) > 310)
                    {
                        direction *= -1;
                    }
                    else if (Hitbox.Distance(Target.Hitbox) > 300)
                    {
                        direction = Vector2.Zero;
                    }
                }

            }
            else
            {
                Vector2 target = new Vector2((float)Globals.graphics.PreferredBackBufferWidth / 2, (float)Globals.graphics.PreferredBackBufferHeight / 2);
                track = new Vector2((float)(target.X - Hitbox.X), (float)(target.Y - Hitbox.Y));
                if (track == Vector2.Zero)
                    direction = Vector2.Zero;
                else
                    direction = Vector2.Normalize(track);
                currentPhase = "peaceful";
                Drawing.Color = Color.DarkGray;
            }
            XMovement = (direction.X * Speed + RecievedKnockback.X) * gameTime.ElapsedGameTime.TotalSeconds;
            if (Math.Abs(XMovement) > Math.Abs(track.X))
                XMovement = track.X;
            Move(XMovement, true);
            YMovement = (direction.Y * Speed + RecievedKnockback.Y) * gameTime.ElapsedGameTime.TotalSeconds;
            if (Math.Abs(YMovement) > Math.Abs(track.Y))
                YMovement = track.Y;
            Move(YMovement, false);
        }
        void CheckForTarget()
        {
            if (EntityTracker.GetEntities<Player>().Count > 0)
                Target = EntityTracker.GetEntities<Player>()[0];
        }
        void Attack()
        {
            if (currentPhase != "peaceful")
            {
                if (currentPhase == "retreat")
                {
                    if (Health > 1000)
                        activeWeapon = 1;
                    else
                        activeWeapon = 2;
                }
                else if (currentPhase == "attack")
                {
                    activeWeapon = 0;
                }
                if (timeSinceLastAttack > Inventory.Get(activeWeapon).Cooldown)
                {
                    Inventory.Get(activeWeapon).OnPrimaryUse(this);
                    timeSinceLastAttack = 0f;
                }
            }
        }
        public override bool DamageSelf(int damage, Entity source, DamageType damageType = DamageType.Unknown)
        {
            bool isDamaged = false;
            if(source.owner is Player)
            {
                Health -= damage;
                isDamaged = true;
            }
            if (Health <= 0)
            {
                Kill();
            }
            if (isDamaged && currentPhase == "peaceful")
            {
                angryFor = 10;
                currentPhase = "attack";
                Globals.Rooms[Globals.activeRoomCoords].CanLeave = false;
            }
            return isDamaged;
        }
        public override void Load() 
        {
            base.Load();
            HealthBar.Load();
        }
        public override void UnLoad() 
        {
            base.UnLoad();
            HealthBar.UnLoad();
        }
        public override void Kill()
        {
            base.Kill();
            Globals.Rooms[Globals.activeRoomCoords].CanLeave = true;
            Globals.HUD.EnemiesLeft--;
            Globals.HUD.points += 5;
        }
    }
}

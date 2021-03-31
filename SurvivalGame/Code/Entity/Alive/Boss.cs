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

        string currentPhase = "attack";
        float timeSincePhaseChange = 0f;
        float timeSinceLastAttack = 0f;
        Inventory Inventory = new Inventory(1);
        HealthBar HealthBar;
        public Boss() : base(false)
        {
            MaxHealth = 200;
            Health = MaxHealth;
            Hitbox = new Circle(Globals.graphics.PreferredBackBufferWidth / 2, Globals.graphics.PreferredBackBufferHeight / 2, 40);
            Collision = true;
            Speed = 200;
            Target = null;
            Drawing = new Drawing(TextureName.Circle, Hitbox.GetTopLeftPosVector(), Color.Black, 0f, Hitbox.GetScaleVector());
            Inventory.Add(new SwordItem(50, 0.5f, knockbackStrenght: 5));
            HealthBar = new HealthBar(this);
        }
        public override void Update(GameTime gameTime)
        {
            timeSincePhaseChange += (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeSinceLastAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
            PhaseChanger();
            CheckForTarget();

            UpdateMovement();
            Move(XMovement * gameTime.ElapsedGameTime.TotalSeconds, true);
            Move(YMovement * gameTime.ElapsedGameTime.TotalSeconds, false);
            
            RecievedKnockback = (RecievedKnockback - Vector2.One) / 2 + Vector2.One;

            Attack();

            Drawing.Position = Hitbox.GetTopLeftPosVector();
        }
        void PhaseChanger()
        {
            switch (currentPhase)
            {
                case "attack":
                    if(timeSincePhaseChange > attackLenght)
                    {
                        currentPhase = "retreat";
                        timeSincePhaseChange = 0;
                    }
                    break;
                case "retreat":
                    if(timeSincePhaseChange > retreatLenght)
                    {
                        currentPhase = "attack";
                        timeSincePhaseChange = 0;
                    }
                    break;
            }
        }
        void UpdateMovement()
        {
            if (!(Target is null) && !Target.IsDead)
            {
                double xedge = Hitbox.X - Target.Hitbox.X;
                double yedge = Hitbox.Y - Target.Hitbox.Y;
                if (currentPhase == "attack")
                {
                    XMovement = -xedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed * RecievedKnockback.X);
                    YMovement = -yedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed * RecievedKnockback.Y);
                }
                else if (currentPhase == "retreat")
                {
                    XMovement = -xedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed * RecievedKnockback.X);
                    YMovement = -yedge / ((Math.Abs(xedge) + Math.Abs(yedge)) * Speed * RecievedKnockback.Y);
                    if (Hitbox.Distance(Target.Hitbox) > 500)
                    {
                        XMovement = 0;
                        YMovement = 0;
                    }
                }
            }
        }
        void CheckForTarget()
        {
            if (EntityTracker.GetEntities<Player>().Count > 0)
                Target = EntityTracker.GetEntities<Player>()[0];
        }
        void Attack()
        {
            if(timeSinceLastAttack > Inventory.Get(0).Cooldown)
            {
                Inventory.Get(0).OnPrimaryUse(this);
                timeSinceLastAttack = 0f;
            }
        }
        public override bool DamageSelf(int damage, Entity source, DamageType damageType = DamageType.Unknown)
        {
            if(source is Player)
            {
                Health -= damage;
                return true;
            }
            return false;
        }
        public override void Load() 
        { 
            Drawing.IsDrawn = true;
            HealthBar.Load();
        }
        public override void UnLoad() 
        { 
            Drawing.IsDrawn = false;
            HealthBar.UnLoad();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootShapesUp
{
    class PlayerShip : Entity
    {
        private static PlayerShip instance;
        public static float ShotPattern = 0.1f;
        public static float InverseShotPattern = -0.1f;
        public static int PlasmaAmmo = 20;
        public static int Bombs = 3;
        public static PlayerShip Instance
           
        {
            get
            {
                if (instance == null)
                    instance = new PlayerShip();

                return instance;
            }
        }

        const int cooldownFrames = 6;
        int cooldownRemaining = 0;

        int framesUntilRespawn = 0;
        public bool IsDead { get { return framesUntilRespawn > 0; } }

        static Random rand = new Random();

        private PlayerShip()
        {
            image = GameRoot.Player;
            Position = GameRoot.ScreenSize / 2;
            Radius = 10;
        }

        public override void Update()
        {
            if (IsDead)
            {
                --framesUntilRespawn;
                return;
            }

            var aim = Input.GetAimDirection();
            if (aim.LengthSquared() > 0 && cooldownRemaining <= 0 && Input.mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) //if left mouse button is pressed the ship's gun fires
            {
                cooldownRemaining = cooldownFrames;
                float aimAngle = aim.ToAngle();
                Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

                float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f); //-0.04f, 0.04f
                Vector2 vel = 11f * new Vector2((float)Math.Cos(aimAngle + randomSpread), (float)Math.Sin(aimAngle + randomSpread));

                Vector2 offset = Vector2.Transform(new Vector2(35, -8), aimQuat);
                EntityManager.Add(new Bullet(Position + offset, vel));

                offset = Vector2.Transform(new Vector2(35, 8), aimQuat);
                EntityManager.Add(new Bullet(Position + offset, vel));

                GameRoot.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0); //0.02f,0
            }

            if (aim.LengthSquared() > 0 && cooldownRemaining <= 0 && Input.mouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && PlasmaAmmo >= 1) //if right mouse button is pressed the ship's gun fires
            {
                cooldownRemaining = cooldownFrames;
                float aimAngle = aim.ToAngle();
                Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

                float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f); //-0.04, 0.04
                Vector2 vel = 11f * new Vector2((float)Math.Cos(aimAngle + randomSpread), (float)Math.Sin(aimAngle + randomSpread));

                Vector2 offset = Vector2.Transform(new Vector2(35, -30), aimQuat);
                EntityManager.Add(new Bullet(Position + offset, vel));

                offset = Vector2.Transform(new Vector2(35, -15), aimQuat);
                EntityManager.Add(new Bullet(Position + offset, vel));

                offset = Vector2.Transform(new Vector2(35, 15), aimQuat);
                EntityManager.Add(new Bullet(Position + offset, vel));

                offset = Vector2.Transform(new Vector2(35, 30), aimQuat);
                EntityManager.Add(new Bullet(Position + offset, vel));

                PlasmaAmmo--;

                GameRoot.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0); //0.02f,0
            }

            if (Input.WasBombButtonPressed() && Bombs >=1)
            {
                EntityManager.Kill();
                Bombs--;
            }

            if (cooldownRemaining > 0)
                cooldownRemaining--;

            const float speed = 8;
            Velocity = speed * Input.GetMovementDirection();
            Position += Velocity;
            Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDead)
                base.Draw(spriteBatch);
        }

        public void Kill()
        {
            framesUntilRespawn = 60;
        }
    }
}

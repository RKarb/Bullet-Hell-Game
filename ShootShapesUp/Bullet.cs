using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootShapesUp
{
    class Bullet : Entity
    {
        public Bullet(Vector2 position, Vector2 velocity)
        {
            image = GameRoot.Bullet;
            Position = position;
            Velocity = velocity;
            Orientation = Velocity.ToAngle();
            Radius = 8;  // Bullet Hitbox size
        }

        public override void Update()
        {
            if (Input.mouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) // changes bullet image depending on button press
            {
                image = GameRoot.Bullet2;
            }
            else if (Input.mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) // changes bullet image depending on button press
            {
                image = GameRoot.Bullet;
            }

            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();

            Position += Velocity;

            // delete bullets that go off-screen
            if (!GameRoot.Viewport.Bounds.Contains(Position.ToPoint()))
                IsExpired = true;
        }
    }
}

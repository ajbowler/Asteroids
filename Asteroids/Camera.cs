using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    class Camera
    {
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        public GraphicsDevice Device { get; set; }

        public Camera(GraphicsDevice device)
        {
            this.Device = device;

            this.View = Matrix.CreateLookAt(
                new Vector3(0, 15f, 40f),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0)
            );

            this.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                device.Viewport.AspectRatio,
                1.0f,
                2000.0f
            );
        }

        public void Update(Spaceship spaceship)
        {
            Vector3 newPosition = new Vector3(0f, 15f, -40f);
            newPosition = Vector3.Transform(
                newPosition, 
                Matrix.CreateFromQuaternion(spaceship.Rotation)
            );
            newPosition += spaceship.Position;

            Vector3 newUp = new Vector3(0, 1, 0);
            newUp = Vector3.Transform(
                newUp, 
                Matrix.CreateFromQuaternion(spaceship.Rotation)
            );

            this.View = Matrix.CreateLookAt(newPosition, spaceship.Position, newUp);

            this.Projection =
                Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4, this.Device.Viewport.AspectRatio, 1.0f, 100000.0f
                );
        }

        public Vector3 GetDirection()
        {
            return Matrix.Invert(this.View).Forward;
        }

        public Vector3 GetUp()
        {
            return Matrix.Invert(this.View).Up;
        }

        public Vector3 GetRight()
        {
            return Vector3.Cross(GetDirection(), GetUp());
        }
    }
}

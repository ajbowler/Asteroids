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
        Matrix view;
        Matrix projection;
        GraphicsDevice device;

        public Camera(GraphicsDevice device)
        {
            this.device = device;

            this.view = Matrix.CreateLookAt(
                new Vector3(0, 15f, 40f),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0)
            );

            this.projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                device.Viewport.AspectRatio,
                1.0f,
                1000.0f
            );
        }

        public void Update(Spaceship spaceship)
        {
            Vector3 newPosition = new Vector3(0f, 15f, 40f);
            newPosition = Vector3.Transform(
                newPosition, 
                Matrix.CreateFromQuaternion(spaceship.getRotation())
            );
            newPosition += spaceship.getPosition();

            Vector3 newUp = new Vector3(0, 1, 0);
            newUp = Vector3.Transform(
                newUp, 
                Matrix.CreateFromQuaternion(spaceship.getRotation())
            );

            setView(
                Matrix.CreateLookAt(
                    newPosition, spaceship.getPosition(), newUp
                )
            );

            setProjection(
                Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4, this.device.Viewport.AspectRatio, 1.0f, 1000.0f
                )
            );
        }

        public Matrix getView()
        {
            return this.view;
        }

        public void setView(Matrix view)
        {
            this.view = view;
        }

        public Matrix getProjection()
        {
            return this.projection;
        }

        public void setProjection(Matrix projection)
        {
            this.projection = projection;
        }
    }
}

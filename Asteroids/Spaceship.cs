using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    class Spaceship
    {
        public const string MODEL_PATH = "Models/spaceship";
        public const string TEXTURE_PATH = "Models/metal";

        public const float ACCEL_CONSTANT = .7f;
        public const float DECEL_CONSTANT = .2f;
        public const float VELOCITY_MAX = 40f;

        private Vector3 position;
        private Quaternion rotation;
        private float velocity;
        private Matrix world;
        private Model model;
        private Texture2D texture;
        private BoundingSphere boundingSphere;

        public Spaceship()
        {
            this.position = new Vector3();
            this.rotation = Quaternion.Identity;
            this.model = null;
            this.texture = null;
            this.velocity = 0f;
            this.world = Matrix.Identity;
            this.boundingSphere = new BoundingSphere();
        }

        public void LoadModel(ContentManager content, BasicEffect effect)
        {
            float radius = 0f;
            this.model = content.Load<Model>(MODEL_PATH);
            foreach (ModelMesh mesh in model.Meshes)
            {
                radius = Math.Max(radius, mesh.BoundingSphere.Radius);

                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    this.texture = currentEffect.Texture;
                }

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect.Clone();
                }
            }

            this.boundingSphere = new BoundingSphere(getPosition(), radius);
        }

        public void Update(CollisionEngine collisionEngine, MouseState originalMouseState, 
            GameTime gameTime, GraphicsDevice device)
        {
            CheckCollisions(collisionEngine);
            ProcessKeyboard(gameTime);
            ProcessMouse(originalMouseState, gameTime, device);
        }

        public void Draw(ContentManager content, Matrix view, Matrix projection)
        {
            setWorldMatrix(Matrix.CreateRotationX(MathHelper.Pi / 2) *
                Matrix.CreateRotationZ(MathHelper.Pi) *
                Matrix.CreateFromQuaternion(getRotation()) *
                Matrix.CreateTranslation(getPosition()));

            this.texture = content.Load<Texture2D>(TEXTURE_PATH);

            Matrix[] transformation = new Matrix[this.model.Bones.Count];
            this.model.CopyAbsoluteBoneTransformsTo(transformation);
            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.TextureEnabled = true;
                    effect.Texture = this.texture;
                }
                mesh.Draw();
            }
        }

        public Model getModel()
        {
            return this.model;
        }

        public BoundingSphere getBoundingSphere()
        {
            return this.boundingSphere;
        }

        public void setBoundingSphere(Vector3 position, float radius)
        {
            this.boundingSphere = new BoundingSphere(position, radius);
        }

        public Vector3 getPosition()
        {
            return this.position;
        }

        public void setPosition(Vector3 position)
        {
            this.position = position;
            setBoundingSphere(position, getBoundingSphere().Radius);
        }

        public Quaternion getRotation()
        {
            return this.rotation;
        }

        public void setRotation(Quaternion rotation)
        {
            this.rotation = rotation;
        }

        public Matrix getWorldMatrix()
        {
            return this.world;
        }

        public void setWorldMatrix(Matrix world)
        {
            this.world = world;
        }

        public float getVelocity()
        {
            return this.velocity;
        }

        public void setVelocity(float velocity)
        {
            this.velocity = velocity;
        }

        private void ProcessKeyboard(GameTime gameTime)
        {
            KeyboardState keys = Keyboard.GetState();
            if (keys.GetPressedKeys().Length > 0)
            {
                // Move forward
                if (keys.IsKeyDown(Keys.W))
                {
                    Thrust(gameTime);
                }

                if (keys.IsKeyDown(Keys.D))
                {
                    Roll(gameTime, "right");
                }

                if (keys.IsKeyDown(Keys.A))
                {
                    Roll(gameTime, "left");
                }
            }
            else
            {
                Stop(gameTime);
            }
        }

        /**
         * The direction vector is a bit different with this model 
         * because of the way it's oriented in Blender. Here we use the Up direction.
         */
        private void Thrust(GameTime gameTime)
        {
            float changeInTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float newVelocity = getVelocity() +
                (ACCEL_CONSTANT / (float)gameTime.ElapsedGameTime.Milliseconds) * changeInTime;
            if (newVelocity > VELOCITY_MAX)
                newVelocity = VELOCITY_MAX;
            setVelocity(newVelocity);
            Vector3 newPosition = getWorldMatrix().Up * newVelocity;
            setWorldMatrix(getWorldMatrix() * Matrix.CreateTranslation(newPosition));
            setPosition(newPosition + getPosition());
        }

        private void Stop(GameTime gameTime)
        {
            float changeInTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float newVelocity = getVelocity() +
                (-DECEL_CONSTANT / (float)gameTime.ElapsedGameTime.Milliseconds) * changeInTime;
            if (newVelocity < 0)
                newVelocity = 0;
            setVelocity(newVelocity);
            Vector3 newPosition = getWorldMatrix().Up * newVelocity;
            setWorldMatrix(getWorldMatrix() * Matrix.CreateTranslation(newPosition));
            setPosition(newPosition + getPosition());
        }

        private void Roll(GameTime gameTime, string direction)
        {
            Quaternion rotation = getRotation();
            float rotationDirection = 0.4f / gameTime.ElapsedGameTime.Milliseconds;
            if (direction.Equals("right"))
            {
                rotation *= Quaternion.CreateFromYawPitchRoll(0, 0, -rotationDirection);
            }
            else if (direction.Equals("left"))
            {
                rotation *= Quaternion.CreateFromYawPitchRoll(0, 0, rotationDirection);
            }

            setRotation(rotation);
        }

        private void ProcessMouse(MouseState originalMouseState, GameTime gameTime, GraphicsDevice device)
        {
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                Quaternion rotation = getRotation();
                float rotationFactor = 0.01f / gameTime.ElapsedGameTime.Milliseconds;
                Quaternion newRotation = Quaternion.CreateFromYawPitchRoll(
                    xDifference * -rotationFactor,
                    yDifference * rotationFactor,
                    0
                );

                Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
                rotation *= newRotation;
                setRotation(rotation);
            }
        }

        private void CheckCollisions(CollisionEngine collisionEngine)
        {
            // Check if the ship hits the edge of the universe
            if (collisionEngine.ShipCollidesWithEdge(this))
            {
                // For now, just negate the velocity and send the ship flying backwards
                this.setVelocity(-getVelocity());
            }
        }
    }
}

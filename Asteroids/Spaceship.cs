using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Asteroids
{
    class Spaceship
    {
        public const string MODEL_PATH = "Models/spaceship";
        public const string TEXTURE_PATH = "Models/metal";

        public const float ACCEL_CONSTANT = 0.7f;
        public const float DECEL_CONSTANT = 0.4f;
        public const float VELOCITY_MAX = 40f;
        public const int MAX_LIVES = 3;

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Velocity { get; set; }
        public float Speed { get; set; }
        public Matrix World { get; set; }
        public Model Model { get; set; }
        public Texture2D Texture { get; set; }
        public BoundingSphere BoundingSphere { get; set; }
        public bool Destroyed { get; set; }
        public int Lives { get; set; }
        public Powerup Shield { get; set; }
        public Powerup Shrink { get; set; }

        public Spaceship()
        {
            this.Position = Vector3.Zero;
            this.Rotation = Quaternion.Identity;
            this.Model = null;
            this.Texture = null;
            this.Velocity = Vector3.Zero;
            this.Speed = 0f;
            this.World = Matrix.Identity;
            this.BoundingSphere = new BoundingSphere();
            this.Destroyed = false;
            this.Lives = 3;
            this.Shield = null;
            this.Shrink = null;
        }

        public void LoadModelAndTexture(ContentManager content, BasicEffect effect)
        {
            float radius = 0f;
            this.Model = content.Load<Model>(MODEL_PATH);
            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                radius = Math.Max(radius, mesh.BoundingSphere.Radius);

                foreach (BasicEffect currentEffect in mesh.Effects)
                    this.Texture = currentEffect.Texture;

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();
            }

            this.BoundingSphere = new BoundingSphere(this.Position, radius);
            this.Texture = content.Load<Texture2D>(TEXTURE_PATH);
        }

        public void Update(Vector3 direction, CollisionEngine collisionEngine, 
            SoundEngine soundEngine, ParticleEngine particleEngine,List<Asteroid> asteroids, 
            List<Powerup> powerups, MouseState originalMouseState, GameTime gameTime, 
            GraphicsDevice device)
        {
            if (this.Shrink !=  null)
            {
                if (this.Shrink.Activated)
                {
                    this.BoundingSphere = new BoundingSphere(
                        this.Position, this.BoundingSphere.Radius * 0.25f);
                }
            }
            CheckCollisions(collisionEngine, soundEngine, powerups, asteroids);

            if (this.Destroyed)
            {
                if (this.Lives == 0)
                    return;
                else
                {
                    particleEngine.AddParticle(gameTime, this.Position);
                    UpdatePosition(new Vector3(0, 0, 0));
                    this.Destroyed = false;
                    this.Speed = 0f;
                    this.Velocity = Vector3.Zero;
                    this.Rotation = Quaternion.Identity;
                }
            }

            ProcessKeyboard(direction, gameTime, soundEngine);
            ProcessMouse(originalMouseState, gameTime, device);
        }

        public void Draw(ContentManager content, Matrix view, Matrix projection)
        {
            this.World = DoShrink() * 
                Matrix.CreateRotationX(MathHelper.Pi / 2) *
                Matrix.CreateRotationZ(MathHelper.Pi) *
                Matrix.CreateFromQuaternion(this.Rotation) *
                Matrix.CreateTranslation(this.Position);

            Matrix[] transformation = new Matrix[this.Model.Bones.Count];
            this.Model.CopyAbsoluteBoneTransformsTo(transformation);
            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = World;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.TextureEnabled = true;
                    effect.Texture = this.Texture;
                }
                mesh.Draw();
            }
        }

        private void ProcessKeyboard(Vector3 direction, GameTime gameTime, SoundEngine soundEngine)
        {
            KeyboardState keys = Keyboard.GetState();
            if (keys.GetPressedKeys().Length > 0)
            {
                if (keys.IsKeyDown(Keys.W))
                {
                    Thrust(direction, gameTime);
                    soundEngine.ShipEngine.Play();
                }

                if (keys.IsKeyDown(Keys.D))
                {
                    Roll(gameTime, "right");
                    soundEngine.ShipEngine.Play();
                }

                if (keys.IsKeyDown(Keys.A))
                {
                    Roll(gameTime, "left");
                    soundEngine.ShipEngine.Play();
                }

                if (keys.IsKeyDown(Keys.Q))
                    this.Shrink.Activated = true;
            }
            else
            {
                Stop(direction, gameTime);
                soundEngine.ShipEngine.Stop();
            }
        }

        /**
         * The direction vector is a bit different with this model 
         * because of the way it's oriented in Blender. Here we use the Up direction.
         */
        private void Thrust(Vector3 direction, GameTime gameTime)
        {
            float changeInTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float speed = (ACCEL_CONSTANT / (float)gameTime.ElapsedGameTime.Milliseconds) 
                * changeInTime
                + this.Speed;
            if (speed > VELOCITY_MAX)
                speed = VELOCITY_MAX;
            this.Speed = speed;
            Vector3 velocity = CalculateVelocityVector(speed, direction);
            this.Velocity = velocity;
            Vector3 newPosition = this.Position + velocity;
            UpdatePosition(newPosition);
        }

        private void Stop(Vector3 direction, GameTime gameTime)
        {
            float changeInTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float speed = (-DECEL_CONSTANT / (float)gameTime.ElapsedGameTime.Milliseconds) 
                * changeInTime
                + this.Speed;
            if (speed < 0)
                speed = 0;
            this.Speed = speed;
            Vector3 velocity = CalculateVelocityVector(speed, direction);
            this.Velocity = velocity;
            Vector3 newPosition = this.Position + velocity;
            UpdatePosition(newPosition);
        }

        private Vector3 CalculateVelocityVector(float speed, Vector3 direction)
        {
            Vector3 velocity = Vector3.Zero + direction;
            velocity.Normalize();
            velocity *= speed;
            return velocity;
        }

        private void Roll(GameTime gameTime, string direction)
        {
            Quaternion rotation = this.Rotation;
            float rotationDirection = 0.4f / gameTime.ElapsedGameTime.Milliseconds;
            if (direction.Equals("right"))
                rotation *= Quaternion.CreateFromYawPitchRoll(0, 0, -rotationDirection);
            else if (direction.Equals("left"))
                rotation *= Quaternion.CreateFromYawPitchRoll(0, 0, rotationDirection);
            this.Rotation = rotation;
        }

        private void ProcessMouse(MouseState originalMouseState, GameTime gameTime, GraphicsDevice device)
        {
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                Quaternion rotation = this.Rotation;
                float rotationFactor = 0.01f / gameTime.ElapsedGameTime.Milliseconds;
                Quaternion newRotation = Quaternion.CreateFromYawPitchRoll(
                    xDifference * -rotationFactor,
                    yDifference * rotationFactor,
                    0
                );

                Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
                rotation *= newRotation;
                this.Rotation = rotation;
            }
        }

        private void CheckCollisions(CollisionEngine collisionEngine, SoundEngine soundEngine, 
            List<Powerup> powerups, List<Asteroid> asteroids)
        {
            // Check if the ship hits the edge of the universe
            if (collisionEngine.CollidesWithEdge(this.Position, this.BoundingSphere))
            {
                float edge = collisionEngine.EDGE_OF_UNIVERSE;

                Vector3 pos = this.Position;
                if (pos.X > edge)
                    pos.X = edge;
                if (pos.X < -edge)
                    pos.X = -edge;
                if (pos.Y > edge)
                    pos.Y = edge;
                if (pos.Y < -edge)
                    pos.Y = -edge;
                if (pos.Z > edge)
                    pos.Z = edge;
                if (pos.Z < -edge)
                    pos.Z = -edge;
                UpdatePosition(pos);
            }

            // The ship is destroyed if it hits an asteroid unless it has a shield, 
            // in that case the asteroid is destroyed.
            foreach (Asteroid asteroid in asteroids)
            {
                if (collisionEngine.CollideTwoObjects(this.BoundingSphere, 
                    asteroid.BoundingSphere))
                {
                    if (this.Shield != null)
                    {
                        asteroid.Destroyed = true;
                        this.Shield = null;
                    }
                    else
                        this.Destroyed = true;
                    if (soundEngine.Explosion.State != SoundState.Playing)
                        soundEngine.Explosion.Play();
                    LoseLife();
                }
            }

            // The ship gets a powerup if it runs over it
            foreach (Powerup powerup in powerups)
            {
                if (collisionEngine.CollideTwoObjects(this.BoundingSphere, powerup.BoundingSphere))
                {
                    if (powerup.Type == Powerup.PowerupType.Shield && this.Shield == null)
                        this.Shield = powerup;
                    if (powerup.Type == Powerup.PowerupType.Shrink && this.Shrink == null)
                        this.Shrink = powerup;
                    powerup.Collected = true;
                    // The powerup is activated if it is a shield
                    if (powerup.Type == Powerup.PowerupType.Shield)
                        powerup.Activated = true;
                    break;
                }
            }
        }

        public void LoseLife()
        {
            this.Lives--;
            if (this.Lives < 0)
                this.Lives = 0;
        }

        public void UpdatePosition(Vector3 position)
        {
            this.Position = position;
            this.BoundingSphere = new BoundingSphere(position, this.BoundingSphere.Radius);
        }

        private Matrix DoShrink()
        {
            if (this.Shrink != null)
            {
                if (this.Shrink.Activated)
                    return Matrix.CreateScale(0.25f);
                else
                    return Matrix.Identity;
            }
            else
                return Matrix.Identity;
        }
    }
}

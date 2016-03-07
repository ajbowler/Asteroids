﻿using System;
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

        public float ACCEL_CONSTANT = .7f;
        public float DECEL_CONSTANT = .2f;
        public float VELOCITY_MAX = 40f;

        private Vector3 position;
        private Quaternion rotation;
        private float velocity;
        private Matrix world;
        private Model model;
        private Texture2D texture;
        private BoundingBox boundingBox;

        public Spaceship()
        {
            this.position = new Vector3();
            this.rotation = Quaternion.Identity;
            this.model = null;
            this.texture = null;
            this.velocity = 0f;
            this.world = Matrix.Identity;
        }

        public void LoadModel(ContentManager content, BasicEffect effect)
        {
            this.model = content.Load<Model>(MODEL_PATH);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    this.texture = currentEffect.Texture;
                }

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect.Clone();
                }
            }
        }

        public void LoadBoundingBox()
        {
            Vector3 modelMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 modelMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                Vector3 meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                Vector3 meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    int stride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    byte[] vertexData = new byte[stride * meshPart.NumVertices];
                    meshPart.VertexBuffer.GetData(meshPart.VertexOffset * stride, vertexData, 0, meshPart.NumVertices, 1); // fixed 13/4/11
                    Vector3 vertPosition = new Vector3();
                    for (int i = 0; i < vertexData.Length; i += stride)
                    {
                        vertPosition.X = BitConverter.ToSingle(vertexData, i);
                        vertPosition.Y = BitConverter.ToSingle(vertexData, i + sizeof(float));
                        vertPosition.Z = BitConverter.ToSingle(vertexData, i + sizeof(float) * 2);
                        meshMin = Vector3.Min(meshMin, vertPosition);
                        meshMax = Vector3.Max(meshMax, vertPosition);
                    }
                }

                Matrix[] transformation = new Matrix[this.model.Bones.Count];
                this.model.CopyAbsoluteBoneTransformsTo(transformation);
                meshMin = Vector3.Transform(meshMin, transformation[mesh.ParentBone.Index]);
                meshMax = Vector3.Transform(meshMax, transformation[mesh.ParentBone.Index]);
                modelMin = Vector3.Min(modelMin, meshMin);
                modelMax = Vector3.Max(modelMax, meshMax);
            }
            this.boundingBox = new BoundingBox(modelMin, modelMax);
        }

        public void Update(GameTime gameTime)
        {
            ProcessKeyboard(gameTime);
            ProcessMouse(gameTime);
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

        public Vector3 getPosition()
        {
            return this.position;
        }

        public void setPosition(Vector3 position)
        {
            this.position = position;
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

        private void ProcessMouse(GameTime gameTime)
        {
            Quaternion rotation = getRotation();
            float rotationFactor = 0.01f / gameTime.ElapsedGameTime.Milliseconds;
            MouseState mouseState = Mouse.GetState();
            if (mouseState != null)
            {
                Quaternion newRotation = Quaternion.CreateFromYawPitchRoll(
                    mouseState.X * -rotationFactor, 
                    mouseState.Y * rotationFactor, 
                    0
                );

                Mouse.SetPosition(0, 0);
                rotation *= newRotation;
                setRotation(rotation);
            }
        }
    }
}

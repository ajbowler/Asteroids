﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    class Torpedo
    {
        public const string MODEL_PATH = "Models/torpedo";

        private Model model;
        private Vector3 position;
        private float velocity;
        private Matrix world;
        private BoundingSphere boundingSphere;

        public Torpedo()
        {
            this.model = null;
            this.position = new Vector3();
            this.velocity = 0f;
            this.world = Matrix.Identity;
        }

        public void LoadModel(ContentManager content, BasicEffect effect)
        {
            float radius = 0f;
            this.model = content.Load<Model>(MODEL_PATH);
            foreach (ModelMesh mesh in model.Meshes)
            {
                radius = Math.Max(radius, mesh.BoundingSphere.Radius);

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect.Clone();
                }
            }

            this.boundingSphere = new BoundingSphere(getPosition(), radius);
        }

        public Model getModel()
        {
            return this.model;
        }

        public Vector3 getPosition()
        {
            return this.position;
        }

        public void setPosition(Vector3 position)
        {
            this.position = position;
        }

        public float getVelocity()
        {
            return this.velocity;
        }

        public void setVelocity(float velocity)
        {
            this.velocity = velocity;
        }

        public Matrix getWorldMatrix()
        {
            return this.world;
        }

        public void setWorldMatrix(Matrix world)
        {
            this.world = world;
        }

        public BoundingSphere getBoundingSphere()
        {
            return this.boundingSphere;
        }

        public void setBoundingSphere(BoundingSphere boundingSphere)
        {
            this.boundingSphere = boundingSphere;
        }
    }
}

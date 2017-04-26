using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class Collider
    {
        public enum BoundingType { Sphere, Box, Complex }

        private readonly GameObject _gameObject;

        public BoundingType BType;

        public List<BoundingSphere> SphereColliders = new List<BoundingSphere>();
        public BoundingSphere SphereCollider;
        public BoundingBox BoxCollider;

        public Collider(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public void CreateSingleBoundingSphereForModel()
        {
            BoundingSphere boundingSphere = new BoundingSphere();

            if (_gameObject.Model == null)
                return;

            if (_gameObject.IsAnimated)
            {
                Matrix[] boneTransforms = new Matrix[_gameObject.Model.Bones.Count];
                _gameObject.Model.CopyAbsoluteBoneTransformsTo(boneTransforms);

                foreach (ModelMesh mesh in _gameObject.Model.Meshes)
                {
                    boundingSphere = BoundingSphere.CreateMerged(boundingSphere, mesh.BoundingSphere);
                    boundingSphere.Transform(boneTransforms[mesh.ParentBone.Index]);
                }
            }
            else
            {
                foreach (ModelMesh mesh in _gameObject.Model.Meshes)
                {
                    boundingSphere = BoundingSphere.CreateMerged(boundingSphere, mesh.BoundingSphere);
                }
            }

            //boundingSphere.Center = this.TransformationMatrix.Translation;
            //this.Scale = this.TransformationMatrix.Scale;
            //float maxScale = new float[] {this.Scale.X, this.Scale.Y, this.Scale.Z}.Max();
            //boundingSphere.Radius *= maxScale;
            boundingSphere = boundingSphere.Transform(_gameObject.TransformationMatrix);

            this.SphereCollider = boundingSphere;
            this.BType = BoundingType.Sphere;
        }

        public void CreateComplexBoundingSphereForModel()
        {
            List<BoundingSphere> collider = new List<BoundingSphere>();

            if (_gameObject.Model == null)
                return;

            if (_gameObject.IsAnimated)
            {
                Matrix[] boneTransforms = new Matrix[_gameObject.Model.Bones.Count];
                _gameObject.Model.CopyAbsoluteBoneTransformsTo(boneTransforms);

                foreach (ModelMesh mesh in _gameObject.Model.Meshes)
                {
                    BoundingSphere sphere = mesh.BoundingSphere;
                    sphere = sphere.Transform(boneTransforms[mesh.ParentBone.Index]);
                    sphere = sphere.Transform(_gameObject.TransformationMatrix);
                    collider.Add(sphere);
                }
            }
            else
            {
                foreach (ModelMesh mesh in _gameObject.Model.Meshes)
                {
                    BoundingSphere sphere = mesh.BoundingSphere;
                    sphere = sphere.Transform(_gameObject.TransformationMatrix);
                    collider.Add(sphere);
                }
            }

            //boundingSphere.Center = this.TransformationMatrix.Translation;
            //this.Scale = this.TransformationMatrix.Scale;
            //float maxScale = new float[] {this.Scale.X, this.Scale.Y, this.Scale.Z}.Max();
            //boundingSphere.Radius *= maxScale;

            this.SphereColliders = collider;
            this.BType = BoundingType.Complex;
        }

        public void CreateBoundingBoxForModel()
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in _gameObject.Model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), _gameObject.TransformationMatrix);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return the model bounding box
            this.BoxCollider = new BoundingBox(min, max);
            this.BType = BoundingType.Box;
        }

        public void RecreateBounding()
        {
            switch (this.BType)
            {
                case BoundingType.Box:
                    CreateBoundingBoxForModel();
                    break;

                case BoundingType.Sphere:
                    CreateSingleBoundingSphereForModel();
                    break;

                case BoundingType.Complex:
                    CreateComplexBoundingSphereForModel();
                    break;
            }           
        }

        public void UpdateBoundary()
        {
            switch (this.BType)
            {
                case BoundingType.Box:
                {
                    Vector3 min = this.BoxCollider.Min;
                    Vector3 max = this.BoxCollider.Max;
                    Vector3 center = new Vector3((min.X + max.X) / 2, (min.Y + max.Y) / 2, (min.Z + max.Z) / 2);
                    Vector3 translation = _gameObject.TransformationMatrix.Translation - center;

                    min += translation;
                    max += translation;
                    this.BoxCollider = new BoundingBox(min, max);
                    break;
                }

                case BoundingType.Sphere:
                {
                    Vector3 center = this.SphereCollider.Center;
                    Vector3 translation = _gameObject.TransformationMatrix.Translation - center;
                    this.SphereCollider.Center = center + translation;
                    break;
                }

                case BoundingType.Complex:
                {
                    for (int i = 0; i < this.SphereColliders.Count; i++)
                    {
                        Vector3 center = SphereColliders[i].Center;
                        Vector3 translation = _gameObject.TransformationMatrix.Translation - center;
                        BoundingSphere sphere = SphereColliders[i];
                        sphere.Center = center + translation;
                        this.SphereColliders[i] = sphere;
                    }
                    break;
                }
            } 
        }

        public bool Intersects(Collider other)
        {
            bool intersect = false;  // flag for complex colliders

            switch (this.BType)
            {
                case BoundingType.Box:

                    switch (other.BType)
                    {
                        case BoundingType.Box:
                            return this.BoxCollider.Intersects(other.BoxCollider);
                            
                        case BoundingType.Sphere:
                            return this.BoxCollider.Intersects(other.SphereCollider);

                        case BoundingType.Complex:
                            foreach (BoundingSphere sphere in other.SphereColliders)
                            {
                                if (this.BoxCollider.Intersects(sphere))
                                    intersect = true;
                            }
                            return intersect;
                    }
                    break;

                case BoundingType.Sphere:

                    switch (other.BType)
                    {
                        case BoundingType.Box:
                            return this.SphereCollider.Intersects(other.BoxCollider);

                        case BoundingType.Sphere:
                            return this.SphereCollider.Intersects(other.SphereCollider);

                        case BoundingType.Complex:
                            foreach (BoundingSphere sphere in other.SphereColliders)
                            {
                                if (this.SphereCollider.Intersects(sphere))
                                    intersect = true;
                            }
                            return intersect;
                    }
                    break;

                case BoundingType.Complex:

                    switch (other.BType)
                    {
                        case BoundingType.Box:
                            foreach (BoundingSphere sphere1 in this.SphereColliders)
                            {
                                if (sphere1.Intersects(other.BoxCollider))
                                    intersect = true;
                            }
                            return intersect;

                        case BoundingType.Sphere:
                            foreach (BoundingSphere sphere1 in this.SphereColliders)
                            {
                                if (sphere1.Intersects(other.SphereCollider))
                                    intersect = true;
                            }
                            return intersect;

                        case BoundingType.Complex:
                            foreach (BoundingSphere sphere1 in this.SphereColliders)
                            {
                                foreach (BoundingSphere sphere2 in other.SphereColliders)
                                {
                                    if (sphere1.Intersects(sphere2))
                                        intersect = true;
                                }
                            }
                            return intersect;
                    }
                    break;
            }

            return false;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ToylandSiege.GameObjects
{
    public abstract class GameObject
    {
        public string Name = "GameObject";
        public string Type;

        public Vector3 Position = new Vector3(0, 0, 0);
        public Vector3 Rotation = new Vector3(0, 0, 0);
        public Vector3 Scale = new Vector3(1, 1, 1);

        public bool IsStatic = false;
        public bool IsCollidable = true;
        public bool IsEnabled = true;

        public Model Model;
        public GameObject Parent = null;
        public List<GameObject> Childs = new List<GameObject>();

        public Matrix TransformationMatrix;

        public enum BoundingType { Box, Sphere };
        public BoundingType BType;
        public BoundingSphere BSphere;
        public BoundingBox BBox;

        protected abstract void Initialize();
        public abstract void Update();

        public virtual void Draw()
            {
            if (!IsEnabled)
                return;

            if (Model != null)
            {
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        if(ToylandSiege.GetToylandSiege().configurationManager.LigthningEnabled)
                            effect.EnableDefaultLighting();
                        effect.AmbientLightColor = new Vector3(0, 0.3f, 0.3f);
                        effect.View = Camera.GetCurrentCamera().ViewMatrix;

                        effect.World = TransformationMatrix;
                        effect.Projection = Camera.GetCurrentCamera().ProjectionMatrix;
                    }
                    mesh.Draw();
                }
            }
            foreach (var child in Childs)
            {
                child.Draw();
            }
        }

        public void AddChild(GameObject obj)
        {
            Childs.Add(obj);
            obj.Parent = this;
            Logger.Log.Debug("GameObject " + obj.Name + " added to " + Name + " GameObject type: " + Type);
        }

        public void RemoveChild(GameObject obj)
        {
            if (Childs.Remove(obj))
                Logger.Log.Debug("GameObject " + obj.Name + " removed from " + Name + " GameObject type: " + Type);
            else
                Logger.Log.Debug("Cant remove GameObject " + obj.Name + " from " + Name + " GameObject type: " + Type);

        }

        public bool RemoveChild(string name)
        {
            bool removed = false;
            for (int i = 0; i < Childs.Count; i++)
                if (Childs[i].Name == name)
                {
                    Childs.RemoveAt(i);
                    Logger.Log.Debug("GameObject " + Childs[i].Name + " removed from scene");
                    i--;

                    removed = true;
                }
            return removed;
        }

        public GameObject FindGameObjectInLevelByName(string name)
        {
            //TODO: Implement reccursive search
            foreach (var item in Childs)
                if (item.Name == name)
                    return item;
            return null;
        }

        public List<GameObject> FindGameObjectsInLevelByName(string name)
        {
            //TODO: Implement reccursive search
            List<GameObject> FoundedItems = new List<GameObject>();

            foreach (var item in Childs)
                if (item.Name == name)
                    FoundedItems.Add(item);


            if (FoundedItems.Count != 0)
                return FoundedItems;
            return null;
        }

        public override string ToString()
        {
            if (Parent == null)
                return "Name: " + this.Name + ", Type:" + this.Type;
            return "Name: " + this.Name + ", Type: " + this.Type + ", Parent: " + Parent.Name;
        }

        public  Matrix CreateTransformationMatrix()
        {
            TransformationMatrix = Matrix.CreateScale(Scale) *
                Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) *
                Matrix.CreateTranslation(Position);

            if (Parent != null)
                TransformationMatrix *= Parent.TransformationMatrix;

            return TransformationMatrix;
        }

        public List<GameObject> GetAllChilds(GameObject obj)
        {
            List<GameObject> childs = new List<GameObject>();
            foreach (var child in obj.Childs)
            {
                childs.Add(child);
                childs = childs.Concat(GetAllChilds(child)).ToList();
            }
            return childs;
        }

        public void CreateBoundingSphereForModel()
        {
            Matrix[] boneTransforms = new Matrix[this.Model.Bones.Count];
            this.Model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            BoundingSphere boundingSphere = new BoundingSphere();
            BoundingSphere meshSphere;

            for (int i = 0; i < this.Model.Meshes.Count; i++)
            {
                meshSphere = this.Model.Meshes[i].BoundingSphere.Transform(boneTransforms[i]);
                boundingSphere = BoundingSphere.CreateMerged(boundingSphere, meshSphere);
            }
            //TODO: Should recalculate matrix here?
            this.BSphere = boundingSphere.Transform(this.TransformationMatrix);
            this.BType = BoundingType.Sphere;
        }

        public void CreateBoundingBoxForModel()
        {
            // Create variables to keep min and max xyz values for the model
            Vector3 modelMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 modelMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                //Create variables to hold min and max xyz values for the mesh
                Vector3 meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                Vector3 meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                // There may be multiple parts in a mesh (different materials etc.) so loop through each
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    // The stride is how big, in bytes, one vertex is in the vertex buffer
                    int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                    byte[] vertexData = new byte[stride * part.NumVertices];
                    part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, 1);

                    // Find minimum and maximum xyz values for this mesh part
                    // We know the position will always be the first 3 float values of the vertex data
                    Vector3 vertPosition = new Vector3();
                    for (int ndx = 0; ndx < vertexData.Length; ndx += stride)
                    {
                        vertPosition.X = BitConverter.ToSingle(vertexData, ndx);
                        vertPosition.Y = BitConverter.ToSingle(vertexData, ndx + sizeof(float));
                        vertPosition.Z = BitConverter.ToSingle(vertexData, ndx + sizeof(float) * 2);

                        // update our running values from this vertex
                        meshMin = Vector3.Min(meshMin, vertPosition);
                        meshMax = Vector3.Max(meshMax, vertPosition);
                    }
                }

                // transform by mesh bone transforms
                meshMin = Vector3.Transform(meshMin, this.Model.Bones[mesh.ParentBone.Index].Transform);
                meshMax = Vector3.Transform(meshMax, this.Model.Bones[mesh.ParentBone.Index].Transform);

                // Expand model extents by the ones from this mesh
                modelMin = Vector3.Min(modelMin, meshMin);
                modelMax = Vector3.Max(modelMax, meshMax);
            }

            // Create and return the model bounding box
            this.BBox = new BoundingBox(modelMin, modelMax);
            this.BType = BoundingType.Box;
        }

        public void UpdateBoundary()
        {
            if (this.BType == BoundingType.Box)
            {
                Vector3 min = this.BBox.Min;
                Vector3 max = this.BBox.Max;
                Vector3 center = new Vector3((min.X + max.X) / 2, (min.Y + max.Y) / 2, (min.Z + max.Z) / 2);
                Vector3 translation = this.TransformationMatrix.Translation - center;

                min += translation;
                max += translation;
                this.BBox = new BoundingBox(min, max);
            }
            else
            {
                Vector3 center = this.BSphere.Center;
                Vector3 translation = this.TransformationMatrix.Translation - center;
                this.BSphere.Center = center + translation;
            }
        }

        public virtual void HandleCollisionWith(GameObject obj2)
        {
            obj2.IsStatic = true;
            this.IsStatic = true;
        }
    }
}

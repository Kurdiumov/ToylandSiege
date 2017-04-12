using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public Dictionary<string, GameObject> Childs = new Dictionary<string, GameObject>();

        public Matrix TransformationMatrix;

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
            foreach (var child in Childs.Values)
            {
                child.Draw();
            }
        }

        public void AddChild(GameObject obj)
        {
            Childs.Add(obj.Name, obj);
            obj.Parent = this;
            Logger.Log.Debug("GameObject " + obj.Name + " added to " + Name + " GameObject type: " + Type);
        }

        public bool RemoveChild(GameObject obj)
        {
            if (RemoveChild(obj.Name))
            {
                Logger.Log.Debug("GameObject " + obj.Name + " removed from " + Name + " GameObject type: " + Type);
                return true;
            }
            Logger.Log.Debug("Cant remove GameObject " + obj.Name + " from " + Name + " GameObject type: " + Type);
            return false;
        }
    

        public bool RemoveChild(string name)
        {
            if (Childs.Remove(name))
            {
                Logger.Log.Debug("GameObject " + name + " removed from scene");
                return true;
            }
            return false;
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
            foreach (var child in obj.Childs.Values)
            {
                childs.Add(child);
                childs = childs.Concat(GetAllChilds(child)).ToList();
            }
            return childs;
        }

        public BoundingSphere CreateBoundingSphereForModel()
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
            return boundingSphere.Transform(this.TransformationMatrix);
        }

        public void HandleCollisionWith(GameObject obj2)
        {
            obj2.IsStatic = true;
            this.IsStatic = true;
        }
    }
}

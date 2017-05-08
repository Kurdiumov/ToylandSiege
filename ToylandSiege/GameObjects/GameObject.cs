using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework.Audio;
using SkinnedModel;

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
        public bool IsAnimated = false;

        public Model Model;
        public GameObject Parent = null;
        public Dictionary<string, GameObject> Childs = new Dictionary<string, GameObject>();

        public Matrix TransformationMatrix;

        public Collider Collider;

        public Dictionary<string, AnimationClip> Clips = new Dictionary<string, AnimationClip>();
        public Effect effect;
        protected GameObject()
        {
            this.Collider = new Collider(this);
            effect = ShaderManager.Get("LightShader");
        }

        protected abstract void Initialize();
        public abstract void Update(GameTime gameTime);

        public virtual void Draw()
        {
            if (!IsEnabled)
                return;
            if (Model != null)
            {
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    if (mesh.Effects.All(e => !(e is SkinnedEffect)))
                    {
                            DrawModelWithEffect();
                      /*  
                        
                            foreach (BasicEffect effect in mesh.Effects)
                            {
                                if (ToylandSiege.GetInstance().configurationManager.LigthningEnabled)
                                    effect.EnableDefaultLighting();
                                effect.AmbientLightColor = new Vector3(0, 0.3f, 0.3f);
                                effect.View = Camera.GetCurrentCamera().ViewMatrix;
                                effect.PreferPerPixelLighting = true;

                                effect.World = TransformationMatrix;
                                effect.Projection = Camera.GetCurrentCamera().ProjectionMatrix;
                                GlobalLightning.DrawGlobalLightning(effect);
                            }
                        */

                    }
                    else if (AnimationPlayer != null && mesh.Effects.All(e => e is SkinnedEffect))
                    {
                        Matrix[] bones = AnimationPlayer.GetSkinTransforms();

                        foreach (SkinnedEffect effect in mesh.Effects)
                        {
                            (effect).SetBoneTransforms(bones);
                            if (ToylandSiege.GetInstance().configurationManager.LigthningEnabled)
                                effect.EnableDefaultLighting();
                            effect.AmbientLightColor = new Vector3(0, 0.3f, 0.3f);
                            effect.View = Camera.GetCurrentCamera().ViewMatrix;

                            effect.World = TransformationMatrix;
                            effect.Projection = Camera.GetCurrentCamera().ProjectionMatrix;
                            GlobalLightning.DrawGlobalLightning(effect);
                        }
                    }
                    mesh.Draw();
                }
            }
            foreach (var child in Childs.Values)
            {
                child.Draw();
            }
        }

        private void DrawModelWithEffect()
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(Camera.GetCurrentCamera().World * TransformationMatrix);
                    effect.Parameters["View"].SetValue(Camera.GetCurrentCamera().ViewMatrix);
                    effect.Parameters["Projection"].SetValue(Camera.GetCurrentCamera().ProjectionMatrix);
                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * Camera.GetCurrentCamera().World));
                    effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                    

                    var viewVector = Vector3.Transform(Camera.GetCurrentCamera().Direction, Matrix.CreateRotationY(0));
                    viewVector.Normalize();
                    effect.Parameters["ViewVector"].SetValue(viewVector);

                }
                mesh.Draw();
            }
        }
        

        public AnimationPlayer AnimationPlayer;
        public void AddChild(GameObject obj)
        {
            Childs.Add(obj.Name, obj);
            obj.Parent = this;
            if (!(obj is Field)) // For better performance. Dont want to write to log 70+ objects
                Logger.Log.Debug("GameObject " + obj.Name + " added to " + Name + " GameObject type: " + Type);
        }

        public bool RemoveChild(GameObject obj)
        {
            //TODO: should check all childs recursively
            if (RemoveChild(obj.Name))
            {
                Logger.Log.Debug("GameObject " + obj.Name + " removed from " + Name + " GameObject type: " + Type);
                return true;
            }
            return false;
        }


        public bool RemoveChild(string name)
        {
            //TODO: should check all childs recursively
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

        public Matrix CreateTransformationMatrix()
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

        public virtual void HandleCollisionWith(GameObject obj2)
        {
            obj2.IsStatic = true;
            this.IsStatic = true;
        }

        public void Destroy()
        {
            Parent.RemoveChild(this);
        }
    }
}

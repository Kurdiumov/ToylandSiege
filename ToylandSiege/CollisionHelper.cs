using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToylandSiege.GameObjects;
using static ToylandSiege.GameObjects.GameObject;

namespace ToylandSiege
{
    class CollisionHelper
    {
        public static bool CheckCollision(GameObject obj1, GameObject obj2)
        {
            if (!obj1.IsEnabled || !obj1.IsCollidable)
                return false;
            if (!obj2.IsEnabled || !obj2.IsCollidable)
                return false;

            if (obj1.BType == BoundingType.Box)
            {
                if (obj2.BType == BoundingType.Box)
                {
                    if (obj1.BBox.Intersects(obj2.BBox)) {
                        OnCollisionDetected(obj1, obj2);
                        return true;
                    }
                }
                else
                {
                    if (obj1.BBox.Intersects(obj2.BSphere))
                    {
                        OnCollisionDetected(obj1, obj2);
                        return true;
                    }
                }
            }
            else
            {
                if (obj2.BType == BoundingType.Box)
                {
                    if (obj1.BSphere.Intersects(obj2.BBox))
                    {
                        OnCollisionDetected(obj1, obj2);
                        return true;
                    }
                }
                else
                {
                    if (obj1.BSphere.Intersects(obj2.BSphere))
                    {
                        OnCollisionDetected(obj1, obj2);
                        return true;
                    }
                }
            }

            return false;
        }

        private static void OnCollisionDetected(GameObject obj1, GameObject obj2)
        {
            obj1.HandleCollisionWith(obj2);
            obj2.HandleCollisionWith(obj1);
        }

        public static void CalculateCollisions()
        {
            List<GameObject> dynamicObjects = new List<GameObject>();

            foreach (var obj in ToylandSiege.CurrentLevel.RootGameObject.Childs.Values)
            {
                if (obj.IsEnabled && !obj.IsStatic && obj.IsCollidable)
                {
                    dynamicObjects.Add(obj);
                    // TODO: Add recalculation of Box to GameObject.Rotate (BB must be parallel to axis)
                    // others to Constructor only
                    obj.UpdateBoundary();
                }
            }

            int currIndex = 0;

            foreach (var obj1 in dynamicObjects)
            {
                foreach (var obj2 in dynamicObjects.Skip(currIndex + 1))
                    CheckCollision(obj1, obj2);
                currIndex++;
            }
        }

        public static void DrawBoundingSphere(BoundingSphere bs)
        {

        }
    }
}

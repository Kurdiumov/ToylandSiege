using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace ToylandSiege
{
    public static class JSONHelper
    {
        #region Helpers
        public static Vector3 ParseVector3(JToken currentObject)
        {
            List<float> arr = new List<float>();
            foreach (var item in currentObject)
            {

                float result = 0;
                if (!float.TryParse(item.ToString(), out result))
                {
                    Logger.Log.Error("Error while parsing " + currentObject.ToString() + " (" + item.ToString() + ") Replacing value with 0");
                }
                arr.Add(result);
            }

            if (arr.Count == 3)
            {
                return new Vector3(arr[0], arr[1], arr[2]);
            }
            throw new ArgumentException("Arguments count should equal 3");
        }

        public static string GetValue(string value, JObject obj)
        {
            if (ValueExist(value, obj))
                return obj.GetValue(value).ToString();
            throw new ArgumentException("Value " + value + " does not exist in current context. " + obj.ToString());
        }

        public static bool ValueExist(string value, JObject obj)
        {
            try
            {
                var exists = !String.IsNullOrEmpty(obj.GetValue(value).ToString());
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool ToBool(string value)
        {
            if (value == "True")
                return true;
            return false;
        }
        #endregion
    }
}

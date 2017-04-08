using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class DebugUtilities
    {
        public static void ShowAllGameObjects(GameObject rootObject, int tabulation = 0)
        {
            string output = "";
            for (int i = 0; i <= tabulation; i++)
                output += "\t";

            System.Diagnostics.Debug.Print(output + rootObject);
            foreach (var child in rootObject.Childs)
            {
                ShowAllGameObjects(child, tabulation+1);
            }
        }
    }
}

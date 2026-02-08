using UnityEngine;

namespace Dlcs
{
    public class Extensions
    {
        public static GameObject GetChildByName(GameObject parent, string childName)
        {
            var children = parent.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child.name == childName)
                {
                    return child.gameObject;
                }
            }
            return null;
        }
    }
}
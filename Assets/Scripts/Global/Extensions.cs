using UnityEngine;

namespace Dlcs
{
    public static class Extensions
    {
        public static GameObject GetChildByName(this GameObject parent, string childName)
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
        
        public static GameObject GetChildByNameRecursively(this GameObject parent, string childName)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.name == childName)
                {
                    return child.gameObject;
                }
                var result = GetChildByNameRecursively(child.gameObject, childName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
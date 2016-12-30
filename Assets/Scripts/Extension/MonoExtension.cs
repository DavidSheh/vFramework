using UnityEngine;

namespace vFramework.Extension
{
    public static class MonoExtension
    {
        public static void ActiveObj(this GameObject go, bool active)
        {
            if (null == go)
            {
                Debug.LogWarning("GameObject is null!");
                return;
            }

            if (go.activeSelf != active)
            {
                go.SetActive(active);
            }
        }

        public static void ActiveObj(this Transform obj, bool active)
        {
            if (null == obj)
            {
                Debug.LogWarning("Transform is null!");
                return;
            }

            if (obj.gameObject.activeSelf != active)
            {
                obj.gameObject.SetActive(active);
            }
        }

        public static void SetParentObj(this Transform obj, Transform parent)
        {
            obj.SetParent(parent);
            obj.localPosition = Vector3.zero;
            obj.localRotation = Quaternion.identity;
            obj.localScale = Vector3.one;
        }
    }
}

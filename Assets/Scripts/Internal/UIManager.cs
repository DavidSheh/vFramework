using UnityEngine;
using System.Collections.Generic;

namespace vFramework.Internal
{
    public static class UIManager
    {
        public static List<string> backViewList = new List<string>();
        public static Dictionary<string, BaseView> viewDic = new Dictionary<string, BaseView>();

        public static void ShowView(string name)
        {
            BaseView view = null;
            if (!viewDic.TryGetValue(name, out view))
            {
                view = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(name) as BaseView;

                viewDic[name] = view;
            }

            if (view.ViewMode == UIMode.NeedBack && !string.Equals(PeekView(), name))
            {
                backViewList.Add(name);
            }

            view.Show();
        }

        public static void HideView()
        {
            string name = PeekView();
            BaseView view = null;
            if (viewDic.TryGetValue(name, out view))
            {
                if (view.ViewMode == UIMode.NeedBack)
                {
                    backViewList.Remove(name);
                }
                view.Hide();
            }
            else
            {
                Debug.LogWarning("No need-back view!");
            }
        }

        public static void HideView(string name)
        {
            BaseView view = null;
            if (!viewDic.TryGetValue(name, out view))
            {
                Debug.LogWarning(name + " view is not exit!");
                return;
            }

            if (view.ViewMode == UIMode.NeedBack)
            {
                backViewList.Remove(name);
            }
            view.Hide();
        }

        public static void DestroyView(string name)
        {
            BaseView view = null;
            if (!viewDic.TryGetValue(name, out view))
            {
                Debug.LogWarning(name + " is not exit!");
                return;
            }

            viewDic.Remove(name);
            view.Destroy();
        }

        public static bool IsViewActive()
        {
            return false;
        }

        public static void Clear()
        {

        }

        private static string PeekView()
        {
            string top = "";
            int count = backViewList.Count;
            if (count > 0)
            {
                top = backViewList[count - 1];
            }

            return top;
        }
    }
}

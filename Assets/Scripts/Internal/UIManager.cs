using UnityEngine;
using System.Collections.Generic;

namespace vFramework.Internal
{
    public static class UIManager
    {
        public static List<string> backViewList = new List<string>();
        public static Dictionary<string, BaseView> viewDic = new Dictionary<string, BaseView>();

        /// <summary>
        /// 显示指定界面，也可以通过此方法找到指定界面
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void ShowView(string name)
        {
            BaseView view = null;
            if (!viewDic.TryGetValue(name, out view))
            {
                view = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(name) as BaseView;

                viewDic[name] = view;
            }

            if ((view.ViewMode == ViewMode.NeedBack || view.ViewMode == ViewMode.OnlyBack)
               && !string.Equals(PeekView(), name))
            {
                backViewList.Add(name);
            }

            if (view.ViewMode == ViewMode.NeedBack && view.ViewMode != ViewMode.None)
            {
                CloseOthers(name);
            }

            view.Show();
        }

        /// <summary>
        /// 关闭指定界面
        /// </summary>
        /// <param name="name"></param>
        public static void HideView(string name)
        {
            BaseView view = null;
            if (!viewDic.TryGetValue(name, out view))
            {
                Debug.LogWarning(name + " view is not exit!");
                return;
            }

            Close(view);
        }

        /// <summary>
        /// 隐藏最上层的界面
        /// </summary>
        public static void HideView()
        {
            string name = PeekView();
            BaseView view = null;
            if (viewDic.TryGetValue(name, out view))
            {
                Close(view);
            }
            else
            {
                Debug.LogWarning("No need-back view!");
            }
        }

        /// <summary>
        /// 找到指定界面
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static BaseView FindView(string name)
        {
            BaseView view = null;
            
            if (!viewDic.TryGetValue(name, out view))
            {
                Debug.LogWarning("Dont contain the view. Name: " + name);
            }

            return view;
        }
        
        /// <summary>
        /// 销毁指定界面
        /// </summary>
        /// <param name="name"></param>
        public static void DestroyView(string name)
        {
            BaseView view = null;
            if (!viewDic.TryGetValue(name, out view))
            {
                Debug.LogWarning(name + " is not exit!");
                return;
            }

            viewDic.Remove(name);
            backViewList.Remove(name);
            view.Destroy();
        }

        /// <summary>
        /// 隐藏所有界面
        /// </summary>
        public static void HideAll()
        {
            CloseOthers();
        }

        /// <summary>
        /// 清除所有界面
        /// </summary>
        public static void Clear()
        {
            CloseOthers();
            backViewList.Clear();
            viewDic.Clear();
        }

        #region private functions
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

        /// <summary>
        /// 关闭其他界面，默认关闭所有
        /// </summary>
        /// <param name="name"></param>
        private static void CloseOthers(string name = "")
        {
            foreach (var item in viewDic)
            {
                if(!string.Equals(item.Key, name))
                {
                    Close(item.Value);
                }
            }
        }

        private static void Close(BaseView view)
        {
            string name = view.GetType().ToString();
            if (view.ViewMode == ViewMode.NeedBack || view.ViewMode == ViewMode.OnlyBack)
            {
                backViewList.Remove(name);
            }
            view.Hide();
        }
        #endregion
    }
}

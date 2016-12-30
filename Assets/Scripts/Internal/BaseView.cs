using UnityEngine;

namespace vFramework.Internal
{
    public enum UIType
    {
        /// <summary>
        /// 没有根节点
        /// </summary>
        NoRoot,
        /// <summary>
        /// 2D UI根节点
        /// </summary>
        ScreenRoot,
        /// <summary>
        /// 3D UI根节点
        /// </summary>
        WorldRoot,
        /// <summary>
        /// 跟踪根节点，VR模式下视野的中心点
        /// </summary>
        TraceRoot,
        /// <summary>
        /// 跟随根节点，VR模式下，跟随头部移动（在竖直方向上不跟随，只在水平方向上跟随）
        /// </summary>
        FollowRoot,
    }

    public enum UIMode
    {
        DoNothing,
        HideOther,     // 闭其他界面
        NeedBack,      // 点击返回按钮关闭当前,不关闭其他界面(需要调整好层级关系)
        NoNeedBack,    // 关闭TopBar,关闭其他界面,不加入backSequence队列
    }

    public abstract class BaseView
    {
        public UIType ViewType { get; private set; }
        public UIMode ViewMode { get; private set; }
        public bool IsActive { get; private set; }

        private string ViewPath;
        protected GameObject viewObject;

        public BaseView() { }
        public BaseView(string uiPath, UIType type, UIMode mode)
        {
            ViewPath = uiPath;
            ViewType = type;
            ViewMode = mode;
        }

        public void Show()
        {
            if (null == viewObject)
            {
                CreateUI();

                InitView();
            }

            if (null != viewObject && !viewObject.activeSelf)
            {
                IsActive = true;
                viewObject.SetActive(IsActive);
            }

            //OnShow();
        }

        public void Hide()
        {
            if (null != viewObject && viewObject.activeSelf)
            {
                //OnHide();
                IsActive = false;
                viewObject.SetActive(IsActive);
            }
        }

        public void Destroy()
        {
            //OnDestroy();
            viewObject = null;
        }

        protected abstract void InitView();

        //protected abstract void OnShow();

        //protected abstract void OnHide();

        //protected abstract void OnDestroy();

        private void CreateUI()
        {
            if (!string.IsNullOrEmpty(ViewPath))
            {
                GameObject obj = Resources.Load<GameObject>(ViewPath);
                viewObject = Object.Instantiate(obj);
            }

            AnchorUI();
        }

        private void AnchorUI()
        {
            if (null == viewObject)
            {
                return;
            }

            UIRoot.Instance.SetRoot(viewObject.transform, ViewType);
        }
    }
}
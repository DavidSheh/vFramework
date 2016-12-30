using UnityEngine;

namespace vFramework.Internal
{
    /// <summary>
    /// 界面类型
    /// </summary>
    public enum ViewType
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

    /// <summary>
    /// 界面模式
    /// </summary>
    public enum ViewMode
    {
        /// <summary>
        /// 不入栈且不关闭其他界面
        /// </summary>
        None,
        /// <summary>
        /// 入栈且关闭其他界面
        /// </summary>
        NeedBack,
        /// <summary>
        /// 入栈且不关闭其他界面
        /// </summary>    
        OnlyBack,
    }

    public abstract class BaseView
    {
        public ViewType ViewType { get; private set; }
        public ViewMode ViewMode { get; private set; }
        public bool IsActive { get; private set; }

        private string ViewPath;
        protected GameObject viewObject;

        public BaseView() { }
        public BaseView(string uiPath, ViewType type, ViewMode mode)
        {
            ViewPath = uiPath;
            ViewType = type;
            ViewMode = mode;
        }

        ~BaseView()
        {

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

            OnShow();
        }

        public void Hide()
        {
            if (null != viewObject && viewObject.activeSelf)
            {
                OnHide();
                IsActive = false;
                viewObject.SetActive(IsActive);
            }
        }

        public void Destroy()
        {
            OnDestroy();
            Object.Destroy(viewObject);
            viewObject = null;
        }

        protected abstract void InitView();

        protected virtual void OnShow() { }

        protected virtual void OnHide() { }

        protected virtual void OnDestroy() { }

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
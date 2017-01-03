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
        public string ViewName { get; private set; }
        public ViewType ViewType { get; private set; }
        public ViewMode ViewMode { get; private set; }
        public bool IsActive { get; private set; }

        private string ViewPath;
        protected GameObject viewObject;

        public BaseView() { }
        public BaseView(string uiPath, ViewType type, ViewMode mode)
        {
            ViewName = this.GetType().ToString();
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
                IsActive = true;
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
            IsActive = false;
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

            Vector3 anchorPos = Vector3.zero;
            Vector2 sizeDel = Vector2.zero;
            Vector3 scale = Vector3.one;

            RectTransform rt = viewObject.GetComponent<RectTransform>();
            if (null != rt)
            {
                anchorPos = rt.anchoredPosition;
                sizeDel = rt.sizeDelta;
                scale = rt.localScale;
            }
            else
            {
                anchorPos = viewObject.transform.localPosition;
                scale = viewObject.transform.localScale;
            }

            UIRoot.Instance.SetRoot(viewObject.transform, ViewType);

            if (null != rt)
            {
                viewObject.transform.localPosition = anchorPos;
                rt.anchoredPosition = anchorPos;
                rt.sizeDelta = sizeDel;
                rt.localScale = scale;
            }
            else
            {
                viewObject.transform.localPosition = anchorPos;
                viewObject.transform.localScale = scale;
            }
        }
    }
}
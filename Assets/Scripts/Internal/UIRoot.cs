using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using vFramework.Common;
using vFramework.Extension;

namespace vFramework.Internal
{
    public class UIRoot : Singleton<UIRoot>
    {
        #region fields
        private GameObject screenRoot; // 2D UI的Root
        private GameObject worldRoot; // 3D UI的Root
        private GameObject traceRoot; // 视线追踪的Root
        private GameObject followRoot; // 视线跟随的Root

        private Transform headObj;
        private Vector3 direction;
        private float currentYaw = 0;
        private float previousYaw = 0;

        private bool triggerState;
        #endregion

        #region properties
        public static bool IsCurvedUI { get; set; }

        /// <summary>
        /// 水平方向最大活动角度
        /// </summary>
        public static float MaxYaw { private get; set; }

        public static bool LockYaw { private get; set; }

        public static float LookAngle { private get; set; }

        public static bool Recenter { private get; set; }
        #endregion

        #region events
        public static Action<bool> OnTriggerAngle;
        #endregion

        #region override functions
        protected override void Awake()
        {
            base.Awake();
            InitRoot();

            LookAngle = 30f;// 默认30度触发低头显示控制栏
        }

        void Start()
        {

        }

        void LateUpdate()
        {
            UpdateRoot();
        }

        protected override void OnDestroy()
        {
            UIManager.Clear();
        }
        #endregion

        #region public functions

        public void SetHead(Transform head)
        {
            this.headObj = head;
        }

        public void SetRoot(Transform target, ViewType type)
        {
            if (ViewType.ScreenRoot == type)
            {
                target.SetParentObj(screenRoot.transform);
            }
            else if (ViewType.WorldRoot == type)
            {
                target.SetParentObj(worldRoot.transform);
            }
            else if (ViewType.TraceRoot == type)
            {
                target.SetParentObj(traceRoot.transform);
            }
            else if (ViewType.FollowRoot == type)
            {
                target.SetParentObj(followRoot.transform);
            }
        }

        public void ActiveEventSystem(bool active)
        {
            if (null == eventSystemObj && active)
            {
                SetupEventSystem();
            }

            eventSystemObj.ActiveObj(active);
        }
        #endregion

        GameObject eventSystemObj;
        private void SetupEventSystem()
        {
            //add Event System
            eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.layer = LayerMask.NameToLayer("UI");
            eventSystemObj.transform.SetParent(transform);
            eventSystemObj.AddComponent<EventSystem>();

            if (!Application.isMobilePlatform || Application.isEditor)
            {
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }
            else
            {
                eventSystemObj.AddComponent<TouchInputModule>();
            }
        }

        #region private functions
        private void InitRoot()
        {
            gameObject.name = "UIRoot";

            screenRoot = CreateRoot("ScreenRoot", RenderMode.ScreenSpaceCamera, 0);
            worldRoot = CreateRoot("WorldRoot", RenderMode.WorldSpace, 0);
            worldRoot.transform.localScale = Constant.defaultScale;
            followRoot = CreateRoot("FollowRoot", RenderMode.WorldSpace, 0);
            traceRoot = CreateRoot("TraceRoot", RenderMode.WorldSpace, 500);
        }

        private GameObject CreateRoot(string name, RenderMode mode, int sort)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(transform);
            go.layer = LayerMask.NameToLayer("UI");

            AddRectTransform(go);

            AddCanvas(go, mode, sort);

            CanvasScaler cs = go.AddComponent<CanvasScaler>();
            if (RenderMode.ScreenSpaceCamera == mode)
            {
                cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cs.referenceResolution = Constant.defaultResolution;
                cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            }

            if (sort < 500)
            {
                go.AddComponent<GraphicRaycaster>();
            }

            return go;
        }

        private RectTransform AddRectTransform(GameObject go)
        {
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Constant.size;

            return rt;
        }

        private Canvas AddCanvas(GameObject go, RenderMode mode, int sort)
        {
            Canvas can = go.AddComponent<Canvas>();
            can.renderMode = mode;
            can.pixelPerfect = true;
            can.overrideSorting = true;
            can.sortingOrder = sort;

            return can;
        }

        private void UpdateRoot()
        {
            if (null == headObj)
            {
                return;
            }

            if (LockYaw)
            {
                direction = headObj.rotation * Vector3.forward;
            }
            else
            {
                direction = headObj.forward;
            }

            float deviceYaw = (Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.z) + 360) % 360;
            float devicePitch = Mathf.Rad2Deg * Mathf.Asin(direction.y);

            float deltaYaw = Mathf.DeltaAngle(deviceYaw, previousYaw);
            float targetYaw = currentYaw + deltaYaw;

            previousYaw = deviceYaw;
            Quaternion deviceRotation = Quaternion.AngleAxis(deviceYaw, Vector3.up);

            currentYaw = Mathf.Clamp(targetYaw, -MaxYaw, MaxYaw);

            if (LockYaw)
            {
                followRoot.transform.position = headObj.position;
                followRoot.transform.localRotation = Quaternion.AngleAxis(-devicePitch, Vector3.right);
            }
            else
            {
                followRoot.transform.position = headObj.position;
                followRoot.transform.localRotation = deviceRotation * Quaternion.AngleAxis(currentYaw, Vector3.up);
            }

            traceRoot.transform.position = headObj.position;
            traceRoot.transform.localRotation = deviceRotation * Quaternion.AngleAxis(-devicePitch, Vector3.right);

            TriggerAngle(-devicePitch);
        }

        private void TriggerAngle(float angle)
        {
            bool trigger = angle > LookAngle;

            if (triggerState != trigger && null != OnTriggerAngle)
            {
                OnTriggerAngle(trigger);

                triggerState = trigger;

                if (Recenter && !LockYaw)
                {
                    Recenter = false;
                    currentYaw = 0;
                }
            }
        }
        #endregion
    }
}
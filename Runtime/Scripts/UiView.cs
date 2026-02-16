using serginian.UI.Animation;
using UnityEngine;

namespace serginian.UI
{
    /// <summary>
    /// Base class for all UI views that provides animation, visibility, and interaction management.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UiView: MonoBehaviour
    {
        /// <summary>
        /// Determines whether the view should be interactable when shown.
        /// </summary>
        [SerializeField, Tooltip("Determines whether the view should be interactable when shown")]
        protected bool isInteractable = true;

        /// <summary>
        /// Animation asset used for show/hide transitions.
        /// </summary>
        [Tooltip("Animation asset used for show/hide transitions")]
        public UiViewAnimation animationAsset;

        /// <summary>
        /// Gets the CanvasGroup component attached to this view.
        /// </summary>
        public CanvasGroup CanvasGroup => canvasGroup;

        /// <summary>
        /// Gets the RectTransform component attached to this view.
        /// </summary>
        public RectTransform RectTransform => rectTransform;

        /// <summary>
        /// Gets the size of the view based on its RectTransform.
        /// </summary>
        public Vector2 Size => new Vector2(rectTransform.rect.width, rectTransform.rect.height);

        /// <summary>
        /// Gets a value indicating whether the view is currently visible.
        /// </summary>
        public bool IsVisible { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the view is interactable.
        /// When set, also controls whether the view blocks raycasts.
        /// </summary>
        public bool IsInteractable
        {
            get => canvasGroup?.interactable ?? false;
            set
            {
                if (!canvasGroup)
                    return;

                canvasGroup.interactable = value;
                canvasGroup.blocksRaycasts = value;
            }
        }

        /// <summary>
        /// Cached reference to the CanvasGroup component.
        /// </summary>
        protected CanvasGroup canvasGroup;

        /// <summary>
        /// Cached reference to the RectTransform component.
        /// </summary>
        protected RectTransform rectTransform;
        
        
        
        /********************** MONO BEHAVIOUR **********************/

        /// <summary>
        /// Initializes component references.
        /// </summary>
        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
        }



        /********************** PUBLIC INTERFACE **********************/

        /// <summary>
        /// Shows the view immediately without playing any animation.
        /// Sets the view to visible and interactable state.
        /// </summary>
        public virtual void ShowWithoutAnimation()
        {
            if (!canvasGroup)
                return;

            animationAsset?.Show(this);
            canvasGroup.interactable = isInteractable;
            canvasGroup.blocksRaycasts = isInteractable;
            IsVisible = true;
        }

        /// <summary>
        /// Closes the view immediately without playing any animation.
        /// Sets the view to hidden and non-interactable state.
        /// </summary>
        public virtual void CloseWithoutAnimation()
        {
            if (!canvasGroup)
                return;

            animationAsset?.Close(this);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            IsVisible = false;
        }

        /// <summary>
        /// Shows the view with animation asynchronously without awaiting completion.
        /// </summary>
        public virtual void Show()
        {
            _ = ShowAsync();
        }

        /// <summary>
        /// Closes the view with animation asynchronously without awaiting completion.
        /// </summary>
        public virtual void Close()
        {
            _ = CloseAsync();
        }

        /// <summary>
        /// Shows the view with animation and waits for completion.
        /// If already visible or animation asset is not set, shows without animation.
        /// </summary>
        /// <returns>An awaitable task that completes when the show animation finishes.</returns>
        public virtual async Awaitable ShowAsync()
        {
            if (IsVisible)
                return;

            if (gameObject.activeInHierarchy && animationAsset)
            {
                IsVisible = true;
                await animationAsset.ShowAsync(this);
                IsInteractable = isInteractable;
            }
            else
            {
                ShowWithoutAnimation();
            }
        }

        /// <summary>
        /// Closes the view with animation and waits for completion.
        /// If already hidden or not active in hierarchy, closes without animation.
        /// </summary>
        /// <returns>An awaitable task that completes when the close animation finishes.</returns>
        public virtual async Awaitable CloseAsync()
        {
            if (!IsVisible)
                return;

            if (gameObject.activeInHierarchy)
            {
                if (!canvasGroup)
                    return;

                IsVisible = false;
                IsInteractable = false;

                if (animationAsset)
                    await animationAsset.HideAsync(this);
            }
            else
            {
                CloseWithoutAnimation();
            }
        }
        
    } // end of class
}
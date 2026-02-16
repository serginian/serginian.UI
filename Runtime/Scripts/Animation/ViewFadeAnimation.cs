using DG.Tweening;
using UnityEngine;

namespace serginian.UI.Animation
{
    /// <summary>
    /// Provides a fade in/out animation effect for UI views by animating the alpha value of the CanvasGroup.
    /// </summary>
    [CreateAssetMenu(menuName = "serginian/UI/Animation/Window Fade In-Out", fileName = "Window Fade In-Out", order = 0)]
    public class ViewFadeAnimation : UiViewAnimation
    {
        [Header("Fade In")]
        [SerializeField, Tooltip("Duration of the fade-in animation in seconds.")]
        private float showDuration = 1f;

        [SerializeField, Tooltip("Easing function for the fade-in animation.")]
        private Ease showEase = Ease.Linear;

        [Header("Fade Out")]
        [SerializeField, Tooltip("Duration of the fade-out animation in seconds.")]
        private float hideDuration = 1f;

        [SerializeField, Tooltip("Easing function for the fade-out animation.")]
        private Ease hideEase = Ease.Linear;



        /********************** PUBLIC INTERFACE **********************/

        /// <summary>
        /// Shows the view by fading in the CanvasGroup alpha from 0 to 1.
        /// </summary>
        /// <param name="window">The view to show.</param>
        /// <returns>An awaitable task that completes when the fade-in animation finishes.</returns>
        public override async Awaitable ShowAsync(UiView window)
        {
            window.CanvasGroup.DOKill();
            await window.CanvasGroup.DOFade(1f, showDuration).SetEase(showEase).SetUpdate(true).AsyncWaitForCompletion();
        }

        /// <summary>
        /// Hides the view by fading out the CanvasGroup alpha from 1 to 0.
        /// </summary>
        /// <param name="window">The view to hide.</param>
        /// <returns>An awaitable task that completes when the fade-out animation finishes.</returns>
        public override async Awaitable HideAsync(UiView window)
        {
            window.CanvasGroup.DOKill();
            await window.CanvasGroup.DOFade(0f, hideDuration).SetEase(hideEase).SetUpdate(true).AsyncWaitForCompletion();
        }

        /// <summary>
        /// Closes the view immediately by setting the CanvasGroup alpha to 0 without animation.
        /// </summary>
        /// <param name="window">The view to close.</param>
        public override void Close(UiView window)
        {
            window.CanvasGroup.DOKill();
            window.CanvasGroup.alpha = 0;
        }

        /// <summary>
        /// Shows the view immediately by setting the CanvasGroup alpha to 1 without animation.
        /// </summary>
        /// <param name="window">The view to show.</param>
        public override void Show(UiView window)
        {
            window.CanvasGroup.DOKill();
            window.CanvasGroup.alpha = 1;
        }

    } // end of class
}
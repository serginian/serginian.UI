using DG.Tweening;
using UnityEngine;

namespace serginian.UI.Animation
{
    /// <summary>
    /// Defines the direction for slide animations.
    /// </summary>
    public enum AnimationDirection
    {
        /// <summary>
        /// Slide animation moves from left to right.
        /// </summary>
        LeftToRight,

        /// <summary>
        /// Slide animation moves from right to left.
        /// </summary>
        RightToLeft,

        /// <summary>
        /// No slide animation direction.
        /// </summary>
        None
    }

    /// <summary>
    /// Provides a slide in/out animation effect for UI views that combines sliding and fading.
    /// Extends <see cref="ViewFadeAnimation"/> to add horizontal sliding motion.
    /// </summary>
    [CreateAssetMenu(menuName = "serginian/UI/Animation/Window Slide In-Out", fileName = "Window Slide In-Out", order = 0)]
    public class ViewSlideAnimation : ViewFadeAnimation
    {
        [Header("Slide In")]
        [SerializeField, Tooltip("Duration of the slide animation in seconds.")]
        private float slideDuration = 1f;

        [SerializeField, Tooltip("Easing function for the slide animation.")]
        private Ease slideEase = Ease.OutCirc;

        [SerializeField, Tooltip("Direction of the slide animation.")]
        private AnimationDirection direction = AnimationDirection.RightToLeft;



        /********************** PUBLIC INTERFACE **********************/

        /// <summary>
        /// Shows the view by sliding it in from the specified direction while fading in.
        /// The view starts off-screen and slides to its target position.
        /// </summary>
        /// <param name="window">The view to show.</param>
        /// <returns>An awaitable task that completes when the slide-in animation finishes.</returns>
        public override async Awaitable ShowAsync(UiView window)
        {
            float startPos = 0f;
            switch (direction)
            {
                case AnimationDirection.LeftToRight: startPos = -window.Size.x; break;
                case AnimationDirection.RightToLeft: startPos = window.Size.x; break;
            }
            window.RectTransform.anchoredPosition = new Vector2(startPos, 0);

            _ = base.ShowAsync(window);
            window.RectTransform.DOKill();
            await window.RectTransform.DOAnchorPosX(0f, slideDuration).SetEase(slideEase).SetUpdate(true).AsyncWaitForCompletion();
        }

        /// <summary>
        /// Hides the view by sliding it out in the specified direction while fading out.
        /// The view slides off-screen to the opposite side from which it entered.
        /// </summary>
        /// <param name="window">The view to hide.</param>
        /// <returns>An awaitable task that completes when the slide-out animation finishes.</returns>
        public override async Awaitable HideAsync(UiView window)
        {
            window.RectTransform.DOKill();

            float targetPos = 0f;
            switch (direction)
            {
                case AnimationDirection.LeftToRight: targetPos = window.Size.x; break;
                case AnimationDirection.RightToLeft: targetPos = -window.Size.x; break;
            }

            await window.RectTransform.DOAnchorPosX(targetPos, slideDuration).SetEase(slideEase).SetUpdate(true).AsyncWaitForCompletion();
            _ = base.HideAsync(window);
        }

        /// <summary>
        /// Closes the view immediately without animation.
        /// Calls the base fade close behavior and kills any active slide tweens.
        /// </summary>
        /// <param name="window">The view to close.</param>
        public override void Close(UiView window)
        {
            base.Close(window);
            window.RectTransform.DOKill();
        }

        /// <summary>
        /// Shows the view immediately without animation.
        /// Calls the base fade show behavior and resets the position to center.
        /// </summary>
        /// <param name="window">The view to show.</param>
        public override void Show(UiView window)
        {
            base.Show(window);
            window.RectTransform.DOKill();
            window.RectTransform.anchoredPosition = new Vector2(0, 0);
        }

    } // end of class
}
using DG.Tweening;
using UnityEngine;

namespace serginian.UI.Animation
{
    /// <summary>
    /// Provides a punch animation effect for UI buttons that scales the button on hover and click.
    /// </summary>
    [CreateAssetMenu(menuName = "serginian/UI/Animation/Button Punch", fileName = "Button Punch", order = 1)]
    public class ButtonPunchAnimation : UiButtonAnimation
    {
        [Header("Animation")]
        [SerializeField, Tooltip("Scale factor when button is clicked.")]
        private float clickScale = 1.05f;

        [SerializeField, Tooltip("Scale factor when button is hovered.")]
        private float hoverScale = 1.02f;

        [SerializeField, Tooltip("Default scale factor when button is in normal state.")]
        private float defaultScale = 1f;

        [SerializeField, Tooltip("Duration of the click animation in seconds.")]
        private float clickDuration = 0.2f;

        [SerializeField, Tooltip("Duration of the hover animation in seconds.")]
        private float hoverDuration = 1f;

        [SerializeField, Tooltip("Easing function for click animation.")]
        private Ease clickEase = Ease.InOutQuad;

        [SerializeField, Tooltip("Easing function for hover animation.")]
        private Ease hoverEase = Ease.InOutBounce;



        /********************** PUBLIC INTERFACE **********************/

        /// <summary>
        /// Animates the button with a punch effect when clicked.
        /// Scales up to the click scale, then returns to default scale.
        /// </summary>
        /// <param name="button">The button to animate.</param>
        /// <returns>An awaitable task that completes when the animation finishes.</returns>
        public override async Awaitable Click(UiButton button)
        {
            float halfDuration = clickDuration * 0.5f;
            var targetTransform = button.RectTransform;
            targetTransform.DOKill();
            await targetTransform.DOScale(clickScale, halfDuration).SetEase(clickEase).SetUpdate(true).AsyncWaitForCompletion();
            targetTransform.DOScale(defaultScale, halfDuration).SetEase(clickEase).SetUpdate(true);
        }

        /// <summary>
        /// Animates the button when the pointer enters (hover).
        /// Scales the button to the hover scale.
        /// </summary>
        /// <param name="button">The button to animate.</param>
        /// <returns>An awaitable task.</returns>
        public override Awaitable Enter(UiButton button)
        {
            var targetTransform = button.RectTransform;
            targetTransform.DOKill();
            targetTransform.DOScale(hoverScale, hoverDuration).SetEase(hoverEase).SetUpdate(true);
            return default;
        }

        /// <summary>
        /// Animates the button when the pointer leaves (hover exit).
        /// Returns the button to the default scale.
        /// </summary>
        /// <param name="button">The button to animate.</param>
        /// <returns>An awaitable task.</returns>
        public override Awaitable Leave(UiButton button)
        {
            var targetTransform = button.RectTransform;
            targetTransform.DOKill();
            targetTransform.DOScale(defaultScale, hoverDuration).SetUpdate(true);
            return default;
        }

    } // end of class
}
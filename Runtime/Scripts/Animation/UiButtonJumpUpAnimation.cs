using DG.Tweening;
using UnityEngine;

namespace serginian.UI.Animation
{
    /// <summary>
    /// Provides a jump-up animation effect for UI button selection that vertically translates the button position.
    /// </summary>
    [CreateAssetMenu(menuName = "serginian/UI/Animation/Button Jump Up", fileName = "Button Jump Up", order = 2)]
    public class UiButtonJumpUpAnimation : UiButtonSelectAnimation
    {
        [Header("Animation")]
        [SerializeField, Tooltip("Height to jump in pixels.")]
        private float jumpHeight = 20f;

        [SerializeField, Tooltip("Duration of the jump animation in seconds.")]
        private float jumpDuration = 0.5f;

        [SerializeField, Tooltip("Easing function for the jump animation.")]
        private Ease jumpEase = Ease.OutBounce;

        [SerializeField, Tooltip("Duration of the return animation in seconds.")]
        private float returnDuration = 0.3f;

        [SerializeField, Tooltip("Easing function for the return animation.")]
        private Ease returnEase = Ease.InOutQuad;



        /********************** PUBLIC INTERFACE **********************/

        /// <summary>
        /// Animates the button jumping up when selected.
        /// Moves the button vertically upward by the specified jump height.
        /// </summary>
        /// <param name="button">The button to animate.</param>
        /// <returns>An awaitable task.</returns>
        public override Awaitable Select(UiButton button)
        {
            var targetTransform = button.RectTransform;
            targetTransform.DOKill();

            targetTransform.DOAnchorPosY(targetTransform.anchoredPosition.y + jumpHeight, jumpDuration)
                .SetEase(jumpEase)
                .SetUpdate(true);

            return default;
        }

        /// <summary>
        /// Animates the button returning to its original position when deselected.
        /// Moves the button vertically downward to its initial position.
        /// </summary>
        /// <param name="button">The button to animate.</param>
        /// <returns>An awaitable task.</returns>
        public override Awaitable Deselect(UiButton button)
        {
            var targetTransform = button.RectTransform;
            targetTransform.DOKill();

            targetTransform.DOAnchorPosY(targetTransform.anchoredPosition.y - jumpHeight, returnDuration)
                .SetEase(returnEase)
                .SetUpdate(true);

            return default;
        }

    } // end of class
}
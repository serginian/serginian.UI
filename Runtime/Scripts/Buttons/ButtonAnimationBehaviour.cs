using System;
using serginian.UI.Animation;
using UnityEngine;

namespace serginian.UI.Buttons
{
    /// <summary>
    /// Implements animation-based behavior for UI buttons, triggering animations on hover, leave, and click events.
    /// </summary>
    [Serializable]
    public sealed class ButtonAnimationBehaviour : ICustomButtonBehaviour
    {
        [Tooltip("Animation to play when the button is clicked")]
        public UiButtonAnimation clickAnimation;

        [Tooltip("Animation to play when the button is hovered over")]
        public UiButtonAnimation hoverAnimation;

        [Tooltip("Animation to play when the hover leaves the button")]
        public UiButtonAnimation leaveAnimation;

        /// <inheritdoc/>
        public bool ExecuteWhenDisabled => false;

        /// <inheritdoc/>
        public void Initialize(UiButton button)
        {

        }

        /// <inheritdoc/>
        void ICustomButtonBehaviour.OnEnabled(UiButton button)
        {

        }

        /// <inheritdoc/>
        void ICustomButtonBehaviour.OnDisabled(UiButton button)
        {

        }

        /// <inheritdoc/>
        public async Awaitable OnHover(UiButton button)
        {
            if (hoverAnimation)
                await hoverAnimation.Enter(button);
        }

        /// <inheritdoc/>
        public async Awaitable OnLeave(UiButton button)
        {
            if (leaveAnimation)
                await leaveAnimation.Leave(button);
        }

        /// <inheritdoc/>
        public async Awaitable OnClick(UiButton button)
        {
            if (clickAnimation)
                await clickAnimation.Click(button);
        }
        
    } // end of class
}
using UnityEngine;

namespace serginian.UI.Animation
{
    /// <summary>
    /// Base class for UI button animations that defines the contract for click, enter, and leave animations.
    /// </summary>
    public abstract class UiButtonAnimation : ScriptableObject
    {
        /// <summary>
        /// Animates the button when clicked.
        /// </summary>
        /// <param name="button">The button to animate.</param>
        /// <returns>An awaitable task that completes when the animation finishes.</returns>
        public abstract Awaitable Click(UiButton button);

        /// <summary>
        /// Animates the button when the pointer enters (hover).
        /// </summary>
        /// <param name="button">The button to animate.</param>
        /// <returns>An awaitable task that completes when the animation finishes.</returns>
        public abstract Awaitable Enter(UiButton button);

        /// <summary>
        /// Animates the button when the pointer leaves (hover exit).
        /// </summary>
        /// <param name="button">The button to animate.</param>
        /// <returns>An awaitable task that completes when the animation finishes.</returns>
        public abstract Awaitable Leave(UiButton button);

    } // end of class
}
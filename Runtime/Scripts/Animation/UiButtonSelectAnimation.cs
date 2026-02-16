using UnityEngine;

namespace serginian.UI.Animation
{
    /// <summary>
    /// Base class for UI button selection animations that defines the contract for select and deselect animations.
    /// </summary>
    public abstract class UiButtonSelectAnimation : ScriptableObject
    {
        /// <summary>
        /// Animates the button when it is selected.
        /// </summary>
        /// <param name="button">The button to animate.</param>
        /// <returns>An awaitable task that completes when the animation finishes.</returns>
        public abstract Awaitable Select(UiButton button);

        /// <summary>
        /// Animates the button when it is deselected.
        /// </summary>
        /// <param name="button">The button to animate.</param>
        /// <returns>An awaitable task that completes when the animation finishes.</returns>
        public abstract Awaitable Deselect(UiButton button);

    } // end of class
}
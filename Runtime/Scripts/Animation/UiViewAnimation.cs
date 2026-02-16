using UnityEngine;

namespace serginian.UI.Animation
{
    /// <summary>
    /// Base class for UI view animations that defines the contract for show and hide animations.
    /// </summary>
    public abstract class UiViewAnimation : ScriptableObject
    {
        /// <summary>
        /// Shows the view with animation asynchronously.
        /// </summary>
        /// <param name="window">The view to show.</param>
        /// <returns>An awaitable task that completes when the show animation finishes.</returns>
        public abstract Awaitable ShowAsync(UiView window);

        /// <summary>
        /// Hides the view with animation asynchronously.
        /// </summary>
        /// <param name="window">The view to hide.</param>
        /// <returns>An awaitable task that completes when the hide animation finishes.</returns>
        public abstract Awaitable HideAsync(UiView window);

        /// <summary>
        /// Closes the view immediately without playing any animation.
        /// </summary>
        /// <param name="window">The view to close.</param>
        public abstract void Close(UiView window);

        /// <summary>
        /// Shows the view immediately without playing any animation.
        /// </summary>
        /// <param name="window">The view to show.</param>
        public abstract void Show(UiView window);

    } // end of class
}
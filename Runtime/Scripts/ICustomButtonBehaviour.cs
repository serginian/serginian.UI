using UnityEngine;

namespace serginian.UI
{
    /// <summary>
    /// Defines custom behavior for UI buttons.
    /// </summary>
    public interface ICustomButtonBehaviour
    {
        /// <summary>
        /// Gets a value indicating whether the behavior should execute when the button is disabled.
        /// </summary>
        public bool ExecuteWhenDisabled { get; }

        /// <summary>
        /// Initializes the button behavior.
        /// </summary>
        /// <param name="button">The button to initialize.</param>
        public void Initialize(UiButton button);

        /// <summary>
        /// Called when the button is enabled.
        /// </summary>
        /// <param name="button">The button that was enabled.</param>
        public void OnEnabled(UiButton button);

        /// <summary>
        /// Called when the button is disabled.
        /// </summary>
        /// <param name="button">The button that was disabled.</param>
        public void OnDisabled(UiButton button);

        /// <summary>
        /// Called when the button is hovered over.
        /// </summary>
        /// <param name="button">The button being hovered.</param>
        /// <returns>An awaitable task.</returns>
        public Awaitable OnHover(UiButton button);

        /// <summary>
        /// Called when the hover leaves the button.
        /// </summary>
        /// <param name="button">The button being left.</param>
        /// <returns>An awaitable task.</returns>
        public Awaitable OnLeave(UiButton button);

        /// <summary>
        /// Called when the button is clicked.
        /// </summary>
        /// <param name="button">The button that was clicked.</param>
        /// <returns>An awaitable task.</returns>
        public Awaitable OnClick(UiButton button);
    }
}
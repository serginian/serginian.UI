using UnityEngine;
using UnityEngine.Events;

namespace serginian.UI
{
    /// <summary>
    /// Represents a UI window that can be shown, hidden, and destroyed with event notifications.
    /// Extends UiView with lifecycle events and cursor management.
    /// </summary>
    public class UiWindow : UiView
    {
        /// <summary>
        /// Event invoked when the window is destroyed.
        /// </summary>
        public event UnityAction<UiView> OnDestroyed;

        /// <summary>
        /// Event invoked when the window is shown.
        /// </summary>
        public event UnityAction<UiView> OnShown;

        /// <summary>
        /// Event invoked when the window is hidden.
        /// </summary>
        public event UnityAction<UiView> OnHide;

        
        
        /********************** MONO BEHAVIOUR **********************/

        /// <summary>
        /// Called when the window is destroyed. Invokes the OnDestroyed event.
        /// </summary>
        protected virtual void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }

        /// <inheritdoc/>
        protected override void Awake()
        {
            base.Awake();
            CloseWithoutAnimation();
        }

        /// <summary>
        /// Called on the frame when the script is enabled just before any of the Update methods are called.
        /// </summary>
        protected virtual void Start()
        {

        }


        /********************** PUBLIC INTERFACE **********************/

        /// <inheritdoc/>
        public override async Awaitable CloseAsync()
        {
            await base.CloseAsync();
            OnHide?.Invoke(this);
        }

        /// <inheritdoc/>
        public override async Awaitable ShowAsync()
        {
            await base.ShowAsync();
            OnShown?.Invoke(this);
        }

        /// <summary>
        /// Shows the cursor and unlocks it from the center of the screen.
        /// </summary>
        protected void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        /// <summary>
        /// Hides the cursor and locks it to the center of the screen.
        /// </summary>
        protected void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        
    } // end of class
}
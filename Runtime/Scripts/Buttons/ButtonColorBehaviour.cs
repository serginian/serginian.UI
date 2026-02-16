using System;
using DG.Tweening;
using UnityEngine;

namespace serginian.UI.Buttons
{
    /// <summary>
    /// Implements button behavior that changes colors based on button state (default, hover, inactive).
    /// </summary>
    [Serializable]
    public sealed class ButtonColorBehaviour: ICustomButtonBehaviour
    {
        [Header("Simple Button")]
        [Tooltip("The default color of the button when it is enabled and not hovered")]
        public Color defaultColor = Color.white;
        [Tooltip("The color of the button when it is hovered")]
        public Color hoverColor = Color.white;
        [Tooltip("The color of the button when it is disabled")]
        public  Color inactiveColor = Color.gray;
        [Tooltip("The duration in seconds for color transition animations")]
        public float colorChangeDuration = 0.5f;
        
        private UiButton _button;

        /// <inheritdoc />
        public bool ExecuteWhenDisabled => false;
        
        /// <summary>
        /// Sets the colors for all button states and updates the button's current color.
        /// </summary>
        /// <param name="default">The default color when enabled and not hovered.</param>
        /// <param name="hover">The color when hovered.</param>
        /// <param name="inactive">The color when disabled.</param>
        public void SetColors(Color @default, Color hover, Color inactive)
        {
            defaultColor = @default;
            hoverColor = hover;
            inactiveColor = inactive;

            if (!_button)
                return;

            _ = _button.IsInteractable ? SetColors(_button.IsHovered ? hoverColor : defaultColor) : SetColors(inactiveColor);
           // _button.SetInteractable(_button.IsInteractable);
        }

        /// <inheritdoc />
        public void Initialize(UiButton button)
        {
            _button = button;
        }
        
        /// <inheritdoc />
        public void OnEnabled(UiButton button)
        {
            if (button.Graphic == null)
                return;

            _ = SetColors(button.IsHovered ? hoverColor : defaultColor);
        }

        /// <inheritdoc />
        public void OnDisabled(UiButton button)
        {
            if (button.Graphic == null)
                return;

            _ = SetColors(inactiveColor);
        }

        /// <inheritdoc />
        public Awaitable OnHover(UiButton button)
        {
            if (button.Graphic == null)
                return Awaitable.WaitForSecondsAsync(0f);

            return SetColors(hoverColor);
        }

        /// <inheritdoc />
        public Awaitable OnLeave(UiButton button)
        {
            if (button.Graphic == null)
                return Awaitable.WaitForSecondsAsync(0f);

            return SetColors(defaultColor);
        }

        /// <inheritdoc />
        public async Awaitable OnClick(UiButton button)
        {
            // do nothing
        }
        
        
        /************************* INNER LOGIC *************************/
        
        private Awaitable SetColors(Color color)
        {
            if (!_button)
                return Awaitable.WaitForSecondsAsync(0f);
            
            foreach (var graphic in _button.Graphic)
            {
                graphic.DOKill();
                graphic.DOColor(color, colorChangeDuration).SetUpdate(true);
            }
            
            return Awaitable.WaitForSecondsAsync(colorChangeDuration);
        }
        
    } // end of class
}
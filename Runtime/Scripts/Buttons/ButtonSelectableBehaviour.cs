using System;
using UnityEngine;
using UnityEngine.Events;

namespace serginian.UI.Buttons
{
    /// <summary>
    /// Provides selection functionality for buttons, allowing them to maintain a selected state.
    /// When part of a <see cref="UiButtonGroup"/>, ensures only one button in the group can be selected at a time.
    /// </summary>
    [Serializable]
    public class ButtonSelectableBehaviour : ICustomButtonBehaviour
    {
        [Tooltip("The button group this button belongs to. When selected, this button will notify the group.")]
        public UiButtonGroup buttonGroup;

        [Header("Settings")]
        [Tooltip("Whether the button should be selected when clicked.")]
        public bool selectOnClick = true;
        [Tooltip("Whether the button should be selected when initialized.")]
        public bool selectOnStart = false;
        [Tooltip("Whether the button should be disabled when selected.")]
        public bool disableWhenSelected = true;

        [Header("Visuals")]
        [Tooltip("The color to apply when the button is selected.")]
        public Color selectionColor = Color.yellow;

        /// <summary>
        /// Event invoked when the button is selected.
        /// </summary>
        public event UnityAction OnSelected;

        /// <summary>
        /// Event invoked when the button is deselected.
        /// </summary>
        public event UnityAction OnDeselected;

        private bool _isSelected;
        private Color _defaultColor;
        private Color _hoverColor;
        private Color _inactiveColor;
        private UiButton _button;
        private ButtonColorBehaviour _clrBehaviour;

        /// <summary>
        /// Gets a value indicating whether the button is currently selected.
        /// </summary>
        public bool IsSelected => _isSelected;

        /// <inheritdoc />
        public bool ExecuteWhenDisabled => false;
        
        
        
        /********************** PUBLIC INTERFACE **********************/

        /// <inheritdoc />
        public void Initialize(UiButton button)
        {
            _button = button;
            _clrBehaviour = _button.GetBehaviour<ButtonColorBehaviour>();
            _defaultColor = _clrBehaviour.defaultColor;
            _hoverColor = _clrBehaviour.hoverColor;
            _inactiveColor = _clrBehaviour.inactiveColor;

            if (selectOnStart)
                SetSelected(true);
        }

        /// <summary>
        /// Explicitly sets the selected state of the button.
        /// </summary>
        /// <param name="selected">True to select the button; false to deselect it.</param>
        public void SetSelected(bool selected)
        {
            if (_isSelected == selected) return;
    
            _isSelected = selected;
            
            if (disableWhenSelected)
                _button.IsInteractable = !_isSelected;
            
            //Debug.Log($"Button {_button?.name ?? ""} is now {(selected ? "selected" : "deselected")}");
            if (selected && buttonGroup != null )
                buttonGroup.SelectedButton = this;

            ApplyVisuals();
            
            if (selected) OnSelected?.Invoke();
            else OnDeselected?.Invoke();
        }

        /// <inheritdoc />
        public void OnEnabled(UiButton button)
        {
            
        }

        /// <inheritdoc />
        public void OnDisabled(UiButton button)
        {
            
        }

        /// <inheritdoc />
        public async Awaitable OnHover(UiButton button)
        {
            
        }

        /// <inheritdoc />
        public async Awaitable OnLeave(UiButton button)
        {
            
        }

        /// <inheritdoc />
        public async Awaitable OnClick(UiButton button)
        {
            if (selectOnClick)
                SetSelected(true);
        }

        
        
        /********************** INNER LOGIC **********************/
        
        private void ApplyVisuals()
        {
            if (_clrBehaviour == null) 
                return;
            
            if (_isSelected)
                _clrBehaviour.SetColors(selectionColor, selectionColor, selectionColor);
            else
                _clrBehaviour.SetColors(_defaultColor, _hoverColor, _inactiveColor);
        }
        
        
    } // end of class
}

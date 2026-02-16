using serginian.UI.Buttons;
using UnityEngine;

namespace serginian.UI
{
    /// <summary>
    /// Manages a group of UI buttons where only one button can be selected at a time.
    /// </summary>
    [CreateAssetMenu(fileName = "TabGroup", menuName = "serginian/UI/Button Group")]
    public class UiButtonGroup : ScriptableObject
    {
        private ButtonSelectableBehaviour _activeButton;

        /// <summary>
        /// Gets or sets the currently selected button in the group.
        /// When a new button is selected, the previously selected button is automatically deselected.
        /// </summary>
        public ButtonSelectableBehaviour SelectedButton 
        { 
            get => _activeButton;
            set
            {
                if (_activeButton == value || value == null)
                    return;
                
                // deselect old button
                _activeButton?.SetSelected(false);
                
                // select new button
                _activeButton = value;
                _activeButton.SetSelected(true);
            }
        }
        
    } // end of class
}
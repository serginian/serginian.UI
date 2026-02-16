using System;
using UnityEngine;

namespace serginian.UI.Buttons
{
    /// <summary>
    /// Button behavior that plays audio clips in response to button interactions.
    /// </summary>
    [Serializable]
    public class ButtonSoundBehaviour: ICustomButtonBehaviour
    {
        [Tooltip("Audio clip to play when the button is clicked.")]
        public AudioClip clickSound;

        [Tooltip("Audio clip to play when the hover leaves the button.")]
        public AudioClip leaveSound;

        [Tooltip("Audio clip to play when the button is hovered over.")]
        public AudioClip hoverSound;

        [Tooltip("Audio source to use for playing sounds. If not assigned, sounds will play at the camera position.")]
        public AudioSource audioSource;

        
        
        /********************** PUBLIC INTERFACE **********************/
        
        /// <inheritdoc />
        public bool ExecuteWhenDisabled => false;

        /// <inheritdoc />
        public void Initialize(UiButton button)
        {

        }

        /// <inheritdoc />
        void ICustomButtonBehaviour.OnEnabled(UiButton button)
        {

        }

        /// <inheritdoc />
        void ICustomButtonBehaviour.OnDisabled(UiButton button)
        {

        }

        /// <inheritdoc />
        public async Awaitable OnHover(UiButton button)
        {
            if (hoverSound)
                PlaySound(hoverSound);
        }

        /// <inheritdoc />
        public async Awaitable OnLeave(UiButton button)
        {
            if (leaveSound)
                PlaySound(leaveSound);
        }

        /// <inheritdoc />
        public async Awaitable OnClick(UiButton button)
        {
            if (clickSound)
                PlaySound(clickSound);
        }
        
        
        
        /********************** INNER LOGIC **********************/

        private void PlaySound(AudioClip sound)
        {
            if (audioSource)
            {
                audioSource.PlayOneShot(clickSound);
                return;
            }
            
            var camera = Camera.main;
            if (camera)
                AudioSource.PlayClipAtPoint(sound, camera.transform.position);
        }
        
        
    } // end of class
}
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace serginian.UI
{
    /// <summary>
    /// Represents a customizable UI button that can handle interactions such as clicks, hover events,
    /// and exit events. The button supports additional behaviors via custom implementations of
    /// <see cref="ICustomButtonBehaviour"/>.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class UiButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, Tooltip("Whether the button can be interacted with.")]
        private bool isInteractable = true;

        [SerializeField, Tooltip("Minimum time in seconds between accepted clicks (debounce)."), Min(0f)]
        private float minClickInterval = 0.7f;

        [SerializeField, Tooltip("Graphics that will be affected by color changes. Auto-detected from children if not set.")]
        private MaskableGraphic[] targetGraphic;

        [SerializeField, Tooltip("Text component to display on the button.")]
        private TextMeshProUGUI targetText;

        [SerializeReference] private ICustomButtonBehaviour[] buttonBehaviours = Array.Empty<ICustomButtonBehaviour>();

        /// <summary>
        /// Event triggered when the button is clicked, after custom behaviors are executed.
        /// More performant than the serialized onClick UnityEvent.
        /// </summary>
        public event UnityAction OnClick;

        /// <summary>
        /// Unity event invoked when the button is clicked. Use <see cref="OnClick"/> for better performance.
        /// </summary>
        [SerializeField] private UnityEvent onClick;
      

        /// <summary>
        /// Gets the RectTransform component of this button.
        /// </summary>
        public RectTransform RectTransform => _rectTransform;

        /// <summary>
        /// Gets the TextMeshProUGUI component used for displaying text on this button.
        /// </summary>
        public TextMeshProUGUI Text => targetText;

        /// <summary>
        /// Gets the array of MaskableGraphic components affected by color changes.
        /// </summary>
        public MaskableGraphic[] Graphic => targetGraphic;

        /// <summary>
        /// Gets or sets whether the button can be interacted with.
        /// When set, notifies all custom behaviors of the enabled/disabled state change.
        /// </summary>
        public bool IsInteractable
        {
            get => isInteractable;
            set
            {
                isInteractable = value;

                if (value)
                    foreach (var behaviour in buttonBehaviours)
                        behaviour?.OnEnabled(this);
                else
                    foreach (var behaviour in buttonBehaviours)
                        behaviour?.OnDisabled(this);
            }
        }

        /// <summary>
        /// Gets whether the pointer is currently hovering over this button.
        /// </summary>
        public bool IsHovered { get; private set; }
        
        private RectTransform _rectTransform;
        private int _behaviourCount;
        private float _lastClickTime;
        

        
        /********************** MONO BEHAVIOUR **********************/
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            
            if (targetGraphic == null || targetGraphic.Length == 0)
                targetGraphic = GetComponentsInChildren<MaskableGraphic>(true);
            
            // Initialize all behaviors
            if (buttonBehaviours != null)
            {
                foreach (var behaviour in buttonBehaviours)
                    behaviour?.Initialize(this);
            }
            _behaviourCount = buttonBehaviours?.Length ?? 0;
        
            IsInteractable = isInteractable;
        }

        
        /********************** PUBLIC INTERFACE **********************/

        /// <summary>
        /// Programmatically triggers the click behavior of the button, bypassing pointer events and click interval restrictions.
        /// </summary>
        public void PerformClick()
        {
            if (!IsInteractable || buttonBehaviours == null)
                return;
            
            RunBehaviours(PointerClick);
            
            onClick?.Invoke();
            OnClick?.Invoke();
        }

        /// <summary>
        /// Sets the interactable state of the button.
        /// </summary>
        /// <param name="active">True to make the button interactable, false to disable it.</param>
        public void SetInteractable(bool active)
        {
            IsInteractable = active;
        }

        /// <summary>
        /// Sets the visible text of the UI button.
        /// </summary>
        /// <param name="text">The string to display on the button.</param>
        public void SetText(string text)
        {
            if (targetText)
                targetText.text = text;
        }

        /// <summary>
        /// Sets the material used by the text component of the UI button.
        /// </summary>
        /// <param name="material">The material to be applied to the text.</param>
        public void SetTextMaterial(Material material)
        {
            if (targetText && material)
                targetText.fontMaterial = material;
        }

        /// <summary>
        /// Animates the color of all target graphics using DOTween.
        /// </summary>
        /// <param name="color">The target color to tween to.</param>
        /// <param name="duration">Duration of the color animation in seconds.</param>
        public void SetColor(Color color, float duration = 0.2f)
        {
            var target = Graphic;
            if (target == null) return;
            foreach (var graphic in target)
            {
                graphic.DOKill();
                graphic.DOColor(color, duration);
            }
        }

        /// <summary>
        /// Retrieves the first button behavior of the specified type if it exists in the button's behavior list.
        /// </summary>
        /// <typeparam name="T">The type of behavior to retrieve. Must implement <see cref="ICustomButtonBehaviour"/>.</typeparam>
        /// <returns>The first instance of the behavior of the specified type if found; otherwise, the default value for the type.</returns>
        public T GetBehaviour<T>() where T : ICustomButtonBehaviour
        {
            if (buttonBehaviours == null) return default;
            
            foreach (var behaviour in buttonBehaviours)
                if (behaviour is T t)
                    return t;
            return default;
        }
        
        
        /********************** INNER LOGIC **********************/
        
        private Awaitable PointerEnter(ICustomButtonBehaviour t) => t.OnHover(this);
        private Awaitable PointerExit(ICustomButtonBehaviour t) => t.OnLeave(this);
        private Awaitable PointerClick(ICustomButtonBehaviour t) => t.OnClick(this);
        
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (IsHovered)
                return;
            
            IsHovered = true;
            
            RunBehaviours(PointerEnter);
        }
        
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (!IsHovered)
                return;

            IsHovered = false;
            
            RunBehaviours(PointerExit);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (Time.time - _lastClickTime < minClickInterval)
                return;
            
            _lastClickTime = Time.time;
            PerformClick();
        }
        
        private void RunBehaviours(Func<ICustomButtonBehaviour, Awaitable> action)
        {
            if (buttonBehaviours == null) 
                return;
            
            for (int i = 0; i < _behaviourCount; i++)
            {
                if (buttonBehaviours[i] == null || !IsInteractable && !buttonBehaviours[i].ExecuteWhenDisabled) continue;
                _ = action(buttonBehaviours[i]); // do not await behaviors, to make them "parallel". In another case some delays in animations may occur.
            }
        }
        
    } // end of class
}
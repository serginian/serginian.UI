
using System;
using System.Collections.Generic;
using UnityEngine;

namespace serginian.UI.Flows
{
    /// <summary>
    /// FlowCoordinator pattern implementation to work with UI in Unity based on UiMaster workflow.
    /// Manages UI navigation, overlay windows, and back navigation stack for screen flows.
    /// </summary>
    public abstract class Coordinator : MonoBehaviour
    {
        private readonly Dictionary<Type, UiView> _registeredWindows = new();
        private UiView _currentScreen;
        private Stack<UiView> _navigationStack;
        private bool _isBackNavigationEnabled;

        
        
        /************************* FOR INHERITORS *************************/

        /// <summary>
        /// Entry point for the coordinator. Implement this method to define the initial UI flow logic.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Registers a UI view window with the coordinator for later navigation.
        /// </summary>
        /// <typeparam name="TWindow">The type of the UI view to register.</typeparam>
        /// <param name="window">The UI view instance to register.</param>
        /// <param name="setup">Optional setup action to configure the window upon registration.</param>
        protected void Register<TWindow>(TWindow window, Action<TWindow> setup = null) where TWindow : UiView
        {
            if (_registeredWindows.ContainsKey(typeof(TWindow))) 
                return;
                
            _registeredWindows.Add(typeof(TWindow), window);
            setup?.Invoke(window);
        }

        /// <summary>
        /// Unregisters a previously registered UI view from the coordinator.
        /// </summary>
        /// <typeparam name="TWindow">The type of the UI view to unregister.</typeparam>
        protected void Unregister<TWindow>() where TWindow : UiView
        {
            if (ViewIsRegistered<TWindow>())
                _registeredWindows.Remove(typeof(TWindow));
        }

        /// <summary>
        /// Enables back navigation functionality, allowing the coordinator to maintain a navigation stack.
        /// </summary>
        protected void EnableBackNavigation()
        {
            if (_navigationStack == null)
                _navigationStack = new Stack<UiView>();
            _isBackNavigationEnabled = true;
        }
        
        /// <summary>
        /// Disables back navigation functionality, preventing the coordinator from maintaining a navigation stack.
        /// </summary>
        protected void DisableBackNavigation()
        {
            _isBackNavigationEnabled = false;
        }
        
        /// <summary>
        /// Navigates to a registered UI view, closing the current screen and updating the navigation stack if enabled.
        /// </summary>
        /// <typeparam name="TWindow">The type of the UI view to navigate to.</typeparam>
        /// <param name="waitForClose">Whether to wait for the current screen to finish closing before showing the next screen. Default is false.</param>
        protected async Awaitable NavigateTo<TWindow>(bool waitForClose = false)
            where TWindow : UiView
        {
            if (ViewIsRegistered<TWindow>())
                await NavigateTo(_registeredWindows[typeof(TWindow)], _isBackNavigationEnabled, waitForClose);
        }

        /// <summary>
        /// Shows a registered UI view as an overlay without affecting the current screen or navigation stack.
        /// </summary>
        /// <typeparam name="TWindow">The type of the UI view to show as an overlay.</typeparam>
        protected async Awaitable ShowOverlay<TWindow>() where TWindow : UiView
        {
            if (ViewIsRegistered<TWindow>())
                await _registeredWindows[typeof(TWindow)].ShowAsync();
        }
        
        /// <summary>
        /// Closes a registered UI view that was shown as an overlay.
        /// </summary>
        /// <typeparam name="TWindow">The type of the UI view overlay to close.</typeparam>
        protected async Awaitable CloseOverlay<TWindow>() where TWindow : UiView
        {
            if (ViewIsRegistered<TWindow>())
                await _registeredWindows[typeof(TWindow)].CloseAsync();
        }

        /// <summary>
        /// Sets a registered UI view as the current screen without showing or hiding any views.
        /// </summary>
        /// <typeparam name="TWindow">The type of the UI view to set as current.</typeparam>
        protected void SetCurrent<TWindow>() where TWindow : UiView
        {
            if (ViewIsRegistered<TWindow>())
                _currentScreen = _registeredWindows[typeof(TWindow)];
        }

        /// <summary>
        /// Navigates back to the previous screen in the navigation stack.
        /// </summary>
        /// <param name="waitForClose">Whether to wait for the current screen to finish closing before showing the previous screen. Default is true.</param>
        protected async Awaitable NavigateBack(bool waitForClose = true)
        {
            if (_navigationStack == null || _navigationStack.Count == 0)
            {
                Debug.LogWarning("Navigation stack is empty or back navigation is not enabled");
                return;
            }

            var previousScreen = _navigationStack.Pop();
            await NavigateTo(previousScreen, false, waitForClose);
        }
        
        

        /**************************** INNER LOGIC ****************************/
    
        private bool ViewIsRegistered<TWindow>() where TWindow : UiView
        {
            if (_registeredWindows.ContainsKey(typeof(TWindow))) 
                return true;
            
            Debug.LogWarning($"Window {typeof(TWindow)} is not registered in coordinator {name}");
            return false;
        }
        
        private async Awaitable NavigateTo(UiView nextScreen, bool saveInStack = false, bool waitForClose = true)
        {
            if (_currentScreen && saveInStack)
                _navigationStack?.Push(_currentScreen);
    
            if (_currentScreen)
            {
                if (waitForClose)
                    await _currentScreen.CloseAsync();
                else
                    _currentScreen.Close();
            }

            _currentScreen = nextScreen;
            await _currentScreen.ShowAsync();
        }
    
    } // end of class
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace serginian.UI
{
    public class UiMaster : MonoBehaviour
    {
        public string contextID;

        private static readonly HashSet<UiView> AllWindows = new();
        private static readonly Dictionary<Type, UiView> WindowsByType = new();
        private static readonly Dictionary<Type, AsyncOperationHandle<GameObject>> LoadedWindows = new();
        private static readonly Dictionary<string, UiMaster> MastersByContext = new();

        public static string CurrentContext { get; private set; } = String.Empty;

    
    
        /********************** MONO BEHAVIOUR **********************/
    
        private void Awake()
        {
            // Register this UiMaster in the cache
            if (!string.IsNullOrEmpty(contextID))
            {
                if (MastersByContext.ContainsKey(contextID)) 
                    Debug.LogWarning($"UiMaster with contextID '{contextID}' already exists. Overwriting.");
                MastersByContext[contextID] = this;
            }
            
            // Find all UiWindow components in children
            UiWindow[] childWindows = GetComponentsInChildren<UiWindow>(true);
        
            // Register each found window
            foreach (UiWindow window in childWindows)
                RegisterWindow(window);
        }

        private void OnDestroy()
        {
            // Cleanup on destroy
            if (!string.IsNullOrEmpty(contextID) && MastersByContext.TryGetValue(contextID, out var master) && master == this)
            {
                MastersByContext.Remove(contextID);
            }
        }
    
    
        /********************** PUBLIC INTERFACE **********************/

        /// <summary>
        /// Sets the current UI context.
        /// </summary>
        /// <param name="contextID">The context identifier to set.</param>
        public static void SetContext(string contextID)
        {
            CurrentContext = contextID;
        }

        /// <summary>
        /// Retrieves an existing window of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the UI window to retrieve.</typeparam>
        /// <returns>The window of type T if found; otherwise, null.</returns>
        public static T GetWindow<T>() where T : UiWindow
        {
            if (WindowsByType.TryGetValue(typeof(T), out var window) && window)
                return window as T;
            return null;
        }

        /// <summary>
        /// Retrieves an existing window or creates a new one if it does not exist.
        /// </summary>
        /// <typeparam name="T">The type of the UI window.</typeparam>
        /// <param name="contextID">The optional context identifier.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the requested window.</returns>
        public static async Awaitable<T> GetOrCreateWindow<T>(string contextID = "") where T : UiWindow
        {
            T window = GetWindow<T>();
            if (!window)
                return await CreateWindow<T>(contextID);
            return window;
        }

        /// <summary>
        /// Creates a new window of the specified type within a given context.
        /// </summary>
        /// <typeparam name="T">The type of the UI window to create.</typeparam>
        /// <param name="contextID">The optional context identifier.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created window.</returns>
        /// <exception cref="Exception">Thrown when no UiMaster component with the specified context exists in the scene.</exception>
        public static async Awaitable<T> CreateWindow<T>(string contextID = "") where T : UiWindow
        {
            var uiContext = string.IsNullOrWhiteSpace(contextID) ? CurrentContext : contextID;
            
            // Use cached UiMaster instead of FindObjectsByType
            if (!MastersByContext.TryGetValue(uiContext, out UiMaster parent))
                throw new Exception($"There is no UiMaster component in the scene with context \"{uiContext}\".");

            return await CreateWindow_Internal<T>(uiContext, parent);
        }

        /// <summary>
        /// Creates a new window of the specified type within a specified UI master and window context.
        /// </summary>
        /// <typeparam name="T">The type of the UI window to create.</typeparam>
        /// <param name="uiMasterContext">The context identifier for the UiMaster.</param>
        /// <param name="windowContext">The context identifier for the window.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created window.</returns>
        /// <exception cref="Exception">Thrown when no UiMaster component with the specified context exists in the scene.</exception>
        public static async Awaitable<T> CreateWindow<T>(string uiMasterContext, string windowContext) where T : UiWindow
        {
            // Use cached UiMaster instead of FindObjectsByType
            if (!MastersByContext.TryGetValue(uiMasterContext, out UiMaster parent))
            {
                throw new Exception($"There is no UiMaster component in the scene with context \"{uiMasterContext}\".");
            }

            return await CreateWindow_Internal<T>(windowContext, parent);
        }



        /********************** INNER LOGIC **********************/

        private static async Awaitable<T> CreateWindow_Internal<T>(string uiContext, UiMaster uiMaster) where T : UiWindow
        {
            string windowName = typeof(T).Name;
            var address = !String.IsNullOrWhiteSpace(uiContext) ? $"UI/{uiContext}/{windowName}" : $"UI/{windowName}";

            if (LoadedWindows.TryGetValue(typeof(T), out var e) && e.IsValid())
                return InstantiateWindow<T>(e, uiMaster);

            var handle = Addressables.LoadAssetAsync<GameObject>(address);
            await handle.Task;
            LoadedWindows[typeof(T)] = handle;
            return InstantiateWindow<T>(handle, uiMaster);
        }

        private static T InstantiateWindow<T>(AsyncOperationHandle<GameObject> handle, UiMaster parent)
            where T : UiWindow
        {
            var window = Instantiate(handle.Result, parent.transform).GetComponent<T>();
            if (!window)
            {
                Debug.LogError($"Failed to instantiate {typeof(T).Name} window. It has no UiWindow component.");
                return null;
            }

            var rectTransform = window.GetComponent<RectTransform>();
            ResetWindowTransform(rectTransform);

            if (!window)
                throw new Exception($"{typeof(T).Name} does not have a UiWindow component.");

            RegisterWindow(window);
            return window;
        }

        private static void ResetWindowTransform(RectTransform rectTransform)
        {
            // Store original anchor settings
            Vector2 originalAnchorMin = rectTransform.anchorMin;
            Vector2 originalAnchorMax = rectTransform.anchorMax;

            // Reset position and scale first
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;

            // Restore original anchors
            rectTransform.anchorMin = originalAnchorMin;
            rectTransform.anchorMax = originalAnchorMax;

            // Now set offsets to 0 - this should work correctly
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private static void RegisterWindow(UiWindow window)
        {
            // HashSet.Add returns false if element already exists
            if (!AllWindows.Add(window))
                return;
        
            // Also register in type lookup dictionary
            WindowsByType[window.GetType()] = window;
            
            window.OnDestroyed += UnregisterWindow;
        }

        private static void UnregisterWindow(UiView window)
        {
            AllWindows.Remove(window);
            
            Type windowType = window.GetType();
            
            // Remove from type lookup
            WindowsByType.Remove(windowType);
            
            // Release Addressables handle
            if (LoadedWindows.TryGetValue(windowType, out var handle) && handle.IsValid())
            {
                Addressables.Release(handle);
                LoadedWindows.Remove(windowType);
            }
        }
    
    } // end of class
}
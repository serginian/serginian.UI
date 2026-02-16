using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace serginian.UI.Editor
{
    [CustomEditor(typeof(UiButton))]
    public class UiButtonEditor : UnityEditor.Editor
    {
        private SerializedProperty isInteractableProperty;
        private SerializedProperty targetGraphicsProperty;
        private SerializedProperty targetTextProperty;
        private SerializedProperty buttonBehavioursProperty;
        private SerializedProperty onClickProperty;

        private static Dictionary<Type, string> availableBehaviours;

        private void OnEnable()
        {
            isInteractableProperty = serializedObject.FindProperty("isInteractable");
            targetGraphicsProperty = serializedObject.FindProperty("targetGraphic");
            targetTextProperty = serializedObject.FindProperty("targetText");
            buttonBehavioursProperty = serializedObject.FindProperty("buttonBehaviours");
            onClickProperty = serializedObject.FindProperty("onClick");

            // Initialize available behaviours using reflection
            if (availableBehaviours == null)
            {
                InitializeAvailableBehaviours();
            }
        }

        private static void InitializeAvailableBehaviours()
        {
            availableBehaviours = new Dictionary<Type, string>();
            
            // Get all assemblies in the current domain
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    // Get all types that implement ICustomButtonBehaviour
                    var behaviourTypes = assembly.GetTypes()
                        .Where(type => typeof(ICustomButtonBehaviour).IsAssignableFrom(type) &&
                                      !type.IsInterface && 
                                      !type.IsAbstract &&
                                      type.IsSerializable)
                        .ToArray();

                    foreach (var behaviourType in behaviourTypes)
                    {
                        // Try to get a display name from DisplayName attribute, otherwise use class name
                        string displayName = GetDisplayName(behaviourType);
                        availableBehaviours[behaviourType] = displayName;
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Some assemblies might have loading issues, skip them
                    Debug.LogWarning($"Could not load types from assembly {assembly.FullName}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error processing assembly {assembly.FullName}: {ex.Message}");
                }
            }
        }

        private static string GetDisplayName(Type behaviourType)
        {
            // Check for DisplayName attribute
            var displayNameAttr = behaviourType.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>();
            if (displayNameAttr != null)
            {
                return displayNameAttr.DisplayName;
            }

            // Fallback: Convert class name to readable format
            string name = behaviourType.Name;
            
            // Remove "Behaviour" suffix if present
            if (name.EndsWith("Behaviour"))
            {
                name = name.Substring(0, name.Length - "Behaviour".Length);
            }
            
            // Add spaces before capital letters (PascalCase to Title Case)
            return System.Text.RegularExpressions.Regex.Replace(name, "([A-Z])", " $1").Trim();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawMainSection();
            DrawOptionalSection();
            DrawEventsSection();
            DrawBehavioursSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMainSection()
        {
            EditorGUILayout.LabelField("Main settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(isInteractableProperty);
            EditorGUILayout.Space();
        }

        private void DrawOptionalSection()
        {
            EditorGUILayout.LabelField("Optional settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetGraphicsProperty);
            EditorGUILayout.PropertyField(targetTextProperty);
            EditorGUILayout.Space();
        }

        private void DrawEventsSection()
        {
            EditorGUILayout.PropertyField(onClickProperty);
            EditorGUILayout.Space();
        }

        private void DrawBehavioursSection()
        {
            EditorGUILayout.LabelField("Button Behaviours", EditorStyles.boldLabel);
            
            // Add behaviour dropdown
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Behaviour", GUILayout.Width(120)))
            {
                ShowAddBehaviourMenu();
            }
            
            if (buttonBehavioursProperty.arraySize > 0 && GUILayout.Button("Clear All", GUILayout.Width(80)))
            {
                if (EditorUtility.DisplayDialog("Clear All Behaviours", 
                    "Are you sure you want to remove all button behaviours?", "Yes", "Cancel"))
                {
                    buttonBehavioursProperty.ClearArray();
                }
            }

            // Refresh behaviours button (useful during development)
            if (GUILayout.Button("↻", GUILayout.Width(25)))
            {
                availableBehaviours = null;
                InitializeAvailableBehaviours();
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Show discovered behaviours count
            EditorGUILayout.LabelField($"Available Behaviours: {availableBehaviours.Count}", EditorStyles.miniLabel);

            // Draw existing behaviours
            for (int i = 0; i < buttonBehavioursProperty.arraySize; i++)
            {
                DrawBehaviourElement(i);
            }

            if (buttonBehavioursProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No behaviours added. Use 'Add Behaviour' to add functionality to your button.", MessageType.Info);
            }
        }

        private void DrawBehaviourElement(int index)
        {
            var behaviourProperty = buttonBehavioursProperty.GetArrayElementAtIndex(index);
            
            if (behaviourProperty.managedReferenceValue == null)
            {
                EditorGUILayout.HelpBox($"Behaviour {index} is null", MessageType.Error);
                return;
            }

            var behaviourType = behaviourProperty.managedReferenceValue.GetType();
            var behaviourName = availableBehaviours.ContainsKey(behaviourType) 
                ? availableBehaviours[behaviourType] 
                : GetDisplayName(behaviourType);

            // Behaviour header with remove button
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            
            var foldoutRect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(true));
            foldoutRect.width -= 60; // Space for remove button
            
            behaviourProperty.isExpanded = EditorGUI.Foldout(foldoutRect, behaviourProperty.isExpanded, behaviourName, true);
            
            // Remove button
            var removeButtonRect = new Rect(foldoutRect.xMax + 5, foldoutRect.y, 50, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(removeButtonRect, "Remove"))
            {
                if (EditorUtility.DisplayDialog("Remove Behaviour", 
                    $"Remove {behaviourName} behaviour?", "Remove", "Cancel"))
                {
                    buttonBehavioursProperty.DeleteArrayElementAtIndex(index);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
            }
            
            EditorGUILayout.EndHorizontal();

            // Behaviour properties
            if (behaviourProperty.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(behaviourProperty, new GUIContent(behaviourName), true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private void ShowAddBehaviourMenu()
        {
            var menu = new GenericMenu();
            
            foreach (var kvp in availableBehaviours.OrderBy(x => x.Value))
            {
                var behaviourType = kvp.Key;
                var displayName = kvp.Value;
                
                // Check if this behaviour type already exists
                bool alreadyExists = false;
                for (int i = 0; i < buttonBehavioursProperty.arraySize; i++)
                {
                    var existingBehaviour = buttonBehavioursProperty.GetArrayElementAtIndex(i);
                    if (existingBehaviour.managedReferenceValue?.GetType() == behaviourType)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (alreadyExists)
                {
                    menu.AddDisabledItem(new GUIContent(displayName + " (Already added)"));
                }
                else
                {
                    menu.AddItem(new GUIContent(displayName), false, () => AddBehaviour(behaviourType));
                }
            }
            
            if (availableBehaviours.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No behaviours found"));
            }
            
            menu.ShowAsContext();
        }

        private void AddBehaviour(Type behaviourType)
        {
            buttonBehavioursProperty.arraySize++;
            var newBehaviourProperty = buttonBehavioursProperty.GetArrayElementAtIndex(buttonBehavioursProperty.arraySize - 1);
            newBehaviourProperty.managedReferenceValue = Activator.CreateInstance(behaviourType);
            newBehaviourProperty.isExpanded = true;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
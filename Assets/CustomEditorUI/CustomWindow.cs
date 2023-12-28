using System;
using UnityEditor;
using UnityEngine;


namespace CustomEditorUI
{
    
    public class CustomWindow : EditorWindow
    {
        
        private string myString = "Hello World"; 
        private bool groupEnabled; 
        private bool myBool = true; 
        private float myFloat = 1.23f;
        
        
        [MenuItem("Window/Custom Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CustomWindow));
        }


        private void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel); 
            
            myString = EditorGUILayout.TextField("Text Field", myString); 
            
            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled); 
            
            myBool = EditorGUILayout.Toggle("Toggle", myBool); 
            myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3); 
            
            EditorGUILayout.EndToggleGroup();
        }
        
        
    }
}
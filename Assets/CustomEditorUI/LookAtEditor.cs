using UnityEditor;
using UnityEngine;


namespace CustomEditorUI
{
    
    [CustomEditor(typeof(LookAt))]
    [CanEditMultipleObjects]
    public class LookAtEditor : Editor
    {
        
        SerializedProperty lookAtPoint;

        
        void OnEnable()
        {
            lookAtPoint = serializedObject.FindProperty("LookAtPoint");
        }

        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(lookAtPoint);

            if (lookAtPoint.vector3Value.y > (target as LookAt).transform.position.y)
            {
                EditorGUILayout.LabelField("(Above this object)");
            }

            if (lookAtPoint.vector3Value.y < (target as LookAt).transform.position.y)
            {
                EditorGUILayout.LabelField("(Below this object)");
            } 
            
            serializedObject.ApplyModifiedProperties();
        }

        
        public void OnSceneGUI()
        {
            var t = (target as LookAt); 
            EditorGUI.BeginChangeCheck(); 
            Vector3 pos = Handles.PositionHandle(t.LookAtPoint, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move point"); 
                t.LookAtPoint = pos; 
                t.Update();
            }
        }
        
        
    }
}
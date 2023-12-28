using UnityEngine;


namespace CustomEditorUI.PropertyDrawer
{
    
    public class TestRangeAttribute : MonoBehaviour
    {
        
        [RangeAttribute(0, 20), SerializeField] private int _integer; 
        [RangeAttribute(0f, 20f), SerializeField] private float _float; 
        [RangeAttribute(0f, 20), SerializeField] private string _string;
        
    }
}
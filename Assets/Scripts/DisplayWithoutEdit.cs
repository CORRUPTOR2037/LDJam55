#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
    
/// <summary>
/// Allow to display an attribute in inspector without allow editing
/// </summary>
public class DisplayWithoutEdit : PropertyAttribute {
    
    public DisplayWithoutEdit()
    {
    
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DisplayWithoutEdit))]
public class DisplayWithoutEditDrawer : PropertyDrawer {
 
 
    /// <summary>
    /// Display attribute and his value in inspector depending on the type
    /// Fill attribute needed
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.AnimationCurve:
                break;
            case SerializedPropertyType.ArraySize:
                break;
            case SerializedPropertyType.Boolean:
                EditorGUI.LabelField(position, label, new GUIContent(property.boolValue.ToString()));
                break;
            case SerializedPropertyType.Bounds:
                break;
            case SerializedPropertyType.Character:
                break;
            case SerializedPropertyType.Color:
                break;
            case SerializedPropertyType.Enum:
                EditorGUI.LabelField(position, label, new GUIContent(property.enumDisplayNames[property.enumValueIndex]));
                break;
            case SerializedPropertyType.Float:
                EditorGUI.LabelField(position, label, new GUIContent(property.floatValue.ToString()));
                break;
            case SerializedPropertyType.Generic:
                break;
            case SerializedPropertyType.Gradient:
                break;
            case SerializedPropertyType.Integer:
                EditorGUI.LabelField(position, label, new GUIContent(property.intValue.ToString()));
                break;
            case SerializedPropertyType.LayerMask:
                break;
            case SerializedPropertyType.ObjectReference:
                EditorGUI.LabelField(position, label, new GUIContent(property.objectReferenceValue != null ? property.objectReferenceValue.name.ToString() : "__None__"));
                break;
            case SerializedPropertyType.Quaternion:
                break;
            case SerializedPropertyType.Rect:
                break;
            case SerializedPropertyType.String:
                EditorGUI.LabelField(position, label, new GUIContent(property.stringValue));
                break;
            case SerializedPropertyType.Vector2:
                EditorGUI.LabelField(position, label, new GUIContent(string.Format("({0};{1})",
                    property.vector2Value.x,
                    property.vector2Value.y))
                );
                break;
            case SerializedPropertyType.Vector3:
                EditorGUI.LabelField(position, label, new GUIContent(string.Format("({0};{1};{2})",
                    property.vector3Value.x,
                    property.vector3Value.y,
                    property.vector3Value.z))
                );
                break;
            case SerializedPropertyType.Vector4:
                EditorGUI.LabelField(position, label, new GUIContent(string.Format("({0};{1};{2};{3})",
                    property.vector4Value.x,
                    property.vector4Value.y,
                    property.vector4Value.z,
                    property.vector4Value.w))
                );
                break;
        }
    }
}
#endif
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ekka.blendShapeBuilder
{
    [CustomEditor(typeof(BlendShapeBlueprintList))]
    public class BlendshapeBlueprintListInspector : Editor
    {
        // Cashe is required for nested reorderable list.
        private class NestedReorderableListCashe
        {
            private Dictionary<string, ReorderableList> _nestedReorderableLists = new Dictionary<string, ReorderableList>();

            public ReorderableList GetList(SerializedProperty property)
            {
                if (!_nestedReorderableLists.ContainsKey(property.propertyPath))
                {
                    _nestedReorderableLists.Add(property.propertyPath, new ReorderableList(property.serializedObject, property));
                }

                return _nestedReorderableLists[property.propertyPath];
            }
        }

        private SkinnedMeshRenderer _samplingMesh = null;
        private BlendShapeSelectSelector _popup = new BlendShapeSelectSelector();
        private int _selectedIndex = -1;
        private int _selectedSubIndex = -1;

        private ReorderableList _reorderableList;
        private NestedReorderableListCashe _nestedReorderableListCashe = new NestedReorderableListCashe();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Sampling Mesh");

            _samplingMesh = EditorGUILayout.ObjectField(_samplingMesh, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;

            EditorGUILayout.Space(10);

            var blendShapes = serializedObject.FindProperty("blendShapes");
            
            if (_reorderableList is null)
            {
                _reorderableList = new ReorderableList(serializedObject, blendShapes);
                _reorderableList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Blend Shapes");
                
                _reorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
                {
                    var blendShape = blendShapes.GetArrayElementAtIndex(index);
                    var dstBlendShapeName = blendShape.FindPropertyRelative(nameof(BlendShapeBlueprint.newBlendShapeName));
                    var srcBlendShapes = blendShape.FindPropertyRelative("blendShapeSources");

                    _nestedReorderableListCashe.GetList(srcBlendShapes).drawElementCallback = (rectInner, indexInner, isActiveInner, isFocusedInner) =>
                    {
                        var name = srcBlendShapes.GetArrayElementAtIndex(indexInner).FindPropertyRelative("name");
                        EditorGUI.PropertyField(new Rect(rectInner.position, new Vector2(rectInner.width * 3 / 4, EditorGUIUtility.singleLineHeight)), name, GUIContent.none);

                        using (new EditorGUI.DisabledGroupScope(_samplingMesh is null))
                        {
                            if (GUI.Button(new Rect(new Vector2(rectInner.x + rectInner.width * 3 / 4, rectInner.y), new Vector2(rectInner.width / 4, EditorGUIUtility.singleLineHeight)), "Select"))
                            {
                                _selectedIndex = index;
                                _selectedSubIndex = indexInner;
                                _popup.title = dstBlendShapeName.stringValue;
                                _popup.SetWindowSize(
                                    EditorGUIUtility.currentViewWidth,
                                    Screen.height * (EditorGUIUtility.currentViewWidth / Screen.width) - 160
                                    );
                                PopupWindow.Show(new Rect(), _popup);
                            }
                        }
                    };

                    _nestedReorderableListCashe.GetList(srcBlendShapes).DoList(rect);

                    EditorGUI.PropertyField(rect, dstBlendShapeName, GUIContent.none, true);
                };

                _reorderableList.elementHeightCallback = index =>
                {
                    var blendShape = blendShapes.GetArrayElementAtIndex(index);
                    var srcBlendShapes = blendShape.FindPropertyRelative("blendShapeSources");

                    return _nestedReorderableListCashe.GetList(srcBlendShapes).GetHeight() + EditorGUIUtility.singleLineHeight * 0.5f;
                };
            }

            _reorderableList.DoLayoutList();

            if (_selectedIndex >= 0 && _selectedSubIndex >= 0 && _popup.GetSelectedBlendshape().Length > 0)
            {
                var srcBlendShapes = blendShapes.GetArrayElementAtIndex(_selectedIndex).FindPropertyRelative("blendShapeSources");
                var name = srcBlendShapes.GetArrayElementAtIndex(_selectedSubIndex).FindPropertyRelative("name");
                name.stringValue = _popup.GetSelectedBlendshape();
                _popup.ClearSelectedBlendshape();
                _selectedIndex = -1;
                _selectedSubIndex = -1;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

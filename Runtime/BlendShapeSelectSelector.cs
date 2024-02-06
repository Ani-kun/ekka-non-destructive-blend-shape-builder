using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ekka.blendShapeBuilder
{
    public class BlendShapeSelectSelector : PopupWindowContent
    {
        public string title = "Title";
        public SkinnedMeshRenderer _skinnedMesh = null;
        private List<string> _skinnedMeshBlendShapes = new List<string>();

        private Vector2 _windowSize = Vector2.zero;
        private Vector2 _scrollPosition = Vector2.zero;
        private int _selected = -1;
        private string _blendShapeName = string.Empty;

        public string GetSelectedBlendshape()
        {
            return _blendShapeName;
        }
        public void ClearSelectedBlendshape()
        {
            _blendShapeName = "";
        }

        public override Vector2 GetWindowSize()
        {
            return _windowSize;
        }

        public void SetWindowSize(float width, float height)
        {
            _windowSize.x = width;
            _windowSize.y = height;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            _selected = -1;
            _blendShapeName = string.Empty;
            if (_skinnedMesh != null)
            {
                _skinnedMeshBlendShapes.Clear();

                var blendShapeCount = _skinnedMesh.sharedMesh.blendShapeCount;
                for (var index = 0; index < blendShapeCount; index++)
                {
                    var propertyName = _skinnedMesh.sharedMesh.GetBlendShapeName(index);
                    _skinnedMeshBlendShapes.Add(propertyName);
                }
            }
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label(title);

            EditorGUILayout.Space(10);

            if (GUILayout.Button("None"))
            {
                _selected = -1;
            }
            
            using (var scrollViewScope = new GUILayout.ScrollViewScope(_scrollPosition))
            {
                _scrollPosition = scrollViewScope.scrollPosition;
                _selected = GUILayout.SelectionGrid(_selected, _skinnedMeshBlendShapes.ToArray(), 2);
            }

            EditorGUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledGroupScope(_selected < 0))
                {
                    if (GUILayout.Button("OK") || (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return))
                    {
                        _blendShapeName = _skinnedMeshBlendShapes[_selected];
                        editorWindow.Close();
                    }
                }

                if (GUILayout.Button("Cancel") || (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape))
                {
                    editorWindow.Close();
                }
            }
        }
    }
}

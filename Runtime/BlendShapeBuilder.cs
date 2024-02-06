using UnityEngine;
using nadena.dev.ndmf;
using nadena.dev.modular_avatar.core;

namespace ekka.blendShapeBuilder
{
    [AddComponentMenu("EKKA/EKKA Blend Shape Builder")]
    [DisallowMultipleComponent]
    public class BlendShapeBuilder : AvatarTagComponent
    {
        [SerializeField]
        private BlendShapeBlueprintList _scriptableObject = null;

        private SkinnedMeshRenderer _originalMesh = null;

        private void GetAttachedSkinnedMeshRenderer()
        {
            _originalMesh = gameObject.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
        }

        protected override void OnValidate()
        {
            GetAttachedSkinnedMeshRenderer();
        }

        public override void ResolveReferences()
        {
            GetAttachedSkinnedMeshRenderer();
        }

        public void Excute()
        {
            if (_scriptableObject is null || _originalMesh is null)
            {
                return;
            }

            var mesh = Instantiate(_originalMesh.sharedMesh);
            foreach (var blendShapePair in _scriptableObject.blendShapes)
            {
                if (mesh.GetBlendShapeIndex(blendShapePair.newBlendShapeName) >= 0)
                {
                    // Do not add already existing blendshapes.
                    break;
                }

                var compositDeltaVertices = new Vector3[mesh.vertexCount];
                var compositDeltaNormals = new Vector3[mesh.vertexCount];
                var compositDeltaTangents = new Vector3[mesh.vertexCount];
                foreach (var srcBlendShape in blendShapePair.blendShapeSources)
                {
                    var blendShapeIndex = mesh.GetBlendShapeIndex(srcBlendShape.name);
                    if (blendShapeIndex < 0)
                    {
                        break;
                    }

                    var frameCount = mesh.GetBlendShapeFrameCount(blendShapeIndex);
                    for (int frame = 0; frame < frameCount; frame++)
                    {
                        var weight = mesh.GetBlendShapeFrameWeight(blendShapeIndex, frame);
                        if (weight != 100.0f)
                        {
                            // Ignore interframe.
                            break;
                        }

                        var deltaVertices = new Vector3[mesh.vertexCount];
                        var deltaNormals = new Vector3[mesh.vertexCount];
                        var deltaTangents = new Vector3[mesh.vertexCount];

                        mesh.GetBlendShapeFrameVertices(blendShapeIndex, frame, deltaVertices, deltaNormals, deltaTangents);
                        for (int vertex = 0; vertex < mesh.vertexCount; vertex++)
                        {
                            compositDeltaVertices[vertex] += deltaVertices[vertex] * srcBlendShape.weight / 100;
                            compositDeltaNormals[vertex] += deltaNormals[vertex] * srcBlendShape.weight / 100;
                            compositDeltaTangents[vertex] += deltaTangents[vertex] * srcBlendShape.weight / 100;
                        }
                    }
                }

                mesh.AddBlendShapeFrame(blendShapePair.newBlendShapeName, 100.0f, compositDeltaVertices, compositDeltaNormals, compositDeltaTangents);
            }

            ObjectRegistry.RegisterReplacedObject(_originalMesh.sharedMesh, mesh);

            _originalMesh.sharedMesh = mesh;
        }
    }
}

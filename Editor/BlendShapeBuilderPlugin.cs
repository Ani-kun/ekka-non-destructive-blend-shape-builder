using UnityEngine;
using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(ekka.blendShapeBuilder.BlendShapeBuilderPlugin))]
namespace ekka.blendShapeBuilder
{
    public class BlendShapeBuilderPlugin : Plugin<BlendShapeBuilderPlugin>
    {
        public override string QualifiedName => "ekka.ndmf.blend-shape-builder";

        protected override void Configure()
        {
            InPhase(BuildPhase.Generating)
                .BeforePlugin("nadena.dev.modular-avatar")
                .BeforePlugin("com.anatawa12.avatar-optimizer")
                .Run("BlendshapeBuilder", ctx => {
                    var components = ctx.AvatarRootObject.GetComponentsInChildren<BlendShapeBuilder>();
                    foreach (var component in components)
                    {
                        component.Excute();
                        Object.Destroy(component);
                    }
                });
        }
    }
}
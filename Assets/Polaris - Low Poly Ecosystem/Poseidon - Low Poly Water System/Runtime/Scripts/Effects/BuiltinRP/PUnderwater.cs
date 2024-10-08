#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

namespace Pinwheel.Poseidon.FX.PostProcessing
{
    //형모수정
    [System.Serializable]
    [PostProcess(typeof(PUnderwaterRenderer), PostProcessEvent.BeforeStack, "Poseidon/Underwater", false)]
    public sealed class PUnderwater : PostProcessEffectSettings
    {
        [Header("Water Body")]
        public FloatParameter waterLevel = new FloatParameter();
        public FloatParameter maxDepth = new FloatParameter() { value = 10 };
        [Range(0f, 3f)]
        public FloatParameter surfaceColorBoost = new FloatParameter() { value = 1 };

        [Header("Fog")]
        public ColorParameter shallowFogColor = new ColorParameter() { value = new Color(76, 109, 152, 255) };
        public ColorParameter deepFogColor = new ColorParameter() { value = new Color(57, 185, 130, 255) };
        public FloatParameter viewDistance = new FloatParameter() { value = 30 };

        [Header("Caustic")]
        public BoolParameter enableCaustic = new BoolParameter() { value = true };
        public TextureParameter causticTexture = new TextureParameter();
        public FloatParameter causticSize = new FloatParameter() { value = 10 } ;
        [Range(0f, 1f)]
        public FloatParameter causticStrength = new FloatParameter() { value = 1 };

        [Header("Distortion")]
        public BoolParameter enableDistortion = new BoolParameter() { value = true };
        public TextureParameter distortionNormalMap = new TextureParameter();
        public FloatParameter distortionStrength = new FloatParameter() { value = 1 };
        public FloatParameter waterFlowSpeed = new FloatParameter() { value = 1 };

        [Header("Internal")]
        [Range(0f, 1f)]
        public FloatParameter intensity = new FloatParameter() { value = 0.25f };

        //형모수정
        public static PUnderwater StandardPUnderWater()
        {
            PUnderwater tempt = CreateInstance<PUnderwater>();
            tempt.causticTexture.value = Resources.Load<Texture>("Textures/OuterAssets/Caustic");
            tempt.distortionNormalMap.value = Resources.Load<Texture>("Textures/OuterAssets/UnderwaterDistortion");
            tempt.enabled.value = true;
            tempt.SetAllOverridesTo(true);

            return tempt;
        }

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {

            return enabled.value;

            //형모수정
            /*
            return enabled.value
                && intensity.value > 0
                && context.camera.transform.position.y <= waterLevel.value;
            */
            //형모수정
        }
    }
}
#endif

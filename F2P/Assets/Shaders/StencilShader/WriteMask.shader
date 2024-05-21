Shader "Unlit/WriteMask"
{
    Properties
    {
        [IntRange] _StencilWriteMask ("StencilWriteMask", Range(0,255)) = 0
    }

    SubShader
    {
        Tags {"RenderType"="Opaque" "Queue"="Geometry-1" "RenderPipeline" = "UniversalPipeline"}

        Pass
        {
            Blend Zero One
            ZWrite Off

            Stencil
            {
                Ref [_StencilWriteMask]
                Comp Always
				Pass Replace
            }
        }
    }
}

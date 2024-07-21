Shader "BulletDance/UI/LoadSplatter"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _Definition("Definition", Float) = 64
        _ColorNum("ColorNum", Float) = 8
        _Size("Size", Float) = 1
        _Shape("Shape", Float) = 3.14
        _Intensity("Intensity", Float) = 6.07
        _Dispersity("Dispersity", Float) = 0.25
        _BaseColor("BaseColor", Color) = (1, 0, 0, 0)
        _Hue("Hue", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        _Stencil("Stencil ID", Float) = 0
        _StencilComp("StencilComp", Float) = 8
        _StencilOp("StencilOp", Float) = 0
        _StencilReadMask("StencilReadMask", Float) = 255
        _StencilWriteMask("StencilWriteMask", Float) = 255
        _ColorMask("ColorMask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteUnlitSubTarget"
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Stencil{
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _Size;
        float _Definition;
        float _ColorNum;
        float _Dispersity;
        float _Shape;
        float _Intensity;
        float4 _BaseColor;
        float _Hue;
        CBUFFER_END
        
        // Object and Global properties
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Hue_Normalized_float(float3 In, float Offset, out float3 Out)
        {
            // RGB to HSV
            float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
            float4 P = lerp(float4(In.bg, K.wz), float4(In.gb, K.xy), step(In.b, In.g));
            float4 Q = lerp(float4(P.xyw, In.r), float4(In.r, P.yzx), step(P.x, In.r));
            float D = Q.x - min(Q.w, Q.y);
            float E = 1e-10;
            float V = (D == 0) ? Q.x : (Q.x + E);
            float3 hsv = float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), V);
        
            float hue = hsv.x + Offset;
            hsv.x = (hue < 0)
                    ? hue + 1
                    : (hue > 1)
                        ? hue - 1
                        : hue;
        
            // HSV to RGB
            float4 K2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
            float3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
            Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
        }
        
        void Unity_Posterize_float4(float4 In, float4 Steps, out float4 Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        float2 Unity_Voronoi_RandomVector_Deterministic_float (float2 UV, float offset)
        {
            Hash_Tchou_2_2_float(UV, UV);
            return float2(sin(UV.y * offset), cos(UV.x * offset)) * 0.5 + 0.5;
        }
        
        void Unity_Voronoi_Deterministic_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    float2 lattice = float2(x, y);
                    float2 offset = Unity_Voronoi_RandomVector_Deterministic_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
                    if (d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Posterize_float(float In, float Steps, out float Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_8a73e0694ca7426884503472e35c36fa_Out_0 = _BaseColor;
            float _Property_023022bf8c254ce4a2a5d0f23121a5b5_Out_0 = _Hue;
            float3 _Hue_d9342b9d0b654871be850e9cec4dd5c9_Out_2;
            Unity_Hue_Normalized_float((_Property_8a73e0694ca7426884503472e35c36fa_Out_0.xyz), _Property_023022bf8c254ce4a2a5d0f23121a5b5_Out_0, _Hue_d9342b9d0b654871be850e9cec4dd5c9_Out_2);
            float4 _UV_e653f2588110430ca6cf4ce6cce85191_Out_0 = IN.uv0;
            float _Property_9917dafac71a4cd6acab9b1356111997_Out_0 = _Definition;
            float4 _Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2;
            Unity_Posterize_float4(_UV_e653f2588110430ca6cf4ce6cce85191_Out_0, (_Property_9917dafac71a4cd6acab9b1356111997_Out_0.xxxx), _Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2);
            float _Property_c4d42add66084a568af9ea454d1ba2fa_Out_0 = _Dispersity;
            float _Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3;
            Unity_Clamp_float(_Property_c4d42add66084a568af9ea454d1ba2fa_Out_0, -0.25, 0.25, _Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3);
            float _Property_b9784b9b2d444f088b2b149890afed38_Out_0 = _Shape;
            float _Property_640084c7279a46e0bb4cdad977ce5b79_Out_0 = _Intensity;
            float _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3;
            float _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Cells_4;
            Unity_Voronoi_Deterministic_float((_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2.xy), _Property_b9784b9b2d444f088b2b149890afed38_Out_0, _Property_640084c7279a46e0bb4cdad977ce5b79_Out_0, _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3, _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Cells_4);
            float _Remap_0057d562539c4abca83d94a27d34dceb_Out_3;
            Unity_Remap_float(_Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3, float2 (0, 1), float2 (-0.5, 0.5), _Remap_0057d562539c4abca83d94a27d34dceb_Out_3);
            float _Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2;
            Unity_Multiply_float_float(_Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3, _Remap_0057d562539c4abca83d94a27d34dceb_Out_3, _Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2);
            float4 _Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2;
            Unity_Add_float4(_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2, (_Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2.xxxx), _Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2);
            float _Property_f09b459bf28f462586e7e57060b6c9ac_Out_0 = _Size;
            float _Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2;
            Unity_Divide_float(1, _Property_f09b459bf28f462586e7e57060b6c9ac_Out_0, _Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2);
            float _Float_9dd79f21b7534a9abd0d928c58e6cb4e_Out_0 = -0.5;
            float _Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2;
            Unity_Multiply_float_float(_Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2, _Float_9dd79f21b7534a9abd0d928c58e6cb4e_Out_0, _Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2);
            float2 _TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3;
            Unity_TilingAndOffset_float((_Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2.xy), (_Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2.xx), (_Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2.xx), _TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3);
            float _Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2;
            Unity_Distance_float2(_TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3, float2(0, 0), _Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2);
            float _Property_7fe511539bfe41e1a4b2928e8a63229e_Out_0 = _ColorNum;
            float _Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2;
            Unity_Posterize_float(_Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2, _Property_7fe511539bfe41e1a4b2928e8a63229e_Out_0, _Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2);
            float _Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2;
            Unity_Multiply_float_float(_Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2, 2.2, _Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2);
            float _Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3;
            Unity_Clamp_float(_Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2, 0, 1, _Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3);
            float _OneMinus_1b1228248566463f850624229cddffd1_Out_1;
            Unity_OneMinus_float(_Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3, _OneMinus_1b1228248566463f850624229cddffd1_Out_1);
            float2 _Vector2_b26d37ea39484d5ab0ea97feb0792f28_Out_0 = float2(0.5, 0.5);
            float _Distance_513c9b0903294c349493d021aa02c5a7_Out_2;
            Unity_Distance_float2((_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2.xy), _Vector2_b26d37ea39484d5ab0ea97feb0792f28_Out_0, _Distance_513c9b0903294c349493d021aa02c5a7_Out_2);
            float _Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2;
            Unity_Multiply_float_float(_Distance_513c9b0903294c349493d021aa02c5a7_Out_2, 2.2, _Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2);
            float _Remap_216e58c559ed435180c71ad602988046_Out_3;
            Unity_Remap_float(_Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2, float2 (0, 1), float2 (-2.25, 1), _Remap_216e58c559ed435180c71ad602988046_Out_3);
            float _Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2;
            Unity_Posterize_float(_Remap_216e58c559ed435180c71ad602988046_Out_3, 2, _Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2);
            float _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3;
            Unity_Clamp_float(_Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2, 0, 1, _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3);
            float _Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2;
            Unity_Subtract_float(_OneMinus_1b1228248566463f850624229cddffd1_Out_1, _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3, _Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2);
            float _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3;
            Unity_Clamp_float(_Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2, 0, 1, _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3);
            surface.BaseColor = _Hue_d9342b9d0b654871be850e9cec4dd5c9_Out_2;
            surface.Alpha = _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _Size;
        float _Definition;
        float _ColorNum;
        float _Dispersity;
        float _Shape;
        float _Intensity;
        float4 _BaseColor;
        float _Hue;
        CBUFFER_END
        
        // Object and Global properties
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Posterize_float4(float4 In, float4 Steps, out float4 Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        float2 Unity_Voronoi_RandomVector_Deterministic_float (float2 UV, float offset)
        {
            Hash_Tchou_2_2_float(UV, UV);
            return float2(sin(UV.y * offset), cos(UV.x * offset)) * 0.5 + 0.5;
        }
        
        void Unity_Voronoi_Deterministic_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    float2 lattice = float2(x, y);
                    float2 offset = Unity_Voronoi_RandomVector_Deterministic_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
                    if (d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Posterize_float(float In, float Steps, out float Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _UV_e653f2588110430ca6cf4ce6cce85191_Out_0 = IN.uv0;
            float _Property_9917dafac71a4cd6acab9b1356111997_Out_0 = _Definition;
            float4 _Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2;
            Unity_Posterize_float4(_UV_e653f2588110430ca6cf4ce6cce85191_Out_0, (_Property_9917dafac71a4cd6acab9b1356111997_Out_0.xxxx), _Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2);
            float _Property_c4d42add66084a568af9ea454d1ba2fa_Out_0 = _Dispersity;
            float _Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3;
            Unity_Clamp_float(_Property_c4d42add66084a568af9ea454d1ba2fa_Out_0, -0.25, 0.25, _Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3);
            float _Property_b9784b9b2d444f088b2b149890afed38_Out_0 = _Shape;
            float _Property_640084c7279a46e0bb4cdad977ce5b79_Out_0 = _Intensity;
            float _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3;
            float _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Cells_4;
            Unity_Voronoi_Deterministic_float((_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2.xy), _Property_b9784b9b2d444f088b2b149890afed38_Out_0, _Property_640084c7279a46e0bb4cdad977ce5b79_Out_0, _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3, _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Cells_4);
            float _Remap_0057d562539c4abca83d94a27d34dceb_Out_3;
            Unity_Remap_float(_Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3, float2 (0, 1), float2 (-0.5, 0.5), _Remap_0057d562539c4abca83d94a27d34dceb_Out_3);
            float _Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2;
            Unity_Multiply_float_float(_Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3, _Remap_0057d562539c4abca83d94a27d34dceb_Out_3, _Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2);
            float4 _Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2;
            Unity_Add_float4(_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2, (_Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2.xxxx), _Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2);
            float _Property_f09b459bf28f462586e7e57060b6c9ac_Out_0 = _Size;
            float _Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2;
            Unity_Divide_float(1, _Property_f09b459bf28f462586e7e57060b6c9ac_Out_0, _Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2);
            float _Float_9dd79f21b7534a9abd0d928c58e6cb4e_Out_0 = -0.5;
            float _Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2;
            Unity_Multiply_float_float(_Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2, _Float_9dd79f21b7534a9abd0d928c58e6cb4e_Out_0, _Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2);
            float2 _TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3;
            Unity_TilingAndOffset_float((_Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2.xy), (_Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2.xx), (_Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2.xx), _TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3);
            float _Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2;
            Unity_Distance_float2(_TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3, float2(0, 0), _Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2);
            float _Property_7fe511539bfe41e1a4b2928e8a63229e_Out_0 = _ColorNum;
            float _Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2;
            Unity_Posterize_float(_Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2, _Property_7fe511539bfe41e1a4b2928e8a63229e_Out_0, _Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2);
            float _Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2;
            Unity_Multiply_float_float(_Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2, 2.2, _Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2);
            float _Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3;
            Unity_Clamp_float(_Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2, 0, 1, _Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3);
            float _OneMinus_1b1228248566463f850624229cddffd1_Out_1;
            Unity_OneMinus_float(_Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3, _OneMinus_1b1228248566463f850624229cddffd1_Out_1);
            float2 _Vector2_b26d37ea39484d5ab0ea97feb0792f28_Out_0 = float2(0.5, 0.5);
            float _Distance_513c9b0903294c349493d021aa02c5a7_Out_2;
            Unity_Distance_float2((_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2.xy), _Vector2_b26d37ea39484d5ab0ea97feb0792f28_Out_0, _Distance_513c9b0903294c349493d021aa02c5a7_Out_2);
            float _Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2;
            Unity_Multiply_float_float(_Distance_513c9b0903294c349493d021aa02c5a7_Out_2, 2.2, _Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2);
            float _Remap_216e58c559ed435180c71ad602988046_Out_3;
            Unity_Remap_float(_Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2, float2 (0, 1), float2 (-2.25, 1), _Remap_216e58c559ed435180c71ad602988046_Out_3);
            float _Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2;
            Unity_Posterize_float(_Remap_216e58c559ed435180c71ad602988046_Out_3, 2, _Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2);
            float _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3;
            Unity_Clamp_float(_Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2, 0, 1, _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3);
            float _Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2;
            Unity_Subtract_float(_OneMinus_1b1228248566463f850624229cddffd1_Out_1, _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3, _Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2);
            float _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3;
            Unity_Clamp_float(_Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2, 0, 1, _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3);
            surface.Alpha = _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _Size;
        float _Definition;
        float _ColorNum;
        float _Dispersity;
        float _Shape;
        float _Intensity;
        float4 _BaseColor;
        float _Hue;
        CBUFFER_END
        
        // Object and Global properties
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Posterize_float4(float4 In, float4 Steps, out float4 Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        float2 Unity_Voronoi_RandomVector_Deterministic_float (float2 UV, float offset)
        {
            Hash_Tchou_2_2_float(UV, UV);
            return float2(sin(UV.y * offset), cos(UV.x * offset)) * 0.5 + 0.5;
        }
        
        void Unity_Voronoi_Deterministic_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    float2 lattice = float2(x, y);
                    float2 offset = Unity_Voronoi_RandomVector_Deterministic_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
                    if (d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Posterize_float(float In, float Steps, out float Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _UV_e653f2588110430ca6cf4ce6cce85191_Out_0 = IN.uv0;
            float _Property_9917dafac71a4cd6acab9b1356111997_Out_0 = _Definition;
            float4 _Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2;
            Unity_Posterize_float4(_UV_e653f2588110430ca6cf4ce6cce85191_Out_0, (_Property_9917dafac71a4cd6acab9b1356111997_Out_0.xxxx), _Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2);
            float _Property_c4d42add66084a568af9ea454d1ba2fa_Out_0 = _Dispersity;
            float _Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3;
            Unity_Clamp_float(_Property_c4d42add66084a568af9ea454d1ba2fa_Out_0, -0.25, 0.25, _Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3);
            float _Property_b9784b9b2d444f088b2b149890afed38_Out_0 = _Shape;
            float _Property_640084c7279a46e0bb4cdad977ce5b79_Out_0 = _Intensity;
            float _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3;
            float _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Cells_4;
            Unity_Voronoi_Deterministic_float((_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2.xy), _Property_b9784b9b2d444f088b2b149890afed38_Out_0, _Property_640084c7279a46e0bb4cdad977ce5b79_Out_0, _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3, _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Cells_4);
            float _Remap_0057d562539c4abca83d94a27d34dceb_Out_3;
            Unity_Remap_float(_Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3, float2 (0, 1), float2 (-0.5, 0.5), _Remap_0057d562539c4abca83d94a27d34dceb_Out_3);
            float _Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2;
            Unity_Multiply_float_float(_Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3, _Remap_0057d562539c4abca83d94a27d34dceb_Out_3, _Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2);
            float4 _Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2;
            Unity_Add_float4(_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2, (_Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2.xxxx), _Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2);
            float _Property_f09b459bf28f462586e7e57060b6c9ac_Out_0 = _Size;
            float _Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2;
            Unity_Divide_float(1, _Property_f09b459bf28f462586e7e57060b6c9ac_Out_0, _Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2);
            float _Float_9dd79f21b7534a9abd0d928c58e6cb4e_Out_0 = -0.5;
            float _Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2;
            Unity_Multiply_float_float(_Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2, _Float_9dd79f21b7534a9abd0d928c58e6cb4e_Out_0, _Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2);
            float2 _TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3;
            Unity_TilingAndOffset_float((_Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2.xy), (_Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2.xx), (_Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2.xx), _TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3);
            float _Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2;
            Unity_Distance_float2(_TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3, float2(0, 0), _Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2);
            float _Property_7fe511539bfe41e1a4b2928e8a63229e_Out_0 = _ColorNum;
            float _Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2;
            Unity_Posterize_float(_Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2, _Property_7fe511539bfe41e1a4b2928e8a63229e_Out_0, _Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2);
            float _Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2;
            Unity_Multiply_float_float(_Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2, 2.2, _Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2);
            float _Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3;
            Unity_Clamp_float(_Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2, 0, 1, _Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3);
            float _OneMinus_1b1228248566463f850624229cddffd1_Out_1;
            Unity_OneMinus_float(_Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3, _OneMinus_1b1228248566463f850624229cddffd1_Out_1);
            float2 _Vector2_b26d37ea39484d5ab0ea97feb0792f28_Out_0 = float2(0.5, 0.5);
            float _Distance_513c9b0903294c349493d021aa02c5a7_Out_2;
            Unity_Distance_float2((_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2.xy), _Vector2_b26d37ea39484d5ab0ea97feb0792f28_Out_0, _Distance_513c9b0903294c349493d021aa02c5a7_Out_2);
            float _Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2;
            Unity_Multiply_float_float(_Distance_513c9b0903294c349493d021aa02c5a7_Out_2, 2.2, _Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2);
            float _Remap_216e58c559ed435180c71ad602988046_Out_3;
            Unity_Remap_float(_Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2, float2 (0, 1), float2 (-2.25, 1), _Remap_216e58c559ed435180c71ad602988046_Out_3);
            float _Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2;
            Unity_Posterize_float(_Remap_216e58c559ed435180c71ad602988046_Out_3, 2, _Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2);
            float _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3;
            Unity_Clamp_float(_Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2, 0, 1, _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3);
            float _Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2;
            Unity_Subtract_float(_OneMinus_1b1228248566463f850624229cddffd1_Out_1, _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3, _Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2);
            float _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3;
            Unity_Clamp_float(_Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2, 0, 1, _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3);
            surface.Alpha = _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _Size;
        float _Definition;
        float _ColorNum;
        float _Dispersity;
        float _Shape;
        float _Intensity;
        float4 _BaseColor;
        float _Hue;
        CBUFFER_END
        
        // Object and Global properties
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Hue_Normalized_float(float3 In, float Offset, out float3 Out)
        {
            // RGB to HSV
            float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
            float4 P = lerp(float4(In.bg, K.wz), float4(In.gb, K.xy), step(In.b, In.g));
            float4 Q = lerp(float4(P.xyw, In.r), float4(In.r, P.yzx), step(P.x, In.r));
            float D = Q.x - min(Q.w, Q.y);
            float E = 1e-10;
            float V = (D == 0) ? Q.x : (Q.x + E);
            float3 hsv = float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), V);
        
            float hue = hsv.x + Offset;
            hsv.x = (hue < 0)
                    ? hue + 1
                    : (hue > 1)
                        ? hue - 1
                        : hue;
        
            // HSV to RGB
            float4 K2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
            float3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
            Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
        }
        
        void Unity_Posterize_float4(float4 In, float4 Steps, out float4 Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        float2 Unity_Voronoi_RandomVector_Deterministic_float (float2 UV, float offset)
        {
            Hash_Tchou_2_2_float(UV, UV);
            return float2(sin(UV.y * offset), cos(UV.x * offset)) * 0.5 + 0.5;
        }
        
        void Unity_Voronoi_Deterministic_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    float2 lattice = float2(x, y);
                    float2 offset = Unity_Voronoi_RandomVector_Deterministic_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
                    if (d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Posterize_float(float In, float Steps, out float Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_8a73e0694ca7426884503472e35c36fa_Out_0 = _BaseColor;
            float _Property_023022bf8c254ce4a2a5d0f23121a5b5_Out_0 = _Hue;
            float3 _Hue_d9342b9d0b654871be850e9cec4dd5c9_Out_2;
            Unity_Hue_Normalized_float((_Property_8a73e0694ca7426884503472e35c36fa_Out_0.xyz), _Property_023022bf8c254ce4a2a5d0f23121a5b5_Out_0, _Hue_d9342b9d0b654871be850e9cec4dd5c9_Out_2);
            float4 _UV_e653f2588110430ca6cf4ce6cce85191_Out_0 = IN.uv0;
            float _Property_9917dafac71a4cd6acab9b1356111997_Out_0 = _Definition;
            float4 _Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2;
            Unity_Posterize_float4(_UV_e653f2588110430ca6cf4ce6cce85191_Out_0, (_Property_9917dafac71a4cd6acab9b1356111997_Out_0.xxxx), _Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2);
            float _Property_c4d42add66084a568af9ea454d1ba2fa_Out_0 = _Dispersity;
            float _Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3;
            Unity_Clamp_float(_Property_c4d42add66084a568af9ea454d1ba2fa_Out_0, -0.25, 0.25, _Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3);
            float _Property_b9784b9b2d444f088b2b149890afed38_Out_0 = _Shape;
            float _Property_640084c7279a46e0bb4cdad977ce5b79_Out_0 = _Intensity;
            float _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3;
            float _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Cells_4;
            Unity_Voronoi_Deterministic_float((_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2.xy), _Property_b9784b9b2d444f088b2b149890afed38_Out_0, _Property_640084c7279a46e0bb4cdad977ce5b79_Out_0, _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3, _Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Cells_4);
            float _Remap_0057d562539c4abca83d94a27d34dceb_Out_3;
            Unity_Remap_float(_Voronoi_61c6252e53954a8e8ab626f9ea8be85e_Out_3, float2 (0, 1), float2 (-0.5, 0.5), _Remap_0057d562539c4abca83d94a27d34dceb_Out_3);
            float _Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2;
            Unity_Multiply_float_float(_Clamp_c4ffaf3e464e425dbbd0e096ec4349fd_Out_3, _Remap_0057d562539c4abca83d94a27d34dceb_Out_3, _Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2);
            float4 _Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2;
            Unity_Add_float4(_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2, (_Multiply_42bc9f6977464b31adc13ed0b7d48da9_Out_2.xxxx), _Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2);
            float _Property_f09b459bf28f462586e7e57060b6c9ac_Out_0 = _Size;
            float _Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2;
            Unity_Divide_float(1, _Property_f09b459bf28f462586e7e57060b6c9ac_Out_0, _Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2);
            float _Float_9dd79f21b7534a9abd0d928c58e6cb4e_Out_0 = -0.5;
            float _Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2;
            Unity_Multiply_float_float(_Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2, _Float_9dd79f21b7534a9abd0d928c58e6cb4e_Out_0, _Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2);
            float2 _TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3;
            Unity_TilingAndOffset_float((_Add_e9fe70baaf6a42978c5fa075157e49e3_Out_2.xy), (_Divide_73395cf12e5d49a0987d6d3fe66d176c_Out_2.xx), (_Multiply_dbe51045f74946c5b13e9dc71461083c_Out_2.xx), _TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3);
            float _Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2;
            Unity_Distance_float2(_TilingAndOffset_14e08373f9ff4ff7b681fad55997f18d_Out_3, float2(0, 0), _Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2);
            float _Property_7fe511539bfe41e1a4b2928e8a63229e_Out_0 = _ColorNum;
            float _Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2;
            Unity_Posterize_float(_Distance_df32fb518f0c4ea6b9b72d0c2f03fb95_Out_2, _Property_7fe511539bfe41e1a4b2928e8a63229e_Out_0, _Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2);
            float _Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2;
            Unity_Multiply_float_float(_Posterize_37f72e7bb8934eae9c74fa9a5ff8f802_Out_2, 2.2, _Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2);
            float _Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3;
            Unity_Clamp_float(_Multiply_9561f6d9ab964c8eb1b20055f68f5d15_Out_2, 0, 1, _Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3);
            float _OneMinus_1b1228248566463f850624229cddffd1_Out_1;
            Unity_OneMinus_float(_Clamp_a3c8dda26c2b4e43bdf50160fc762505_Out_3, _OneMinus_1b1228248566463f850624229cddffd1_Out_1);
            float2 _Vector2_b26d37ea39484d5ab0ea97feb0792f28_Out_0 = float2(0.5, 0.5);
            float _Distance_513c9b0903294c349493d021aa02c5a7_Out_2;
            Unity_Distance_float2((_Posterize_d8c10dea2e6843459c9566912793e1d1_Out_2.xy), _Vector2_b26d37ea39484d5ab0ea97feb0792f28_Out_0, _Distance_513c9b0903294c349493d021aa02c5a7_Out_2);
            float _Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2;
            Unity_Multiply_float_float(_Distance_513c9b0903294c349493d021aa02c5a7_Out_2, 2.2, _Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2);
            float _Remap_216e58c559ed435180c71ad602988046_Out_3;
            Unity_Remap_float(_Multiply_77c2893e303e44e0b54ce68fce6906c4_Out_2, float2 (0, 1), float2 (-2.25, 1), _Remap_216e58c559ed435180c71ad602988046_Out_3);
            float _Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2;
            Unity_Posterize_float(_Remap_216e58c559ed435180c71ad602988046_Out_3, 2, _Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2);
            float _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3;
            Unity_Clamp_float(_Posterize_b0c76225594444aaa5a1135eceed5db7_Out_2, 0, 1, _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3);
            float _Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2;
            Unity_Subtract_float(_OneMinus_1b1228248566463f850624229cddffd1_Out_1, _Clamp_794b04795dc24ba8b467de7d04c444d8_Out_3, _Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2);
            float _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3;
            Unity_Clamp_float(_Subtract_9a1fdfba71cd4b4ea3e1be1d8911eb4d_Out_2, 0, 1, _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3);
            surface.BaseColor = _Hue_d9342b9d0b654871be850e9cec4dd5c9_Out_2;
            surface.Alpha = _Clamp_20c4bf2778ce4bf4ab4db41502b07a98_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}
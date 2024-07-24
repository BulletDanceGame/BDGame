Shader "BulletDance/UI/LoadHalftone"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 1)
        [NoScaleOffset]_MainTex("Texture2D", 2D) = "white" {}
        _CircleAmount("CircleAmount", Float) = 12
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
             float3 TimeParameters;
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
        float4 _Color;
        float4 _MainTex_TexelSize;
        float _CircleAmount;
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
        
        void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
        {
            float2 delta = UV - Center;
            float radius = length(delta) * 2 * RadialScale;
            float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
            Out = float2(radius, angle);
        }
        
        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);
        
            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;
        
            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;
        
            Out = UV;
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
        
        void Unity_Posterize_float(float In, float Steps, out float Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
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
            float2 _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1.5, 0, _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4);
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_R_1 = _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4[0];
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_G_2 = _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4[1];
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_B_3 = 0;
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_A_4 = 0;
            float _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1;
            Unity_Absolute_float(IN.TimeParameters.y, _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1);
            float _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2;
            Unity_Multiply_float_float(0.1, _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1, _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2);
            float _Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2;
            Unity_Add_float(-0.3, _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2, _Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2);
            float _Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1;
            Unity_Absolute_float(IN.TimeParameters.z, _Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1);
            float _Multiply_b72a789202c2477fadf6ff1179995afc_Out_2;
            Unity_Multiply_float_float(_Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1, 0.35, _Multiply_b72a789202c2477fadf6ff1179995afc_Out_2);
            float _Add_4584cca0594642c198c5382c4b82b2e7_Out_2;
            Unity_Add_float(_Multiply_b72a789202c2477fadf6ff1179995afc_Out_2, 0.8, _Add_4584cca0594642c198c5382c4b82b2e7_Out_2);
            float2 _Vector2_8046e33d1ecb482399183095481e455e_Out_0 = float2(_Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2, _Add_4584cca0594642c198c5382c4b82b2e7_Out_2);
            float _Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3;
            Unity_Remap_float(_Split_53f04b4f880c4bd8bcb0dcdd7c714645_R_1, float2 (0, 1), _Vector2_8046e33d1ecb482399183095481e455e_Out_0, _Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3);
            float2 _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 0.8, 0, _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4);
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_R_1 = _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4[0];
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_G_2 = _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4[1];
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_B_3 = 0;
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_A_4 = 0;
            float _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1;
            Unity_Absolute_float(IN.TimeParameters.y, _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1);
            float _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2;
            Unity_Multiply_float_float(0.5, _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1, _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2);
            float _Add_78af77dbe04d4502b51cc078272d0c18_Out_2;
            Unity_Add_float(-0.7, _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2, _Add_78af77dbe04d4502b51cc078272d0c18_Out_2);
            float _Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1;
            Unity_Absolute_float(IN.TimeParameters.z, _Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1);
            float _Multiply_379b8f47451f45429190142363595c3e_Out_2;
            Unity_Multiply_float_float(_Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1, 1, _Multiply_379b8f47451f45429190142363595c3e_Out_2);
            float _Add_a53bb492af23452f8f56d8a624ced470_Out_2;
            Unity_Add_float(_Multiply_379b8f47451f45429190142363595c3e_Out_2, 1.2, _Add_a53bb492af23452f8f56d8a624ced470_Out_2);
            float2 _Vector2_fa996f30c6e0436488d603c254a7d31f_Out_0 = float2(_Add_78af77dbe04d4502b51cc078272d0c18_Out_2, _Add_a53bb492af23452f8f56d8a624ced470_Out_2);
            float _Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3;
            Unity_Remap_float(_Split_8b49c4f5d708471886c0fd640dbd42aa_R_1, float2 (0, 1), _Vector2_fa996f30c6e0436488d603c254a7d31f_Out_0, _Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3);
            float _OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1;
            Unity_OneMinus_float(_Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3, _OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1);
            float _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2;
            Unity_Multiply_float_float(-15, IN.TimeParameters.x, _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2);
            float2 _Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3;
            Unity_Rotate_Degrees_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2, _Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3);
            float _Property_a62598059b5e4a6184d9f020e3694739_Out_0 = _CircleAmount;
            float _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3;
            float _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Cells_4;
            Unity_Voronoi_Deterministic_float(_Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3, 0, _Property_a62598059b5e4a6184d9f020e3694739_Out_0, _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3, _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Cells_4);
            float _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1;
            Unity_OneMinus_float(_Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3, _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1);
            float _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2;
            Unity_Multiply_float_float(_OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1, _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1, _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2);
            float _Multiply_aa031044193640438312506b9f365642_Out_2;
            Unity_Multiply_float_float(_Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3, _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2, _Multiply_aa031044193640438312506b9f365642_Out_2);
            float _Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2;
            Unity_Posterize_float(_Multiply_aa031044193640438312506b9f365642_Out_2, 5, _Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2);
            float _Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3;
            Unity_Clamp_float(_Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2, 0, 1, _Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3);
            float _Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3;
            Unity_Remap_float(_Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3, float2 (0, 1), float2 (0, 4), _Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3);
            float _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3;
            Unity_Clamp_float(_Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3, 0, 1, _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3);
            surface.BaseColor = IsGammaSpace() ? float3(1, 1, 1) : SRGBToLinear(float3(1, 1, 1));
            surface.Alpha = _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3;
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
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
        
        #define _ALPHATEST_ON 1
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
             float3 TimeParameters;
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
        float4 _Color;
        float4 _MainTex_TexelSize;
        float _CircleAmount;
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
        
        void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
        {
            float2 delta = UV - Center;
            float radius = length(delta) * 2 * RadialScale;
            float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
            Out = float2(radius, angle);
        }
        
        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);
        
            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;
        
            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;
        
            Out = UV;
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
        
        void Unity_Posterize_float(float In, float Steps, out float Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
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
            float2 _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1.5, 0, _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4);
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_R_1 = _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4[0];
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_G_2 = _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4[1];
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_B_3 = 0;
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_A_4 = 0;
            float _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1;
            Unity_Absolute_float(IN.TimeParameters.y, _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1);
            float _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2;
            Unity_Multiply_float_float(0.1, _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1, _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2);
            float _Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2;
            Unity_Add_float(-0.3, _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2, _Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2);
            float _Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1;
            Unity_Absolute_float(IN.TimeParameters.z, _Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1);
            float _Multiply_b72a789202c2477fadf6ff1179995afc_Out_2;
            Unity_Multiply_float_float(_Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1, 0.35, _Multiply_b72a789202c2477fadf6ff1179995afc_Out_2);
            float _Add_4584cca0594642c198c5382c4b82b2e7_Out_2;
            Unity_Add_float(_Multiply_b72a789202c2477fadf6ff1179995afc_Out_2, 0.8, _Add_4584cca0594642c198c5382c4b82b2e7_Out_2);
            float2 _Vector2_8046e33d1ecb482399183095481e455e_Out_0 = float2(_Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2, _Add_4584cca0594642c198c5382c4b82b2e7_Out_2);
            float _Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3;
            Unity_Remap_float(_Split_53f04b4f880c4bd8bcb0dcdd7c714645_R_1, float2 (0, 1), _Vector2_8046e33d1ecb482399183095481e455e_Out_0, _Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3);
            float2 _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 0.8, 0, _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4);
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_R_1 = _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4[0];
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_G_2 = _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4[1];
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_B_3 = 0;
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_A_4 = 0;
            float _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1;
            Unity_Absolute_float(IN.TimeParameters.y, _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1);
            float _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2;
            Unity_Multiply_float_float(0.5, _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1, _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2);
            float _Add_78af77dbe04d4502b51cc078272d0c18_Out_2;
            Unity_Add_float(-0.7, _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2, _Add_78af77dbe04d4502b51cc078272d0c18_Out_2);
            float _Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1;
            Unity_Absolute_float(IN.TimeParameters.z, _Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1);
            float _Multiply_379b8f47451f45429190142363595c3e_Out_2;
            Unity_Multiply_float_float(_Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1, 1, _Multiply_379b8f47451f45429190142363595c3e_Out_2);
            float _Add_a53bb492af23452f8f56d8a624ced470_Out_2;
            Unity_Add_float(_Multiply_379b8f47451f45429190142363595c3e_Out_2, 1.2, _Add_a53bb492af23452f8f56d8a624ced470_Out_2);
            float2 _Vector2_fa996f30c6e0436488d603c254a7d31f_Out_0 = float2(_Add_78af77dbe04d4502b51cc078272d0c18_Out_2, _Add_a53bb492af23452f8f56d8a624ced470_Out_2);
            float _Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3;
            Unity_Remap_float(_Split_8b49c4f5d708471886c0fd640dbd42aa_R_1, float2 (0, 1), _Vector2_fa996f30c6e0436488d603c254a7d31f_Out_0, _Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3);
            float _OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1;
            Unity_OneMinus_float(_Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3, _OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1);
            float _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2;
            Unity_Multiply_float_float(-15, IN.TimeParameters.x, _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2);
            float2 _Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3;
            Unity_Rotate_Degrees_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2, _Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3);
            float _Property_a62598059b5e4a6184d9f020e3694739_Out_0 = _CircleAmount;
            float _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3;
            float _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Cells_4;
            Unity_Voronoi_Deterministic_float(_Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3, 0, _Property_a62598059b5e4a6184d9f020e3694739_Out_0, _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3, _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Cells_4);
            float _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1;
            Unity_OneMinus_float(_Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3, _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1);
            float _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2;
            Unity_Multiply_float_float(_OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1, _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1, _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2);
            float _Multiply_aa031044193640438312506b9f365642_Out_2;
            Unity_Multiply_float_float(_Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3, _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2, _Multiply_aa031044193640438312506b9f365642_Out_2);
            float _Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2;
            Unity_Posterize_float(_Multiply_aa031044193640438312506b9f365642_Out_2, 5, _Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2);
            float _Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3;
            Unity_Clamp_float(_Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2, 0, 1, _Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3);
            float _Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3;
            Unity_Remap_float(_Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3, float2 (0, 1), float2 (0, 4), _Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3);
            float _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3;
            Unity_Clamp_float(_Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3, 0, 1, _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3);
            surface.Alpha = _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3;
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
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
        
        #define _ALPHATEST_ON 1
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
             float3 TimeParameters;
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
        float4 _Color;
        float4 _MainTex_TexelSize;
        float _CircleAmount;
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
        
        void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
        {
            float2 delta = UV - Center;
            float radius = length(delta) * 2 * RadialScale;
            float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
            Out = float2(radius, angle);
        }
        
        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);
        
            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;
        
            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;
        
            Out = UV;
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
        
        void Unity_Posterize_float(float In, float Steps, out float Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
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
            float2 _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1.5, 0, _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4);
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_R_1 = _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4[0];
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_G_2 = _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4[1];
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_B_3 = 0;
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_A_4 = 0;
            float _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1;
            Unity_Absolute_float(IN.TimeParameters.y, _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1);
            float _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2;
            Unity_Multiply_float_float(0.1, _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1, _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2);
            float _Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2;
            Unity_Add_float(-0.3, _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2, _Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2);
            float _Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1;
            Unity_Absolute_float(IN.TimeParameters.z, _Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1);
            float _Multiply_b72a789202c2477fadf6ff1179995afc_Out_2;
            Unity_Multiply_float_float(_Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1, 0.35, _Multiply_b72a789202c2477fadf6ff1179995afc_Out_2);
            float _Add_4584cca0594642c198c5382c4b82b2e7_Out_2;
            Unity_Add_float(_Multiply_b72a789202c2477fadf6ff1179995afc_Out_2, 0.8, _Add_4584cca0594642c198c5382c4b82b2e7_Out_2);
            float2 _Vector2_8046e33d1ecb482399183095481e455e_Out_0 = float2(_Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2, _Add_4584cca0594642c198c5382c4b82b2e7_Out_2);
            float _Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3;
            Unity_Remap_float(_Split_53f04b4f880c4bd8bcb0dcdd7c714645_R_1, float2 (0, 1), _Vector2_8046e33d1ecb482399183095481e455e_Out_0, _Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3);
            float2 _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 0.8, 0, _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4);
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_R_1 = _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4[0];
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_G_2 = _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4[1];
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_B_3 = 0;
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_A_4 = 0;
            float _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1;
            Unity_Absolute_float(IN.TimeParameters.y, _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1);
            float _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2;
            Unity_Multiply_float_float(0.5, _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1, _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2);
            float _Add_78af77dbe04d4502b51cc078272d0c18_Out_2;
            Unity_Add_float(-0.7, _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2, _Add_78af77dbe04d4502b51cc078272d0c18_Out_2);
            float _Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1;
            Unity_Absolute_float(IN.TimeParameters.z, _Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1);
            float _Multiply_379b8f47451f45429190142363595c3e_Out_2;
            Unity_Multiply_float_float(_Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1, 1, _Multiply_379b8f47451f45429190142363595c3e_Out_2);
            float _Add_a53bb492af23452f8f56d8a624ced470_Out_2;
            Unity_Add_float(_Multiply_379b8f47451f45429190142363595c3e_Out_2, 1.2, _Add_a53bb492af23452f8f56d8a624ced470_Out_2);
            float2 _Vector2_fa996f30c6e0436488d603c254a7d31f_Out_0 = float2(_Add_78af77dbe04d4502b51cc078272d0c18_Out_2, _Add_a53bb492af23452f8f56d8a624ced470_Out_2);
            float _Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3;
            Unity_Remap_float(_Split_8b49c4f5d708471886c0fd640dbd42aa_R_1, float2 (0, 1), _Vector2_fa996f30c6e0436488d603c254a7d31f_Out_0, _Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3);
            float _OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1;
            Unity_OneMinus_float(_Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3, _OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1);
            float _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2;
            Unity_Multiply_float_float(-15, IN.TimeParameters.x, _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2);
            float2 _Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3;
            Unity_Rotate_Degrees_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2, _Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3);
            float _Property_a62598059b5e4a6184d9f020e3694739_Out_0 = _CircleAmount;
            float _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3;
            float _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Cells_4;
            Unity_Voronoi_Deterministic_float(_Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3, 0, _Property_a62598059b5e4a6184d9f020e3694739_Out_0, _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3, _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Cells_4);
            float _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1;
            Unity_OneMinus_float(_Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3, _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1);
            float _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2;
            Unity_Multiply_float_float(_OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1, _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1, _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2);
            float _Multiply_aa031044193640438312506b9f365642_Out_2;
            Unity_Multiply_float_float(_Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3, _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2, _Multiply_aa031044193640438312506b9f365642_Out_2);
            float _Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2;
            Unity_Posterize_float(_Multiply_aa031044193640438312506b9f365642_Out_2, 5, _Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2);
            float _Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3;
            Unity_Clamp_float(_Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2, 0, 1, _Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3);
            float _Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3;
            Unity_Remap_float(_Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3, float2 (0, 1), float2 (0, 4), _Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3);
            float _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3;
            Unity_Clamp_float(_Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3, 0, 1, _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3);
            surface.Alpha = _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3;
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
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
             float3 TimeParameters;
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
        float4 _Color;
        float4 _MainTex_TexelSize;
        float _CircleAmount;
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
        
        void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
        {
            float2 delta = UV - Center;
            float radius = length(delta) * 2 * RadialScale;
            float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
            Out = float2(radius, angle);
        }
        
        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);
        
            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;
        
            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;
        
            Out = UV;
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
        
        void Unity_Posterize_float(float In, float Steps, out float Out)
        {
            Out = floor(In / (1 / Steps)) * (1 / Steps);
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
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
            float2 _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1.5, 0, _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4);
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_R_1 = _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4[0];
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_G_2 = _PolarCoordinates_ad35f33ae98a4e1cbb652e052423ff82_Out_4[1];
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_B_3 = 0;
            float _Split_53f04b4f880c4bd8bcb0dcdd7c714645_A_4 = 0;
            float _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1;
            Unity_Absolute_float(IN.TimeParameters.y, _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1);
            float _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2;
            Unity_Multiply_float_float(0.1, _Absolute_7dbce6f3c0eb41a4ac1ed7054eb2c70d_Out_1, _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2);
            float _Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2;
            Unity_Add_float(-0.3, _Multiply_d52641cb23414e16a1bc3445140fac88_Out_2, _Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2);
            float _Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1;
            Unity_Absolute_float(IN.TimeParameters.z, _Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1);
            float _Multiply_b72a789202c2477fadf6ff1179995afc_Out_2;
            Unity_Multiply_float_float(_Absolute_4e24af03238b47a6a6fc9c00ecbd09e7_Out_1, 0.35, _Multiply_b72a789202c2477fadf6ff1179995afc_Out_2);
            float _Add_4584cca0594642c198c5382c4b82b2e7_Out_2;
            Unity_Add_float(_Multiply_b72a789202c2477fadf6ff1179995afc_Out_2, 0.8, _Add_4584cca0594642c198c5382c4b82b2e7_Out_2);
            float2 _Vector2_8046e33d1ecb482399183095481e455e_Out_0 = float2(_Add_6bab2dff0848428fa9111f93eab9b9f5_Out_2, _Add_4584cca0594642c198c5382c4b82b2e7_Out_2);
            float _Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3;
            Unity_Remap_float(_Split_53f04b4f880c4bd8bcb0dcdd7c714645_R_1, float2 (0, 1), _Vector2_8046e33d1ecb482399183095481e455e_Out_0, _Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3);
            float2 _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 0.8, 0, _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4);
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_R_1 = _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4[0];
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_G_2 = _PolarCoordinates_4be9f01f5f7b430f9321633f04942d03_Out_4[1];
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_B_3 = 0;
            float _Split_8b49c4f5d708471886c0fd640dbd42aa_A_4 = 0;
            float _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1;
            Unity_Absolute_float(IN.TimeParameters.y, _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1);
            float _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2;
            Unity_Multiply_float_float(0.5, _Absolute_b14c3048e2224c2c93c33845f3fed4f5_Out_1, _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2);
            float _Add_78af77dbe04d4502b51cc078272d0c18_Out_2;
            Unity_Add_float(-0.7, _Multiply_0dd81bd9b8fd416d873bd19b341aadd8_Out_2, _Add_78af77dbe04d4502b51cc078272d0c18_Out_2);
            float _Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1;
            Unity_Absolute_float(IN.TimeParameters.z, _Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1);
            float _Multiply_379b8f47451f45429190142363595c3e_Out_2;
            Unity_Multiply_float_float(_Absolute_5d5ff11c71594bbcb0e175d2e1c966bf_Out_1, 1, _Multiply_379b8f47451f45429190142363595c3e_Out_2);
            float _Add_a53bb492af23452f8f56d8a624ced470_Out_2;
            Unity_Add_float(_Multiply_379b8f47451f45429190142363595c3e_Out_2, 1.2, _Add_a53bb492af23452f8f56d8a624ced470_Out_2);
            float2 _Vector2_fa996f30c6e0436488d603c254a7d31f_Out_0 = float2(_Add_78af77dbe04d4502b51cc078272d0c18_Out_2, _Add_a53bb492af23452f8f56d8a624ced470_Out_2);
            float _Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3;
            Unity_Remap_float(_Split_8b49c4f5d708471886c0fd640dbd42aa_R_1, float2 (0, 1), _Vector2_fa996f30c6e0436488d603c254a7d31f_Out_0, _Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3);
            float _OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1;
            Unity_OneMinus_float(_Remap_92c540effa2a4b759751ee6c2ba88f9d_Out_3, _OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1);
            float _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2;
            Unity_Multiply_float_float(-15, IN.TimeParameters.x, _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2);
            float2 _Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3;
            Unity_Rotate_Degrees_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_052f1a2aea7f448da4b5515059d7d7b2_Out_2, _Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3);
            float _Property_a62598059b5e4a6184d9f020e3694739_Out_0 = _CircleAmount;
            float _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3;
            float _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Cells_4;
            Unity_Voronoi_Deterministic_float(_Rotate_5bdc7bd803e94e54a77aa827bf9abcde_Out_3, 0, _Property_a62598059b5e4a6184d9f020e3694739_Out_0, _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3, _Voronoi_aced490cf1fe40cea099ea6af8de3e18_Cells_4);
            float _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1;
            Unity_OneMinus_float(_Voronoi_aced490cf1fe40cea099ea6af8de3e18_Out_3, _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1);
            float _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2;
            Unity_Multiply_float_float(_OneMinus_c894e7b3a6074dfab7e58ae3cce92d78_Out_1, _OneMinus_03ea8065657148ebbf23bcd24fac3766_Out_1, _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2);
            float _Multiply_aa031044193640438312506b9f365642_Out_2;
            Unity_Multiply_float_float(_Remap_ae90817ab0944d898e2fb188e1eecb2a_Out_3, _Multiply_cb1968e658044de6afbbc6e9d19527ae_Out_2, _Multiply_aa031044193640438312506b9f365642_Out_2);
            float _Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2;
            Unity_Posterize_float(_Multiply_aa031044193640438312506b9f365642_Out_2, 5, _Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2);
            float _Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3;
            Unity_Clamp_float(_Posterize_f7b2efc13fff403cb14835c8153d053b_Out_2, 0, 1, _Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3);
            float _Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3;
            Unity_Remap_float(_Clamp_bc709f2b3f3e4280bc952f012b2c0478_Out_3, float2 (0, 1), float2 (0, 4), _Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3);
            float _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3;
            Unity_Clamp_float(_Remap_b1fbe8503d73484c9daf563becf6e4c4_Out_3, 0, 1, _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3);
            surface.BaseColor = IsGammaSpace() ? float3(1, 1, 1) : SRGBToLinear(float3(1, 1, 1));
            surface.Alpha = _Clamp_58efc46006a54a79a1febf67829c09a1_Out_3;
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
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
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
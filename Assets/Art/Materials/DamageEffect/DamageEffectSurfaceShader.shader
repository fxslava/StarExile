Shader "Custom/DamageEffectSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _DamageTex ("Damage", 3D) = "black" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _DamageGridInvExtents ("Damage Grid Inverted Extents (XYZ)", Vector) = (1,1,1,1)
        _HoleThreashold ("Hole Threashold", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler3D _DamageTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float4 _DamageGridInvExtents;
        float _HoleThreashold;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        
        
        float3 ThermoGrad (float value)
        {
           return (value < 0.5) ? 
               lerp(float3(0,0,0), float3(1.f, 0.f, 0.f), saturate(value * 2)) : 
               lerp(float3(1.f, 0.f, 0.f), float3(1.f, 1.f, 0.f), saturate(value * 2 - 1));
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 uvw = (mul(unity_WorldToObject, float4(IN.worldPos, 1)).xyz * _DamageGridInvExtents.xyz + 1.0f) / 2.0f;
            float2 damage = tex3D(_DamageTex, uvw).rg;

            if (damage.r > _HoleThreashold)
                discard;

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb * (1.0f - damage.r * 2.0);
            o.Emission = ThermoGrad(damage.g * 2.0) * damage.g;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

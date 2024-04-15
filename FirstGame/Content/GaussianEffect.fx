#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

bool isDiffuse;

float2 _sampleOffsets[5];

float center = 0.2270270270f;
float close = 0.3162162162f;
float far = 0.0702702703f;

Texture2D RenderTargetTexture;
sampler2D RenderTargetSampler = sampler_state
{
    Texture = <RenderTargetTexture>;
};

//struct VertexShaderInput
//{
//    float4 Position : POSITION0;
//    float2 TexCoords : TEXCOORD0;
//};

//struct VertexShaderOutput
//{
//    float4 Position : POSITION0;
//    float2 TexCoords : TEXCOORD0;
//};

//VertexShaderOutput MainVS(in VertexShaderInput input)
//{
//    VertexShaderOutput output = (VertexShaderOutput) 0;
	
//    output.Position = input.Position;
//    output.TexCoords = input.TexCoords;

//    return output;
//}

float4 MainPS(float2 TexCoords : TEXCOORD0) : COLOR
{
    float4 FragColor = { 0, 0, 1, 1 };
    FragColor.rgb =
            0.0702702703f * tex2D(RenderTargetSampler, TexCoords + _sampleOffsets[0]).rgb +
            0.3162162162f * tex2D(RenderTargetSampler, TexCoords + _sampleOffsets[1]).rgb +
            0.2270270270f * tex2D(RenderTargetSampler, TexCoords + _sampleOffsets[2]).rgb +
            0.3162162162f * tex2D(RenderTargetSampler, TexCoords + _sampleOffsets[3]).rgb +
            0.0702702703f * tex2D(RenderTargetSampler, TexCoords + _sampleOffsets[4]).rgb;
    return FragColor;
}

technique BasicColorDrawing
{
    pass P0
    {
        //VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
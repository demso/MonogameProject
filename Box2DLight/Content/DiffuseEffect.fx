#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//matrix WorldViewProjection;
float4 Ambient;
Texture2D RenderTargetTexture;
sampler2D RenderTargetSampler = sampler_state
{
	Texture = <RenderTargetTexture>;
};

struct VertexShaderInput
{
	float4 APosition : POSITION0;
	float2 ATexCoords : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoords : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = input.APosition; //mul(input.APosition, WorldViewProjection);
	output.TexCoords = input.APosition;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    Ambient.a = 1.0;
    return Ambient.rgb = (Ambient.rgb + tex2D(RenderTargetSampler, input.TexCoords).rgb);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
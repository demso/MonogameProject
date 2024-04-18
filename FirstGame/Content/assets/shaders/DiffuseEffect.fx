#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4 Ambient;
Texture2D RenderTargetTexture;
sampler2D RenderTargetSampler = sampler_state
{
	Texture = <RenderTargetTexture>;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoords : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoords : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = input.Position;
	output.TexCoords = input.TexCoords;

	return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    float4 tempColor = Ambient;
    tempColor.a = 1.0;
    tempColor.rgb = (Ambient.rgb + tex2D(RenderTargetSampler, input.TexCoords).rgb);
    return tempColor;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
bool isDiffuse;
uniform float FBO_W;
uniform float FBO_H;
float2 dir;
static float2 futher;
static float2 closer;
static const float center = 0.2270270270;
static const float close = 0.3162162162;
static const float far = 0.0702702703;
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
    float2 TexCoords0 : TEXCOORD0;
    float2 TexCoords1 : TEXCOORD1;
    float2 TexCoords2 : TEXCOORD2;
    float2 TexCoords3 : TEXCOORD3;
    float2 TexCoords4 : TEXCOORD4;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
    
    futher.x = 3.2307692308 / FBO_W;
    futher.y = 3.2307692308 / FBO_H;
    closer.x = 1.3846153846 / FBO_W;
    closer.y = 1.3846153846 / FBO_H;

    float2 f = futher * dir;
    float2 c = closer * dir;
	
    output.Position = input.Position;
    output.TexCoords0 = input.TexCoords - f;
    output.TexCoords1 = input.TexCoords - c;
    output.TexCoords2 = input.TexCoords;
    output.TexCoords3 = input.TexCoords + c;
    output.TexCoords4 = input.TexCoords + f;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 FragColor = {0, 0, 0, 1};
    if (isDiffuse == true)
    {
        FragColor.rgb =
            far * tex2D(RenderTargetSampler, input.TexCoords0).rgb +
            close * tex2D(RenderTargetSampler, input.TexCoords1).rgb +
            center * tex2D(RenderTargetSampler, input.TexCoords2).rgb +
            close * tex2D(RenderTargetSampler, input.TexCoords3).rgb +
            far * tex2D(RenderTargetSampler, input.TexCoords4).rgb;
        return FragColor;
    }
    else
    {
        FragColor =
            far * tex2D(RenderTargetSampler, input.TexCoords0) +
            close * tex2D(RenderTargetSampler, input.TexCoords1) +
            center * tex2D(RenderTargetSampler, input.TexCoords2) +
            close * tex2D(RenderTargetSampler, input.TexCoords3) +
            far * tex2D(RenderTargetSampler, input.TexCoords4);
        return FragColor;
    }
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
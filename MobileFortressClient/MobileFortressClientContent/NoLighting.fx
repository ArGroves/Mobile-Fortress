float4x4 ViewProjection : VIEWPROJECTION;
float4x4 World : WORLD;
float4x3 WorldIT : WORLDINVERSETRANSPOSE;
float3 ViewPosition : VIEWPOSITION;
float3 LightDir : LIGHTDIR0_DIRECTION;
float3 LightColor : LIGHTDIR0_COLOR;
float3 materialEmissive : EMISSIVE;
float3 materialAmbient : AMBIENT;
float4 materialDiffuse : DIFFUSE;
float3 materialSpecular : SPECULAR;
float3 materialSpecPower : SPECULARPOWER;
texture xTexture : TEXTURE0;
sampler sampTexture = sampler_state {
	Texture = (xTexture);
};
bool enableFog;
float fogStart;
float fogEnd;
float3 fogColor;

struct TVSInput
{
    float4 Position : POSITION;
	float3 Normal : NORMAL;
	float2 TexCoord : TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct TVSOutput
{
    float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 PixPos : TEXCOORD2;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

#define TPSInput TVSOutput

TVSOutput TVertexShaderFunction(TVSInput In)
{	
    TVSOutput Out;

	float4x4 preWorldViewProjection = mul(World, ViewProjection);

	Out.Position = (float4)mul(In.Position, preWorldViewProjection);
	Out.PixPos = (float3)Out.Position;
	Out.TexCoord = In.TexCoord;
	Out.Normal = mul(In.Normal, WorldIT);

    return Out;
}

float4 TPixelShaderFunction(TPSInput In) : COLOR
{
	// Sample the texture
	float4 texColor = tex2D(sampTexture, In.TexCoord);
 
	// Combine all the color components
	float3 color = texColor;

	if(enableFog)
	{
		float l = saturate((In.PixPos.z - fogStart) / (fogEnd - fogStart));
		color = lerp(color,fogColor,l);
	}


	// Return the pixel's color
	return float4(color,1);
}

technique TTexSM2
{
    pass P
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 TVertexShaderFunction();
        PixelShader = compile ps_2_0 TPixelShaderFunction();
    }
}
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
texture xTexture0;
sampler sampTexture0 = sampler_state {
	Texture = (xTexture0);
};
texture xTexture1;
sampler sampTexture1 = sampler_state {
	Texture = (xTexture1);
};
texture xTexture2;
sampler sampTexture2 = sampler_state {
	Texture = (xTexture2);
};
texture xTexture3;
sampler sampTexture3 = sampler_state {
	Texture = (xTexture3);
};
bool enableFog;
float fogStart;
float fogEnd;
float3 fogColor;

float mipStart;
float mipEnd;

struct VSInput
{
    float4 Position : POSITION;
	float3 Normal : NORMAL;
	float2 TexCoord : TEXCOORD0;
	float4 TexWeights : TEXCOORD1;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VSOutput
{
    float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 PixPos : TEXCOORD2;
	float4 TexWeights : TEXCOORD3;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

#define PSInput VSOutput

VSOutput VertexShaderFunction(VSInput In)
{	
    float4x4 preWorldViewProjection = mul(World,ViewProjection);

    VSOutput Out;

	Out.Position = mul(In.Position, preWorldViewProjection);
	Out.PixPos = Out.Position;
	Out.TexCoord = In.TexCoord;
	Out.Normal = mul(In.Normal, WorldIT);
	Out.TexWeights = In.TexWeights;

    return Out;
}

float4 PixelShaderFunction(PSInput In) : COLOR
{
    float3 preLight = normalize(-LightDir);
 
	// Calculate the diffuse reflection
	float3 diffuse = saturate(dot(In.Normal, preLight) * materialDiffuse.rgb);
 
	// Sample the texture
	float4 texColor = tex2D(sampTexture0, In.TexCoord) * In.TexWeights.x;
	texColor += tex2D(sampTexture1, In.TexCoord) * In.TexWeights.y;
	texColor += tex2D(sampTexture2, In.TexCoord) * In.TexWeights.z;
	texColor += tex2D(sampTexture3, In.TexCoord) * In.TexWeights.w;
 
	// Combine all the color components
	float3 color = (saturate(materialAmbient+diffuse)*texColor) * LightColor + materialEmissive;

	if(enableFog)
	{
		float l = saturate((In.PixPos.z - fogStart) / (fogEnd - fogStart));
		color = lerp(color,fogColor,l);
	}


	// Return the pixel's color
	return float4(color,materialDiffuse.a);
}

technique Simple
{
    pass P
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
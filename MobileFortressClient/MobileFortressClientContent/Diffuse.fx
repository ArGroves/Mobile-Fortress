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

bool customColors;
float4 customColorA;



struct VSInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL;
	//float2 TexCoord : TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VSOutput
{
    float4 Position : POSITION0;
	//float2 TexCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 PixPos : TEXCOORD2;

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
	//Out.TexCoord = In.TexCoord;
	Out.Normal = mul(In.Normal, WorldIT);

    return Out;
}

float4 PixelShaderFunction(PSInput In) : COLOR
{
    float3 preLight = normalize(-LightDir);
	float3 normal = In.Normal;
 
	// Calculate the diffuse reflection
	float3 diffuse = saturate(dot(normal, preLight) * materialDiffuse.rgb);
 
	// Fetch the texture coordinates
	//float2 texCoord = In.TexCoord;
 
	// Sample the texture
	float4 texColor = float4(1,1,1,1); //tex2D(sampTexture, texCoord);
 
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

technique TSM2
{
    pass P
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

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
    float3 preLight = normalize(-LightDir);
 
	// Calculate the diffuse reflection
	float3 diffuse = saturate(dot(In.Normal, preLight) * materialDiffuse.rgb);
 
	// Sample the texture
	float4 texColor = tex2D(sampTexture, In.TexCoord);

	if(customColors)
	{
		if(texColor.r == texColor.g && texColor.g == texColor.b)
		{
			texColor = customColorA * texColor;
		}
	}
 
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

technique TTexSM2
{
    pass P
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 TVertexShaderFunction();
        PixelShader = compile ps_2_0 TPixelShaderFunction();
    }
}
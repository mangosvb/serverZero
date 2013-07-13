//
// Terrain.fx
// 
// This file is part of the QuickStart Game Engine.  See http://www.codeplex.com/QuickStartEngine
// for license details.
//

//------- Constants --------
float4x4 View				: VIEW;
float4x4 Projection			: PROJECTION;
float4x4 World				: WORLD;
float4x4 WorldViewProj		: WORLDVIEWPROJECTION;
float4 LightDirection		: LIGHT_DIRECTION;
float4 AmbientColor			: AMBIENT_COLOR;
float AmbientPower			: AMBIENT_POWER;
float4 SpecularColor		: SPECULAR_COLOR;
float SpecularPower			: SPECULAR_POWER;
float4 DiffuseColor			: DIFFUSE_COLOR;
float4 CameraForward		: VIEW_FORWARD;
float4 CameraPos			: VIEW_POS;
float fogNear				: FOG_NEAR;
float fogFar				: FOG_FAR;
float4 fogColor				: FOG_COLOR;
float fogAltitudeScale		: FOG_ALTITUDE;
float fogThinning			: FOG_THINNING;

float TerrainScale			: SCALE_FACTOR;
float TerrainWidth			: TERRAIN_WIDTH;

//------- Texture Samplers --------
Texture TextureMap			: TEXTURE_MAP;
sampler TextureMapSampler = sampler_state { texture = <TextureMap> ; magfilter = LINEAR; minfilter = LINEAR; 
                                                                         mipfilter = LINEAR; AddressU  = Wrap;
                                                                         AddressV  = Wrap; AddressW  = Wrap;};

Texture GrassTexture		: GRASS_TEXTURE;
sampler GrassTextureSampler = sampler_state { texture = <GrassTexture> ; magfilter = LINEAR; minfilter = LINEAR; 
                                                                         mipfilter=LINEAR; AddressU  = Wrap;
                                                                         AddressV  = Wrap; AddressW  = Wrap;};

Texture SandTexture			: SAND_TEXTURE;
sampler SandTextureSampler = sampler_state { texture = <SandTexture> ; magfilter = LINEAR; minfilter = LINEAR; 
                                                                       mipfilter =LINEAR; AddressU  = Wrap;
                                                                       AddressV  = Wrap; AddressW  = Wrap;};

Texture RockTexture			: ROCK_TEXTURE;
sampler RockTextureSampler = sampler_state { texture = <RockTexture> ; magfilter = LINEAR; minfilter = LINEAR; 
                                                                       mipfilter = LINEAR; AddressU  = Wrap;
                                                                       AddressV  = Wrap; AddressW  = Wrap;};

Texture GrassNormal			:GRASS_NORMAL;
sampler2D GrassNormalSampler : TEXUNIT1 = sampler_state
{ Texture   = (GrassNormal); magfilter = LINEAR; minfilter = LINEAR; 
                             mipfilter = LINEAR; AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

Texture SandNormal			: SAND_NORMAL;
sampler2D SandNormalSampler : TEXUNIT1 = sampler_state
{ Texture   = (SandNormal); magfilter  = LINEAR; minfilter = LINEAR; 
                             mipfilter = LINEAR; AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

Texture RockNormal			: ROCK_NORMAL;
sampler2D RockNormalSampler : TEXUNIT1 = sampler_state
{ Texture   = (RockNormal); magfilter = LINEAR; minfilter = LINEAR; 
                             mipfilter=LINEAR; AddressU  = Wrap;
                             AddressV  = Wrap; AddressW  = Wrap;};

//------- Technique: MultiTexturedNormaled --------
 
 struct VS_INPUT
 {
     float4 Position	: POSITION0;    
     float3 Normal		: NORMAL0;
 };

struct VS_OUTPUT
{
    float4 Position     : POSITION;
    float4 TexCoord     : TEXCOORD0;
    float3 Normal		: TEXCOORD1;
    float3 WorldPos		: TEXCOORD2;
};

 VS_OUTPUT MultiTexturedNormaledVS( VS_INPUT input)    
 {
     VS_OUTPUT Output;
  
     Output.Position = mul(input.Position, WorldViewProj);
     Output.WorldPos = mul(input.Position, World);
     Output.Normal = mul(input.Normal, World);

     Output.TexCoord.x = input.Position.x * 0.0625f / TerrainScale;
     Output.TexCoord.y = input.Position.z * 0.0625f / TerrainScale;
     
     Output.TexCoord.z = input.Position.x / (TerrainWidth * TerrainScale);
     Output.TexCoord.w = input.Position.z / (TerrainWidth * TerrainScale);

     return Output;    
 }
 
 float4 MultiTexturedNormaledPS(VS_OUTPUT input) : COLOR0
 {
	 float3 TerrainColorWeight = tex2D(TextureMapSampler, input.TexCoord.zw);
	 
     float4 normalFromMap = (2.0f * tex2D(SandNormalSampler, input.TexCoord) - 1.0f)	* TerrainColorWeight.r;
     normalFromMap += (2.0f * tex2D(GrassNormalSampler, input.TexCoord) - 1.0f)			* TerrainColorWeight.g;
     normalFromMap += (2.0f * tex2D(RockNormalSampler, input.TexCoord) - 1.0f)			* TerrainColorWeight.b;
     
     // Factor in normal mapping and terrain vertex normals as well in lighting of the pixel
     float lightingFactor = saturate(dot(normalFromMap + input.Normal, -LightDirection));

     float4 Color = tex2D(SandTextureSampler, input.TexCoord)   * TerrainColorWeight.r;
     Color += tex2D(GrassTextureSampler, input.TexCoord)		* TerrainColorWeight.g;
     Color += tex2D(RockTextureSampler, input.TexCoord)			* TerrainColorWeight.b;

     float3 Reflect = (lightingFactor * input.Normal) + LightDirection;
     float3 specular = pow(saturate(dot(Reflect, -CameraForward)), SpecularPower);
     
     float d = length(input.WorldPos - CameraPos);  
     float l = saturate((d - fogNear) / (fogFar - fogNear) / clamp(input.WorldPos.y / fogAltitudeScale + 1, 1, fogThinning));
     
	 //Color.rgb *= (AmbientColor + (DiffuseColor * lightingFactor) + (SpecularColor * specular * lightingFactor)) * AmbientPower;
	 Color.a = 1.0f;
	 
	 return lerp(Color, fogColor, l);
 }
 
 technique MultiTexturedNormaled
 {
     pass Pass0
     {
         VertexShader = compile vs_1_1 MultiTexturedNormaledVS();
         PixelShader = compile ps_2_0 MultiTexturedNormaledPS();
     }
 }
 
 // ================================================
 //------- Technique: MultiTextured --------

 VS_OUTPUT MultiTexturedVS( VS_INPUT input)    
 {
     VS_OUTPUT Output;
  
     Output.Position = mul(input.Position, WorldViewProj);
     Output.WorldPos = mul(input.Position, World);
     Output.Normal = mul(input.Normal, World);

     Output.TexCoord.x = input.Position.x * 0.0625f / TerrainScale;
     Output.TexCoord.y = input.Position.z * 0.0625f / TerrainScale;
     
     Output.TexCoord.z = input.Position.x / (TerrainWidth * TerrainScale);
     Output.TexCoord.w = input.Position.z / (TerrainWidth * TerrainScale);

     return Output;    
 }
 
 float4 MultiTexturedPS(VS_OUTPUT input) : COLOR0
 {
	 float3 TerrainColorWeight = tex2D(TextureMapSampler, input.TexCoord.zw);
	 
	 input.Normal = normalize(input.Normal);

     // Factor in normal mapping and terrain vertex normals as well in lighting of the pixel
     float lightingFactor = saturate(dot(input.Normal, -LightDirection));

     // Multi-texture blending occurs in these three lines
     float4 Color = tex2D(SandTextureSampler, input.TexCoord)   * TerrainColorWeight.r;
     Color += tex2D(GrassTextureSampler, input.TexCoord) * TerrainColorWeight.g;
     Color += tex2D(RockTextureSampler,  input.TexCoord) * TerrainColorWeight.b;

     float3 Reflect = (lightingFactor * input.Normal) + LightDirection;
     float3 specular = pow(saturate(dot(Reflect, -CameraForward)), SpecularPower);
     
	 //Color.rgb *= (AmbientColor + (DiffuseColor * lightingFactor) + (SpecularColor * specular * lightingFactor)) * AmbientPower;
	 Color.a = 1.0f;
 
     return Color;
 }
 
 technique MultiTextured
 {
     pass Pass0
     {
         VertexShader = compile vs_1_1 MultiTexturedVS();
         PixelShader = compile ps_2_0 MultiTexturedPS();
     }
 }
 
 // ================================================
 //------- Technique: Wireframed --------
 
 VS_OUTPUT WireframedVS( VS_INPUT input)    
 {
    VS_OUTPUT Output;
  
     Output.Position = mul(input.Position, WorldViewProj);
     Output.WorldPos = mul(input.Position, World);
     Output.Normal = mul(input.Normal, World);

     Output.TexCoord.x = input.Position.x * 0.0625f / TerrainScale;
     Output.TexCoord.y = input.Position.z * 0.0625f / TerrainScale;
     
     Output.TexCoord.z = input.Position.x / (TerrainWidth * TerrainScale);
     Output.TexCoord.w = input.Position.z / (TerrainWidth * TerrainScale);

     return Output;
 }
 
 float4 WireframedPS(VS_OUTPUT input) : COLOR0
 {
	return float4(1, 1, 1, 1);
 }
 
 technique Wireframed
 {
     pass Pass0
     {
         CullMode = None;
         FillMode = Wireframe;
        
         VertexShader = compile vs_1_1 WireframedVS();
         PixelShader = compile ps_2_0 WireframedPS();
     }
 }
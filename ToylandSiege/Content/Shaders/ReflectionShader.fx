float4x4 World;
float4x4 View;
float4x4 Projection;
Texture2D Texture;

sampler2D SampleType = sampler_state
{
  Texture = <Texture>;
  MinFilter = LINEAR;
  MagFilter = LINEAR;
  MipFilter = LINEAR;
  AddressU = CLAMP;
  AddressV = CLAMP;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
  float2 UV     : TEXCOORD0;
  float3 Normal   : NORMAL;
};

struct VertexShaderOutput
{
    float4 Position           : POSITION0;
  float2 UV               : TEXCOORD0;
  float3 Normal         : TEXCOORD1;
  float4 RefractionPosition : TEXCOORD3;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
  matrix viewProjectWorld;

  // Change the position vector to be 4 units for proper matrix calculations.
  input.Position.w = 1.0f;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

  float4x4 preViewProjection = mul(View, Projection);
  float4x4 preWorldViewProjection = mul(World, preViewProjection);

  // Create the view projection world matrix for refraction.
  viewProjectWorld = mul(View, Projection);
  viewProjectWorld = mul(World, viewProjectWorld);

  // Calculate the input position against the viewProjectWorld matrix.
  float4x4 worldViewProj = mul(World, View);
  worldViewProj = mul(worldViewProj, Projection);
  output.RefractionPosition = mul(input.Position, worldViewProj);

  output.Normal = input.Normal;

  output.UV = input.UV;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
  float2 refractTexCoord;
  float4 refractionColor;
  float4 projTexC;


  //transform the projective texcoords to NDC space
  //and scale and offset xy to correctly sample a DX texture
  projTexC = input.RefractionPosition;
  projTexC.xyz /= projTexC.w;
  projTexC.x = 0.5f*projTexC.x + 0.5f;
  projTexC.y = -0.5f*projTexC.y + 0.5f;
  projTexC.z = .1f / projTexC.z; //refract more based on distance from the camera
  
  // Calculate the projected refraction texture coordinates.
  refractTexCoord.x = (input.RefractionPosition.x / input.RefractionPosition.w) / 2.0f + 0.5f;
  refractTexCoord.y = (-input.RefractionPosition.y / input.RefractionPosition.w) / 2.0f + 0.5f;

  float4 refr = tex2D(SampleType, projTexC.xy - projTexC.z /* * normalT.xz*/);

  //return refr;
  return tex2D(SampleType, refractTexCoord);
  //return float4(1, 0, 0, 0);
  //return float4(refractTexCoord.x, refractTexCoord.y, 0, 0);
}

technique ClassicTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
float4x4 World;
float4x4 View;
float4x4 Projection;
int Shape;

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color;
	if (Shape == 0) // Circle
	{
		color = float4(0, 1, 0, 0.3F);
	}
	else if (Shape == 2) // Triangle
	{
		color = float4(0, 0, 1, 0.3F);
	}
	else // Square
	{
		color = float4(1, 0, 0, 0.3F);
	}
    
    return color;
}

technique Cursor
{
    pass Pass1
    {
        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

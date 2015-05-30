// Effect dynamically changes color saturation.

sampler TextureSampler : register(s0);

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the texture color.
    float4 tex = tex2D(TextureSampler, texCoord);
    
    // Convert it to greyscale. The constants 0.3, 0.59, and 0.11 are because
    // the human eye is more sensitive to green light, and less to blue.
    tex.rgb = dot(tex.rgb, float3(0.3, 0.59, 0.11));

    return tex;
}

technique Desaturate
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}
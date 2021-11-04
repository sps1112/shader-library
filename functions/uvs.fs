float yScale = 1.0f;

vec2 get_uv(vec2 fragCoord)
{
    vec2 uv = fragCoord / iResolution.xy;
    uv = uv * 2.0 - 1.0;
    uv *= yScale;
    uv.x *= (iResolution.x / iResolution.y);
    return uv;
}

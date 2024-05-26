// UV_SETTINGS
//------------------------------
float scale=1.0f;

float cycle_through(float n)
{
    float m=n+1.0f;
    m=floor(m/2.0f)*2.0f;
    return abs(m-n);
}

vec2 get_seamless(vec2 uv)
{
    uv = uv * 2.0f-1.0f;
    uv=abs(uv);
    uv *= scale;
    uv = vec2(cycle_through(uv.x),cycle_through(uv.y));
    return uv;
}
//------------------------------
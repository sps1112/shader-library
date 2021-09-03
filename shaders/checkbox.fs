// Checkbox Settings
int boxCount = 5;
vec2 startOffset = vec2(0.0f, 0.0f);

// Color Settings
vec3 colorA = vec3(0.15f, 0.60f, 0.80f);
vec3 colorB = vec3(0.85f, 0.85f, 0.85f);

// Cycles the value of a in the cycle [minVal, maxVal]
float cycle_float(float a, float minVal, float maxVal)
{
    float diff = maxVal - minVal;
    if(a > maxVal)
    {
        float aDiff = a - maxVal;
        float div = float(int(aDiff / diff));
        a = minVal + (aDiff - (div * diff));
    }
    else if(a < minVal)
    {
        float aDiff = minVal - a;
        float div = float(int(aDiff / diff));
        a = maxVal - (aDiff - (div * diff));
    }
    return a;
}

// Cycles through the point so that it lies between (2.0f to 4.0f)
vec2 cycle_through(vec2 pos)
{
    pos.x = cycle_float(pos.x, 0.0f, 2.0f);
    pos.y = cycle_float(pos.y, 0.0f, 2.0f);
    return pos;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{ 
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = fragCoord / iResolution.xy;
    vec2 pos = uv * float(boxCount);
    pos.x *= (iResolution.x / iResolution.y);
    
    // Get Final Coord
    vec2 timeOffset = vec2(0.0f);
    timeOffset.x = 5.5f * sin(iTime * 2.0f);
    timeOffset.y = -3.5f * iTime;
    vec2 final=cycle_through(pos + startOffset + timeOffset);
    int X = int(final.x);
    int Y = int(final.y);
    
    // Setup Color
    vec3 col;

    // Check the Pos
    // Both even or both odd
    if((X % 2) == (Y % 2))
    {
        col = colorA;
    }
    else
    {
        col = colorB;
    }
    
    // Outpur Color
    fragColor = vec4(col, 1.0);
}

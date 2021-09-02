// Checkbox Settings
int boxCount = 5;
vec2 startOffset = vec2(0.0f, 0.0f);

// Color Settings
vec3 colorA = vec3(0.1f, 0.1f, 0.6f);
vec3 colorB = vec3(0.5f, 0.5f, 0.5f);

// Cycles through the point so that it lies between (2.0f to 4.0f)
vec2 cycle_through(vec2 pos)
{
    float xFac = float(int(pos.x / 2.0f));
    pos.x = pos.x + (2.0f * xFac) + 2.0f;
    float yFac = float(int(pos.y / 2.0f));
    pos.y = pos.y + (2.0f * yFac) + 2.0f;
    return pos;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{ 
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = fragCoord / iResolution.xy;
    vec2 pos = uv * float(boxCount);
    pos.x *= (iResolution.x / iResolution.y);
    
    // Get Final Coord
    vec2 final=cycle_through(pos+startOffset);
    int X=int(final.x);
    int Y=int(final.y);
    
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

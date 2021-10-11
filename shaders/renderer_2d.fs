#define TO_INTERPOLATE 1
#define RENDER_TRIANGLE 0

// Image Settings
float yScaling = 1.0f;

// Scene Settings
vec3 bgColor = vec3(0.05f);
vec3 objectColor = vec3(0.85f,0.2f,0.25f);

// Vertex Data
#if RENDER_TRIANGLE
// 1. Triangle
// a. All the vertices of 2D Object
vec3[3] vertices = vec3[3](
vec3(0.0f, 0.5f, 0.0f),
vec3(0.5f, -0.5f, 0.0f),
vec3(-0.5f, -0.5f, 0.0f)
);
// b. The Index order for each triangle of the Object
int[3] indices = int[3](
0, 1 , 2
);
#else
// 2. Rectangle
// a. All the vertices of 2D Object
vec3[4] vertices = vec3[4](
vec3(0.5f, 0.5f, 0.0f),
vec3(0.5f, -0.5f, 0.0f),
vec3(-0.5f, -0.5f, 0.0f),
vec3(-0.5f, 0.5f, 0.0f)
);
// b. The Index order for each triangle of the Object
int[6] indices=int[6](
0, 1, 2,
2, 3, 0
);
#endif
// The Color of each vertex of Object
vec3[4] basicColors = vec3[4](
vec3(0.25f, 0.1f, 0.95f),
vec3(0.5f, 0.1f, 0.95f),
vec3(0.25f, 0.8f, 0.95f),
vec3(0.25f, 0.1f, 0.15f)
/*vec3(1.0f),
vec3(1.0f, 0.0f, 0.0f),
vec3(0.0f, 1.0f, 0.0f),
vec3(0.0f, 0.0f, 1.0f)*/
);

// Checks if target lies of the same side of (lineA <-> lineB) as lineC 
bool valid_point(vec2 lineA, vec2 lineB, vec2 lineC, vec2 target)
{
    bool status = false;
    float m = (lineB.y - lineA.y) / (lineB.x - lineA.x);
    if(lineB.x == lineA.x)
    {
        m = 9999.9f;
    }
    float c = lineB.y - (m * lineB.x);
    float s1 = lineC.y- (m * lineC.x) - c;
    float s2 = target.y- (m * target.x) - c;
    if((s1 * s2) >= 0.0f)
    {
        status = true;
    }
    return status;
}

// Gets the Point Closest to X
int get_closest_point(vec2 A, vec2 B, vec2 C, vec2 X)
{
    int index = -1;
    float dA = distance(X, A);
    float dB = distance(X, B);
    float dC = distance(X, C);
    if(dA <= dB && dA <= dC)
    {
        index = 0;
    }
    else if(dB <= dA && dB <= dC)
    {
        index = 1;
    }
    else if(dC <= dB && dC <= dA)
    {
        index = 2;
    }
    return index;
}

// Returns the Minimum of A, B and C
float get_min(float A, float B, float C)
{
    return min(min(A, B), C);
}


// Returns the Maximum of A, B and C
float get_max(float A, float B, float C)
{
    return max(max(A, B), C);
}

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

// Returns the Interpolated color for the given f value
vec3 get_time_color(float f, float minV, float maxV)
{
    return mix(basicColors[int(cycle_float(f, minV, maxV))],
        basicColors[int(cycle_float(f + 1.0f, minV, maxV))],
        f - float(int(f)));
}


// Gets the Color for the Pixel based on which triangle it is inside
vec3 get_pixel_color(vec2 pos)
{
    // Set Default Colors
    float offset = 2.0f * iTime;
    if(offset >= 4.0f)
    {
        offset -= 4.0f;
    }
    #if RENDER_TRIANGLE
    float size = 3.0f;
    #else
    float size = 4.0f;
    #endif
    vec3[4] colors = vec3[4](
      get_time_color(offset, 0.0f, size),
      get_time_color(offset + 1.0f, 0.0f, size),
      get_time_color(offset + 2.0f, 0.0f, size),
      get_time_color(offset + 3.0f, 0.0f, size)
    );
    vec3 col = bgColor;
    for(int i = 0; (i+2) < indices.length(); i = (i + 3))
    {
        vec2 vertexA = vec2(vertices[indices[i]]);
        vec2 vertexB = vec2(vertices[indices[i + 1]]);
        vec2 vertexC = vec2(vertices[indices[i + 2]]);
        bool validPoint = false;
        validPoint = valid_point(vertexA, vertexB, vertexC, pos);
        validPoint = (validPoint) ? (valid_point(vertexB, vertexC, vertexA, pos)) : validPoint;
        validPoint = (validPoint) ? (valid_point(vertexC, vertexA, vertexB, pos)) : validPoint;
        if(validPoint)
        {
#if !TO_INTERPOLATE
            int index = get_closest_point(vertexA, vertexB, vertexC, pos);
            col = colors[indices[i + index]];
#else
             float dA = distance(pos, vertexA);
             float dB = distance(pos, vertexB);
             float dC = distance(pos, vertexC);
             float minVal = get_min(dA, dB, dC);
             float maxVal = get_max(dA, dB, dC);
             dA = 1.0f - smoothstep(minVal, maxVal, dA);
             dB = 1.0f - smoothstep(minVal, maxVal, dB);
             dC = 1.0f - smoothstep(minVal, maxVal, dC);
             col = ((colors[indices[i]] * dA) + 
                   (colors[indices[i+1]] * dB) +
                   (colors[indices[i+2]] * dC));
#endif
        }
    }
    return col;
}

// Gets the Pixel in Coordinate Space
vec2 get_pos(vec2 fragCoord)
{
    // From (0, 0) to (1, 1)
    vec2 pos = fragCoord / iResolution.xy;
    // From (-1, -1) to (1, 1)
    pos = (pos * 2.0f) - 1.0f;
    // From (-Y, -Y) to (Y, Y)
    pos *= yScaling;
    // From (-X, -Y) to (X, Y)
    pos.x *= (iResolution.x / iResolution.y);
    return pos;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    // Get Pixel Position
    vec2 pos = get_pos(fragCoord);
    
    // Pixel color
    vec3 col = get_pixel_color(pos);

    // Output to screen
    fragColor = vec4(col,1.0);
}

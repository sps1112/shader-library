#define AREA_GRAPH 0

// Graph Color Properties
vec3 bgColor = vec3(0.05f);
vec3 gridColor = vec3(0.25f, 0.75f, 0.25f);
vec3 axesColor = vec3(0.9f, 0.9f, 0.1f);
vec3 eqColor = vec3(0.15f, 0.65f, 0.95f);

// Graph Render Properties
float yLimit = 6.0f;
float gridWidth = 0.2f;
int pixelWidth = 2;

// Equation Properties
float errorMargin = 0.25f;

// Gets f(x, y)
float get_result(vec2 point)
{
    //float radius = 25.0f * ((sin(iTime * 1.5f) + 1.0f) / 2.0f) + 0.05f;
    // f(x,y) = y^2 + x^2 - R
    //float res = ((point.y * point.y) + (point.x * point.x) - radius);
    float res = point.y - sin(abs(point.x)) * sqrt(abs(point.x)) * cos(iTime * 3.0f) * 2.0f;
    return res;
}

// Checks if the point satifies the result equation
bool check_equation(vec2 point)
{
    bool status = false;
    float result = get_result(point);
    
    // Points that lie on the Curve
#if !AREA_GRAPH
    if(abs(result) < errorMargin)
    {
        status = true;
    }
#else
    // All points inside the Curve
    if(result < errorMargin)
    {
        status = true;
    }
#endif
    return status;
}

// Gets the integer part of A ==> (1.2 => 1) and (-1.2 => -1)
float get_int(float a)
{
    if(a < 0.0f)
    {
        return float(-1 * int(-1.0f * a));
    }
    return float(int(a));
}

// Gets the fractional part of A ==> (1.2 => 0.2) and (-1.3 => 0.3)
float get_frac(float a)
{
    return abs(a - get_int(a));
}

// Returns the pixel in screen space
float get_pixel(float a, float limit, float res)
{
    // A is from (-(L+1), (L+1))
    // From (-1, 1)
    a = a / (limit + 1.0f);
    // From (0 , 1)
    a = (a + 1.0f) / 2.0f;
    // From (0 , res)
    a *= res;
    return a;
}

// Checks if the point lies on the Axes lines
bool check_point(float pos, float limit, float resolution)
{
    bool status = false;
    float width = gridWidth;
    float fracPart = get_frac(pos);
    float intPart = get_int(pos);
    if(intPart == 0.0f)
    {
        width /= 2.0f;
        if(width < 0.02f)
        {
            width = 0.02f;
        }
    }
    if(abs(fracPart) < width)
    {
       float pixelVal = get_pixel(pos, limit, resolution);
       float correctVal = get_int(get_pixel(intPart, limit, resolution));
       float dist = abs(pixelVal - correctVal);
       float testDist = float(pixelWidth);
       if(intPart == 0.0f)
       {
           // testDist /= 2.0f;
           testDist = get_int(testDist);
       }
       if(dist <= testDist)
       {
           status = true;
       }
    }
    return status;
}


void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    // 1. Setup Data
    
    // Gets the Y limit for square grid
    float xLimit = (yLimit + 1.0f) * (iResolution.x / iResolution.y) - 1.0f;
    
    // From (0 to 1)
    vec2 uv = fragCoord / iResolution.xy;
    
    // From (-1 to 1)
    vec2 pos = (uv * 2.0f) - 1.0f;
    
    // From (-X to X, -Y to Y)
    pos.x *= (xLimit + 1.0f);
    pos.y *= (yLimit + 1.0f);
    
    // 2. Setup Color
    
    // a. Default Color is the Background
    vec3 col = bgColor;
    
    // b. If the point lies on the grid lines, then change color
    if(check_point(pos.x, xLimit , iResolution.x) || check_point(pos.y, yLimit, iResolution.y))
    {
        col = gridColor;
        if((check_point(pos.x, xLimit , iResolution.x) && get_int(pos.x) == 0.0f) || 
            (check_point(pos.y, yLimit , iResolution.y) && get_int(pos.y) == 0.0f))
        {
            col = axesColor;
        }
    }
    
    // c. If the point satisfies the Equation, then change color
    if(check_equation(pos))
    {
        col = eqColor;
    }

    // 3. Output to screen
    fragColor = vec4(col,1.0);
}

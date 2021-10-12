//----------------------------------------------------
// SCENE_SETTINGS
//----------------------------------------------------
vec3 bgColor = vec3(0.4f,0.8f,0.9f);
float yScale = 1.0f;
vec3 wUp = vec3(0.0f, 1.0f, 0.0f);
//----------------------------------------------------
//----------------------------------------------------


//----------------------------------------------------
// CAMERA_SETTINGS
//----------------------------------------------------
vec3 cPos = vec3(0.0f, 0.0f, -5.0f);
vec3 cLookAt = vec3(0.0f,0.0f,0.0f);
float fov = 45.0f;

vec3 get_target(vec2 uv, vec3 origin, vec3 lookAt)
{
    vec3 view = normalize(lookAt - origin);
    vec3 right = normalize(cross(view , wUp));
    vec3 up = normalize(cross(right , view));
    right *= tan(radians(fov));
    up *= tan(radians(fov));
    vec3 target = ((origin + view) + (up * uv.y) + (right * uv.x));
    return target;
}
//----------------------------------------------------
//----------------------------------------------------


//----------------------------------------------------
// SCENE_DATA
//----------------------------------------------------
vec4 sphere = vec4(0.0f,0.0f,5.0f,4.0f);
float plane = -4.0f;
vec3 lPos = vec3(10.0f,8.0f,10.0f);
vec3 lightCol = vec3(1.0f,1.0f,0.5f);
//----------------------------------------------------
//----------------------------------------------------


//----------------------------------------------------
// RAYMARCHING
//----------------------------------------------------
#define MAX_STEPS 2000
#define MAX_DISTANCE 2000.0f
#define SURFACE_DISTANCE 0.001f

float get_dist(vec3 point)
{
    vec3 sp=sphere.xyz;
    sp.x += sin(iTime*2.5f)*2.5f;
    sp.z += cos(iTime*2.5f)*2.5f;
    float dSphere = distance(point, sp) - sphere.w;
    float dPlane = point.y - plane;
    return min(dSphere, dPlane);
}

float ray_march(vec3 rO, vec3 rD)
{
    float dist = 0.0f;
    for(int i = 0; i < MAX_STEPS; i++)
    {
        vec3 current = rO + (dist * rD);
        float delta = get_dist(current);
        dist += delta;
        if(dist <= SURFACE_DISTANCE || dist >= MAX_DISTANCE)
        {
            break;
        }
    }
    return dist;
}

vec3 get_normal(vec3 p)
{
    float dist=get_dist(p);
    vec2 e=vec2(0.01f,0.0f);
    vec3 n = dist- vec3(get_dist(p-e.xyy),
                        get_dist(p-e.yxy),
                        get_dist(p-e.yyx));
    return normalize(n);
}

float get_diffuse(vec3 p)
{
    vec3 lp=lPos;
    lp.xz =vec2(sin(iTime*5.0f),cos(iTime*5.0f))*7.0f;
    vec3 l = normalize(lp-p);
    vec3 n = get_normal(p);
    float dif=clamp(dot(n,l),0.0f,1.0f);
    // Shadows
    float d = ray_march(p + n*1.5f,lp);
    if(d<length(lp-p))
    {
        dif*=0.1f;
    }
    return dif;
}

vec3 trace_ray(vec2 uv)
{
    vec3 col = vec3(0.0f);
    vec3 rOrigin = cPos;
    vec3 offset=vec3(1.0f,0.0f,0.0f)*sin(iTime)*50.0f;
    //rOrigin+=offset;
    vec3 target = get_target(uv, rOrigin, cLookAt);
    vec3 rDir = normalize(target - rOrigin);
    float rayMarch = ray_march(rOrigin, rDir);
    if(rayMarch >= MAX_DISTANCE)
    {
        col= bgColor;
    }
    else
    {
        vec3 end = rOrigin + rayMarch*rDir;
        float diff=get_diffuse(end);
        col=lightCol*diff;
    }
    return col;
}
//----------------------------------------------------
//----------------------------------------------------


//----------------------------------------------------
// UTILITY
//----------------------------------------------------

// Gets the UV coordinate for Image
vec2 get_uv(vec2 fragCoord)
{
    vec2 uv = fragCoord / iResolution.xy;
    uv = (uv * 2.0f) - 1.0f;
    uv *= yScale;
    uv.x *= (iResolution.x / iResolution.y);
    return uv;
    // return (yScale * 2.0f * (fragCoord - 0.5f * (iResolution.xy))) / iResolution.y;
}

//----------------------------------------------------
//----------------------------------------------------


// Main Image Function
void mainImage(out vec4 fragColor,in vec2 fragCoord)
{
    // From (-X,-Y) to (X,Y)
    vec2 uv = get_uv(fragCoord);
    
    // Pixel Color
    vec3 col = trace_ray(uv);
    
    // Output to screen
    fragColor = vec4(col, 1.0f);
}

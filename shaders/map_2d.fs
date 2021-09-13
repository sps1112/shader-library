//----------------------------------------------------
// TETXURE_SETTINGS
//----------------------------------------------------
#define TEXTURE_ROWS 100.0f
#define GAMMA_CORRECTION 0

// Gets the UV coordinate for Image
vec2 get_uv(vec2 fragCoord)
{
    vec2 uv = fragCoord / iResolution.xy;
    uv *= TEXTURE_ROWS;
    // uv = (uv * 2.0f) - 1.0f;
    uv.x *= (iResolution.x / iResolution.y);
    return uv;
}

//----------------------------------------------------
//----------------------------------------------------


//----------------------------------------------------
// PERLIN_NOISE_GENERATOR
//----------------------------------------------------

float fade(float t)
{
    return t * t * t * (t * ((t * 6.0f) - 15.0f) + 10.0f);
}

float grad(int hash, float x, float y, float z)
{
    int h = hash & 15;
    // Convert lower 4 bits of hash into 12 gradient directions
    float u = (h < 8) ? x : y,
           v = ((h < 4) ? y : (h == 12 || h == 14 ? x : z));
    return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
}

int[512] p=int[512](151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142,
             8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117,
             35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71,
             134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41,
             55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
             18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226,
             250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182,
             189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167,
             43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246,
             97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239,
             107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
             138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
             151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142,
             8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117,
             35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71,
             134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41,
             55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
             18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226,
             250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182,
             189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167,
             43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246,
             97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239,
             107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
             138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180);
             
float noise(float x, float y, float z)
{  
    // Find the unit cube that contains the point
    int X = int(floor(x)) & 255;
    int Y = int(floor(y)) & 255;
    int Z = int(floor(z)) & 255;

    // Find relative x, y,z of point in cube
    x -= floor(x);
    y -= floor(y);
    z -= floor(z);
    
    // Compute fade curves for each of x, y, z
    float u = fade(x);
    float v = fade(y);
    float w = fade(z);

    // Hash coordinates of the 8 cube corners
    int A = p[X] + Y;
    int AA = p[A] + Z;
    int AB = p[A + 1] + Z;
    int B = p[X + 1] + Y;
    int BA = p[B] + Z;
    int BB = p[B + 1] + Z;

    // Add blended results from 8 corners of cube
    float res = mix(
        mix(
            mix(grad(p[AA], x, y, z),
                 grad(p[BA], x - 1.0f, y, z),
                 u),
            mix(grad(p[AB], x, y - 1.0f, z),
                 grad(p[BB], x - 1.0f, y - 1.0f, z),
                 u),
            v),
        mix(
            mix(grad(p[AA + 1], x, y, z - 1.0f),
                 grad(p[BA + 1], x - 1.0f, y, z - 1.0f),
                 u),
            mix(grad(p[AB + 1], x, y - 1.0f, z - 1.0f),
                 grad(p[BB + 1], x - 1.0f, y - 1.0f, z - 1.0f),
                 u),
            v),
        w);
    return (res + 1.0f) / 2.0f;
}

//----------------------------------------------------
//----------------------------------------------------


//----------------------------------------------------
// NOISE_GENERATOR
//----------------------------------------------------
#define NOISE_FUNCTION get_octave_noise
#define NOISE_SCALE 50
#define NUMBER_OCTAVES 4
#define PERSISTANCE 0.5f
#define LACUNARITY 2.0f
#define SCALE_FROM_CENTER 1

// Generates a random noise between 0.0 and 1.0
float rand(vec2 pos)
{
    return fract(sin(dot(pos, vec2(12.9898f, 78.233f))) * 43758.5453f);
}

// Gets the Random Noise Height
float get_noise(vec2 pos)
{
    pos *= float(TEXTURE_ROWS);
    pos = vec2(floor(pos.x),floor(pos.y));
    return rand(pos);
}

float perlin(vec2 pos)
{
    return noise(pos.x,pos.y,0.0f);
}

float get_perlin_noise(vec2 pos)
{
     float scale = float(NOISE_SCALE);
     if(scale<=0.0f)
     {
         scale+=0.001f;
     }
     pos *= float(TEXTURE_ROWS);
     //pos = vec2(floor(pos.x),floor(pos.y));
     return perlin(pos/scale);
}

float get_octave_noise(vec2 pos)
{
    float rows = float(TEXTURE_ROWS);
    float columns = rows*(iResolution.x/iResolution.y);
    float scale = float(NOISE_SCALE);
    if(scale<=0.0f)
    {
        scale+=0.001f;
    }
    
    int octaves=int(NUMBER_OCTAVES);
    float lacunarity = max(LACUNARITY, 1.0f);
    float persistence = min(PERSISTANCE, 1.0f);
    
    float halfX = 0.0f;
    float halfY = 0.0f;
#if SCALE_FROM_CENTER
    halfX = columns / 2.0f;
    halfY = rows / 2.0f;
#endif

    float[20] offsets=float[20](
    0.0f, 0.0f,0.0f, 0.0f,0.0f,
    0.0f, 0.0f,0.0f, 0.0f,0.0f,
    0.0f, 0.0f,0.0f, 0.0f,0.0f,
    0.0f, 0.0f,0.0f, 0.0f,0.0f
    );
    for(int i=0;i<octaves*2;i++)
    {
        offsets[i] = rand(vec2(pos.x,pos.y))*0.00f;
    }
    
    float amplitude = 1.0f;
    float frequency = 1.0f;
    float noiseVal = 0.0f;
    pos *= rows;
    // pos = vec2(floor(pos.x),floor(pos.y));
    
    vec2 offset =1.05f*vec2(iTime*1.25f,iTime*1.25f);
    scale+=sin(iTime*0.85f)*5.25f;
    
    for (int i = 0; i < octaves; i++)
    {
        float sampleX =(((pos.x-halfX) / scale) * frequency)+offset.x+offsets[i*2];
        float sampleY =(((pos.y-halfY) / scale) * frequency)+offset.y+offsets[(i*2)+1];
        float noise = (get_perlin_noise(vec2(sampleX,sampleY)) * 2.0f) - 1.0f;
        noiseVal += noise * amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }   
    noiseVal=smoothstep(-0.95f,1.1f,noiseVal);
    return noiseVal;
}
//----------------------------------------------------
//----------------------------------------------------


//----------------------------------------------------
// MAP_SETTINGS
//----------------------------------------------------
vec3[8] regions=vec3[8](
vec3(0.1f, 0.1f, 0.85f), // DARK BLUE
vec3(0.2f, 0.4f, 1.0f), // OCEAN BLUE
vec3(0.9f, 0.9f, 0.2f), // SAND YELLOW
vec3(0.2f, 0.9f, 0.2f), // LIGHT GREEN
vec3(0.1f, 0.65f, 0.1f), // DARK GREEN
vec3(0.5f, 0.25f, 0.0f), // LIGHT BROWN
vec3(0.3f, 0.075f, 0.0f), // DARK BROWN
vec3(0.85f, 1.0f, 1.0f) // SNOW BLUE
);

float[8] heights=float[8](
0.0f, // DEEP OCEAN
0.0325f, // OCEAN
0.35f, // COAST
0.375f, // PLAINS
0.572f, // FORESTS
0.675f, // HILLS
0.825f, // MOUNTAINS
0.95f // SNOW
);

int get_region_index(float h)
{
    int index=-1;
    for(int i=0;i<8;i++)
    {
        if(h>=heights[i])
        {
            index++;
        }
        else
        {
            break;
        }
    }
    return index;
}
//----------------------------------------------------
//----------------------------------------------------


void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv = fragCoord/iResolution.y;
    
    float height = NOISE_FUNCTION(uv);
    vec3 col = regions[get_region_index(height)];
#if GAMMA_CORRECTION
    col = pow(col, vec3(1.0f/2.2f));
#endif
    // Output to screen
    fragColor = vec4(col,1.0);
}

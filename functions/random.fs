//----------------------------------------------------
// Generates a random noise between 0.0 and 1.0
float rand(vec2 pos)
{   
    return fract(sin(dot(pos, vec2(12.9898f, 78.233f))) * 43758.5453f);
}
//----------------------------------------------------

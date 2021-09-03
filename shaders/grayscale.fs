float timePeriod = 2.0f;

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = fragCoord / iResolution.xy;

    vec3 texCol = vec3(texture(iChannel0, uv.xy));
    // float av = (texCol.x + texCol.y + texCol.z) / 3.0f;
    float av = (0.299f * texCol.x) + (0.587f * texCol.y) + (0.114f * texCol.z);
    vec3 greyCol = vec3(av);
    float lerpVal = (sin(iTime * timePeriod) + 1.0f) / 2.0f;
    vec3 col = mix(texCol, greyCol, lerpVal);

    // Output to screen
    fragColor = vec4(col, 1.0);
}

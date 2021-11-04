// TEXTURE SETTINGS
//------------------------------
vec3 brickColA=vec3(0.3f,0.3f,0.5f);
vec3 brickColB=vec3(0.1f,0.1f,0.1f);
vec3 borderCol=vec3(0.2f,0.1f,0.0f);

float scale=1.5f;
float rows=6.0f;
vec2 widthRange=vec2(0.05f,0.4f);
float borderWidth=0.005f;

// Generates a random noise between 0.0 and 1.0
float rand(vec2 pos)
{   
    return fract(sin(dot(pos, vec2(12.9898f, 78.233f))) * 43758.5453f);
}

vec3 get_texture_col(vec2 p)
{
    vec3 col=brickColA;
    
    float rowWidth=1.0f/rows;
    float rowNum=floor(p.y/rowWidth);
    
    float lowerLimit=rowNum*rowWidth;
    float upperLimit=(rowNum+1.0f)*rowWidth;
    
    float uDiff=abs(upperLimit-p.y);
    float lDiff=abs(lowerLimit-p.y);
    
    if(uDiff<borderWidth || lDiff<borderWidth)
    {
        col=borderCol;
    } 
    else
    {
        float fac=1.0f;
        float rN=rowNum+1.0f;
        
        float rI=rand(vec2(rN*fac,rN*2.0f*fac));
        float widthI=mix(widthRange.x,widthRange.y,rI);
        
        float cR=rand(vec2(widthI*6.0f,widthI*9.0f));
        vec3 startCol=mix(brickColA,brickColB,cR);
        
        float diff=p.x-widthI;
        float brickDist=0.0f;
        if(diff>0.0f)
        {
            while(diff>0.0f)
            {
                if(diff<borderWidth)
                {
                    col=borderCol;
                    break;
                }
                brickDist+=widthI;
                fac*=2.0f;
                rI=rand(vec2(rN*fac,rN*2.0f*fac));
                widthI=mix(widthRange.x,widthRange.y,rI);
                diff-=widthI;
            }
            if(diff<0.0f)
            {
                if(brickDist+widthI>1.0f*scale)
                {
                    col=startCol;
                }
                else
                {
                    float cR=rand(vec2(widthI*6.0f,widthI*9.0f));
                    col=mix(brickColA,brickColB,cR);
                }
            }
        }
        else
        {
           col=startCol;
        }
      
    }
    return col;
    return vec3(texture(iChannel0,p));
}
//------------------------------

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv=fragCoord/iResolution.xy;
    uv*=scale;

    vec3 col = get_texture_col(uv);
    
    // Output to screen
    fragColor = vec4(col,1.0);
}

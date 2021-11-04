//----------------------------------------------------
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
//----------------------------------------------------

// CartesianToToroidal.hlsl

// Cartesian to toroidal coordinates conversion
void CartesianToToroidal_float(float Longitude, float Latitude,float R, float r, out float3 Out)
{
    // Cartesian to toroidal coordinates conversion.
    // Where R and r are Major and minor radius. and A, B are the angles varies from 0 to 2*pi.
    // x= (R-r*CosB)CosA
    // y= (R-r*CosB)SinA
    // z= r*SinB

    // #if SHADERGRAPH_PREVIEW
    //     Longitude = 0.0;
    //     Latitude = 0.0;
    //     Out = float3(0.0,0.0,0.0);
    // #else  
    float A = Longitude * TWO_PI;
    float B = Latitude * TWO_PI;
            
    float cosA = cos(A);
    float sinA = sin(A);
    float cosB = cos(B);
    float sinB = sin(B);

    float x = (R-r*cosB)*cosA;
    float y = (R-r*cosB)*sinA;
    float z = r*sinB;

    float3 toroid = float3(x,y,z);
    Out = toroid;
    // #endif
}

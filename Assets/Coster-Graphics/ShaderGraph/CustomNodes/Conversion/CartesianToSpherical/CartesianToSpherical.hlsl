// Cartesian to spherical coordinates conversion.
void CartesianToSpherical_float(float Longitude, float Latitude, out float3 Out)
{
    #if defined(SHADERGRAPH_PREVIEW)
        Longitude = 0.0;
        Latitude = 0.0;
        Out = float3(0.0,0.0,0.0);
    
    #else  
        float3 p;
		p.y = sin(Latitude * PI);
		p.x = p.y * sin(Longitude * (PI * -2.0));
		p.z = p.y * sin((Longitude + 0.25) * (PI * 2.0));
		p.y = sin((Latitude - 0.5) * PI);
		Out = p;
    #endif
}

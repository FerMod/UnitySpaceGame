// Interpolation functions.
float NearestNeighbour(float a,float b,float t){
	return t < 0.5 ? a : b;
}
// float lerp(float a,float b,float t) {
// 	return a + t * (b - a);
// }
float Cosine(float a,float b,float t){
	t = (1 - cos(t * PI)) * 0.5;
	return a + t * (b - a);
}

void Checkerboard3D_float(float3 Position, float Size, out float Out){
	float size = Size;
	float isize = 1.0 / Size;
	
    float x0 = floor(Position.x * isize);
    float y0 = floor(Position.y * isize);
    float z0 = floor(Position.z * isize);
    
    float x0v = x0 % 2;
    float y0v = y0 % 2;
    float z0v = z0 % 2;
    
    float value = (x0v + y0v + z0v) % 2 ? 1 : -1;

    Out = value;
}

void Checkerboard3DLinear_float(float3 Position, float Size, out float Out){
	float size = Size;
	float isize = 1 / Size;
	
    float x0 = floor(Position.x * isize);
    float y0 = floor(Position.y * isize);
    float z0 = floor(Position.z * isize);
    
    float x0v = x0 % 2;
    float y0v = y0 % 2;
    float z0v = z0 % 2;
    
    float value = (x0v + y0v + z0v) % 2 ? 1 : -1;
    
    float tx = (Position.x - x0 * size) * isize;
    float ty = (Position.y - y0 * size) * isize;
    float tz = (Position.z - z0 * size) * isize;
    
    float lx = lerp(value, -value, tx);
    float ly = lerp(lx, -lx, ty);
    float lz = lerp(ly, -ly, tz);
    
    Out = lz;
}

void Checkerboard3DNearest_float(float3 Position, float Size, out float Out){
	float size = Size;
	float isize = 1.0 / Size;
	
    float x0 = floor(Position.x * isize);
    float y0 = floor(Position.y * isize);
    float z0 = floor(Position.z * isize);
    
    float x0v = x0 % 2;
    float y0v = y0 % 2;
    float z0v = z0 % 2;
    
    float value = (x0v + y0v + z0v) % 2 ? 1 : -1;
    
    float tx = (Position.x - x0 * size) * isize;
    float ty = (Position.y - y0 * size) * isize;
    float tz = (Position.z - z0 * size) * isize;
    
    float lx = NearestNeighbour(value, -value, tx);
    float ly = NearestNeighbour(lx, -lx, ty);
    float lz = NearestNeighbour(ly, -ly, tz);
    
    Out = lz;
}

void Checkerboard3DCosine_float(float3 Position, float Size, out float Out){
	float size = Size;
	float isize = 1.0 / Size;
	
    float x0 = floor(Position.x * isize);
    float y0 = floor(Position.y * isize);
    float z0 = floor(Position.z * isize);
    
    float x0v = x0 % 2;
    float y0v = y0 % 2;
    float z0v = z0 % 2;
    
    float value = (x0v + y0v + z0v) % 2 ? 1 : -1;
    
    float tx = (Position.x - x0 * size) * isize;
    float ty = (Position.y - y0 * size) * isize;
    float tz = (Position.z - z0 * size) * isize;
    
    float lx = Cosine(value, -value, tx);
    float ly = Cosine(lx, -lx, ty);
    float lz = Cosine(ly, -ly, tz);
    
    Out = lz;
}
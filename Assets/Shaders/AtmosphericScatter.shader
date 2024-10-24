Shader "Converted/Template"
{
    Properties
    {
        _MainTex ("iChannel0", 2D) = "white" {}
        _SecondTex ("iChannel1", 2D) = "white" {}
        _ThirdTex ("iChannel2", 2D) = "white" {}
        _FourthTex ("iChannel3", 2D) = "white" {}
        _Mouse ("Mouse", Vector) = (0.5, 0.5, 0.5, 0.5)
        [ToggleUI] _GammaCorrect ("Gamma Correction", Float) = 1
        _Resolution ("Resolution (Change if AA is bad)", Range(1, 1024)) = 1
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Built-in properties
            sampler2D _MainTex;   float4 _MainTex_TexelSize;
            sampler2D _SecondTex; float4 _SecondTex_TexelSize;
            sampler2D _ThirdTex;  float4 _ThirdTex_TexelSize;
            sampler2D _FourthTex; float4 _FourthTex_TexelSize;
            float4 _Mouse;
            float _GammaCorrect;
            float _Resolution;

            // GLSL Compatability macros
            #define glsl_mod(x,y) (((x)-(y)*floor((x)/(y))))
            #define texelFetch(ch, uv, lod) tex2Dlod(ch, float4((uv).xy * ch##_TexelSize.xy + ch##_TexelSize.xy * 0.5, 0, lod))
            #define textureLod(ch, uv, lod) tex2Dlod(ch, float4(uv, 0, lod))
            #define iResolution float3(_Resolution, _Resolution, _Resolution)
            #define iFrame (floor(_Time.y / 60))
            #define iChannelTime float4(_Time.y, _Time.y, _Time.y, _Time.y)
            #define iDate float4(2020, 6, 18, 30)
            #define iSampleRate (44100)
            #define iChannelResolution float4x4(                      \
                _MainTex_TexelSize.z,   _MainTex_TexelSize.w,   0, 0, \
                _SecondTex_TexelSize.z, _SecondTex_TexelSize.w, 0, 0, \
                _ThirdTex_TexelSize.z,  _ThirdTex_TexelSize.w,  0, 0, \
                _FourthTex_TexelSize.z, _FourthTex_TexelSize.w, 0, 0)

            // Global access to uv data
            static v2f vertex_output;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv =  v.uv;
                return o;
            }

#define PLANET_POS ((float3)0.)
#define PLANET_RADIUS 6371000.
#define ATMOS_RADIUS 6471000.
#define RAY_BETA float3(0.0000055, 0.000013, 0.0000224)
#define MIE_BETA ((float3)0.000021)
#define AMBIENT_BETA ((float3)0.)
#define ABSORPTION_BETA float3(0.0000204, 0.0000497, 0.00000195)
#define G 0.7
#define HEIGHT_RAY 8000.
#define HEIGHT_MIE 1200.
#define HEIGHT_ABSORPTION 30000.
#define ABSORPTION_FALLOFF 4000.
#if HW_PERFORMANCE==0
#define PRIMARY_STEPS 12
#define LIGHT_STEPS 4
#else
#define PRIMARY_STEPS 32
#define LIGHT_STEPS 8
#endif
#define CAMERA_MODE 2
            float3 calculate_scattering(float3 start, float3 dir, float max_dist, float3 scene_color, float3 light_dir, float3 light_intensity, float3 planet_position, float planet_radius, float atmo_radius, float3 beta_ray, float3 beta_mie, float3 beta_absorption, float3 beta_ambient, float g, float height_ray, float height_mie, float height_absorption, float absorption_falloff, int steps_i, int steps_l)
            {
                start -= planet_position;
                float a = dot(dir, dir);
                float b = 2.*dot(dir, start);
                float c = dot(start, start)-atmo_radius*atmo_radius;
                float d = b*b-4.*a*c;
                if (d<0.)
                    return scene_color;
                    
                float2 ray_length = float2(max((-b-sqrt(d))/(2.*a), 0.), min((-b+sqrt(d))/(2.*a), max_dist));
                if (ray_length.x>ray_length.y)
                    return scene_color;
                    
                bool allow_mie = max_dist>ray_length.y;
                ray_length.y = min(ray_length.y, max_dist);
                ray_length.x = max(ray_length.x, 0.);
                float step_size_i = (ray_length.y-ray_length.x)/float(steps_i);
                float ray_pos_i = ray_length.x+step_size_i*0.5;
                float3 total_ray = ((float3)0.);
                float3 total_mie = ((float3)0.);
                float3 opt_i = ((float3)0.);
                float2 scale_height = float2(height_ray, height_mie);
                float mu = dot(dir, light_dir);
                float mumu = mu*mu;
                float gg = g*g;
                float phase_ray = 3./50.265484*(1.+mumu);
                float phase_mie = allow_mie ? 3./25.132742*((1.-gg)*(mumu+1.))/(pow(1.+gg-2.*mu*g, 1.5)*(2.+gg)) : 0.;
                for (int i = 0;i<steps_i; ++i)
                {
                    float3 pos_i = start+dir*ray_pos_i;
                    float height_i = length(pos_i)-planet_radius;
                    float3 density = float3(exp(-height_i/scale_height), 0.);
                    float denom = (height_absorption-height_i)/absorption_falloff;
                    density.z = 1./(denom*denom+1.)*density.x;
                    density *= step_size_i;
                    opt_i += density;
                    a = dot(light_dir, light_dir);
                    b = 2.*dot(light_dir, pos_i);
                    c = dot(pos_i, pos_i)-atmo_radius*atmo_radius;
                    d = b*b-4.*a*c;
                    float step_size_l = (-b+sqrt(d))/(2.*a*float(steps_l));
                    float ray_pos_l = step_size_l*0.5;
                    float3 opt_l = ((float3)0.);
                    for (int l = 0;l<steps_l; ++l)
                    {
                        float3 pos_l = pos_i+light_dir*ray_pos_l;
                        float height_l = length(pos_l)-planet_radius;
                        float3 density_l = float3(exp(-height_l/scale_height), 0.);
                        float denom = (height_absorption-height_l)/absorption_falloff;
                        density_l.z = 1./(denom*denom+1.)*density_l.x;
                        density_l *= step_size_l;
                        opt_l += density_l;
                        ray_pos_l += step_size_l;
                    }
                    float3 attn = exp(-beta_ray*(opt_i.x+opt_l.x)-beta_mie*(opt_i.y+opt_l.y)-beta_absorption*(opt_i.z+opt_l.z));
                    total_ray += density.x*attn;
                    total_mie += density.y*attn;
                    ray_pos_i += step_size_i;
                }
                float3 opacity = exp(-(beta_mie*opt_i.y+beta_ray*opt_i.x+beta_absorption*opt_i.z));
                return (phase_ray*beta_ray*total_ray+phase_mie*beta_mie*total_mie+opt_i.x*beta_ambient)*light_intensity+scene_color*opacity;
            }

            float2 ray_sphere_intersect(float3 start, float3 dir, float radius)
            {
                float a = dot(dir, dir);
                float b = 2.*dot(dir, start);
                float c = dot(start, start)-radius*radius;
                float d = b*b-4.*a*c;
                if (d<0.)
                    return float2(100000., -100000.);
                    
                return float2((-b-sqrt(d))/(2.*a), (-b+sqrt(d))/(2.*a));
            }

            float3 skylight(float3 sample_pos, float3 surface_normal, float3 light_dir, float3 background_col)
            {
                surface_normal = normalize(lerp(surface_normal, light_dir, 0.6));
                return calculate_scattering(sample_pos, surface_normal, 3.*ATMOS_RADIUS, background_col, light_dir, ((float3)40.), PLANET_POS, PLANET_RADIUS, ATMOS_RADIUS, RAY_BETA, MIE_BETA, ABSORPTION_BETA, AMBIENT_BETA, G, HEIGHT_RAY, HEIGHT_MIE, HEIGHT_ABSORPTION, ABSORPTION_FALLOFF, LIGHT_STEPS, LIGHT_STEPS);
            }

            float4 render_scene(float3 pos, float3 dir, float3 light_dir)
            {
                float4 color = float4(0., 0., 0., 1000000000000.);
                color.xyz = ((float3)dot(dir, light_dir)>0.9998 ? 3. : 0.);
                float2 planet_intersect = ray_sphere_intersect(pos-PLANET_POS, dir, PLANET_RADIUS);
                if (0.<planet_intersect.y)
                {
                    color.w = max(planet_intersect.x, 0.);
                    float3 sample_pos = pos+dir*planet_intersect.x-PLANET_POS;
                    float3 surface_normal = normalize(sample_pos);
                    color.xyz = float3(0., 0.25, 0.05);
                    float3 N = surface_normal;
                    float3 V = -dir;
                    float3 L = light_dir;
                    float dotNV = max(0.000001, dot(N, V));
                    float dotNL = max(0.000001, dot(N, L));
                    float shadow = dotNL/(dotNL+dotNV);
                    color.xyz *= shadow;
                    color.xyz += clamp(skylight(sample_pos, surface_normal, light_dir, ((float3)0.))*float3(0., 0.25, 0.05), 0., 1.);
                }
                
                return color;
            }

            float3 get_camera_vector(float3 resolution, float2 coord)
            {
                float2 uv = coord.xy/resolution.xy-((float2)0.5);
                uv.x *= resolution.x/resolution.y;
                return normalize(float3(uv.x, uv.y, -1.));
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float3 camera_vector = get_camera_vector(iResolution, fragCoord);
#if CAMERA_MODE==0
                float3 camera_position = float3(0., PLANET_RADIUS+100., 0.);
#endif
#if CAMERA_MODE==1
                float3 camera_position = float3(0., ATMOS_RADIUS, ATMOS_RADIUS);
#endif
#if CAMERA_MODE==2
                float3 camera_position = float3(0., ATMOS_RADIUS+-cos(_Time.y/2.)*(ATMOS_RADIUS-PLANET_RADIUS-1.), 0.);
#endif
#if CAMERA_MODE==3
                float offset = (1.-cos(_Time.y/2.))*ATMOS_RADIUS;
                float3 camera_position = float3(0., PLANET_RADIUS+1., offset);
#endif
                float3 light_dir = _Mouse.y==0. ? normalize(float3(0., cos(-_Time.y/8.), sin(-_Time.y/8.))) : normalize(float3(0., cos(_Mouse.y*-5./iResolution.y), sin(_Mouse.y*-5./iResolution.y)));
                float4 scene = render_scene(camera_position, camera_vector, light_dir);
                float3 col = ((float3)0.);
                col += calculate_scattering(camera_position, camera_vector, scene.w, scene.xyz, light_dir, ((float3)40.), PLANET_POS, PLANET_RADIUS, ATMOS_RADIUS, RAY_BETA, MIE_BETA, ABSORPTION_BETA, AMBIENT_BETA, G, HEIGHT_RAY, HEIGHT_MIE, HEIGHT_ABSORPTION, ABSORPTION_FALLOFF, PRIMARY_STEPS, LIGHT_STEPS);
                col = 1.-exp(-col);
                fragColor = float4(col, 1.);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}

#version 330 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;

out vec2 TexCoords;
out vec3 Normal;
out vec3 FragPos;
out vec4 FragPosLightSpace;

uniform mat4 transform;
uniform mat4 model;
uniform mat4 viewProjection;
uniform mat4 lightSpaceMatrix;

void main()
{
    FragPos = vec3(vec4(aPosition, 1.0) * model);
    Normal = aNormal * mat3(transpose(inverse(model)));
    TexCoords = aTexCoord;
    FragPosLightSpace = vec4(FragPos, 1.0) * lightSpaceMatrix;
    gl_Position = vec4(aPosition, 1.0) * model * viewProjection;

}
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum ParticleColor
{
    Red,
    Green,
    Blue
}

public struct ParticlePosition : IComponentData
{
    public float2 Value;
}

public struct ParticleVelocity : IComponentData
{
    public float2 Value;
}

public struct ParticleType : IComponentData
{
    public ParticleColor Value;
}

public partial struct ParticleInitializerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        EntityCommandBuffer ecb = new(Allocator.Temp);

        int particleCount = 1000;
        for (int i = 0; i < particleCount; i++)
        {
            Entity entity = ecb.CreateEntity();
            ecb.AddComponent(entity, new ParticlePosition { Value = UnityEngine.Random.insideUnitCircle * 10 });
            ecb.AddComponent(entity, new ParticleVelocity { Value = float2.zero });
            ecb.AddComponent(entity, new ParticleType { Value = (ParticleColor)UnityEngine.Random.Range(0, 3) });
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct ParticleLifeSystem : ISystem
{
    private const int TYPE_COUNT = 3;
    private static readonly float[,] attractionMatrix = new float[TYPE_COUNT, TYPE_COUNT]
    {
        {  1.0f, -0.5f,  0.3f },
        { -0.2f,  1.0f, -0.8f },
        {  0.1f, -0.1f,  1.0f }
    };

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ParticlePosition>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        EntityQuery query = state.GetEntityQuery(
            ComponentType.ReadOnly<ParticlePosition>(),
            ComponentType.ReadOnly<ParticleType>());

        var positions = query.ToComponentDataArray<ParticlePosition>(Allocator.Temp);
        var types = query.ToComponentDataArray<ParticleType>(Allocator.Temp);
        var entities = query.ToEntityArray(Allocator.Temp);

        int count = positions.Length;
        var velocities = new NativeArray<ParticleVelocity>(count, Allocator.Temp);

        for (int i = 0; i < count; i++)
        {
            float2 acc = float2.zero;
            float2 posI = positions[i].Value;
            ParticleColor typeI = types[i].Value;

            for (int j = 0; j < count; j++)
            {
                if (i == j) continue;
                float2 dir = positions[j].Value - posI;
                float dist = math.length(dir);
                if (dist > 0.01f && dist < 5f)
                {
                    float force = attractionMatrix[(int)typeI, (int)types[j].Value];
                    acc += math.normalize(dir) * (force / dist);
                }
            }

            velocities[i] = new ParticleVelocity
            {
                Value = math.clamp(acc * dt * 10f, -10f, 10f)
            };
        }

        for (int i = 0; i < count; i++)
        {
            var pos = positions[i];
            var vel = velocities[i];
            pos.Value += vel.Value * dt;
            state.EntityManager.SetComponentData(entities[i], pos);
            state.EntityManager.SetComponentData(entities[i], vel);
        }

        positions.Dispose();
        types.Dispose();
        entities.Dispose();
        velocities.Dispose();
    }
}

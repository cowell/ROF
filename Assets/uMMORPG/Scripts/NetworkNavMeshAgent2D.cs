// UNET's current NetworkTransform is really laggy, so we make it smooth by
// simply synchronizing the agent's destination. We could also lerp between
// the transform positions, but this is much easier and saves lots of bandwidth.
//
// Using a NavMeshAgent also has the benefit that no rotation has to be synced
// while moving.
//
// Notes:
//
// - Teleportations have to be detected and synchronized properly
// - Caching the agent won't work because serialization sometimes happens
//   before awake/start
// - We also need the stopping distance, otherwise entities move too far.
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent2D))]
// unreliable is enough and we want
[NetworkSettings(channel=Channels.DefaultUnreliable, sendInterval=0.1f)]
public class NetworkNavMeshAgent2D : NetworkBehaviour
{
    public NavMeshAgent2D agent; // assign in Inspector (instead of GetComponent)
    Vector2 requiredVelocity; // to apply received velocity in Update constanly

    // remember last serialized values for dirty bit
    Vector2 lastUpdatePosition;
    Vector2 lastSerializedDestination;
    Vector2 lastSerializedVelocity;

    void Update()
    {
        if (isServer)
        {
            // detect move mode
            bool hasPath = agent.hasPath || agent.pathPending; // might still be computed

            // click movement and destination changed since last sync?
            if (hasPath && agent.destination != lastSerializedDestination)
            {
                //Debug.LogWarning(name + " dirty because destination changed from: " + lastSerializedDestination + " to " + agent.destination + " hasPath=" + agent.hasPath + " pathPending=" + agent.pathPending);
                SetDirtyBit(1);
            }
            // wasd movement and velocity changed since last sync?
            else if (!hasPath && agent.velocity != lastSerializedVelocity)
            {
                //Debug.LogWarning(name + " dirty because velocity changed from: " + lastVelocity + " to " + agent.velocity);
                SetDirtyBit(1);
            }
            // neither click or wasd movement, but position changed further than 'speed'?
            // then we must have teleported, no other way to move this fast.
            else if (!hasPath && Vector2.Distance(transform.position, lastUpdatePosition) > agent.speed)
            {
                // warp detection is just about 100% correct, so let's send a
                // Rpc to warp the client and not leave it up to OnDeserialize's
                // guess wether or not we warped. this is worth it for corerctness.
                //Debug.Log(name + " teleported from: " + lastUpdatePosition + " to: " + transform.position);
                RpcWarped(transform.position);
            }

            lastUpdatePosition = transform.position;
        }
        else if (isClient)
        {
            // apply velocity constantly, not just in OnDeserialize
            // (not on host because server handles it already anyway)
            if (requiredVelocity != Vector2.zero)
            {
                agent.ResetPath(); // needed after click movement before we can use .velocity
                agent.velocity = requiredVelocity;
            }
        }
    }

    [ClientRpc(channel=Channels.DefaultReliable)] // important => reliable
    public void RpcWarped(Vector2 position)
    {
        agent.Warp(position);
    }

    // server-side serialization
    //
    // I M P O R T A N T
    //
    // always read and write the same amount of bytes. never let any errors
    // happen. otherwise readstr/readbytes out of range bugs happen.
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        // always send position so client knows if he's too far off and needs warp
        writer.Write((Vector2)transform.position);

        // always send speed in case it's modified by something
        writer.Write(agent.speed);

        // click or wasd movement?
        // (no need to send everything all the time, saves bandwidth)
        bool hasPath = agent.hasPath || agent.pathPending;
        writer.Write(hasPath);
        if (hasPath)
        {
            // destination
            writer.Write(agent.destination);

            // always send stopping distance because monsters might stop early etc.
            writer.Write(agent.stoppingDistance);

            // remember last serialized path so we do it again if it changed.
            // (first OnSerialize never seems to detect path yet for whatever
            //  reason, so this way we can be 100% sure that it's called again
            //  as soon as the path was detected)
            lastSerializedDestination = agent.destination;
        }
        else
        {
            // velocity
            writer.Write(agent.velocity);

            // remember last serialized velocity
            lastSerializedVelocity = agent.velocity;
        }
        return true;
    }

    // client-side deserialization
    //
    // I M P O R T A N T
    //
    // always read and write the same amount of bytes. never let any errors
    // happen. otherwise readstr/readbytes out of range bugs happen.
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        // read position, speed and movement type
        Vector2 position = reader.ReadVector2();
        agent.speed = reader.ReadSingle();
        bool hasPath = reader.ReadBoolean();

        // click or wasd movement?
        if (hasPath)
        {
            // read destination and stopping distance
            Vector2 destination = reader.ReadVector2();
            float stoppingDistance = reader.ReadSingle();
            //Debug.Log("OnDeserialize: click: " + destination);

            // try setting destination if on navmesh
            // (might not be while falling from the sky after joining etc.)
            if (agent.isOnNavMesh)
            {
                agent.stoppingDistance = stoppingDistance;
                agent.destination = destination;
            }
            else Debug.LogWarning("NetworkNavMeshAgent.OnSerialize: agent not on NavMesh, name=" + name + " position=" + transform.position + " destination=" + destination);

            requiredVelocity = Vector2.zero; // reset just to be sure
        }
        else
        {
            // read velocity
            Vector2 velocity = reader.ReadVector2();
            //Debug.Log("OnDeserialize: wasd: " + velocity);

            // apply required velocity in Update later
            requiredVelocity = velocity;
        }

        // rubberbanding: if we are too far off because of a rapid position
        // change or latency, then warp
        // -> agent moves 'speed' meter per seconds
        // -> if we are speed * 2 units behind, then we teleport
        //    (using speed is better than using a hardcoded value)
        // -> we use speed * 2 for update/network latency tolerance. player
        //    might have moved quit a bit already before OnSerialize was called
        //    on the server.
        if (Vector2.Distance(transform.position, position) > agent.speed * 2 && agent.isOnNavMesh)
        {
            agent.Warp(position);
            //Debug.Log(name + " rubberbanding to " + position);
        }
    }
}

// Draws the agent's path as Gizmo.
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent2D))]
public class NavmeshPath2DGizmo : MonoBehaviour
{
    public NavMeshAgent2D agent;

    void OnDrawGizmos()
    {
        // only while game is running, otherwise navmeshagent2d has no 3d agent.
        if (Application.isPlaying)
        {
            NavMeshPath2D path = agent.path;

            // color depends on status
            Color color = Color.white;
            switch (path.status)
            {
                case NavMeshPathStatus.PathComplete: color = Color.white; break;
                case NavMeshPathStatus.PathInvalid: color = Color.red; break;
                case NavMeshPathStatus.PathPartial: color = Color.yellow; break;
            }

            // draw the path
            for (int i = 1; i < path.corners.Length; ++i)
                Debug.DrawLine(path.corners[i-1], path.corners[i], color);

            // draw velocity
            Debug.DrawLine(transform.position, transform.position + (Vector3)agent.velocity, Color.blue, 0, false);
        }
    }
}
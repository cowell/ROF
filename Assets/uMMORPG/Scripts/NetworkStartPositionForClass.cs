// Simple script that inherits from NetworkStartPosition to make class based
// spawns.
using UnityEngine;
using UnityEngine.Networking;

public class NetworkStartPositionForClass : NetworkStartPosition
{
    public Player playerPrefab;
}

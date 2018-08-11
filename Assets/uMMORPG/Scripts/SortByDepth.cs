// simple script to always set y position to order in layer for visiblity
using UnityEngine;

public class SortByDepth : MonoBehaviour
{
    new public SpriteRenderer renderer;

    // precision is useful for cases where two players stand at
    //   y=0 and y=0.1, which would both be sortingOrder=0 otherwise
    public int precision = 100;

    void Update()
    {
        // we negate it because that's how Unity's sorting order works
        renderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * precision);
    }
}

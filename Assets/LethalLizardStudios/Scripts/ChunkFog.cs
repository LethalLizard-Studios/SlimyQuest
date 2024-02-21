using System.Collections.Generic;
using UnityEngine;

public class ChunkFog : MonoBehaviour
{
    [SerializeField] private Transform leftFog;
    [SerializeField] private Transform rightFog;

    private float leftRenderedChunk = -1000;
    private float rightRenderedChunk = 1000;

    private void UpdateFogPosition()
    {
        leftFog.position = new Vector3(leftRenderedChunk+2, 0, 0);
        rightFog.position = new Vector3(rightRenderedChunk+32, 0, 0);
    }

    public void SetBorder(ref Dictionary<Vector3, Chunks> chunks)
    {
        float rightMost = -1;
        float leftMost = -1;

        foreach (Vector3 activeChunk in chunks.Keys)
        {
            if (rightMost == -1 || leftMost == -1)
            {
                rightMost = activeChunk.x;
                leftMost = activeChunk.x;
            }
            else
            {
                if (activeChunk.x < leftMost)
                    leftMost = activeChunk.x;
                if (activeChunk.x > rightMost)
                    rightMost = activeChunk.x;
            }
        }

        leftRenderedChunk = leftMost;
        rightRenderedChunk = rightMost;
        UpdateFogPosition();
    }
}

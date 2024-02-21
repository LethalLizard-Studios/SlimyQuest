using UnityEngine;

public class Drill : MonoBehaviour
{
    private void Start()
    {
        Miner miner;

        if (transform.parent.TryGetComponent<Miner>(out miner))
        {
            miner.SetupBlock(transform, 0);
        }
    }
}

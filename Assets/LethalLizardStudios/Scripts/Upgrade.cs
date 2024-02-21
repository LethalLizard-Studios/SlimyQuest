using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private int type = 1;

    private void Start()
    {
        Miner miner;

        if (transform.parent.TryGetComponent<Miner>(out miner))
        {
            miner.SetupBlock(transform, type);
        }
    }
}

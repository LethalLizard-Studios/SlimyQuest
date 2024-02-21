using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Machine
{
    [SerializeField] private float LerpMultiplier = 2;

    private Vector3 nextPos = new Vector3();

    private List<Transform> drills = new List<Transform>();

    private int speedMultiplier = 1;

    void Start()
    {
        nextPos = transform.position;

        StartCoroutine(BeginExpedition());
    }

    IEnumerator BeginExpedition()
    {
        nextPos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        yield return new WaitForSeconds(2.0f / speedMultiplier);

        DrillBlocks();

        if (transform.position.y > 5)
            StartCoroutine(BeginExpedition());
        else
            Destroy(gameObject);
    }

    private void DrillBlocks()
    {
        for (int i = 0; i < drills.Count; i++)
        {
            //Only allow 3 drills max
            if (i >= 3)
                return;

            BlockSelection.Instance.PlaceBlock(0, new Vector3(drills[i].position.x, drills[i].position.y - 1.5f, 1.5f), false);
        }
    }

    public void SetupBlock(Transform pos, int type)
    {
        switch (type)
        {
            case 0:
                drills.Add(pos);
                break;
            case 1:
                speedMultiplier = Mathf.Clamp(speedMultiplier + 2, 1, 7);
                break;
        }
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, nextPos, Time.deltaTime * LerpMultiplier * speedMultiplier);
    }
}

using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleTrigger : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;

    private void OnTriggerExit(Collider other)
    {
        Play(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        Play(other);
    }

    private void Play(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            particle.transform.parent = other.transform;
            particle.transform.position = other.transform.position;
            particle.Play();
        }
    }
}

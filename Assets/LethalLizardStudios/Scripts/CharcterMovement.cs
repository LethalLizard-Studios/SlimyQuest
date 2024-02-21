using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharcterMovement : MonoBehaviour
{
    CharacterController cc;

    [SerializeField] private Transform playerModel;

    public Transform particleSpawn;
    public ParticleSystem splashParticles;
    public ParticleSystem poofParticles;
    public ParticleSystem oozeParticles;

    [SerializeField] private MultiSFX jumpSFX;
    [SerializeField] private AudioClip[] jumpClips;

    public float speed = 6f;
    public float gForce = 20f;
    //public float jumpSpeed = 10;
    private float xSpeed;
    private float ySpeed;
    private float movementSpeed;
    public Vector2 moveDirection;

    private float skyboxRotation = 0f;

    public AnimationCurve jumpCurve;
    float jumpProgress = 0;

    private float touchedGround = 3;

    private bool jumped = false;
    private bool canDouble = false;
    private int jumpsLeft = 1;

    private bool hasLanded = false;
    private Quaternion rotation = Quaternion.identity;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Input.GetButtonUp("Jump"))
        {
            canDouble = true;
        }
        else if (!jumped && Input.GetButtonDown("Jump"))
        {
            jumpSFX.clip = jumpClips[Random.Range(0, jumpClips.Length)];
            jumpSFX.Pitch(Random.Range(0.98f, 1.02f));
            jumpSFX.Play();
        }

        playerModel.position = Vector3.Lerp(playerModel.position, transform.position, Time.deltaTime * 32f);
        playerModel.localRotation = Quaternion.Lerp(playerModel.localRotation, rotation, Time.deltaTime * 12f);
    }

    void FixedUpdate()
    {
        if (!oozeParticles.isPlaying && !cc.isGrounded)
            oozeParticles.Play();
        else if (oozeParticles.isPlaying && cc.isGrounded)
            oozeParticles.Stop();

        if (!hasLanded && cc.isGrounded)
        {
            splashParticles.transform.position = particleSpawn.position;
            splashParticles.Play();
            hasLanded = true;
        }

        float side = Input.GetAxis("Horizontal");

        int direction;

        if (side > 0) { direction = 1; }
        else if (side < 0) { direction = -1; } else { direction = 0; }

        rotation = Quaternion.Euler(Mathf.Clamp(moveDirection.y + 7.2f, -32, 32), -direction * 28f, 0);
        Vector3 scale = new Vector3(1, Mathf.Clamp(1 + (Mathf.Abs(moveDirection.y) + -7.2f) / 16f, 0.75f, 1.2f), 1);

        playerModel.localScale = Vector3.Lerp(playerModel.localScale, scale, Time.deltaTime * 18f);

        moveDirection = new Vector2(side, 0);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed + (touchedGround / 3f) * 2.5f;

        skyboxRotation += Time.deltaTime * 1.25f;
        WeatherManager.Instance.currentSkybox.SetFloat("_Rotation", skyboxRotation + (transform.position.x * 0.4f));

        if ((cc.isGrounded || (jumpProgress > 1f && !cc.isGrounded && jumpsLeft > 0 && canDouble)) && Input.GetButton("Jump"))
        {
            if (canDouble)
            {
                //poofParticles.transform.position = particleSpawn.position;
                poofParticles.Play();

                jumpSFX.clip = jumpClips[Random.Range(0, jumpClips.Length)];
                jumpSFX.Pitch(Random.Range(0.98f, 1.02f));
                jumpSFX.Play();
            }

            hasLanded = false;
            jumped = true;
            jumpProgress = 0;
            jumpsLeft--;
        }

        if (jumped)
        {
            jumpProgress += Time.deltaTime * 3;

            if (jumpsLeft != 1)
            {
                canDouble = false;
                touchedGround = 3;
            }

            moveDirection.y += jumpCurve.Evaluate(jumpProgress) * 8;

            if (jumpProgress > 2.5f || (cc.isGrounded && jumpProgress > 0.4f))
                jumped = false;
        }

        moveDirection.y -= gForce * (Time.deltaTime * touchedGround);

        if (!cc.isGrounded)
        {
            float value = touchedGround + (Time.deltaTime * 4);
            touchedGround = Mathf.Clamp(value, 3f, 10f);
        }
        else
        {
            jumpsLeft = 1;
            touchedGround = 3;
        }

        cc.Move(moveDirection * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, transform.position.y, 1.5f);
    }
}

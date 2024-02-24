using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Animator anim;
    public Transform enemy;
    public Transform player;
    public Vector3 offset;
    public float catchUpSeed;
    public float jumpForce;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, transform.position.y, player.position.z) + offset, catchUpSeed);
        transform.rotation = player.rotation;
    }

    public void Jump(float s)
    {
        float t = Vector3.Distance(enemy.position, player.position) / s;
        Invoke("StartJump", t);
    }

    public void StartJump()
    {
        anim.SetTrigger("jump");
        rb.velocity = Vector3.up * jumpForce;
    }
}
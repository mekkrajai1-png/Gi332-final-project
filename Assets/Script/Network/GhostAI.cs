using UnityEngine;

public class GhostAI : MonoBehaviour
{
    [Header("Mode 1")]
    public float speedMode1 = 3f;
    public float damageCooldownMode1 = 1f;
    public Sprite spriteMode1;

    [Header("Mode 2")]
    public float xMode2 = 140f;
    public float speedMode2 = 5f;
    public float damageCooldownMode2 = 0.7f;
    public Sprite spriteMode2;

    [Header("Mode 3")]
    public float xMode3 = 242f;
    public float speedMode3 = 7f;
    public float damageCooldownMode3 = 0.4f;
    public Sprite spriteMode3;

    [Header("Current Setting")]
    public float speed = 3f;
    public float damageCooldown = 1f;

    private Transform target;
    private float lastDamageTime;

    public bool gameStarted = false;

    private SpriteRenderer spriteRenderer;
    private int currentMode = 1;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetMode(1);
    }

    void Update()
    {
        if (!gameStarted) return;

        CheckModeByPositionX();
        FindClosestPlayer();

        if (target != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }
    }

    void CheckModeByPositionX()
    {
        float x = transform.position.x;

        if (x > xMode3)
        {
            SetMode(3);
        }
        else if (x > xMode2)
        {
            SetMode(2);
        }
        else
        {
            SetMode(1);
        }
    }

    void SetMode(int mode)
    {
        if (currentMode == mode) return;

        currentMode = mode;

        if (mode == 1)
        {
            speed = speedMode1;
            damageCooldown = damageCooldownMode1;

            if (spriteRenderer != null && spriteMode1 != null)
                spriteRenderer.sprite = spriteMode1;
        }
        else if (mode == 2)
        {
            speed = speedMode2;
            damageCooldown = damageCooldownMode2;

            if (spriteRenderer != null && spriteMode2 != null)
                spriteRenderer.sprite = spriteMode2;
        }
        else if (mode == 3)
        {
            speed = speedMode3;
            damageCooldown = damageCooldownMode3;

            if (spriteRenderer != null && spriteMode3 != null)
                spriteRenderer.sprite = spriteMode3;
        }
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject p in players)
        {
            HeartHealth hp = p.GetComponent<HeartHealth>();

            if (hp == null || !hp.IsAlive())
                continue;

            float dist = Vector2.Distance(transform.position, p.transform.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                closest = p.transform;
            }
        }

        target = closest;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!gameStarted) return;

        if (!Unity.Netcode.NetworkManager.Singleton.IsServer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime > damageCooldown)
            {
                HeartHealth hp = collision.gameObject.GetComponent<HeartHealth>();

                if (hp != null && hp.IsAlive())
                {
                    hp.TakeDamage(1);
                    lastDamageTime = Time.time;
                }
            }
        }
    }
}
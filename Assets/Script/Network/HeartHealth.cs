using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;
using UnityEngine.SceneManagement;


public class HeartHealth : NetworkBehaviour
{
    public int maxHealth = 2;

    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public Image heart1;
    public Image heart2;

    public Sprite fullHeart;
    public Sprite emptyHeart;
    
    [Header("UI")]
    public GameObject gameOverUI;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            currentHealth.Value = maxHealth;

        currentHealth.OnValueChanged += OnHealthChanged;

        if (!IsOwner)
            HideHearts();
    }

    void OnHealthChanged(int oldHp, int newHp)
    {
        if (IsOwner)
            UpdateHearts(newHp);

        if (newHp <= 0)
            Die();
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        if (currentHealth.Value <= 0) return;

        currentHealth.Value -= damage;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, maxHealth);
    }

    public bool IsAlive()
    {
        return currentHealth.Value > 0;
    }

    void UpdateHearts(int hp)
    {
        if (heart1)
            heart1.sprite = (hp >= 1) ? fullHeart : emptyHeart;

        if (heart2)
            heart2.sprite = (hp >= 2) ? fullHeart : emptyHeart;
    }

    void Die()
    {
        GetComponent<Controller2D>().enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.velocity = Vector2.zero; 
            rb.bodyType = RigidbodyType2D.Static;
        }

        GetComponent<SpriteRenderer>().color = Color.gray;

        if (IsOwner && gameOverUI != null)
        {
            gameOverUI.SetActive(true);

            
            StartCoroutine(ReturnToMenuAfterDelay());
        }
    }
    IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSeconds(5f);

       
        SceneManager.LoadScene("Main Menu");
    }

    void HideHearts()
    {
        if (heart1) heart1.enabled = false;
        if (heart2) heart2.enabled = false;
    }
}
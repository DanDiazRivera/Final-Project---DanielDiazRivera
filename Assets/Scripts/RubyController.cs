using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public static int level = 1;
    public int maxHealth = 5;
    public GameObject projectilePrefab;
    public Text endText;
    public GameObject HealEffect;
    public GameObject HitEffect;
    private int scoreValue = 0;
    public Text score;
    public Text cogs;
    private int cogsValue = 4;

    public AudioClip throwSound;
    public AudioClip hitSound;
    private BackgroundMusic bgMusic;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    bool gameOver = false;
    bool winFlag = false;
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        endText.text = "";
        score.text = "Robots Fixed: " + scoreValue.ToString() + "/4";
        cogs.text = "Cogs: " + cogsValue.ToString();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        GameObject bgMusicObject = GameObject.FindWithTag("BackgroundMusic"); 
        if (bgMusicObject != null)
        {
            bgMusic = bgMusicObject.GetComponent<BackgroundMusic>(); 
        }
        bgMusic.ChangeSound(0);
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C) && gameOver == false)
        {
            Launch();
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
            } else if(winFlag == true){
                level = 1;
                SceneManager.LoadScene("Main");
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && gameOver == false)
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (scoreValue == 4)
                {
                    level = 2;
                    SceneManager.LoadScene("Level2");
                } else 
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0 && currentHealth > 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject projectileObject = Instantiate(HitEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }
        if (amount > 0)
        {
            GameObject projectileObject = Instantiate(HealEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        if (currentHealth == 0 && gameOver == false)
        {
            endText.text = "You lost! Press R to restart";
            gameOver = true;
            speed = 0.0f;
            bgMusic.ChangeSound(2);
        }
    }
    public void ChangeScore(int scoreAmount)
    {
        scoreValue += scoreAmount;
        score.text = "Robots Fixed: " + scoreValue.ToString() + "/4";
        if (scoreValue == 4 && gameOver == false && level == 1){
            endText.text = "Talk to Jambi to visit stage two!";
        }
        if(scoreValue == 4 && gameOver == false && level == 2){
            endText.text = "You Win! Game Created By: Daniel Diaz-Rivera.\nPress R to Return to Stage 1";
            winFlag = true;
            bgMusic.ChangeSound(1);
        }
    }
    public void ChangeCogs(int cogsAmount)
    {
        cogsValue += cogsAmount;
        cogs.text = "Cogs: " + cogsValue.ToString();
        GameObject projectileObject = Instantiate(HealEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
         
    }

    void Launch()
    {
        if(cogsValue > 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);
            animator.SetTrigger("Launch");
            PlaySound(throwSound);
            cogsValue--;
            cogs.text = "Cogs: " + cogsValue.ToString();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
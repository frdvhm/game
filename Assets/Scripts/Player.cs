using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;

    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private Collider2D[] results;
    private Vector2 direction;
    public float moveSpeed = 1f;
    public float jumpStrength = 1f;
    private bool grounded;
    private bool climbing;

    // Variabel untuk tombol virtual
    public bool moveLeft;
    public bool moveRight;
    public bool jump;

    public AudioClip hitSound; // Assign hitsound.mp3 in the Inspector
    public AudioClip jumpSound; // Assign jump.mp3 here
    public AudioClip pickupDiamondSound;
    private AudioSource audioSource;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        results = new Collider2D[4];
    }

    void Start()
    {
        // Add AudioSource if not already present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f / 12f, 1f / 12f); // Animasi sprite setiap 1/12 detik
    }

    private void OnDisable()
    {
        CancelInvoke();
        rigidbody.gravityScale = 1f; // Reset gravitasi saat pemain tidak aktif
    }

    private void CheckCollision()
    {
        grounded = false;
        climbing = false;

        Vector2 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;

        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, results);

        for (int i = 0; i < amount; i++)
        {
            GameObject hit = results[i].gameObject;

            if (hit.layer == LayerMask.NameToLayer("Ground"))
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f);
                Physics2D.IgnoreCollision(collider, results[i], !grounded);
            }
            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                climbing = true;
                rigidbody.gravityScale = 0f;
            }
        }
    }

    private void Update()
    {
        // Cek apakah Tokoh berada di tanah atau sedang memanjat
        CheckCollision();

        // Pergerakan di vertical (untuk climbing)
        if (climbing)
        {
            direction.y = Input.GetAxis("Vertical") * moveSpeed;

            if (Input.GetButton("Jump") || jump) // Untuk lompat
            {
                direction.y = moveSpeed;
            }
        }
        else if (grounded && (Input.GetButtonDown("Jump") || jump)) // Melompat dengan tombol virtual atau keyboard
        {
            direction = Vector2.up * jumpStrength;
            jump = false; // Reset tombol lompat virtual setelah digunakan

            // Play Jump Sound
            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
        else
        {
            direction += Physics2D.gravity * Time.deltaTime;
        }

        // Input horizontal (panah kiri-kanan atau tombol virtual)
        float horizontalInput = 0f;

        // Keyboard input (panah kiri-kanan)
        if (Input.GetKey(KeyCode.LeftArrow) || moveLeft) // Move left
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || moveRight) // Move right
        {
            horizontalInput = 1f;
        }

        // Set ke arah horizontal berdasarkan input
        direction.x = horizontalInput * moveSpeed;

        // Menjaga Tokoh agar tidak jatuh terlalu cepat saat berada di tanah
        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }

        // Pembalikan arah saat bergerak
        if (direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (!climbing)
        {
            direction.y += Physics2D.gravity.y * Time.fixedDeltaTime;
        }

        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);
    }

    private void AnimateSprite()
    {
        if (climbing)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direction.x != 0f)
        {
            spriteIndex++;

            if (spriteIndex >= runSprites.Length)
            {
                spriteIndex = 0;
            }

            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }

    public void MoveLeft(bool isPressed)
    {
        moveLeft = isPressed;
    }

    public void MoveRight(bool isPressed)
    {
        moveRight = isPressed;
    }

    public void Jump(bool isPressed)
    {
        jump = isPressed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective"))
        {
            // Play the Pickup Diamond Sound
            if (pickupDiamondSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupDiamondSound);
            }
            enabled = false;
            GameManager.Instance.LevelComplete();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            PlayHitSound();

            GameManager.Instance.LevelFailed();
        }
    }

    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound); // Play the hit sound
        }
    }
}

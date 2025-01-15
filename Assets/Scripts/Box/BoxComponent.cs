using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class BoxComponent : MonoBehaviour
{
    // the box's attack type
    public enum Type {
        HORIZONTAL,
        VERTICAL
    }

    public UnityEvent onBoxDeathEvent;
    public UnityEvent onBoxHitEvent;

    [Header("References")]
    public GameObject horizontalSprite;
    public GameObject verticalSprite;
    public GameObject lightObject;

    [Header("Box Settings")]
    [SerializeField] public int maxHealth = 10; // the max health of the box
    [SerializeField] public float speed = 2.0f; // how fast the box should move
    [SerializeField] public float iFrameTime = 1.0f; // the number of seconds the box will be invincible for after taking damage
    [SerializeField] private Type boxType = Type.HORIZONTAL; // the type of collision to deal damage to other boxes
    [SerializeField] private Vector2 minScale = new Vector2(0, 0);
    [SerializeField] private Vector2 maxScale = new Vector2(2, 2);

    private float iFrameTimer = 0.0f;

    public float currentSpeed = 2.0f;

    public int health = 10; // the health the box has

    private Rigidbody2D rb;

    private void Start(){

        currentSpeed = speed;

        // register listener
        onBoxHitEvent.AddListener(GameManager.Instance.onBoxDamage);

        // turn off both sprites
        horizontalSprite.SetActive(false); verticalSprite.SetActive(false);

        // based on type, turn on sprite
        if (boxType == Type.HORIZONTAL){
            horizontalSprite.SetActive(true);
        }
        else if (boxType == Type.VERTICAL){
            verticalSprite.SetActive(true);
        }

        // fetch rb
        rb = GetComponent<Rigidbody2D>();
        if(rb == null){
            // send error and close application
            Debug.Log("The Box component is missing a Rigidbody2D!");
            Application.Quit();
        }

        // give the box a initial random linear velocity
        Vector2 randomVelocity = new Vector2(Random.value, Random.value).normalized;
        rb.linearVelocity = randomVelocity * currentSpeed;
    }

    private void Update(){
        float healthPercentage = (float) health / (float) maxHealth;

        transform.localScale = Vector2.Lerp(minScale, maxScale, healthPercentage);

        // decrement timer
        if(iFrameTimer > 0) {
            iFrameTimer -= Time.deltaTime;
            iFrameTimer = Mathf.Max(0, iFrameTimer); // clamp to 0
        }
    }

    private void FixedUpdate(){
        FixBoxMovement();
    }

    /**
     * Only takes damage if the side of the box it collided with matches the box type it has
     */
    private void OnCollisionEnter2D(Collision2D collision) {
        // play sound
        SoundManager.Instance.playSound(SoundManager.hitSound, 0.25f, 3.0f);

        // fetch the box component
        BoxComponent boxComp = collision.gameObject.GetComponentInParent<BoxComponent>();
        if (boxComp == null) return;
        if (iFrameTimer != 0) return;

        iFrameTimer = iFrameTime;

        // determine the side the box hit
        Type side = Type.HORIZONTAL;

        foreach(ContactPoint2D contact in collision.contacts) {
            //spawn particle at collision
            Vector3 collisionPosition = contact.point; // Get the position of the collision
            ParticleManager.Instance.spawnParticle(ParticleManager.Instance.impactParticle, collisionPosition, Quaternion.identity);

            Vector2 hitNormal = contact.normal;
            if(Mathf.Abs(hitNormal.x) > Mathf.Abs(hitNormal.y)) {
                side = Type.HORIZONTAL;
            }
            else {
                side = Type.VERTICAL;
            }
        }

        if(side == boxComp.boxType) {
            // deal damage
            Damage(1);
        }
    }

    /**
     * Damages the box given a damage value
     */
    private void Damage(int damage) {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0) {
            // trigger death event
            onBoxDeathEvent.Invoke();
            Destroy(gameObject);
        }

        onBoxHitEvent.Invoke();
    }

    /**
     * prevent the box from moving in a straight line consistently
     */
    private void FixBoxMovement() {
        if (rb.linearVelocityX < 0.5) {
            rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * 0.8f;
        }

        if (rb.linearVelocityY < 0.5) {
            rb.linearVelocityY = Mathf.Sign(rb.linearVelocityY) * 0.8f;
        }

        rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
    }

    private void OnDrawGizmos(){
        if (rb == null) return;

        // draw the motion of the box 
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, rb.linearVelocity);
    }

    public void setColor(Color color) {
        SpriteRenderer horizontalRenderer = horizontalSprite.GetComponent<SpriteRenderer>();
        horizontalRenderer.color = new Color(color.r, color.g, color.b, 1);

        SpriteRenderer verticalRenderer = verticalSprite.GetComponent<SpriteRenderer>();
        verticalRenderer.color = new Color(color.r, color.g, color.b, 1);

        Light2D light = lightObject.GetComponent<Light2D>();
        light.color = new Color(color.r, color.g, color.b, 1);
    }


}

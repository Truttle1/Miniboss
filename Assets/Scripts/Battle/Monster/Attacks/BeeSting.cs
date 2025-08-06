using System.Collections;
using UnityEngine;

public class BeeSting : BattleAttack
{
    private GameObject konrad;
    private Animator animator;
    private float xSpeed = 6f;
    private Vector3 walkVelocity;
    private Vector3 start;
    private Rigidbody2D rb;

    [SerializeField]
    private float[] attackSpeeds = { 0.75f, 1f, 1f, 1.5f};

    [SerializeField]
    private int poisonTurns = 3;

    [SerializeField] private AudioClip stingSound;
    [SerializeField] private AudioClip stingSoundPoison;

    void Start()
    {
        konrad = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        walkVelocity = new Vector3(xSpeed, 0.0f, 0.0f);
    }
    private IEnumerator attack()
    {
        // Set attack speed
        float attackSpeed = attackSpeeds[Random.Range(0, attackSpeeds.Length)];

        start = transform.position;
        transform.position = new Vector3(start.x, start.y, -7f);
        transform.localScale = new Vector2(1f, 1f);

        // Walk to Konrad
        animator.speed = attackSpeed;

        animator.SetBool("moving", true);
        while (transform.position.x - 2.0f > konrad.transform.position.x)
        {
            rb.MovePosition(transform.position - walkVelocity * Time.fixedDeltaTime);
            yield return null;
        }
        animator.SetBool("moving", false);

        // Sting
        animator.SetBool("stinging", true);
        yield return new WaitForSeconds(26 / (24.0f * attackSpeed));
        int dmg = konrad.GetComponent<HasHP>().damage(GetComponent<Monster>().attack * 2);

        if(attackSpeed != 1 && !konrad.GetComponent<HasHP>().isBlocking())
        {
            konrad.GetComponent<Konrad>().SetStatusEffect(KonradStatusEffect.Poison, poisonTurns);
            EventBus.Publish(new PlaySFXEvent(stingSoundPoison));
        }
        else 
        {    
            EventBus.Publish(new PlaySFXEvent(stingSound));
        }

        if (konrad.GetComponent<Knockback>() != null)
        {
            konrad.GetComponent<Knockback>().doKnockback(-1.5f, .3f, dmg);
        }

        konrad.GetComponent<Konrad>().DisableBlock();


        // Walk back
        animator.speed = 1.0f;
        yield return new WaitForSeconds(.7f);
        animator.SetBool("stinging", false);
        animator.SetBool("moving", true);
        while (transform.position.x < start.x)
        {
            transform.localScale = new Vector2(-1f, 1f);
            rb.MovePosition(transform.position + walkVelocity * Time.fixedDeltaTime);
            yield return null;
        }
        transform.localScale = new Vector2(1f, 1f);
        animator.SetBool("moving", false);
        rb.MovePosition(start);
        transform.position = new Vector3(start.x, start.y, -1f);
        
        running = false;
    }

    public override void startAttack()
    {
        running = true;
        StartCoroutine(attack());
    }
}

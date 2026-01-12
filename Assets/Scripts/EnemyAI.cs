using UnityEngine;
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;         
    public float chaseDistance = 1f;  
    public float attackDistance = 1f; 

    [Header("Patrol")]
    public Transform[] patrolPoints; 
    private int currentPoint = 0;    

    [Header("References")]
    public Transform player;          
    public Animator anim;            

    [Header("Attack")]
    public int damage = 10;           
    public float atkCooldown = 0.5f; 
    private float lastAtkTime;   

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackDistance)
            Attack();
        else if (distance <= chaseDistance)
            Chase();
        else
            Patrol();
        
    }


    void Patrol()
    {
        anim.SetBool("IsMoving", true);

        Transform target = patrolPoints[currentPoint];

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        Flip(target.position.x - transform.position.x);
    }

    void Chase()
    {
        anim.SetBool("IsMoving", true);

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );
        Flip(player.position.x - transform.position.x);
    }
    void Attack()
    {
        if (Time.time < lastAtkTime + atkCooldown)
            return;
        anim.SetTrigger("Attack");

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
            stats.TakeDamage(damage);

        lastAtkTime = Time.time;
    }
    void Flip(float directionX)
    {
        if (Mathf.Abs(directionX) < 0.01f) 
        return;

        transform.localScale = new Vector3(
            Mathf.Sign(directionX),1,1);
    }
}
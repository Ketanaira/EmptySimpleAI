using UnityEngine;

// Ten skrypt steruje zachowaniem przeciwnika.
// Wróg potrafi: patrolować, gonić gracza i atakować.

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;          // prędkość poruszania się przeciwnika
    public float chaseDistance = 4f;  // odległość, od której wróg zaczyna gonić gracza
    public float attackDistance = 1f; // odległość, od której wróg atakuje

    [Header("Patrol")]
    public Transform[] patrolPoints;  // punkty patrolu (puste obiekty w scenie)
    private int currentPoint = 0;     // aktualny punkt patrolu

    [Header("References")]
    public Transform player;          // obiekt gracza (przeciągnąć w Inspectorze)
    public Animator anim;             // animator przeciwnika

    [Header("Attack")]
    public int damage = 10;           // obrażenia zadawane graczowi
    public float attackCooldown = 1.5f; // czas pomiędzy atakami
    private float lastAttackTime;     // kiedy był ostatni atak

    void Update()
    {
        // Sprawdzamy odległość między przeciwnikiem a graczem
        float distance = Vector2.Distance(transform.position, player.position);

        // Jeśli gracz jest bardzo blisko → atak
        if (distance <= attackDistance)
        {
            Attack();
        }
        // Jeśli gracz jest w zasięgu widzenia → pościg
        else if (distance <= chaseDistance)
        {
            Chase();
        }
        // Jeśli gracz jest daleko → patrol
        else
        {
            Patrol();
        }
    }

    // ---------------- PATROL ----------------
    void Patrol()
    {
        // Włączamy animację ruchu
        anim.SetBool("IsMoving", true);

        // Aktualny punkt patrolu
        Transform target = patrolPoints[currentPoint];

        // Ruch w kierunku punktu patrolu
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Jeśli dotarliśmy do punktu → zmień punkt
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }

        // Obrót sprite’a w stronę ruchu
        Flip(target.position.x - transform.position.x);
    }

    // ---------------- CHASE ----------------
    void Chase()
    {
        anim.SetBool("IsMoving", true);

        // Ruch w stronę gracza
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );

        Flip(player.position.x - transform.position.x);
    }

    // ---------------- ATTACK ----------------
    void Attack()
    {
        // Cooldown – żeby wróg nie atakował co klatkę
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        // Trigger animacji ataku
        anim.SetTrigger("Attack");

        // Zadawanie obrażeń graczowi
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.TakeDamage(damage);
        }

        lastAttackTime = Time.time;
    }
    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    /// <param name="directionX"></param>
    // ---------------- FLIP ----------------
    void Flip(float directionX)
    {
        // Jeśli różnica jest bardzo mała – nie obracamy sprite’a
        if (Mathf.Abs(directionX) < 0.01f) return;

        // Obrót sprite’a w lewo / prawo
        transform.localScale = new Vector3(
            Mathf.Sign(directionX),
            1,
            1
        );
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PeopleController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private float dieDelay = 2f;
    [SerializeField] private float targetThreshold = 1f;
    [SerializeField] private bool hasPartner = false;
    [SerializeField] private PeopleController partner;
    [SerializeField] private PeopleController parent;
    [SerializeField] private int score = 30;
    [SerializeField] private int penalty = 60;
    [SerializeField] private float lookForPoliceRadius = 10f;
    private Transform target;

    private bool isDead = false;

    public void SetTarget(Transform target)
    {
        this.target = target;

        agent.SetDestination(target.position);
    }

    private void Update()
    {
        if (!target || isDead) return;

        if (Vector3.Distance(transform.position, target.position) < targetThreshold)
        {
            GameManager.instance.RemovePeople(this);
            Destroy(gameObject);
        }
    }

    private void TurnActive()
    {
        parent.GetComponent<PeopleController>().enabled = false;
        parent.GetComponent<NavMeshAgent>().enabled = false;

        GetComponent<PeopleController>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;

        SetTarget(GameManager.instance.GetRandomWaypoint());
    }

    public void GetHit()
    {
        if (isDead) return;

        if (hasPartner)
        {
            if (partner)
            {
                GameManager.instance.AddScore(score);
                partner.TurnActive();
            }
            else
                GameManager.instance.AddScore(penalty);
        }

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, lookForPoliceRadius, Vector3.up, 0f);
        foreach (RaycastHit hit in hits)
        {
            Police police = hit.collider.GetComponent<Police>();
            if (police)
            {
                GameManager.instance.EndGame();
            }
        }

        Die();
    }

    public void Die()
    {
        isDead = true;

        GameManager.instance.RemovePeople(this);

        GetComponent<NavMeshAgent>().enabled = false;

        if (animator)
            animator.SetTrigger("Die");

        Destroy(gameObject, dieDelay);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookForPoliceRadius);
    }
}

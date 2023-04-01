using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PeopleController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float targetThreshold = 1f;
    [SerializeField] private bool hasPartner = false;
    [SerializeField] private PeopleController partner;
    [SerializeField] private PeopleController parent;
    [SerializeField] private int score = 30;
    [SerializeField] private int penalty = 60;
    [SerializeField] private float lookForPoliceRadius = 10f;
    private Transform target;


    public void SetTarget(Transform target)
    {
        this.target = target;

        agent.SetDestination(target.position);
    }

    private void Update()
    {
        if (!target) return;

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
        else
        {
            GameManager.instance.EndGame();
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
        GameManager.instance.RemovePeople(this);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookForPoliceRadius);
    }
}

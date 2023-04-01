using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject[] peoplePrefabs;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private AnimationCurve peopleCountOverTime;

    [SerializeField] private List<PeopleController> people = new List<PeopleController>();
    private float timeInLevel = 0f;
    private int score = 0;
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Update()
    {
        timeInLevel += Time.deltaTime;

        int peopleCount = Mathf.RoundToInt(peopleCountOverTime.Evaluate(timeInLevel));
        if (peopleCount > people.Count)
            Spawn();
    }

    public void EndGame()
    {
        Debug.Log("Game Over");
    }

    private void SpawnPeople(int _amount)
    {
        for (int i = 0; i < _amount; i++)
        {
            Spawn();
        }
    }

    public Transform GetRandomWaypoint()
    {
        return waypoints[Random.Range(0, waypoints.Length)];
    }

    private void Spawn()
    {
        GameObject peoplePrefab = peoplePrefabs[Random.Range(0, peoplePrefabs.Length)];
        Transform spawnWaypoint = GetRandomWaypoint();
        Transform targetWaypoint = GetRandomWaypoint();

        while (targetWaypoint == spawnWaypoint)
        {
            targetWaypoint = GetRandomWaypoint();
        }

        PeopleController newPeople = Instantiate(peoplePrefab, spawnWaypoint.position, Quaternion.identity).GetComponent<PeopleController>();
        newPeople.SetTarget(targetWaypoint);
        people.Add(newPeople);
    }

    public void RemovePeople(PeopleController _people)
    {
        people.Remove(_people);
    }

    public void AddScore(int _score)
    {
        score += _score;
        scoreText.text = score.ToString();
    }
}

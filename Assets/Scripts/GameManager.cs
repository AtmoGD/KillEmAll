using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private PlayerController player;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject[] peoplePrefabs;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private AnimationCurve peopleCountOverTime;

    [SerializeField] private List<PeopleController> people = new List<PeopleController>();
    private float timeInLevel = 0f;
    private int score = 0;
    private bool isGameOver = false;
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
        if (isGameOver) return;

        timeInLevel += Time.deltaTime;

        int peopleCount = Mathf.RoundToInt(peopleCountOverTime.Evaluate(timeInLevel));
        if (peopleCount > people.Count)
            Spawn();
    }

    public void EndGame()
    {
        if (isGameOver) return;

        isGameOver = true;
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = score.ToString();
        player.EndGame();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        if (isGameOver) return;

        GameObject peoplePrefab = peoplePrefabs[Random.Range(0, peoplePrefabs.Length)];
        Transform spawnWaypoint = GetRandomWaypoint();
        Transform targetWaypoint = GetRandomWaypoint();

        while (targetWaypoint == spawnWaypoint)
        {
            targetWaypoint = GetRandomWaypoint();
        }

        PeopleController newPeople = Instantiate(peoplePrefab, spawnWaypoint.position, Quaternion.identity).GetComponent<PeopleController>();
        people.Add(newPeople);
        newPeople.SetTarget(targetWaypoint);
    }

    public void RemovePeople(PeopleController _people)
    {
        people.Remove(_people);
    }

    public void AddScore(int _score)
    {
        if (isGameOver) return;

        score += _score;
        scoreText.text = score.ToString();
    }
}

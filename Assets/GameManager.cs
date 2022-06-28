using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Snake snake;
    public static GameManager GM;
    [SerializeField] GameObject GameOverCanvas;
    [SerializeField] GameObject foodPrefab;
    [SerializeField] GameObject SpoiledfoodPrefab;
    public float xRange = 10.0f;
    public float yRange = 5.0f;
    [SerializeField] TMPro.TMP_Text scoreText;
    [SerializeField] int score = 0;
    public List<GameObject> spoiledMeats = new List<GameObject>();
    bool gameOver = false;
    public bool GameOver { get => gameOver; set => gameOver = value; }
    public int Score { get => score; set => score = value; }

    private void Awake()
    {
        if (GM != null)
        {
            Destroy(gameObject);
        }
        GM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        snake = FindObjectOfType<Snake>();
        SpawnFood();
    }

    public void SpawnFood()
    {
        scoreText.SetText($"Score : {score}");
        Vector3Int newPos = GetRandomPos();

        Instantiate(foodPrefab, newPos, foodPrefab.transform.rotation);
        if (score % 10 == 0 && score != 0)
        {
            SpawnSpoiledFood();
        }
    }

    private Vector3Int GetRandomPos()
    {
        int xPos = (int)(Random.Range(-xRange, xRange));
        int yPos = (int)(Random.Range(-yRange, yRange));

        Vector3Int tempPos = new Vector3Int(xPos, yPos, 0);
        if (spoiledMeats.Count > 0)
        {
            for (int i = 0; i < spoiledMeats.Count; i++)
            {
                if (tempPos == spoiledMeats[i].transform.position)
                {
                    tempPos = GetRandomPos();
                }
            }
        }
        if (snake.Segments.Count > 0)
        {
            for (int i = 0; i < snake.Segments.Count; i++)
            {
                if (tempPos == snake.Segments[i].transform.position)
                {
                    tempPos = GetRandomPos();
                }
            }
        }
        return tempPos;
    }

    public void EndGame()
    {
        gameOver = true;
        GameOverCanvas.SetActive(gameOver);
    }

    public void SpawnSpoiledFood()
    {
        Vector3Int newPos = GetRandomPos();
        Instantiate(SpoiledfoodPrefab, newPos, SpoiledfoodPrefab.transform.rotation);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] List<GameObject> segments = new List<GameObject>();
    [SerializeField] GameObject segmentPrefab;
    [SerializeField] GameObject tailPrefab;
    [SerializeField] Vector2Int moveDir;
    [SerializeField] float moveSpeed;
    [SerializeField] KeyCode upKey;
    [SerializeField] KeyCode downKey;
    [SerializeField] KeyCode leftKey;
    [SerializeField] KeyCode rightKey;
    Sprite tailSprite;
    float xRange = 11;
    float yRange = 7f;

    public List<GameObject> Segments { get => segments; set => segments = value; }

    // Start is called before the first frame update
    void Start()
    {
        tailSprite = tailPrefab.GetComponent<SpriteRenderer>().sprite;
        moveDir = Vector2Int.right;
        Time.timeScale = .05f;
        for (int i = 0; i < 3; i++)
        {
            if (i == 2)
            {
                CreateSegment(tailPrefab);
            }
            else
            {
                CreateSegment(segmentPrefab);
            }
        }
    }

    void Update()
    {
        if (!GameManager.GM.GameOver)
        {
            SnakeMove();
        }
    }

    private void SnakeMove()
    {
        if (Input.GetKeyDown(upKey))
        {
            moveDir = Vector2Int.up;
            transform.rotation = Quaternion.Euler(0, 0, SetRotation());
        }
        else if (Input.GetKeyDown(downKey))
        {
            moveDir = Vector2Int.down;
            transform.rotation = Quaternion.Euler(0, 0, SetRotation());
        }
        else if (Input.GetKeyDown(leftKey))
        {
            moveDir = Vector2Int.left;
            transform.rotation = Quaternion.Euler(0, 0, SetRotation());
        }
        else if (Input.GetKeyDown(rightKey))
        {
            moveDir = Vector2Int.right;
            transform.rotation = Quaternion.Euler(0, 0, SetRotation());
        }
    }

    private float SetRotation()
    {
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }

    void CreateSegment(GameObject SegmentToAdd)
    {
        var GO = Instantiate(SegmentToAdd, transform.position, Quaternion.identity);
        GO.transform.position = segments[segments.Count - 1].transform.position;
        segments.Add(GO);
    }

    private void RemoveTail()
    {
        GameObject segmentToRemove = segments[segments.Count - 1];
        segments.RemoveAt(segments.Count - 1);
        Destroy(segmentToRemove);
    }

    private void FixedUpdate()
    {
        if (!GameManager.GM.GameOver)
        {
            SegmentsFollow();
            SetBounds();
            transform.position += new Vector3(moveDir.x, moveDir.y, 0) * moveSpeed;
        }
    }

    private void SetBounds()
    {
        if (transform.position.x > xRange)
        {
            transform.position = new Vector3(-xRange, transform.position.y);
        }
        else if (transform.position.x < -xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y);
        }

        if (transform.position.y > yRange)
        {
            transform.position = new Vector3(transform.position.x, -yRange);
        }
        else if (transform.position.y < -yRange)
        {
            transform.position = new Vector3(transform.position.x, yRange);
        }
    }

    private void SegmentsFollow()
    {
        //there is a problem where first segment is on same place as head.
        //no idea why it happens.
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].transform.position = segments[i - 1].transform.position;
            segments[i].transform.rotation = segments[i - 1].transform.rotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Body"))
        {
            print("ded snek");
            GameManager.GM.EndGame();
        }
        if (collision.CompareTag("Food"))
        {
            print("snek food");
            Destroy(collision.gameObject);
            AddSegmentAndReplaceTail();
            GameManager.GM.Score++;
            GameManager.GM.SpawnFood();
        }
        if (collision.CompareTag("BadFood"))
        {
            print("snek food bad");
            Destroy(collision.gameObject);
            GameManager.GM.spoiledMeats.Remove(collision.gameObject);
            RemoveSegmentAndReplaceTail();
            GameManager.GM.Score--;
        }
    }

    public void AddSegmentAndReplaceTail()
    {
        RemoveTail();
        CreateSegment(segmentPrefab);
        CreateSegment(tailPrefab);
    }

    public void RemoveSegmentAndReplaceTail()
    {
        for (int i = 0; i < 2; i++)
        {
            RemoveTail();//remove last 2 segments
        }
        CreateSegment(tailPrefab);//readd a Tail
    }
}
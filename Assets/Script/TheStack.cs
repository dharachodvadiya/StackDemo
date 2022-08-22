using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour
{
	
    private const float BOUND_SIZE = 3.5f;
    private const float STACK_MOVING_SPEED = 5f;
    private const float ERROR_MARGIN = 0.1f;
    private GameObject[] theStack;
    Vector2 stackBounds = new Vector3(BOUND_SIZE, BOUND_SIZE);
    // Start is called before the first frame update
    int scoreCount = 0;
    int stackIndex = 0;
    int combo = 0;

    float tileTransition = 0;
    float tilespeed = 2.5f;
    float secondaryPosition;

    bool isMovingOnX = true;
    bool isGameOver = false;

    Vector3 desirePosition;
    Vector3 lastTilePosition;

    void Start()
    {
        theStack = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;
        }
        stackIndex = transform.childCount - 1;
        spawnTile();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(PlaceTile())
            {
                scoreCount++;
                spawnTile();
            }
            else
            {
                endGame();
            }
           
            
        }
        if(!isGameOver)
        {
            moveTile();
        }
       

        //move stack
        transform.position = Vector3.Lerp(transform.position, desirePosition, STACK_MOVING_SPEED * Time.deltaTime);
    }

    void spawnTile()
    {
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex--;
        if(stackIndex<0)
        {
            stackIndex = transform.childCount - 1;
        }

        desirePosition = (Vector3.down) * scoreCount;
        theStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
        theStack[stackIndex].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.HSVToRGB((scoreCount / 100f) % 1,0.5f, 1));
        theStack[stackIndex].SetActive(true);
        Camera.main.backgroundColor = Color.HSVToRGB((scoreCount / 100f) % 1, 0.5f, 1);
    }

    void moveTile()
    {
        tileTransition += Time.deltaTime * tilespeed;
        if(isMovingOnX)
        {
            theStack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUND_SIZE, scoreCount, secondaryPosition);
        }
        else
        {
            theStack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUND_SIZE);
        }
        
    }

    bool PlaceTile()
    {
        Transform t = theStack[stackIndex].transform;

        if(isMovingOnX)
        {
            float deltaX = lastTilePosition.x - t.position.x;

            if(Mathf.Abs(deltaX) > ERROR_MARGIN)
            {
                //cut the tile
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                    return false;

                float middle = lastTilePosition.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                float posX = middle - (lastTilePosition.x / 2);
                CreateRubble(
                    new Vector3((t.position.x > 0 )
                    ?t.position.x + (t.localScale.x/2)
                    : t.position.x - (t.localScale.x / 2)
                    , t.position.y
                    , t.position.z),
                    new Vector3(Mathf.Abs(deltaX),1,t.localScale.z)
                    );
                t.localPosition = new Vector3(posX, scoreCount, lastTilePosition.z);

            }
        }
        else
        {
            float deltaZ = lastTilePosition.z - t.position.z;

            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {
                //cut the tile
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0)
                    return false;

                float middle = lastTilePosition.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                float posZ = middle - (lastTilePosition.z / 2);
                CreateRubble(
                   new Vector3(t.position.x, 
                   t.position.y, 
                   (t.position.z > 0)
                   ? t.position.z + (t.localScale.z / 2)
                   : t.position.z - (t.localScale.z / 2)),
                   new Vector3(t.localScale.x, 1, Mathf.Abs(deltaZ))
                   );
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, posZ);
            }
        }
        secondaryPosition = (isMovingOnX) ? t.localPosition.x : t.localPosition.z;

        isMovingOnX = !isMovingOnX;
        return true;
    }

    void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();
        go.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.HSVToRGB((scoreCount / 100f) % 1, 0.5f, 1));
    }

    void endGame()
    {
        isGameOver = true;
        Debug.Log("end game");
        theStack[stackIndex].AddComponent<Rigidbody>();
    }
}

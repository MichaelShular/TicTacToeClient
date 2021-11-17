using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform[] boxPositions;
    [SerializeField] GameObject boxField;
    public GameObject[] allCurrentBoxState;
    
    public bool currentPlayerAction;
    public BoxStates roleInGame;

    private Vector3[] winList;

    // Start is called before the first frame update
    void Start()
    {
        winnerStatesCreation();
        allCurrentBoxState = new GameObject[boxPositions.Length];
        for (int i = 0; i < boxPositions.Length; i++)
        {
            GameObject tempGameObject = Instantiate(boxField);
            tempGameObject.transform.position = boxPositions[i].position;
            tempGameObject.GetComponent<boxFieldController>().currentBoxState = BoxStates.NONE;
            allCurrentBoxState[i] = tempGameObject;
        }
        roleInGame = BoxStates.NONE;
        currentPlayerAction = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void winnerStatesCreation()
    {
        winList = new Vector3[8];
        winList[0] = new Vector3(0, 1, 2);
        winList[1] = new Vector3(3, 4, 5);
        winList[2] = new Vector3(6, 7, 8);
        winList[3] = new Vector3(0, 3, 6);
        winList[4] = new Vector3(1, 4, 7);
        winList[5] = new Vector3(2, 5, 8);
        winList[6] = new Vector3(0, 4, 8);
        winList[7] = new Vector3(2, 4, 6);
    }
    public void checkIfThereIsWinner(BoxStates a)
    {
        //012,345,678,036,147,258,048,246

        for (int i = 0; i < winList.Length; i++)
        {
            if(allCurrentBoxState[Mathf.RoundToInt(winList[i].x)].GetComponent<boxFieldController>().currentBoxState == a &&
                allCurrentBoxState[Mathf.RoundToInt(winList[i].y)].GetComponent<boxFieldController>().currentBoxState == a &&
                allCurrentBoxState[Mathf.RoundToInt(winList[i].z)].GetComponent<boxFieldController>().currentBoxState == a)
            {
                Debug.Log("Winner");
            }
        }
    }

    public void SetWhichPlayer(BoxStates a)
    {
        roleInGame = a;
    }

}

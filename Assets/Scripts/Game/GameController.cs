using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform[] boxPositions;
    [SerializeField] GameObject boxField;
    public GameObject[] allCurrentBoxState;

    public bool currentPlayerAction;
    public BoxStates roleInGame;

    private Vector3[] winList;

    GameObject NetworkClient;

    public int currentTurn;
    public int lastPlayedBox;
    public bool matchOver;

    public GameObject Text;

    public List<int> movesMadeForReplay;

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
            tempGameObject.GetComponent<boxFieldController>().boxID = i;

            allCurrentBoxState[i] = tempGameObject;
        }
        roleInGame = BoxStates.NONE;
        currentPlayerAction = false;
        currentTurn = 0;
        NetworkClient = GameObject.Find("NetworkController");
        matchOver = false;
        movesMadeForReplay = new List<int>();
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
            if (allCurrentBoxState[Mathf.RoundToInt(winList[i].x)].GetComponent<boxFieldController>().currentBoxState == a &&
                allCurrentBoxState[Mathf.RoundToInt(winList[i].y)].GetComponent<boxFieldController>().currentBoxState == a &&
                allCurrentBoxState[Mathf.RoundToInt(winList[i].z)].GetComponent<boxFieldController>().currentBoxState == a)
            {
                Debug.Log("Winner");
                whoWon(a);
            }
        }
    }

    public void whoWon(BoxStates a)
    {
        NetworkClient.GetComponent<GameSystemController>().ChangeGameState(GameState.matchFinished);
        if (a == BoxStates.PLAYERONE)
        {
            Text.GetComponent<Text>().text = "Player One Wins";
        }
        else
        {
            Text.GetComponent<Text>().text = "Player Two Wins";
        }
        if (matchOver == true)
        {
            return;
        }
        else
        {
            NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.matchIsOver + "," + (int)a);
        }
        matchOver = true;
    }

    public void SetWhichPlayer(BoxStates a)
    {
        roleInGame = a;
    }

    public void SetWhichPlayerTurn(bool a)
    {
        currentPlayerAction = a;
    }

    public void setLastPlayedBox(int a)
    {
        lastPlayedBox = a;
    }
    public void setNextPlayerTurnToServer()
    {
        currentTurn++;
        currentPlayerAction = !currentPlayerAction;
        //GameObject.Find("GameUI").GetComponent<GameMessages>().displayMessageChat(lastPlayedBox.ToString());
        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacToeMove + "," + currentTurn + "," + lastPlayedBox);
    }

    public void gameReset()
    {
        foreach (GameObject box in allCurrentBoxState)
        {
            box.GetComponent<boxFieldController>().currentBoxState = BoxStates.NONE;

        }
        currentTurn = 0;
        currentPlayerAction = false;
        matchOver = false;
    }

    public void updateBoardState(int whichBox, int whichPlayer)
    {
        allCurrentBoxState[whichBox].GetComponent<boxFieldController>().currentBoxState = (BoxStates)whichPlayer;
    }

    public void setMovesForReplay(int a)
    {
        movesMadeForReplay.Add(a);
    }

    public void nextButtonPressed()
    {
        int[] temp = movesMadeForReplay.ToArray();
        if (currentTurn < temp.Length)
        {
            if (!currentPlayerAction)
            {
                roleInGame = BoxStates.PLAYERONE;
            }
            else
            {
                roleInGame = BoxStates.PLAYERTWO;
            }
            Debug.Log(currentTurn);
            Debug.Log(movesMadeForReplay.Count);

            allCurrentBoxState[temp[currentTurn]].GetComponent<boxFieldController>().currentBoxState = roleInGame;

            currentPlayerAction = !currentPlayerAction;
            currentTurn++;
        }

    }

    public void perButtonPressed()
    {
        int[] temp = movesMadeForReplay.ToArray();
        if (currentTurn > 0)
        {
            currentTurn--;
            allCurrentBoxState[temp[currentTurn]].GetComponent<boxFieldController>().currentBoxState = BoxStates.NONE;

            currentPlayerAction = !currentPlayerAction;
            
        }
    }
}

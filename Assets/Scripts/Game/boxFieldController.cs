using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Summary: Used to to control what will happen when a player clicks on a TictacToe box
public class boxFieldController : MonoBehaviour
{
    public PlayerStates currentBoxState;
    public GameController gameController;
    public int boxID;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameState").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentBoxState == PlayerStates.PLAYERONE)
        {
            this.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else if(currentBoxState == PlayerStates.PLAYERTWO)
        {
            this.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentBoxState = PlayerStates.NONE;
        }
    }

    private void OnMouseDown()
    {
        PlayerStates role = GameObject.Find("GameState").GetComponent<GameController>().roleInGame;
        bool yourTurn = GameObject.Find("GameState").GetComponent<GameController>().currentPlayerAction;

        if (PlayerStates.NONE == 0)
        {
            if (role == PlayerStates.PLAYERONE && !yourTurn)
            {
                currentBoxState = PlayerStates.PLAYERONE;                
                gameController.setLastPlayedBox(boxID);
                gameController.setNextPlayerTurnToServer();
                gameController.checkIfThereIsWinner(currentBoxState);

            }
            else if (role == PlayerStates.PLAYERTWO && yourTurn)
            {
                currentBoxState = PlayerStates.PLAYERTWO;            
                gameController.setLastPlayedBox(boxID);
                gameController.setNextPlayerTurnToServer();
                gameController.checkIfThereIsWinner(currentBoxState);
            }
        }
        //GameObject.Find("GameState").GetComponent<GameController>().currentPlayerAction = !GameObject.Find("GameState").GetComponent<GameController>().currentPlayerAction;
    }


}

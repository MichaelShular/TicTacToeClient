using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class boxFieldController : MonoBehaviour
{
    public BoxStates currentBoxState;
    public GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameState").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentBoxState == BoxStates.PLAYERONE)
        {
            this.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else if(currentBoxState == BoxStates.PLAYERTWO)
        {
            this.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentBoxState = BoxStates.NONE;
        }
    }

    private void OnMouseDown()
    {
        BoxStates role = GameObject.Find("GameState").GetComponent<GameController>().roleInGame;
        bool yourTurn = GameObject.Find("GameState").GetComponent<GameController>().currentPlayerAction;

        if (role == BoxStates.PLAYERONE && !yourTurn)
        {
            currentBoxState = BoxStates.PLAYERONE;
            gameController.checkIfThereIsWinner(currentBoxState);
            gameController.setNextPlayerTurnToServer();

        }
        else if(role == BoxStates.PLAYERTWO && yourTurn)
        {
            currentBoxState = BoxStates.PLAYERTWO;
            gameController.checkIfThereIsWinner(currentBoxState);
            gameController.setNextPlayerTurnToServer();

        }
        //GameObject.Find("GameState").GetComponent<GameController>().currentPlayerAction = !GameObject.Find("GameState").GetComponent<GameController>().currentPlayerAction;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class boxFieldController : MonoBehaviour
{
    public BoxStates currentBoxState;
    // Start is called before the first frame update
    void Start()
    {
        
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

    }

    private void OnMouseDown()
    {
        if (GameObject.Find("GameState").GetComponent<GameController>().currentPlayerAction)
        {
            currentBoxState = BoxStates.PLAYERONE;
        }
        else
        {
            currentBoxState = BoxStates.PLAYERTWO;
        }
        GameObject.Find("GameState").GetComponent<GameController>().currentPlayerAction = !GameObject.Find("GameState").GetComponent<GameController>().currentPlayerAction;
    }
}

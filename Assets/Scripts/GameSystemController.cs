using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemController : MonoBehaviour
{
    GameObject inputUsername, inputPassword, submitButton, toggleLogin, toggleCreate;
    GameObject NetworkClient;
    [SerializeField] private Canvas loginMenu, gameMenu, waiting, playingGame; 
    // Start is called before the first frame update
    void Start()
    {
        NetworkClient = GameObject.Find("NetworkController");
        GameObject[] allgameObjects = GameObject.FindGameObjectsWithTag("LoginMenuGameObjects");
        
        foreach(GameObject go in allgameObjects)
        {
            if(go.name == "Username")
            {
                inputUsername = go;
            }
            else if (go.name == "Password")
            {
                inputPassword = go;
            }
            else if (go.name == "Submit")
            {
                submitButton = go;
            }
            else if (go.name == "Login")
            {
                toggleLogin = go;
            }
            else if (go.name == "NewAccount")
            {
                toggleCreate = go;
            }
        }

        submitButton.GetComponent<Button>().onClick.AddListener(submitButtonPressed);
        toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChange);
        toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChange);

        ChangeGameState(GameState.LoginMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void submitButtonPressed() 
    {
        string n = inputUsername.GetComponent<InputField>().text;
        string p = inputPassword.GetComponent<InputField>().text;

        if (toggleLogin.GetComponent<Toggle>().isOn)
        {
            NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.Login + "," + n + "," + p);
        }
        else
        {
            NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.CreateAccount + "," + n + "," + p);
        }

    }
    public void ToggleLoginValueChange(bool newVal)
    {
        toggleCreate.GetComponent<Toggle>().SetIsOnWithoutNotify(!newVal);
    }
    public void ToggleCreateValueChange(bool newVal)
    {
        toggleLogin.GetComponent<Toggle>().SetIsOnWithoutNotify(!newVal);
    }
    
    public void ChangeGameState(int newState)
    {
        loginMenu.enabled = gameMenu.enabled = false; 
        switch (newState)
        {
            case 1:
                loginMenu.enabled = true;
                break;
            case 2:
                gameMenu.enabled = true;
                break;
            case 3:
                break;
            case 4:
                break;
            default:
                Debug.Log("Unknownstate");
                break;
        }
    }
}

public static class GameState
{
    public const int LoginMenu = 1;

    public const int GameMenu = 2;

    public const int WaitingForGame = 3;

    public const int PlayingTicTacToe = 4;
}

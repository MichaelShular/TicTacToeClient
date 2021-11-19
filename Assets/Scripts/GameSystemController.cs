using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemController : MonoBehaviour
{
    GameObject inputUsername, inputPassword, submitButton, toggleLogin, toggleCreate, observeGame, observeGameNumber, backToMenu, replayGameButton, replayGameID, backToMenuFromReplay;
    GameObject NetworkClient;
    [SerializeField] private Canvas loginMenu, gameMenu, waiting, chooseGameRoom, matchFinished, replayGame;
    GameObject findGame;
    // Start is called before the first frame update
    void Start()
    {
        NetworkClient = GameObject.Find("NetworkController");
        GameObject[] allgameObjects = GameObject.FindGameObjectsWithTag("MenuGameObjects");

        foreach (GameObject go in allgameObjects)
        {
            if (go.name == "Username")
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
            else if (go.name == "FindGame")
            {
                findGame = go;
            }
            else if (go.name == "ObserveGame")
            {
                observeGame = go;
            }
            else if (go.name == "ObserveGameNumber")
            {
                observeGameNumber = go;
            }
            else if (go.name == "BackToMenu")
            {
                backToMenu = go;
            }
            else if (go.name == "ReplayGame")
            {
                replayGameButton = go;
            }
            else if (go.name == "ReplayGameID")
            {
                replayGameID = go;
            }
            else if (go.name == "BackToGameMenu")
            {
                backToMenuFromReplay = go;
            }
        }

        backToMenuFromReplay.GetComponent<Button>().onClick.AddListener(backToMenuButton);
        backToMenu.GetComponent<Button>().onClick.AddListener(backToMenuButton);
        submitButton.GetComponent<Button>().onClick.AddListener(submitButtonPressed);
        toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChange);
        toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChange);
        findGame.GetComponent<Button>().onClick.AddListener(StartGameButton);
        observeGame.GetComponent<Button>().onClick.AddListener(observeAnotherGame);
        replayGameButton.GetComponent<Button>().onClick.AddListener(replayAnotherGame);
        ChangeGameState(GameState.LoginMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void backToMenuButton()
    {
        ChangeGameState(GameState.GameMenu);
        GameObject.Find("GameState").GetComponent<GameController>().gameReset();
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

    public void replayAnotherGame()
    {
        string n = replayGameID.GetComponent<InputField>().text;

        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.findReplay + "," + n);
        ChangeGameState(GameState.showAReplayOfgame);
    }
    public void observeAnotherGame()
    {
        string n = observeGameNumber.GetComponent<InputField>().text;

        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.lookForGameToWatch + "," + n);
        ChangeGameState(GameState.lookForGameToWatch);
    }
    public void StartGameButton()
    {
        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.StartLookingForPlayer + "");
        ChangeGameState(GameState.WaitingForGame);
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
        loginMenu.enabled = gameMenu.enabled = waiting.enabled = chooseGameRoom.enabled = matchFinished.enabled = replayGame.enabled = false; 
        switch (newState)
        {
            case 1:
                loginMenu.enabled = true;
                break;
            case 2:
                gameMenu.enabled = true;
                break;
            case 3:
                waiting.enabled = true;
                break;
            case 4:
                break;
            case 5:
                //chooseGameRoom.enabled = true;
                break;
            case 6:
                matchFinished.enabled = true;
                break;
            case 7:
                replayGame.enabled = true;
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

    public const int lookForGameToWatch = 5;

    public const int matchFinished = 6;
    
    public const int showAReplayOfgame = 7;

}

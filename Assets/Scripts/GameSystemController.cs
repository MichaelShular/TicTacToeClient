using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Used to control what canvas a player will see and functionality to most UI elements in
public class GameSystemController : MonoBehaviour
{
    //interactive UI elements
    GameObject inputUsername, inputPassword, submitButton, toggleLogin, toggleCreate, observeGame, observeGameNumber, backToMenu, replayGameButton, replayGameID, backToMenuFromReplay, findGame;

    [SerializeField] private Canvas loginMenu, gameMenu, waiting, chooseGameRoom, matchFinished, replayGame;

    GameObject NetworkClient;

    // Start is called before the first frame update
    void Start()
    {
        NetworkClient = GameObject.Find("NetworkController");

        SettingUIElements();
        AddingListeners();
        
        //scene player loads into
        ChangeUICanvas(GameState.LoginMenu);
    }

    // Update is called once per frame
    void Update()
    {
        //allow user to quit game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    //Used to variables to gameObjects in scene 
    private void SettingUIElements()
    {
        GameObject[] allgameObjects = GameObject.FindGameObjectsWithTag("MenuGameObjects");

        //Finding gameObjects in scene 
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
    }

    //Used to add listeners to interactive UI elements
    private void AddingListeners()
    {
        backToMenuFromReplay.GetComponent<Button>().onClick.AddListener(backToMenuButton);
        backToMenu.GetComponent<Button>().onClick.AddListener(backToMenuButton);
        submitButton.GetComponent<Button>().onClick.AddListener(submitButtonPressed);
        toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChange);
        toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChange);
        findGame.GetComponent<Button>().onClick.AddListener(StartGameButton);
        observeGame.GetComponent<Button>().onClick.AddListener(observeAnotherGame);
        replayGameButton.GetComponent<Button>().onClick.AddListener(replayAnotherGame);
    }

    //change state to GameMenu and reset game values 
    public void backToMenuButton()
    {
        ChangeUICanvas(GameState.GameMenu);
        GameObject.Find("GameState").GetComponent<GameController>().gameReset();
    }
    
    //submit account information in LoginMenu
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

    //find a replay of a game base on ID from input field 
    public void replayAnotherGame()
    {
        string n = replayGameID.GetComponent<InputField>().text;

        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.findReplay + "," + n);
        ChangeUICanvas(GameState.showAReplayOfgame);
    }

    //observe a game base on ID of a player currently in a game session
    public void observeAnotherGame()
    {
        string n = observeGameNumber.GetComponent<InputField>().text;

        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.lookForGameToWatch + "," + n);
        ChangeUICanvas(GameState.lookForGameToWatch);
    }

    //notify server play is look for oppenent and change state to WaitingForGame  
    public void StartGameButton()
    {
        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.StartLookingForPlayer + "");
        ChangeUICanvas(GameState.WaitingForGame);
    }

    //the two toggle control if the sever will creating a new account or load an exsiting account 
    public void ToggleLoginValueChange(bool newVal)
    {
        toggleCreate.GetComponent<Toggle>().SetIsOnWithoutNotify(!newVal);
    }
    public void ToggleCreateValueChange(bool newVal)
    {
        toggleLogin.GetComponent<Toggle>().SetIsOnWithoutNotify(!newVal);
    }

    //controls which canvas are visable based on state
    public void ChangeUICanvas(int newState)
    {
        //disable all canvas so multiple canvas arent active
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

//A class of signifiers to used to switch the canvas in game
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

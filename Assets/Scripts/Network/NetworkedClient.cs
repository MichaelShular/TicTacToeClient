using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

//Summary: Used to initialize connection to server, process messages for server and hold signifiers classes to determine the type of message 
public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;

    GameSystemController gameSystemManager;
    GameController gameController;
    GameMessages gameMessages;

    // Start is called before the first frame update
    void Start()
    {
        gameSystemManager = this.GetComponent<GameSystemController>();
        gameController = GameObject.Find("GameState").GetComponent<GameController>();
        gameMessages = GameObject.Find("GameUI").GetComponent<GameMessages>();
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SendMessageToHost("Hello from client");

        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }

    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.1.10", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);
                Debug.Log("hi");
            }
        }
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }

    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if(signifier == ServerToClientSignifiers.LoginResponses)
        {
            int loginResult = int.Parse(csv[1]);

            if(loginResult == LoginResponses.Success)
            {
                gameSystemManager.ChangeUICanvas(GameState.GameMenu);
            }
        }
        else if(signifier == ServerToClientSignifiers.GameSessionStarted)
        {
            gameSystemManager.ChangeUICanvas(GameState.PlayingTicTacToe);
        }
        else if(signifier == ServerToClientSignifiers.GameResponses)
        {
            int gameResult = int.Parse(csv[1]);

            if (gameResult == GameResponses.playerOne)
            {
                gameController.SetWhichPlayer(PlayerStates.PLAYERONE);
            }
            if (gameResult == GameResponses.playerTwo)
            {
                gameController.SetWhichPlayer(PlayerStates.PLAYERTWO);
            }
            if (gameResult == GameResponses.observer)
            {
                gameController.SetWhichPlayer(PlayerStates.NONE);
            }
        }
        else if (signifier == ServerToClientSignifiers.messagingAnotherPlayer)
        {
            gameMessages.displayMessageChat(csv[1]);
        }
        else if (signifier == ServerToClientSignifiers.OppnentTicTacToePlay)
        {
            gameController.currentTurn = int.Parse(csv[1]);
            Debug.Log(int.Parse(csv[2]));
            gameController.updateBoardState(int.Parse(csv[2]), int.Parse(csv[3]));
            gameController.currentPlayerAction = !gameController.currentPlayerAction;
        }
        else if (signifier == ServerToClientSignifiers.matchIsOver)
        {
            gameController.matchOver = true;
            gameController.whoWon((PlayerStates)int.Parse(csv[1]));            
        }
        else if (signifier == ServerToClientSignifiers.sendReplay)
        {
            for (int i = 1; i < (csv.Length -1); i++)
            {
                Debug.Log(csv[i]);
                gameController.movesMadeForReplay.Add(int.Parse(csv[i]));
            }
        }
        else if (signifier == ServerToClientSignifiers.lookforGameResponses && int.Parse(csv[1]) == lookforGameResponses.Success)
        {
            for (int i = 2; i < (csv.Length - 1); i++)
            {
                gameController.updateObserverView(int.Parse(csv[i]));
            }
        }

    }

    public bool IsConnected()
    {
        return isConnected;
    }
}
public static class ClientToServerSignifiers
{
    public const int Login = 1;

    public const int CreateAccount = 2;

    public const int StartLookingForPlayer = 3;

    public const int TicTacToeMove = 4;

    public const int messagingAnotherPlayer = 5;

    public const int lookForGameToWatch = 6;

    public const int matchIsOver = 7;

    public const int findReplay = 8;

}

public static class ServerToClientSignifiers
{
    public const int LoginResponses = 1;
    
    public const int GameSessionStarted = 2;

    public const int OppnentTicTacToePlay = 3;

    public const int GameResponses = 4;

    public const int messagingAnotherPlayer = 5;

    public const int lookforGameResponses = 6;

    public const int matchIsOver = 7;

    public const int sendReplay = 8;

}

public static class LoginResponses
{
    public const int Success = 1;

    public const int FailureNameInUse = 2;

    public const int FailureNameNotFound = 3;

    public const int FailureIncorrectPassword = 4;
}

public static class lookforGameResponses
{
    public const int Success = 1;

    public const int Fail = 2;

}
public static class GameResponses
{
    public const int observer = 0;

    public const int playerOne = 1;

    public const int playerTwo = 2;

}

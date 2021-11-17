using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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
                gameSystemManager.ChangeGameState(GameState.GameMenu);
            }
        }
        else if(signifier == ServerToClientSignifiers.GameSessionStarted)
        {
            gameSystemManager.ChangeGameState(GameState.PlayingTicTacToe);
        }
        else if(signifier == ServerToClientSignifiers.GameResponses)
        {
            int gameResult = int.Parse(csv[1]);

            if (gameResult == GameResponses.playerOne)
            {
                gameController.SetWhichPlayer(BoxStates.PLAYERONE);
            }
            if (gameResult == GameResponses.playerTwo)
            {
                gameController.SetWhichPlayer(BoxStates.PLAYERTWO);
            }
            if (gameResult == GameResponses.observer)
            {
                gameController.SetWhichPlayer(BoxStates.NONE);
            }
        }
        else if (signifier == ServerToClientSignifiers.messagingAnotherPlayer)
        {
            gameMessages.displayMessageChat(csv[1]);
        }
        else if (signifier == ServerToClientSignifiers.OppnentTicTacToePlay)
        {
            gameController.currentPlayerAction = !gameController.currentPlayerAction;
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
}

public static class ServerToClientSignifiers
{
    public const int LoginResponses = 1;
    
    public const int GameSessionStarted = 2;

    public const int OppnentTicTacToePlay = 3;

    public const int GameResponses = 4;

    public const int messagingAnotherPlayer = 5;

    public const int lookforGameResponses = 6;
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
    public const int playerOne = 1;

    public const int playerTwo = 2;

    public const int observer = 3;

}

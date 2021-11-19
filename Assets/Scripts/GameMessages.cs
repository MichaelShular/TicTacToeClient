using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Summary: Used to display chat messages and send messages prefixed messages to other player  
public class GameMessages : MonoBehaviour
{
    public GameObject Text;
    public GameObject contentPanel;
    GameObject NetworkClient;

    // Start is called before the first frame update
    void Start()
    {
        NetworkClient = GameObject.Find("NetworkController");
    }
    public void prefixedMessageOne()
    {
        whatMessage("Hi");
    }

    public void prefixedMessageTwo()
    {
        whatMessage("GG");
    }

    public void prefixedMessageThree()
    {
        whatMessage("What");
    }

    private void whatMessage(string a)
    {
        GameObject temp = Instantiate(Text, contentPanel.transform);
        temp.transform.SetParent(contentPanel.transform);
        temp.GetComponent<Text>().text = a;
        NetworkClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.messagingAnotherPlayer + "," + a);  
    }
    public void displayMessageChat(string a)
    {
        GameObject temp = Instantiate(Text, contentPanel.transform);
        temp.transform.SetParent(contentPanel.transform); 
        temp.GetComponent<Text>().text = a;
    }
}

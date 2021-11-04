using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform[] boxPositions;
    [SerializeField] GameObject boxField;
    public GameObject[] allCurrentBoxState;
    public bool currentPlayerAction;

    // Start is called before the first frame update
    void Start()
    {
        allCurrentBoxState = new GameObject[boxPositions.Length];
        for (int i = 0; i < boxPositions.Length; i++)
        {
            GameObject tempGameObject = Instantiate(boxField);
            tempGameObject.transform.position = boxPositions[i].position;
            tempGameObject.GetComponent<boxFieldController>().currentBoxState = BoxStates.NONE;
            allCurrentBoxState[i] = tempGameObject;
        }

        currentPlayerAction = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(allCurrentBoxState[0].GetComponent<boxFieldController>().currentBoxState == BoxStates.PLAYERONE)
        {
            Debug.Log("1");
        }
    }
}

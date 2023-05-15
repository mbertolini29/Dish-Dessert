using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] public GameObject[] levels;
    public int currentLevelIndex = 0;

    public GameObject tutorial;
    public Text tutorialText; //Drag the dish and drop it in the shown place!

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void LoadLevel()
    {
        //si es 0 el tutorial recien arrancaria..
        if (currentLevelIndex == 0)
        {
            tutorial.SetActive(true);
        }
        else
        {
            tutorial.SetActive(false);
        }
    }
}

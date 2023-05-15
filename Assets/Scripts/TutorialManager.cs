using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public GameObject[] popUps;
    public int popUpIndex;

    [Header("Tutorial")]
    public bool tutorial = true;
    public int numTutorial = 0;
    public bool tutorial1 = false;
    public bool tutorial2 = false;

    public Linedraw linePrefab;

    public GameObject dish1;
    public GameObject targetCell1;

    public GameObject dish2;
    public GameObject targetCell2;

    //animator
    public GameObject hand;
    public Animator animator;
    public Text tutorialText; //Drag the dish and drop it in the shown place!

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void Start()
    {
        animator = hand.gameObject.GetComponent<Animator>();
        hand.gameObject.GetComponent<Image>().enabled = false;
        tutorial = true;
    }

    private void Update()
    {
        if(numTutorial == 0)
        {
            numTutorial++;

            //desactivar la animacion.
            animator.SetBool("Handle 1", false);

            dish2 = GameObject.Find("Dish 2");
            dish2.gameObject.GetComponent<CapsuleCollider>().enabled = false;

            LoadTutorial();

            popUpIndex++;
        }
        else if(numTutorial == 2)
        {
            numTutorial++;

            //desactivar la animacion.
            animator.SetBool("Handle 1", false);
            animator.SetBool("Handle 2", false);

            LoadTutorial();

            popUpIndex++;
        }
    }

    public void DesactiveHandle()
    {
        hand.gameObject.GetComponent<Image>().enabled = false;
        linePrefab.gameObject.GetComponent<LineRenderer>().enabled = false;
    }

    public void ActiveHandle()
    {
        hand.gameObject.GetComponent<Image>().enabled = true;
        linePrefab.gameObject.GetComponent<LineRenderer>().enabled = true;
    }

    public int ReturnNumTutorial()
    {
        return popUpIndex;
    }

    public void LoadTutorial()
    {
        if(popUpIndex == 0)
        {
            //newLine = GameObject.Find("Drawline");
            dish1 = GameObject.Find("Dish 1");

            //hay que bloquear todas las celdas. y solo librar la targetCell1
            targetCell1 = GameObject.Find("Dish[1][2]");
            targetCell1.GetComponentInChildren<Cell>().isBusy = false;

            StartCoroutine(OneTutorial());
        }
        else if(popUpIndex == 1)
        {
            //aca tiene q mover el segundo plato a la posicion q indica, al lado del plato 0
            dish2 = GameObject.Find("Dish 2");
            dish2.gameObject.GetComponent<CapsuleCollider>().enabled = true;

            //hay que bloquear todas las celdas. y solo librar la targetCell1
            targetCell2 = GameObject.Find("Dish[2][2]");
            targetCell2.GetComponentInChildren<Cell>().isBusy = false;
            //targetCell1

            //change text
            tutorialText.text = "Complete the cupcake";

            StartCoroutine(TwoTutorial());
        }
        else if (popUpIndex == 2)
        {
            tutorial = false;
            //change text
            tutorialText.text = "Now dishes may come with 2 types of dessert in one dish";
            DesactiveHandle();
        }
    }

    IEnumerator OneTutorial()
    {
        yield return new WaitForSeconds(1f);

        ActiveHandle();

        //linePrefab.AssignTarget(dish1.transform.position, targetCell1.transform);
        linePrefab.AssignTarget(new Vector3(-1.465f, -5.86f, -0.6f), new Vector3(-1.33f, 2.04f, -1.5f));

        //activar la animacion.
        animator.SetBool("Handle 1", true);
    }

    IEnumerator TwoTutorial()
    {
        yield return new WaitForSeconds(0.5f);

        ActiveHandle();

        //linePrefab.AssignTarget(dish1.transform.position, targetCell1.transform);
        linePrefab.AssignTarget(new Vector3(0.75f, -5.86f, -0.6f), new Vector3(0.9f, 2.04f, -1.5f));

        //activar la animacion.
        animator.SetBool("Handle 2", true);
    }
}

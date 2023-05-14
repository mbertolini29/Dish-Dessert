using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Estrella : MonoBehaviour
{
    public float speed;

    public Transform target;
    //public Transform intial;
    public GameObject starPrefab;
    public Camera cam;

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    public void StartStarMove(Vector3 _intial, Action onComplete)
    {
        //Vector3 intialPos = cam.ScreenToWorldPoint(new Vector3(intial.position.x, intial.position.y, cam.transform.position.z * -1));
        Vector3 targetPos = cam.ScreenToWorldPoint(new Vector3(target.position.x, target.position.y, cam.transform.position.z * -1));
        //targetPos = new Vector3(targetPos.x - 0.365f, targetPos.y - 0.4f, targetPos.z); //cam 9.9
        targetPos = new Vector3(targetPos.x - 0.55f, targetPos.y - 0.6f, targetPos.z); //cam 12
        //targetPos = new Vector3(1.613f, 8.193f, targetPos.z); //cam 12 (2.7, 8.7)
        GameObject estrellaVisual = GameObject.Find("Estrella");

        //estrellaVisual.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        StartCoroutine(MoveStar(estrellaVisual.transform, _intial, targetPos, onComplete));
        StartCoroutine(RotateStar(estrellaVisual.transform));
    }

    IEnumerator MoveStar(Transform obj, Vector3 startPos, Vector3 endPos, Action onComplete)
    {
        yield return new WaitForSeconds(0.5f);

        float time = 0;

        while(time < 1)
        {
            time += speed * Time.deltaTime;
            obj.position = Vector3.Lerp(startPos, endPos, time);
            yield return new WaitForEndOfFrame();
        }

        //yield return null;
        onComplete.Invoke();
    }

    IEnumerator RotateStar(Transform obj)
    {
        obj.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        yield return new WaitForSeconds(0.5f);

        float time = 0;

        while(time < 2)
        {
            time += speed * Time.deltaTime;
            obj.localScale = Vector3.Lerp(obj.localScale, Vector3.zero, time/240); //480
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    //IEnumerator EstrellaTime(GameObject gameobjectDish, float duration)
    //{
    //    yield return new WaitForSeconds(0.5f);

    //    //GameObject estrellaVisual = Instantiate(estrella);
    //    GameObject estrellaVisual = GameObject.Find("Estrella");
    //    starPos = GameObject.Find("ImageScore").transform;
    //    //estrellaVisual.transform.position = previousSelected.transform.position;
    //    estrellaVisual.transform.localPosition = previousSelected.transform.position;

    //    GameObject background2 = GameObject.Find("Background 2");
    //    estrellaVisual.transform.parent = background2.transform;

    //    float currentTime = 0.0f;
    //    do
    //    {
    //        estrellaVisual.transform.localPosition = Vector3.Lerp(estrellaVisual.transform.localPosition,
    //                                                             starPos.position,
    //                                                             currentTime / duration);

    //        //estrellaVisual.transform.localScale = Vector3.Lerp(estrellaVisual.transform.localScale,
    //        //                                                    Vector3.zero, currentTime / duration);

    //        currentTime += Time.deltaTime;
    //        yield return null;
    //    } while (currentTime <= duration);
    //}
}

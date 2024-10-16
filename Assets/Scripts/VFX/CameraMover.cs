using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(Shake(0.3f, 0.05f, 2.5f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator Shake(float duration, float shakeRate, float intensity)
    {
        Vector3 initialPosition = mainCamera.transform.position;
        float initialTime = Time.time;
        float timer = initialTime;

            
        while (timer < duration)
        {
            Vector3 randomDelta = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));

            mainCamera.transform.position = initialPosition + randomDelta;
            yield return new WaitForSeconds(shakeRate);

            Debug.Log(timer);

            timer = Time.time - initialTime;
        }

        mainCamera.transform.position = initialPosition;

        yield return null;
    }




    void Boweeew(float duration, float maxFOV)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;


public class SceneFader : MonoBehaviour
{
    public Image img;

    private void Start()
    {
        StartCoroutine(FaceIn());
    }

    IEnumerator FaceIn()
    {
        float duration = 3f; // thoi gian fade in dai hon de muot
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.SmoothStep(1f, 0f, t / duration);// muot hon
            img.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        img.color = new Color(0f, 0f, 0f, 0f); // dam bao alpha = 0
        img.gameObject.SetActive(false);
    }

}

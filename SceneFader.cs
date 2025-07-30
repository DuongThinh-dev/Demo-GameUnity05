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

    IEnumerator FaceIn ()
    {
        float t = 1f;

        while (t > 0f)
        {
            t -= Time.deltaTime;
            img.color = new Color(0f, 0f, 0f, t);
            yield return 0;
        }

        img.gameObject.SetActive(false);
    }
}

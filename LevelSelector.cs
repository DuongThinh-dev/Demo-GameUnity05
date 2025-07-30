using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons;

    private void Awake()
    {
        // Tu dong lay tat ca cac Button la con truc tiep
        int childCount = transform.childCount;
        levelButtons = new Button[childCount];

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Button btn = child.GetComponent<Button>();
            if (btn != null)
            {
                levelButtons[i] = btn;
            }
            else
            {
                Debug.LogWarning($"Child {child.name} does not have a Button component.");
            }
        }
    }

    private void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levedReached", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
                levelButtons[i].interactable = false;
        }

        PlayerPrefs.DeleteAll();
    }
}

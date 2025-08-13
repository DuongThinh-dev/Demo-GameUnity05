using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvaManager : MonoBehaviour
{
    public GameObject scrollViewTower;
    public GameObject scrollViewTrap;
    public GameObject scrollViewDino;

    private void Start()
    {
        scrollViewTower.SetActive(false);
        scrollViewTrap.SetActive(false);
        scrollViewDino.SetActive(false);
    }

    public void TurnOn(int number)
    {
        switch (number)
        {
            case 0:
                scrollViewTower.SetActive(true);
                break;
            case 1:
                scrollViewTrap.SetActive(true);
                break;
            case 2: 
                scrollViewDino.SetActive(true);
                break;
        }
    }
    
    public void TurnOff(int number)
    {
        switch(number)
        {
            case 0:
                scrollViewTower.SetActive(false);
                break;
            case 1:
                scrollViewTrap.SetActive(false);
                break;
            case 2:
                scrollViewDino.SetActive(false);
                break;

        }
    }
}

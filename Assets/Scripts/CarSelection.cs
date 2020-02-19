using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelection : MonoBehaviour
{
    public GameObject[] selectableCars;
    public int carSelectionNumber;

    // Start is called before the first frame update
    void Start ()
    {
        carSelectionNumber = 0;

        ActivateCar (carSelectionNumber);
    }

    // Activate relevant car
    private void ActivateCar (int i)
    {
        // Deactivate all player cars in the player list
        foreach (GameObject selectableCar in selectableCars)
        {
            selectableCar.SetActive (false);
        }
        // Activate relevant player car
        selectableCars[i].SetActive (true);
    }

    // Move to the next car in the list
    public void NextCar ()
    {
        carSelectionNumber += 1;
        if (carSelectionNumber >= selectableCars.Length)
        {
            carSelectionNumber = 0;
        }
        ActivateCar (carSelectionNumber);
    }

    // Move to the previous car in the list
    public void PreviousCar ()
    {
        carSelectionNumber -= 1;
        if (carSelectionNumber < 0)
        {
            carSelectionNumber = selectableCars.Length - 1;
        }
        ActivateCar(carSelectionNumber);
    }
}
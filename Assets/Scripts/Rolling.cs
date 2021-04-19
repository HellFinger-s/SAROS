using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Rolling : MonoBehaviour
{
    public int Changer;//переменная характеризующая сторону прокрутки | variable of scroll direction
    public int IndexOfActiveElement;//индекс активного элемента | index of active element
    public GameObject Keeper;//объект, хранящий директории | 
    List<InputField> StartDirFields;//коллекция вводных полей для начальных директорий | collection of input fields for initial directories
    List<InputField> EndDirFields;//коллекция вводных полей для конечных директорий | collection of input fields for target directories
    List<InputField> InpLangFields;//коллекция вводных полей для исходных языков | collection of input fields for source languages
    List<InputField> OutpLangFields;//коллекция вводных полей для итоговых языков | collection of input fields for target languages

    public void RollButtonPressed()
    {
        StartDirFields = Keeper.GetComponent<Keeping>().ListOfStartDirectoriesFieldsChildren;
        EndDirFields = Keeper.GetComponent<Keeping>().ListOfEndDirectoriesFieldsChildren;
        InpLangFields = Keeper.GetComponent<Keeping>().ListOfInputLanguagesChildren;
        OutpLangFields = Keeper.GetComponent<Keeping>().ListOfOutputLanguagesChildren;
        if (StartDirFields.Count > 1)
        {
            IndexOfActiveElement = FindActiveElement(StartDirFields);
            StartDirFields[IndexOfActiveElement].gameObject.SetActive(false);
            EndDirFields[IndexOfActiveElement].gameObject.SetActive(false);
            InpLangFields[IndexOfActiveElement].gameObject.SetActive(false);
            OutpLangFields[IndexOfActiveElement].gameObject.SetActive(false);
            IndexOfActiveElement += Changer;
            if (IndexOfActiveElement >= StartDirFields.Count)
            {
                IndexOfActiveElement -= StartDirFields.Count;
            }
            if (IndexOfActiveElement < 0)
            {
                IndexOfActiveElement = StartDirFields.Count + IndexOfActiveElement;
            }
            StartDirFields[IndexOfActiveElement].gameObject.SetActive(true);
            EndDirFields[IndexOfActiveElement].gameObject.SetActive(true);
            InpLangFields[IndexOfActiveElement].gameObject.SetActive(true);
            OutpLangFields[IndexOfActiveElement].gameObject.SetActive(true);
        }
    }
    public int FindActiveElement(List<InputField> ListToCheck)
    {
        int IndexOfActiveElement = 0;
        foreach (InputField child in ListToCheck)
        {
            if (child.gameObject.activeInHierarchy)
            {
                IndexOfActiveElement = ListToCheck.IndexOf(child);
            }
        }
        return IndexOfActiveElement;
    }
}

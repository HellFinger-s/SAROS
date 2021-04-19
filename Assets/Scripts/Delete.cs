using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Delete : MonoBehaviour
{
    public GameObject Keeper;//объект хранитель коллекций | object keeper of collections
    public GameObject Scroll;//кнопка прокрутки | roll button
    public void DeleteButtonPressed()
    {
        List<InputField> StartDirFields;//коллекция вводных полей для начальных директорий | collection of input fields for initial directories
        List<InputField> EndDirFields;//коллекция вводных полей для конечных директорий | collection of input fields for target directories
        List<InputField> InpLangFields;//коллекция вводных полей для исходных языков | collection of input fields for source languages
        List<InputField> OutpLangFields;//коллекция вводных полей для итоговых языков | collection of input fields for target languages
        StartDirFields = Keeper.GetComponent<Keeping>().ListOfStartDirectoriesFieldsChildren;
        EndDirFields = Keeper.GetComponent<Keeping>().ListOfEndDirectoriesFieldsChildren;
        InpLangFields = Keeper.GetComponent<Keeping>().ListOfInputLanguagesChildren;
        OutpLangFields = Keeper.GetComponent<Keeping>().ListOfOutputLanguagesChildren;
        if (StartDirFields.Count > 1)
        {
            int IndexOfActiveElement = Scroll.GetComponent<Rolling>().IndexOfActiveElement;
            if(IndexOfActiveElement == 0)
            {
                Destroy(Keeper.GetComponent<Keeping>().ListOfStartDirectoriesFieldsChildren[IndexOfActiveElement].gameObject);
                Destroy(Keeper.GetComponent<Keeping>().ListOfEndDirectoriesFieldsChildren[IndexOfActiveElement].gameObject);
                Destroy(Keeper.GetComponent<Keeping>().ListOfInputLanguagesChildren[IndexOfActiveElement].gameObject);
                Destroy(Keeper.GetComponent<Keeping>().ListOfOutputLanguagesChildren[IndexOfActiveElement].gameObject);
                Keeper.GetComponent<Keeping>().ListOfStartDirectoriesFieldsChildren.RemoveAt(IndexOfActiveElement);
                Keeper.GetComponent<Keeping>().ListOfEndDirectoriesFieldsChildren.RemoveAt(IndexOfActiveElement);
                Keeper.GetComponent<Keeping>().ListOfInputLanguagesChildren.RemoveAt(IndexOfActiveElement);
                Keeper.GetComponent<Keeping>().ListOfOutputLanguagesChildren.RemoveAt(IndexOfActiveElement);
                Keeper.GetComponent<Keeping>().ListOfStartDirectoriesFieldsChildren[IndexOfActiveElement].gameObject.SetActive(true);
                Keeper.GetComponent<Keeping>().ListOfEndDirectoriesFieldsChildren[IndexOfActiveElement].gameObject.SetActive(true);
                Keeper.GetComponent<Keeping>().ListOfInputLanguagesChildren[IndexOfActiveElement].gameObject.SetActive(true);
                Keeper.GetComponent<Keeping>().ListOfOutputLanguagesChildren[IndexOfActiveElement].gameObject.SetActive(true);
            }
            else
            {
                Destroy(Keeper.GetComponent<Keeping>().ListOfStartDirectoriesFieldsChildren[IndexOfActiveElement].gameObject);
                Destroy(Keeper.GetComponent<Keeping>().ListOfEndDirectoriesFieldsChildren[IndexOfActiveElement].gameObject);
                Destroy(Keeper.GetComponent<Keeping>().ListOfInputLanguagesChildren[IndexOfActiveElement].gameObject);
                Destroy(Keeper.GetComponent<Keeping>().ListOfOutputLanguagesChildren[IndexOfActiveElement].gameObject);
                Keeper.GetComponent<Keeping>().ListOfStartDirectoriesFieldsChildren.RemoveAt(IndexOfActiveElement);
                Keeper.GetComponent<Keeping>().ListOfEndDirectoriesFieldsChildren.RemoveAt(IndexOfActiveElement);
                Keeper.GetComponent<Keeping>().ListOfInputLanguagesChildren.RemoveAt(IndexOfActiveElement);
                Keeper.GetComponent<Keeping>().ListOfOutputLanguagesChildren.RemoveAt(IndexOfActiveElement);
                Keeper.GetComponent<Keeping>().ListOfStartDirectoriesFieldsChildren[IndexOfActiveElement-1].gameObject.SetActive(true);
                Keeper.GetComponent<Keeping>().ListOfEndDirectoriesFieldsChildren[IndexOfActiveElement-1].gameObject.SetActive(true);
                Keeper.GetComponent<Keeping>().ListOfInputLanguagesChildren[IndexOfActiveElement-1].gameObject.SetActive(true);
                Keeper.GetComponent<Keeping>().ListOfOutputLanguagesChildren[IndexOfActiveElement-1].gameObject.SetActive(true);
                Scroll.GetComponent<Rolling>().IndexOfActiveElement -= 1;
            }
        }
    }
}

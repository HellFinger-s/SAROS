using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Create : MonoBehaviour
{
    public InputField StartDirectoryField;//префаб поля ввода | input field prefab
    public InputField EndDirectoryField;
    public InputField FromLanguageField;
    public InputField OutputLanguageField;
    public GameObject StartDirectoryParent;//родительский объект, хранящий поля ввода | parent gameObject which keeps input fileds
    public GameObject EndDirectoryParent;
    public GameObject FromLanguageParent;
    public GameObject OutputLanguageParent;
    public GameObject Keeper;//объект хранитель коллекций | object keeper of collections

    public void AddPressed()
    {
        InputField ChildStartDirectory = Instantiate(StartDirectoryField,StartDirectoryParent.transform);
        ChildStartDirectory.gameObject.SetActive(false);
        Keeper.GetComponent<Keeping>().ListOfStartDirectoriesFieldsChildren.Add(ChildStartDirectory);
        ChildStartDirectory.transform.SetParent(StartDirectoryParent.transform);
        InputField ChildEndDirectory = Instantiate(EndDirectoryField, EndDirectoryParent.transform);
        ChildEndDirectory.gameObject.SetActive(false);
        Keeper.GetComponent<Keeping>().ListOfEndDirectoriesFieldsChildren.Add(ChildEndDirectory);
        ChildEndDirectory.transform.SetParent(EndDirectoryParent.transform);
        InputField ChildFromLanguage = Instantiate(FromLanguageField, FromLanguageParent.transform);
        ChildFromLanguage.gameObject.SetActive(false);
        Keeper.GetComponent<Keeping>().ListOfInputLanguagesChildren.Add(ChildFromLanguage);
        ChildFromLanguage.transform.SetParent(FromLanguageParent.transform);
        InputField ChildOutputLanguage = Instantiate(OutputLanguageField, OutputLanguageParent.transform);
        ChildOutputLanguage.gameObject.SetActive(false);
        Keeper.GetComponent<Keeping>().ListOfOutputLanguagesChildren.Add(ChildOutputLanguage);
        ChildOutputLanguage.transform.SetParent(OutputLanguageParent.transform);
    }
}

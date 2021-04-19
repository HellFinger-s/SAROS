using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
    public class Keeping : MonoBehaviour
    {
        public List<InputField> ListOfStartDirectoriesFieldsChildren;//коллекция вводных полей для начальных директорий | collection of input fields for initial directories
        public List<InputField> ListOfEndDirectoriesFieldsChildren;//коллекция вводных полей для конечных директорий | collection of input fields for target directories
        public List<InputField> ListOfInputLanguagesChildren;//коллекция вводных полей для исходных языков | collection of input fields for source languages
        public List<InputField> ListOfOutputLanguagesChildren;//коллекция вводных полей для итоговых языков | collection of input fields for target languages
        public InputField StartDirFirst;//первое вводное поле с начальной директорией | first input field with initial directory
        public InputField EndDirFirst;//первое вводное поле с конечной директорией | first input field with target directory
        public InputField InLangFirst;//первое вводное поле с исходным языков | first input field with source language
        public InputField OutLangFirst;//первое вводное поле с итоговым языком | first input field with target language

        public void Start()
        {
            ListOfStartDirectoriesFieldsChildren.Add(StartDirFirst);
            ListOfEndDirectoriesFieldsChildren.Add(EndDirFirst);
            ListOfInputLanguagesChildren.Add(InLangFirst);
            ListOfOutputLanguagesChildren.Add(OutLangFirst);
        }
    }
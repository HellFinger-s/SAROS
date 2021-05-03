using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
namespace VariableFinder
{
    public class Finder : MonoBehaviour
    {
        public List<string> MainFind(List<string> TextFromFile, string FromLanguage)//главный метод, вызываемый из другого класса | main method, called from another class
        {
            List<string> VariableNames;//коллекция с именами переменными | collection with variable names
            if(FromLanguage.IndexOf("#") != -1)
            {
                FromLanguage = FromLanguage.Substring(0, FromLanguage.IndexOf("#")) + FromLanguage.Substring(FromLanguage.IndexOf("#") + 1) + "SHARP";
            }
            string MethodName = "Finder" + FromLanguage.ToUpper();//формируем имя метода | forming the method name
            MethodInfo Finder = this.GetType().GetMethod(MethodName);
            VariableNames = (List<string>)Finder.Invoke(this, new object[] { TextFromFile });//вызов метода | call method
            return VariableNames;
        }


        //методы поиска имен переменных | methods for finding variable names

        public List<string> FinderPYTHON(List<string> TextFromFile)
        {
            List<string> VariableNames = new List<string>();
            //код заполняющий VariableNames именами переменных на языке python
            //code that fills Variable Names with variable names in python
            return VariableNames;
        }

        public List<string> FinderCSHARP(List<string> TextFromFile)
        {
            List<string> VariableNames = new List<string>();
            //код заполняющий VariableNames именами переменных на языке c#
            //code that fills Variable Names with variable names in c#
            return VariableNames;
        }
        
    }
}

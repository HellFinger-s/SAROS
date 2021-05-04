using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using VariableFinder;
public class ButtonPressed : MonoBehaviour
{
    public GameObject Keeper;//объект хранитель коллекций | object keeper of collections
    public Text SuccessText;//объект, содержащий успешный текст | object with success text
    public void Press()//метод, который вызывается при нажатии кнопки начать в интерфейсе | method that is called when the start button is clicked in the interface
    {
        List<InputField> StartDirectories = Keeper.GetComponent<Keeping>().ListOfStartDirectoriesFieldsChildren;//коллекция вводных полей для начальных директорий | collection of input fields for initial directories
        List<InputField> EndDirectories = Keeper.GetComponent<Keeping>().ListOfEndDirectoriesFieldsChildren;//коллекция вводных полей для конечных директорий | collection of input fields for target directories
        List<InputField> InputLanguages = Keeper.GetComponent<Keeping>().ListOfInputLanguagesChildren;//коллекция вводных полей для исходных языков | collection of input fields for source languages
        List<InputField> OutputLanguages = Keeper.GetComponent<Keeping>().ListOfOutputLanguagesChildren;//коллекция вводных полей для итоговых языков | collection of input fields for target languages
        List<string> VariableNames = new List<string> { };//коллекция имен переменных | collection of variable names
        List<string> TextFromFile = new List<string>();//коллекция строк из начального файла | collection of lines of file from initial directory
        var FinderObject = new Finder();

        //ниже основной цикл программы | below the main loop of programm
        for (int Number = 0; Number < StartDirectories.Count; Number++)
        {
            SuccessText.gameObject.SetActive(true);
            
            string FromLanguage = InputLanguages[Number].text.ToLower();
            string OutputLanguage = OutputLanguages[Number].text.ToLower();
            string InputPath = StartDirectories[Number].text;
            string OutputPath = EndDirectories[Number].text;
            TextFromFile = CommentChangerAndReader(InputPath, FromLanguage, OutputLanguage, SuccessText);
            TextFromFile = ChangeEOL(TextFromFile, FromLanguage, OutputLanguage);
            VariableNames = FinderObject.MainFind(TextFromFile, FromLanguage);//ищем имена переменных | find variable names
            TextFromFile = SpecialSymbolsChanger(VariableNames, TextFromFile, FromLanguage, OutputLanguage);
            WriteToFile(TextFromFile, OutputPath);
            SuccessText.gameObject.SetActive(true);
            SuccessText.text = "Готово " + (Number+1).ToString() + "/" + StartDirectories.Count.ToString();

        }
        StartCoroutine(Co_WaitForSeconds(2));
    }



    //ниже расположены методы, которые используется в основном цикле | below are the methods that are used in the main loop



    //метод, производящий замену специальных символов перед именами переменных
    //method that changes special symbols before variable names

    public List<string> SpecialSymbolsChanger(List<string> VarNames, List<string> TextFromFile, string FromLanguage, string OutputLanguage)
    {
        string buffer = "";
        Dictionary<string, string> SpecialSymbol = new Dictionary<string, string>
        { };
        foreach (string line in File.ReadAllLines(@"SpecialSymbols.txt"))// заполняем словарь специальных символов | filling in the dictionary of special characters
        {
            SpecialSymbol.Add(line.Substring(0, line.IndexOf(" ")), line.Substring(line.IndexOf(" ") + 1));
        }
        if (SpecialSymbol[FromLanguage] != SpecialSymbol[OutputLanguage])
        {
            for (int NumberOfLine = 0; NumberOfLine < TextFromFile.Count; NumberOfLine++)
            {
                for (int VarNameNumber = 0; VarNameNumber < VarNames.Count; VarNameNumber++)
                {
                    buffer = TextFromFile[NumberOfLine];
                    try
                    {
                        if (IndexWithoutQuotesFromStart(TextFromFile[NumberOfLine], VarNames[VarNameNumber], "") != -1)// в строке находится переменная | variable in line                         
                        {
                            while (IndexWithoutQuotesFromStart(buffer, VarNames[VarNameNumber], "") != -1)
                            {
                                if (SpecialSymbol[FromLanguage] != "")
                                {
                                    buffer = TextFromFile[NumberOfLine].Substring(0, TextFromFile[NumberOfLine].IndexOf(VarNames[VarNameNumber]) - SpecialSymbol[FromLanguage].Length) + TextFromFile[NumberOfLine].Substring(TextFromFile[NumberOfLine].IndexOf(VarNames[VarNameNumber]));// замена специального символа | changing special symbol
                                }
                                TextFromFile[NumberOfLine] = TextFromFile[NumberOfLine].Substring(0, (TextFromFile[NumberOfLine].Length - buffer.Length) + IndexWithoutQuotesFromStart(buffer, VarNames[VarNameNumber], VarNames[VarNameNumber])) + SpecialSymbol[OutputLanguage] + TextFromFile[NumberOfLine].Substring((TextFromFile[NumberOfLine].Length - buffer.Length) + IndexWithoutQuotesFromStart(buffer, VarNames[VarNameNumber], VarNames[VarNameNumber]));
                                buffer = buffer.Substring(IndexWithoutQuotesFromStart(buffer, VarNames[VarNameNumber], "") + VarNames[VarNameNumber].Length);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
        return TextFromFile;
    }


    //метод, записывающий коллекцию в файл
    //method that write collection in file

    public void WriteToFile(List<string> TextFromFile, string OutputPath)
    {
        string[] WriteToFile = new string[TextFromFile.Count];
        for (int i = 0; i < TextFromFile.Count; i++)
        {
            WriteToFile[i] = Convert.ToString(TextFromFile[i]);
        }
        File.WriteAllLines(OutputPath, WriteToFile);
    }

    //метод, заменяющий комментарии и записывающий строки из файла в коллекцию
    //method that read text to collection and change comment symbols

    public List<string> CommentChangerAndReader(string InputPath, string FromLanguage, string OutputLanguage, Text text)
    {
        List <string> TextFromFile = new List<string>();
        int CommentIndex = -1;
        int MultilineStartCommentIndex = -1;
        int MultilineEndCommentIndex = -1;
        Dictionary<string, string> Comment = new Dictionary<string, string>
        { };
        //Добавление символов комментирования в словарь по такому типу: язык символ
        foreach (string line in File.ReadAllLines("Comment.txt"))
        {
            Comment.Add(line.Substring(0, line.IndexOf(" ")), line.Substring(line.IndexOf(" ") + 1));
        }
        Dictionary<string, string> MultilineStartComment = new Dictionary<string, string>
        { };
        foreach (string line in File.ReadAllLines("MultilineStartComment.txt"))
        {
            MultilineStartComment.Add(line.Substring(0, line.IndexOf(" ")), line.Substring(line.IndexOf(" ") + 1));
        }
        Dictionary<string, string> MultilineEndComment = new Dictionary<string, string>
        { };
        foreach (string line in File.ReadAllLines("MultilineEndComment.txt"))
        {
            MultilineEndComment.Add(line.Substring(0, line.IndexOf(" ")), line.Substring(line.IndexOf(" ") + 1));
        }
        //Конец добавления
        bool IsOpen = false;
        foreach (string line in File.ReadAllLines(InputPath))
        {
            string buffer = line;
            if (Comment[FromLanguage] != Comment[OutputLanguage])
            {
                CommentIndex = IndexWithoutQuotesFromStart(buffer, Comment[FromLanguage], Comment[FromLanguage]);
            }
            if (MultilineStartComment[FromLanguage] != MultilineStartComment[OutputLanguage])
            {
                MultilineStartCommentIndex = IndexWithoutQuotesFromStart(buffer, MultilineStartComment[FromLanguage], MultilineStartComment[FromLanguage]);
            }
            if (MultilineEndComment[FromLanguage] != MultilineEndComment[OutputLanguage])
            {
                MultilineEndCommentIndex = IndexWithoutQuotesFromEnd(buffer, MultilineEndComment[FromLanguage], MultilineEndComment[FromLanguage]);
            }
            if (MultilineStartCommentIndex == MultilineEndCommentIndex)//многострочные комментарии одинаковые
            {
                MultilineEndCommentIndex = -1;
            }
            if (MultilineEndCommentIndex == -1 && IsOpen)
            {
                MultilineEndCommentIndex = MultilineStartCommentIndex;
                MultilineStartCommentIndex = -1;
            }
            if (CommentIndex != -1)
            {
                buffer = buffer.Substring(0, CommentIndex) + Comment[OutputLanguage] + buffer.Substring(CommentIndex + Comment[FromLanguage].Length);
            }
            if (MultilineStartCommentIndex != -1 && IsOpen == false)
            {
                buffer = buffer.Substring(0, MultilineStartCommentIndex) + MultilineStartComment[OutputLanguage] + buffer.Substring(MultilineStartCommentIndex + MultilineStartComment[FromLanguage].Length);
                IsOpen = true;
            }
            if (MultilineEndCommentIndex != -1 && IsOpen == true)
            {
                MultilineEndCommentIndex = IndexWithoutQuotesFromEnd(buffer, MultilineEndComment[FromLanguage], MultilineEndComment[FromLanguage]);
                buffer = buffer.Substring(0, MultilineEndCommentIndex) + MultilineEndComment[OutputLanguage] + buffer.Substring(MultilineEndCommentIndex + MultilineEndComment[FromLanguage].Length);
                IsOpen = false;
            }
            TextFromFile.Add(buffer);
        }
        return TextFromFile;
    }


    //метод, производящий замену символов окончания строки
    //method that change end of line symbols

    public List<string> ChangeEOL(List<string> TextFromFile, string FromLanguage, string OutputLanguage)
    {
        List<string> BannedCollocationsFromLanguage = new List<string>();//словосочетания, после которых не ставится символ завершения строки | collocations that are not followed by a line-ending character
        List<string> BannedCollocationsOutputLanguage = new List<string>();
        string FileName = FromLanguage.ToLower() + "Rules.txt";
        string buffer = "";
        bool IsBannedCollocationOfInputLanguage = false;
        bool IsBannedCollocationOfOutputLanguage = false;
        bool isOpen = false;
        //заполняем запрещенные словосочетания для обоих языков | fill in the forbidden phrases for both languages
        foreach (string line in File.ReadAllLines(FileName))
        {
            BannedCollocationsFromLanguage.Add(line);
        }
        FileName = OutputLanguage.ToLower() + "Rules.txt";
        foreach (string line in File.ReadAllLines(FileName))
        {
            BannedCollocationsOutputLanguage.Add(line);
        }
        int MinimalIndex = -1;
        Dictionary<string, string> Comment = new Dictionary<string, string>
        { };
        //заполняем однострочные комментарии | filling in one-line comments
        foreach (string line in File.ReadAllLines("Comment.txt"))
        {
            Comment.Add(line.Substring(0, line.IndexOf(" ")), line.Substring(line.IndexOf(" ") + 1));
        }
        Dictionary<string, string> MultilineStartComment = new Dictionary<string, string>
        { };
        //заполняем многострочные стартовые комментарии | filling in multi-line start comments
        foreach (string line in File.ReadAllLines("MultilineStartComment.txt"))
        {
            MultilineStartComment.Add(line.Substring(0, line.IndexOf(" ")), line.Substring(line.IndexOf(" ") + 1));
        }
        Dictionary<string, string> MultilineEndComment = new Dictionary<string, string>
        { };
        //заполняем многострочные конечные комментарии | filling in multi-line end comments
        foreach (string line in File.ReadAllLines("MultilineEndComment.txt"))
        {
            MultilineEndComment.Add(line.Substring(0, line.IndexOf(" ")), line.Substring(line.IndexOf(" ") + 1));
        }
        Dictionary<string, string> EndOfLineSymbol = new Dictionary<string, string>
        { };
        //заполняем символы завершения строки | filling in end of line characters
        foreach (string line in File.ReadLines("EndOfLineSymbol.txt"))
        {
            EndOfLineSymbol.Add(line.Substring(0, line.IndexOf(" ")), line.Substring(line.IndexOf(" ") + 1));
        }
        Dictionary<string, string> StartBlocks = new Dictionary<string, string>()
        { };
        //заполняем символы начала блока | fill in the characters of the beginning of the block
        foreach (string line in File.ReadLines("StartBlocks.txt"))
        {
            StartBlocks.Add(line.Substring(0, line.IndexOf(" ")), line.Substring(line.IndexOf(" ") + 1));
        }
        Dictionary<string, string> EndBlocks = new Dictionary<string, string>()
        { };
        //заполняем символы окончания блока | fill in the characters of the ending of the block
        foreach (string line in File.ReadLines("EndBlocks.txt"))
        {
            EndBlocks.Add(line.Substring(0, line.IndexOf(" ")), line.Substring(line.IndexOf(" ") + 1));
        }
        Dictionary<string, string> TabSymbol = new Dictionary<string, string>()
        { };
        //заполняем символы табуляции | fill in the tab characters
        foreach (string line in File.ReadLines("TabSymbols.txt"))
        {
            TabSymbol.Add(line.Substring(0, line.IndexOf("___")), line.Substring(line.IndexOf("___") + 3));
        }
        if (EndOfLineSymbol[FromLanguage] != EndOfLineSymbol[OutputLanguage])
        {
            for (int LineNumber = 0; LineNumber < TextFromFile.Count; LineNumber++)
            {
                //ищем тип первого комментария в строке | looking for the type of the first comment in the line
                MinimalIndex = MinimalValue(IndexWithoutQuotesFromStart(TextFromFile[LineNumber], Comment[OutputLanguage], Comment[OutputLanguage]),
                                            IndexWithoutQuotesFromStart(TextFromFile[LineNumber], MultilineStartComment[OutputLanguage], MultilineStartComment[OutputLanguage]),
                                            IndexWithoutQuotesFromStart(TextFromFile[LineNumber], MultilineEndComment[OutputLanguage], MultilineEndComment[OutputLanguage]));
                if (EndOfLineSymbol[FromLanguage] == "")
                {
                    for (int SymbolNumber = 0; SymbolNumber < TextFromFile[LineNumber].Length; SymbolNumber++)
                    {
                        //копируем строку без пробелов и символов табуляции в buffer | copy the string without spaces and tabs to buffer
                        if (TextFromFile[LineNumber][SymbolNumber].ToString() != " " && TextFromFile[LineNumber][SymbolNumber].ToString() != TabSymbol[FromLanguage])
                        {
                            buffer += TextFromFile[LineNumber][SymbolNumber];
                        }
                    }
                    //либо строка пустая, либо начинается с символа комментирования | either the string is empty, or it starts with a comment character
                    if (buffer == "" || IndexWithoutQuotesFromStart(buffer, Comment[OutputLanguage], Comment[OutputLanguage]) == 0 
                        || IndexWithoutQuotesFromStart(buffer, MultilineStartComment[OutputLanguage], Comment[OutputLanguage]) == 0 
                        || IndexWithoutQuotesFromStart(buffer, MultilineEndComment[OutputLanguage], Comment[OutputLanguage]) == 0)
                    {
                        IsBannedCollocationOfInputLanguage = true;
                        IsBannedCollocationOfOutputLanguage = true;
                    }
                    else
                    {
                        foreach (string CollocationOutputLanguage in BannedCollocationsOutputLanguage)
                        {
                            if (TrueIndex(TextFromFile[LineNumber], CollocationOutputLanguage, MinimalIndex) != -1)
                            {
                                IsBannedCollocationOfOutputLanguage = true;
                                break;
                            }
                        }
                    }
                    if (!IsBannedCollocationOfOutputLanguage)
                    {
                        if (MinimalIndex == -1)//строка не содержит комментариев | line does not contain comments
                        {
                            TextFromFile[LineNumber] = TextFromFile[LineNumber] + EndOfLineSymbol[OutputLanguage];
                        }
                        else
                        {
                            TextFromFile[LineNumber] = TextFromFile[LineNumber].Substring(0, MinimalIndex) + EndOfLineSymbol[OutputLanguage] + TextFromFile[LineNumber].Substring(MinimalIndex + EndOfLineSymbol[OutputLanguage].Length - 1);
                        }
                    }
                } 
                else
                {
                    if (IndexOfEOL(TextFromFile[LineNumber], EndOfLineSymbol[FromLanguage], Comment[OutputLanguage], MultilineStartComment[OutputLanguage], MultilineEndComment[OutputLanguage], isOpen) != -1)//в строке присутствует символ завершения строки | end of line character in line
                    {
                        if(IndexWithoutQuotesFromStart(TextFromFile[LineNumber], MultilineStartComment[OutputLanguage], Comment[OutputLanguage]) != -1 && !isOpen)
                        {
                            isOpen = true;
                        }
                        if (IndexWithoutQuotesFromStart(TextFromFile[LineNumber], MultilineEndComment[OutputLanguage], Comment[OutputLanguage]) != -1 && isOpen)
                        {
                            isOpen = false;
                        }
                        for (int SymbolNumber = 0; SymbolNumber < TextFromFile[LineNumber].Length; SymbolNumber++)
                        {
                            if (TextFromFile[LineNumber][SymbolNumber].ToString() != " " && TextFromFile[LineNumber][SymbolNumber].ToString() != TabSymbol[FromLanguage])
                            {
                                buffer += TextFromFile[LineNumber][SymbolNumber];
                            }
                        }
                        if (buffer == "" || IndexWithoutQuotesFromStart(buffer, Comment[OutputLanguage], Comment[OutputLanguage]) == 0 
                            || IndexWithoutQuotesFromStart(buffer, MultilineStartComment[OutputLanguage], Comment[OutputLanguage]) == 0 
                            || IndexWithoutQuotesFromStart(buffer, MultilineEndComment[OutputLanguage], Comment[OutputLanguage]) == 0)
                        {
                            IsBannedCollocationOfInputLanguage = true;
                            IsBannedCollocationOfOutputLanguage = true;
                        }
                        else
                        {
                            foreach (string CollocationInputLanguage in BannedCollocationsFromLanguage)
                            {
                                if (TrueIndex(TextFromFile[LineNumber], CollocationInputLanguage, MinimalIndex) != -1)
                                {
                                    IsBannedCollocationOfInputLanguage = true;
                                    break;
                                }
                            }
                            foreach (string CollocationOutputLanguage in BannedCollocationsOutputLanguage)
                            {
                                if (TrueIndex(TextFromFile[LineNumber], CollocationOutputLanguage, MinimalIndex) != -1)
                                {
                                    IsBannedCollocationOfOutputLanguage = true;
                                    break;
                                }
                            }
                        }
                        if (IsBannedCollocationOfInputLanguage)
                        {
                            if (!IsBannedCollocationOfOutputLanguage)
                            {
                                if (MinimalIndex == -1)//в строке нет символов комментирования | no comments characters in line
                                {
                                    TextFromFile[LineNumber] = TextFromFile[LineNumber].Substring(0, TextFromFile[LineNumber].Length - EndOfLineSymbol[FromLanguage].Length) + EndOfLineSymbol[OutputLanguage];
                                }
                                else
                                {
                                    TextFromFile[LineNumber] = TextFromFile[LineNumber].Substring(0, MinimalIndex) + EndOfLineSymbol[OutputLanguage] + TextFromFile[LineNumber].Substring(MinimalIndex + EndOfLineSymbol[OutputLanguage].Length - 1);
                                }
                            }
                        }
                        else
                        {
                            if (IsBannedCollocationOfOutputLanguage)
                            {
                                TextFromFile[LineNumber] = TextFromFile[LineNumber].Substring(0, IndexOfEOL(TextFromFile[LineNumber], EndOfLineSymbol[FromLanguage], Comment[OutputLanguage], MultilineStartComment[OutputLanguage], MultilineEndComment[OutputLanguage], isOpen)) + TextFromFile[LineNumber].Substring(IndexOfEOL(TextFromFile[LineNumber], EndOfLineSymbol[FromLanguage], Comment[OutputLanguage], MultilineStartComment[OutputLanguage], MultilineEndComment[OutputLanguage], isOpen) + EndOfLineSymbol[FromLanguage].Length);
                            }
                            else
                            {
                                if (IndexOfEOL(TextFromFile[LineNumber], EndOfLineSymbol[FromLanguage], Comment[OutputLanguage], MultilineStartComment[OutputLanguage], MultilineEndComment[OutputLanguage], isOpen) + EndOfLineSymbol[OutputLanguage].Length + 1 == TextFromFile[LineNumber].Length)
                                {
                                    TextFromFile[LineNumber] = TextFromFile[LineNumber].Substring(0, IndexOfEOL(TextFromFile[LineNumber], EndOfLineSymbol[FromLanguage], Comment[OutputLanguage], MultilineStartComment[OutputLanguage], MultilineEndComment[OutputLanguage], isOpen)) + EndOfLineSymbol[OutputLanguage] + TextFromFile[LineNumber].Substring(IndexOfEOL(TextFromFile[LineNumber], EndOfLineSymbol[FromLanguage], Comment[OutputLanguage], MultilineStartComment[OutputLanguage], MultilineEndComment[OutputLanguage], isOpen) + EndOfLineSymbol[OutputLanguage].Length + 1);
                                }
                                else
                                {
                                    TextFromFile[LineNumber] = TextFromFile[LineNumber].Substring(0, IndexOfEOL(TextFromFile[LineNumber], EndOfLineSymbol[FromLanguage], Comment[OutputLanguage], MultilineStartComment[OutputLanguage], MultilineEndComment[OutputLanguage], isOpen)) + EndOfLineSymbol[OutputLanguage] + TextFromFile[LineNumber].Substring(IndexOfEOL(TextFromFile[LineNumber], EndOfLineSymbol[FromLanguage], Comment[OutputLanguage], MultilineStartComment[OutputLanguage], MultilineEndComment[OutputLanguage], isOpen) + EndOfLineSymbol[FromLanguage].Length);
                                }
                            }
                        }
                    }

                }
                IsBannedCollocationOfInputLanguage = false;
                IsBannedCollocationOfOutputLanguage = false;
                buffer = "";
            }
        }
        return TextFromFile;
    }


    //Корутина для удаления элемента с текстом
    //courutine for delete

    private IEnumerator Co_WaitForSeconds(float value)
    {
        yield return new WaitForSeconds(value);
        SuccessText.gameObject.SetActive(false);
    }

    //метод, проверяющий наличие слова, после которого в строке не ставится символ завершения строки | method that checks for a word that is not followed by a line ending character

    public int TrueIndex(string where, string what, int commentIndex = -1)
    {
        int index = -1;
        if (where.IndexOf(what) != -1)
        {
            for (int i = 0; i <= where.Length; i++)
            {
                index = where.IndexOf(what);
                if(index+what.Length == where.Length || !Char.IsLetter(where[index+what.Length]))
                {
                    break;
                }
                index = -1;
                where.Substring(index + what.Length);
            }
        }
        if(commentIndex != -1 && index > commentIndex)
        {
            index = -1;
        }
        return index;
    }

    //метод ищущий индекс первого вхождения элемента без кавычек
    //method that find index of first occurence without quotes

    public int IndexWithoutQuotesFromStart(string where, string what, string comment)
    {
        int index = -1;
        string QuotesNow = "";
        int StartIndex = 0;
        int EndIndex = where.Length + 1;
        if (comment.IndexOf("'") != -1)//Если комментарий содержит ' как в python
        {
            for (int SymbolNumber = 0; SymbolNumber < where.Length; SymbolNumber++)
            {
                if (where.IndexOf(what) != -1)
                {
                    index = where.IndexOf(what);
                    if (where[SymbolNumber] == '"')
                    {
                        if (QuotesNow == "")
                        {
                            QuotesNow = where[SymbolNumber].ToString();
                            StartIndex = SymbolNumber;
                        }
                        else
                        {
                            if (QuotesNow == where[SymbolNumber].ToString())
                            {
                                EndIndex = SymbolNumber;
                                if (index < StartIndex || index > EndIndex)
                                {
                                    break;
                                }
                                QuotesNow = "";
                                where = where.Substring(0, index) + new string('f', what.Length) + where.Substring(index + what.Length);
                                index = -1;
                            }

                        }
                    }
                }
            }
        }
        else
        {
            if (comment.IndexOf('"') != -1)
            {
                for (int SymbolNumber = 0; SymbolNumber < where.Length; SymbolNumber++)
                {
                    if (where.IndexOf(what) != -1)
                    {
                        index = where.IndexOf(what);
                        if (where[SymbolNumber].ToString() == "'")
                        {
                            if (QuotesNow == "")
                            {
                                QuotesNow = where[SymbolNumber].ToString();
                                StartIndex = SymbolNumber;
                            }
                            else
                            {
                                if (QuotesNow == where[SymbolNumber].ToString())
                                {
                                    EndIndex = SymbolNumber;
                                    if (index < StartIndex || index > EndIndex)
                                    {
                                        break;
                                    }
                                    QuotesNow = "";
                                    where = where.Substring(0, index) + new string('f', what.Length) + where.Substring(index + what.Length);
                                    index = -1;
                                }

                            }
                        }
                    }
                }
            }
            else
            {
                for (int SymbolNumber = 0; SymbolNumber < where.Length; SymbolNumber++)
                {
                    if (where.IndexOf(what) != -1)
                    {
                        index = where.IndexOf(what);
                        if (where[SymbolNumber] == '"' || where[SymbolNumber].ToString() == "'")
                        {
                            if (QuotesNow == "")
                            {
                                QuotesNow = where[SymbolNumber].ToString();
                                StartIndex = SymbolNumber;
                            }
                            else
                            {
                                if (QuotesNow == where[SymbolNumber].ToString())
                                {
                                    EndIndex = SymbolNumber;
                                    if (index < StartIndex || index > EndIndex)
                                    {
                                        break;
                                    }
                                    QuotesNow = "";
                                    where = where.Substring(0, index) + new string('f', what.Length) + where.Substring(index + what.Length);
                                    index = -1;
                                }

                            }
                        }
                    }
                }
            }
        }
        return index;
    }

    //метод ищущий индекс последнего вхождения элемента без кавычек
    //method that find index of last occurence without quotes

    public int IndexWithoutQuotesFromEnd(string where, string what, string comment)
    {
        int index = -1;
        string QuotesNow = "";
        int StartIndex = 0;
        int EndIndex = where.Length + 1;
        if (comment.IndexOf("'") != -1)
        {
            for (int SymbolNumber = where.Length - 1; SymbolNumber >= 0; SymbolNumber--)
            {
                if (where.LastIndexOf(what) != -1)
                {
                    index = where.LastIndexOf(what);
                    if (where[SymbolNumber] == '"')
                    {
                        if (QuotesNow == "")
                        {
                            QuotesNow = where[SymbolNumber].ToString();
                            StartIndex = SymbolNumber;
                        }
                        else
                        {
                            if (QuotesNow == where[SymbolNumber].ToString())
                            {
                                EndIndex = SymbolNumber;
                                if (index > StartIndex || index < EndIndex)
                                {
                                    break;
                                }
                                QuotesNow = "";
                                where = where.Substring(0, index) + new string('f', what.Length) + where.Substring(index + what.Length);
                                index = -1;
                            }

                        }
                    }
                }
            }
        }
        else
        {
            if (comment.IndexOf('"') != -1)
            {
                for (int SymbolNumber = where.Length - 1; SymbolNumber >= 0; SymbolNumber--)
                {
                    if (where.LastIndexOf(what) != -1)
                    {
                        index = where.LastIndexOf(what);
                        if (where[SymbolNumber].ToString() == "'")
                        {
                            if (QuotesNow == "")
                            {
                                QuotesNow = where[SymbolNumber].ToString();
                                StartIndex = SymbolNumber;
                            }
                            else
                            {
                                if (QuotesNow == where[SymbolNumber].ToString())
                                {
                                    EndIndex = SymbolNumber;
                                    if (index > StartIndex || index < EndIndex)
                                    {
                                        break;
                                    }
                                    QuotesNow = "";
                                    where = where.Substring(0, index) + new string('f', what.Length) + where.Substring(index + what.Length);
                                    index = -1;
                                }

                            }
                        }
                    }
                }
            }
            else
            {
                for (int SymbolNumber = where.Length - 1; SymbolNumber >= 0; SymbolNumber--)
                {
                    if (where.LastIndexOf(what) != -1)
                    {
                        index = where.LastIndexOf(what);
                        if (where[SymbolNumber].ToString() == "'" || where[SymbolNumber] == '"')
                        {
                            if (QuotesNow == "")
                            {
                                QuotesNow = where[SymbolNumber].ToString();
                                StartIndex = SymbolNumber;
                            }
                            else
                            {
                                if (QuotesNow == where[SymbolNumber].ToString())
                                {
                                    EndIndex = SymbolNumber;
                                    if (index > StartIndex || index < EndIndex)
                                    {
                                        break;
                                    }
                                    QuotesNow = "";
                                    where = where.Substring(0, index) + new string('f', what.Length) + where.Substring(index + what.Length);
                                    index = -1;
                                }

                            }
                        }
                    }
                }
            }
        }
        return index;
    }

    //метод, который ищет позицию символа завершения строки
    //method that searches for the position of the line-ending character

    public int IndexOfEOL(string where, string what, string oneLineComment, string multiLineStartComment, string multiLineEndComment, bool isOpen)
    {
        int index = -1;
        string buffer = "";
        if (isOpen)
        {
            index = MinimalValue(IndexWithoutQuotesFromStart(where, multiLineStartComment, oneLineComment), IndexWithoutQuotesFromStart(where, oneLineComment, oneLineComment), -1);
        }
        else
        {
            if(IndexWithoutQuotesFromStart(where, multiLineEndComment, oneLineComment) != -1)
            {
                buffer = buffer.Substring(IndexWithoutQuotesFromStart(where, multiLineEndComment, oneLineComment));
                index = where.Length - buffer.Length;
                if (buffer.Length != 0)
                {
                    if (IndexWithoutQuotesFromStart(buffer, oneLineComment, oneLineComment) != -1)
                    {
                        index += IndexWithoutQuotesFromStart(buffer, oneLineComment, oneLineComment);
                    }
                }
            }
            else
            {
                if (IndexWithoutQuotesFromStart(where, oneLineComment, oneLineComment) != -1)
                {

                    index = IndexWithoutQuotesFromStart(where, oneLineComment, oneLineComment) - 1;
                }
                else
                {
                    index = where.Length - 1;
                }
            }
        }
        return index;
    }

    //метод ищущий минимальное значение среди n чисел, где n <= 3
    //method that find minimal value from n numbers, where n <= 3

    public int MinimalValue(int first, int second, int third)
    {
        int minimal = 0;
        List<int> old = new List<int>() { first, second, third };
        List<int> buffer = new List<int>();
        for(int number = 0; number < old.Count; number ++)
        {
            if(old[number] != -1)
            {
                buffer.Add(old[number]);
            }
        }
        if(buffer.Count == 3)
        {
            if (buffer[0] > buffer[1])
            {
                minimal = buffer[1];
            }
            else
            {
                minimal = buffer[0];
            }
            if(buffer[2] < minimal)
            {
                minimal = buffer[2];
            }
        }
        if(buffer.Count == 2)
        {
            if(buffer[0] > buffer[1])
            {
                minimal = buffer[1];
            }
            else
            {
                minimal = buffer[0];
            }
        }
        if(buffer.Count == 1)
        {
            minimal = buffer[0];
        }
        if(buffer.Count == 0)
        {
            minimal = -1;
        }
        return minimal;
    }
}

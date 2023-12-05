using Network;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager _singleton;
    public static SaveLoadManager Singleton
    {
        get => _singleton;
        set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Destroy(value);
                Debug.Log($"{nameof(SaveLoadManager)} instance already exists, destroying duplicate");
            }
        }
    }

    private string directoryPath { get; set; }

    const string EnglishFileName = "English.txt";
    const string MathsFileName = "Maths.txt";
    const string UrduFileName = "Urdu.txt";


    private void Awake()
    {
        Singleton = this;
        CreateDirectory();

        LoadTeachersData();
        LoadParentsData();
        LoadStudentsData();
    }

    private void Start()
    {
        //DataManager.Singleton.AddTeacher(new Teacher("Usman", "usman", "12345", new List<int> { 1, 2, 4 }));
    }

    public string LoadStudentRecommendationData(int _studentID, string _name)
    {
        string folderName = $"{_name} - {_studentID}";
        string folderDirectory = Path.Combine(directoryPath, folderName);

        string data = string.Empty;

        if (!Directory.Exists(folderDirectory))
        {
            Directory.CreateDirectory(folderDirectory);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(folderDirectory, "Recommendation.txt"), false))
            {
                outputFile.WriteLine("");
                outputFile.Close();
            }
        }
        else
        {
            string filePath = Path.Combine(folderDirectory, "Recommendation.txt");
            if (File.Exists(filePath))
            {
                data = File.ReadAllText(filePath);
            }
            else
            {
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(folderDirectory, "Recommendation.txt"), false))
                {
                    outputFile.WriteLine("");
                    outputFile.Close();
                }
            }
        }
        return data;
    }

    public void SaveStudentRecommendation(int _studentID, string _name, string _recommend)
    {
        string folderDirectory = Path.Combine(directoryPath, $"{_name} - {_studentID}");
        string filePath = Path.Combine(folderDirectory, "Recommendation.txt");

        using (StreamWriter outputFile = new StreamWriter(filePath, false))
        {
            outputFile.WriteLine(_recommend);
            outputFile.Close();
        }
    }

    public string LoadStudentData(int _studentID, string _name)
    {
        string folderName = $"{_name} - {_studentID}";
        string folderDirectory = Path.Combine(directoryPath, folderName);

        string data = string.Empty;

        if (!Directory.Exists(folderDirectory))
        {
            Directory.CreateDirectory(folderDirectory);
            Data tempData = new Data();
            WriteLineData(folderDirectory, tempData);
            data = DataManager.Singleton.ToJson(tempData);
        }
        else
        {
            string filePath = Path.Combine(folderDirectory, EnglishFileName);
            if (File.Exists(filePath))
            {
                Data tempData = ReadAllData(folderDirectory);
                data = DataManager.Singleton.ToJson(tempData);
            }
            else
            {
                Data tempData = new Data();
                WriteLineData(folderDirectory, tempData);
                data = DataManager.Singleton.ToJson(tempData);
            }
        }

        return data;
    }

    public void SaveStudentData(int _studentID, string _name, string data)
    {
        string newDir = Path.Combine(directoryPath, $"{_name} - {_studentID}");

        if (!Directory.Exists(newDir))
        {
            Directory.CreateDirectory(newDir);
        }

        Data receivedData = DataManager.Singleton.FromJsonToWholeData(data);
        if (receivedData != null)
        {
            WriteLineData(newDir, receivedData);
        }
    }


    void WriteLineData(string _folderPath, Data _data)
    {
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(_folderPath, EnglishFileName), false))
        {
            outputFile.WriteLine(DataManager.Singleton.ToJson(_data.English));
            outputFile.Close();
        }

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(_folderPath, MathsFileName), false))
        {
            outputFile.WriteLine(DataManager.Singleton.ToJson(_data.Maths));
            outputFile.Close();
        }

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(_folderPath, UrduFileName), false))
        {
            outputFile.WriteLine(DataManager.Singleton.ToJson(_data.Urdu));
            outputFile.Close();
        }
    }

    Data ReadAllData(string _folderDirectory)
    {
        EnglishData englishData = DataManager.Singleton.FromJsonToEnglish(File.ReadAllText(Path.Combine(_folderDirectory, EnglishFileName)));
        MathsData mathsData = DataManager.Singleton.FromJsonToMaths(File.ReadAllText(Path.Combine(_folderDirectory, MathsFileName)));
        UrduData urduData = DataManager.Singleton.FromJsonToUrdu(File.ReadAllText(Path.Combine(_folderDirectory, UrduFileName)));

        Data tempData = new Data();
        tempData.English = englishData;
        tempData.Maths = mathsData;
        tempData.Urdu = urduData;
        return tempData;
    }

    public OverallProgress GetOverallProgressEnglish(List<int> _ids, List<string> _names)
    {
        OverallProgress studentsOverallProgress = new OverallProgress();

        for (int i = 0; i < _ids.Count; i++)
        {
            string studentData = LoadStudentData(_ids[i], _names[i], "English");
            if (studentData != string.Empty)
            {
                EnglishData student = DataManager.Singleton.FromJsonToEnglish(studentData);
                studentsOverallProgress.studentProgresses.Add(new StudentProgress(_names[i], DataManager.Singleton.calculator.Average(student)));
            }
        }
        return studentsOverallProgress;
    }

    public OverallProgress GetOverallProgressMaths(List<int> _ids, List<string> _names)
    {
        OverallProgress studentsOverallProgress = new OverallProgress();

        for (int i = 0; i < _ids.Count; i++)
        {
            string studentData = LoadStudentData(_ids[i], _names[i], "Maths");
            if (studentData != string.Empty)
            {
                MathsData student = DataManager.Singleton.FromJsonToMaths(studentData);
                studentsOverallProgress.studentProgresses.Add(new StudentProgress(_names[i], DataManager.Singleton.calculator.Average(student)));
            }
        }
        return studentsOverallProgress;
    }

    public OverallProgress GetOverallProgressUrdu(List<int> _ids, List<string> _names)
    {
        OverallProgress studentsOverallProgress = new OverallProgress();

        for (int i = 0; i < _ids.Count; i++)
        {
            string studentData = LoadStudentData(_ids[i], _names[i], "Urdu");
            if (studentData != string.Empty)
            {
                UrduData student = DataManager.Singleton.FromJsonToUrdu(studentData);
                studentsOverallProgress.studentProgresses.Add(new StudentProgress(_names[i], DataManager.Singleton.calculator.Average(student)));
            }
        }
        return studentsOverallProgress;
    }


    /// <summary>
    /// This Function returns the data for the individual student
    /// </summary>
    /// <param name="_studentID"> Student ID assigned </param>
    /// <param name="_name"> Name of the student </param>
    /// <param name="_subject"> Specify the subject name like "English", "Maths" OR "Urdu" </param>
    /// <returns></returns>
    public string LoadStudentData(int _studentID, string _name, string _subject)
    {
        string folderName = $"{_name} - {_studentID}";
        string folderDirectory = Path.Combine(directoryPath, folderName);

        string subjectData = string.Empty;
        string subjectFilePath = Path.Combine(folderDirectory, $"{_subject}.txt");
        if (File.Exists(subjectFilePath))
        {
            subjectData = File.ReadAllText(subjectFilePath);
        }
        return subjectData;
    }

    void LoadTeachersData()
    {
        string[] lines = LoadData("Teachers.txt");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            List<int> numList = new List<int>();
            for (int j = 4; j < values.Length; j++)
            {
                if (int.TryParse(values[j].Trim(), out int num))
                {
                    numList.Add(num);
                }
            }
            Teacher t = new Teacher(values[1], values[2], values[3], numList);
            if (int.TryParse(values[0].Trim(), out int id))
            {
                t.ID = id;
            }
            DataManager.Singleton.AddTeacher(t);
        }
    }

    void LoadParentsData()
    {
        string[] lines = LoadData("Parents.txt");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            List<int> numList = new List<int>();
            for (int j = 4; j < values.Length; j++)
            {
                if (int.TryParse(values[j].Trim(), out int num))
                {
                    numList.Add(num);
                }
            }

            Parent p = new Parent(values[1], values[2], values[3], numList);
            if (int.TryParse(values[0].Trim(), out int id))
            {
                p.ID = id;
            }
            DataManager.Singleton.AddParent(p);
        }
    }

    void LoadStudentsData()
    {
        string[] lines = LoadData("Students.txt");

        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            Student s = new Student(values[1], values[2], values[3]);
            if (int.TryParse(values[0].Trim(), out int id))
            {
                s.ID = id;
            }
            DataManager.Singleton.AddStudent(s);
        }
    }

    string[] LoadData(string _fileName)
    {
        string[] lines = File.ReadAllLines(Path.Combine(directoryPath, _fileName));
        return lines;
    }

    void CreateDirectory()
    {
        string path = Application.dataPath;

        int lastIndex = path.LastIndexOf('/');
        path = path.Remove(lastIndex, path.Length - lastIndex);

        path = Path.Combine(path, "Data Files");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        directoryPath = path;
    }
}
    
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static NetworkManager;

public class DataManager : MonoBehaviour
{
    private static DataManager _singleton;
    public static DataManager Singleton
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
                Debug.Log($"{nameof(DataManager)} instance already exists, destroying duplicate");
            }
        }
    }

    public Dictionary<int, Teacher> Teachers { get; set; }
    public Dictionary<int, Parent> Parents { get; set; }
    public Dictionary<int, Student> Students { get; set; }

    public Calculator calculator;

    private void Awake()
    {
        Singleton = this;
        Teachers = new Dictionary<int, Teacher>();
        Parents = new Dictionary<int, Parent>();
        Students = new Dictionary<int, Student>();
        calculator = new Calculator();
    }

    /*private void Start()
    {
        EnglistData englishData = FromJsonToEnglish(SaveLoadManager.Singleton.LoadStudentData(2, "Bilal", "English"));
        print("Average English: " + calculator.Average(englishData));


        print("Overall Progress of class: " + StudentsOverallProgressEnglish(new List<int> { 2, 4 }, new List<string> { "Bilal", "Danish" }));

    }*/

    // ////////////////////// English Data /////////////////// //

    public string ToJson(EnglishData data)
    {
        return JsonUtility.ToJson(data);
    }

    public EnglishData FromJsonToEnglish(string _jsonFormatData)
    {
        try
        {
            EnglishData englishData = JsonUtility.FromJson<EnglishData>(_jsonFormatData);
            return englishData == null ? throw new InvalidDataException("Data is in invalid format") : englishData;
        }
        catch (Exception ex)
        {
            print("Json string is Corrupted!!!!!. Cause: " + ex.Message);
            return null;
        }
    }


    // ////////////////////// Maths Data /////////////////// //

    public string ToJson(MathsData data)
    {
        return JsonUtility.ToJson(data);
    }

    public MathsData FromJsonToMaths(string _jsonFormatData)
    {
        try
        {
            MathsData mathsData = JsonUtility.FromJson<MathsData>(_jsonFormatData);
            return mathsData == null ? throw new InvalidDataException("Data is in invalid format") : mathsData;
        }
        catch (Exception ex)
        {
            print("Json string is Corrupted!!!!!. Cause: " + ex.Message);
            return null;
        }
    }


    // ////////////////////// Urdu Data /////////////////// //

    public string ToJson(UrduData data)
    {
        return JsonUtility.ToJson(data);
    }

    public UrduData FromJsonToUrdu(string _jsonFormatData)
    {
        try
        {
            UrduData urduData = JsonUtility.FromJson<UrduData>(_jsonFormatData);
            return urduData == null ? throw new InvalidDataException("Data is in invalid format") : urduData;
        }
        catch (Exception ex)
        {
            print("Json string is Corrupted!!!!!. Cause: " + ex.Message);
            return null;
        }
    }

    // /////////// Whole Data //////////////// //

    public string ToJson(Data data)
    {
        return JsonUtility.ToJson(data);
    }

    public Data FromJsonToWholeData(string _jsonFormatData)
    {
        try
        {
            Data data = JsonUtility.FromJson<Data>(_jsonFormatData);
            return data == null ? throw new InvalidDataException("Data is in invalid format") : data;
        }
        catch (Exception ex)
        {
            print("Json string is Corrupted!!!!!. Cause: " + ex.Message);
            return null;
        }
    }


    // 5, 6
    // ////////////////////////////  Student In-Game Data Functions ////////////////////////////// //

    // Student In Game Data Load, and send to client
    [MessageHandler((ushort)ServerToClient.StudentInGameDataRequest)]
    private static void ReceiveStudentInGameDataRequest(ushort _clientID, Message _message)
    {
        int studentID = _message.GetInt();
        string studentName = _message.GetString();

        string data = SaveLoadManager.Singleton.LoadStudentData(studentID, studentName);

        Message sendMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.StudentInGameDataRequest);
        sendMessage.AddString(data);
        
        // String.empty if no data found, else there is some data
        NetworkManager.Singleton.Server.Send(sendMessage, _clientID);
    }


    // Receive Student In Game Uploaded Data and save in the server files
    [MessageHandler((ushort)ServerToClient.StudentInGameDataSave)]
    private static void ReceiveStudentInGameDataSave(ushort _clientID, Message message)
    {
        int id = message.GetInt();
        string name = message.GetString();
        string data = message.GetString();

        SaveLoadManager.Singleton.SaveStudentData(id, name, data);

        // Data Saved Acknowledge
        Message sendMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.StudentInGameDataSave);
        sendMessage.AddString("Data Saved");
        NetworkManager.Singleton.Server.Send(sendMessage, _clientID);
    }

    // 7
    // ////////// Get Individual Student Data from the subject name /////////////// //
    [MessageHandler((ushort)ServerToClient.ISD_English)]
    private static void ReceiveStudentDataEnglish(ushort _clientID, Message _message)
    {
        int id = _message.GetInt();
        string name = _message.GetString();

        string data = SaveLoadManager.Singleton.LoadStudentData(id, name, "English");
        Message sendMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.ISD_English);
        sendMessage.AddString(data);
        NetworkManager.Singleton.Server.Send(sendMessage, _clientID);
    }

    // 8
    [MessageHandler((ushort)ServerToClient.ISD_Maths)]
    private static void ReceiveStudentDataMaths(ushort _clientID, Message _message)
    {
        int id = _message.GetInt();
        string name = _message.GetString();

        string data = SaveLoadManager.Singleton.LoadStudentData(id, name, "Maths");
        Message sendMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.ISD_Maths);
        sendMessage.AddString(data);
        NetworkManager.Singleton.Server.Send(sendMessage, _clientID);
    }

    //9
    [MessageHandler((ushort)ServerToClient.ISD_Urdu)]
    private static void ReceiveStudentDataUrdu(ushort _clientID, Message _message)
    {
        int id = _message.GetInt();
        string name = _message.GetString();

        string data = SaveLoadManager.Singleton.LoadStudentData(id, name, "Urdu");
        Message sendMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.ISD_Urdu);
        sendMessage.AddString(data);
        NetworkManager.Singleton.Server.Send(sendMessage, _clientID);
    }


    // TODO: Complete Overall class progress from server side and start working on the client data visualization.
    // But first change the ClientToServer ID's in client side

    // 10
    // Overall English Progress
    [MessageHandler((ushort)ServerToClient.OCP_English)]
    private static void ReceiveStudentsOverallDataEnglish(ushort _clientID, Message _message)
    {
        List<int> ids = _message.GetInts().ToList();
        List<string> names = _message.GetStrings().ToList();

        string progressData = Singleton.StudentsOverallProgressEnglish(ids, names);

        print(progressData);

        Message sendMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.OCP_English);
        sendMessage.AddString(progressData);
        NetworkManager.Singleton.Server.Send(sendMessage, _clientID);
    }

    string StudentsOverallProgressEnglish(List<int> _ids, List<string> _names)
    {
        OverallProgress studentsOverallProgress = SaveLoadManager.Singleton.GetOverallProgressEnglish(_ids, _names);
        return JsonUtility.ToJson(studentsOverallProgress);
    }


    // 11
    // Overall Maths Progress
    [MessageHandler((ushort)ServerToClient.OCP_Maths)]
    private static void ReceiveStudentsOverallDataMaths(ushort _clientID, Message _message)
    {
        List<int> ids = _message.GetInts().ToList();
        List<string> names = _message.GetStrings().ToList();

        string progressData = Singleton.StudentsOverallProgressMaths(ids, names);

        Message sendMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.OCP_Maths);
        sendMessage.AddString(progressData);
        NetworkManager.Singleton.Server.Send(sendMessage, _clientID);
    }

    string StudentsOverallProgressMaths(List<int> _ids, List<string> _names)
    {
        OverallProgress studentsOverallProgress = SaveLoadManager.Singleton.GetOverallProgressMaths(_ids, _names);
        return JsonUtility.ToJson(studentsOverallProgress);
    }


    // 12
    // Overall Urdu Progress
    [MessageHandler((ushort) ServerToClient.OCP_Urdu)]
    private static void ReceiveStudentsOverallDataUrdu(ushort _clientID, Message _message)
    {
        List<int> ids = _message.GetInts().ToList();
        List<string> names = _message.GetStrings().ToList();

        string progressData = Singleton.StudentsOverallProgressUrdu(ids, names);

        Message sendMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.OCP_Urdu);
        sendMessage.AddString(progressData);
        NetworkManager.Singleton.Server.Send(sendMessage, _clientID);
    }

    string StudentsOverallProgressUrdu(List<int> _ids, List<string> _names)
    {
        OverallProgress studentsOverallProgress = SaveLoadManager.Singleton.GetOverallProgressUrdu(_ids, _names);
        return JsonUtility.ToJson(studentsOverallProgress);
    }


    // 14
    // //////////// Get Student ID's and Name for further Data Visualization ///////////////////// //

    [MessageHandler((ushort)ServerToClient.StudentIDAndName)]
    static void ReceiveStudentsIDs(ushort _clientID, Message message)
    {
        int[] ids = message.GetInts();

        // Debug
        string d = "";
        foreach (int i in ids)
            d += i.ToString() + " ";
        print("Student ID's Recevied: " + d);

        for (int i = 0; i < ids.Length; i++)
        {
            Student stud = Singleton.FindStudent(ids[i]);
            if (stud != null)
            {
                SendStudentsDataBack(_clientID, stud);
            }
        }
    }

    static void SendStudentsDataBack(ushort _clientID, Student _student)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClient.StudentIDAndName);
        message.AddStudent(_student);
        NetworkManager.Singleton.Server.Send(message, _clientID);
    }




    [MessageHandler((ushort)ServerToClient.StudentRecommendation)]
    private static void ReceiveStudentRecommendationRequest(ushort clientID, Message message)
    {
        Student student = message.GetStudent();

        string recommendationStr = SaveLoadManager.Singleton.LoadStudentRecommendationData(student.ID, student.Name);

        Message sendMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.StudentRecommendation);
        sendMessage.AddString(recommendationStr);
        NetworkManager.Singleton.Server.Send(sendMessage, clientID);
    }


    [MessageHandler((ushort)ServerToClient.SendRecommendation)]
    private static void ReceiveStudentRecommendation(ushort clientID, Message message)
    {
        Student student = message.GetStudent();
        string recommen = message.GetString();

        SaveLoadManager.Singleton.SaveStudentRecommendation(student.ID, student.Name, recommen);

        Message sendMessage = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.SendRecommendation);
        sendMessage.AddString("Recommendation Saved");
        NetworkManager.Singleton.Server.Send(sendMessage, clientID);
    }


    #region Login

    // Receive Login Request Teacher
    [MessageHandler((ushort)ServerToClient.LoginTeacher)]
    private static void ReceivingLoginRequestTeacher(ushort _clientID, Message _message)
    {
        string[] data = _message.GetLogin();
        Teacher teacher = Singleton.FindTeacher(data[0], data[1]);
        Singleton.SendLoginRequestResult(teacher, _clientID);
    }

    // Receive Login Request Parent
    [MessageHandler((ushort)ServerToClient.LoginParent)]
    private static void ReceivingLoginRequestParent(ushort _clientID, Message _message)
    {
        string[] data = _message.GetLogin();
        Parent parent = Singleton.FindParent(data[0], data[1]);
        Singleton.SendLoginRequestResult(parent, _clientID);
    }

    // Receive Login Request Student
    [MessageHandler((ushort)ServerToClient.LoginStudent)]
    private static void ReceivingLoginRequestStudent(ushort _clientID, Message _message)
    {
        string[] data = _message.GetLogin();
        Student student = Singleton.FindStudent(data[0], data[1]);
        Singleton.SendLoginRequestResult(student, _clientID);
    }

    void SendLoginRequestResult(Teacher _teacher, ushort _clientID)
    {
        if (_teacher.ID == -1)
        {
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.Invalid);
            message.AddString($"Invalid");
            NetworkManager.Singleton.Server.Send(message, _clientID);
            StartCoroutine(DisconnectClient(_clientID));
        }
        else
        {
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.LoginTeacher);
            message.AddTeacher(_teacher);
            NetworkManager.Singleton.Server.Send(message, _clientID);
        }
    }

    void SendLoginRequestResult(Parent _parent, ushort _clientID)
    {
        if (_parent.ID == -1)
        {
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.Invalid);
            message.AddString("Invalid");
            NetworkManager.Singleton.Server.Send(message, _clientID);
            StartCoroutine(DisconnectClient(_clientID));
        }
        else
        {
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.LoginParent);
            message.AddParent(_parent);
            NetworkManager.Singleton.Server.Send(message, _clientID);
        }
    }

    void SendLoginRequestResult(Student _student, ushort _clientID)
    {
        if (_student.ID == -1)
        {
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.Invalid);
            message.AddString($"Invalid");
            NetworkManager.Singleton.Server.Send(message, _clientID);
            StartCoroutine(DisconnectClient(_clientID));
        }
        else
        {
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClient.LoginStudent);
            message.AddStudent(_student);
            NetworkManager.Singleton.Server.Send(message, _clientID);
        }
    }

    IEnumerator DisconnectClient(ushort _clientID)
    {
        yield return new WaitForSeconds(0.1f);
        NetworkManager.Singleton.Server.DisconnectClient(_clientID);
    }

    #endregion

    #region Finding Functions

    // ////////////////////////////////////// Teacher's Functions ////////////////////////////////////// //

    Teacher FindTeacher(string _username, string _password)
    {
        foreach (Teacher teacher in Teachers.Values)
        {
            if (teacher.Username.Equals(_username) &&
                teacher.Password.Equals(_password))
            {
                return teacher;
            }
        }
        return new Teacher();
    }

    Teacher FindTeacher(int _teacherID)
    {
        if(Teachers.TryGetValue(_teacherID, out Teacher teacher))
            return teacher;
        return null;
    }

    public bool AddTeacher(Teacher _teacher)
    {
        if (!Teachers.ContainsKey(_teacher.ID))
        {
            if(_teacher.ID == -1)
                _teacher.ID = Teachers.Count + 1;
            Teachers.Add(_teacher.ID, _teacher);
            return true;
        }
        return false;
    }

    // ////////////////////////////////////// Parent's Functions ////////////////////////////////////// //

    Parent FindParent(string _username, string _password)
    {
        foreach (Parent parent in Parents.Values)
        {
            if (parent.Username.Equals(_username) &&
                parent.Password.Equals(_password))
            {
                return parent;
            }
        }
        return new Parent();
    }

    public bool AddParent(Parent _parent)
    {
        if (!Parents.ContainsKey(_parent.ID))
        {
            if (_parent.ID == -1)
                _parent.ID = Parents.Count + 1;
            Parents.Add(_parent.ID, _parent);
            return true;
        }
        return false;
    }

    // ////////////////////////////////////// Student's Functions ////////////////////////////////////// //

    Student FindStudent(string _username, string _password)
    {
        foreach (Student student in Students.Values)
        {
            if (student.Username.Equals(_username) &&
                student.Password.Equals(_password))
            {
                return student;
            }
        }
        return new Student();
    }

    Student FindStudent(int _studentID)
    {
        if(Students.TryGetValue(_studentID, out Student student))
            return student;
        return null;
    }

    public bool AddStudent(Student _student)
    {
        if (!Students.ContainsKey(_student.ID))
        {
            _student.ID = Students.Count + 1;
            Students.Add(_student.ID, _student);
            return true;
        }
        return false;
    }

    #endregion

}

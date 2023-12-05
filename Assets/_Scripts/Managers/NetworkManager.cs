using Network.Utils;
using Network;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public enum ServerToClient : ushort
    {
        // Login
        LoginTeacher = 1,
        LoginParent = 2,
        LoginStudent = 3,
        Invalid = 4,

        // Student In Game Data
        StudentInGameDataRequest = 5,
        StudentInGameDataSave = 6,

        // Individual Student Data Request
        ISD_English = 7,
        ISD_Urdu = 8,
        ISD_Maths = 9,

        // Overall Class Progress
        OCP_English = 10,
        OCP_Urdu = 11,
        OCP_Maths = 12,

        // Recommendation
        StudentRecommendation = 13,

        // Student Name and ID to get data
        StudentIDAndName = 14,

        // Send Recommendation
        SendRecommendation = 15,
    }

    private static NetworkManager _singleton;
    public static NetworkManager Singleton
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
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate");
            }
        }
    }

    [SerializeField] ushort m_MaxClientCount;
    [SerializeField] ushort m_Port;

    public Server Server { get; private set; }

    private void Awake()
    {
        Singleton = this;

        Application.targetFrameRate = 60;

        NetworkLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Server = new Server(new Network.Transports.Tcp.TcpServer());
        Server.Start(m_Port, m_MaxClientCount);
    }


    private void FixedUpdate()
    {
        Server.Update();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }
}

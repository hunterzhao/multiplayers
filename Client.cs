using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System;
using System.Text;

public class Client : MonoBehaviour
{
    public GameObject m_PlayerPrefab;
    public int m_LocalNumber;
    private Dictionary<int, PlayerManager> m_Players = new Dictionary<int, PlayerManager>();
    private NetworkStream m_stream;
    private Queue<Message> m_ConstructQueue = new Queue<Message>();
    // Use this for initialization
    void Start()
    {
        //创建无参线程对象,限制256KB堆栈大小
        Thread thr = new Thread(Func, 262144);
        //启动线程
        thr.Start();
    }

    private void Update()
    {
        if (m_ConstructQueue.Count == 0)
        {
            return;
        }
        Console.WriteLine("this is Console Write line test ");
        //Debug.Log("msg checked!");
        var msg = m_ConstructQueue.Dequeue();
        SpawnPlayers(msg.id, new Vector3(0,0,0));
    }

    void SpawnPlayers(int id, Vector3 position)
    {
        PlayerManager tmp = new PlayerManager();
        if (tmp == null) return;
        tmp.m_Instance = Instantiate(m_PlayerPrefab, position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
        tmp.m_PlayerNumber = id;
        tmp.m_LocalNumber = m_LocalNumber;
        tmp.m_sender = this;
        tmp.m_MsgQueue = new Queue<Message>();
        tmp.Setup();
        m_Players[id] = tmp;
        Debug.Log("Player:" + Convert.ToString(id) + " construct");
    }

    public void Send(Message msg)
    {
        string sendmsg = msg.SaveToString() + "#";
        //直接将msg发送出去
        ASCIIEncoding asen = new ASCIIEncoding();
        //Debug.Log("Transmitting..." + sendmsg);
        byte[] ba = asen.GetBytes(sendmsg);
        m_stream.Write(ba, 0, ba.Length);
    }

    void Func()
    {
        //创建服务器连接
        //connect to server
        TcpClient cln = new TcpClient();
        cln.Connect("192.168.0.121", 8899);
        m_stream = cln.GetStream();
        Debug.Log("connected");

        //发送一个创建player消息
        Message msg = new Message();
        msg.id = m_LocalNumber;
        msg.type = 0;
        Send(msg);

        // Buffer to store the response bytes.
        Byte[] data = new Byte[256];

        while (true)
        {
            String responseData = String.Empty;

            Debug.Log("wait for msg");
            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = m_stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            Debug.Log("msg received :" + responseData);

            string[] sArray = responseData.Split('#');
            if (sArray.Length == 0) break;
            for (int i = 0; i < sArray.Length; i++)
            {
                Message recv = Message.CreateFromJSON(sArray[i]);
                Debug.Log("already pase this msg");
                if (recv == null)
                {
                    //Debug.Log("error parse: messgae id" + Convert.ToString(recv.id) + " message type:" + Convert.ToString(recv.type));
                    continue;
                }
                if (recv.type == 0)
                {
                    m_ConstructQueue.Enqueue(recv);
                    Debug.Log("already push construct msg");
                }
                else if (recv.type == 1)
                {
                    m_Players[recv.id].m_MsgQueue.Enqueue(recv);
                    Debug.Log("push move msg");
                }
            }
            Debug.Log("loop over");
        }
        Debug.Log("over");
        // Close everything.
        m_stream.Close();
        cln.Close();    
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public int id; //消息作用玩家id
    public float x = 0.0f;
    public float z = 0.0f;
    public int type;  //0 创建player 1 move

    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }

    public static Message CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Message>(jsonString);
    }
}

public class PlayerManager
{
    [HideInInspector]
    public int m_PlayerNumber;
    [HideInInspector]
    public int m_LocalNumber;
    [HideInInspector]
    public GameObject m_Instance;
    [HideInInspector]
    public Client m_sender;
    [HideInInspector]
    public Queue<Message> m_MsgQueue;

    private Movement m_Movement;

    public void Setup()
    {
        // Get references to the components.
        m_Movement = m_Instance.GetComponent<Movement>();
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Movement.m_LocalNumber = m_LocalNumber;
        m_Movement.m_sender = m_sender;
        m_Movement.m_MsgQueue = m_MsgQueue;
    }
}

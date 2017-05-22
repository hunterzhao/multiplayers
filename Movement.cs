using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    private Rigidbody m_Rigidbody;

    [HideInInspector]
    public int m_PlayerNumber;
    [HideInInspector]
    public int m_LocalNumber;
    [HideInInspector]
    public Client m_sender;
    [HideInInspector]
    public Queue<Message> m_MsgQueue;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {

    }

    public bool CheckAndGetMsg(ref Message msg)
    {
        //检查队列是否有消息，有消息则返回消息
        if (m_MsgQueue.Count == 0)
        {
            return false;
        }
        //Debug.Log("msg checked!");
        msg = m_MsgQueue.Dequeue();

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        Message msg = new Message();
        //检查队列是否收到消息： 
        while (CheckAndGetMsg(ref msg))
        {
            switch (msg.type)
            {
                case 1://判断消息是移动某个player?
                    float x = msg.x;  //接收到的网络命令包
                    float z = msg.z;  //接收到的网络命令包

                    //对特定物体执行运动指令
                    Move(x);
                    Turn(z);
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_LocalNumber != m_PlayerNumber)
            return;
        // Store the value of both input axes. for localplayer
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
        //将 x z 与playerid写入发送队列
        Message msg = new Message();
        msg.id = m_LocalNumber;
        msg.x = x;
        msg.z = z;
        msg.type = 1;
        m_sender.Send(msg);
    }

    public void Move(float m_MovementInputValue)
    {
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        Vector3 movement = transform.forward * m_MovementInputValue * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    public void Turn(float m_TurnInputValue)
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInputValue * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}
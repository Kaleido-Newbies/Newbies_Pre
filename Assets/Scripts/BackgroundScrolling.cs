using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    public static BackgroundScrolling backGround;
    
    [SerializeField] Transform[] m_tfBackgrounds = null;
    [SerializeField] private float m_speed = 0f;

    private float m_leftPosX = 0f;
    private float m_rightPosX = 0f;
    
    void Start()
    {
        float t_length = m_tfBackgrounds[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        m_leftPosX = -t_length;
        m_rightPosX = t_length * m_tfBackgrounds.Length;
    }
    
    void Update()
    {
        backGround = this;
        backGroundMove();
    }

    void backGroundMove()
    {
        for (int i = 0; i < m_tfBackgrounds.Length; i++)
        {
            m_tfBackgrounds[i].position += new Vector3(m_speed, 0, 0) * Time.deltaTime;

            if (m_tfBackgrounds[i].position.x < m_leftPosX)
            {
                Vector3 t_selfPos = m_tfBackgrounds[i].position;
                t_selfPos.Set(t_selfPos.x + m_rightPosX, t_selfPos.y, t_selfPos.z);
                m_tfBackgrounds[i].position = t_selfPos;
            }
        }
    }
    
}

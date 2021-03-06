﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;
using System.Security.Cryptography;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private bool clave;

    [SerializeField]
    private Light2D light;

    [SerializeField]
    private Image heart;

    [SerializeField]
    private Image heart2;

    [SerializeField]
    private Image heart3;

    [SerializeField]
    private int life;

    [SerializeField]
    private float speed = 5;

    [SerializeField]
    private Animator animator;

    private Rigidbody2D rb;

    [SerializeField]
    private Camera camara;
    private Vector3Int playerPos;

    Vector3 startPos;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    public static bool usingBlade = false;

    [SerializeField]
    public static bool usingShield = false;

    public static Vector3 bladePosLeft;
    public static Vector3 bladePosRight;
    public static Vector2 bladeLeftOrRight;
    public static Vector3 shieldPosLeft;
    public static Vector3 shieldPosRight;
    public static Vector2 shieldLeftOrRight;

    [SerializeField]
    private Tilemap tilemap;
    private Vector3Int localPlace;
    private int[,] bitmap;
    private string string_bitmap;

    private bool map_sent = true;

    private void putLife()
    {

        if (life == 3)
        {

            heart.GetComponent<Animator>().SetBool("hit", false);
            heart2.GetComponent<Animator>().SetBool("hit", false);
            heart3.GetComponent<Animator>().SetBool("hit", false);

        }

        else if (life == 2)
        {

            heart.GetComponent<Animator>().SetBool("hit", true);
            heart2.GetComponent<Animator>().SetBool("hit", false);
            heart3.GetComponent<Animator>().SetBool("hit", false);

        }

        else if (life == 1)
        {

            heart.GetComponent<Animator>().SetBool("hit", true);
            heart2.GetComponent<Animator>().SetBool("hit", true);
            heart3.GetComponent<Animator>().SetBool("hit", false);

        }

        else if (life == 0)
        {

            heart.GetComponent<Animator>().SetBool("hit", true);
            heart2.GetComponent<Animator>().SetBool("hit", true);
            heart3.GetComponent<Animator>().SetBool("hit", true);

        }

    }

    // Use this for initialization
    void Start()
    {

        startPos = transform.position;

        //Envía el mapa de bits del nivel actual al servidor.
        createBitMap();

    }

    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        life = 3;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //SE EJECUTA LA ACCIÓN COMO RESULTADO DE PRESIONAR LA TECLA F
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!usingBlade)
            {
                if (usingShield)
                {
                    usingShield = false;
                    usingBlade = true;
                }
                else
                {
                    usingBlade = true;
                }
            }
            else
            {
                usingBlade = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!usingShield)
            {
                if (usingBlade)
                {
                    usingBlade = false;
                    usingShield = true;
                }
                else
                {
                    usingShield = true;
                }
            }
            else
            {
                usingShield = false;
            }
        }

        this.putLife();

        if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        else if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }

        camara.transform.position = new Vector3(transform.position.x, transform.position.y, camara.transform.position.z);

        bladePosRight = new Vector3(transform.position.x + 0.65f, transform.position.y, 0);
        bladePosLeft = new Vector3(transform.position.x - 0.65f, transform.position.y, 0);
        bladeLeftOrRight = transform.localScale;

        shieldPosRight = new Vector3(transform.position.x + 1f, transform.position.y, 0);
        shieldPosLeft = new Vector3(transform.position.x - 1f, transform.position.y, 0);
        shieldLeftOrRight = transform.localScale;

        if (light != null)
        {
            light.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
        }


        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(horizontal * speed, vertical * speed);

        animator.SetFloat("Velocidad", Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        //Envía la posición al servidor

        if (rb.velocity.magnitude != 0)
        {
            if (map_sent)
            {
                Client.Instance.sendMsgTCP("bitmap," + string_bitmap);
                map_sent = false;
            }
            playerPos = grid.WorldToCell(rb.transform.position);
            Client.Instance.sendMsgUDP("player,pos," + playerPos.x.ToString("0") + ","
                                        + playerPos.y.ToString("0") + "\n");
        }

    }

    public void restartPlayerPos()
    {
        transform.position = startPos;
    }
    public int getLife()
    {
        return this.life;
    }
    public bool getKey()
    {
        return this.clave;
    }
    public Vector3 getStartPos()
    {
        return this.startPos;
    }

    public void setKey(bool key)
    {
        this.clave = key;
    }
    public void setLife(int life)
    {
        this.life = life;
    }


    //Crea el mapa de bits que el servidor usará para aplicar los algoritmos.
    void createBitMap()
    {
        bitmap = new int[tilemap.size.x, tilemap.size.y];
        for (int i = tilemap.origin.x; i < tilemap.origin.x + tilemap.size.x; i++)
        {
            for (int j = tilemap.origin.y; j < tilemap.origin.y + tilemap.size.y; j++)
            {
                localPlace = (new Vector3Int(i, j, (int)tilemap.transform.position.y));
                if (tilemap.HasTile(localPlace))
                {
                    bitmap[localPlace.x - tilemap.origin.x, localPlace.y - tilemap.origin.y] = 1;
                }
                else
                {
                    bitmap[localPlace.x - tilemap.origin.x, localPlace.y - tilemap.origin.y] = 0;
                }
            }
        }

        string_bitmap = "";
        for (int j = tilemap.size.y - 1; j >= 0; j--)
        {
            for (int i = 0; i < tilemap.size.x; i++)
            {
                string_bitmap += bitmap[i, j].ToString("0");
            }
            string_bitmap += "\n";
        }
    }
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//Author: Adrian Zimmer
//Description: Hitpoints + Shield script. Zur benutzung: auf dem Script Applydamage(amount,schadensQuelle als gameobjekt) aufrufen
//Date Created: 03.04.2017
//Last edited:
//Edited by:

public class Hitpoints : NetworkBehaviour
{
    [Header("Parameter")]
    public const int maxHitpoints = 100;
    public int maxShield = 100;
    public float timeTilShieldRestore;  //Zeit die man keinen damage bekommen darf bis das Schild sich wieder auflädt
    public AudioClip hitSound;
    private bool dead = false;

    [Header("Refferenzen")]
    public Text hitpointsText;          //Hud Text Hp
    public Text shieldText;             //Hud Text Shield
    public Text respawnTimerText;
    public GameObject respawnHud;
    public GameObject Mesh;
    public GameObject Skeleton;
    public CharacterController ctrl;

    private NetworkStartPosition[] spawnPoints;
    [SyncVar(hook = "OnChangeHealth")]
    public int hitpoints = maxHitpoints;
    [SyncVar(hook = "OnChangeShield")]
    public int shield;
    private AudioSource audioSource;
    private float lastHitTimestamp;      //Zeitpunkt zu dem man das letzte mal schaden bekommen + cooldown
    private bool restoringShield;        //gibt an ob das shield gerade am aufladen ist

    [SyncVar]
    public int deaths = 0;
    [SyncVar]
    public int kills = 0;
    [SyncVar]
    public int score = 0;

    [SyncVar]
    public string playerName;

    private bool hasDoubleShield;
    public bool HasDoubleShield
    {
        get
        {
            return hasDoubleShield;
        }
        set
        {
            hasDoubleShield = value;
            if(value == true)
            {
                shield = 200;
                maxShield = 200;
                CancelInvoke("ResetDoubleShield");
                Invoke("ResetDoubleShield", DoubleShield.durationStatic);
            }
        }
    }
    private void ResetDoubleShield()
    {
        hasDoubleShield = false;
        maxShield = 100;
        if (shield > 100)
            shield = 100;
    }

    private bool hasVampire;
    public bool HasVampire
    {
        get { return hasVampire; }
        set
        {
            hasVampire = value;
            if(value == true)
            {
                CancelInvoke("ResetVampire");
                Invoke("ResetVampire", Vampire.durationStatic);
            }
        }
    }
    private void ResetVampire()
    {
        HasVampire = false;
    }
    void Start()
    {
        hitpoints = maxHitpoints;
        shield = maxShield;
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
            GameObject hudCanvas = GameObject.Find("HudCanvas");
            hitpointsText = hudCanvas.transform.Find("HealthUI/Hitpoints").GetComponent<Text>();
            shieldText = hudCanvas.transform.Find("HealthUI/Shield").GetComponent<Text>();
            respawnTimerText = hudCanvas.transform.Find("DeathUI/bg/TimerText").GetComponent<Text>();
            respawnHud = hudCanvas.transform.Find("DeathUI").gameObject;

        }
        hitpoints = maxHitpoints;
        shield = maxShield;

    }

    public void Update()
    {
        if (!isServer)
            return;

        if (lastHitTimestamp < Time.time && shield < maxShield && !restoringShield && !dead)
        {
            StartCoroutine(RestoreShield());
        }
    }

    public void Heal(int amount)
    {
        if (!isServer)
            return;

        if (hitpoints + amount <= maxHitpoints)
        {
            hitpoints += amount;
        }
        else
        {
            hitpoints = maxHitpoints;
        }

    }
    public void TakeDamage(int amount, GameObject inflicter)
    {
        if (!isServer || dead)
            return;

        lastHitTimestamp = Time.time + timeTilShieldRestore;
        if(inflicter)
        {
            Hitpoints inflicterHp = inflicter.GetComponent<Hitpoints>();
            if (inflicterHp != null && inflicterHp.HasVampire)
            {
                inflicterHp.Heal((int)(amount * 0.5f));
            }
        }

        //Schild+damage abzugsberechnungen
        int differenz = shield - amount;
        if (differenz < 0)
        {
            hitpoints += differenz;

            if (hitpoints < 0)
                hitpoints = 0;

            shield = 0;
        }
        else
        {
            shield -= amount;
        }


        if (audioSource != null)
            audioSource.PlayOneShot(hitSound);

        if (hitpoints <= 0)
        {
            dead = true;
            HasVampire = false;
            HasDoubleShield = false;
            deaths++;
            if(inflicter)
                inflicter.GetComponent<Hitpoints>().AddScore();

            RpcRespawn();

        }
    }

    [Server]
    private void AddScore()
    {
        kills++;
        score += 10;
    }

    [ClientRpc]
    private void RpcRespawn()
    {
        Mesh.SetActive(false);
        Skeleton.SetActive(false);
        ctrl.detectCollisions = false;
        if (isLocalPlayer)
        {
            PlayerMovement mov = GetComponent<PlayerMovement>();
            mov.HasInfiniteDash = false;
            mov.movementBlocked = true;
            StartCoroutine(respawnTimer());
        }
    }

    private IEnumerator respawnTimer()
    {

        int seconds = 0;
        ScoreboardManager.s_Singleton.GenerateScoreboard();
        // Set the spawn point to origin as a default value
        Vector3 spawnPoint = Vector3.zero;

        respawnHud.SetActive(true);
        // If there is a spawn point array and the array is not empty, pick one at random
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
        }

        while (seconds < 4)
        {
            seconds++;
            if(seconds == 3)
                transform.position = spawnPoint;
            respawnTimerText.text = "Respawn in " + (4 - seconds);
            yield return new WaitForSeconds(1);
        }
        CmdRespawned();
        GetComponent<Combat.GunController>().PickGun(0);
        // Set the player’s position to the chosen spawn point

        CmdRespawned();
        respawnHud.SetActive(false);
        PlayerMovement mov = GetComponent<PlayerMovement>();
        mov.movementBlocked = false;
        mov.dashes = mov.maxDashes;
        mov.UpdateDashText();
    }

    [Command]
    public void CmdRespawned()
    {
        hitpoints = maxHitpoints;
        shield = maxShield;
        dead = false;
        RpcRespawned();
    }

    [ClientRpc]
    private void RpcRespawned()
    {
        Mesh.SetActive(true);
        Skeleton.SetActive(true);
        ctrl.detectCollisions = true;
    }

    private void OnChangeHealth(int currentHealth)
    {
        //GUI Text setzen
        if (hitpointsText)
            hitpointsText.text = "" + currentHealth;

    }

    private void OnChangeShield(int currentShield)
    {
        //GUI Text setzen
        if (shieldText)
            shieldText.text = "" + currentShield;
    }

    private IEnumerator RestoreShield()
    {
        restoringShield = true;
        while (shield < maxShield && lastHitTimestamp < Time.time && !dead)
        {
            shield += 2;

            if (shield > maxShield)
                shield = maxShield;
            if (shieldText)
                shieldText.text = "" + shield;
            yield return new WaitForSeconds(0.09f);
        }
        restoringShield = false;
    }

}

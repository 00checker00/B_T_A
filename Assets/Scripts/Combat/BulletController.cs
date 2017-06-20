﻿using UnityEngine;
using UnityEngine.Networking;

namespace Combat
{
    public class BulletController : NetworkBehaviour
    {
        [SyncVar]
        public NetworkInstanceId spawnedBy;
        public GameObject obj;
        private bool damaged;
        public override void OnStartClient()
        {
            obj = ClientScene.FindLocalObject(spawnedBy);
            Collider[] playerColliders = obj.GetComponents<Collider>();
            Collider bulletCollider = gameObject.GetComponent<Collider>();
            foreach (Collider c in playerColliders)
            {
                Physics.IgnoreCollision(c, bulletCollider);
            }
        }

        [SerializeField]
        private int _damage;
        public int Damage
        {
            get { return _damage; }
            set { _damage = value; }
        }
        public void SetObj(GameObject enemyObj)
        {
            obj = enemyObj;
        }

        //Valentin: Habe Vampire und Double Damage hier aufgerufen
        void OnTriggerEnter(Collider collision)
        {
            if (damaged)
                return;

            GameObject hit = collision.gameObject;
            Hitpoints health = hit.GetComponent<Hitpoints>();
            PowerUpCountDown countdown = obj.GetComponent<PowerUpCountDown>();
            Hitpoints instantiatorHealth = obj.GetComponent<Hitpoints>();
            if (health != null)
            {
                health.TakeDamage(Damage, obj);
                damaged = true;
                if (countdown.hasPowerUp && instantiatorHealth.hasVampire)
                {
                    //Debug.Log("VampirePowerUpinstantiatorHealth: " + instantiatorHealth + "hit: " + hit);
                    instantiatorHealth.Heal(5);

                }

                if (countdown.hasPowerUp && instantiatorHealth.hasDoubleDamage)
                {
                    //Debug.Log("hit doubleDamage");
                    health.TakeDamage(Damage * 2, gameObject);
                }

            }

            //countdown.hasPowerUp

            //Debug.Log("Player " + obj + " hit Player " + hit);
        
    

            Destroy(gameObject);
        }

    }
}


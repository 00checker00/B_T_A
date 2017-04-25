﻿using AssemblyCSharp;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Combat
{
	public class GunController : NetworkBehaviour
	{
        public Transform FirePoint;
        public Transform GunHolder;

        public IGun CurrentGun { get; private set; }

        [SerializeField]
        private GameObject _bullet;
        public GameObject Bullet
        {
            get { return _bullet; }
            set { _bullet = value; }
        }

        void Update ()
	    {
            if (!isLocalPlayer)
            {
                return;
            }

            if (CurrentGun != null)
	        {
	            CurrentGun.Update();

	            if (Input.GetMouseButton(0))
	            {
	                CurrentGun.Shoot(!Input.GetMouseButtonDown(0));
                    //CmdFire();
	            }
	        }

	        // Temporarily switch weapons using Number Keys
	        if (Input.GetKeyDown("1"))
	        {
	            ChangeGun(Weapons.Templates.Pistol.CreateGun(this));
	        }
	        else if (Input.GetKeyDown("2"))
	        {
	            ChangeGun(Weapons.Templates.Shotgun.CreateGun(this));
	        }
	        else if (Input.GetKeyDown("3"))
	        {
	            ChangeGun(Weapons.Templates.AssaultRifle.CreateGun(this));
	        }
	        else if (Input.GetKeyDown("4"))
	        {
	            ChangeGun(Weapons.Templates.Railgun.CreateGun(this));
	        }
	    }


        [Command]
        public void CmdFire(Quaternion rotation, float speed, float maxDistance)
        {
            


            var newBullet = GameObject.Instantiate(Bullet, FirePoint.position, rotation);

            Debug.Log(newBullet + " newBullet");

            newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * speed;


            //var bulletController = newBullet.GetComponent<BulletController>();
            //bulletController.MaxDistance =maxDistance;


            NetworkServer.Spawn(newBullet);

            Destroy(newBullet, 2f);
        }

	    void Start ()
	    {

            if (!isLocalPlayer)
            {
                return;
            }

            ChangeGun(Weapons.Templates.Pistol.CreateGun(this));
	    }

	    public void ChangeGun(IGun gun)
	    {
	        CurrentGun = gun;

            //direct assignment saves headaches
	        //FirePoint = transform.Find("FirePoint");
	        //GunHolder = transform.Find("GunHolder");
	    }
	}
}


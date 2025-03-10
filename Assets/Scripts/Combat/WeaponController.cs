﻿using System;
using UnityEngine;

namespace Combat
{
    public class WeaponController : MonoBehaviour {
        public int Id;
        public float Cooldown = 0f;
        public int Damage = 5;
        public AudioClip Sound;
        public Sprite Image;
        public Color Color;
        public float MaxShotDistance = 50f;
        public Boolean AutoFire = false;
        public float ResetCoolown = 0f;
        public float BulletSpeed = 50f;
        public int BulletsPerShot = 1;
        public float BulletBaseSpread = 0f;
        public float BulletMaxSpread = 0f;
        public float BulletSpreadIncrease = 0f;
        public BulletController Bullet = null;
        public Transform Ray = null;
        public Transform Model = null;
        public WeaponType WeaponType = WeaponType.Projectile;
		//Esteban ---Ammo
		public float AmmoPerPickUp = 10f;
    }
}

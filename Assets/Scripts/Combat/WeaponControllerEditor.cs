﻿#if UNITY_EDITOR
using Combat;
using UnityEditor;
using UnityEngine;

namespace Combat
{
    [CustomEditor(typeof(WeaponController))]
    [CanEditMultipleObjects]
    public class WeaponControllerEditor : Editor {
        SerializedProperty _cooldown;
        SerializedProperty _maxShotDistance;
        SerializedProperty _autoFire;
        SerializedProperty _resetCoolown;
        SerializedProperty _damage;
        SerializedProperty _bulletSpeed;
        SerializedProperty _bulletsPerShot;
        SerializedProperty _bulletBaseSpread;
        SerializedProperty _bulletMaxSpread;
        SerializedProperty _bulletSpreadIncrease;
        SerializedProperty _bullet;
        SerializedProperty _ray;
        SerializedProperty _model;
        SerializedProperty _weaponType;
        SerializedProperty _id;
        SerializedProperty _sound;
        SerializedProperty _color;
        SerializedProperty _image;
		
		//Esteban --- Ammo
		SerializedProperty _ammoPerPickUp;

        void OnEnable()
        {
            _cooldown = serializedObject.FindProperty("Cooldown");
            _maxShotDistance = serializedObject.FindProperty("MaxShotDistance");
            _autoFire = serializedObject.FindProperty("AutoFire");
            _damage = serializedObject.FindProperty("Damage");
            _resetCoolown = serializedObject.FindProperty("ResetCoolown");
            _bulletSpeed = serializedObject.FindProperty("BulletSpeed");
            _bulletsPerShot = serializedObject.FindProperty("BulletsPerShot");
            _bulletBaseSpread = serializedObject.FindProperty("BulletBaseSpread");
            _bulletMaxSpread = serializedObject.FindProperty("BulletMaxSpread");
            _bulletSpreadIncrease = serializedObject.FindProperty("BulletSpreadIncrease");
            _bullet = serializedObject.FindProperty("Bullet");
            _ray = serializedObject.FindProperty("Ray");
            _model = serializedObject.FindProperty("Model");
            _weaponType = serializedObject.FindProperty("WeaponType");
            _id = serializedObject.FindProperty("Id");
            _sound = serializedObject.FindProperty("Sound");
			_ammoPerPickUp = serializedObject.FindProperty("AmmoPerPickUp");
            _image = serializedObject.FindProperty("Image");
            _color = serializedObject.FindProperty("Color");
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            EditorGUILayout.PropertyField(_id);

            EditorGUILayout.PropertyField(_weaponType);
            EditorGUILayout.Space();

			EditorGUILayout.PropertyField(_ammoPerPickUp);
            EditorGUILayout.PropertyField(_cooldown);
            EditorGUILayout.PropertyField(_damage);
            EditorGUILayout.PropertyField(_maxShotDistance);
            EditorGUILayout.PropertyField(_autoFire);
            EditorGUILayout.PropertyField(_resetCoolown);
            EditorGUILayout.PropertyField(_model);
            EditorGUILayout.PropertyField(_sound);
            EditorGUILayout.PropertyField(_color);
            EditorGUILayout.PropertyField(_image);
            EditorGUILayout.Space();
            
            switch ((WeaponType) _weaponType.enumValueIndex)
            {
                case WeaponType.HitScan:
                    EditorGUILayout.PropertyField(_ray);
                    break;

                case WeaponType.Projectile:
                    EditorGUILayout.PropertyField(_bullet);
                    EditorGUILayout.PropertyField(_bulletSpeed);
                    EditorGUILayout.PropertyField(_bulletsPerShot);
                    EditorGUILayout.PropertyField(_bulletBaseSpread);
                    EditorGUILayout.PropertyField(_bulletMaxSpread);
                    EditorGUILayout.PropertyField(_bulletSpreadIncrease);
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif
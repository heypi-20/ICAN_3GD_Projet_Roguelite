using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Simple : MonoBehaviour
{
    [Header("Références")]
    public Transform weaponMuzzle; // Point de sortie du projectile
    public GameObject projectilePrefab; // Préfabriqué du projectile

    [Header("Propriétés du tir")]
    public float cooldown = 0.5f; // Temps entre chaque tir

    public GameObject Jump_Noeud;

    private float lastShootTime; // Temps du dernier tir
    public GameObject cross;

    void Update()
    {
        // Détection de l'entrée utilisateur
        if (Input.GetMouseButton(0) && Time.time > lastShootTime + cooldown && GameManager.instance.energyPoints >= 0.1)
        {
            Shoot();
            lastShootTime = Time.time;
            GameManager.instance.AddEnergyPointOnShoot();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Instantiate(Jump_Noeud,weaponMuzzle.position, weaponMuzzle.rotation);
        }

        if (Input.GetMouseButtonDown(1))
        {
            cross.gameObject.SetActive(true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            cross.gameObject.SetActive(false);
        }
    }

    void Shoot()
    {
        // Instancier le projectile à l'emplacement du muzzle avec son orientation
        GameObject projectile = Instantiate(projectilePrefab, weaponMuzzle.position, weaponMuzzle.rotation);

        Debug.Log("Projectile tiré !");
    }
}

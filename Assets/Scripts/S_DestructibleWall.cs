﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_DestructibleWall : MonoBehaviour
{
    public string targetTag = "Player"; // Tag de l'objet qui déclenche la désactivation du mur
    public string rangeTag = "Enemy"; // Tag pour la détection dans la portée
    public GameObject destructionEffect; // L'effet de particule lors de la désactivation du mur
    public GameObject resetPrefab; // Le prefab à générer lors de la réinitialisation
    [Space(20)]
    [Header("Paramètres de gameplay")]
    public int regrowThreshold = 3; // Nombre de réinitialisations avant la régénération du mur
    

    private Vector3 destroyedWallPosition;
    private bool hasResetPrefab = false;
    private int currentResetRound = 0;

    private GameObject activeEffect;
    private GameObject wallChild; // Référence au sous-objet (mur)
    private Collider wallCollider; // Collider du sous-objet (mur)
    private float detectionInterval = 0.1f; // Fréquence de la détection de la portée
    private float nextDetectionTime = 0f; // Prochaine fois pour la détection

    // S'abonner à l'événement de réinitialisation lors de l'activation
    void OnEnable()
    {
        S_ZoneResetSysteme.OnZoneReset += ResetDestroyedWall;
    }

    // Se désabonner de l'événement de réinitialisation lors de la désactivation
    void OnDisable()
    {
        S_ZoneResetSysteme.OnZoneReset -= ResetDestroyedWall;
    }

    void Start()
    {
        // Assigner le premier enfant comme étant le mur à désactiver/réactiver
        if (transform.childCount > 0)
        {
            wallChild = transform.GetChild(0).gameObject;
            wallCollider = wallChild.GetComponent<Collider>(); // Récupérer le collider du mur
        }
    }

    void Update()
    {
        // Appeler la détection de portée à intervalles réguliers
        if (Time.time >= nextDetectionTime && wallChild.activeSelf)
        {
            DetectInRange();
            nextDetectionTime = Time.time + detectionInterval; // Mettre à jour l'heure de la prochaine détection
        }
    }

    // Détection dans la portée autour du mur
    private void DetectInRange()
    {
        if (wallCollider == null) return;

        // Utiliser un box collider pour créer une zone de détection basée sur la taille du collider du mur
        Collider[] hitColliders = Physics.OverlapBox(wallCollider.bounds.center, wallCollider.bounds.extents, Quaternion.identity);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(rangeTag))
            {
                Debug.Log("Un objet avec le tag 'Enemy' a été détecté dans la portée.");
                DisableWall();
                return; // Arrêter de vérifier si un objet est trouvé
            }
        }
    }

    // Déclenché lors de la collision avec un objet ayant le bon tag
    void OnCollisionEnter(Collision collision)
    {
        // Vérifier si l'objet en collision a le bon tag
        if (collision.gameObject.CompareTag(targetTag))
        {
            
            // Vérifier si le collider en collision appartient bien au sous-objet (mur)
            if (collision.contacts[0].thisCollider.gameObject == wallChild)
            {
                // Désactiver le sous-objet (mur) et enregistrer sa position
                DisableWall();
                Debug.Log("Le sous-objet (mur) a été désactivé.");
            }
            Destroy(collision.gameObject);
        }
    }

    // Désactiver le mur et générer un effet de particule
    private void DisableWall()
    {
        if (wallChild == null) return;

        destroyedWallPosition = wallChild.transform.position;

        // Désactiver le sous-objet (mur) au lieu de le détruire
        wallChild.SetActive(false);

        // Générer les particules si nécessaire
        if (destructionEffect != null)
        {
            Destroy(activeEffect);
            activeEffect = Instantiate(destructionEffect, destroyedWallPosition, Quaternion.identity,this.transform);
        }
    }

    // Réinitialiser le sous-objet désactivé
    public void ResetDestroyedWall()
    {
        // Ignorer si le mur n'est pas désactivé
        if (wallChild.activeSelf)
        {
            Debug.Log("Le mur est déjà actif, réinitialisation ignorée.");
            return;
        }

        currentResetRound++;

        // Générer le resetPrefab uniquement la première fois
        if (!hasResetPrefab)
        {
            Instantiate(resetPrefab, destroyedWallPosition, Quaternion.identity);
            hasResetPrefab = true;
        }

        // Si le mur doit être réactivé après un certain nombre de réinitialisations
        if (currentResetRound >= regrowThreshold)
        {
            // Réactiver le sous-objet (mur)
            wallChild.SetActive(true);

            // Détruire l'effet de particule si présent
            if (activeEffect != null)
            {
                Destroy(activeEffect);
                 
            }

            // Réinitialiser les variables
            currentResetRound = 0;
            hasResetPrefab = false; // Réinitialiser pour permettre la génération de resetPrefab à nouveau
        }
    }

    // Visualiser la zone de détection avec des Gizmos
    private void OnDrawGizmos()
    {
        if (wallCollider != null)
        {
            Gizmos.color = Color.red; // Définir la couleur du Gizmo
            Gizmos.DrawWireCube(wallCollider.bounds.center, wallCollider.bounds.size); // Dessiner le Gizmo autour du mur
        }
    }
}
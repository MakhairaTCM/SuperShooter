using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject weaponPanel;
    public Button[] choiceButtons;
    public List<GameObject> allWeaponPrefabs; // Glisse tes 5 prefabs ici dans l'inspector
    public Transform player;

    void Start()
    {
        // Pause le jeu au début
        Time.timeScale = 0;
        weaponPanel.SetActive(true);
        SetupChoices();
    }

    void SetupChoices()
    {
        // Mélange la liste et prend les 3 premiers (pour l'exemple simple)
        // Dans un vrai jeu, utilise une copie de liste pour ne pas perdre l'ordre

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int index = i; // Copie locale pour le bouton
            GameObject weaponToGive = allWeaponPrefabs[Random.Range(0, allWeaponPrefabs.Count)];

            // Change le texte du bouton
            choiceButtons[i].GetComponentInChildren<Text>().text = weaponToGive.GetComponent<WeaponBase>().weaponName;

            // Ajoute l'événement au clic
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => SelectWeapon(weaponToGive));
        }
    }

    void SelectWeapon(GameObject weaponPrefab)
    {
        // Instancie l'arme comme enfant du joueur
        GameObject newWeapon = Instantiate(weaponPrefab, player.position, Quaternion.identity);
        newWeapon.transform.SetParent(player);

        // Lance le jeu
        weaponPanel.SetActive(false);
        Time.timeScale = 1;
    }
}
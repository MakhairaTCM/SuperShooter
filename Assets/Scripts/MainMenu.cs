using UnityEngine;
using UnityEngine.SceneManagement; // Nécessaire pour changer de scène
using UnityEngine.UI; // Nécessaire pour toucher aux InputField

public class MainMenu : MonoBehaviour
{
    public InputField nameInput;

    void Start()
    {
        // Au démarrage, on regarde si un pseudo existe déjà
        if (PlayerPrefs.HasKey("Pseudo"))
        {
            // On remplit le champ automatiquement
            nameInput.text = PlayerPrefs.GetString("Pseudo");
        }
    }

    public void PlayGame()
    {
        // 1. On sauvegarde le pseudo (si le champ n'est pas vide)
        if (!string.IsNullOrEmpty(nameInput.text))
        {
            PlayerPrefs.SetString("Pseudo", nameInput.text);
            PlayerPrefs.Save(); // Force la sauvegarde sur le disque
        }
        else
        {
            // Si pas de pseudo, on met un défaut
            PlayerPrefs.SetString("Pseudo", "Survivor");
        }

        // 2. On charge la scène de jeu (Assure-toi qu'elle s'appelle bien "Game")
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Debug.Log("Fermeture du jeu...");
        Application.Quit();
    }
}
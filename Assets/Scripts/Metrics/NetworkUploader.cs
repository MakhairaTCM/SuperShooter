using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class NetworkUploader : MonoBehaviour
{
    [Header("Configuration Serveur")]
    // Remplace 'vsapo' par ton nom de compte exact s'il est différent
    // L'URL doit pointer vers ton fichier index.php
    public string uploadUrl = "https://vsapo.alwaysdata.net/api/index.php";

    public void SendDataToDatabase(GameSessionData data)
    {
        // On lance la tâche de fond (Coroutine) pour ne pas bloquer le jeu
        StartCoroutine(PostRequest(data));
    }

    IEnumerator PostRequest(GameSessionData data)
    {
        // 1. On transforme tes données C# en texte JSON
        string json = JsonUtility.ToJson(data);

        // 2. Préparation de la requête Web
        // On utilise UnityWebRequest qui est le standard actuel
        var request = new UnityWebRequest(uploadUrl, "POST");

        // On prépare le corps du message (le JSON) en format binaire
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer(); // Pour lire la réponse du serveur

        // TRES IMPORTANT : Dire au PHP "Hey, je t'envoie du JSON !"
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log(" Envoi des données vers la BDD en cours...");

        // 3. On envoie et on attend la réponse
        yield return request.SendWebRequest();

        // 4. Vérification du résultat
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(" Erreur d'envoi : " + request.error);
            Debug.LogError("Réponse serveur : " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log(" SUCCÈS ! Données enregistrées en base.");
            Debug.Log("Réponse du serveur : " + request.downloadHandler.text);
        }
    }
}
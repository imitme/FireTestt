using Firebase;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Auth : MonoBehaviour
{
    private void Awake()
    {
#if !UNITY_EDITOR
 CheckGooglePlayService();
#else
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://firetestt-5ca1a.firebaseio.com/");
        Next();
#endif
    }

    private void CheckGooglePlayService()
    {
        try
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result; if (dependencyStatus == DependencyStatus.Available)
                {
                    Next();
                }
                else
                {
                    Debug.LogError(
                   System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    Invoke("Quit", 3f);
                }
            });
        }
        catch
        {
            Application.Quit();
        }
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void Next()
    {
        SceneManager.LoadScene("Main");
    }
}
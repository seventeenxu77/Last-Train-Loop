using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Skipper : MonoBehaviour
{
    public void Skip()
    {
        SceneManager.LoadScene("mainscene");
    }
}

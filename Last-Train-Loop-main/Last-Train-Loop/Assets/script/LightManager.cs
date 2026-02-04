using UnityEngine;

public class LightManager : MonoBehaviour
{
    Light[] allLights;
    [SerializeField]public Light playerPointLight, cameraLight;
    private void Awake()
    {
        allLights = FindObjectsOfType<Light>();
    }
    public void TurnOffAllLights()
    {
        foreach (Light light in allLights)
        {
            light.enabled = false; 
        }
        cameraLight.enabled = true;
    }
    public void TurnOnAllLights()
    {
        foreach (Light light in allLights)
        {
            light.enabled = true; 
        }
        playerPointLight.enabled = false;
        cameraLight.enabled = false;
    }
}

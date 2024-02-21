using System.Collections;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    private static WeatherManager _instance;
    public static WeatherManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<WeatherManager>();
            return _instance;
        }
    }

    [SerializeField] private Weather[] weather;
    [SerializeField] private Camera mainCam;

    private int currentWeather = 0;

    [HideInInspector] public Material currentSkybox;

    void Start()
    {
        currentSkybox = weather[currentWeather].skybox;
        StartCoroutine(ChangeWeather());   
    }

    IEnumerator ChangeWeather()
    {
        yield return new WaitForSeconds(weather[currentWeather].duration);

        foreach (Weather feature in weather)
        {
            if (feature.hasAdditional && feature.additionalFeature != null)
                feature.additionalFeature.SetActive(false);
        }

        currentWeather = Random.Range(0, weather.Length);

        Debug.Log("<color=cyan>WEATHER SET TO: " + weather[currentWeather].name + "</color>");

        currentSkybox = weather[currentWeather].skybox;
        RenderSettings.skybox = currentSkybox;

        if (weather[currentWeather].hasAdditional)
            weather[currentWeather].additionalFeature.SetActive(true);

        StartCoroutine(ChangeWeather());   
    }
}

[System.Serializable]
public class Weather {
    public string name;
    public Material skybox;
    public int duration  = 0;

    public bool hasAdditional = false;
    public GameObject additionalFeature = null;
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

/// <summary>
/// Takes care of the settings screen
/// </summary>
public class Settings : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] Slider quality = null;
    [SerializeField] TextMeshProUGUI qualityText = null;

    [SerializeField] Slider resolution = null;
    [SerializeField] TextMeshProUGUI resolutionText = null;

    [SerializeField] Toggle windowed = null, vsync = null;

    [Header("Audio")] [Space(20)]
    [SerializeField] AudioMixer audioMixer = null;
    [SerializeField] Slider master = null, sfx = null, bgm = null;

    SettingsInformation currentSettings;

    const string FILENAME = "Settings.xml";

    private void Awake()
    {
        resolution.maxValue = Screen.resolutions.Length - 1;
        SettingsInformation oldSettings = SaveManager.XmlLoad<SettingsInformation>(FILENAME);

        if (oldSettings != default) //If there is any old settings saved on the computer
        {
            UpdateSettings(oldSettings);
            currentSettings = oldSettings;
        }
        else //Default values
        {
            quality.value = QualitySettings.GetQualityLevel();
            qualityText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
            resolution.value = Screen.resolutions.Length - 1;
            resolutionText.text = Screen.resolutions[(int)resolution.value].ToString().Split('@')[0];
            windowed.isOn = false;
            vsync.isOn = false;

            SetSlider("MasterVol", master);
            SetSlider("SFXVol", sfx);
            SetSlider("BGMVol", bgm);
            currentSettings = GetCurrentSettings();
        }
    }

    /// <summary>
    /// Sets slider value from mixer volume
    /// </summary>
    /// <param name="mixerString">Name of mixer part</param>
    /// <param name="slider">The slider to set value on</param>
    void SetSlider(string mixerString, Slider slider)
    {
        float value;
        audioMixer.GetFloat(mixerString, out value);
        slider.value = value;
    }

    public void Open()
    {
        EventSystem.current.SetSelectedGameObject(quality.gameObject);
        quality.OnSelect(null);
    }

    #region UI methods

    //GRAPHICS

    public void SetQualityLevel(float f)
    {
        QualitySettings.SetQualityLevel((int)f, true);
        qualityText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
    }

    public void SetResolution(float f)
    {
        Resolution res = Screen.resolutions[(int)f];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        resolutionText.text = res.ToString().Split('@')[0];
    }

    public void SetWindowedMode(bool b)
    {
        Screen.fullScreen = !b;
    }

    public void SetVSync(bool b)
    {
        QualitySettings.vSyncCount = b ? 1 : 0;
    }

    //VOLUME

    public void SetMasterVolume(float f)
    { 
        audioMixer.SetFloat("MasterVol", f);
    }

    public void SetSFXVolume(float f)
    {
        audioMixer.SetFloat("SFXVol", f);
    }

    public void SetBGMVolume(float f)
    {
        audioMixer.SetFloat("BGMVol", f);
    }

    /// <summary>
    /// Saves the chosen settings to file
    /// </summary>
    public void Apply()
    {
        SettingsInformation newSettings = GetCurrentSettings();
        SaveManager.XmlSave(newSettings, FILENAME);
        currentSettings = newSettings;
        MainMenu.Instance.BackToMain();
    }

    /// <summary>
    /// Returns settings to as they were
    /// </summary>
    public void Discard()
    {
        UpdateSettings(currentSettings);
        MainMenu.Instance.BackToMain();
    }

    #endregion

    /// <summary>
    /// Sets all settings and UI elements to match
    /// </summary>
    /// <param name="settings">Settings to set everything to</param>
    void UpdateSettings(SettingsInformation settings)
    {
        QualitySettings.SetQualityLevel(settings.QualityLevel, true);
        quality.value = settings.QualityLevel;
        qualityText.text = QualitySettings.names[settings.QualityLevel];

        Resolution res = Screen.resolutions[settings.Resolution];
        Screen.SetResolution(res.width, res.height, settings.Windowed);
        resolution.value = settings.Resolution;

        Screen.fullScreen = !settings.Windowed;
        windowed.isOn = settings.Windowed;

        QualitySettings.vSyncCount = settings.VSync ? 1 : 0;
        vsync.isOn = settings.VSync;  

        audioMixer.SetFloat("MasterVol", settings.MasterVolume);
        master.value = settings.MasterVolume;

        audioMixer.SetFloat("SFXVol", settings.SFXVolume);
        sfx.value = settings.SFXVolume;

        audioMixer.SetFloat("BGMVol", settings.BGMVolume);
        bgm.value = settings.BGMVolume;
    }

    /// <summary>
    /// Returns the current settings information from UI elements
    /// </summary>
    SettingsInformation GetCurrentSettings()
    {
        return new SettingsInformation(
            (int)quality.value,
            (int)resolution.value,
            windowed.isOn,
            vsync.isOn,
            master.value,
            sfx.value,
            bgm.value);
    }

    /// <summary>
    /// Class for settings to be saved/loaded
    /// </summary>
    [System.Serializable]
    public class SettingsInformation
    {
        public int QualityLevel { get; set; }
        public int Resolution { get; set; }
        public bool Windowed { get; set; }
        public bool VSync { get; set; }
        
        public float MasterVolume { get; set; }
        public float SFXVolume { get; set; }
        public float BGMVolume { get; set; }

        public SettingsInformation()
        {
            QualityLevel = 5;
            Resolution = 0;
            Windowed = false;
            VSync = false;

            MasterVolume = 0;
            SFXVolume = 0;
            BGMVolume = 0;
        }

        public SettingsInformation(int quality, int resolution, bool windowed, bool vsync, float master, float sfx, float bgm)
        {
            QualityLevel = quality;
            Resolution = resolution;
            Windowed = windowed;
            VSync = vsync;

            MasterVolume = master;
            SFXVolume = sfx;
            BGMVolume = bgm;
        }
    }
}

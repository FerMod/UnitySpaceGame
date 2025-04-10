using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace SpaceGame.UI
{
    public static class PlayerPrefsKeys
    {
        public const string MouseSensitivity = "MouseSensitivity";
        public const string MasterVolume = "MasterVolume";
        public const string MusicVolume = "MusicVolume";
        public const string EffectsVolume = "EffectsVolume";
    }

    public static class AudioMixerParameters
    {
        public const string MasterVolume = "Master";
        public const string MusicVolume = "Music";
        public const string EffectsVolume = "Effects";
    }

    public class OptionsMenu : MonoBehaviour
    {
        public GameObject menu;
        public GameObject previousMenu;

        [Header("Controls")]
        public MouseFlightController mouseFlightController;
        public Slider mouseSensitivity;

        [Header("Sound")]
        public AudioMixer audioMixer;

        public Slider masterSlider;
        public Slider musicSlider;
        public Slider effectsSlider;

        private void Start()
        {
            //#if UNITY_EDITOR
            //            PlayerPrefs.DeleteAll();
            //#endif

            LoadData();

            // Controls
            mouseSensitivity.onValueChanged.AddListener(SetMouseSensitivity);

            // Sound
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
        }

        private void OnDestroy()
        {
            // Controls
            if (mouseSensitivity != null)
            {
                mouseSensitivity.onValueChanged.RemoveListener(SetMouseSensitivity);
            }

            // Sound
            if (masterSlider != null)
            {
                masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
            }
            if (musicSlider != null)
            {
                musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
            }
            if (effectsSlider != null)
            {
                effectsSlider.onValueChanged.RemoveListener(SetEffectsVolume);
            }
        }

        #region Controls        
        public void SetMouseSensitivity(float value)
        {
            if (mouseFlightController == null) return;

            mouseFlightController.MouseSensitivity = value;
        }
        #endregion

        #region Sound
        public void SetMasterVolume(float value)
        {
            audioMixer.SetFloat("Master", ValueToDecibels(value));
        }

        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat("Music", ValueToDecibels(value));
        }

        public void SetEffectsVolume(float value)
        {
            audioMixer.SetFloat("Effects", ValueToDecibels(value));
        }
        #endregion

        public void OnBackPressed()
        {
            SaveData();

            menu.SetActive(false);
            previousMenu.SetActive(true);
        }

        #region Load/Save
        private void LoadData()
        {
            LoadControlsData();
            LoadSoundConfig();
        }

        private void SaveData()
        {
            SaveControlsData();
            SaveSoundConfig();
        }

        public void SaveControlsData()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MouseSensitivity, mouseSensitivity.value);
        }

        public void LoadControlsData()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.MouseSensitivity)) return;

            mouseSensitivity.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.MouseSensitivity, mouseFlightController != null ? mouseFlightController.MouseSensitivity : 0f);
        }

        public void SaveSoundConfig()
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MasterVolume, masterSlider.value);
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume, musicSlider.value);
            PlayerPrefs.SetFloat(PlayerPrefsKeys.EffectsVolume, effectsSlider.value);
        }

        public void LoadSoundConfig()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.MasterVolume))
            {
                masterSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.MasterVolume);
                SetMasterVolume(masterSlider.value);
            }
            else
            {
                audioMixer.GetFloat("Master", out var dbVolume);
                masterSlider.value = DecibelsToValue(dbVolume);
            }

            if (PlayerPrefs.HasKey(PlayerPrefsKeys.MusicVolume))
            {
                musicSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume);
                SetMusicVolume(musicSlider.value);
            }
            else
            {
                audioMixer.GetFloat("Music", out var dbVolume);
                musicSlider.value = DecibelsToValue(dbVolume);
            }

            if (PlayerPrefs.HasKey(PlayerPrefsKeys.EffectsVolume))
            {
                effectsSlider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.EffectsVolume);
                SetEffectsVolume(effectsSlider.value);
            }
            else
            {
                audioMixer.GetFloat("Effects", out var dbVolume);
                effectsSlider.value = DecibelsToValue(dbVolume);
            }
        }
        #endregion

        private float ValueToDecibels(float value)
        {
            return value > 0 ? Mathf.Log10(value) * 20 : -80f;
        }
        private float DecibelsToValue(float decibels)
        {
            var value = Mathf.Pow(10, decibels / 20);
            return Mathf.Clamp(value, 0f, 1f);
        }
    }

}

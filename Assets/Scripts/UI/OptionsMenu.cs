using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

namespace SpaceGame.UI
{

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
            audioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
        }

        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat("Music", Mathf.Log10(value) * 20);
        }
        public void SetEffectsVolume(float value)
        {
            audioMixer.SetFloat("Effects", Mathf.Log10(value) * 20);
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
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity.value);
        }

        public void LoadControlsData()
        {
            if (!PlayerPrefs.HasKey("MouseSensitivity")) return;

            mouseSensitivity.value = PlayerPrefs.GetFloat("MouseSensitivity", mouseFlightController != null ? mouseFlightController.MouseSensitivity : 0f);
        }

        public void SaveSoundConfig()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
            PlayerPrefs.SetFloat("EffectsVpolume", effectsSlider.value);
        }

        public void LoadSoundConfig()
        {
            if (!PlayerPrefs.HasKey("MasterVolume")) return;

            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            effectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume");
        }
        #endregion
    }
}

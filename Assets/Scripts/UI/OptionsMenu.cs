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

        [Space(16)]
        //public MouseFlightController mouseFlightController;

        [Header("Menu")]
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
            mouseSensitivity.onValueChanged.AddListener(OnMouseSensitivityChange);

            // Sound
            masterSlider.onValueChanged.AddListener(OnMasterSoundChange);
            musicSlider.onValueChanged.AddListener(OnMusicSoundChange);
            effectsSlider.onValueChanged.AddListener(OnEffectsSoundChange);
        }

        private void OnDestroy()
        {
            // Controls
            mouseSensitivity?.onValueChanged.RemoveListener(OnMouseSensitivityChange);

            // Sound
            masterSlider?.onValueChanged.RemoveListener(OnMasterSoundChange);
            musicSlider?.onValueChanged.RemoveListener(OnMusicSoundChange);
            effectsSlider?.onValueChanged.RemoveListener(OnEffectsSoundChange);
        }

        #region Controls
        public void OnMouseSensitivityChange(float value)
        {
            //mouseFlightController.MouseSensitivity = value;
        }
        #endregion

        #region Sound
        public void OnMasterSoundChange(float value)
        {
            audioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
        }

        public void OnMusicSoundChange(float value)
        {
            audioMixer.SetFloat("Music", Mathf.Log10(value) * 20);
        }
        public void OnEffectsSoundChange(float value)
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

        private void LoadData()
        {
            //mouseSensitivity.value = PlayerPrefs.GetFloat("MouseSensitivity", mouseFlightController.MouseSensitivity);
            PlayerPrefs.GetFloat("MouseSensitivity");

            LoadSoundConfig();
        }

        private void SaveData()
        {
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity.value);

            SaveSoundConfig();
        }

        public void SaveControlsData()
        {
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity.value);
        }

        public void LoadControlsData()
        {
            if (!PlayerPrefs.HasKey("MouseSensitivity")) return;
            mouseSensitivity.value = PlayerPrefs.GetFloat("MouseSensitivity");
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
    }
}

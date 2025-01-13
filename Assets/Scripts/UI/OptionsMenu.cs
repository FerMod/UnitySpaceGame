using System;
using UnityEditor;
using UnityEngine;
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

        private void Start()
        {
            LoadData();

            mouseSensitivity.onValueChanged.AddListener(OnMouseSensitivityChange);
        }

        private void OnDestroy()
        {
            mouseSensitivity?.onValueChanged.RemoveListener(OnMouseSensitivityChange);
        }

        public void OnMouseSensitivityChange(float value)
        {
            //mouseFlightController.MouseSensitivity = value;
        }

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
        }

        private void SaveData()
        {
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity.value);
        }
    }
}

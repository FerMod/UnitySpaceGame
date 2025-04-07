
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour
{
    // https://prasetion.medium.com/manage-audio-with-audio-mixer-in-unity-676c08467e60
    public AudioMixer masterMixer;

    public Slider sliderMaster;
    public Slider sliderMusica;
    public Slider sliderEfectos;


    private void Start()
    {
        CargarConfiguracion();
    }

    public void SetMaster(float soundLevel)
    {
        masterMixer.SetFloat("Master", Mathf.Log(soundLevel) * 20);
    }

    public void SetMusica(float soundLevel)
    {
        masterMixer.SetFloat("Musica", Mathf.Log(soundLevel) * 20);
    }

    public void SetEfectos(float soundLevel)
    {
        masterMixer.SetFloat("Efectos", Mathf.Log(soundLevel) * 20);
    }


    public void GuardarConfiguracion()
    {
        PlayerPrefs.SetFloat("VolMaster", sliderMaster.value);
        PlayerPrefs.SetFloat("VolMusica", sliderMusica.value);
        PlayerPrefs.SetFloat("VolEfectos",sliderEfectos.value);
    }

    public void CargarConfiguracion()
    {
        if (PlayerPrefs.HasKey("VolMaster")) // hay una conf guardada
        {
            sliderMaster.value = PlayerPrefs.GetFloat("VolMaster");
            sliderMusica.value = PlayerPrefs.GetFloat("VolMusica");
            sliderEfectos.value = PlayerPrefs.GetFloat("VolEfectos");
        }
        else // no hay conf guardada
        {

        }
    }

}

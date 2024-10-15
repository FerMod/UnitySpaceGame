using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vector3UIField : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Text x;
    [SerializeField] private Text y;
    [SerializeField] private Text z;


    public void OnValueChanged(Vector3 value)
    {
        x.text = ((int)value.x).ToString();
        y.text = ((int)value.y).ToString();
        z.text = ((int)value.z).ToString();
    }
}

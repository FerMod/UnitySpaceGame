using UnityEngine;
using UnityEngine.UI;

public class ValueUIField : MonoBehaviour
{
    [SerializeField] private Text field;

    public void OnValueChanged(string value)
    {
        field.text = value;
    }
}

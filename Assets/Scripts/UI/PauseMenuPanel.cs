using TMPro;
using UnityEngine;

public class PauseMenuPanel : MonoBehaviour
{

    [SerializeField]
    private TMP_Text menuName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeMenuName(string value)
    {
        menuName.text = $"—  {value}  —";
    }

}

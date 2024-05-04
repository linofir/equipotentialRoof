using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateInfo : MonoBehaviour
{
    public string projectName;
    private TextMeshProUGUI textMeshdPro;
    // Start is called before the first frame update
    void Start()
    {
        textMeshdPro = GetComponent<TextMeshProUGUI>();
        textMeshdPro.text = $"Projeto {projectName}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

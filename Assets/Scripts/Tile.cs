using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour{
	[SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    // [SerializeField] public TextMeshPro prob;
    //public GameObject uiSceneText;
    //public TextMeshProUGUI textMeshProUGUI;
    // [SerializeField] private Color _clickColor;

 
    public void Init(bool isOffset) {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
        // prob = GetComponent<TextMeshPro>();
    }
 
    void OnMouseEnter() {
        _highlight.SetActive(true);
    }
 
    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    void OnMouseDown(){
        // Change the color based on the probabilities
        _highlight.SetActive(false);
        _renderer.color = Color.yellow;
        //uiSceneText.SetActive(true);
        //textMeshProUGUI.text = "0.1";
        // _renderer.material.SetColor("_Color", Color.red);
    }
}

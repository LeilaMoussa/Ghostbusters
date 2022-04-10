using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public class Tile : MonoBehaviour{
	[SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
 
    public void Init(bool isOffset) {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
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
        TextMesh text = UtilsClass.CreateWorldText("0.01", null, _renderer.transform.position, 40, Color.black, TextAnchor.MiddleCenter);
        text.transform.localScale = new Vector3((float)0.1,(float)0.1,(float)0.1);
    }
}

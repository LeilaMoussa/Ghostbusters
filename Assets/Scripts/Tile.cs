using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;
using UnityEngine.EventSystems;
using static Game.GamePlay;

public class Tile : MonoBehaviour{  // , IPointerClickHandler
	[SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] public TextMesh proba = null;
    private int clickCount = 0;

    public void Init(bool isOffset) {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }
 
    public void OnMouseEnter() {
        _highlight.SetActive(true);
    }
 
    public void OnMouseExit(){
        _highlight.SetActive(false);
    }

    public void OnMouseDown(){
        // Change the color based on the probabilities
        clickCount++;
        if(clickCount == 1){
            _highlight.SetActive(false);
            _renderer.color = Color.yellow;
            // Here we should get the probability to be displayed, then send it to SetProba()
            SetProba(0.5);
            // UpdatePosteriorProbabilities(_renderer.transform.position, this);
        }
        else if(clickCount == 2){
            Debug.Log("You busted the position " + _renderer.transform.position);
            _renderer.color = Color.red;
        }
    }

    public void SetProba(double proba){
        double percent = proba*100;
        string percentProba = percent + "%";
        if(this.proba != null){
            // Update the probability
            this.proba.text = percentProba;
        }
        else{
            Debug.Log(_renderer.transform.position);
            // Initialize the probability
            this.proba = UtilsClass.CreateWorldText(percentProba, null, _renderer.transform.position, 30, Color.black, TextAnchor.MiddleCenter);
            this.proba.transform.localScale = new Vector3((float)0.1,(float)0.1,(float)0.1);
        }
    }
}

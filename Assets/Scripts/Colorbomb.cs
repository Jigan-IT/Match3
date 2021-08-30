using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colorbomb : MonoBehaviour {
    
    public SpriteRenderer red,green,blue, yellow, purple;
    public GameObject bob;
    //bob.GetComponent<BoardItem>().id
    //this gameObject id;
    // Use this for initialization
    void Start () {
        
        
       
	}
	
	// Update is called once per frame
	public void Bomba (int id) {
        switch (bob.GetComponent<BoardItem>().id)
        {
            case 0:

                bob.GetComponent<SpriteRenderer>().sprite = blue.sprite;
                break;
            case 1:

                bob.GetComponent<SpriteRenderer>().sprite  = green.sprite;
                break;
            case 2:

                bob.GetComponent<SpriteRenderer>().sprite  = purple.sprite;
                break;
            case 3:

                bob.GetComponent<SpriteRenderer>().sprite  = red.sprite;
                break;
            case 4:

                bob.GetComponent<SpriteRenderer>().sprite = yellow.sprite;
                break;
            default:

                break;
        }
        
    }
}

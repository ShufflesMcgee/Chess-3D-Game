using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardHighlights : MonoBehaviour 
{
	public static boardHighlights Instance {set; get;}

	public GameObject highlightPrefab;

	private List<GameObject> highlights;

	private float offSet = 0.5f;

	private void Start()
		{
			Instance = this;
			highlights = new List<GameObject> ();
		}

	private GameObject getHightlightObject()
		{
			GameObject go = highlights.Find(g => !g.activeSelf);

			if(go == null)
				{
					go = Instantiate(highlightPrefab);
					highlights.Add(go);
				}

				return go;
		}

	public void highlightAllowedMoves(bool[,] moves)
		{
			for(int i = 0; i < 8; i++)
				{
					for(int j = 0; j < 8; j++)
						{
							if(moves[i,j])
								{
									GameObject go = getHightlightObject();
									go.SetActive(true);
									go.transform.position = new Vector3(i + offSet, 0.005f, j + offSet);
								}
						}
				}
		}

	public void hideHighlights()
		{
			foreach(GameObject go in highlights)
				go.SetActive(false);
		}
}

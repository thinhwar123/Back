using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private bool draw;
    [SerializeField] private bool newDraw;
    [SerializeField] private TileBase ruleTile;

    [SerializeField] private int wallLength;
    [SerializeField] private int wallWidth;
    [SerializeField] private int wallThick;


    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject groundGrid;

    public void OnDrawGizmos()
    {
        if (!draw && groundGrid == null)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(wallLength, wallWidth, 0));
        }
        else if(draw && groundGrid == null)
        {
            groundGrid = Instantiate(groundPrefab, transform.position, Quaternion.identity);
            for (int i = 0; i < wallLength; i++)
            {
                for (int j = 0; j < wallWidth; j++)
                {
                    if (i < wallThick || j < wallThick || i >= wallLength - wallThick || j >= wallWidth - wallThick)
                    {
                        groundGrid.GetComponentInChildren<Tilemap>().SetTile(new Vector3Int(i - wallLength / 2, j - wallWidth / 2, 0), ruleTile);
                    }
                    
                }
            }

        }
        else if(!draw && groundGrid != null)
        {
            DestroyImmediate(groundGrid);
            groundGrid = null;
        }
        else if (newDraw)
        {
            groundGrid = null;
            draw = false;
        }
    }
}

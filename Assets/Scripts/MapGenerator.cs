﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour {

    [SerializeField]
    private int width;
    [SerializeField]
    private int height;

    public int borderSize = 1;

    [SerializeField]
    [Range(0,100)]
    public int randomFillPercent;

    public string seed;
    public bool useRandomSeed;

    int[,] map;

	void Start () {
        GenerateMap();

    }
	
	void Update () {
        randomFillPercent = GameControl.entity.fillPercentage;
        borderSize = GameControl.entity.borderSize;
        seed = GameControl.entity.seed;
        useRandomSeed = GameControl.entity.randomSeed;

        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
	}

    private void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                {
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                }
                else
                {
                    borderedMap[x, y] = 1;
                }
            }
        }

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(borderedMap, 1);
    }

    private void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i == 0|| i == width-1 ||j ==0||j ==height - 1)
                {
                    map[i, j] = 1;
                }else
                    map[i, j] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }

    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neightbourWallTiles = GetSurroundingWallCount(x, y);

                if (neightbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neightbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

   
}

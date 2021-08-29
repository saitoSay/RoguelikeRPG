﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [SerializeField] Cell cellPefab;
    int _mapMinX = 0;
    int _mapMinY = 0;
    [SerializeField] int _mapMaxX = 20;
    [SerializeField] int _mapMaxY = 20;
    const int _minRoomSpritNum = 4;
    [SerializeField] int _splitNum = 0;
    Cell[,] _cells = null;
    private void Start()
    {
        _cells = new Cell[_mapMaxX, _mapMaxY];
        for (int y = 0; y < _mapMaxY; y++)
        {
            for (int x = 0; x < _mapMaxX; x++)
            {
                var cell = Instantiate(cellPefab, this.transform);
                cell.MapState = MapStates.Wall;
                _cells[x, y] = cell;
                _cells[x, y].transform.position = new Vector3(x * 1.5f, y * 1.5f);
            }
        }
        SplitRoom(_mapMinX, _mapMinY, _mapMaxX, _mapMaxY, _splitNum, _cells);
    }
    public void SplitRoom(int minX, int minY, int maxX, int maxY, int splitNum, Cell[,] cells)
    {
        int xLineNum;
        int yLineNum;
        int subSplitNum = splitNum + 1;
        if (splitNum == 0) return;
        while (splitNum > 0)
        {
            if (maxX - minX > _minRoomSpritNum * 2 || maxY - minY > _minRoomSpritNum * 2)
            {
                int rand = Random.Range(-2, 3);
                if (maxX - minX >= maxY - minY)
                {
                    xLineNum = (maxX - minX) / 2 + minX + rand;
                    if(maxX - xLineNum >= xLineNum - minX)
                    {
                        //分割した左半分に部屋を作る
                        SetID(minX, minY, xLineNum, maxY, cells, subSplitNum - splitNum);
                        CreateRoom(minX, minY, xLineNum, maxY, cells);
                        CreateRoad(minX, minY, xLineNum, maxY, cells,Vector2.right);
                        minX = xLineNum + 1;
                    }
                    else
                    {
                        //分割した右半分に部屋を作る
                        SetID(xLineNum, minY, maxX, maxY, cells, subSplitNum - splitNum);
                        CreateRoom(xLineNum, minY, maxX, maxY, cells);
                        CreateRoad(xLineNum, minY, maxX, maxY, cells, Vector2.left);
                        maxX = xLineNum - 1;
                    }
                }
                else
                {
                    yLineNum = (maxY - minY) / 2 + minY + rand;
                    if (maxY - yLineNum >= yLineNum - minY)
                    {
                        //分割した下半分に部屋を作る
                        SetID(minX, minY, maxX, yLineNum, cells, subSplitNum - splitNum);
                        CreateRoom(minX, minY, maxX, yLineNum, cells);
                        CreateRoad(minX, minY, maxX, yLineNum, cells, Vector2.up);
                        minY = yLineNum + 1;
                    }
                    else
                    {
                        //分割した上半分に部屋を作る
                        SetID(minX, yLineNum, maxX, maxY, cells, subSplitNum - splitNum);
                        CreateRoom(minX, yLineNum, maxX, maxY, cells);
                        CreateRoad(minX, yLineNum, maxX, maxY, cells, Vector2.down);
                        maxY = yLineNum - 1;
                    }
                }
                splitNum--;
            }
            else
            {
                SetID(minX, minY, maxX, maxY, cells, subSplitNum - splitNum);
                CreateRoom(minX, minY, maxX, maxY, cells);
                break;
            }
        }
    }
    public void CreateRoom(int minX, int minY, int maxX, int maxY, Cell[,] cells)
    {
        //今後部屋のサイズをランダムに可変するための下書き
        //int xMinRand = Random.Range(minX + 1, maxX - minX / 2);
        //int xMaxRand = Random.Range(maxX - minX / 2, maxX - 1);

        //int yMinRand = Random.Range(minY + 1, maxY - minY / 2); 
        //int yMaxRand = Random.Range(maxY - minY / 2, maxY - 1);

        //for (int y = yMinRand; y < yMaxRand; y++)
        //{
        //    for (int x = xMinRand; x < xMaxRand; x++)
        //    {
        //        cells[x, y].MapState = MapStates.Floor;
        //    }
        //}
        for (int y = minY + 1; y < maxY - 1 ; y++)
        {
            for (int x = minX + 1; x < maxX - 1; x++)
            {
                cells[x, y].MapState = MapStates.Floor;
            }
        }

    }
    public void SetID(int minX, int minY, int maxX, int maxY, Cell[,] cells, int id)
    {
        for (int y = minY; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                cells[x,y].RoomId = id;
            }
        }
    }
    /// <summary>部屋から道を伸ばす関数</summary>
    /// <param name="minX">伸ばす元の部屋の最小のxPos</param>
    /// <param name="minY">伸ばす元の部屋の最小のyPos</param>
    /// <param name="maxX">伸ばす元の部屋の最大のxPos</param>
    /// <param name="maxY">伸ばす元の部屋の最大のyPos</param>
    /// /// <param name="maxY">道を伸ばす方向</param>
    public void CreateRoad(int minX, int minY, int maxX, int maxY, Cell[,] cells, Vector2 vec)
    {
        if (vec == Vector2.up)
        {
            int x = Random.Range(minX + 1, maxX);
            int y = maxY - 1;
            while (cells[x, y].RoomId != 0)
            {
                cells[x, y].MapState = MapStates.Floor;
                y++;
            }
            cells[x, y].MapState = MapStates.Floor;
            cells[x, y].Intersection = true;
        }
        else if (vec == Vector2.down)
        {
            int x = Random.Range(minX + 1, maxX);
            int y = minY;
            while (cells[x, y].RoomId != 0)
            {
                cells[x, y].MapState = MapStates.Floor;
                y--;
            }
            cells[x, y].MapState = MapStates.Floor;
            cells[x, y].Intersection = true;
        }
        else if (vec == Vector2.right)
        {
            int x = maxX - 1;
            int y = Random.Range(minY + 1, maxY);
            while (cells[x, y].RoomId != 0)
            {
                cells[x, y].MapState = MapStates.Floor;
                x++;
            }
            cells[x, y].MapState = MapStates.Floor;
            cells[x, y].Intersection = true;
        }
        else if (vec == Vector2.left)
        {
            int x = minX;
            int y = Random.Range(minY + 1, maxY);
            while (cells[x, y].RoomId != 0)
            {
                cells[x, y].MapState = MapStates.Floor;
                x--;
            }
            cells[x, y].MapState = MapStates.Floor;
            cells[x, y].Intersection = true;
        }
    }
}

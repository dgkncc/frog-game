using FrogGame._Core.Common;
using FrogGame._Core.Common.CoreEnums;
using FrogGame._Core.EventService;
using FrogGame._Core.EventService.CoreEvents;
using FrogGame._Core.Managers;
using FrogGame.Cell;
using FrogGame.Common.Enums;
using FrogGame.Common.Events;
using FrogGame.Grid;
using FrogGame.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace FrogGame.Board
{
    public class GameBoard : SingleMonoBehaviour<GameBoard>
    {
        [SerializeField] private Transform gridOrigin;
        private int _gridWidth;
        private int _gridHeight;
        private float _gridCellSize;
        private Grid2D _grid;
        private List<CellBlock> _gridCellBlocks;

        private void OnEnable()
        {
            GameEventService.On<GameStateChanged>(OnGameStateChanged);
            GameEventService.On<InitializeGameBoardEvent>(OnInitializeGameBoardEvent);
        }

        private void OnDisable()
        {
            GameEventService.Off<GameStateChanged>(OnGameStateChanged);
            GameEventService.Off<InitializeGameBoardEvent>(OnInitializeGameBoardEvent);
        }

        private void OnGameStateChanged(GameStateChanged e)
        {
            if (e.GameState == GameState.Complete || e.GameState == GameState.Fail)
                ClearGrid();
        }

        private void OnInitializeGameBoardEvent(InitializeGameBoardEvent e)
        {
            AssignGridSettings(e.GridWidth, e.GridHeight, GameSettings.Instance.CellSize);
            InitializeGrid();
            CreateGridFloor();
            InsertGridCellBlocks(e.CellBlockColumns);
        }

        private void InitializeGrid()
        {
            _grid = new Grid2D(gridOrigin.position, _gridWidth, _gridHeight, _gridCellSize);
            _gridCellBlocks = new List<CellBlock>();
        }

        private void CreateGridFloor()
        {
            for (int x = 0; x < _gridHeight; x++)
            {
                for (int y = 0; y < _gridWidth; y++)
                {
                    var node = _grid.GetNode(y, x);
                    var cellBlock = CreateCellBlock();
                    cellBlock.transform.position = node.Position;
                    _gridCellBlocks.Add(cellBlock);
                }
            }
        }

        private void ClearGrid()
        {
            if (_gridCellBlocks == null || _gridCellBlocks.Count == 0)
                return;

            foreach (var cell in _gridCellBlocks)
            {
                if (cell.IsOutOfPool)
                    cell.ReturnCellBlockToPool();
            }
        }

        private void InsertGridCellBlocks(List<List<CellBlock>> cellBlockColumns)
        {
            var cellHeight = GameSettings.Instance.CellHeight;
            for (int x = 0; x < _gridHeight; x++)
            {
                for (int y = 0; y < _gridWidth; y++)
                {
                    var gridNode = _grid.GetNode(x, y);

                    for (int i = 0; i < cellBlockColumns[((y * _gridHeight) + x)].Count; i++)
                    {
                        var cellBlock = cellBlockColumns[((y * _gridHeight) + x)][i];
                        cellBlock.transform.position = gridNode.Position + new Vector3(0f, cellHeight * (i + 1), 0f);
                        gridNode.InsertCellBlock(cellBlock);
                        _gridCellBlocks.Add(cellBlock);
                        cellBlock.AssignGridNode(gridNode);
                    }

                    gridNode.ActivateNextCellBlock();
                }
            }
        }

        private void AssignGridSettings(int gridWidth, int gridHieght, float gridCellSize)
        {
            _gridWidth = gridWidth;
            _gridHeight = gridHieght;
            _gridCellSize = gridCellSize;
        }

        private CellBlock CreateCellBlock()
        {
            var cell = PoolManager.Instance.GetFromPool<CellBlock>();
            cell.transform.SetParent(null);
            return cell;
        }

        public GridNode GetNodeInPosition(int posX, int posY) => _grid.GetNode(posX, posY);

        public GridNode GetNodeInDirection(int posX, int posY, CellContentDirection direction)
        {
            switch (direction)
            {
                case (CellContentDirection.Up):
                    posY++;
                    break;
                case (CellContentDirection.Down):
                    posY--;
                    break;
                case (CellContentDirection.Left):
                    posX--;
                    break;
                case (CellContentDirection.Right):
                    posX++;
                    break;

            }

            return _grid.GetNode(posX, posY);
        }
    }
}
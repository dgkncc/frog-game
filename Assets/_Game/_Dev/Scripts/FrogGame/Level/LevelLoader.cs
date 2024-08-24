using AYellowpaper.SerializedCollections;
using FrogGame._Core.Common;
using FrogGame._Core.EventService;
using FrogGame._Core.Managers;
using FrogGame.Cell;
using FrogGame.Common.Enums;
using FrogGame.Common.Events;
using FrogGame.ScriptableObjects.Level;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FrogGame.Level
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private LevelsCollectionData levelsCollection;
        [SerializeField] private SerializedDictionary<CellContentType, PoolMonoBehaviour> prefabTypes = new();

        private void OnEnable()
        {
            GameEventService.On<LoadCurrentLevel>(OnLoadCurrentLevel);
        }

        private void OnDisable()
        {
            GameEventService.Off<LoadCurrentLevel>(OnLoadCurrentLevel);
        }

        private async void OnLoadCurrentLevel(LoadCurrentLevel _)
        {
            int currentLevelIndex = PlayerPrefs.GetInt(GlobalConstants.CURRENT_LEVEL_KEY);
            LevelData currentLevel;

            switch (currentLevelIndex)
            {
                case >= 0 when currentLevelIndex < levelsCollection.levels.Count:
                    {
                        currentLevel = levelsCollection.levels[currentLevelIndex];
                        break;
                    }

                default:
                    {
                        PlayerPrefs.SetInt(GlobalConstants.CURRENT_LEVEL_KEY, 0);
                        currentLevel = levelsCollection.levels[0];
                        break;
                    }
            }

            await LoadLevel(currentLevel);
        }

        private async Task LoadLevel(LevelData levelData)
        {
            List<List<CellBlock>> cellBlockColumns = new List<List<CellBlock>>();
            List<CellBlock> cellBlocks;
            for (int i = 0; i < levelData.gridCellContents.Count; i++)
            {
                cellBlocks = new();
                for (int j = 0; j < levelData.gridCellContents[i].contentDataCollection.Count; j++)
                {
                    var contentData = levelData.gridCellContents[i].contentDataCollection[j];
                    var contentType = contentData.type;
                    var contentColor = contentData.color;
                    var contentDirection = contentData.direction;
                    var grapeCount = contentData.grapeCount;
                    await Task.Yield();
                    var cell = CreateCellBlock();
                    var cellContent = CreateCellContent(contentType, cell);
                    cellContent.SetCellContentData(contentData);
                    cell.InsertCellContent(cellContent);
                    cellContent.InitializeContent(contentColor, contentDirection, grapeCount, new Vector2Int(i % levelData.height, i / levelData.height));

                    cellBlocks.Add(cell);
                }
                cellBlockColumns.Add(cellBlocks);
            }

            GameEventService.Fire(new InitializeGameBoardEvent
            {
                GridWidth = levelData.width,
                GridHeight = levelData.height,
                CellBlockColumns = cellBlockColumns
            });

            GameEventService.Fire(new AdjustCamera
            {
                GridWidth = levelData.width
            });

            GameEventService.Fire(new StartLevel
            {
                MoveCount = levelData.moveCount
            });
        }


        private CellBlock CreateCellBlock()
        {
            var cell = PoolManager.Instance.GetFromPool<CellBlock>();
            cell.transform.SetParent(null);
            return cell;
        }

        private ICellContent CreateCellContent(CellContentType type, CellBlock cell)
        {
            if (prefabTypes.TryGetValue(type, out var cellContentPrefab))
            {
                var content = PoolManager.Instance.GetFromPool(cellContentPrefab);
                content.transform.SetParent(cell.transform);
                return content as ICellContent;

            }
            return null;
        }
    }
}
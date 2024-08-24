using FrogGame.Board;
using FrogGame.Cell;
using FrogGame.Cell.Contents.GrapeContent;
using FrogGame.Common.Enums;
using FrogGame.Common.Structs;
using FrogGame.Grid;
using FrogGame.ScriptableObjects;
using FrogGame.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrogTongue : MonoBehaviour
{
    [SerializeField] private LineRenderer tongueBaseLineRenderer;
    [SerializeField] private LineRenderer tongueTiplineRenderer;
    [SerializeField] private Vector3 tonguePositionOffset;
    [SerializeField] private Transform tonguePosition;

    private bool _isInitialized = false;
    private float _duration = 0f;
    private int _grapeCount;
    private CellContentColor _color;
    private CellContentDirection _direction;
    private Vector2Int _startPosition;


    private List<Vector3> _path;
    private Stack<Grape> _grapes;
    private Stack<Vector3> _grapePaths;
    private Stack<GridNode> _grapeGridNodes;
    private Stack<bool> _popExtra;

    public event Action OnComplete;
    public event Action OnFail;

    private void OnDisable()
    {
        Terminate();
    }

    public void ExtendTongue()
    {
        if (!_isInitialized)
            return;

        ResetTongue();

        _path.Add(tonguePosition.position);
        _grapePaths.Push(transform.position);

        StartCoroutine(ExtendTongueCoroutine());
    }

    public void Initialize(int grapeCount, CellContentColor color, CellContentDirection direction, Vector2Int startPosition)
    {
        if (_isInitialized)
            return;

        _isInitialized = true;
        _duration = GameSettings.Instance.TongueExtendDuration;
        _grapeCount = grapeCount;
        _color = color;
        _direction = direction;
        _startPosition = startPosition;
        _path = new List<Vector3>();
        _grapes = new Stack<Grape>();
        _grapePaths = new Stack<Vector3>();
        _grapeGridNodes = new Stack<GridNode>();
        _popExtra = new Stack<bool>();
        OnFail += ResetTongue;
        OnComplete += ResetTongue;
        OnComplete += Terminate;
    }

    private void Terminate()
    {
        if (!_isInitialized)
            return;

        _isInitialized = false;
        OnFail -= ResetTongue;
        OnComplete -= ResetTongue;
        OnComplete -= Terminate;
    }

    private void ClearAllCollections()
    {
        _path.Clear();
        _grapes.Clear();
        _grapePaths.Clear();
        _grapeGridNodes.Clear();
        _popExtra.Clear();
    }

    private void ResetTongue()
    {
        ClearAllCollections();
        tongueBaseLineRenderer.positionCount = 0;
        tongueTiplineRenderer.positionCount = 0;
    }

    #region Tongue Extension

    private IEnumerator ExtendTongueCoroutine()
    {
        Vector2Int currentPos = _startPosition;
        GameBoard gameBoard = GameBoard.Instance;
        CellContentDirection direction = _direction;

        for (int i = 0; i < _grapeCount; i++)
        {
            var nextNode = gameBoard.GetNodeInDirection(currentPos.x, currentPos.y, direction);
            if (nextNode == null || !TryGetCellContentData(nextNode, out var cellContentData))
            {
                FailExtension();
                yield break;
            }

            Vector3 nextPos = nextNode.ActiveCellBlock.transform.position + tonguePositionOffset;
            currentPos = nextNode.PositionInGrid;

            _path.Add(nextPos);
            UpdateLineRenderers();

            yield return MovePath(nextPos);

            _grapePaths.Push(nextPos);
            _grapeGridNodes.Push(nextNode);

            if (!ProcessCellContent(cellContentData, nextNode, ref i, ref direction))
            {
                FailExtension();
                yield break;
            }
        }

        StartCoroutine(RetractTongueCoroutine(OnComplete));
        StartCoroutine(MoveGrapes());
    }


    private bool TryGetCellContentData(GridNode nextNode, out CellContentData cellContentData)
    {
        if (nextNode.ActiveCellBlock.TryGetCellContentData(out CellContentData data))
        {
            cellContentData = data;
            return true;
        }

        cellContentData = default;
        return false;
    }

    private IEnumerator MovePath(Vector3 nextPos)
    {

        Vector3 startPos = _path[_path.Count - 2];
        UpdateLineRenderers();

        for (float elapsedTime = 0; elapsedTime < _duration; elapsedTime += Time.deltaTime)
        {
            Vector3 interpolatedPos = Vector3.Lerp(startPos, nextPos, elapsedTime / _duration);
            _path[_path.Count - 1] = interpolatedPos;
            UpdateLineRenderers();
            yield return null;
        }
        UpdateLineRenderers();
        _path[_path.Count - 1] = nextPos;
    }



    private bool ProcessCellContent(CellContentData cellContentData, GridNode nextNode, ref int index, ref CellContentDirection direction)
    {
        if (cellContentData.type == CellContentType.Arrow && cellContentData.color == _color)
        {
            direction = cellContentData.direction;
            index--;
            _popExtra.Push(true);
        }
        else if (cellContentData.type == CellContentType.Grape && ProcessGrapeContent(cellContentData, nextNode))
        {
            _popExtra.Push(false);
        }
        else
        {
            return false;
        }
        return true;
    }

    private bool ProcessGrapeContent(CellContentData cellContentData, GridNode nextNode)
    {
        var cellBlock = nextNode.ActiveCellBlock;
        if (!cellBlock.TryGetCellContent(out ICellContent cellContent) || !(cellContent is Grape grape))
        {
            return false;
        }

        grape.PunchScale();
        if (cellContentData.color != _color)
        {
            grape.FlashColor(_duration);
            return false;
        }

        _grapes.Push(grape);
        return true;
    }

    private void FailExtension()
    {
        _grapeGridNodes.Clear();
        StartCoroutine(RetractTongueCoroutine(OnFail));
    }

    #endregion

    #region Tongue Retraction

    private IEnumerator RetractTongueCoroutine(Action callBackAction)
    {
        while (_path.Count > 1)
        {
            yield return MovePathBack();

            if (_grapeGridNodes.Count > 0)
            {
                var gridNode = _grapeGridNodes.Pop();
                var cellBlock = gridNode.ActiveCellBlock;

                if (cellBlock.TryGetCellContentData(out CellContentData data) && data.type == CellContentType.Grape)
                    cellBlock.RemoveContent();

                gridNode.CompleteActiveCellBlock();
            }


        }

        _path[0] = transform.position;
        callBackAction?.Invoke();
    }

    private IEnumerator MovePathBack()
    {
        Vector3 startPos = _path[_path.Count - 1];
        Vector3 endPos = _path[_path.Count - 2];
        UpdateLineRenderers();

        for (float elapsedTime = 0; elapsedTime < _duration; elapsedTime += Time.deltaTime)
        {
            Vector3 interpolatedPos = Vector3.Lerp(startPos, endPos, elapsedTime / _duration);
            _path[_path.Count - 1] = interpolatedPos;
            UpdateLineRenderers();
            yield return null;
        }

        UpdateLineRenderers();
        _path.RemoveAt(_path.Count - 1);
    }


    #endregion

    #region Grape Movement
    private IEnumerator MoveGrapes()
    {
        float delay = 0f;
        float pathMoveDuration = _duration / 2;
        for (int i = 0; i < _grapeCount; i++)
        {
            yield return new WaitForSeconds(delay + GetExtraDuration());
            StartCoroutine(MoveGrapeAlongPath(_grapes.Pop(), _grapePaths.ToList()));
            delay = pathMoveDuration;
        }
    }


    private float GetExtraDuration()
    {
        float extraDuration = 0f;
        _grapePaths.Pop();
        while (_popExtra.Count > 0 && _popExtra.Pop())
        {
            extraDuration += _duration;
            _grapePaths.Pop();
        }

        return extraDuration;
    }

    private IEnumerator MoveGrapeAlongPath(Grape grape, List<Vector3> path)
    {
        grape.transform.SetParent(null);

        float duration = _duration;
        float timer = 0f;
        int index = 0;

        Vector3 startPos = grape.transform.position;
        Vector3 endPos = path[index];
        Vector3 direction = (endPos - startPos).normalized;

        while (index < path.Count)
        {
            MoveGrapeTowardsPosition(grape, startPos, endPos, duration, ref timer);

            if (grape.transform.position == endPos)
            {
                index++;
                if (index < path.Count)
                {
                    UpdateNextPathPoint(ref startPos, ref endPos, path, index, out direction, ref timer);
                }
            }

            CheckFinalPoint(index, path.Count, grape, _duration);

            yield return null;
        }
    }

    private void MoveGrapeTowardsPosition(Grape grape, Vector3 startPos, Vector3 endPos, float duration, ref float timer)
    {
        float distance = Vector3.Distance(grape.transform.position, endPos);
        float step = (Time.deltaTime / duration) * distance;
        timer += Time.deltaTime;

        grape.transform.position = Vector3.Lerp(startPos, endPos, timer / duration);
    }

    private void UpdateNextPathPoint(ref Vector3 startPos, ref Vector3 endPos, List<Vector3> path, int index, out Vector3 direction, ref float timer)
    {
        startPos = endPos;
        endPos = path[index];
        direction = (endPos - startPos).normalized;
        timer = 0f;
    }

    private void CheckFinalPoint(int index, int pathCount, Grape grape, float duration)
    {
        if (index == pathCount - 1)
        {
            grape.CompleteGrape(duration);
        }
    }

    private void UpdateLineRenderers()
    {

        var hermitePath = _path.GenerateHermiteCurve(0.5f, 10).ToArray();
        tongueBaseLineRenderer.positionCount = hermitePath.Length;
        tongueBaseLineRenderer.SetPositions(hermitePath);

        if (hermitePath.Length > 2)
        {
            var startPos = hermitePath[hermitePath.Length - 2];
            var endPos = hermitePath[hermitePath.Length - 1];

            var lineDirection = (endPos - startPos).normalized;
            tongueTiplineRenderer.positionCount = 2;
            tongueTiplineRenderer.SetPosition(0, endPos);
            tongueTiplineRenderer.SetPosition(1, endPos + lineDirection * tongueTiplineRenderer.startWidth * 2);
        }
        else
        {
            tongueTiplineRenderer.positionCount = 0;
        }
    }

    #endregion


}

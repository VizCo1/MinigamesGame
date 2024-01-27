using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconHolderSpace : MonoBehaviour
{
    [SerializeField] protected Transform _iconContainer;
    [SerializeField] protected GameObject _iconTemplate;

    [Header("Holder space settings")]
    [SerializeField] protected Transform _initialPosTranform;
    [SerializeField] protected int _rows;
    [SerializeField] protected int _columns;
    [SerializeField] protected float _iconHeight;
    [SerializeField] protected float _iconWidth;
    [SerializeField] protected Vector2 _spacing;

    protected IconPosition[] _iconPositions;

    private void Start()
    {
        DOVirtual.DelayedCall(0.5f, InitializeSpace);
    }

    protected virtual void InitializeSpace() { }

    protected Vector2 GetAvailableStartingPosition()
    {
        for (int i = 0; i < _iconPositions.Length; i++)
        {
            if (!_iconPositions[i].IsOccupied)
            {
                _iconPositions[i].IsOccupied = true;
                return _iconPositions[i].Position;
            }
        }

        return Vector2.zero;
    }

    //private void OnDrawGizmos()
    //{
    //    _iconPositions = new IconPosition[_rows * _columns];

    //    Vector2 initialPos = _initialPosTranform.position;
    //    float width = _iconWidth + _spacing.x;
    //    float height = _iconHeight + _spacing.y;

    //    for (int y = 0; y < _columns; y++)
    //    {
    //        for (int x = 0; x < _rows; x++)
    //        {
    //            Vector2 pos = initialPos + new Vector2(width * x, height * -y) * GetComponentInParent<Canvas>().scaleFactor;
    //            _iconPositions[y * _rows + x] = new IconPosition(pos, false);
    //        }
    //    }

    //    Gizmos.color = Color.yellow;
    //    foreach (IconPosition iconPosition in _iconPositions)
    //    {
    //        Gizmos.DrawCube(iconPosition.Position, new Vector2(_iconWidth * GetComponentInParent<Canvas>().scaleFactor,
    //            _iconHeight * GetComponentInParent<Canvas>().scaleFactor));
    //    }
    //}

    protected struct IconPosition
    {
        public Vector2 Position { get; set; }
        public bool IsOccupied { get; set; }

        public IconPosition(Vector2 pos, bool occupied)
        {
            Position = pos;
            IsOccupied = occupied;
        }
    }

    public virtual void AddIcon() { }
    public virtual void RemoveIcon(GameObject icon) { }
    public virtual int FindProperIndex(Vector2 iconPos) { return default; }

    public void SetIconPositionStatus(GameObject icon, bool setter) => _iconPositions[(FindProperIndex(icon.transform.position))].IsOccupied = setter;    
}
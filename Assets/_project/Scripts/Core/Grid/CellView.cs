using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _project.Scripts.Domain.Grid
{
    public class CellView : MonoBehaviour
    {
        bool isHighlighted = false;
        public GridCell cell;
        private Tweener tween;
        Action<bool,GridCell> toggleAction;
        Vector3 mainPosition;
        public void Init(GridCell cell)
        {
            this.cell = cell;
        }

        private void Awake()
        {
            mainPosition = transform.position;
        }

        public void Block()
        {
            cell.Block();
            GetComponent<Renderer>().material.color = Color.gray;
        }

        public void UnBlock()
        {
            cell.Unblock();
            GetComponent<Renderer>().material.color = Color.white;
        }

        public void ChangeColor(Color color)
        {
            GetComponent<Renderer>().material.color = color;
        }
        public void Highlight()
        {
            if (isHighlighted) return;
            tween.Kill();
            tween = transform.DOMove(mainPosition + (Vector3.up*0.2f),0.5f).SetEase(Ease.OutBack);
            isHighlighted = true;
        }

        public void UnHighlight()
        {
            if (!isHighlighted) return;
            tween.Kill();
            tween = transform.DOMove(mainPosition,0.5f).SetEase(Ease.OutBack);
            isHighlighted = false;
        }
    }
}
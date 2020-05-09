using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 0.3f;
    private Vector3 moveDir;

    void Start()
    {
        moveDir = Vector3.zero;
    }

    void Update()
    {
        HandleKeyInput();
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Physics2D.queriesStartInColliders = true;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            if (hit.transform != null)
            {
                if (MapManager.Instance.GridSystem == null)
                    return;

                BaseCell cell = MapManager.Instance.GridSystem.GetCellByPos(CellManager.Instance.cellShape, hit.point);

                if (cell == null)
                    return;

                MessageManager.Instance.Do(Event.ClickOnCell, cell.ID);
            }
        }
    }

    private void HandleKeyInput()
    {
        moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir += Vector3.up;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDir += Vector3.down;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDir += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDir += Vector3.right;
        }

        moveDir = moveDir.normalized;

        transform.Translate(moveDir * moveSpeed);
    }
}

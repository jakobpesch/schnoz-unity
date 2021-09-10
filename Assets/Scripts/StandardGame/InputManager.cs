using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace Utils
{
  public class InputManager : MonoBehaviour
  {
    public Camera mainCam;
    public Vector3 mousePositionWorldPoint;
    public Vector3 mousePositionScreenPoint;
    public bool mouseDown;
    public bool mouseDrag;
    public bool mouseUp;
    public List<bool> SelectCardButtons;
    public Vector3 lastMousePositionScreenPoint;
    public float scroll;
    public GameObject selectedTile;
    public GameObject hoveredTile;
    public bool RotateLeft;
    public bool RotateRight;
    public bool FlipHorizontal;
    public bool FlipVertical;
    public bool Cancel;

    public bool didMouseMove
    {
      get => lastMousePositionScreenPoint != mousePositionScreenPoint; // WP or SP doesn't matter here
    }
    private float x0, y0, x1, y1;
    void Start()
    {
      mainCam = Camera.main;
      SelectCardButtons = new List<bool>() { false, false, false };
    }


    void Update()
    {
      mousePositionScreenPoint = Input.mousePosition;
      mousePositionWorldPoint = mainCam.ScreenToWorldPoint(mousePositionScreenPoint);
      mousePositionWorldPoint.Set(mousePositionWorldPoint.x, mousePositionWorldPoint.y, 0);

      mouseDown = Input.GetMouseButtonDown(0);
      mouseDrag = Input.GetMouseButton(0);
      mouseUp = Input.GetMouseButtonUp(0);

      RotateLeft = Input.GetKeyDown(KeyCode.D);
      RotateRight = Input.GetKeyDown(KeyCode.A);
      FlipHorizontal = Input.GetKeyDown(KeyCode.C);
      FlipVertical = Input.GetKeyDown(KeyCode.Y);

      Cancel = Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1);

      scroll = Input.GetAxis("Mouse ScrollWheel");

      // hoveredTile = HoveredTile();
      // selectedTile = SelectedTile();

      SelectCardButtons[0] = Input.GetKeyDown(KeyCode.Alpha1);
      SelectCardButtons[1] = Input.GetKeyDown(KeyCode.Alpha2);
      SelectCardButtons[2] = Input.GetKeyDown(KeyCode.Alpha3);

    }

    // public GameObject HoveredTile()
    // {
    //   // Check if still on the same tile from the previous frame
    //   if (hoveredTile != null)
    //   {
    //     if (mousePositionWorldPoint.x > x0 && mousePositionWorldPoint.x < x1 &&
    //         mousePositionWorldPoint.y > y0 && mousePositionWorldPoint.y < y1)
    //       return hoveredTile;
    //   }

    //   // Check which tile is hovered
    //   foreach (GameObject tile in Map.I.Tiles)
    //   {
    //     x0 = tile.transform.position.x - Map.I.HalfTileSize;
    //     y0 = tile.transform.position.y - Map.I.HalfTileSize;
    //     x1 = tile.transform.position.x + Map.I.HalfTileSize;
    //     y1 = tile.transform.position.y + Map.I.HalfTileSize;

    //     if (mousePositionWorldPoint.x > x0 && mousePositionWorldPoint.x < x1 &&
    //         mousePositionWorldPoint.y > y0 && mousePositionWorldPoint.y < y1)
    //       return tile;
    //   }
    //   return null;
    // }

    // public GameObject SelectedTile()
    // {
    //   if (mouseDown && hoveredTile != null && SpawnManager.I.selectedCardFormation == null)
    //   {
    //     selectedTile = hoveredTile;
    //   }

    //   if (mouseDown && hoveredTile == null || Cancel)
    //   {
    //     selectedTile = null;
    //   }

    //   // Not currently selected any tile
    //   return selectedTile;
    // }
  }
}

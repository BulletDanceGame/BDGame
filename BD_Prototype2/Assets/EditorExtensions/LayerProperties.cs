using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;


namespace BulletDance.LevelBuilder
{
    public enum LayerType { Tile, GameObject };
    public class LayerProperties : MonoBehaviour
    {
        #if UNITY_EDITOR
            public LayerType    layerType;
            public GridSettings defaultGrid;

            public Grid grid;
            public Tilemap tilemap;

        public void SetProperties(LayerType _layerType, GridSettings _defaultGrid = null, 
                                  Grid _grid = null, Tilemap _tilemap = null)
        {
            layerType   = _layerType;
            defaultGrid = _defaultGrid != null ? _defaultGrid : new GridSettings();
            grid        = _grid ? _grid : GetComponent<Grid>();
            tilemap     = _tilemap ? _tilemap : GetComponent<Tilemap>();
        }

        public void ResetGrid()
        {
            if(grid && defaultGrid != null)
            {
                grid.cellGap  = defaultGrid.cellGap;
                grid.cellSize = defaultGrid.cellSize;
            }
        }
        #endif
    }

    [System.Serializable]
    public class GridSettings
    {
        public Vector3 cellSize;
        public Vector3 cellGap;

        public GridSettings()
        {
            cellSize = new Vector3(1, 1, 0);
            cellGap  = new Vector3(0, 0, 0);
        }

        public GridSettings(Vector3 _cellSize, Vector3 _cellGap)
        {
            cellSize = _cellSize;
            cellGap  = _cellGap;
        }

        public static GridSettings TileGrid()
        {
            GridSettings grid = new GridSettings();
            grid.cellSize = new Vector3(4, 4, 0);
            return grid;
        }

        public static GridSettings ObjectGrid()
        {
            GridSettings grid = new GridSettings();
            grid.cellSize = new Vector3(0.5f, 0.5f, 0);
            return grid;
        }
    }

}
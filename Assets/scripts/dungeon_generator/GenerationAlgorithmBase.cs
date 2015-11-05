using UnityEngine;
using System.Collections;

public abstract class GenerationAlgorithmBase 
{
  public virtual void Do(Grid grid)
  {
  }

  /// <summary>
  /// Each cell has a special parameter - CellStatusForCleanup, which is CellType.EMPTY by default.
  /// Some algorithms use different approaches to maze generation. They may be passage carvers, i.e.
  /// map fills with walls at start and then algorithm "floors" certain cells, or wall builders, i.e.
  /// when map is filled with empty tiles by default and then algorithm "floors" certain tiles and then
  /// marks boundaries with walls.
  /// In the latter case only some tiles will be of type floor, so we mark tiles, that touch these floor tiles
  /// as CellType.BOUNDARY in CellStatusForCleanup variable. Then, in next two for loops all tiles, that are
  /// not marked are made empty, so we have nice and clean map. :-)
  /// 
  /// For example, when we generate rooms, we first wall off the chosen size of the room, then carve space inside it.
  /// During this cleanup method, all walls that touch floor tiles of the room are marked as CellType.BOUNDARY.
  /// So, invisible corner wall blocks are then removed during next block of for loops:
  /// 
  /// ####       ## 
  /// #..#      #..#
  /// #..#  ->  #..#
  /// #..#      #..#
  /// ####       ##
  /// 
  /// </summary>
  /// <param name="grid">Output of dungeon generation algorithm</param>
  public virtual void Cleanup(Grid grid)
  {    
    // Mark walls that touch floor tiles
    for (int x = 0; x < grid.MapHeight; x++)
    {
      for (int y = 0; y < grid.MapWidth; y++)
      {        
        if (grid.Map[x == 0 ? 0 : x - 1, y].CellType == CellType.FLOOR || grid.Map[x == grid.MapHeight - 1 ? grid.MapHeight - 1 : x + 1, y].CellType == CellType.FLOOR
         || grid.Map[x, y == 0 ? 0 : y - 1].CellType == CellType.FLOOR || grid.Map[x, y == grid.MapWidth - 1 ? grid.MapWidth - 1 : y + 1].CellType == CellType.FLOOR)
        {
          grid.Map[x, y].CellStatusForCleanup = CellType.BOUNDARY;
        }
      }
    }

    // Remove all cells that are not marked
    for (int x = 0; x < grid.MapHeight; x++)
    {
      for (int y = 0; y < grid.MapWidth; y++)
      {
        if (grid.Map[x, y].CellStatusForCleanup != CellType.BOUNDARY)
        {
          grid.Map[x, y].CellType = CellType.EMPTY;
        }
      }
    }
  }
}

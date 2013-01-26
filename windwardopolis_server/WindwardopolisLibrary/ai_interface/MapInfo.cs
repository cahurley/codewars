
using System.Drawing;
using WindwardopolisLibrary.map;

namespace WindwardopolisLibrary.ai_interface
{
	public class MapInfo
	{
		private readonly GameMap map;

		public MapInfo(GameMap map)
		{
			Trap.trap();
			this.map = map;
		}

		/// <summary>
		/// The map squares. This is in the format [x][y].
		/// </summary>
		public MapSquare[][] Squares
		{
			// bugbug - need MapSquareInfo (MapSquare can inherit from it)
			get { return map.Squares; }
		}

		/// <summary>
		/// The width of the map. Units are squares.
		/// </summary>
		public int Width
		{
			get { return map.Squares.Length; }
		}

		/// <summary>
		/// The height of the map. Units are squares.
		/// </summary>
		public int Height
		{
			get { return map.Squares[0].Length; }
		}

		/// <summary>
		/// Returns the requested point or null if off the map.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public MapSquare SquareOrDefault(Point pt)
		{
			if ((pt.X < 0) || (pt.Y < 0) || (pt.X >= Width) || (pt.Y >= Height))
				return null;
			return map.Squares[pt.X][pt.Y];
		}
	}
}

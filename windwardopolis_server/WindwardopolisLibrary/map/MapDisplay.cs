// Created by Windward Studios, Inc. (www.windward.net). No copyright claimed - do anything you want with this code.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WindwardopolisLibrary.units;

namespace WindwardopolisLibrary.map
{
	/// <summary>
	/// The map window.
	/// </summary>
	public partial class MapDisplay : UserControl
	{
		private readonly List<Point> limoPaintLocations = new List<Point>();
		private readonly Font fontOfficeName = new Font("Calibri", 12);
		private readonly StringFormat format = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };

		public delegate void MouseClicked(Point mapUnits, MouseButtons buttons);

		public event MouseClicked MouseClickedEvent;

		/// <summary>
		/// Display map coordinates on the top and left.
		/// </summary>
		public bool DisplayCoordinates { get; set; }

		/// <summary>
		/// Display a shadow below remote A.I. cars.
		/// </summary>
		public bool DisplayShadows { get; set; }

		/// <summary>
		/// Display a shadow below all cars.
		/// </summary>
		public bool DisplayShadowsAll { get; set; }

		/// <summary>
		/// Create the map window.
		/// </summary>
		public MapDisplay()
		{
			InitializeComponent();
		}

		private IMapInfo Engine
		{
			get
			{
				Control ctrl = Parent;
				while (ctrl != null)
				{
					IMapInfo wnd = ctrl as IMapInfo;
					if (wnd != null)
						return wnd;
					ctrl = ctrl.Parent;
				}
				return null;
			}
		}

		private void MapDisplay_Load(object sender, EventArgs e)
		{
			ZoomChanged();
		}

		public void ZoomChanged()
		{
			// not parent in design mode
			IMapInfo engine = Engine;
			if (engine == null)
				return;

			GameMap map = engine.Map;
			if (map == null)
				return;

			AutoScrollMinSize = new Size(map.Width * Engine.PixelsPerTile, map.Height * Engine.PixelsPerTile);
			limoPaintLocations.Clear();
			Invalidate();
		}

		/// <summary>
		/// Invalidate all window locations where limos were last painted and where they are now.
		/// </summary>
		/// <param name="limoLocations"></param>
		public void InvalidateLimos(List<Point> limoLocations)
		{
			int extent = (Engine.PixelsPerTile * 3) / 2 + 2;
			int offsetX = extent / 2 - AutoScrollPosition.X;
			int offsetY = extent / 2 - AutoScrollPosition.Y;
			Rectangle rect = new Rectangle(0, 0, extent, extent);

			foreach (Point pt in limoPaintLocations)
			{
				rect.X = pt.X - offsetX;
				rect.Y = pt.Y - offsetY;
				Invalidate(rect);
			}

			// Limo.Location is 24 units per tile
			float adjustLocation = (float)Engine.PixelsPerTile / TileMovement.UNITS_PER_TILE;
			foreach (Point pt in limoLocations)
			{
				rect.X = (int)(pt.X * adjustLocation) - offsetX;
				rect.Y = (int)(pt.Y * adjustLocation) - offsetY;
				Invalidate(rect);
			}
		}

		/// <summary>
		/// Paint the window.
		/// </summary>
		private void MapDisplay_Paint(object sender, PaintEventArgs pea)
		{

			// not parent in design mode
			IMapInfo engine = Engine;
			if (engine == null)
				return;
			GameMap map = engine.Map;
			if (map == null)
				return;

			bool displayStart = engine.Players == null || engine.Players.Count == 0;

			pea.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
			pea.Graphics.CompositingQuality = CompositingQuality.HighQuality;
			pea.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			// no - draws lines around each square
			// pea.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

			IList<SignalSquare> signals = new List<SignalSquare>();
			IList<SignalSquare> offices = new List<SignalSquare>();
			Rectangle rectTile = new Rectangle(0, 0, Engine.PixelsPerTile, Engine.PixelsPerTile);
			int divSize = Sprites.road.Height/Engine.PixelsPerTile;
			Rectangle rectStart = new Rectangle(0, 0, Sprites.arrow_up_green.Width/divSize, Sprites.arrow_up_green.Height/divSize);

			int xStart = (pea.ClipRectangle.X - AutoScrollPosition.X) / Engine.PixelsPerTile;
			int xEnd = (pea.ClipRectangle.X + pea.ClipRectangle.Width - AutoScrollPosition.X + Engine.PixelsPerTile - 1) / Engine.PixelsPerTile;
			xEnd = Math.Min(xEnd, map.Squares.Length);
			int yStart = (pea.ClipRectangle.Y - AutoScrollPosition.Y) / Engine.PixelsPerTile;
			int yEnd = (pea.ClipRectangle.Y + pea.ClipRectangle.Height - AutoScrollPosition.Y + Engine.PixelsPerTile - 1) / Engine.PixelsPerTile;
			yEnd = Math.Min(yEnd, map.Squares[0].Length);

			for (int x = xStart; x < xEnd; x++)
			{
				rectTile.X = x * Engine.PixelsPerTile;
				for (int y = yStart; y < yEnd; y++)
				{
					MapSquare square = map.Squares[x][y];
					rectTile.Y = y*Engine.PixelsPerTile;
					pea.Graphics.DrawImage(square.SpriteBitmap, rectTile);

					// small logo if a bus stop
					if (square.Tile.Type == MapTile.TYPE.BUS_STOP && square.Company != null)
					{
						Bitmap bmp = square.Company.Logo;
						offices.Add(new SignalSquare(x, y, square));
						int width = Sprites.road.Height/divSize/2;
						int height = (bmp.Height*width)/bmp.Width;
						Rectangle rect = new Rectangle(rectTile.X + (Engine.PixelsPerTile - width)/2,
						                               rectTile.Y + (Engine.PixelsPerTile - height)/2, width, height);
						pea.Graphics.DrawImage(bmp, rect);
					}

					// below stuff is roads only
					if (square.Tile.Type != MapTile.TYPE.ROAD)
						continue;

					// if start position, that goes right over the road art
					if (displayStart)
						switch (square.StartPosition)
						{
							case MapSquare.COMPASS_DIRECTION.NORTH:
								rectStart.X = rectTile.X + Engine.PixelsPerTile/2 + (Engine.PixelsPerTile/2 - rectStart.Width)/2;
								rectStart.Y = rectTile.Y + (Engine.PixelsPerTile - rectStart.Height)/2;
								pea.Graphics.DrawImage(Sprites.arrow_up_green, rectStart);
								break;
							case MapSquare.COMPASS_DIRECTION.EAST:
								rectStart.X = rectTile.X + (Engine.PixelsPerTile - rectStart.Width)/2;
								rectStart.Y = rectTile.Y + Engine.PixelsPerTile/2 + (Engine.PixelsPerTile/2 - rectStart.Height)/2;
								pea.Graphics.DrawImage(Sprites.arrow_right_green, rectStart);
								break;
							case MapSquare.COMPASS_DIRECTION.SOUTH:
								rectStart.X = rectTile.X + (Engine.PixelsPerTile/2 - rectStart.Width)/2;
								rectStart.Y = rectTile.Y + (Engine.PixelsPerTile - rectStart.Height)/2;
								pea.Graphics.DrawImage(Sprites.arrow_down_green, rectStart);
								break;
							case MapSquare.COMPASS_DIRECTION.WEST:
								rectStart.X = rectTile.X + (Engine.PixelsPerTile - rectStart.Width)/2;
								rectStart.Y = rectTile.Y + (Engine.PixelsPerTile/2 - rectStart.Height)/2;
								pea.Graphics.DrawImage(Sprites.arrow_left_green, rectStart);
								break;
						}

					// write signals below
					if (square.StopSigns != MapSquare.STOP_SIGNS.NONE || square.SignalDirection != MapSquare.SIGNAL_DIRECTION.NONE)
						signals.Add(new SignalSquare(x, y, square));
				}
			}

#if NAMES
			// Office names. On top of terrain but below everything else
			if (Engine.PixelsPerTile >= 24)
			{
				foreach (SignalSquare officeOn in offices)
				{
					int x = officeOn.X*Engine.PixelsPerTile + Engine.PixelsPerTile/2;
					int y = officeOn.Y*Engine.PixelsPerTile + (int) (Engine.PixelsPerTile*1.5f);
					switch (officeOn.Square.Tile.Direction)
					{
						case MapTile.DIRECTION.NORTH_UTURN:
							y -= Engine.PixelsPerTile*2;
							format.Alignment = StringAlignment.Center;
							break;
						case MapTile.DIRECTION.SOUTH_UTURN:
							format.Alignment = StringAlignment.Center;
							break;
						case MapTile.DIRECTION.WEST_UTURN:
							x -= Engine.PixelsPerTile/2;
							y -= Engine.PixelsPerTile;
							format.Alignment = StringAlignment.Far;
							break;
						case MapTile.DIRECTION.EAST_UTURN:
							x += Engine.PixelsPerTile/2;
							y -= Engine.PixelsPerTile;
							format.Alignment = StringAlignment.Near;
							break;
					}
					pea.Graphics.DrawString(officeOn.Square.Company.Name, fontOfficeName, Brushes.DarkOrange, x, y, format);
				}
				format.Alignment = StringAlignment.Center;
			}
#endif

			// draw the signals next, half off the roads. After above because they draw onto adjoining parts.
			Rectangle rectSignal = new Rectangle(0, 0, Sprites.trafficlight_on.Width/divSize,
			                                     Sprites.trafficlight_on.Height/divSize);
			Rectangle rectStop = new Rectangle(0, 0, Sprites.stop.Width/divSize, Sprites.stop.Height/divSize);
			foreach (SignalSquare signalOn in signals)
			{
				if (signalOn.Square.SignalDirection != MapSquare.SIGNAL_DIRECTION.NONE)
				{
					// west bound (east side)
					rectSignal.X = (signalOn.X + 1)*Engine.PixelsPerTile;
					rectSignal.Y = signalOn.Y*Engine.PixelsPerTile - (rectSignal.Height*3)/4;
					Bitmap bmp = signalOn.Square.SignalDirection == MapSquare.SIGNAL_DIRECTION.EAST_WEST_GREEN
					             	? Sprites.trafficlight_green
					             	: (signalOn.Square.SignalDirection == MapSquare.SIGNAL_DIRECTION.EAST_WEST_YELLOW
					             	   	? Sprites.trafficlight_yellow
					             	   	: Sprites.trafficlight_red);
					pea.Graphics.DrawImage(bmp, rectSignal);
					// north bound (south side)
					rectSignal.X = signalOn.X*Engine.PixelsPerTile + (Engine.PixelsPerTile/2) - rectSignal.Width/2;
					rectSignal.Y = (signalOn.Y + 1)*Engine.PixelsPerTile;
					bmp = signalOn.Square.SignalDirection == MapSquare.SIGNAL_DIRECTION.NORTH_SOUTH_GREEN
					      	? Sprites.trafficlight_green
					      	: (signalOn.Square.SignalDirection == MapSquare.SIGNAL_DIRECTION.NORTH_SOUTH_YELLOW
					      	   	? Sprites.trafficlight_yellow
					      	   	: Sprites.trafficlight_red);
					pea.Graphics.DrawImage(bmp, rectSignal);
				}
				if ((signalOn.Square.StopSigns & MapSquare.STOP_SIGNS.STOP_NORTH) != 0)
				{
					rectStop.X = signalOn.X*Engine.PixelsPerTile - rectStop.Width/2;
					rectStop.Y = signalOn.Y*Engine.PixelsPerTile - rectStop.Height;
					pea.Graphics.DrawImage(Sprites.stop, rectStop);
				}
				if ((signalOn.Square.StopSigns & MapSquare.STOP_SIGNS.STOP_EAST) != 0)
				{
					rectStop.X = (signalOn.X + 1)*Engine.PixelsPerTile;
					rectStop.Y = signalOn.Y*Engine.PixelsPerTile - rectStop.Height/2;
					pea.Graphics.DrawImage(Sprites.stop, rectStop);
				}
				if ((signalOn.Square.StopSigns & MapSquare.STOP_SIGNS.STOP_SOUTH) != 0)
				{
					rectStop.X = (signalOn.X + 1)*Engine.PixelsPerTile - rectStop.Width/2;
					rectStop.Y = (signalOn.Y + 1)*Engine.PixelsPerTile;
					pea.Graphics.DrawImage(Sprites.stop, rectStop);
				}
				if ((signalOn.Square.StopSigns & MapSquare.STOP_SIGNS.STOP_WEST) != 0)
				{
					rectStop.X = signalOn.X*Engine.PixelsPerTile - rectStop.Width;
					rectStop.Y = (signalOn.Y + 1)*Engine.PixelsPerTile - rectStop.Height/2;
					pea.Graphics.DrawImage(Sprites.stop, rectStop);
				}
			}

			// want this for the cars
			pea.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

			if (!displayStart)
			{
				// Limo.Location is 24 units per tile
				float adjustLocation = (float) Engine.PixelsPerTile/TileMovement.UNITS_PER_TILE;

				// now the cars
				limoPaintLocations.Clear();
				foreach (Player plyrOn in engine.Players)
				{
					Bitmap bmp = plyrOn.Limo.BitmapAtAngle;
					int x = (int) (plyrOn.Limo.Location.MapPosition.X*adjustLocation);
					int y = (int) (plyrOn.Limo.Location.MapPosition.Y*adjustLocation);
					int width = bmp.Width/divSize;
					int height = bmp.Height/divSize;

					// circle for the AI car(s)
					if (DisplayShadowsAll || (DisplayShadows && plyrOn.TcpGuid != null))
						using (Brush brush = new SolidBrush(plyrOn.TransparentSpriteColor))
						{
							int radius = (Engine.PixelsPerTile*3)/2;
							pea.Graphics.FillEllipse(brush, x - radius/2, y - radius/2, radius, radius);
						}

					limoPaintLocations.Add(new Point(x, y));
					Rectangle rectLimo = new Rectangle(x - width/2, y - height/2, width, height);
					pea.Graphics.DrawImage(bmp, rectLimo);
				}
			}

			// write coordinates
			if (DisplayCoordinates)
			{
				int skip = Engine.PixelsPerTile >= 24 ? 1 : (Engine.PixelsPerTile >= 18 ? 2 : 5);
				for (int x = 0; x < map.Squares.Length; x += skip)
					pea.Graphics.DrawString(Convert.ToString(x), fontOfficeName, Brushes.White,
					                    x*Engine.PixelsPerTile + Engine.PixelsPerTile/2,
					                    - AutoScrollPosition.Y + 12, format);
				for (int y = 0; y < map.Squares[0].Length; y += skip)
					if (y != 0)
						pea.Graphics.DrawString(Convert.ToString(y), fontOfficeName, Brushes.White,
						                    -AutoScrollPosition.X + 12,
						                    y*Engine.PixelsPerTile + Engine.PixelsPerTile/2,
						                    format);
			}
		}

		private class SignalSquare
		{
			public int X { get; private set; }
			public int Y { get; private set; }
			public MapSquare Square { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="T:System.Object"/> class.
			/// </summary>
			public SignalSquare(int x, int y, MapSquare square)
			{
				X = x;
				Y = y;
				Square = square;
			}
		}

		public void NewMap()
		{
			limoPaintLocations.Clear();
			MapDisplay_Load(null, null);
			Invalidate();
		}

		void MapDisplay_Scroll(object sender, ScrollEventArgs e)
		{
			limoPaintLocations.Clear();
			Invalidate();
		}

		private void MapDisplay_Resize(object sender, EventArgs e)
		{
			limoPaintLocations.Clear();
			Invalidate();
		}

		private void MapDisplay_MouseClick(object sender, MouseEventArgs e)
		{
			if (MouseClickedEvent == null)
				return;
			int x = ((e.X - AutoScrollPosition.X) * TileMovement.UNITS_PER_TILE) / Engine.PixelsPerTile;
			int y = ((e.Y - AutoScrollPosition.Y) * TileMovement.UNITS_PER_TILE) / Engine.PixelsPerTile;
			MouseClickedEvent(new Point(x, y), e.Button);
		}
	}
}

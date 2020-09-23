using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TNT.Drawing
{
	/// <summary>
	/// <see cref="Control"/> that provides a grid that can be used to draw on
	/// </summary>
	public class Canvas : Control
	{
		private const int MINIMUM_PADDING = 1000;
		private const int PADDING = 20;

		private int _ScalePercentage = 100;
		private bool _ShowGrid = true;
		private bool AdjustPostion = false;
		private SolidBrush BackgrounBrush = new SolidBrush(Color.White);
		private KeyEventArgs keyEventArgs = null;
		private Point PreviousCursorPosition = Point.Empty;
		private Point PreviousGridPosition;
		private ScrollableControl ScrollableParent = null;

		/// <summary>
		/// The backgrond of the <see cref="Canvas"/>
		/// </summary>
		public Grid Grid { get; set; } = new Grid(Color.Aqua, 10);

		/// <summary>
		/// The <see cref="ScalePercentage"/> represented as a <see cref="float"/>
		/// </summary>
		[Browsable(false)]
		public float Zoom { get { return ScalePercentage / 100F; } }

		/// <summary>
		/// Amount the <see cref="Canvas"/> should be scaled
		/// </summary>
		[Category("Appearance")]
		public int ScalePercentage
		{
			get { return _ScalePercentage; }
			set { _ScalePercentage = value; Refresh(); }
		}

		/// <summary>
		/// Grid visibility
		/// </summary>
		public bool ShowGrid
		{
			get { return _ShowGrid; }
			set
			{
				_ShowGrid = value;
				Refresh();
			}
		}

		/// <summary>
		/// Canvas background color
		/// </summary>
		public Color BackgroundColor
		{
			get { return BackgrounBrush.Color; }
			set { BackgrounBrush.Color = value; Refresh(); }
		}

		/// <summary>
		/// Scaled grid width
		/// </summary>
		protected float ScaledWidth { get { return Grid.Width * Zoom; } }

		/// <summary>
		/// Scaled grid height
		/// </summary>
		protected float ScaledHeight { get { return Grid.Height * Zoom; } }

		/// <summary>
		/// Initializes a <see cref="Canvas"/>
		/// </summary>
		public Canvas(Control parent, int left, int top, int width, int height)
			: base(parent, string.Empty, left, top, width, height)
		{
			DoubleBuffered = true;
			parent.SizeChanged += OnParentResize;
			ScrollableParent = (Parent as ScrollableControl);
			ScrollableParent.AutoScroll = true;
			Grid.OnRefreshRequest = () => { Refresh(); };
		}

		/// <summary>
		/// Fits the grid within the parent
		/// </summary>
		public void Fit()
		{
			var parentWidth = Parent.Width;
			var parentHeight = Parent.Height;
			var gridWidth = Grid.Width;
			var gridHeight = Grid.Height;
			var gridRatio = gridWidth / (gridHeight * 1F);
			var parentRatio = parentWidth / (parentHeight * 1F);
			float newScale;

			if (gridRatio > parentRatio)
			{
				// Width is greater
				newScale = 100 * (parentWidth * 1F) / (gridWidth + PADDING * 2);
			}
			else
			{
				// Height is greater
				newScale = 100 * (parentHeight * 1F) / (gridHeight + PADDING * 2);
			}

			ScalePercentage = Convert.ToInt32(newScale);
			var position = new Point(-(Parent.Width / 2 - Width / 2), -(Parent.Height / 2 - Height / 2));
			ScrollableParent.AutoScrollPosition = position;
		}

		/// <summary>
		/// Draws the grid on the canvas
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			UpdateClientDimensions();
			var graphics = GetTransformedGraphics(e.Graphics);

			graphics.FillRectangle(BackgrounBrush, Grid.Rect);
			if (ShowGrid) Grid.Draw(graphics);

			if (AdjustPostion)
			{
				var previousCanvasPosition = PreviousGridPosition.ToCanvasCoordinates(graphics);
				var currentCanvasPosition = PointToClient(Cursor.Position);
				RepositionToAlignWithMouse(previousCanvasPosition, currentCanvasPosition);
				AdjustPostion = false;
			}

			base.OnPaint(e);
		}

		private void RepositionToAlignWithMouse(Point previousPosition, Point currentPosition)
		{
			var min = 0;
			var deltaPosition = previousPosition.Subtract(currentPosition);
			var dx = -deltaPosition.X;
			var dy = -deltaPosition.Y;
			var newLocation = Location.Subtract(deltaPosition);

			var newLeft = Left + dx;
			var newRight = Right + dx;
			var newTop = Top + dy;
			var newBottom = Bottom + dy;

			if (newLeft > min)
			{
				newLocation.X = min;
			}
			else if (newRight < Parent.Width - min)
			{
				newLocation.X = Parent.Width - min - Width;
			}

			if (newTop > min)
			{
				newLocation.Y = min;
			}
			else if (newBottom < Parent.Height - min)
			{
				newLocation.Y = Parent.Height - min - Height;
			}

			ScrollableParent.AutoScrollPosition = new Point(-newLocation.X, -newLocation.Y);
		}

		/// <summary>
		/// Updates the client's dimensions
		/// </summary>
		private void UpdateClientDimensions()
		{
			// Adjust the width and height of the canvas to fit the drawing canvas
			var newWidth = (int)ScaledWidth > Parent.ClientRectangle.Width ? (int)ScaledWidth : Parent.ClientRectangle.Width;
			var newHeight = (int)ScaledHeight > Parent.ClientRectangle.Height ? (int)ScaledHeight : Parent.ClientRectangle.Height;

			Width = newWidth + MINIMUM_PADDING;
			Height = newHeight + MINIMUM_PADDING;
		}

		/// <summary>
		/// Focus this control when mouse movements are detected
		/// </summary>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			var currentCursorPosition = Cursor.Position;
			var graphics = GetTransformedGraphics();
			var mousePosition = new Point(e.X, e.Y);
			PreviousGridPosition = mousePosition.ToGridCoordinates(graphics);

			if (keyEventArgs?.KeyCode == Keys.Space)
			{
				RepositionToAlignWithMouse(PreviousCursorPosition, currentCursorPosition);
			}

			PreviousCursorPosition = currentCursorPosition;

			Focus();
			base.OnMouseMove(e);
		}

		/// <summary>
		/// Sets <see cref="KeyEventArgs"/>
		/// </summary>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			keyEventArgs = e;
			switch (keyEventArgs.KeyCode)
			{
				case Keys.Space:
					Cursor = Cursors.Hand;
					break;
			}
		}

		/// <summary>
		/// Sets <see cref="KeyEventArgs"/>
		/// </summary>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			keyEventArgs = null;
			Cursor = Cursors.Default;
		}

		/// <summary>
		/// Changes <see cref="ScalePercentage"/>
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			// Size of a scroll delta (seems to be 120)
			var wheelDelta = SystemInformation.MouseWheelScrollDelta;
			// Change, smaller (-1) or larger (1)
			var change = e.Delta / wheelDelta;
			// Amount of change that should be applied to the scale percentage
			//var detents = change / (keyEventArgs?.Shift == true ? 100.0f : 10.0f);
			var detents = change * (keyEventArgs?.Shift == true ? 1 : 10);

			if (keyEventArgs?.Control == true && ScalePercentage + detents > 0)
			{
				// Adjust position when Paint() is called
				AdjustPostion = true;
				var graphics = GetTransformedGraphics();
				var positionOnCanvas = new Point(e.X, e.Y);
				PreviousGridPosition = positionOnCanvas.ToGridCoordinates(graphics);
				ScalePercentage += detents;
				(e as HandledMouseEventArgs)?.Let(h => h.Handled = true);
			}
		}

		/// <summary>
		/// Redraws <see cref="Canvas"/> when the parent's size changes.
		/// </summary>
		private void OnParentResize(object sender, EventArgs e) => Refresh();

		/// <summary>
		/// Returns a <see cref="Graphics"/> that has been transformed
		/// </summary>
		private Graphics GetTransformedGraphics(Graphics graphics = null)
		{
			if (graphics == null) graphics = CreateGraphics();

			// X translation of the drawing  canvas
			var xTranslation = Math.Max((Width - ScaledWidth) / 2, 0);
			// Y translation of the drawing canvas
			var yTranslation = Math.Max((Height - ScaledHeight) / 2, 0);

			// Scale
			graphics.ScaleTransform(Zoom, Zoom, MatrixOrder.Append);
			// Transform
			graphics.TranslateTransform(xTranslation, yTranslation, MatrixOrder.Append);

			return graphics;
		}
	}
}

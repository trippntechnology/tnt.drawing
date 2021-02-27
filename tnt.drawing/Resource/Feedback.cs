using System.Windows.Forms;

namespace TNT.Drawing.Resource
{
	/// <summary>
	/// Represents feed back returned by an object
	/// </summary>
	public class Feedback
	{
		/// <summary>
		/// The <see cref="Cursor"/> that should be used
		/// </summary>
		public Cursor Cursor { get; private set; }

		/// <summary>
		/// The hint that should be displayed
		/// </summary>
		public string Hint { get; private set; }

		/// <summary>
		/// Default constructor
		/// </summary>
		public Feedback(Cursor cursor, string hint)
		{
			Cursor = cursor;
			Hint = hint;
		}
	}
}

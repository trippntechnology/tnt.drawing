using System.Windows.Forms;

namespace TNT.Drawing.Resource
{
	public class Feedback
	{
		public Cursor Cursor { get; private set; }
		public string Hint { get; private set; }

		public Feedback(Cursor cursor, string hint)
		{
			Cursor = cursor;
			Hint = hint;
		}
	}
}

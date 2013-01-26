

using System.Drawing;

namespace WindwardopolisLibrary
{
	public interface ISprite
	{
		// return true to kill it
		bool IncreaseTick();

		Image SpriteBitmap { get; }
	}
}

namespace DrawingCLI
{
	public interface IDrawableCLI
	{
		void Draw();

		void DrawTextOnly() => Draw();
	}
}
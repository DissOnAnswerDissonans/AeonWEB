namespace DrawingCLI
{
	internal interface IDrawableCLI
	{
		void Draw();

		void DrawTextOnly() => Draw();
	}
}

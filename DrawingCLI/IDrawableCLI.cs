namespace DrawingCLI
{
    interface IDrawableCLI
    {
        void Draw();

		void DrawTextOnly() => Draw();
    }
}

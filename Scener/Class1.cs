using DrawingCLI;

namespace Scener;

abstract public class Scene : IDrawableCLI
{
	private LinkedList<Container> _containers;

	private Container _focusContainer;

	public void Draw()
	{
		foreach (var item in _containers) {
			item.Draw();
		}
	}

	public T AddContainer<T>(Rect borders, params IDrawableCLI[] drawables) where T : Container
	{
		T c = (T) Activator.CreateInstance(typeof(T), this, borders);
		c.Init(drawables);

		return c;
	}
}

abstract public class Container : IDrawableCLI
{
	internal Scene _scene;
	internal Rect _borders;
	internal Container(Scene scene, Rect rect) { _scene = scene; _borders = rect; }

	abstract public IDrawableCLI Focus { get; }

	public abstract void Draw();

	public abstract void Init(IDrawableCLI[] drawables);
}



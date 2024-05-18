public abstract class Presenter
{
    protected IView view;

    public Presenter(IView view)
    {
        this.view = view;
    }

    public abstract void InitializePresenter();
    public abstract void HandleEvent(string eventName);
}
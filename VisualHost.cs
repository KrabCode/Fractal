using System;


public class VisualHost : FrameworkElement
{
    // Create a collection of child visual objects.
    private VisualCollection _children;

    public VisualHost()
    {
        _children = new VisualCollection(this);
        _children.Add(CreateDrawingVisualRectangle());
        _children.Add(CreateDrawingVisualText());
        _children.Add(CreateDrawingVisualEllipses());

        // Add the event handler for MouseLeftButtonUp.
        this.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MyVisualHost_MouseLeftButtonUp);
    }


}

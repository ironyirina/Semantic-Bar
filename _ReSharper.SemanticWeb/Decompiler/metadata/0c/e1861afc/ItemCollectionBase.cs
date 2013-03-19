// Type: MindFusion.Diagramming.Wpf.ItemCollectionBase`1
// Assembly: MindFusion.Diagramming.Wpf, Version=3.0.0.25712, Culture=neutral, PublicKeyToken=1080b51628c81789
// Assembly location: C:\Users\Ирина\Documents\Чуприна\SemanticWeb\SemanticWeb\testMindFusion\bin\Debug\MindFusion.Diagramming.Wpf.dll

using System.Windows;

namespace MindFusion.Diagramming.Wpf
{
  public abstract class ItemCollectionBase<T> : CollectionBase<T> where T : DiagramItem
  {
    protected bool notifyParent;
    protected ItemEventArgs itemEventArgs;
    public override void Clear();
    public override void Add(T item);
    public override void Insert(int index, T item);
    public DiagramItem GetAt(int index);
    public override bool Remove(T item);
    public override void RemoveAt(int index);
    protected void RaiseAdding(UIElement item);
    protected void RaiseInserting(DiagramItem item, int index);
    protected void RaiseRemoving(DiagramItem item);
  }
}

// Type: MindFusion.Diagramming.Wpf.Diagram
// Assembly: MindFusion.Diagramming.Wpf, Version=3.0.0.25712, Culture=neutral, PublicKeyToken=1080b51628c81789
// Assembly location: C:\Users\Ирина\Documents\Чуприна\SemanticWeb\SemanticWeb\testMindFusion\bin\Debug\MindFusion.Diagramming.Wpf.dll

using A;
using MindFusion.Diagramming.Wpf.Behaviors;
using MindFusion.Diagramming.Wpf.Commands;
using MindFusion.Diagramming.Wpf.Lanes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Xml;

namespace MindFusion.Diagramming.Wpf
{
  [ContentProperty("Content")]
  [DefaultProperty("MeasureUnit")]
  [DefaultEvent("NodeCreated")]
  [LicenseProvider(typeof (GL))]
  public class Diagram : DiagramBase, IItemFactory
  {
    public static readonly DependencyProperty IsDiagramScrollerProperty;
    public GetManipulatorRect CustomExpandButtonPosition;
    public static readonly DependencyProperty ZoomFactorProperty;
    public static DependencyProperty AlignmentGuidePenProperty;
    public static DependencyProperty NodeTemplateProperty;
    public DummyNode Dummy;
    public static readonly RoutedUICommand NavigateLeft;
    public static readonly RoutedUICommand NavigateRight;
    public static readonly RoutedUICommand NavigateUp;
    public static readonly RoutedUICommand NavigateDown;
    public static readonly DependencyProperty NodeEffectsSourceProperty;
    public static DependencyProperty ShapeNodeStyleProperty;
    public static DependencyProperty TableNodeStyleProperty;
    public static DependencyProperty ContainerNodeStyleProperty;
    public static DependencyProperty TreeViewNodeStyleProperty;
    public static DependencyProperty DiagramLinkStyleProperty;
    public static readonly DependencyProperty MagnifierFactorProperty;
    public static readonly DependencyProperty MagnifierWidthProperty;
    public static readonly DependencyProperty MagnifierHeightProperty;
    public static readonly DependencyProperty MagnifierStyleProperty;
    public Diagram();
    public override void BeginInit();
    public override void EndInit();
    public string SaveToString();
    public string SaveToString(SaveToStringFormat format, bool clearDirty);
    public void LoadFromString(string str);
    object IItemFactory.VM(string A);
    public static void RegisterClass(Type classType, string classId, int classVersion);
    public static void RegisterItemClass(Type itemClass, string classId, int classVersion);
    public void SaveToXml(string fileName);
    public virtual void SaveToXml(XmlDocument document);
    public void LoadFromXml(string fileName);
    public void LoadFromXml(XmlDocument document);
    protected override Visual GetVisualChild(int index);
    public void InvalidateBackground();
    public void ClearAll();
    public SelectionCopy CopySelection(Diagram source, bool unconnectedLinks, bool copyGroups);
    public bool PasteSelection(Diagram doc, SelectionCopy data, CompositeCmd cmd, Vector offset);
    public void Invalidate(object sender, Rect invalidRect);
    public BitmapSource CreateImage();
    public BitmapSource CreateImage(double scale);
    public void DrawStyledText(DrawingContext graphics, string text, ITextAttributes textAttributes, Rect textBounds);
    protected internal virtual void DrawGrid(DrawingContext graphics);
    public Size MeasureString(string text, ITextAttributes textAttributes, int maxWidth);
    protected override void OnVisualParentChanged(DependencyObject oldParent);
    public bool HitTestManipulators(Point pt);
    protected override void OnPreviewMouseDown(MouseButtonEventArgs e);
    protected override void OnPreviewMouseMove(MouseEventArgs e);
    public static bool GetIsDiagramScroller(DependencyObject obj);
    public static void SetIsDiagramScroller(DependencyObject obj, bool value);
    protected override void OnPreviewMouseUp(MouseButtonEventArgs e);
    protected override void OnMouseDown(MouseButtonEventArgs e);
    protected override void OnMouseUp(MouseButtonEventArgs e);
    public void CancelDrag();
    public void BeginEdit(InplaceEditable item);
    public void BeginEdit(InplaceEditable editable, Point mousePosition);
    public void EndEdit(bool accept);
    public void ScrollTo(Point newTopLeft);
    public void ResizeToFitItem(DiagramItem item);
    public void RouteAllLinks();
    public Point PixelToUnit(Point point);
    public Point UnitToPixel(Point point);
    public Rect GetContentBounds(bool forPrint, bool onlyVisible);
    public void ResizeToFitItems(double borderGap);
    public void ResizeToFitItems(double borderGap, bool onlyVisible);
    protected override Size MeasureOverride(Size availableSize);
    protected override Size ArrangeOverride(Size finalSize);
    public bool IsItemVisible(DiagramItem item);
    public bool IsItemInteractive(DiagramItem item);
    public bool IsItemLocked(DiagramItem item);
    public void BringIntoView(DiagramItem item, bool centered);
    public void BringIntoView(DiagramItem item);
    public Point DocToClient(Point docPoint);
    public Rect DocToClient(Rect docRect);
    public Point ClientToDoc(Point clientPoint);
    public Rect ClientToDoc(Rect clientRect);
    public void ZoomIn();
    public void ZoomOut();
    public void ZoomToFit();
    public void ZoomToRect(Rect rect, bool centered);
    public void ZoomToRect(Rect rect);
    public void SetZoomFactor(double zoomFactor, Point pivotPoint);
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void UpdateRuntimeIndices();
    public DiagramNode GetNodeAt(Point point);
    public DiagramNode GetNodeAt(Point point, double threshold);
    public DiagramNodeCollection GetNodesAt(Point point);
    public DiagramNode GetNodeAt(Point point, bool excludeLocked, bool excludeSelected);
    public DiagramLink GetLinkAt(Point pt, double maxDist);
    public DiagramLink GetLinkAt(Point pt, double maxDist, bool exclLocked);
    public DiagramLink GetLinkAt(Point pt, double maxDist, bool exclLocked, ref int segmNum);
    public DiagramItem GetItemAt(Point pt, bool exclLocked);
    public bool CopyToClipboard(bool persist);
    public bool CopyToClipboard(bool persist, bool groups);
    public bool CutToClipboard(bool copy);
    public bool CutToClipboard(bool copy, bool groups);
    public bool PasteFromClipboard(Vector offset);
    public bool PasteFromClipboard(Vector offset, bool unconnectedLinks);
    public DiagramNode FindNode(object tagValue);
    public DiagramLink FindLink(object tagValue);
    public DiagramNode FindNodeById(object id);
    public DiagramLink FindLinkById(object id);
    public Group FindGroup(object tagValue);
    public virtual Point AlignPointToGrid(Point point);
    public DiagramNode GetNearestNode(Point pt, double maxDistance, DiagramNode ignored);
    public DiagramNodeCollection GetNodesInViewport();
    public void ExecuteCommand(Command cmd);
    public static DiagramItem GetDiagramItem(DependencyObject item);
    public static Rect GetItemBounds(UIElement item);
    public static void SetItemBounds(UIElement item, Rect value);
    public void RaiseCreated(DiagramItem item);
    public void RaiseClicked(DiagramItem item, Point mousePosition, MindFusion.Diagramming.Wpf.MouseButton mouseButton);
    public void RaiseDoubleClicked(DiagramItem item, Point mousePosition, MindFusion.Diagramming.Wpf.MouseButton mouseButton);
    public bool RaiseSelecting(DiagramItem item, Point mousePosition);
    public bool RaiseDeleting(DiagramItem item);
    public void RaiseClicked(MindFusion.Diagramming.Wpf.MouseButton mouseButton, Point mousePosition);
    public bool RaiseTreeItemTextEditing(TreeViewItem item);
    public void RaiseDoubleClicked(MindFusion.Diagramming.Wpf.MouseButton mouseButton, Point mousePosition);
    protected override void OnDrop(DragEventArgs e);
    public bool RaiseNodeTextEditing(DiagramNode node);
    public bool RaiseLinkTextEditing(DiagramLink link);
    public bool RaiseCellTextEditing(TableNode table, int column, int row);
    public bool RaiseLaneGridHeaderTextEditing(Header header);
    public void RaiseNodeTextEdited(DiagramNode node, string oldText, string newText);
    public void RaiseLinkTextEdited(DiagramLink link, string oldText, string newText);
    public void RaiseCellTextEdited(TableNode.Cell cell, string oldText, string newText, int column, int row);
    public void RaiseLaneGridHeaderTextEdited(Header header, string oldText, string newText);
    public void RaisePasted(DiagramItem item);
    public Theme CreateThemeFromDefaults();
    protected override int VisualChildrenCount { get; }
    [Browsable(false)]
    public Rect Viewport { get; }
    public override DocumentPaginator DocumentPaginator { get; }
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Factory Factory { get; }
    [Browsable(false)]
    public Grid LaneGrid { get; }
    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("Specifies whether the lane grid is enabled.")]
    public bool EnableLanes { get; set; }
    public InteractionState Interaction { get; }
    [Browsable(false)]
    public DiagramItemCollection Items { get; }
    [Browsable(false)]
    public DiagramNodeCollection Nodes { get; }
    [Browsable(false)]
    public DiagramLinkCollection Links { get; }
    [Browsable(false)]
    public GroupCollection Groups { get; }
    [TypeConverter(typeof (ExpandableObjectConverter))]
    [Browsable(false)]
    [Description("Selection settings.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Selection Selection { get; }
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DiagramItem ActiveItem { get; set; }
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IList<Layer> Layers { get; }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public bool Dirty { get; set; }
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("If enabled, validity checks are performed each time an item is added to the Diagram. That involves enumerating the item collections and can slow up the process considerably for large diagrams. Disable this property to skip the checks, however be sure that you don\'t add an item twice to the diagram and that links are created only between items in the same Diagram.")]
    public bool ValidityChecks { get; set; }
    public UIElement DocumentPlane { get; }
    public GraphicsUnit MeasureUnit { get; set; }
    [DefaultValue(false)]
    [Description("Specifies whether groups should be sorted by Z order when their items are drawn.")]
    [Category("Appearance")]
    public bool SortGroupsByZ { get; set; }
    [DefaultValue(2)]
    [Category("Appearance")]
    [Description("Size of selection handles.")]
    public double AdjustmentHandlesSize { get; set; }
    [Category("Appearance")]
    [DefaultValue(true)]
    [Description("Specifies if selected items should be painted on top of other items or the Z order should be always preserved.")]
    public bool SelectionOnTop { get; set; }
    [Category("Appearance")]
    [Description("Gets or sets when anchor point marks are displayed")]
    [DefaultValue(typeof (ShowAnchors), "Auto")]
    public ShowAnchors ShowAnchors { get; set; }
    [Description("The document area defined by Bounds is painted using this brush.")]
    [Category("Appearance")]
    public Brush BackBrush { get; set; }
    [DefaultValue(typeof (ExpandButtonPosition), "OuterRight")]
    [Category("Appearance")]
    public ExpandButtonPosition ExpandButtonPosition { get; set; }
    [Category("Appearance")]
    [DefaultValue(typeof (LinkCrossings), "Straight")]
    [Description("Specifies how to display intersection points where links cross their paths.")]
    public LinkCrossings LinkCrossings { get; set; }
    [DefaultValue(1.5)]
    [Category("Appearance")]
    [Description("Specifies the radius of arcs displayed over intersection points where links cross their paths.")]
    public double CrossingRadius { get; set; }
    [DefaultValue(1)]
    [Description("Specifies the relative size of arcs displayed at the corners of rounded rectangles.")]
    [Category("Appearance")]
    public double RoundRectFactor { get; set; }
    [DefaultValue(true)]
    [Description("Specifies whether to display selection handles of objects under the mouse while another object is being modified.")]
    public bool ShowHandlesOnDrag { get; set; }
    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("Specifies whether arrow segment joins should be rounded.")]
    public bool RoundedLinks { get; set; }
    [DefaultValue(2)]
    [Category("Appearance")]
    [Description("The radius of the arc segment-joins of rounded links.")]
    public double RoundedLinksRadius { get; set; }
    [DefaultValue(null)]
    [Localizable(true)]
    [Category("Appearance")]
    [Description("The diagram background image.")]
    public ImageSource BackgroundImage { get; set; }
    [Description("Specifies alignment style and position for the background image.")]
    [DefaultValue(typeof (ImageAlign), "Center")]
    [Category("Appearance")]
    public ImageAlign BackgroundImageAlign { get; set; }
    [Category("Appearance")]
    [Description("Manipulation handles of the active object are rendered with this brush.")]
    public HandlesVisualStyle ActiveItemHandlesStyle { get; }
    [Category("Appearance")]
    [Description("Manipulation handles of selected objects are rendered in this color.")]
    public HandlesVisualStyle SelectedItemHandlesStyle { get; }
    [Category("Appearance")]
    [Description("Provides Pen and Brush attributes that let you customize the appearance of disabled adjustment handles.")]
    public HandlesVisualStyle DisabledHandlesStyle { get; }
    [DefaultValue(true)]
    [Category("Appearance")]
    public bool ShowDisabledHandles { get; set; }
    [DefaultValue(true)]
    [Description("Determines whether newly-created objects are automatically selected")]
    [Category("Behavior")]
    public bool SelectAfterCreate { get; set; }
    [Description("Specifies whether users can create or drag objects outside document boundaries.")]
    [Category("Behavior")]
    [DefaultValue(typeof (RestrictToBounds), "Intersection")]
    public RestrictToBounds RestrictItemsToBounds { get; set; }
    [DefaultValue(typeof (HitTestPriority), "NodesBeforeLinks")]
    [Category("Behavior")]
    [Description("Specifies whether nodes should have higher priority than links when hit testing.")]
    public HitTestPriority HitTestPriority { get; set; }
    [Description("Specifies the direction in which node hierarchies are expanded and collapsed.")]
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool ExpandOnIncoming { get; set; }
    [DefaultValue(false)]
    [Category("Behavior")]
    [Description("Enables recursive multi-level expanding of node hierarchies.")]
    public bool RecursiveExpand { get; set; }
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Specifies whether two nodes can be connected one to another with more than one link.")]
    public bool AllowLinksRepeat { get; set; }
    [Category("Behavior")]
    [DefaultValue(typeof (SnapToAnchor), "OnCreate")]
    [Description("Specifies when arrow ends snap to nearest anchor point")]
    public SnapToAnchor SnapToAnchor { get; set; }
    [DefaultValue(false)]
    [Description("Allow drawing links without connecting them to nodes.")]
    [Category("Behavior")]
    public bool AllowUnconnectedLinks { get; set; }
    [Category("Behavior")]
    [Description("Allow attaching links to nodes that have not anchor points.")]
    [DefaultValue(true)]
    public bool AllowUnanchoredLinks { get; set; }
    [Category("Behavior")]
    public Size MinimumNodeSize { get; set; }
    [Category("Behavior")]
    [DefaultValue(typeof (AutoResize), "RightAndDown")]
    [Description("Automatically resize document scrolling area when items are created, deleted or removed.")]
    public AutoResize AutoResize { get; set; }
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Specifies whether arrow end points can be moved after the arrow is created.")]
    public bool LinkEndsMovable { get; set; }
    [Category("Behavior")]
    [DefaultValue(false)]
    [Description("Specifies whether link segments can be added and removed interactively.")]
    public bool AllowSplitLinks { get; set; }
    [Category("Behavior")]
    [Description("Enables or disables creation of reflexive links.")]
    [DefaultValue(true)]
    public bool AllowSelfLoops { get; set; }
    [DefaultValue(false)]
    [Category("Routing")]
    [Description("Initial value of the AutoRoute property of links.")]
    public bool RouteLinks { get; set; }
    [TypeConverter(typeof (ExpandableObjectConverter))]
    [Description("Settings for the link routing algorithm.")]
    [Category("Routing")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public RoutingOptions RoutingOptions { get; }
    [Description("Gets or sets the component used to find paths for auto-routed links.")]
    [Category("Routing")]
    [TypeConverter(typeof (SL))]
    public ILinkRouter LinkRouter { get; set; }
    [Category("Behavior")]
    [DefaultValue(typeof (ExpandButtonAction), "ExpandTreeBranch")]
    [Description("Specifies the behavior of the +/- buttons displayed beside expandable nodes.")]
    public ExpandButtonAction ExpandButtonAction { get; set; }
    public IList Content { get; }
    [DefaultValue(0)]
    [Category("Behavior")]
    [Description("The maximum distance between adjacent control points of an arrow at which the respective segments can be merged.")]
    public double MergeThreshold { get; set; }
    [DefaultValue(true)]
    [Category("Grid")]
    [Description("Specifies whether to align items to grid points.")]
    public bool AlignToGrid { get; set; }
    [DefaultValue(false)]
    [Description("Shows or hides the grid points.")]
    [Category("Grid")]
    public bool ShowGrid { get; set; }
    [Description("Gets or sets the color used to paint grid dots or lines.")]
    [Category("Grid")]
    public Pen GridPen { get; set; }
    [Description("The horizontal distance between grid points.")]
    [DefaultValue(4)]
    [Category("Grid")]
    public double GridSizeX { get; set; }
    [DefaultValue(4)]
    [Category("Grid")]
    [Description("The vertical distance between grid points.")]
    public double GridSizeY { get; set; }
    [Category("Grid")]
    [DefaultValue(double.NaN)]
    [Description("Specifies the horizontal offset of the first column of grid points.")]
    public double GridOffsetX { get; set; }
    [DefaultValue(double.NaN)]
    [Category("Grid")]
    [Description("Specifies the vertical offset of the first row of grid points.")]
    public double GridOffsetY { get; set; }
    [Description("Indicates how to draw the grid.")]
    [DefaultValue(GridStyle.Points)]
    [Category("Grid")]
    public GridStyle GridStyle { get; set; }
    [Category("Layout")]
    public Rect Bounds { get; set; }
    [Browsable(false)]
    public ScriptHelper ScriptHelper { get; }
    [Browsable(false)]
    public UndoManager UndoManager { get; }
    public bool IsTrialVersion { get; }
    public bool IsProEdition { get; }
    public bool NowLoading { get; }
    Hashtable IItemFactory.TypeTable { get; }
    Hashtable IItemFactory.ClsidTable { get; }
    [Description("A value specifying how far from a link a click is still considered a hit.")]
    [Category("Behavior")]
    [NotifyParentProperty(true)]
    [DefaultValue(0)]
    public double LinkHitDistance { get; set; }
    [Description("A Brush used to fill highlighted table rows.")]
    [Category("Appearance")]
    public Brush RowHighlightBrush { get; set; }
    [Description("Specifies whether a table row should be highlighted when the user clicks on it.")]
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool AutoHighlightRows { get; set; }
    [Description("Specifies the type and appearance of DiagramNodeAdapter selection handles.")]
    [DefaultValue(typeof (HandlesStyle), "HatchHandles")]
    [Category("Defaults")]
    public HandlesStyle AdapterHandlesStyle { get; set; }
    [Browsable(false)]
    public Type DefaultControlType { get; set; }
    [DefaultValue(typeof (ControlMouseAction), "SelectNode")]
    [Category("Defaults")]
    public ControlMouseAction ControlMouseAction { get; set; }
    [Description("Allows activating inplace editing mode by double clicking.")]
    [DefaultValue(false)]
    [Category("Inplace text editing")]
    public bool AllowInplaceEdit { get; set; }
    [Category("Inplace text editing")]
    [DefaultValue(false)]
    [Description("End inplace editing and accept changes when ENTER is pressed.")]
    public bool InplaceEditAcceptOnEnter { get; set; }
    [Description("End inplace editing and cancel changes when ESC is pressed.")]
    [Category("Inplace text editing")]
    [DefaultValue(true)]
    public bool InplaceEditCancelOnEsc { get; set; }
    [Category("Inplace text editing")]
    [Description("Font of text being edited inplace.")]
    public Font InplaceEditFont { get; set; }
    [Description("Specifies if objects have to be selected in order to modify them.")]
    [Category("Behavior")]
    [DefaultValue(typeof (ModificationStart), "SelectedOnly")]
    public ModificationStart ModificationStart { get; set; }
    [Category("Behavior")]
    public ModificationEffect ModificationEffect { get; set; }
    [Description("Specifies whether multiple selected nodes can be resized simultaneously.")]
    [Category("Behavior")]
    [DefaultValue(false)]
    public bool AllowMultipleResize { get; set; }
    [Category("Mouse cursors")]
    [Description("Cursor displayed when the mouse pointer is over empty document area.")]
    public Cursor PointerCursor { get; set; }
    [Description("Cursor displayed when an object cannot be created.")]
    [Category("Mouse cursors")]
    public Cursor DisallowCursor { get; set; }
    [Category("Mouse cursors")]
    [Description("Cursor displayed when an object can be modified.")]
    public Cursor MoveCursor { get; set; }
    [Description("Cursor displayed when dragging the mouse will create an arrow.")]
    [Category("Mouse cursors")]
    public Cursor DrawLinkCursor { get; set; }
    [Category("Mouse cursors")]
    [Description("Cursor displayed when an arrow can be created.")]
    public Cursor AllowLinkCursor { get; set; }
    [Description("Cursor displayed if an arrow cannot be created.")]
    [Category("Mouse cursors")]
    public Cursor DisallowLinkCursor { get; set; }
    [Description("Indicates that an object would be resized horizontally.")]
    [Category("Mouse cursors")]
    public Cursor HorizontalResizeCursor { get; set; }
    [Category("Mouse cursors")]
    [Description("Indicates that dragging a selection handle would rotate the selected node.")]
    public Cursor RotateCursor { get; set; }
    [Category("Mouse cursors")]
    [Description("Indicates that an object would be resized vertically.")]
    public Cursor VerticalResizeCursor { get; set; }
    [Description("Displayed while panning the view.")]
    [Category("Mouse cursors")]
    public Cursor PanCursor { get; set; }
    [Browsable(false)]
    public Cursor OverrideCursor { get; set; }
    [Category("Mouse cursors")]
    [Description("Indicates that an object would be resized in both directions.")]
    public Cursor DiagonalResizeCursor { get; set; }
    [Description("Indicates that an object would be resized in both directions.")]
    [Category("Mouse cursors")]
    public Cursor CounterDiagonalResizeCursor { get; set; }
    [DefaultValue(true)]
    [Category("Behavior")]
    [Description("Enables automatic scrolling when the mouse is dragged outside the document boundaries.")]
    public bool AutoScroll { get; set; }
    [Browsable(false)]
    public ModifierKeyActions ModifierKeyActions { get; }
    [Category("Behavior")]
    [Description("Specifies how the control responds to users actions with the mouse.")]
    [DefaultValue(Behavior.LinkShapes)]
    public Behavior Behavior { get; set; }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public BehaviorBase CustomBehavior { get; set; }
    [Category("Layout")]
    [DefaultValue(0)]
    public double ScrollX { get; set; }
    [DefaultValue(0)]
    [Category("Layout")]
    public double ScrollY { get; set; }
    [Category("Layout")]
    [DefaultValue(100)]
    public double ZoomFactor { get; set; }
    [Description("Specifies the function of the middle mouse button.")]
    [DefaultValue(typeof (MouseButtonActions), "None")]
    [Category("Behavior")]
    public MouseButtonActions MiddleButtonActions { get; set; }
    [DefaultValue(typeof (MouseButtonActions), "Cancel")]
    [Category("Behavior")]
    [Description("Specifies the function of the right mouse button.")]
    public MouseButtonActions RightButtonActions { get; set; }
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Type CustomNodeType { get; set; }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public Type CustomLinkType { get; set; }
    [Category("Behavior")]
    [DefaultValue(typeof (DelKeyAction), "DeleteSelectedItems")]
    [Description("Specifies what action should be performed when the user hits the Del key.")]
    public DelKeyAction DelKeyAction { get; set; }
    [Category("Behavior")]
    [DefaultValue(false)]
    [Description("Specifies whether nodes should automatically align to the borders ot other nodes during user interaction.")]
    public bool AutoAlignNodes { get; set; }
    [Category("Behavior")]
    [Description("Specifies the maximal horizontal or vertical distance at which a node aligns to the borders ot other nodes.")]
    [DefaultValue(8)]
    public double AutoAlignDistance { get; set; }
    [Description("Specifies the pen used to draw alignment guides.")]
    public Pen AlignmentGuidePen { get; set; }
    [DefaultValue("[Font: Name=Microsoft Sans Serif, Size=2.9, Units=0, GdiCharSet=1, GdiVerticalFont=False]")]
    [Obsolete("Use a Theme object to specify default values for diagram items.")]
    [Localizable(true)]
    [Category("Defaults")]
    [Description("The default font assigned to newly created objects.")]
    public Font Font { get; set; }
    [DefaultValue(4)]
    [Description("Initial number of rows in new tables.")]
    [Category("Defaults")]
    public int TableRowCount { get; set; }
    [Description("Initial number of columns in new tables.")]
    [DefaultValue(2)]
    [Category("Defaults")]
    public int TableColumnCount { get; set; }
    [Description("The initial value of the CustomDraw property of shape nodes.")]
    [Category("Defaults")]
    [DefaultValue(typeof (CustomDraw), "None")]
    public CustomDraw ShapeCustomDraw { get; set; }
    [Category("Defaults")]
    [Description("Gets or sets the default value for the CustomDraw property of new tables.")]
    [DefaultValue(typeof (CustomDraw), "None")]
    public CustomDraw TableCustomDraw { get; set; }
    [DefaultValue(typeof (CustomDraw), "None")]
    [Description("Gets or sets the default value for the CellCustomDraw property of new tables.")]
    [Category("Defaults")]
    public CustomDraw CellCustomDraw { get; set; }
    [Category("Defaults")]
    [DefaultValue(typeof (CustomDraw), "None")]
    [Description("The initial value of the CustomDraw property of links.")]
    public CustomDraw LinkCustomDraw { get; set; }
    [DefaultValue(false)]
    [Category("Defaults")]
    [Description("Initial value of links\' Dynamic property, specifying whether links automatically adjust the coordinates of their end points to align them to node outlines.")]
    public bool DynamicLinks { get; set; }
    [Category("Defaults")]
    [Description("Initial value of links\' AutoSnapToNode property, specifying whether arrow end points are automatically aligned to node borders.")]
    [DefaultValue(false)]
    public bool AutoSnapLinks { get; set; }
    [DefaultValue(60)]
    [Description("Specifies the maximum distance at which links auto-snap to nodes.")]
    [Category("Behavior")]
    public double AutoSnapDistance { get; set; }
    [Category("Defaults")]
    [Description("Initial value of links\' RetainForm property, specifying whether links automatically adjust the coordinates of their points so their relative initial position remains the same.")]
    [DefaultValue(false)]
    public bool LinksRetainForm { get; set; }
    [Description("The default width of table columns.")]
    [Category("Defaults")]
    [DefaultValue(18)]
    public double TableColumnWidth { get; set; }
    [Description("The default height of table rows.")]
    [Category("Defaults")]
    [DefaultValue(6)]
    public double TableRowHeight { get; set; }
    [DefaultValue(5)]
    [Category("Defaults")]
    [Description("Default height of table captions.")]
    public double TableCaptionHeight { get; set; }
    [Category("Defaults")]
    [Description("Specifies how tables can be related one to another - as integral entities, by rows, or both.")]
    [DefaultValue(typeof (TableConnectionStyle), "Rows")]
    public TableConnectionStyle TableConnectionStyle { get; set; }
    [Category("Behavior")]
    [DefaultValue(typeof (Orientation), "Auto")]
    [Description("Orientation of the first segment of cascading links.")]
    public Orientation LinkCascadeOrientation { get; set; }
    [Category("Defaults")]
    [Description("Gets or sets a value specifying the visual style of cell border lines.")]
    [DefaultValue(typeof (CellFrameStyle), "System3D")]
    public CellFrameStyle CellFrameStyle { get; set; }
    [DefaultValue(false)]
    [Category("Defaults")]
    [Description("Specifies the initial value of the Expandable property of new nodes.")]
    public bool NodesExpandable { get; set; }
    [DefaultValue(false)]
    [Category("Defaults")]
    public bool PolygonalTextLayout { get; set; }
    [DefaultValue(0)]
    [Category("Defaults")]
    public double ShapeOrientation { get; set; }
    [Category("Defaults")]
    [DefaultValue(false)]
    public bool EnableStyledText { get; set; }
    [Description("Specifies what shape to display at links\' end points.")]
    [Category("Defaults")]
    public Shape LinkHeadShape { get; set; }
    [Description("Specifies what shape to display at links\' origin points.")]
    [Category("Defaults")]
    public Shape LinkBaseShape { get; set; }
    [Category("Defaults")]
    [Description("Specifies what shape to display at the middles of links segments.")]
    public Shape LinkIntermediateShape { get; set; }
    [Description("Gets or sets the default size of LinkHeadShape shapes.")]
    [Category("Defaults")]
    [DefaultValue(5)]
    public double LinkHeadShapeSize { get; set; }
    [Category("Defaults")]
    [Description("Size of LinkBaseShape shapes.")]
    [DefaultValue(5)]
    public double LinkBaseShapeSize { get; set; }
    [Category("Defaults")]
    [Description("Gets or sets the default size of LinkIntermediateShape shapes.")]
    [DefaultValue(5)]
    public double LinkIntermediateShapeSize { get; set; }
    [DefaultValue(typeof (LinkTextStyle), "Center")]
    [Description("Specifies the orientation and placement of links text.")]
    [Category("Defaults")]
    public LinkTextStyle LinkTextStyle { get; set; }
    [Category("Defaults")]
    [Obsolete("Use a Theme object to specify default values for diagram items.")]
    [Description("Pen used to paint shape frame lines.")]
    public Pen ShapePen { get; set; }
    [Category("Defaults")]
    [Description("Brush used to fill interior of shapes.")]
    [Obsolete("Use a Theme object to specify default values for diagram items.")]
    public Brush ShapeBrush { get; set; }
    [Category("Defaults")]
    [Obsolete("Use a Theme object to specify default values for diagram items.")]
    [Description("Pen used to paint arrow lines.")]
    public Pen LinkPen { get; set; }
    [Obsolete("Use a Theme object to specify default values for diagram items.")]
    [Description("Brush used to fill interior of arrowheads.")]
    [Category("Defaults")]
    public Brush LinkBrush { get; set; }
    [Category("Defaults")]
    [Obsolete("Use a Theme object to specify default values for diagram items.")]
    [Description("Pen used to paint table frame lines.")]
    public Pen TablePen { get; set; }
    [Description("Brush used to fill interior of tables.")]
    [Category("Defaults")]
    [Obsolete("Use a Theme object to specify default values for diagram items.")]
    public Brush TableBrush { get; set; }
    [Description("Specifies the default shape of tables.")]
    [DefaultValue(typeof (SimpleShape), "Rectangle")]
    [Category("Defaults")]
    public SimpleShape TableShape { get; set; }
    [Category("Defaults")]
    [DefaultValue(null)]
    [Description("Specifies the default value for UIElement.Effect property of new items.")]
    public Effect DefaultEffect { get; set; }
    [Category("Defaults")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Description("The default alignment and formatting style for text displayed inside nodes and table cells.")]
    public StringFormat TextFormat { get; set; }
    [Category("Defaults")]
    [DefaultValue(false)]
    [Description("Specifies the initial value of the Scrollable property of new tables.")]
    public bool TablesScrollable { get; set; }
    [Description("The default text brush.")]
    [Obsolete("Use a Theme object to specify default values for diagram items.")]
    [DefaultValue(typeof (Brush), "Black")]
    [Category("Defaults")]
    public Brush TextBrush { get; set; }
    [Description("Specifies the initial shape of new links.")]
    [DefaultValue(typeof (LinkShape), "Polyline")]
    [Category("Defaults")]
    public LinkShape LinkShape { get; set; }
    [DefaultValue(1)]
    [Description("Specifies the initial number of segments of new links.")]
    [Category("Defaults")]
    public short LinkSegments { get; set; }
    [Description("Specifies the type and appearance of arrow selection handles.")]
    [DefaultValue(typeof (HandlesStyle), "SquareHandles")]
    [Category("Defaults")]
    public HandlesStyle LinkHandlesStyle { get; set; }
    [Description("Specifies the type and appearance of shape node selection handles.")]
    [Category("Defaults")]
    [DefaultValue(typeof (HandlesStyle), "SquareHandles")]
    public HandlesStyle ShapeHandlesStyle { get; set; }
    [Category("Defaults")]
    [Description("Specifies the default value for the HandlesStyle property of new tables.")]
    [DefaultValue(typeof (HandlesStyle), "DashFrame")]
    public HandlesStyle TableHandlesStyle { get; set; }
    [Browsable(false)]
    public Shape DefaultShape { get; set; }
    public DataTemplate NodeTemplate { get; set; }
    [Description("Default table caption.")]
    [Category("Defaults")]
    [DefaultValue("Table")]
    public string TableCaption { get; set; }
    [Description("Default value of the Text property of shape nodes.")]
    [Category("Defaults")]
    [DefaultValue("")]
    public string ShapeText { get; set; }
    [DefaultValue("")]
    [Category("Defaults")]
    [Description("Default value of the Text property of links.")]
    public string LinkText { get; set; }
    [Description("The Pen used to draw container frame lines when a node is dragged over a container.")]
    [Category("Defaults")]
    public Pen ContainerHighlightPen { get; set; }
    [Description("How much space should be left between contained nodes and the container borders.")]
    [Category("Defaults")]
    [DefaultValue(10)]
    public double ContainerMargin { get; set; }
    [Description("Specifies the minimum size of a container. The container won\'t shrink smaller than that size, even if it does not contain any items.")]
    [Category("Defaults")]
    public Size ContainerMinimumSize { get; set; }
    [DefaultValue("Container")]
    [Description("Specifies the default container caption text.")]
    [Category("Defaults")]
    public string ContainerCaption { get; set; }
    [DefaultValue(5)]
    [Category("Defaults")]
    [Description("Specifies the default container caption height")]
    public double ContainerCaptionHeight { get; set; }
    [DefaultValue(true)]
    [Description("Specifies whether new containers should be made foldable.")]
    [Category("Defaults")]
    public bool ContainersFoldable { get; set; }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public int ActiveLayer { get; set; }
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IList<NodeEffect> NodeEffects { get; }
    public Theme Theme { get; set; }
    public System.Windows.Style ShapeNodeStyle { get; set; }
    public System.Windows.Style TableNodeStyle { get; set; }
    public System.Windows.Style ContainerNodeStyle { get; set; }
    public System.Windows.Style TreeViewNodeStyle { get; set; }
    public System.Windows.Style DiagramLinkStyle { get; set; }
    [Category("Magnifier")]
    [DefaultValue(false)]
    [Description("Specifies whether the magnifier tool is currently enabled.")]
    public bool MagnifierEnabled { get; set; }
    [Description("The zoom factor of the magnifier tool.")]
    [Category("Magnifier")]
    [DefaultValue(150)]
    public double MagnifierFactor { get; set; }
    [Category("Magnifier")]
    [DefaultValue(150)]
    [Description("The width of the magnifier tool.")]
    public double MagnifierWidth { get; set; }
    [DefaultValue(150)]
    [Category("Magnifier")]
    [Description("The height of the magnifier tool.")]
    public double MagnifierHeight { get; set; }
    [Category("Magnifier")]
    [Description("The style of the magnifier tool.")]
    public System.Windows.Style MagnifierStyle { get; set; }
    event EventHandler<InstantiateItemEventArgs> IItemFactory.InstantiateItem;
  }
}

   M S B A M L   `   `   `  ����  CompositeLayoutF    @MindFusion.Diagramming.Wpf.Samples.CS.CompositeLayout.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      e aMindFusion.Diagramming.Wpf, Version=3.0.0.25712, Culture=neutral, PublicKeyToken=1080b51628c81789/diag$http://mindfusion.eu/diagramming/wpf  5      -   5          � Title$5  /MindFusion.Diagramming Sample: Composite Layout��5      $	��600��5      $	��800��6    � Icon$ /App.ico��6   .��5      � -   5         �Name$ 
LayoutRoot��5       � RowDefinitions 5      �� 5      $
��Auto �6   �� 5      �� 5      $
��Auto �6   .+�5      �� $��5q�6    �� Orientation$ 
Horizontal=�6   .+�5      �� $��0,0,10,0q�6    �� CornerRadius$ 5��6   
�������$��1q�6B    �� Padding$ 5q�6V   ._�5      �� $ 
Horizontal=�6   .+�5      �� -   5        $��
saveButton��5      $��0,0,5,0q�6    $��70��61   $��Сохранить��6<   6k   �� 5      -   5        $��
loadButton��5      $��0,0,5,0q�6    $��70��61   $��Отменить��6<   6j   5      5      �� 5       $��0,0,10,0q�6   $ 5��6+   
�������$��1q�6O   $ 5q�6c   ._�5!      �� $ 
Horizontal=�6   .+�5"      �� -   5        $��zoomInButton��5"      $��0,0,5,0q�6"   $��70��63   $��Zoom In��6>   6P   �� 5#      -   5        $��zoomOutButton��5#      $��0,0,5,0q�62   $��70��6C   $��Zoom Out��6N   6   �� 5$      -   5        $��	fitButton��5$      $��0,0,5,0q�6   $��70��60   $	��Fit��6;   6c   �� 5%      -   5        $��noZoomButton��5%      $��70��6"   $��No Zoom��6-   6\   5&      5'      5(      �� 5*      $��1��6   $
��Auto=�6(   $��0q�6M   .��5+      (  "MindFusion.Diagramming.Wpf.Diagram  -	   5          Name$ DD��5+         	BackBrush
 �����	   AllowInplaceEdit	 . 
   InplaceEditAcceptOnEnter
 .    DiagramLinkStyle 5,      �� 5-       �� 
TargetType,  &MindFusion.Diagramming.Wpf.DiagramLink"  6   .�5.      �� ,  &MindFusion.Diagramming.Wpf.DiagramItem �� Property �@ Brush �� Value
 �   �6    5/         ShapeNodeStyle 51      �� 52      *  $MindFusion.Diagramming.Wpf.ShapeNode"  6   .�53      ��  �@ Brush �	#FFB0C4DE6    54      56      57      � 59      $��2��6	   .+�5:      9� -
   5         9�Name$ expanderErrList��5:      $��Bottom=�60   $#��Список действий��6H   
������ $
��Auto��6z   .��5;      �� .��5<      �� -   5         ��Name$ 
ListBoxLog��5<      5>      5?      5@      5A      5C      5E      
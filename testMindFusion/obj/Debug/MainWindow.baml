   M S B A M L   `   `   `  ����I  EtextMindFusion, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:textMindFusiontextMindFusion  5   	   J FDiagramControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullJ6clr-namespace:DiagramControls;assembly=DiagramControlsDiagramControls 5   	       textMindFusion.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6K   7textMindFusionclr-namespace:textMindFusion     5   	   RDiagramControls6clr-namespace:DiagramControls;assembly=DiagramControls     5   	   -   5        $��MyWindow��5   W     � Title$+  %Визуальный редактор��6	   $	��600��6%   $	��800��62    � WindowState$ 	Maximized=�5   	      WindowBackground#��[  6!    J� CommandBindings 5      )  #System.Windows.Input.CommandBinding  5	   
   -   5           Command$	 New��5	      5
   2     5   
   -   5        $
 Open��5      5   3     5   
   -   5        $( "textMindFusion:MainWindow.LoadDemo��5      5   7     5   
   -   5        $
 Save��5      5   7     5   
   -   5        $ SaveAs��5      5   7     5   
   -   5        $ Close��5      5   ;     5   
   -   5        $( "textMindFusion:MainWindow.LoadData��5      5    ;     5"   
   -	   5        $' !textMindFusion:MainWindow.Consult��5"      5$   :     5&   
   -
   5        $&  textMindFusion:MainWindow.Cancel��5&      5(   @     5*   
   -   5        $- 'textMindFusion:MainWindow.ZoomInCommand��5*      5,   9     5.   
   -   5        $. (textMindFusion:MainWindow.ZoomOutCommand��5.      50   :     52   
   -   5        $. (textMindFusion:MainWindow.FitSizeCommand��52      54   :     56   
   -   5        $- 'textMindFusion:MainWindow.NoZoomCommand��56      58   9   .��5;      � -   5         �Name$ 
LayoutRoot��5;       � RowDefinitions 5<   
   �� 5=      $
��Auto �6   6,   �� 5>      $
��Auto �6   �� 5?      $��* �6   �� 5@      $
��Auto �6   �� 5A      $
��Auto �6   �� 5B      $
��Auto �6   .+�5E   
   o� $��0��6   
������ 
����ح�.@�5F      m� $��Файл��6   .@�5G      m�  m� Command$	 New��6   6+   m� 5H      $
 Open��6   6,   m� 5I      $( "textMindFusion:MainWindow.LoadDemo��6   6J   �� 5J      6   m� 5K      $
 Save��6   6,   m� 5L      $ SaveAs��6   6.   �� 5M      6   m� 5N      $ Close��6   6-   5P      m� 5Q      $"��Редактирование��6   .@�5R      m� $+��%Приобретение знаний��6   $( "textMindFusion:MainWindow.LoadData��68   6g   5S      m� 5T      $��Консультация��6   .@�5U      m� $+��%Начать консультацию��6   $' !textMindFusion:MainWindow.Consult��5V      6I   5W      5X      �� 5Z   
   -   5        $��TopStackPanel��5Z      $��1��6*   $��5q�67    �� Orientation$ 
Horizontal=�6B   .+�5\      ��  �� CornerRadius$ 5��6   
�������$��1q�6:   	 �� Padding$	 2q�6N   ._�5]      �� $ 
Horizontal=�6   .+�5^      �� -   5        $��
saveButton��5^      $��40��6/   $
��Save��6;   $	��Top=�5_      
������ .��5`      �� $��img2/save.png��6    6:   
 � ToolTip$+
 %Сохранить изменения��5a   M   5b      �� 5c      -   5        $��
loadButton��5c      $��40��60   $��40��6;   $	��Top=�6G   $&�� textMindFusion:MainWindow.Cancel��6`   
������ .��5e      �� $��img2/undo.png��6    6:   $)
 #Отменить изменения��5f   L   5g      5h      5i      �� 5k      $ 5��6   
�������$��1q�6:   $	 2q�6N   ._�5l      �� $ 
Horizontal=�6   .+�5m      �� -   5        $��zoomInButton��5m      $��40��62   $��40��6=   $	��Top=�6I   $-��'textMindFusion:MainWindow.ZoomInCommand��5n      
������ {�.  .��5p      �� $��img2/zoom_in.png��6    6=   $'
 !Увеличить масштаб��5q   K   5r      �� 5s      -   5        $��zoomOutButton��5s      $��40��63   $��40��6>   $	��Top=�6J   $.��(textMindFusion:MainWindow.ZoomOutCommand��5t      
������ {�.  $'
 !Уменьшить масштаб��5v   K   .��5w      �� $��img2/zoom_out.png��6    6=   5x      �� 5y      -   5        $��	fitButton��5y      $��40��6.   $��40��69   $	��Top=�6E   $.��(textMindFusion:MainWindow.FitSizeCommand��5z      
������ {�.  $'
 !Подогнать масштаб��5|   K   .��5}      �� $��img/fit2.png��6    69   5~      �� 5      -   5        $��noZoomButton��5      $��40��61   $��40��6<   $	��Top=�6H   $
��100%��6`   $-��'textMindFusion:MainWindow.NoZoomCommand��5�      
������ {�.  $��10(�5�   H   5�      5�      5�      �� 5�      
�������$��1q�6(   $	 2q�6<   ._�5�      �� $ 
Horizontal=�6   .+�5�      �� -   5        $��AddNodeButton��5�      $��40��63   $��40��6>   $	��Top=�6J   
������ {�. .��5�      �� $��img2/add.png��6    68   $%
 Добавить вершину��5�   J   5�      �� 5�      -   5        $��ChangeNodeButton��5�      $��40��66   $��40��6A   $	��Top=�6M   
������ {�. .��5�      �� $��img2/edit.png��6    69   $%
 Изменить вершину��5�   M   5�      �� 5�      -   5        $��DeleteNodeButton��5�      $��40��66   $��40��6A   $	��Top=�6M   
������ {�. .��5�      �� $��img2/remove.png��6    6;   $#
 Удалить вершину��5�   M   5�      5�      5�      5�      $  DiagramControls.DiagramControl  5�   
   -   5          Name$ MyDiag��5�   7   $��2��6)   6I   � 5�      $��5��6   .+�5�      9� -   5         9�Name$ expanderErrList��5�      $��Bottom=�60   $��Отчёты.��6H   
������ $
��Auto��6r   .��5�      �� .@�5�      �� $��Действия��6   .��5�      �� .��5�      �� -   5         ��Name$ 
ListBoxLog��5�   &    � DataContext 6:   ��  �� Path$ Messages�6b   5�      5�      �� 5�      $��Список ошибок��6   .��5�      �� .��5�      �� -   5        $ ListBoxValidation��5�   &    6A   �� 6[   5�      5�      5�      5�      5�      �� 5�   
   -   5        $��checkBoxIsEdit��5�       $��4��6   $-��'Режим редактирования��66    d� 	IsChecked$
 TrueY�6i   5�      5�      
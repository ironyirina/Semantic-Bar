   M S B A M L   `   `   `  ����\  XMindFusion.UI.Wpf, Version=1.0.0.10002, Culture=neutral, PublicKeyToken=2e742e0ce9634ec0P:clr-namespace:MindFusion.UI.Wpf;assembly=MindFusion.UI.WpfMindFusion.UI.Wpf  5   	    textMindFusion   textMindFusion.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   e aMindFusion.Diagramming.Wpf, Version=3.0.0.25712, Culture=neutral, PublicKeyToken=1080b51628c81789/diag$http://mindfusion.eu/diagramming/wpf  6@   DWpf:clr-namespace:MindFusion.UI.Wpf;assembly=MindFusion.UI.Wpf  5   	   -   5          � Title$  
MainWindow��5   	   $	��700��6   $
��1000��6)    � 	Resources 5         ReadySBStyle �� Property �� Value � Offset � Color  
EndSBStyle��%�  &	        &	 �     �� 5   
   .�5	      ��  � �� 5
      �� 5      .��5      � $ 0Q�6'   $ GreenYellow��62   � 5      $	 0.7Q�6'   $ White��64   � 5      $ 1Q�6'   $ GreenYellow��62   5      5      5      �� 5   
   .�5      ��  � �� 5      �� 5      .��5      � $ 0Q�6'   $ Azure��62   � 5      $	 0.7Q�6'   $ White��64   � 5      $ 1Q�6'   $ Azure��62   5      5      5      5      ��5"      �� 5#   
   .��5$      � $ 	LightCyan��6   $ 0Q�6-   � 5%      $ White��6   $	 0.8Q�6)   � 5&      $ 	LightCyan��6   $ 1Q�6-   5'      .��5)      ]� ��. .+�5*   
   o� $	��Top=�6   .@�5+      m� $��Файл��6   .@�5,      m� $��Создать��6   m� 5-      $��Открыть��6   m� 5.      $��Сохранить��6   m� 5/      $��Сохранть как..��6   �� 50      m� 51      $��
Выход��6   52      53      �� 55   
   -   5         ��Name$ sbState��55      $��Bottom=�6%   #��[  6=   .@�56      �� $��Готово��6   �� 57      $��Right=�6   .��58      �� 59      5:      9� 5<   
   -   5         9�Name$ expanderErrList��5<      $��Bottom=�6,   $��Список ошибок��6D   
������  9� 
IsExpanded . .��5=      �� .@�5>      �� .��6   16!   �� 5?      .��6   26!   �� 5@      .��6   36!   5B      5C      � 5D   
   $��Bottom=�6   	 � ColumnDefinitions	 5E      �� 5F      $	��16* �6#   �� 5G      $
��Auto �6#   �� 5H      $
��0.5* �6#   �� 5I      $
��auto �6'   .+�5K      �� -   5        $��1��5K      $��Right=�6&   $��28��6B   $��Bottom=�6O   $	��400��6j   
 � SmallChange$
 1Q�6�    � Maximum$ 8Q�6�    � Minimum$ -8Q�6�    �� TickPlacement$ TopLeft=�6�   5L      (  "MindFusion.Diagramming.Wpf.Diagram  5M      -   5          Name$ DD��5M      ���	#FF404040   AllowInplaceEdit .    
ZoomFactor$	 200Q�5V         	BackBrush 5Z   9   Z� 5\      5]      
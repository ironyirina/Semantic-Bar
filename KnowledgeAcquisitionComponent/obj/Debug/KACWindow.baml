   M S B A M L   `   `   `  ����X  TKnowledgeAcquisitionComponent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:KnowledgeAcquisitionComponentKnowledgeAcquisitionComponent  5   	   -    'KnowledgeAcquisitionComponent.KacWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   UKnowledgeAcquisitionComponent+clr-namespace:KnowledgeAcquisitionComponent     5   	     � Title$+  %Приобретение знаний��6[   $	��187��6w   $	��650��6�    � 	Resources 5         ButtonStyle �� Property �� Value  
LabelStyle  TextBoxStyle��%�  &	        &	     &	 �    �� 5   
   .�5      ��  � ��$ 12q�6.   6;   �� 5       � ��$ 	10,5,10,5q�6/   6C   �� 5	       � ��
 ���� 6H   �� 5
       � ��
 ��ح�6G   �� 5       � ��$ 16(�60   6=   �� 5       � ��$ Stretch=�6;   6M   5      �� 5   
   .�5      ��  � ��$ Center=�6:   6K   �� 5       � ��$ 10q�6-   6:   �� 5       � ��$ 16(�6/   6<   5      �� 5   
   .�5      ��  � ��$ Center=�6:   6K   �� 5       � ��$ 10q�6/   6<   �� 5       � ��$ 3q�60   6<   �� 5       � ��$ 14(�61   6>   5      5      ��5      �� 5   
   .��5      �  � Color$ 	LightCyan��6    � Offset$ 0Q�6-   6:   � 5       $ White��6   $	 0.8Q�6)   68   � 5!      $ 	LightCyan��6   $ 1Q�6-   6:   5"       J� CommandBindings 5%      )  #System.Windows.Input.CommandBinding  5&   
   -   5           Command$4 .KnowledgeAcquisitionComponent:KacWindow.Select��5&      6n     5'   
   -   5        $2 ,KnowledgeAcquisitionComponent:KacWindow.Load��5'      5(   7   .��5+      �  � RowDefinitions 5,   
   �� 5-      6   �� 5.      6   �� 5/      $
��Auto �6   6,   �� 50      $
��Auto �6   6,   	 � ColumnDefinitions	 52   
   �� 53      $��* �6   6+   �� 54      $��4* �6   6,   �� 55      $��* �6   6+   .+�58   
   �� $��Right=�6   $��Center=�60   #��[ 6K   	Файл:6v   �� 59   
   -   5        $��tbPath��59      $��1��6    #��[ 60   6X   �� 5:   
   -   5        $��btnSelectPath��5:      $��2��6&   #��[  66   $��Center=�6[   $4��.KnowledgeAcquisitionComponent:KacWindow.Select��5;      .��6J   ...6O   �� 5<   
   -   5        $��btnLoad��5<      $��1��6    $��1��6-   #��[  6=   $2��,KnowledgeAcquisitionComponent:KacWindow.Load��5=      .��6H   Загрузить6S   9� 5?   
   $��2��6   $��3��6    $��Действия��64   .��5@      �� -   5        $��	tblockLog��5@      6+   5A      5B      5C      
   M S B A M L   `   `   `  ����E  ARestaurant, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:Restaurant.ConvertersRestaurant.Converters  5   	   =#clr-namespace:Restaurant.ViewModelsRestaurant.ViewModels  5   	   .    (Restaurant.View.MenuWindowWithoutAccount   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   4conv#clr-namespace:Restaurant.Converters     5   	   2vm#clr-namespace:Restaurant.ViewModels     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$   Meniu Restaurant (Oaspete)��5	   	   $	��800��6,   $
��1200��69    � WindowState$ 	Maximized=�5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � 	Resources 5      5   /Restaurant.Converters.BoolToVisibilityConverter   BoolToVisibilityConverterB   <Restaurant.Converters.StringNullOrEmptyToVisibilityConverter * &StringNullOrEmptyToVisibilityConverter2   ,Restaurant.Converters.EnumToBooleanConverter  EnumToBooleanConverter  ProductCard �� 
TargetType �� Property �� Value3  -System.Windows.Media.Effects.DropShadowEffect   Color   
BlurRadius	   ShadowDepth  ProductTemplate
 �� DataType,   &Restaurant.ViewModels.ProductViewModel � RowDefinitions �� StringFormat �� 	Converter �� ConverterParameter  MenuTemplate*   $Restaurant.ViewModels.MeniuViewModel �� TextAlignment �� Path��%�
  &	        &	      &	 &     &	 9     &	 �    7  7   7   &	 J    7  7   7     5   
   6)     5   
   66     5   
   6&   �� 5   
   " ��6$   .�5      ��  � ��$	 250��6&   �� 5       � ��$	 300��6'   �� 5       � ��$ 10q�6'   �� 5       �@��CornerRadius$ 8��6-   �� 5       � ��
 �����6+   �� 5       �@J�Effect 5        5      $ 	#66000000��6'   $ 5Q�69   $	 2Q�6H   5      5      �� 5   
   "
  6/   .R�5      �� 8��  ._�5       �  5!      �� 5"      $��2* �6(   �� 5#      $��* �6(   .+�5%      �� $��0��6   $��UniformToFill=�6F   $	��150��6^   $	��Top=�6n   $��5q�6�   ��6)   �� *	ImagePath+6�   �� 5&      $��1��6!   $��8q�6.   $	��Top=�69   .+�5'      �� $��16(�6:   $
��Bold"�6H   $��CharacterEllipsis=�6Z   ��6$   �� *Name+6Z   �� 5(      $��14(�6K   
��� d �$��0,4,0,0q�6p   ��6$   �� *Price+$ C��6p   �� 5)      $��11(�6L   
���  ��$��Normal"�6o   $��0,6,0,0q�6�   $
��Wrap=�6�   ��6$   �� *AllergensDisplayString+y�6�   �� *HasAllergens+8  5*      �� 5+      $��0��6   $��2��6*   ���	#AAFFFFFFy�6R   �� *IsAvailable+8  $ Invert��._�5,      �� $��Indisponibil��6$   
���  ��$��20(�6I   $
��Bold"�6W   $��Center=�6i   $��Center=�6�   5-      5.      5/      50      �� 52   
   "
  6,   .R�53      �� 8��  ._�54      �  55      �� 56      $��2* �6(   �� 57      $
��Auto �6(   �� 58      $��* �6(   .+�5:      �� $��0��6   $��UniformToFill=�6F   $	��120��6^   $	��Top=�6n   $��5q�6�   ��6)   �� *	ImagePath+6�   �� 5;      $��1��6!   $��8,8,8,4q�6.   $��Stretch=�6?   .+�5<      �� $��16(�6:   $
��Bold"�6H   $ Center=�6Z   $��CharacterEllipsis=�6q   ��6$   �� *Name+6q   �� 5=      $��14(�6K   
��� d �$��0,4,0,0q�6p   $ Center=�6�   ��6$   �� *Price+$ C��6�   5>      �� 5?      $��2��6#   $��8,0,8,4q�60   $
��Auto=�6A   $	��100��6d   .��5@      �� .+�5A      �� $��	Conține:��6(   $��12(�68   $��SemiBold"�6F   $��0,0,0,2q�6\   
�������y�6   �� *Items.Count+8  �� 5B      $��0,0,0,5q�6I   ��6+   �� *Items+��5C   "   �� 5D   &   .R�5E   *   �� $��11(�64   
���iii�$��CharacterEllipsis=�6W   �� 6y   $
�• ��6}   �� 6�   �6�   �� $ .�5F   +   5G   '   5I      �� 5J      $��10(�6P   
���  ��$��Normal"�6s   $��0,4,0,0q�6�   $
��Wrap=�6�   ��6(   �� *AllergensDisplayString+y�6�   �� *HasAllergens+8  5K      5L      5M      5N      5O      5P      .��5R      ]� .+�5S   
   � $	��Top=�6   ���	#FF3498DB 5T      �� 5U      $
��Auto �6    �� 5V      $
��Auto �6     � ColumnDefinitions 5X      �� 5Y      $
��Auto �6#   �� 5Z      $
��Auto �6#   �� 5[      $��* �6#   .+�5^      �� $��0��6   $��0��6%   $"��Restaurant Comenzi (Oaspete)��65   $��24(�6Y   $
��Bold"�6g   
�������$��Center=�6�   $��	10,5,10,2q�6�   �� 5`      $��0��6   $��1��6#   $	��300��63   $��10,5,0,2q�6?   $��Center=�6Q   ��5a      �� *SearchKeyword+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=�.��6g   ��5b      �� 5c      " ��6   .�5d      ��  � ��$ Center=�6E   �� 5e       � ��$	 5,0q�64   �� 5f       �@�Tag 60   �� *SearchPlaceholderText+�� 5g       � �� 5h      �� 5i   "    �� 
TargetType" ��62   .R�5j   &   �� #��z��6-   #��z��5k   -   #��z��5l   -    �� CornerRadius$ 3��5m   -   ._�5n   *   � .+�5o   .   ��  ��Name$ PART_ContentHost��6;   $��Center=�6U   �� 5p   .   
 � Tag#��z 68    J� IsHitTestVisible$ False��5q   8   
�������$��Center=�5r   8   $��5,0,0,0q�6S    �� Padding$ 0q�6d   y�5s   8   �� *Text+ �� RelativeSource �� *TemplatedParent+# [ 5t   +   5u   '   5v   #   5x      5y      .��5z   !   5{      �� 5}      $��1��6   $��1��6&    �� Orientation$ 
Horizontal=�66   $��10,0,0,5q�6O   $��Center=�6a   .+�5~      	� $
��Nume��6    	� 	GroupName$ SearchModeGuest��6-   
�������$��0,0,10,0q�6\    d� 	IsChecked 5      �� *SelectedSearchMode+# [ -   'Restaurant.ViewModels.SearchModeOptions   ByName# Z 	� 5�      $��Exclude Alergen��6   $ SearchModeGuest��68   
������� 5�      �� *SelectedSearchMode+# [    ExcludeByAllergen# Z 5�      5�      �� 5�   
   $
��Left=�6   $	��200��6'   ���	#FFF0F0F0$��0,5,0,0q�6J   ._�5�      ��   �� DisplayMemberPath$
  Name��5�      $��10q�6/   $��0q�6;   ��5�      �� *
Categories+! �� SelectedItem! 69   �� *SelectedCategory+$ TwoWay=�5�   ;   5�      �� 5�   
   $��5,0,0,0q�6   .@�5�      �� $��	Preparate��6   .��5�      �� $
��Auto=�6   $��Disabled=�6B   .��5�      �� #��[ 6L   ��6#   �� *FilteredProducts+��5�      �� 5�      .R�5�   "   � " � Orientation$" 
Horizontal=�6,   5�      5�      5�      5�      �� 5�      $��Meniuri��6   .��5�      �� $
��Auto=�6   $��Disabled=�6B   .��5�      �� #��[ 6I   ��6#   �� *FilteredMenus+��5�      �� 5�      .R�5�   "   � $" 
Horizontal=�6,   5�      5�      5�      5�      5�      5�      5�      
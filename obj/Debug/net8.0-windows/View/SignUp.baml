   M S B A M L   `   `   `  ����E  ARestaurant, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:Restaurant.ViewModelsRestaurant.ViewModels  5   	       Restaurant.View.SignUp   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7localVM#clr-namespace:Restaurant.ViewModels     5   	     � Title$*  $Restaurant Comenzi – Înregistrare��5   	   $	��700��5	   	   $	��700��6    � WindowStartupLocation$ CenterScreen=�6"    � WindowState$ 	Maximized=�5
   	    � DataContext 5      +   %Restaurant.ViewModels.SignUpViewModel  5   
    � 	Resources 5         PrimaryColor  AccentColor  CardBackground  
LabelColor  PrimaryBrush �� Color  AccentBrush  	CardBrush  
LabelBrush �� 
TargetType  PrimaryButton �� Property �� Value	 �� 
TargetType
 �� CornerRadius J� SnapsToDevicePixels 	 
InputLabel 
 InputBox ��Name  PasswordInputBox��%�  &	        &	 !     &	 B     &	 c     &	 �     7   &	 �     7  &	 �     7  &	 �     7  &	 �     7  &		 �    7  &	
 �    7  &	 �    7  ��5   
   	#FF3498DB��60   ��5   
   	#FF2C3E50��6/   ��5   
   	#FFFFFFFF��62   ��5   
   	#FF555555��6.   �� 5   
   8   6/   �� 5   
   8   6/   �� 5   
   8   6/   �� 5   
   8   6/   �� 5   
   " ��6   .�5      ��  � ��8   6+   �� 5       � ��
 �����6+   �� 5       � ��$ 16(�6)   �� 5       � ��$ SemiBold"�6+   �� 5        � ��$
 12,8q�6(   �� 5!       � ��$ 0,25,0,0q�6'   �� 5"       �@�Cursor$
 Hand��6'   �� 5#       � �� 5$      �� 5%      "	 ��6&   .R�5&      �� #��z��6!   $
 8��5'   !   $
 True��5(   !   ._�5)      �� $��Center=�6/   $��Center=�5*   /   $
��True��5+   /   5,      5-      5/      50      �� 52   
   " ��6   .�53      ��  � ��$ 14(�6)   �� 54       � ��$ SemiBold"�6+   �� 55       � ��8   6+   �� 56       � ��$ 0,12,0,3q�6'   57      �� 59   
   " ��6   .�5:      ��  � ��$ 40��6'   �� 5;       � ��$ 0,0,0,12q�6'   �� 5<       � ��$ 8q�6(   �� 5=       � ��$ 14(�6)   �� 5>       � ��8   6,   �� 5?       � ��$ 1q�60   �� 5@       � ��
 �����6+   �� 5A       � ��
 �   �6+   �� 5B       � �� 5C      �� 5D      "	 ��6&   .R�5E      �� #��z��6!   #��z��5F   !   #��z��5G   !   $
 4��5H   !   ._�5I      �� $ PART_ContentHost��6+   5J      5K      5M      5N      �� 5O   
   " G�6   .�5P      ��  � ��$ 40��6'   �� 5Q       � ��$ 0,0,0,12q�6'   �� 5R       � ��$ 8q�6(   �� 5S       � ��$ 14(�6)   �� 5T       � ��8   6,   �� 5U       � ��$ 1q�60   �� 5V       � ��
 �����6+   �� 5W       � ��
 �   �6+   �� 5X       � �� 5Y      �� 5Z      "	 G�6&   .R�5[      �� #��z��6!   #��z��5\   !   #��z��5]   !   $
 4��5^   !   ._�5_      �� $ PART_ContentHost��6+   5`      5a      5c      5d      5e      .��5g      � ��5h   
   �� 5i       �� 
StartPoint$	 0,0&�6"    �� EndPoint$	 1,1&�63   .��5j      �  � Color$ 	#FF2C3E50��6    � Offset$	 0.0Q�61   � 5k      $ 	#FF3498DB��6   $	 1.0Q�61   5l      .+�5o   
   �� $	��600��6    �� Padding$ 40q�6   #��[ 6*   $
 12��6R   $��Center=�5p      $��Center=�6,    J� Effect 5q      3  -System.Windows.Media.Effects.DropShadowEffect  5r         Color$ 	#66000000��6#      
BlurRadius$ 10Q�65      ShadowDepth$ 2Q�6E   ._�5u      �� $
��Auto=�6   .��5v      �� .+�5w      �� $��Creează Cont Nou��6    $��30(�68   $
��Bold"�6F   #��[ 5x       $��Center=�6J   $��0,0,0,25q�6g   �� 5z      $��Nume:��6    #��[	 6-   �� 5{      #��[
 6   
 � Tag$ Introduceți numele dvs.��6{   ��6@   �� *Nume+ �� UpdateSourceTrigger$ PropertyChanged=�6{   �� 5}      $��Prenume:��6    #��[	 60   �� 5~      #��[
 6   $! Introduceți prenumele dvs.��6~   ��6@   �� *	Prenume+$ PropertyChanged=�6~   �� 5�      $��Telefon:��6    #��[	 60   �� 5�      #��[
 6   $&  Introduceți numărul de telefon��6~   ��6@   �� *	Telefon+$ PropertyChanged=�6~   �� 5�      $��Adresă de livrare:��6    #��[	 6:   �� 5�      #��[
 6   $# Introduceți adresa completă��6�    �� TextWrapping$
 Wrap=�6�    �� AcceptsReturn$
 True��6�    �� MinLines$ 2��6�    �� MaxLines$ 3��6�   $
��Auto��6�   ��6@   �� *AdresaLivrare+$ PropertyChanged=�6�   �� 5�      $��Adresă de email:��6    #��[	 68   �� 5�      #��[
 6   $ exemplu@email.com��6|   ��6@   �� *Email+$ PropertyChanged=�6|   �� 5�      $��Parolă:��6    #��[	 6/   G� 5�      -   5         G�Name$ ParolaPasswordBox��5�   "   #��[ 6=   $$ Introduceți o parolă sigură��6g    G� PasswordChar$	 ●��6�   6�   �� 5�      $��Confirmă Parola:��6    #��[	 68   G� 5�      -   5        $ ConfirmaParolaPasswordBox��5�   "   #��[ 6E   $ Reintroduceți parola��6o   $	 ●��6�   6�   �� 5�      $��Înregistrare��6   #��[ 64   $��Center=�5�      $	��220��6:   ��5�      �� *RegisterCommand+5�   :   5�      5�      5�      5�      5�      
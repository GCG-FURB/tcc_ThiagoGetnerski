   M S B A M L   `   `   `  ����  i1Sharp   i1Sharp.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��525��6)   .��5      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��2* �6   �� 5      $��* �6   .+�5   
   �� $��SDK Version:��6   $��0��6'   $��0��67   $��5q�6D   �� 5   
   -   5         ��Name$ 
sdkVersion��5      $��1��6&   $��0��66   $��5q�6C   �� 5   
   $��Device:��6   $��0��6"   $��1��62   $��5q�6?   �� 5   
   -   5         ��Name$ devicesList��5      $��1��6(   $��1��68   $��5q�6E    �� DisplayMemberPath$
 Name��5   j   ��6   ��  �� Path$ Devices� �� Mode$ OneWay=� �� UpdateSourceTrigger$ PropertyChanged=�	 �� SelectedItem	 5      �� *CurrentDevice+$ TwoWay=��� 5   
   $��0��6   $��2��6%   .+�5      �� -   5        
 ��Name$
 calibrateButton��5      $��	Calibrate��6.   $��5q�6B   6M   �� 5      -   5         ��Name$ hasUVLED��5      $��UV LED��6)   $��5q�6:   {�.  6E   �� 5      -   5        $ hasUVCutFilter��5      $��UV cut filter��6/   $��5q�6G   {�.  6R   �� 5      -   5        $ hasWavelengthLED��5      $��Wavelength LED��61   $��5q�6J   {�.  6U   �� 5      -   5        $ hasZebraSensor��5      $��Zebra sensor��6/   $��5q�6F   {�.  6Q   �� 5       -   5        $ hasIndicatorLED��5       $��Indicator LED��60   $��5q�6H   {�.  6S   5!      &   System.Windows.Controls.DataGrid  5"   
   -	   5          Name$ spectrumGrid��5"      $��1��6)   $��2��69   $��5,5,5,8q�6F      AutoGenerateColumns .  ��5#      �� *Measurements+$ OneWay=�$ PropertyChanged=�5$      �� 5%   
   $��2��6   $��Bottom=�6$   $��3��6?   $��2��6J   $��Stretch=�6^   �� 5&   
   -
   5         ��Name$ logBox��5&      $��1��6&   $��3��66   $��5q�6C    �� 
IsReadOnly .  �� AcceptsReturn . 6`   5'      5(      
   M S B A M L   `   `   `  ����   i1Sharp   i1Sharp.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  
MainWindow��$	��450��$	��525��.���  � ColumnDefinitions �� $
��Auto ��� $��* � � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��2* ��� $��* �.+��� $��SDK Version:��$��0��$��0��$��5q��� -    ��Name$ 
sdkVersion��$��1��$��0��$��5q��� $��Device:��$��0��$��1��$��5q��� -    ��Name$ devicesList��$��1��$��1��$��5q� �� DisplayMemberPath$
 Name������  �� Path$ Devices� �� Mode$ OneWay=� �� UpdateSourceTrigger$ PropertyChanged=�	 �� SelectedItem	 �� *CurrentDevice+$ TwoWay=��� $��0��$��2��.+��� -   
 ��Name$
 calibrateButton��$��	Calibrate��$��5q��� -    ��Name$ hasUVLED��$��UV LED��$��5q�{�.  �� -   $ hasUVCutFilter��$��UV cut filter��$��5q�{�.  �� -   $ hasWavelengthLED��$��Wavelength LED��$��5q�{�.  �� -   $ hasZebraSensor��$��Zebra sensor��$��5q�{�.  �� -   $ hasIndicatorLED��$��Indicator LED��$��5q�{�.  &   System.Windows.Controls.DataGrid  -	     Name$ spectrumGrid��$��1��$��2��$��5,5,5,8q�   AutoGenerateColumns .  ���� *Measurements+$ OneWay=�$ PropertyChanged=��� $��2��$��Bottom=�$��3��$��2��$��Stretch=��� -
    ��Name$ logBox��$��1��$��3��$��5q� �� 
IsReadOnly .  �� AcceptsReturn . 
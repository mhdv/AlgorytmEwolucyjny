   M S B A M L   `   `   `  ����M  IAlgorytmEwolucyjny, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:AlgorytmEwolucyjnyAlgorytmEwolucyjny  5   	   #    AlgorytmEwolucyjny.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   R NOxyPlot.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0 oxyhttp://oxyplot.org/wpf  5   	   ,local clr-namespace:AlgorytmEwolucyjny  5   	   -   5          � Title$9  3Teoria i metody optymalizacji - algorytm ewolucyjny��5
   	    � Icon$ icon.ico��6E   $	��600��6U   $	��800��6b   .��5      (�  (� Stretch$
 Fill=�6   .��5      � .+�5        OxyPlot.Wpf.PlotView  -   5          Name$ pltModel��5      $��10,206,359,10q�6@      Model 6$   �� *	plotModel+6@   �� 5      -   5         ��Name$ txtEquation��5      $
��Left=�6"   $��22��6=   $��	10,35,0,0q�6I    �� TextWrapping$
 Wrap=�6\   $�� ��6p   $	��Top=�6x   $	��772��6�   6�   �� 5      -   5         ��Name$ btnEquation��5      $��
Zatwierdź��6!   $
��Left=�65   $��707,539,0,0q�6P   $	��Top=�6e   $��75��6}   6�   �� 5      -   5        $ tmpSolution��5      $
��Left=�6"   $��	10,68,0,0q�6=   $
 Wrap=�6P    �� 
IsReadOnly . "	 �� VerticalScrollBarVisibility$
	 Auto=�6v   $��Rozwiązanie: ��6�   $	��Top=�6�   $	��133��6�   $	��423��6�   �� 5      -   5        $ txtPopulationSize��5      $
��Left=�6(   $��23��6C   $��618,317,0,0q�6O   $
 Wrap=�6d   $��20��6x   
 �� 	MaxLength$
 3��6�   $
	 Auto=�6�   $	��Top=�6�   $	��154��6�    J� RenderTransformOrigin$ 0.567,3.217&�6�   �� 5      -   5        $ txtIterations��5      $
��Left=�6$   $��23��6?   $��618,345,0,0q�6K   $
 Wrap=�6`   $��50��6t   $
 4��6~   $
	 Auto=�6�   $	��Top=�6�   $	��154��6�   $ 0.567,3.217&�6�   � 5      
���   �$	��491��6   $��1��6*   $��Center=�64   $��445,68,346,10q�6O   �� 5      $��Times New Roman)�6   $
��Bold"�6(   $��Wielkość populacji:��6:   $
��Left=�6X   $��462,317,0,0q�6s   $	��Top=�6�   �� 5      $��Times New Roman)�6   $
��Bold"�6(   $��Operator krzyżowania:��6:   $
��Left=�6Z   $��462,375,0,0q�6u   $	��Top=�6�   �� 5      $��Times New Roman)�6   $
��Bold"�6(   $��Liczba iteracji:��6:   $
��Left=�6U   $��462,346,0,0q�6p   $	��Top=�6�   �� 5      $��Times New Roman)�6   $
��Bold"�6(   $B��<Wprowadź równanie (składnia MATLAB "2*x1^2+sqrt(x2)"...):��6:   $
��Left=�6�   $��10,6,0,0q�6�   $	��Top=�6�   $	��358��6�   � 5      
���   �$��1��6   $��Center=�6(   $��10,62,10,506q�6C   �� 5      -   5         ��Name$ comboReproductionMethod��5       �� SelectedIndex$ 1��6/   $
��Left=�6A   $��618,373,0,0q�6\   $	��Top=�6q   $	��154��6�   �� 5      -	   5        $ comboFunctions��5      $ 0��6&   $
��Left=�68   $��	541,8,0,0q�6S   $	��Top=�6f   $	��241��6~   6�   � 5      
���   �$��24��6   $��1��6)   $��Center=�63   $��357,6,434,539q�6N   �� 5      $��Times New Roman)�6   $
��Bold"�6(   $��Przykładowe funkcje:��6:   $
��Left=�6Y   $��	414,6,0,0q�6t   $	��Top=�6�   �� 5      $1��+Autorzy: Michał Filipowicz & Rafał Madeja��6   $
��Left=�6C   $��451,533,0,0q�6^   $	��Top=�6s   &   System.Windows.Controls.DataGrid  5      -
   5          Name$ variablesGrid��5         AutoGenerateColumns . $
��Left=�6@   $	��160��6[   $��
462,94,0,0q�6h   $	��Top=�6|   $	��310��6�   � 5      -   5         �Name$ 
pbProgress��5      $
��Left=�6%   $��33��6@   $��462,501,0,0q�6L   $	��Top=�6a   $	��320��6y   �� 5       -   5         ��Name$ txtProgress��5       $��Center=�6$   $��618,509,138,0q�6A   $
��Wrap=�6X   $��0%��6l   $	��Top=�6v   $��36��6�   �� 5!      $��Times New Roman)�6   $
��Bold"�6(   $2��,Argumenty i przedziały punktów startowych:��6:   $
��Left=�6o   $��
462,70,0,0q�6�   $	��Top=�6�   �� 5"      $��Times New Roman)�6   $
��Bold"�6(   $��Postęp:��6:   $
��Left=�6L   $��462,477,0,0q�6g   $	��Top=�6|   �� 5#      $��Times New Roman)�6   $
��Bold"�6(   $&�� Dokładność wykresu (próbki):��6:   $
��Left=�6a   $��462,404,0,0q�6|   $	��Top=�6�   �� 5$      -   5        $ txtPlot��5$      $
��Left=�6   $��23��69   $��655,400,0,0q�6E   $
 Wrap=�6Z   $��40��6n   $
 3��6x   $
	 Auto=�6�   $	��Top=�6�   $	��117��6�   $ 0.567,3.217&�6�   6  �� 5%      -   5        $ plotRefresh��5%      $��Wymuś odświeżenie��6!   $
��Left=�6=   $��313,206,0,0q�6X   $	��Top=�6m   $	��119��6�   6�   �� 5&      -   5        $ plotOnlyBest��5&      $��Zaznacz najlepszy��6"   $
��Left=�6>   $��
10,206,0,0q�6Y   $	��Top=�6m   $	��129��6�   6�   �� 5'      $��Times New Roman)�6   $
��Bold"�6(   $��Szukaj:��6:   $
��Left=�6L   $��462,288,0,0q�6g   $	��Top=�6|   �� 5(      -   5        $ 	comboFind��5(      $ 0��6!   $
��Left=�63   $��618,290,0,0q�6N   $	��Top=�6c   $	��154��6{   � 5)      -   5        $ plotPb��5)      $
��Left=�6!   $��9��6<   $��313,231,0,0q�6G   $	��Top=�6\   $	��119��6t   �� 5*      -   5        $ plotSurfaceBtn��5*      $��3D��6$   $
��Left=�61   $��414,539,0,0q�6L   $	��Top=�6a   $��26��6y   6�   �� 5+      -   5        $ comboStrategy��5+      $ 0��6%   $
��Left=�67   $��541,263,0,0q�6R   $	��Top=�6g   $	��231��6   6�   �� 5,      $��Times New Roman)�6   $
��Bold"�6(   $��
Strategia:��6:   $
��Left=�6O   $��462,263,0,0q�6j   $	��Top=�6   �� 5-      $��Times New Roman)�6   $
��Bold"�6(   $��Współczynnik μ:��6:   $
��Left=�6T   $��462,433,0,0q�6o   $	��Top=�6�   �� 5.      -   5        $ miFact��5.      {�.  $
��Left=�6/   $��23��6J   $��655,428,0,0q�6V   $
 Wrap=�6k   $��10��6   $	��Top=�6�   $	��117��6�   50      51      52      
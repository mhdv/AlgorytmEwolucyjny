   M S B A M L   `   `   `  ���� M  IAlgorytmEwolucyjny, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:AlgorytmEwolucyjnyAlgorytmEwolucyjny  #    AlgorytmEwolucyjny.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     R NOxyPlot.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0 oxyhttp://oxyplot.org/wpf  ,local clr-namespace:AlgorytmEwolucyjny  -     � Title$9  3Teoria i metody optymalizacji - algorytm ewolucyjny�� � Icon$ icon.ico��$	��600��$	��800��.��(�  (� Stretch$
 Fill=�.��� .+�  OxyPlot.Wpf.PlotView  -     Name$ pltModel��$��10,206,359,10q�   Model �� *	plotModel+�� -    ��Name$ txtEquation��$
��Left=�$��22��$��	10,35,0,0q� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$	��772���� -    ��Name$ btnEquation��$��
Zatwierdź��$
��Left=�$��707,539,0,0q�$	��Top=�$��75���� -   $ tmpSolution��$
��Left=�$��	10,68,0,0q�$
 Wrap=� �� 
IsReadOnly . "	 �� VerticalScrollBarVisibility$
	 Auto=�$��Rozwiązanie: ��$	��Top=�$	��133��$	��423���� -   $ txtPopulationSize��$
��Left=�$��23��$��618,356,0,0q�$
 Wrap=�$��20��
 �� 	MaxLength$
 3��$
	 Auto=�$	��Top=�$	��154�� J� RenderTransformOrigin$ 0.567,3.217&��� -   $ txtIterations��$
��Left=�$��23��$��618,384,0,0q�$
 Wrap=�$��50��$
 4��$
	 Auto=�$	��Top=�$	��154��$ 0.567,3.217&�� 
���   �$	��491��$��1��$��Center=�$��445,68,346,10q��� $��Times New Roman)�$
��Bold"�$��Wielkość populacji:��$
��Left=�$��462,356,0,0q�$	��Top=��� $��Times New Roman)�$
��Bold"�$��Operator krzyżowania:��$
��Left=�$��462,414,0,0q�$	��Top=��� $��Times New Roman)�$
��Bold"�$��Liczba iteracji:��$
��Left=�$��462,385,0,0q�$	��Top=��� $��Times New Roman)�$
��Bold"�$B��<Wprowadź równanie (składnia MATLAB "2*x1^2+sqrt(x2)"...):��$
��Left=�$��10,6,0,0q�$	��Top=�$	��358��� 
���   �$��1��$��Center=�$��10,62,10,506q��� -    ��Name$ comboReproductionMethod�� �� SelectedIndex$ 0��$
��Left=�$��618,412,0,0q�$	��Top=�$	��154���� -	   $ comboFunctions��$ 0��$
��Left=�$��	541,8,0,0q�$	��Top=�$	��241��� 
���   �$��24��$��1��$��Center=�$��357,6,434,539q��� $��Times New Roman)�$
��Bold"�$��Przykładowe funkcje:��$
��Left=�$��	414,6,0,0q�$	��Top=��� $1��+Autorzy: Michał Filipowicz & Rafał Madeja��$
��Left=�$��451,533,0,0q�$	��Top=�&   System.Windows.Controls.DataGrid  -
     Name$ variablesGrid��   AutoGenerateColumns . $
��Left=�$	��192��$��
462,94,0,0q�$	��Top=�$	��310��� -    �Name$ 
pbProgress��$
��Left=�$��33��$��462,501,0,0q�$	��Top=�$	��320���� -    ��Name$ txtProgress��$��Center=�$��618,509,138,0q�$
��Wrap=�$��0%��$	��Top=�$��36���� $��Times New Roman)�$
��Bold"�$2��,Argumenty i przedziały punktów startowych:��$
��Left=�$��
462,70,0,0q�$	��Top=��� $��Times New Roman)�$
��Bold"�$��Postęp:��$
��Left=�$��462,477,0,0q�$	��Top=��� $��Times New Roman)�$
��Bold"�$&�� Dokładność wykresu (próbki):��$
��Left=�$��462,443,0,0q�$	��Top=��� -   $ txtPlot��$
��Left=�$��23��$��655,439,0,0q�$
 Wrap=�$��40��$
 3��$
	 Auto=�$	��Top=�$	��117��$ 0.567,3.217&��� -   $ plotRefresh��$��Wymuś odświeżenie��$
��Left=�$��313,206,0,0q�$	��Top=�$	��119���� -   $ plotOnlyBest��$��Zaznacz najlepszy��$
��Left=�$��
10,206,0,0q�$	��Top=�$	��129���� $��Times New Roman)�$
��Bold"�$��Szukaj:��$
��Left=�$��462,327,0,0q�$	��Top=��� -   $ 	comboFind��$ 0��$
��Left=�$��618,329,0,0q�$	��Top=�$	��154��� -   $ plotPb��$
��Left=�$��9��$��313,231,0,0q�$	��Top=�$	��119���� -   $ plotSurfaceBtn��$��3D��$
��Left=�$��414,539,0,0q�$	��Top=�$��26���� -   $ comboStrategy��$ 0��$
��Left=�$��541,302,0,0q�$	��Top=�$	��231���� $��Times New Roman)�$
��Bold"�$��
Strategia:��$
��Left=�$��462,302,0,0q�$	��Top=�
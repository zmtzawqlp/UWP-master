using MyUWPToolkit.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MyUWPToolkit
{
    [TemplatePart(Name = "Root", Type = typeof(Grid))]
    public class ColumnChart : Control
    {
        #region Fields
        private const string Root = "Root";
        private Grid _root;
        private bool isLoaded;
        private List<string> _independentValues = new List<string>();
        private List<long> _dependentValues = new List<long>();
        #endregion

        #region Property

        public string DependentValuePath { get; set; }

        public string IndependentValuePath { get; set; }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ColumnChart), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as ColumnChart;
            if (cc != null)
            {
                cc.UpdateGrid();
            }

        }

        public Brush GridLineBrush
        {
            get { return (Brush)GetValue(GridLineBrushProperty); }
            set { SetValue(GridLineBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridLineBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register("GridLineBrush", typeof(Brush), typeof(ColumnChart), new PropertyMetadata(null));




        public Brush PositiveValueBrush
        {
            get { return (Brush)GetValue(PositiveValueBrushProperty); }
            set { SetValue(PositiveValueBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PositiveValueBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositiveValueBrushProperty =
            DependencyProperty.Register("PositiveValueBrush", typeof(Brush), typeof(ColumnChart), new PropertyMetadata(null));


        public Brush NegativeValueBrush
        {
            get { return (Brush)GetValue(NegativeValueBrushProperty); }
            set { SetValue(NegativeValueBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NegativeValueBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NegativeValueBrushProperty =
            DependencyProperty.Register("NegativeValueBrush", typeof(Brush), typeof(ColumnChart), new PropertyMetadata(null));



        public Brush IndependentValueBrush
        {
            get { return (Brush)GetValue(IndependentValueBrushProperty); }
            set { SetValue(IndependentValueBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndependentValueBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndependentValueBrushProperty =
            DependencyProperty.Register("IndependentValueBrush", typeof(Brush), typeof(ColumnChart), new PropertyMetadata(null));



        public double IndependentValueFontSize
        {
            get { return (double)GetValue(IndependentValueFontSizeProperty); }
            set { SetValue(IndependentValueFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndependentValueFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndependentValueFontSizeProperty =
            DependencyProperty.Register("IndependentValueFontSize", typeof(double), typeof(ColumnChart), new PropertyMetadata(12.0));



        public double DependentValueFontSize
        {
            get { return (double)GetValue(DependentValueFontSizeProperty); }
            set { SetValue(DependentValueFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DependentValueFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DependentValueFontSizeProperty =
            DependencyProperty.Register("DependentValueFontSize", typeof(double), typeof(ColumnChart), new PropertyMetadata(12.0));


        #endregion

        protected override void OnApplyTemplate()
        {
            _root = GetTemplateChild(Root) as Grid;
            base.OnApplyTemplate();
            isLoaded = true;
            UpdateGrid();
        }

        public ColumnChart()
        {
            this.DefaultStyleKey = typeof(ColumnChart);
            Loaded += ColumnChart_Loaded;
            SizeChanged += ColumnChart_SizeChanged;

        }

        private void ColumnChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateGrid();
        }

        private void _series_ItemsSourceChanged(object sender, EventArgs e)
        {
            UpdateGrid();
        }

        private void ColumnChart_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        void UpdateGrid()
        {
            if (_root == null || ItemsSource == null)
            {
                return;
            }
            _root.Children.Clear();
            _root.ColumnDefinitions.Clear();
            _independentValues.Clear();
            _dependentValues.Clear();
            BindingEvaluator independentBinding = new BindingEvaluator(IndependentValuePath);
            BindingEvaluator dependentBinding = new BindingEvaluator(DependentValuePath);

            int column = 0;
            foreach (var item in ItemsSource)
            {
                var independentValue = independentBinding.Eval(item).ToString();
                var dependentValue = (long)dependentBinding.Eval(item);
                _dependentValues.Add(dependentValue);
                _root.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                TextBlock independentTB = new TextBlock();
                independentTB.Text = independentValue;
                independentTB.FontSize = IndependentValueFontSize;
                independentTB.Foreground = IndependentValueBrush;
                independentTB.VerticalAlignment = VerticalAlignment.Center;
                independentTB.HorizontalAlignment = HorizontalAlignment.Center;
                independentTB.SetValue(Grid.RowProperty, 3);
                independentTB.SetValue(Grid.ColumnProperty, column++);
                _root.Children.Add(independentTB);
            }
            Rectangle rc = new Rectangle();
            rc.Fill = GridLineBrush;
            rc.Height = 1;
            rc.SetValue(Grid.RowProperty, 1);
            rc.SetValue(Grid.ColumnSpanProperty, column);
            _root.Children.Add(rc);

            var min = _dependentValues.Min();
            var max = _dependentValues.Max();
            long temp = 0;
            if (min >= 0 && max >= 0)
            {
                temp = max;
            }
            else if (min < 0 && max >= 0)
            {
                temp = max - min;
            }
            else if (max < 0)
            {
                temp = -min;
            }

            var height = (this.ActualHeight - 1 - 30) / temp;


            for (int i = 0; i < _dependentValues.Count; i++)
            {
                var dependentValue = _dependentValues[i];
                TextBlock dependentTB = new TextBlock();
                dependentTB.Text = dependentValue.ToString();
                dependentTB.FontSize = DependentValueFontSize;
                dependentTB.Foreground = dependentValue >= 0 ? PositiveValueBrush : NegativeValueBrush;
                dependentTB.VerticalAlignment = dependentValue > 0 ? VerticalAlignment.Top : VerticalAlignment.Bottom;
                dependentTB.HorizontalAlignment = HorizontalAlignment.Center;
                dependentTB.SetValue(Grid.RowProperty, dependentValue > 0 ? 2 : 0);
                dependentTB.SetValue(Grid.ColumnProperty, i);

                Rectangle dependentRC = new Rectangle();
                dependentRC.Fill = dependentValue >= 0 ? PositiveValueBrush : NegativeValueBrush;
                dependentRC.Height = Math.Abs(height * dependentValue);
                dependentRC.Margin = new Thickness(20, 0, 20, 0);
                dependentRC.VerticalAlignment = dependentValue <= 0 ? VerticalAlignment.Top : VerticalAlignment.Bottom;
                dependentRC.SetValue(Grid.RowProperty, dependentValue <= 0 ? 2 : 0);
                dependentRC.SetValue(Grid.ColumnProperty, i);
                _root.Children.Add(dependentRC);
                _root.Children.Add(dependentTB);

            }
        }

    }


}

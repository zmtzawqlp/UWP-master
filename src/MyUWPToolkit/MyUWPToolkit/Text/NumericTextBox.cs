using MyUWPToolkit.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyUWPToolkit
{ //public class NumericTextBox : TextBox
    //{

    //    private bool _isChangingTextWithCode;
    //    private bool _isChangingValueWithCode;
    //    private const double Epsilon = .00001;

    //    public event EventHandler ValueChanged;


    //    public bool AllowNullValue
    //    {
    //        get { return (bool)GetValue(AllowNullValueProperty); }
    //        set { SetValue(AllowNullValueProperty, value); }
    //    }

    //    // Using a DependencyProperty as the backing store for AllowNullValue.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty AllowNullValueProperty =
    //        DependencyProperty.Register("AllowNullValue", typeof(bool), typeof(NumericTextBox), new PropertyMetadata(false));


    //    public double? Value
    //    {
    //        get { return (double?)GetValue(ValueProperty); }
    //        set { SetValue(ValueProperty, value); }
    //    }

    //    // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty ValueProperty =
    //        DependencyProperty.Register("Value", typeof(double?), typeof(NumericTextBox), new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));

    //    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        (d as NumericTextBox).UpdateValueText();
    //        (d as NumericTextBox).OnValueChanged();
    //    }

    //    public string ValueFormat
    //    {
    //        get { return (string)GetValue(ValueFormatProperty); }
    //        set { SetValue(ValueFormatProperty, value); }
    //    }

    //    // Using a DependencyProperty as the backing store for ValueFormat.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty ValueFormatProperty =
    //        DependencyProperty.Register("ValueFormat", typeof(string), typeof(NumericTextBox), new PropertyMetadata("F0"));


    //    public double Minimum
    //    {
    //        get { return (double)GetValue(MinimumProperty); }
    //        set { SetValue(MinimumProperty, value); }
    //    }

    //    // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty MinimumProperty =
    //        DependencyProperty.Register("Minimum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(double.MinValue));

    //    public double Maximum
    //    {
    //        get { return (double)GetValue(MaximumProperty); }
    //        set { SetValue(MaximumProperty, value); }
    //    }

    //    // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty MaximumProperty =
    //        DependencyProperty.Register("Maximum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(double.MaxValue));



    //    public NumericTextBox()
    //    {
    //        if (this.Value != null)
    //        {
    //            Text = this.Value.Value.ToString(CultureInfo.CurrentCulture);
    //        }
    //        else
    //        {
    //            this.Text = "";
    //        }

    //        TextChanged += this.OnValueTextBoxTextChanged;
    //        KeyDown += this.OnValueTextBoxKeyDown;
    //        PointerExited += this.OnValueTextBoxPointerExited;
    //    }

    //    private void OnValueTextBoxPointerExited(object sender, PointerRoutedEventArgs e)
    //    {

    //    }

    //    private void OnValueTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
    //    {

    //    }

    //    private void OnValueTextBoxTextChanged(object sender, TextChangedEventArgs e)
    //    {
    //        this.UpdateValueFromText();
    //    }

    //    protected override void OnGotFocus(RoutedEventArgs e)
    //    {
    //        base.OnGotFocus(e);
    //    }

    //    protected override void OnLostFocus(RoutedEventArgs e)
    //    {
    //        this.UpdateValueFromText();
    //        base.OnLostFocus(e);
    //    }


    //    private void UpdateValueText()
    //    {
    //        _isChangingTextWithCode = true;
    //        if (this.Value != null)
    //        {
    //            this.Text = this.Value.Value.ToString(this.ValueFormat);
    //        }
    //        else
    //        {
    //            this.Text = "";
    //        }
    //        this.SelectionStart = this.Text.Length;
    //        _isChangingTextWithCode = false;
    //    }

    //    private void OnValueChanged()
    //    {
    //        if (ValueChanged != null)
    //        {
    //            ValueChanged(null, null);
    //        }
    //    }

    //    private bool UpdateValueFromText()
    //    {
    //        if (_isChangingTextWithCode)
    //        {
    //            return false;
    //        }

    //        double val;
    //        if (double.TryParse(this.Text, NumberStyles.Any, CultureInfo.CurrentUICulture, out val) ||
    //            Calculator.TryCalculate(this.Text, out val))
    //        {
    //            _isChangingValueWithCode = true;
    //            if (val < Minimum)
    //            {
    //                val = Minimum;
    //            }
    //            if (val > Maximum)
    //            {
    //                val = Maximum;
    //            }

    //            this.Value = val;

    //            UpdateValueText();

    //            _isChangingValueWithCode = false;

    //            return true;
    //        }
    //        else
    //        {
    //            if (this.Text == "")
    //            {
    //                if (!AllowNullValue)
    //                {
    //                    this.Value = Minimum;
    //                }
    //                else
    //                {
    //                    this.Value = null;
    //                }

    //            }

    //            UpdateValueText();

    //        }

    //        return false;
    //    }


    //    //private bool SetValueAndUpdateValidDirections(double value)
    //    //{
    //    //    // Range coercion is handled by base class.
    //    //    var oldValue = this.Value;
    //    //    if (value < Minimum)
    //    //    {
    //    //        value = Minimum;
    //    //    }
    //    //    if (value > Maximum)
    //    //    {
    //    //        value = Maximum;
    //    //    }
    //    //    this.Value = value;
    //    //    if (value < Minimum || value > Maximum)
    //    //    {
    //    //        UpdateValueText();
    //    //    }
    //    //    //this.SetValidIncrementDirection();

    //    //    return Math.Abs(this.Value - oldValue) > Epsilon;
    //    //}
    //}


    public class NumericTextBox : TextBox
    {
        private bool _isChangingTextWithCode;
        private bool _isChangingValueWithCode;
        public event EventHandler ValueChanged;

        public bool AllowNullValue { get; set; }
        //public bool AllowNullValue
        //{
        //    get { return (bool)GetValue(AllowNullValueProperty); }
        //    set { SetValue(AllowNullValueProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for AllowNullValue.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty AllowNullValueProperty =
        //    DependencyProperty.Register("AllowNullValue", typeof(bool), typeof(NumericTextBox), new PropertyMetadata(false, new PropertyChangedCallback(OnValueConditionChanged)));

        public string ValueFormat
        {
            get { return (string)GetValue(ValueFormatProperty); }
            set { SetValue(ValueFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register("ValueFormat", typeof(string), typeof(NumericTextBox), new PropertyMetadata("F0", new PropertyChangedCallback(OnValueConditionChanged)));

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(double.MinValue, new PropertyChangedCallback(OnValueConditionChanged)));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(double.MaxValue, new PropertyChangedCallback(OnValueConditionChanged)));



        public bool CanNegNum
        {
            get { return (bool)GetValue(CanNegNumProperty); }
            set { SetValue(CanNegNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanNegNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanNegNumProperty =
            DependencyProperty.Register("CanNegNum", typeof(bool), typeof(NumericTextBox), new PropertyMetadata(false, new PropertyChangedCallback(OnValueConditionChanged)));

        private static void OnValueConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericTextBox).OnValueConditionChanged();
        }


        public double? Value
        {
            get { return (double?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double?), typeof(NumericTextBox), new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NumericTextBox).UpdateValueText();
            (d as NumericTextBox).OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, null);
            }
        }

        ClsNumTextTagIn clsNumTextTagIn;
        public NumericTextBox()
        {
            this.SelectionChanged += NumericTextBox_SelectionChanged;
            this.TextChanged += NumericTextBox_TextChanged;
            this.TextChanging += NumericTextBox_TextChanging;
            clsNumTextTagIn = new ClsNumTextTagIn();
            clsNumTextTagIn.MyPattern = ValueFormat;
            clsNumTextTagIn.AllowNullValue = AllowNullValue;
            clsNumTextTagIn.CanNegNum = CanNegNum;
        }

        private void OnValueConditionChanged()
        {
            clsNumTextTagIn.MyPattern = ValueFormat;
            clsNumTextTagIn.AllowNullValue = AllowNullValue;
            clsNumTextTagIn.CanNegNum = CanNegNum;
            UpdateValueText();
        }

        private void NumericTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (_isChangingTextWithCode)
            {
                return;
            }
            _isChangingValueWithCode = true;
            var value = clsNumTextTagIn.NumericText(this);
            if (value == null)
            {
                Value = null;
                _isChangingValueWithCode = false;
            }
            else
            {

                if (value <= Maximum && value >= Minimum)
                {
                    Value = value;
                    _isChangingValueWithCode = false;
                }
                else
                {
                    _isChangingValueWithCode = false;
                    if (value < Minimum)
                    {
                        if (Value != Minimum)
                        {
                            Value = Minimum;
                        }
                        else
                        {
                            Value = Minimum;
                            UpdateValueText();
                        }

                    }
                    if (value > Maximum)
                    {
                        if (Value != Maximum)
                        {
                            Value = Maximum;
                        }
                        else
                        {
                            Value = Maximum;
                            UpdateValueText();
                        }
                    }
                }
            }


        }

        private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            clsNumTextTagIn.Refresh(this);
        }

        private void NumericTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            clsNumTextTagIn.Refresh(this);
        }

        private void UpdateValueText()
        {
            if (_isChangingValueWithCode)
            {
                return;
            }
            _isChangingTextWithCode = true;
            if (this.Value != null)
            {
                if (!CanNegNum && Value < 0)
                {
                    _isChangingTextWithCode = false;
                    this.Text = Math.Max(Minimum, 0).ToString(this.ValueFormat);
                }
                else if (this.Value <= Maximum && this.Value >= Minimum)
                {
                    this.Text = this.Value.Value.ToString(this.ValueFormat);
                }
                else if (this.Value > Maximum)
                {
                    _isChangingTextWithCode = false;
                    this.Text = Maximum.ToString(this.ValueFormat);
                }
                else if (this.Value < Minimum)
                {
                    _isChangingTextWithCode = false;
                    this.Text = Minimum.ToString(this.ValueFormat);
                }
            }
            else
            {
                if (AllowNullValue)
                {
                    this.Text = "";
                }
                else
                {
                    _isChangingTextWithCode = false;
                    this.Text = Minimum.ToString(this.ValueFormat);
                }
            }
            this.SelectionStart = this.Text.Length;
            _isChangingTextWithCode = false;
        }
    }


    public class ClsNumTextTagIn
    {
        private class cTxtBoxProperty
        {
            public int iBeg { get; set; }
            public int iSel { get; set; }
            public int iLen { get; set; }
            public string sLef { get; set; }
            public string sRig { get; set; }
            public string sSel { get; set; }
            public string sTxt { get; set; }
            public cTxtBoxProperty(Windows.UI.Xaml.Controls.TextBox sender)
            {
                iBeg = sender.SelectionStart;
                iSel = sender.SelectionLength;
                iLen = sender.Text.Length;
                sLef = sender.Text.Substring(0, sender.SelectionStart);
                sRig = sender.Text.Substring(sender.SelectionStart + sender.SelectionLength);
                sSel = sender.SelectedText;
                sTxt = sender.Text;
            }
        }

        private cTxtBoxProperty org { get; set; }
        public string MyPattern { get; set; }
        public bool AllowNullValue { get; set; }
        public bool IsNumOnly { get; private set; }
        public bool IsDotSepa { get; set; }
        public bool CanNegNum { get; set; }
        public System.Collections.Generic.List<string> LstRemStr { get; set; }
        public object Tag { get; set; } //use this Tag

        public ClsNumTextTagIn()
        {
            //org = new cTxtBoxProperty(sender);
            MyPattern = "N2";
            IsNumOnly = true;
            IsDotSepa = true;
            CanNegNum = true;
            LstRemStr = new System.Collections.Generic.List<string>();
            //Tag = anyTag; //use this Tag
        }
        /// <summary>
        /// Routine to test numeric if current TextChanging is numeric and display accordinly to current culture.
        /// </summary>
        /// <param name="sender">Target TextBox</param>
        /// <param name="pattern">Currently only Numeric Pattern is supported eg: "N2", "N6"...etc. N stands for number.</param>
        /// <param name="numbersOnly">true = allow numbers only; false = leave text if its not numeric decimal.</param>
        /// <param name="dotAsDecimalSeparator">true = "." counts as decimal separator (regardless of culture) and it will go for decimal part of pattern (if any).</param>
        /// <param name="allowNegativeNumbers">true = can have negative numbers, false = prevents negative numbers.</param>
        /// <param name="lstRemoveStrings">List of any other strings you wish remove eg: "(", " / ", ")", " - ", " + "...etc, if none just pass "null" parameter.</param>
        /// <param name="anyTag">Use paramenter to pass some object in Tag, if none just pass "null" parameter.</param>
        public ClsNumTextTagIn(Windows.UI.Xaml.Controls.TextBox sender, string pattern, bool numbersOnly, bool dotAsDecimalSeparator, bool allowNegativeNumbers, System.Collections.Generic.List<string> lstRemoveStrings, System.Object anyTag)
        {
            org = new cTxtBoxProperty(sender);
            MyPattern = pattern;
            IsNumOnly = numbersOnly;
            IsDotSepa = dotAsDecimalSeparator;
            CanNegNum = allowNegativeNumbers;
            LstRemStr = lstRemoveStrings;
            Tag = anyTag; //use this Tag
        }
        public void Refresh(Windows.UI.Xaml.Controls.TextBox sender)
        {
            org = new cTxtBoxProperty(sender);
        }
        public double? NumericText(Windows.UI.Xaml.Controls.TextBox sender)
        { //logics
            if (org == null)
            {
                org = new cTxtBoxProperty(sender);
            }

            //System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CurrentUICulture;
            System.Globalization.CultureInfo ci = new CultureInfo("zh-CN");
            cTxtBoxProperty edi = new cTxtBoxProperty(sender);
            string sPtn = MyPattern;
            string sAdd = string.Empty;
            string sSub = string.Empty;
            int iChg = edi.iLen - org.iBeg - org.sRig.Length;

            if (iChg >= 0)
            {
                sAdd = edi.sTxt.Substring(org.iBeg, iChg);
                sSub = org.sSel;
            }
            else
            { //backspace?
                if (org.iLen >= edi.iLen)
                {
                    if (org.iBeg - (org.iLen - edi.iLen) >= 0)
                    {
                        sSub = org.sTxt.Substring(org.iBeg - (org.iLen - edi.iLen), (org.iLen - edi.iLen));
                    }
                }
            }

            int iBgx = edi.iBeg;
            int iSlx = 0;
            string sFnl = org.sTxt;

            int iNeg = sFnl.IndexOf(ci.NumberFormat.NegativeSign);
            int iDot = sFnl.IndexOf(ci.NumberFormat.NumberDecimalSeparator);

            if (sAdd == ci.NumberFormat.NegativeSign)
            {
                if (iNeg == -1)
                { //add
                    if (CanNegNum)
                    {//accepts negative numbers
                     //if (ci.TextInfo.IsRightToLeft)
                     //{
                     //    sFnl = ci.NumberFormat.NegativeSign + sFnl; //sFnl = sFnl + ci.NumberFormat.NegativeSign;
                     //    iBgx = edi.iBeg - ci.NumberFormat.NegativeSign.Length;
                     //}
                     //else
                     //{
                        sFnl = ci.NumberFormat.NegativeSign + sFnl;
                        iBgx = edi.iBeg;
                        //}
                    }
                    else
                    {//does not accepts negative numbers


                        //if (ci.TextInfo.IsRightToLeft)
                        //{
                        iBgx = edi.iBeg - ci.NumberFormat.NegativeSign.Length;
                        //}
                        //else
                        //{
                        //    iBgx = edi.iBeg;
                        //}


                    }
                }
                else
                {//remove
                    sFnl = sFnl.Remove(iNeg, ci.NumberFormat.NegativeSign.Length);
                    if (iNeg >= iBgx)
                    {
                        iBgx = edi.iBeg - ci.NumberFormat.NegativeSign.Length;
                    }
                    else
                    {
                        //if (ci.TextInfo.IsRightToLeft)
                        //{
                        //    iBgx = edi.iBeg - ci.NumberFormat.NegativeSign.Length;
                        //}
                        //else
                        //{
                        iBgx = edi.iBeg - ci.NumberFormat.NegativeSign.Length - ci.NumberFormat.NegativeSign.Length;
                        //}
                    }
                }
            } //end NegativeSign
            else if (sAdd == ci.NumberFormat.NumberDecimalSeparator)
            { //NumberDecimalSeparator
                if (iDot == -1)
                { //add
                    sFnl = edi.sTxt;
                }
                else
                { //go to point
                    sFnl = org.sTxt;
                    iBgx = iDot + ci.NumberFormat.NumberDecimalSeparator.Length;
                }
            }
            else if (sAdd == ".")
            { //dotAsDecimalSeparator
                if (IsDotSepa)
                {
                    if (iDot == -1)
                    { //add
                        sFnl = edi.sTxt;
                    }
                    else
                    { //go to point
                        sFnl = org.sTxt;
                        iBgx = iDot + ci.NumberFormat.NumberDecimalSeparator.Length;
                    }
                }
                else
                {
                    sFnl = edi.sTxt; //override
                }
            }
            else if (sSub == ci.NumberFormat.NumberGroupSeparator & org.iSel == 0)
            { //backspace and GroupSeparator
                if (edi.iBeg == 0)
                {
                    sFnl = edi.sTxt;
                }
                else
                {
                    sFnl = edi.sLef.Replace(ci.NumberFormat.NumberGroupSeparator, string.Empty);
                    int iRem = edi.sLef.Length - sFnl.Length;
                    sFnl = sFnl.Substring(0, edi.sLef.Length - iRem - 1) + edi.sRig;
                    iBgx = iBgx - iRem - 1;
                }
            }//end backspace and GroupSeparator
            else
            {
                sFnl = edi.sTxt;
            }

            if (iBgx > sFnl.Length)
            {
                iBgx = sFnl.Length;
            }
            string sLfx = sFnl.Substring(0, iBgx);
            string sRgx = sFnl.Substring(iBgx);

            sRgx = sRgx.Replace(ci.NumberFormat.NumberGroupSeparator, string.Empty);
            sFnl = sFnl.Replace(ci.NumberFormat.NumberGroupSeparator, string.Empty);

            if (LstRemStr != null)
            {
                foreach (string sRem in LstRemStr)
                {
                    sRgx = sRgx.Replace(sRem, string.Empty); //automatically trimmed
                    sFnl = sFnl.Replace(sRem, string.Empty); //automatically trimmed
                }
            }

            //s += "Rgx: " + sRgx.ToString() + Environment.NewLine;
            //s += "Fnl: " + sFnl.ToString() + Environment.NewLine;

            if (!CanNegNum)
            {//remove negative sign
                sRgx = sRgx.Replace(ci.NumberFormat.NegativeSign, string.Empty);
                sFnl = sFnl.Replace(ci.NumberFormat.NegativeSign, string.Empty);
            }
            //Debug.WriteLine("ClsNumTextTagIn--------------" + sFnl);
            decimal dFnl = decimal.Zero;
            decimal dRgx = decimal.Zero;
            if (decimal.TryParse(sFnl, out dFnl))
            {
                if (AllowNullValue && sFnl.StartsWith(".") && dFnl == decimal.Zero)
                {
                    sender.Text = "";
                    sender.SelectionStart = 0;
                    sender.SelectionLength = 0;
                    return null;
                }

                int iDtx = sFnl.IndexOf(ci.NumberFormat.NumberDecimalSeparator);
                int iDif = sFnl.Length - dFnl.ToString().Length;

                if (iDtx == -1)
                {//DecimalSeparator does not exist
                    sFnl = dFnl.ToString(sPtn);
                    int iTmp = sFnl.IndexOf(ci.NumberFormat.NumberDecimalSeparator);
                    if (iTmp == -1)
                    {//DecimalSeparator not found after apply pattern
                        if (sRgx == string.Empty)
                        {
                            iBgx = sFnl.Length;
                        }
                        else
                        {
                            if (sRgx.StartsWith(decimal.Zero.ToString()))
                            {
                                sRgx = decimal.One.ToString() + sRgx;
                                if (decimal.TryParse(sRgx, out dRgx))
                                {
                                    sRgx = dRgx.ToString(sPtn);
                                    if (sRgx.StartsWith(decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator))
                                    {
                                        iBgx = sFnl.Length - sRgx.Length + (decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator).Length;
                                    }
                                    else
                                    {
                                        iBgx = sFnl.Length - sRgx.Length + decimal.One.ToString().Length;
                                    }
                                }
                            }
                            else
                            {
                                if (decimal.TryParse(sRgx, out dRgx))
                                {
                                    sRgx = dRgx.ToString(sPtn);
                                    iBgx = sFnl.Length - sRgx.Length;
                                }
                            }
                        }
                    }
                    else
                    {//DecimalSeparator found after apply pattern
                        if (sRgx == string.Empty)
                        {
                            iBgx = iTmp;
                        }
                        else
                        {
                            if (sRgx.StartsWith(decimal.Zero.ToString()))
                            {
                                sRgx = decimal.One.ToString() + sRgx;
                                if (decimal.TryParse(sRgx, out dRgx))
                                {
                                    sRgx = dRgx.ToString(sPtn);
                                    if (sRgx.StartsWith(decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator))
                                    {
                                        iBgx = sFnl.Length - sRgx.Length + (decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator).Length;
                                    }
                                    else
                                    {
                                        iBgx = sFnl.Length - sRgx.Length + decimal.One.ToString().Length;
                                    }
                                }
                            }
                            else
                            {
                                if (decimal.TryParse(sRgx, out dRgx))
                                {
                                    sRgx = dRgx.ToString(sPtn);
                                    iBgx = sFnl.Length - sRgx.Length;
                                }
                            }
                        }
                    }

                    if (iBgx < 0)
                    {
                        iBgx = 0;
                    }
                }
                else
                {//DecimalSeparator does exist
                    if (sFnl.Length - sRgx.Length <= iDtx)
                    {//before DecimalSeparator
                        sFnl = dFnl.ToString(sPtn);
                        if (sRgx.StartsWith(decimal.Zero.ToString()))
                        {
                            sRgx = decimal.One.ToString() + sRgx;
                            if (decimal.TryParse(sRgx, out dRgx))
                            {
                                sRgx = dRgx.ToString(sPtn);
                                if (sRgx.StartsWith(decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator))
                                {
                                    iBgx = sFnl.Length - sRgx.Length + (decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator).Length;
                                }
                                else
                                {
                                    iBgx = sFnl.Length - sRgx.Length + decimal.One.ToString().Length;
                                }
                            }
                        }
                        else if (sRgx.StartsWith(ci.NumberFormat.NumberDecimalSeparator))
                        {
                            if (decimal.TryParse(sRgx, out dRgx))
                            {
                                sRgx = dRgx.ToString(sPtn);
                                iBgx = sFnl.Length - sRgx.Length + ci.NumberFormat.NumberDecimalSeparator.Length;
                            }
                        }
                        else
                        {
                            if (decimal.TryParse(sRgx, out dRgx))
                            {
                                sRgx = dRgx.ToString(sPtn);
                                iBgx = sFnl.Length - sRgx.Length;
                            }
                        }

                        if (iBgx < 0)
                        {
                            iBgx = 0;
                        }
                    }
                    else
                    {//after DecimalSeparator




                        sFnl = dFnl.ToString(sPtn);

                        var newValue = decimal.Parse(sFnl);
                        //四舍五入了，如要去掉
                        if (newValue > dFnl)
                        {
                            var de = (newValue - dFnl);
                            int i = 1;
                            var deString = de.ToString();
                            if (int.TryParse(deString.LastOrDefault().ToString(), out i))
                            {
                                dFnl = dFnl - (de / i * 5);
                                sFnl = dFnl.ToString(sPtn);      
                            };
                        }
                        dFnl = decimal.Parse(sFnl);

                        int iTmp = sFnl.IndexOf(ci.NumberFormat.NumberDecimalSeparator); //override
                        if (iTmp == -1)
                        {// pattern does not accept DecimalSeparator
                            iBgx = sFnl.Length;
                        }
                        else
                        {
                            if (sSub != string.Empty & sAdd == string.Empty)
                            {// if backspace (after decimal separator)
                                if (iTmp + 1 < iBgx)
                                {
                                    iBgx = iBgx - 1;
                                }
                                else
                                {
                                    iBgx = iTmp;
                                }
                            }

                            if (sFnl.Length > iBgx)
                            {
                                if (iBgx > iTmp)
                                {
                                    iSlx = 1;
                                }
                            }
                        }
                    }
                }
                sender.Text = sFnl;
                sender.SelectionStart = iBgx;
                sender.SelectionLength = iSlx;
            }//end of if decimal
            else
            {//not decimal
                if (IsNumOnly)
                { //IsNumOnly =true
                    decimal dOrg = decimal.Zero;

                    if (sFnl != string.Empty && decimal.TryParse(org.sTxt, out dOrg))
                    {
                        sender.Text = org.sTxt;
                        sender.SelectionStart = org.iBeg;
                        sender.SelectionLength = org.iSel;
                        return (double)dOrg;
                    }
                    else
                    {
                        if (AllowNullValue && sFnl == string.Empty)
                        {
                            sender.Text = "";
                            sender.SelectionStart = 0;
                            sender.SelectionLength = 0;
                            return null;
                        }
                        else
                        {
                            decimal dAdd = decimal.Zero;
                            decimal.TryParse(sAdd, out dAdd);
                            sFnl = dAdd.ToString(sPtn);
                            int iTmp = sFnl.IndexOf(ci.NumberFormat.NumberDecimalSeparator);

                            if (iTmp == -1)
                            {
                                iBgx = sFnl.Length;
                            }
                            else
                            {
                                iBgx = iTmp;
                            }
                            sender.Text = sFnl;
                            sender.SelectionStart = iBgx;
                            sender.SelectionLength = 0;
                        }

                    }
                }
                else
                { //numbersOnly false
                    sender.Text = edi.sTxt;
                    sender.SelectionStart = edi.iBeg;
                    sender.SelectionLength = edi.iSel;
                }
            }

            return (double)dFnl;
        }
    }
}
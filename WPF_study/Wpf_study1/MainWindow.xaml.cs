using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Wpf_study1
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private Display display = new Display { MainDisplay = "0", SubDisplay = "" };
        private Calculator calc = new Calculator();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = display;            
        }

        private void buttonCE_Click(object sender, RoutedEventArgs e)
        {
            calc.ClearEntry();
            UpdateDisplay();
        }

        private void buttonC_Click(object sender, RoutedEventArgs e)
        {
            calc.Clear();
            UpdateDisplay();
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            calc.Back();
            UpdateDisplay();
        }

        private void buttonSum_Click(object sender, RoutedEventArgs e)
        {
            calc.SetOpcode(Opcode.Sum);
            UpdateDisplay();
        }

        private void buttonSubtract_Click(object sender, RoutedEventArgs e)
        {
            calc.SetOpcode(Opcode.Subtract);
            UpdateDisplay();
        }

        private void buttonMultiply_Click(object sender, RoutedEventArgs e)
        {
            calc.SetOpcode(Opcode.Multiply);
            UpdateDisplay();
        }

        private void buttonDivide_Click(object sender, RoutedEventArgs e)
        {
            calc.SetOpcode(Opcode.Divide);
            UpdateDisplay();
        }
        
        private void buttonPlusMinus_Click(object sender, RoutedEventArgs e)
        {
            calc.ChangeSign();
            UpdateDisplay();
        }

        private void button0_Click(object sender, RoutedEventArgs e)
        {            
            InputDigit(0);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            InputDigit(1);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            InputDigit(2);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            InputDigit(3);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            InputDigit(4);            
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            InputDigit(5);            
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            InputDigit(6);            
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            InputDigit(7);            
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            InputDigit(8);            
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            InputDigit(9);            
        }

        private void InputDigit(int digit)
        {
            calc.InputDigit(digit);
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            display.Update(calc);
            mainDisplay.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            subDisplay.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
        }

        private void buttonPoint_Click(object sender, RoutedEventArgs e)
        {
            calc.StartFloatingPoint();
            UpdateDisplay();
        }

        private void buttonEqual_Click(object sender, RoutedEventArgs e)
        {
            calc.CalcResult();
            UpdateDisplay();
        }
    }

    public class Display
    {
        private string mainDisplay;
        public string MainDisplay
        {
            get { return mainDisplay; }
            set { mainDisplay = value; }
        }

        private string subDisplay;
        public string SubDisplay
        {
            get { return subDisplay; }
            set { subDisplay = value; }
        }

        public void Update(Calculator calc)
        {            
            if (null == calc.LeftOperand && Opcode.None == calc.Opcode /*&& null == rightOperand*/)
            {
                SubDisplay = "";
                MainDisplay = "";
            }
            else if (null != calc.LeftOperand && Opcode.None == calc.Opcode /*&& null == rightOperand*/)
            {
                SubDisplay = "";
                MainDisplay = calc.LeftOperand;
            }
            else if (null != calc.LeftOperand && Opcode.None != calc.Opcode && null == calc.RightOperand)
            {
                SubDisplay = calc.LeftOperand + " " + Calculator.OpcodeToString(calc.Opcode);
                MainDisplay = calc.LeftOperand;
            }
            else if (null != calc.LeftOperand && Opcode.None != calc.Opcode && null != calc.RightOperand)
            {
                SubDisplay = calc.LeftOperand + " " + Calculator.OpcodeToString(calc.Opcode);
                MainDisplay = calc.RightOperand;
            }
        }
    }

    public enum Opcode { None=0, Sum, Subtract, Multiply, Divide};
    public enum EditStatus { None = 0, LeftOperand, RightOperand };

    public class Calculator
    {
        public const char MINUS_CHAR = '-';
        public const char ZERO_CHAR = '0';
        public const char POINT_CHAR = '.';
        public const string ZERO_STRING = "0";
        public const string MINUS_ZERO_STRING = "-0";

        private StringBuilder leftOperand = new StringBuilder("0");
        private StringBuilder rightOperand = new StringBuilder("0");
        private Opcode opcode = Opcode.None;
        
        public void Back()
        {            
            StringBuilder operand = GetEditingOperand();
            if(operand.Length == 1 && operand[0] != ZERO_CHAR)
            {
                operand[0] = ZERO_CHAR;
            }
            else if(operand.Length > 1)
            {
                operand.Remove(operand.Length - 1, 1);
            }
        }

        public void CalcResult()
        {
            double operand2 = double.Parse(rightOperand.ToString());
            if (0.0 == operand2 && Opcode.Divide == opcode)
            {
                MessageBox.Show("0으로 나눌 수 없습니다!");
                return;
            }

            double operand1 = double.Parse(leftOperand.ToString());
            double result = Calc(operand1, opcode, operand2);
            
            if( (double)((int)result) == result ) // 소수점이 없으면 정수
            {
                leftOperand = new StringBuilder(((int)result).ToString());                
            }
            else
            {
                leftOperand = new StringBuilder(result.ToString());
            }

            CleanOperand(rightOperand);
            opcode = Opcode.None;
        }

        public double Calc(double operand1, Opcode opcode, double operand2)
        {
            double result = 0;
            if (Opcode.Sum == opcode)
            {
                result = operand1 + operand2;
            }
            else if (Opcode.Subtract == opcode)
            {
                result = operand1 - operand2;
            }
            else if (Opcode.Multiply == opcode)
            {
                result = operand1 * operand2;
            }
            else if(Opcode.Divide == opcode)
            {                
                result = operand1 / operand2;                
            }
            return result;
        }

        public void ChangeSign()
        {
            
            StringBuilder operand = GetEditingOperand();
            
            if(null != operand)
            {
                if (MINUS_CHAR == operand[0])
                {
                    operand.Remove(0, 1);
                }
                else
                {
                    operand.Insert(0, MINUS_CHAR);                    
                }
            }
        }

        public void ClearEntry()
        {            
            CleanOperand(GetEditingOperand());
        }

        public void Clear()
        {
            CleanOperand(leftOperand);
            opcode = Opcode.None;
            CleanOperand(rightOperand);
        }

        public void CleanOperand(StringBuilder operand)
        {
            operand.Clear();
            operand.Append(ZERO_CHAR);
        }

        private EditStatus GetEditStatus()
        {
            EditStatus status = EditStatus.None;
            if(Opcode.None == opcode)
            {
                status = EditStatus.LeftOperand;
            }
            else
            {
                status = EditStatus.RightOperand;
            }
            return status;
        }

        public void InputDigit(int digit)
        {
            StringBuilder operand = GetEditingOperand();
            string operandStr = operand.ToString();
            if (ZERO_STRING == operandStr || MINUS_ZERO_STRING == operandStr)
            {
                operand[operand.Length-1] = (char) (ZERO_CHAR + digit);
            }
            else
            {
                operand.Append(digit.ToString());
            }            
        }

        public string LeftOperand
        {
            get { return leftOperand.ToString(); }
        }
        public string RightOperand
        {
            get { return rightOperand.ToString(); }
        }
        public Opcode Opcode
        {
            get { return opcode; }
        }

        public void SetOpcode(Opcode opcode)
        {
            EditStatus status = GetEditStatus();
            if( EditStatus.LeftOperand == status)
            {
                this.opcode = opcode;
            }
            else if(EditStatus.RightOperand == status)
            {
                CalcResult();
                this.opcode = opcode;
            }
        }

        public bool StartFloatingPoint()
        {            
            StringBuilder operand = GetEditingOperand();
            if(operand.ToString().IndexOf(POINT_CHAR) == -1)
            {
                operand.Append(POINT_CHAR);
                return true;
            }
            return false;
        }

        private StringBuilder GetEditingOperand()
        {
            EditStatus status = GetEditStatus();
            StringBuilder operand = null;
            if(EditStatus.LeftOperand == status)
            {
                operand = leftOperand;
            }
            else if(EditStatus.RightOperand == status)
            {
                operand = rightOperand;
            }
            return operand;
        }

        public static string OpcodeToString(Opcode opcode)
        {
            string returnString = "";

            if (Opcode.Sum == opcode)
            {
                returnString = Properties.Resources.Op_Sum;
            }
            if (Opcode.Subtract == opcode)
            {
                returnString = Properties.Resources.Op_Subtract;
            }
            if (Opcode.Multiply == opcode)
            {
                returnString = Properties.Resources.Op_Multiply;
            }
            if(Opcode.Divide == opcode)
            {
                returnString = Properties.Resources.Op_Divide;
            }

            return returnString;
        }
    }
}

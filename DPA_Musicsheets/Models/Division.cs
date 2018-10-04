using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    class Division
    {
        private long _numerator;
        private long _denominator;
       
        public long Numerator
        {
            get
            {
                return _numerator;
            }
            set
            {
                _numerator = value;
            }
        }

        public long Denominator
        {
            get
            {
                return _denominator;
            }
            set
            {
                _denominator = value;
            }
        }

        public Division(long number)
        {
            this.Numerator = number;
            this.Denominator = 1;
        }

        public Division(string strValue)
        {
            Division temp = InitializeWithString(strValue);
            this.Numerator = temp.Numerator;
            this.Denominator = temp.Denominator;
        }

        public Division(long numerator, long denominator)
        {
            this.Denominator = denominator;
            this.Numerator = numerator;
        }

        public double ToDouble()
        {
            return ((double)this.Numerator / this.Denominator);
        }

        public override string ToString()
        {
            if (this.Denominator == 1)
            {
                return this.Numerator.ToString();
            } else
            {
                return this.Numerator + "/" + this.Denominator;
            }
        }
        
        private Division InitializeWithString(string strValue)
        {
            int i;
            for (i = 0; i < strValue.Length; i++)
                if (strValue[i] == '/')
                    break;

            if (i == strValue.Length)
                return (InitializeWithDouble(Convert.ToDouble(strValue)));

            long iNumerator = Convert.ToInt64(strValue.Substring(0, i));
            long iDenominator = Convert.ToInt64(strValue.Substring(i + 1));

            return new Division(iNumerator, iDenominator);
        }

        private Division InitializeWithDouble(double dValue)
        {
            try
            {
                checked
                {
                    Division div;
                    if (dValue % 1 == 0)
                    {
                        div = new Division((long)dValue);
                    }
                    else
                    {
                        double dTemp = dValue;
                        long iMultiple = 1;
                        string strTemp = dValue.ToString();
                        while (strTemp.IndexOf("E") > 0)  
                        {
                            dTemp *= 10;
                            iMultiple *= 10;
                            strTemp = dTemp.ToString();
                        }
                        int i = 0;
                        while (strTemp[i] != '.')
                            i++;
                        int iDigitsAfterDecimal = strTemp.Length - i - 1;
                        while (iDigitsAfterDecimal > 0)
                        {
                            dTemp *= 10;
                            iMultiple *= 10;
                            iDigitsAfterDecimal--;
                        }
                        div = new Division((int)Math.Round(dTemp), iMultiple);
                    }
                    return div;
                }
            }
            catch (OverflowException)
            {
                throw new Exception("Conversion not possible due to overflow");
            }
            catch (Exception)
            {
                throw new Exception("Conversion not possible");
            }
        }
    }
}

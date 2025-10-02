
using System;
using System.Globalization;


namespace InputLog.Core.Analyses.Copytask.Bigrams
{
    /// <summary>
    /// A container for known bigrams. This bigram can be read directly from a CSV file and
    /// it's data can be maped to the right attributes using the delegates by inheriting
    /// from the CSVReader.CSVItem and overriding InitDelegates(). 
    /// </summary>
    public class Bigram : InputLog.Core.IO.CSV.CSVItem
    {
        /// <summary>
        /// Enumeration of possible frequencies for bigrams.
        /// </summary>
        public enum FrequencyClass
        {
            Indeterminate,
            HF,
            LF
        }

        /// <summary>
        /// Enumeration of possible hand combinations required 
        /// to make a certain bigram.
        /// </summary>
        public enum HandCombination
        {
            LL,
            LR,
            RL,
            RR,
            Unknown
        }

        public int Frequency { get; private set; }
        public string Value { get; private set; }
       // public double Percentile100 { get; private set; }
        public FrequencyClass FreqClass { get; private set; }
        public bool Adjacent { get; private set; }
        public bool Repetitive { get; private set; }
        public HandCombination Hand { get; private set; }

        public Bigram()
        {
            this.FreqClass = FrequencyClass.Indeterminate;
        }

        /// <summary>
        /// Initialize delegates. This creates a mapping between CSVHeader names
        /// and attributes for this CSVItem. A bigram will be read from a CSVLine
        /// as a single CSVItem. The headers in the CSVFile may be mapped to methods
        /// of this Bigram class that will set the respective attributes in this instance
        /// of the class.
        /// </summary>
        protected override void InitSetDelegates()
        {
            this._setDelegates.Add("bigram", this.SetBigram);
            this._setDelegates.Add("freq", this.SetFrequency);
            this._setDelegates.Add("freq10", this.SetFreq10);
            this._setDelegates.Add("freq20", this.SetFreq20);
            this._setDelegates.Add("freq30_50", this.SetFreq30);
            this._setDelegates.Add("adjacent", this.SetAdjacent);
            this._setDelegates.Add("repetitive", this.SetRepetitive);
            this._setDelegates.Add("hand", this.SetHandComb);
        }

        /// <summary>
        /// Initialize delegates. This creates a mapping between CSVHeader names
        /// and attributes for this CSVItem. A bigram 's attribute can be written to a
        /// CSV file based on the header name for the attribute and the delegate function
        /// linked to it.
        /// </summary>
        protected override void InitGetDelegates()
        {
            this._getDelegates.Add("bigram", this.GetBigram);
            this._getDelegates.Add("freqClass", this.GetFrequencyClass);
            this._getDelegates.Add("adjacent", this.GetAdjacent);
            this._getDelegates.Add("repetitive", this.GetRepetitive);
            this._getDelegates.Add("hand", this.GetHandComb);
        }

        # region Delegates for setting the bigram's values
        public void SetBigram(string bigram)
        {
            this.Value = bigram;
        }

        public string GetBigram()
        {
            return this.Value;
        }

        public void SetFrequency(string frequency)
        {
            try
            {
                this.Frequency = int.Parse(frequency);
            }
            catch (Exception e)
            {
                throw new BigramValueException(
                    "[Invalid Format] Frequency value (\"" +
                    frequency + "\") is not an integer", e
                );
            }
        }

        public string GetFrequencyClass()
        {
            return this.FreqClass.ToString();
        }

        public void SetFreq10(string freq10)
        {
            this.SetFreqClass(freq10);
        }

        public void SetFreq20(string freq20)
        {
            this.SetFreqClass(freq20);
        }

        public void SetFreq30(string freq30)
        {
            this.SetFreqClass(freq30);
        }


        private void SetFreqClass(string freq)
        {
            FrequencyClass newFrequencyClass = this.FreqStringToClass(freq);

            bool FreqNotSet = this.FreqClass == FrequencyClass.Indeterminate;
            bool NewAndOldFreqClassesDiffer = this.FreqClass != newFrequencyClass;
            bool NewFreqIsSpecific = newFrequencyClass != FrequencyClass.Indeterminate;

            // The frequency may only be changed if it isn't set yet. It may only be changed to 
            // a class that is more specific. (HF, LF are more specific than Inderterminate)
            if (FreqNotSet)
            {
                this.FreqClass = newFrequencyClass;
            }
            else if (NewAndOldFreqClassesDiffer && NewFreqIsSpecific)
            {
                throw new BigramValueException(
                    "[Inconsistent Values] Freq10, Freq20, and Freq30 values for bigram are inconsistent."
                );
            }
        }

        private FrequencyClass FreqStringToClass(string freqClass)
        {
            switch (freqClass)
            {
                case "HF":
                    return FrequencyClass.HF;
                case "LF":
                    return FrequencyClass.LF;
                default:
                    if (String.IsNullOrWhiteSpace(freqClass))
                    {
                        return FrequencyClass.Indeterminate;
                    }
                    else
                    {
                        throw new BigramValueException(
                            "[Invalid Format] Frequency Class (\"" + freqClass + "\") " +
                            "is invalid. Valid values are \"\", \"LF\", \"HF\"."
                        );
                    }
            }

        }

        public void SetAdjacent(string adjacent)
        {
            try
            {
                this.Adjacent = this.ParseBool(adjacent);
            }
            catch (Exception e)
            {
                throw new BigramValueException(
                    "[Invalid Format] Adjacent value (\"" + adjacent +
                    "\") is not a valid true (1) or false (0) value.",
                    e
                );
            }
        }

        public string GetAdjacent()
        {
            return this.Adjacent ? "1" : "0";
        }

        public void SetRepetitive(string repetitive)
        {
            try
            {
                this.Repetitive = this.ParseBool(repetitive);
            }
            catch (Exception e)
            {
                throw new BigramValueException(
                    "[Invalid Format] Reptitive value (\"" + repetitive +
                    "\") is not a valid true (1 or \"true\") or false (0 or \"false\") value.",
                    e
                );
            }
        }

        public string GetRepetitive()
        {
            return this.Repetitive ? "1" : "0";
        }

        private bool ParseBool(string value)
        {
            if (value == "0")
            {
                return false;
            }
            else if (value == "1")
            {
                return true;
            }
            else
            {
                return bool.Parse(value);
            }
        }

        public void SetHandComb(string handComb)
        {
            switch (handComb)
            {
                case "LL":
                    this.Hand = HandCombination.LL;
                    break;
                case "LR":
                    this.Hand = HandCombination.LR;
                    break;
                case "RL":
                    this.Hand = HandCombination.RL;
                    break;
                case "RR":
                    this.Hand = HandCombination.RR;
                    break;
                case "":
                    this.Hand = HandCombination.Unknown;
                    break;
                default:
                    throw new BigramValueException(
                        "[Invalid Format] Hand Combination value (" + handComb +
                        ") is not valid. Valid combinations are \"\", \"LL\", \"LR\", \"RL\" and \"RR\""
                    );
            }
        }

        public string GetHandComb()
        {
            return this.Hand.ToString();
        }
        #endregion
    }


    /// <summary>
    /// Exception thrown when an invalid value has been encountered
    /// for one of the bigram values.
    /// </summary>
    public class BigramValueException : Exception
    {
        public BigramValueException(string message, Exception e = null) : base(message, e) { }
    }
}

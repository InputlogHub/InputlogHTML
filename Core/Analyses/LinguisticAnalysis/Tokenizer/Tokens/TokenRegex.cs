
namespace InputLog.Core.Analyses.LinguisticAnalysis.Tokenizer.Tokens
{
    /// <summary>
    ///  Tokenizing Text with ICU4j's RuleBasedBreakIterator
    /// file:///C:/Users/EVH/Documents/InputlogDoc/Tokenizer/Salmon%20Run%20Tokenizing%20Text%20with%20ICU4j's%20RuleBasedBreakIterator.htm
    /// Collection of regexes (regular expressions) to find specific token properties.
    /// </summary>
    public static class TokenRegex
    {
        // ** W-NOTATION RELATED REGEXES¨**
        // A 'deletion' in W-Notation starts with a digit surrounded by underscores. 
        // It precedes a square opening bracket ('[') before the deleted string sequence 
        // (that may contain other markup symbols or blanks/spaces) and is followed by a square 
        // closing bracket (']') followed by the same digit surrounded by underscores.
        public const string DELETE_MARKUP = @"\u005F([0-9]*)\u005F\[.*?\s?\]\u005F\1\u005F";
        public const string NESTEDDELETE = @"\[.*?\s?\][^]]*\]";
        public const string SHORTDELETE = @"\[.*?\s?\]";
        // An 'insertion' in W-Notation has curly brackets preceded and followed with digits surrounded by underscores.
        public const string INSERT_MARKUP = @"\u005F([0-9]*)\u005F\{.*?\s?\}\u005F\1\u005F";
        // A left insert symbol in W-Notation: a digit surrounded by underscores followed by a curly opening brace ('{').
        public const string LEFT_INSERT_MARKUP = @"\u005F[0-9]*\u005F\{";
        // A right insert symbol in W-Notation: a curly closing brace ('}') followed by a digit surrounded by underscores.
        public const string RIGHT_INSERT_MARKUP = @"\}\u005F[0-9]*\u005F";
        // A left delete symbol in W-Notation: a digit surrounded by underscores followed by a opening bracket('[') 
        // and one or zero symbols or an optional blank.
        public const string LEFT_DELETE_MARKUP = @"\u005F[0-9]*\u005F\[.\s?";
        public const string SHORTL_DELETE_MARKUP = @"\u005F[0-9]*\u005F\[";
        // A right delete symbol in W-Notation: a closing bracket (']') followed by a digit surrounded by underscores 
        // and one or zero symbols or an optional blank.
        public const string RIGHT_DELETE_MARKUP = @".\s?\]\u005F[0-9]*\u005F";
        public const string SHORTR_DELETE_MARKUP = @"\]\u005F[0-9]*\u005F";
        // A Break in W-Notation: a caret ('^') followed by a digit surrounded by underscores.
        public const string BREAK_MARKUP = @"\^\u005F[0-9]*\u005F";
        // Whitespaces are represented by a 'middle dot' in the W-Notation.
        public const string WHITE_SPACE_DOT = @"\u00B7";
        // An unspecified markup symbol in W-Notation: a digit surrounded by underscores.
        public const string MARKUP = @"(\u005F[0-9]*\u005F)";
        // The underscore character used in the markup symbols.
        public const string UNDERSCORE = @"\u005F";
        // Remaining closing markup symbols.
        public const string REMAINS = @"^[.\s]?]"; // +|^[.\s]?}+
        // Token candidate in a W-Notation string with middle dot as word boundary and on optional right insertion markup
        public const string TOKEN = @".+?\u00B7(\}\u005F[0-9]*\u005F)?"; // @".+?\u00B7";
        // Token candidate with Unicode punctuation class inside, including apostrophe, ignoring single and double quotes.
        public const string TOKEN_BIS = @".+?(?![""'])([\p{Po}\u2019]).+?"; // @".+?([\p{Po}\u2019]).+?";
        // A deletion with missing left surrounding digits.
        public const string SLDELETE = @"\[.*?\s?\]\u005F[0-9]*\u005F";
        // A deletion with missing right surrounding digits.
        public const string SRDELETE = @"\u005F[0-9]*\u005F\[.*?\s?\]";
        // A trailing brace at beginning or end.
        public const string LBRACE = @"\{";
        public const string RBRACE = @"}$";
        public const string LBRACKET = @"\[";
        public const string RBRACKET = @"\]$";
        // Looking for a break with the same number as the most near insert or deletion.
        public const string RREVISION_DEL = @".\s?\]\u005F([0-9]*)\u005F.*?\^\u005F\1\u005F";
        public const string RREVISION_INS = @"\}\u005F([0-9]*)\u005F.*?\^\u005F\1\u005F";
        public const string LREVISION_DEL = @"\^\u005F([0-9]*)\u005F.*?\u005F\1\u005F\[.\s?";
        public const string LREVISION_INS = @"\^\u005F([0-9]*)\u005F.*?\u005F\1\u005F\{";

        // ** STANDARD REGEXES **
        // The regex for a 'word'.
        public const string WORD = @"\w+";
        // Unicode punctuation class 'other'.
        public const string PUNCT = @"\p{Po}";
        // Unicode punctuation class at end of a word, except underscore, and middle dot but allows the french apostrophe.
        public const string FRENCH_PUNCT = @"(?![\u00B7\u005F{}\[\]])(\p{Po}|\p{Pe}|\p{Pd})";
        // Unicode punctuation class at end of a word, except underscore, apostrophe, and middle dot.
        public const string CLOSE_PUNCT = @"(?![\u0027\u00B7\u005F{}\[\]])(\p{Po}|\p{Pe})";
        // Spanish inverted question and exclamation mark at begin of a word, except underscore, apostrophe, and middle dot.
        public const string OPEN_PUNCT = @"(?![\u0027\u00B7\u005F{}\[\]])(\u00BF|\u00A1)";
        // Newline, tab, form feed.
        public const string LINEBREAKS = @"\r\n?|\n+|\f+"; // @"\s+";
        // Abbreviation: Alphanumeric chars separated by period and optionally followed by a period.
        public const string ABBREVIATION = @"\b[A-Za-z0-9](\.[A-Za-z0-9])+(\.)*";
        // Short Abbreviation: At least 2 alphanumeric chars followed by a period and a space (middle dot!), but not by an EOL.
        public const string SHORT_ABBREVIATION = @"\b\w{1,}\.(?:\u00B7|\W)(?!$)";
        // Hyphenated Word : sequence of letter or digit, punctuated by '-' or '_', with following letter or digit sequence.
        public const string HYPHENATED_WORD = @"[A-Za-z0-9]+([\-_][A-Za-z0-9]+)+";
        // Email address: sequence of letters, digits and punctuation followed by @ and followed by another sequence.
        public const string EMAIL_ADDRESS = @"[A-Za-z0-9_\-\.]+\@[A-Za-z][A-Za-z0-9_]+\.[a-z]+";
        // Internet Addresses
        public const string INTERNET_ADDRESS = @"[a-z]+\:\/\/[a-z0-9]+(\.[a-z0-9]+)+(\/[a-z0-9][a-z0-9\.]+)";
        //URL
        public const string URL = @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*";
        // EU_Date: European date format dd-mm-yyyy with four possible separators (.,  , /, -)
        public const string EU_DATE = @"\b(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d";
        // Norm_Date: Normalized date format yyyymmdd, yyyy-mm-dd (BIN), yyyy mm dd, yyyy.mm.dd, yyyy/mm/dd
        public const string NORMALIZED_DATE = @"\b(19|20)\d\d[- /.]?(0[1-9]|1[012])[- /.]?(0[1-9]|[12][0-9]|3[01])";
        // US_Date: US date format mm-dd-yyyy with four possible separators (.,  , /, -). 
        public const string US_DATE = @"\b(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d";
    }
}

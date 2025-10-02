%namespace InputLog.Core.Analyses.PauseParser

//%option stack, babel, minimize, parser, verbose, persistbuffer, unicode, compressNext, embedbuffers
%option stack, parser, verbose, unicode

%{
public override void yyerror(string format, params object[] args)
{
	System.Console.Error.WriteLine("Error: line {0} - " + format, yyline);
}
%}

//
//Tokens
//
CommentStart	\/\*
CommentEnd		\*\/
LineComment		"//".*
LPar			"("
RPar			")"
Star			"*"
LBrack			"["
RBrack			"]"
LBrace			"{"
RBrace			"}"
LAngle			"<"
RAngle			">"
Identifier		([a-zA-Z]([a-zA-Z0-9_])*)
Number			[0-9]+
Arrow			"->"
Dollar			"$"
Hash			"#"
Comma			","
Period			"."
Excl			"!"
Qst				"?"
Colon			":"
SemiColon		";"
Pipe			"|"
Plus			"+"
Minus			"-"
WhiteSpace		[ \t]
Eol				(\r\n?|\n)


// The states into which this FSA can pass.
%x CMMT		// Inside a comment.
%x CMMT2	// Inside a comment.
%%

//
// Scanner Rules
//
// Remove whitespaces.
{WhiteSpace}+	{ ; }

// End of Line 
{Eol}+			{ ; }

// Remove these lines 
{LineComment}+			{yy_push_state (CMMT2);}//return (int) Tokens.COMMENT; }
<CMMT2>{
	{Eol} { yy_pop_state ();}
}

/* Move to a 'comment' state on seeing comments. */
{CommentStart}					{  yy_push_state (CMMT); }//return (int) Tokens.COMMENT; }

// Inside a block comment.
<CMMT>{
	[^*\n]+				{/* return (int) Tokens.COMMENT;*/}
	"*"					{/* return (int) Tokens.COMMENT; */}
	{CommentEnd}		{ yy_pop_state(); /*return (int) Tokens.COMMENT;*/}
	<<EOF>>				{ ; /* raise an error. */ }
}

{Identifier}		{yylval.String = yytext;
					  return (int) Tokens.IDENTIFIER;}

{Arrow}				{return (int) Tokens.ARROW;}

{LAngle}			{return (int) Tokens.LANGLE;}

{RAngle}			{return (int) Tokens.RANGLE;}

{Dollar}			{return (int) Tokens.DOLLAR;}

{Hash}				{return (int) Tokens.HASH;}

{Comma}				{return (int) Tokens.COMMA;}

{Colon}				{return (int) Tokens.COLON;}

{SemiColon}			{return (int) Tokens.SEMICOLON;}

{Number}			{yylval.String = yytext;
					return (int) Tokens.NUMBER;}

{LPar}				{return (int) Tokens.LPAR;}

{RPar}				{return (int) Tokens.RPAR;}

{Star}				{return (int) Tokens.STAR;}

{LBrack}			{return (int) Tokens.LBRACK;}

{RBrack}			{return (int) Tokens.RBRACK;}

{LBrace}			{return (int) Tokens.LBRACE;}

{RBrace}			{return (int) Tokens.RBRACE;}

{Pipe}				{return (int) Tokens.PIPE;}

{Period}			{return (int) Tokens.PERIOD;}

{Excl}				{return (int) Tokens.EXCL;}

{Qst}				{return (int) Tokens.QST;}

{Plus}				{return (int) Tokens.PLUS;}

{Minus}				{return (int) Tokens.MINUS;}
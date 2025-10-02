%using System.Collections.Generic;
%using InputLog.Core.Analyses;
%using InputLog.Core.Util;

%output=Parser.cs
%namespace InputLog.Core.Analyses.PauseParser

%{
	private AFSMFactory<string> factory;
	private IRuleFactory<string> ruleFactory;
	private List<ARegexRule<string>> ruleSet;

%}

//Starting rule
%start fsmstart

//Define possible types
%union {
	public string String;//Otherwise error on "string" being a keyword
	public bool Bool;//Otherwise error on "bool" being a keyword
	public int Int;//Otherwise error on "int" being a keyword
    public List<string> Stringlist;
	public List<Triple<string,bool,int>> Stringboolintlist;
	public ARegexRule<string> Ruletype;
}

//Define tokens and potential returntypes
%token COMMENT
%token <String> IDENTIFIER
%token DOLLAR
%token COMMA
%token HASH
%token COLON
%token SEMICOLON
%token ARROW
%token <String> NUMBER
%token LPAR
%token RPAR
%token STAR
%token LBRACK
%token RBRACK
%token LBRACE
%token RBRACE
%token LANGLE
%token RANGLE
%token PIPE
%token PERIOD
%token EXCL
%token QST
%token PLUS
%token MINUS
%token EOL


//Define returntypes of parser rules
%type <Stringlist> identlist
%type <Stringlist> identlist2
%type <Bool> eventinverse
%type <Int> eventpeek
%type <Stringboolintlist> eventlist
%type <Stringboolintlist> eventlist2
%type <Stringlist> actionlist
%type <Ruletype> rule
%type <Ruletype> compoundRule


// YACC Rules
%%
fsmstart		:	rulelist
				;

ruleend			:	SEMICOLON
				;

rulelist		:	rule ruleend rulelist
					{
						ruleSet.Insert(0, $1); 
					}

				|	/* Empty */
					{
						//Do nothing
					}
				;

//Builds up a list, starting from the back, inserting elements in the front
identlist		:	IDENTIFIER identlist2 {$2.Insert(0, $1);}
				;

identlist2		: 	COMMA identlist 
					{
						$$ = $2;
					}

				|	/* Empty */ 
					{
						$$ = new List<string>();
					}
				;

//Specialised version of identlist
actionlist		:	LBRACE identlist RBRACE 
					{
						$$ = $2;
					}

				|	/* Empty */ 
					{
						$$ = new List<string>();
					}
				;
//Specialised version of identlist
eventinverse	:	EXCL		
					{ 
						$$ = true;
					}

				|	/* Empty */	
					{ 
						$$ = false;
					}
				;

eventpeek		:	LANGLE NUMBER RANGLE		
					{ 
						$$ = int.Parse($2);
					}

				|	/* Empty */	
					{ 
						$$ = 0;
					}
				;

eventlist		:	eventinverse IDENTIFIER eventpeek eventlist2 
					{
						$4.Insert(0, new Triple<string,bool, int>($2, $1, $3));
					}
				;

eventlist2		: 	COMMA eventlist 
					{
						$$ = $2;
					}

				|	/* Empty */ 
					{
						$$ = new List<Triple<string,bool,int>>();
					}
				;

//	Returns the identifier of the start and end states of the rule in a pair
rule			:	compoundRule
					{
						$$ = $1;
					}

				//	Base case last
				|	eventlist actionlist
					{
						$$ = ruleFactory.MakeSymbolRule($1, $2);
					}
				;

compoundRule	:	//	Unary operators first!	
					LPAR rule RPAR actionlist
						{
							$$ = ruleFactory.MakeParenthesesRule($2, $4);
						}

					|	rule STAR actionlist
						{
							$$ = ruleFactory.MakeKleeneStarRule($1, $3);
						}

					|	rule PLUS actionlist
						{
							$$ = ruleFactory.MakePlusRule($1, $3);
						}
					
					|	rule LBRACK NUMBER RBRACK actionlist
						{
							int times = int.Parse($3);
							$$ = ruleFactory.MakeNumberRule($1, times, $5);
						}
					
					//	Binary operators, order is important here (correct grouping)
					|	rule PERIOD rule actionlist
						{
							$$ = ruleFactory.MakeConcatRule($1, $3, $4);
						}

					|	rule PIPE rule actionlist
						{
							$$ = ruleFactory.MakeUnionRule($1, $3, $4);
						}
					;
				

%%
/// <summary>
/// Constructor
/// </summary>
/// <param name="scn">Scanner processing the file</param>
/// <param name="factory">Factory to build the finite state machine</param>
/// <param name="ruleFactory">Factory to build the rules</param>
public Parser(Scanner scn, AFSMFactory<string> factory, IRuleFactory<string> ruleFactory) : base(scn) 
{
	this.factory = factory;
	this.ruleFactory = ruleFactory;
	this.ruleSet =  new List<ARegexRule<string>>();
}

/// <summary>
/// Returns the NFAs that were read by this parser
/// </summary>
///<returns>A list of epsilon non-deterministic finite automata, as they were read from file</returns>
public List<NonDeterministicFiniteStateMachine<string>> GetNonDeterministicFiniteStateMachines()
{
	List<NonDeterministicFiniteStateMachine<string>> enfsmList = new List<NonDeterministicFiniteStateMachine<string>>();
	
	this.Parse();

	foreach(ARegexRule<string> startRule in ruleSet)
	{
		factory.NewFiniteStateMachine();
		string startState = string.Empty;
		if(startRule != null)
		{
			Pair<string, string> startPair = startRule.AddToFSM(factory);
			startState = startPair.First;
		}
		NonDeterministicFiniteStateMachine<string> enfsm = factory.BuildFiniteStateMachine();
		enfsm.SetStartState(startState);
		enfsmList.Add(enfsm);
	}
	return enfsmList;
}

/// <summary>
/// Returns a list of DFAs, equivalent to the NFAs that were read by this parser
/// </summary>
///<returns>A list of deterministic finite automata</returns>
public List<DeterministicFiniteStateMachine<ISet<string>>> GetDeterministicFiniteStateMachines()
{
	List<NonDeterministicFiniteStateMachine<string>> enfsmList = this.GetNonDeterministicFiniteStateMachines();
	List<DeterministicFiniteStateMachine<ISet<string>>> dfsmList = new List<DeterministicFiniteStateMachine<ISet<string>>>();
	foreach(NonDeterministicFiniteStateMachine<string> enfsm in enfsmList)
	{
		dfsmList.Add(DeterministicFiniteStateMachine<string>.ConstructDFA(enfsm));
	}
	return dfsmList;
}

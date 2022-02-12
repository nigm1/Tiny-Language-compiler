using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny_Language
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string Namee)
        {
            this.Name = Namee;
        }
    }

    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }

        void printError(string Expected, int inputPointer = -1)
        {
            if (inputPointer == -1)
                inputPointer = InputPointer;
            Errors.Error_List.Add("Parsing Error: Expected "
                        + Expected + " and " +
                        TokenStream[inputPointer].token_type.ToString() +
                        "  found\r\n");
            InputPointer++;
        }

        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(ProgramX());
            program.Children.Add(MainFunction());
            MessageBox.Show("Success");
            return program;
        }

        Node ProgramX()
        {
            Node programx = new Node("Program'");
            if (InputPointer + 1 < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float
                || TokenStream[InputPointer].token_type == Token_Class.String) && TokenStream[InputPointer + 1].token_type != Token_Class.Main)
            {
                programx.Children.Add(FunctionStatement());
                programx.Children.Add(ProgramX());
                return programx;
            }
            else
                return null;
        }

        Node FunctionStatement()
        {
            Node functionStatement = new Node("Function Statement");
            functionStatement.Children.Add(FunctionDeclaration());
            functionStatement.Children.Add(FunctionBody());
            return functionStatement;
        }

        Node FunctionDeclaration()
        {
            Node functionDec = new Node("Function Declaration");
            functionDec.Children.Add(Datatype());
            functionDec.Children.Add(match(Token_Class.Idenifier));
            functionDec.Children.Add(match(Token_Class.LParanthesis));
            functionDec.Children.Add(Parameters());
            functionDec.Children.Add(match(Token_Class.RParanthesis));
            return functionDec;
        }

        Node Parameters()
        {
            Node parameteres = new Node("Parameters");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float
                || TokenStream[InputPointer].token_type == Token_Class.String))
            {
                parameteres.Children.Add(Parameter());
                parameteres.Children.Add(ParametersX());
                return parameteres;
            }
            else
                return null;
        }

        Node ParametersX()
        {
            Node parameterX = new Node("Parameter'");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                parameterX.Children.Add(match(Token_Class.Comma));
                parameterX.Children.Add(Parameter());
                parameterX.Children.Add(ParametersX());
                return parameterX;
            }
            else
                return null;
        }

        Node Parameter()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Token_Class.Idenifier));
            return parameter;
        }

        Node Datatype()
        {
            Node datatype = new Node("Datatype");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Int)
            {
                datatype.Children.Add(match(Token_Class.Int));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                datatype.Children.Add(match(Token_Class.Float));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                datatype.Children.Add(match(Token_Class.String));
            }
            else
            {
                printError("Datatype");
            }
            return datatype;
        }

        Node MainFunction()
        {
            Node main = new Node("Main");
            main.Children.Add(Datatype());
            main.Children.Add(match(Token_Class.Main));
            main.Children.Add(match(Token_Class.LParanthesis));
            main.Children.Add(match(Token_Class.RParanthesis));
            main.Children.Add(FunctionBody());
            return main;
        }

        Node FunctionBody()
        {
            Node functionBody = new Node("Function Body");
            functionBody.Children.Add(match(Token_Class.LCurlyBracket));
            functionBody.Children.Add(Statements());
            functionBody.Children.Add(ReturnStatement());
            functionBody.Children.Add(match(Token_Class.RCurlyBracket));
            return functionBody;
        }

        Node ReturnStatement()
        {
            Node returnStatement = new Node("Return Statement");
            returnStatement.Children.Add(match(Token_Class.Return));
            returnStatement.Children.Add(Expression());
            returnStatement.Children.Add(match(Token_Class.Semicolon));
            return returnStatement;
        }

        Node Expression()
        {
            Node exp = new Node("Expression");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                exp.Children.Add(match(Token_Class.String));
            }
            //is Term?     
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.LParanthesis
              || (TokenStream[InputPointer].token_type == Token_Class.Constant || TokenStream[InputPointer].token_type == Token_Class.Idenifier)))
            {
                exp.Children.Add(Equation());
            }
            else
                printError("Expression");
            return exp;
        }

        Node Equation()
        {
            Node equation = new Node("Equation");
            equation.Children.Add(Factor());
            equation.Children.Add(EquationX());
            return equation;
        }

        Node Factor()
        {
            Node factor = new Node("Factor");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                factor.Children.Add(match(Token_Class.LParanthesis));
                factor.Children.Add(Equation());
                factor.Children.Add(match(Token_Class.RParanthesis));
                factor.Children.Add(FactorX());
            }
            //is Term? 
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Constant || TokenStream[InputPointer].token_type == Token_Class.Idenifier))
            {
                factor.Children.Add(Term());
                factor.Children.Add(FactorX());
            }
            else
                printError("Factor");
            return factor;
        }

        Node Term()
        {
            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Constant)
            {
                term.Children.Add(match(Token_Class.Constant));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                {
                    term.Children.Add(FunctionCall());
                }
                else
                    term.Children.Add(match(Token_Class.Idenifier));
            }
            else
                printError("Term");
            return term;
        }

        Node FactorX()
        {
            Node factorX = new Node("Factor'");
            //is multi operation?
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.DivideOp))
            {
                factorX.Children.Add(MultOp());
                factorX.Children.Add(Equation());
                factorX.Children.Add(FactorX());
                return factorX;
            }
            else
                return null;
        }

        Node EquationX()
        {
            Node equationX = new Node("Equation'");
            //is add operation?
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.PlusOp || TokenStream[InputPointer].token_type == Token_Class.MinusOp))
            {
                equationX.Children.Add(AddOp());
                equationX.Children.Add(Factor());
                equationX.Children.Add(EquationX());
                return equationX;
            }
            else
                return null;
        }

        Node AddOp()
        {
            Node addOp = new Node("Add Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                addOp.Children.Add(match(Token_Class.PlusOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                addOp.Children.Add(match(Token_Class.MinusOp));
            }
            else
                printError("Addition Operator");
            return addOp;
        }
        Node MultOp()
        {
            Node multOp = new Node("Mult Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                multOp.Children.Add(match(Token_Class.MultiplyOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                multOp.Children.Add(match(Token_Class.DivideOp));
            }
            else
                printError("Multiply or Divide Operator");
            return multOp;
        }
        Node Statements()
        {
            Node statements = new Node("Statements");
            //is statment?
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float
                || TokenStream[InputPointer].token_type == Token_Class.String || TokenStream[InputPointer].token_type == Token_Class.Write ||
                TokenStream[InputPointer].token_type == Token_Class.Read || TokenStream[InputPointer].token_type == Token_Class.Idenifier ||
                TokenStream[InputPointer].token_type == Token_Class.If || TokenStream[InputPointer].token_type == Token_Class.Repeat))
            {
                statements.Children.Add(Statement());
                statements.Children.Add(StatementsX());
                return statements;
            }
            else
                return null;
        }

        Node StatementsX()
        {
            Node statementX = new Node("Statements'");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float
                || TokenStream[InputPointer].token_type == Token_Class.String || TokenStream[InputPointer].token_type == Token_Class.Write ||
                TokenStream[InputPointer].token_type == Token_Class.Read || TokenStream[InputPointer].token_type == Token_Class.Idenifier ||
                TokenStream[InputPointer].token_type == Token_Class.If || TokenStream[InputPointer].token_type == Token_Class.Repeat))
            {
                statementX.Children.Add(Statement());
                statementX.Children.Add(StatementsX());
                return statementX;
            }
            else
                return null;
        }

        Node Statement()
        {
            Node statement = new Node("Statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                // Assignment
                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOp)
                {
                    statement.Children.Add(AssignmentStatment());
                    statement.Children.Add(match(Token_Class.Semicolon));
                }
                //Condition
                //is condition operation?         
                else if (InputPointer + 1 < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.LessThanOp || TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp ||
                TokenStream[InputPointer].token_type == Token_Class.EqualOp || TokenStream[InputPointer].token_type == Token_Class.NotEqualOp))
                {
                    statement.Children.Add(ConditionStatement());
                }
                //Function Call
                else if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                {
                    statement.Children.Add(FunctionCall());
                    statement.Children.Add(match(Token_Class.Semicolon));
                }
                else
                    printError("Assignment or Condition or Function", InputPointer + 1);
            }
            // Declaration
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float
                || TokenStream[InputPointer].token_type == Token_Class.String))
            {
                statement.Children.Add(DeclarationStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                statement.Children.Add(WriteStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                statement.Children.Add(ReadStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.If)
            {
                statement.Children.Add(IfStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Repeat)
            {
                statement.Children.Add(RepeatStatement());
            }
            else
                printError("Statement");
            return statement;
        }

        Node RepeatStatement()
        {
            Node repeatStatement = new Node("Repeat Statement");
            repeatStatement.Children.Add(match(Token_Class.Repeat));
            repeatStatement.Children.Add(Statements());
            repeatStatement.Children.Add(match(Token_Class.Until));
            repeatStatement.Children.Add(ConditionStatement());
            return repeatStatement;
        }

        Node IfStatement()
        {
            Node ifStatement = new Node("If Statement");
            ifStatement.Children.Add(match(Token_Class.If));
            ifStatement.Children.Add(ConditionStatement());
            ifStatement.Children.Add(match(Token_Class.Then));
            ifStatement.Children.Add(StatementsOrReturn());
            ifStatement.Children.Add(ELSE_PART());

            return ifStatement;
        }
        Node StatementsOrReturn()
        {
            Node statementsOrReturn = new Node("Statements Or Return");
            //is statment?
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float
                || TokenStream[InputPointer].token_type == Token_Class.String || TokenStream[InputPointer].token_type == Token_Class.Write ||
                TokenStream[InputPointer].token_type == Token_Class.Read || TokenStream[InputPointer].token_type == Token_Class.Idenifier ||
                TokenStream[InputPointer].token_type == Token_Class.If || TokenStream[InputPointer].token_type == Token_Class.Repeat))
            {
                statementsOrReturn.Children.Add(Statements());
                statementsOrReturn.Children.Add(StatementsOrReturn());
                return statementsOrReturn;
            }
            //is return?
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                statementsOrReturn.Children.Add(ReturnStatement());
                return statementsOrReturn;
            }
            else
                return null;
        }
        Node ELSE_PART()
        {
            Node else_part = new Node("Else part");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Elseif)
            {
                else_part.Children.Add(ElseIfStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                else_part.Children.Add(ElseStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.End)
            {
                else_part.Children.Add(match(Token_Class.End));
            }
            else
                printError("End");
            return else_part;
        }

        Node ElseStatement()
        {
            Node elseStatement = new Node("Else Statement");
            elseStatement.Children.Add(match(Token_Class.Else));
            elseStatement.Children.Add(StatementsOrReturn());
            elseStatement.Children.Add(match(Token_Class.End));
            return elseStatement;
        }

        Node ElseIfStatement()
        {
            Node elseIfStatement = new Node("Else If Statement");
            elseIfStatement.Children.Add(match(Token_Class.Elseif));
            elseIfStatement.Children.Add(ConditionStatement());
            elseIfStatement.Children.Add(match(Token_Class.Then));
            elseIfStatement.Children.Add(StatementsOrReturn());
            elseIfStatement.Children.Add(ELSE_PART());
            return elseIfStatement;
        }

        Node ReadStatement()
        {
            Node readStatement = new Node("Read Statement");
            readStatement.Children.Add(match(Token_Class.Read));
            readStatement.Children.Add(match(Token_Class.Idenifier));
            readStatement.Children.Add(match(Token_Class.Semicolon));
            return readStatement;
        }

        Node WriteStatement()
        {
            Node writeStatement = new Node("Write Statement");
            writeStatement.Children.Add(match(Token_Class.Write));
            writeStatement.Children.Add(WriteD());
            writeStatement.Children.Add(match(Token_Class.Semicolon));
            return writeStatement;
        }

        Node WriteD()
        {
            Node writeD = new Node("Write'");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                writeD.Children.Add(match(Token_Class.Endl));
            }
            //is term?
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.String ||
                TokenStream[InputPointer].token_type == Token_Class.LParanthesis || (TokenStream[InputPointer].token_type == Token_Class.Constant || TokenStream[InputPointer].token_type == Token_Class.Idenifier)))
            {
                writeD.Children.Add(Expression());
            }
            else
                printError("Write Statement");
            return writeD;
        }

        Node AssignmentStatment()
        {
            Node assign = new Node("Assignment Statment");
            assign.Children.Add(match(Token_Class.Idenifier));
            assign.Children.Add(match(Token_Class.AssignmentOp));
            assign.Children.Add(Expression());
            return assign;
        }

        Node DeclarationStatement()
        {
            Node declarationStatement = new Node("Declaration Statement");
            declarationStatement.Children.Add(Datatype());
            declarationStatement.Children.Add(D_Identifier());
            declarationStatement.Children.Add(match(Token_Class.Semicolon));
            return declarationStatement;
        }

        Node D_Identifier()
        {
            Node d_id = new Node("D_IDs");
            if (InputPointer + 1 < TokenStream.Count && (TokenStream[InputPointer + 1].token_type == Token_Class.Comma || TokenStream[InputPointer + 1].token_type == Token_Class.Semicolon))
            {
                d_id.Children.Add(match(Token_Class.Idenifier));
                d_id.Children.Add(D_Identifierx());
            }
            else if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.AssignmentOp)
            {
                d_id.Children.Add(AssignmentStatment());
                d_id.Children.Add(D_Identifierx());
            }
            else
                printError("Identifiers");
            return d_id;
        }

        Node D_Identifierx()
        {
            Node d_idX = new Node("D_IDsX");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                d_idX.Children.Add(match(Token_Class.Comma));
                d_idX.Children.Add(D_Identifier());
                return d_idX;
            }
            else
                return null;
        }

        Node ConditionStatement()
        {
            Node conditionStatement = new Node("Condition Statement");
            conditionStatement.Children.Add(Condition());
            conditionStatement.Children.Add(Conditions());
            return conditionStatement;
        }

        Node Conditions()
        {
            Node conditions = new Node("Conditions");
            //is boolean opeartion
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.OrOp || TokenStream[InputPointer].token_type == Token_Class.AndOp))
            {
                conditions.Children.Add(BooleanOp());
                conditions.Children.Add(Condition());
                conditions.Children.Add(Conditions());
                return conditions;
            }
            else
                return null;
        }

        Node BooleanOp()
        {
            Node boolOp = new Node("Boolean Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                boolOp.Children.Add(match(Token_Class.AndOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                boolOp.Children.Add(match(Token_Class.OrOp));
            }
            else
                printError("Boolean Operator");
            return boolOp;
        }

        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(ConditionOp());
            condition.Children.Add(Term());

            return condition;
        }
        Node ConditionOp()
        {
            Node condOp = new Node("Condition Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                condOp.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                condOp.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                condOp.Children.Add(match(Token_Class.EqualOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                condOp.Children.Add(match(Token_Class.NotEqualOp));
            }
            else
                printError("Condition Operator");
            return condOp;
        }

        Node FunctionCall()
        {
            Node funcCall = new Node("Function Call");
            funcCall.Children.Add(match(Token_Class.Idenifier));
            funcCall.Children.Add(match(Token_Class.LParanthesis));
            funcCall.Children.Add(Identifiers());
            funcCall.Children.Add(match(Token_Class.RParanthesis));
            return funcCall;
        }

        Node Identifiers()
        {
            Node ids = new Node("Identifiers");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                ids.Children.Add(match(Token_Class.Idenifier));
                ids.Children.Add(IdentifierD());
                return ids;
            }
            else
                return null;
        }

        Node IdentifierD()
        {
            Node idD = new Node("Identifier'");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                idD.Children.Add(match(Token_Class.Comma));
                idD.Children.Add(match(Token_Class.Idenifier));
                idD.Children.Add(IdentifierD());
                return idD;
            }
            else
                return null;
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
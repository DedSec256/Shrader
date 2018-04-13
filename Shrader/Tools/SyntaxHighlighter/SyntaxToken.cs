using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shrader.IDE.Tools.SyntaxHighlighter
{
	public enum SyntaxToken
	{
		EOF, 
		UNDEFINED, 
		PREPROCESSOR, 
		KEYWORD, 
		KEYWORD_FX,
		KEYWORD_SPECIAL,
		TYPE,
		IDENTIFIER,
		INTRINSIC,
		COMMENT_LINE,
		COMMENT,
		NUMBER,
		FLOAT,
		STRING_LITERAL,
		OPERATOR,
		DELIMITER,
		LEFT_BRACKET,
		RIGHT_BRACKET,
		LEFT_PARENTHESIS,
		RIGHT_PARENTHESIS,
		LEFT_SQUARE_BRACKET,
		RIGHT_SQUARE_BRACKET,
		UNITY_STRUCTURE,
		UNITY_TYPE,
		UNITY_VALUE,
		UNITY_FIXED
	}
}

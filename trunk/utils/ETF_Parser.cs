using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;


// This file is a part of great Engine "NeoAxis"!
// Copyright (C) 2006-2011 NeoAxis Group Ltd.
// Visit http://www.NeoAxis.com, to see it!



public class ETF_StringUtils

{

	// a..z->a..z

	// A..Z->A..Z

	// 0..9->0..9

	// #->#

	// _ -> _	

	//" " -> \p

	// .|,|?|'|"|!|@|#|$|%|^|&|*|(|)|-|+|>|<|~| "|" |{|}|[|]|/|\  ->  \(char)

	// \n -> \n

	// \t -> \t

	// other -> 

	// (int)(other)< 10 -> \a(other)

	// (int)(other)< 100 -> \b(other)

	// (int)(other)< 1000 -> \c(other)

	// (int)(other)< 10 000 -> \d(other)

	// (int)(other)>=10 000 -> \e(other)e

	

	public static string EncodeDelimiterFormatString(string Value) 

	{

        string result = "";

        for (int i = 0; i < Value.Length; i++)

        {

            if (Value[i] == '\"') result = result + "\\q";

            else

                if (Value[i] == '\n') result = result + "\\n";

                else

                    if (Value[i] == '\t') result = result + "\\t";

                    else

                        if (Value[i] == '\\') result = result + "\\\\";

                        else

                            result = result + Value[i];

        }

        return result;

	}

	

	public static string DecodeDelimiterFormatString(string Value)

    {

        string Result = "";

        for (int i = 0; i < Value.Length; i++)

        {

            if (Value[i] == '\\')

            {

                i++;

                switch (Value[i])

                {

                    case '\\': Result = Result + '\\'; break;

                    case 'n': Result = Result + '\n'; break;

                    case 't': Result = Result + '\t'; break;

                    case '\"': Result = Result + '\"'; break;

                    default: throw new Exception("Error of Encoding \\`" + Value[i] + "`."); 

                }

            }

            else

                Result = Result + Value[i];

        }

        return Result;

	}

}



	//namespace Engine.Utils
	//{
	/// <summary>
	/// The class allows to store the text information in the hierarchical form.
	/// Supports creation of children and attributes.
	/// </summary>
	/// 
	public sealed class ETF_TextBlock

	{

		ETF_TextBlock parent;

		string name;

		string data;

		TermType type;

	

		List<ETF_TextBlock> children = new List<ETF_TextBlock>();

		ReadOnlyCollection<ETF_TextBlock> childrenAsReadOnly;

		List<ETF_Attribute> attributes = new List<ETF_Attribute>();

		ReadOnlyCollection<ETF_Attribute> attributesAsReadOnly;



		//



		/// <summary>

		/// Defines <see cref="Engine.Utils.TextBlock"/> attribute.

		/// </summary>

		public sealed class ETF_Attribute

		{

			internal string name;

			internal string value;

			internal TermType type;



            internal ETF_Attribute() { }



			/// <summary>

			/// Gets the attribute name.

			/// </summary>

			public string Name

			{

				get { return name; }

			}



			/// <summary>

			/// Gets the attribute value.

			/// </summary>

			public string Value

			{

				get { return value; }

			}

		

			public TermType Type

			{

				get {return type;}

			}

	

			/// <summary>

			/// Returns a string that represents the current attribute.

			/// </summary>

			/// <returns>A string that represents the current attribute.</returns>

			public override string ToString()

			{

				return string.Format( "Name: \"{0}\", Value \"{1}\"", name, value );

			}

		}



		/// <summary>

		/// It is applied only to creation root blocks. Not for creation of children.

		/// </summary>

		/// <example>Example of creation of the block and filling by data.

		/// <code>

		/// TextBlock block = new TextBlock();

		/// TextBlock childBlock = block.AddChild( "childBlock", "child block data" );

		/// childBlock.SetAttribute( "attribute", "attribute value" );

		/// </code>

		/// </example>

		/// <seealso cref="Engine.Utils.TextBlock.AddChild(string,string)"/>

		/// <seealso cref="Engine.Utils.TextBlock.SetAttribute(string,string)"/>

		public ETF_TextBlock()

		{

			childrenAsReadOnly = new ReadOnlyCollection<ETF_TextBlock>( children );

			attributesAsReadOnly = new ReadOnlyCollection<ETF_Attribute>( attributes );

		}



		//Hierarchy



		/// <summary>

		/// Gets the parent block.

		/// </summary>

		public ETF_TextBlock Parent

		{

			get { return parent; }

		}



		/// <summary>

		/// Gets or set block name.

		/// </summary>

		public string Name

		{

			get { return name; }

			set

			{

				if( name == value )

					return;

				name = value;



				if( string.IsNullOrEmpty( name ) )

					//Log.Fatal( "TextBlock.set Name: \"name\" is null or empty." );

				throw new Exception( "TextBlock.set Name: \"name\" is null or empty." );

			}

		}

		

		public TermType Type

		{

			get {return type;}

			set {type=value;}

		}

	

		/// <summary>

		/// Gets or set block string data.

		/// </summary>

		public string Data

		{

			get { return data; }

			set { data = value; }

		}



		/// <summary>

		/// Gets the children collection.

		/// </summary>

		public IList<ETF_TextBlock> Children

		{

			get { return childrenAsReadOnly; }

		}



		/// <summary>

		/// Finds child block by name.

		/// </summary>

		/// <param name="name">The block name.</param>

		/// <returns><see cref="Engine.Utils.TextBlock"/> if the block has been exists; otherwise, <b>null</b>.</returns>

		public ETF_TextBlock FindChild( string name )

		{

			for( int n = 0; n < children.Count; n++ )

			{

				ETF_TextBlock child = children[ n ];

				if( child.Name == name )

					return child;

			}

			return null;

		}

	

		/// <summary>

		/// Creates the child block.

		/// </summary>

		/// <param name="name">The block name.</param>

		/// <param name="data">The block data string.</param>

		/// <param name="type">The block data type.</param>

		/// <returns>The child block.</returns>

		/// <remarks>

		/// Names of blocks can repeat.

		/// </remarks>

		public ETF_TextBlock AddChild( string name, string data, TermType type )

		{

			if( string.IsNullOrEmpty( name ) )

				//Log.Fatal( "TextBlock.AddChild: \"name\" is null or empty." );

			throw new Exception( "TextBlock.AddChild: \"name\" is null or empty." );



			ETF_TextBlock child = new ETF_TextBlock();

			child.parent = this;

			child.name = name;

			child.data = data;

			child.type=type;

			children.Add( child );

			return child;

		}



		/// <summary>

		/// Creates the child block.

		/// </summary>

		/// <param name="name">The block name.</param>

		/// <param name="data">The block data string.</param>

		/// <returns>The child block.</returns>

		/// <remarks>

		/// Names of blocks can repeat.

		/// </remarks>

		public ETF_TextBlock AddChild( string name, string data )

		{

			if( string.IsNullOrEmpty( name ) )

				//Log.Fatal( "TextBlock.AddChild: \"name\" is null or empty." );

			throw new Exception( "TextBlock.AddChild: \"name\" is null or empty." );



			ETF_TextBlock child = new ETF_TextBlock();

			child.parent = this;

			child.name = name;

			child.data = data;

			child.type=TermType.STRING;

			children.Add( child );

			return child;

		}



		/// <summary>

		/// Creates the child block.

		/// </summary>

		/// <param name="name">The block name.</param>

		/// <returns>The child block.</returns>

		/// <remarks>

		/// Names of blocks can repeat.

		/// </remarks>

		public ETF_TextBlock AddChild( string name )

		{

			return AddChild( name, "",TermType.STRING );

		}



		/// <summary>

		/// Deletes child block.

		/// </summary>

		/// <param name="child">The child block.</param>

		public void DeleteChild( ETF_TextBlock child )

		{

			children.Remove( child );

			child.parent = null;

			child.name = "";

			child.data = "";

			child.children = null;

			child.attributes = null;

		}



		/// <summary>

		/// Returns the attribute value by name.

		/// </summary>

		/// <param name="name">The attribute name.</param>

		/// <param name="defaultValue">Default value. If the attribute does not exist that this value will return.</param>

		/// <returns>The attribute value if the attribute exists; otherwise, default value.</returns>

		public string GetAttribute( string name, string defaultValue )

		{

			for( int n = 0; n < attributes.Count; n++ )

			{

				ETF_Attribute attribute = attributes[ n ];

				if( attribute.Name == name )

					return attribute.Value;

			}

			return defaultValue;

		}



		/// <summary>

		/// Returns the attribute value by name.

		/// </summary>

		/// <param name="name">The attribute name.</param>

		/// <returns>The attribute value if the attribute exists; otherwise, empty string.</returns>

		public string GetAttribute( string name )

		{

			return GetAttribute( name, "" );

		}



		/// <summary>

		/// Gets the attributes collection.

		/// </summary>

		public IList<ETF_Attribute> Attributes

		{

			get { return attributesAsReadOnly; }

		}



		/// <summary>

		/// Checks existence of attribute.

		/// </summary>

		/// <param name="name">The attribute name.</param>

		/// <returns><b>true</b> if the block exists; otherwise, <b>false</b>.</returns>

		public bool IsAttributeExist( string name )

		{

			for( int n = 0; n < attributes.Count; n++ )

			{

				ETF_Attribute attribute = attributes[ n ];

				if( attribute.Name == name )

					return true;

			}

			return false;

		}



		/// <summary>

		/// Sets attribute. If such already there is that rewrites him.

		/// </summary>

		/// <param name="name">The attribute name.</param>

		/// <param name="value">The attribute value.</param>

		public void SetAttribute( string name, string value )

		{

			if( string.IsNullOrEmpty( name ) )

				//Log.Fatal( "TextBlock.AddChild: \"name\" is null or empty." );

			throw new Exception( "TextBlock.AddChild: \"name\" is null or empty." );

				

			if( value == null )

				//Log.Fatal( "TextBlock.AddChild: \"value\" is null." );

			throw new Exception( "TextBlock.AddChild: \"value\" is null." );



			for( int n = 0; n < attributes.Count; n++ )

			{

				ETF_Attribute attribute = attributes[ n ];

				if( attribute.Name == name )

				{

					attribute.value = value;
                    attribute.type = TermType.STRING;
					return;

				}

			}

            ETF_Attribute a = new ETF_Attribute();

			a.name = name;

			a.value = value;
            a.type = TermType.STRING;
			attributes.Add( a );

		}

	

		/// <summary>

		/// Sets attribute. If such already there is that rewrites him.

		/// </summary>

		/// <param name="name">The attribute name.</param>

		/// <param name="value">The attribute value.</param>

		/// <param name="type">The attribute value type.</param>

		public void SetAttribute( string name, string value,TermType type)

		{

			if( string.IsNullOrEmpty( name ) )

				//Log.Fatal( "TextBlock.AddChild: \"name\" is null or empty." );

			throw new Exception( "TextBlock.AddChild: \"name\" is null or empty." );

				

			if( value == null )

				//Log.Fatal( "TextBlock.AddChild: \"value\" is null." );

			throw new Exception( "TextBlock.AddChild: \"value\" is null." );



			for( int n = 0; n < attributes.Count; n++ )

			{

				ETF_Attribute attribute = attributes[ n ];

				if( attribute.Name == name )

				{

					attribute.value = value;

					attribute.type=type;

					return;

				}

			}

            ETF_Attribute a = new ETF_Attribute();

			a.name = name;

			a.value = value;

			a.type=type;

			attributes.Add( a );

		}

	

		/// <summary>

		/// Change type of existing attribute.

		/// if Attribute not exist, do nothinc

		/// </summary>

		/// <param name="name">The attribute name.</param>

		/// <param name="type">The attribute value type.</param>

		/// <returns>false if Attribute not exist.</returns>

		public bool TryChangeAttributeType( string name, TermType newType)

		{

			if( string.IsNullOrEmpty( name ) )

				//Log.Fatal( "TextBlock.AddChild: \"name\" is null or empty." );

			throw new Exception( "TextBlock.AddChild: \"name\" is null or empty." );

				

			for( int n = 0; n < attributes.Count; n++ )

			{

				ETF_Attribute attribute = attributes[ n ];

				if( attribute.Name == name )

				{

					attribute.type=newType;

					return true;

				}

			}

		

			return false;

		}

	



		/// <summary>

		/// Deletes attribute if he exists.

		/// </summary>

		/// <param name="name">The attribute name.</param>

		public void DeleteAttribute( string name )

		{

			for( int n = 0; n < attributes.Count; n++ )

			{

				if( name == attributes[ n ].name )

				{

					ETF_Attribute attribute = attributes[ n ];

					attribute.name = "";

					attribute.value = "";

					attributes.RemoveAt( n );

					return;

				}

			}

		}



		/// <summary>

		/// Deletes all attributes.

		/// </summary>

		public void DeleteAllAttributes()

		{

			attributes.Clear();

		}



		static string TabLevelToString( int level )

		{

			string str = "";

			for( int n = 0; n < level; n++ )

				str += "\t";

			return str;

		}



		static bool IsNeedQuotesForLexeme( string text, bool thisIsAttributeValue )

		{

			if( !thisIsAttributeValue )

			{

				foreach( char c in text )

				{

					bool good = ( c >= 'A' && c <= 'Z' ) || ( c >= 'a' && c <= 'z' ) ||

						( c >= '0' && c <= '9' ) || c == '_' || c == '#' || c == '$';

					if( !good )

						return true;

				}

				return false;

			}

			else

			{

				if( text.Length > 0 )

				{

					if( text[ 0 ] == ' ' || text[ text.Length - 1 ] == ' ' )

						return true;

				}



				foreach( char c in text )

				{

					bool good = ( c >= 'A' && c <= 'Z' ) || ( c >= 'a' && c <= 'z' ) ||

						( c >= '0' && c <= '9' ) || c == '_' || c == '#' || c == '$' || c == '.' ||

						c == ',' || c == '-' || c == '!' || c == '%' || c == '&' || c == '(' ||

						c == ')' || c == '*' || c == '+' || c == '?' || c == '[' || c == ']' ||

						c == '^' || c == '|' || c == '~' || c == ' ';



					if( !good )

						return true;

				}



				return false;

			}

		}



		void DumpToString( StringBuilder builder, int tabLevel )

		{

			string tabPrefix = TabLevelToString( tabLevel );



			if( !string.IsNullOrEmpty( Name ) )

			{

				{

					builder.Append( tabPrefix );



					string v;

					if( IsNeedQuotesForLexeme( Name, false ) )

                        v = string.Format("\"{0}\"", ETF_StringUtils.EncodeDelimiterFormatString(Name));

					else

						v = Name;

					builder.Append( v );

				}



				if( !string.IsNullOrEmpty( Data ) )

				{

					builder.Append( " " );



					string v;

					if( IsNeedQuotesForLexeme( Data, false ) )

                        v = string.Format("\"{0}\"", ETF_StringUtils.EncodeDelimiterFormatString(Data));

					else

						v = Data;

					builder.Append( v );

				}



				builder.Append( "\r\n" );

				builder.Append( tabPrefix );

				builder.Append( "{\r\n" );

			}



			foreach( ETF_Attribute attribute in attributes )

			{

				string name;

				string value;



				if( IsNeedQuotesForLexeme( attribute.Name, false ) )

				{

					name = string.Format( "\"{0}\"",

                        ETF_StringUtils.EncodeDelimiterFormatString(attribute.Name));

				}

				else

					name = attribute.Name;



				if( IsNeedQuotesForLexeme( attribute.Value, true ) )

				{

					value = string.Format( "\"{0}\"",

                        ETF_StringUtils.EncodeDelimiterFormatString(attribute.Value));

				}

				else

					value = attribute.Value;



				builder.Append( tabPrefix );

				builder.Append( tabLevel != -1 ? "\t" : "" );

				if (attribute.Type!=TermType.STRING)

					builder.AppendFormat( "{0} = {1} {2}\r\n", name,TermTypeToString(attribute.Type), value );

				else

					builder.AppendFormat( "{0} = {1}\r\n", name, value );

			}



			foreach( ETF_TextBlock child in children )

				child.DumpToString( builder, tabLevel + 1 );



			if( !string.IsNullOrEmpty( Name ) )

			{

				builder.Append( tabPrefix );

				builder.Append( "}\r\n" );

			}

		}

		

		/// <summary>

		/// Returns a string containing TermType like "@T", where T-char with typeID.

		/// </summary>

		public static string TermTypeToString(TermType tType)

		{

			string result="@";

			switch(tType)

			{

				case TermType.ATOM: result+="A";break;

				case TermType.SMALL_ATOM: result+="a";break;

				case TermType.INTEGER: result+="i";break;

				case TermType.SMALL_INTEGER: result+="b";break;

				case TermType.STRING: result+="s";break;

				case TermType.NEW_FLOAT: result+="f";break;

				case TermType.BINARY: result+="d";break;

				case TermType.NIL: result+="n";break;

				case TermType.LARGE_TUPLE: result+="T";break;

                case TermType.LIST: result += "L"; break;

				case TermType.SMALL_TUPLE: result+="t";break;				

				default: result+="u";break;

			}

			return result;

		}

	

		/// <summary>

		/// Returns a TermType from string like "@T", where T-char with typeID.

		/// 

		/// </summary>

		public static TermType StringToTermType( string tType,bool returnString)

		{

			if (tType.Length!=2 || tType[0]!='@')

				if (returnString)

					return TermType.ERLANG_BINARY;

				else

					return TermType.ERROR;

		

			switch(tType[1])

			{

				case 'A':return TermType.ATOM;

				case 'a':return TermType.SMALL_ATOM;

				case 'i':return TermType.INTEGER;

				case 'b':return TermType.SMALL_INTEGER;

				case 's':return TermType.STRING;

				case 'f':return TermType.NEW_FLOAT;

				case 'd':return TermType.BINARY;

				case 'e':return TermType.NIL;

				case 'T':return TermType.LARGE_TUPLE;

                case 'L': return TermType.LIST;

				case 't':return TermType.SMALL_TUPLE;

				default: return TermType.ERROR;

			}
		}



		/// <summary>

		/// Returns a string containing all data about the block and his children.

		/// </summary>

		/// <returns>A string containing all data about the block and his children.</returns>

		/// <remarks>

		/// This method is applied at preservation of data of the block in a file.

		/// </remarks>

		/// <example>Example of preservation of data of the block in a file.

		/// <code>

		/// TextBlock block = ...

		/// StreamWriter writer = new StreamWriter( fileName );

		/// writer.Write( block.DumpToString() );

		/// writer.Close();

		/// </code>

		/// </example>

		/// <seealso cref="Engine.Utils.TextBlock.Parse(string,out string)"/>

		public string DumpToString()

		{

			StringBuilder builder = new StringBuilder();

			DumpToString( builder, -1 );

			return builder.ToString();

		}



		/// <summary>

		/// Returns a string that represents the current text block.

		/// </summary>

		/// <returns>A string that represents the current text block.</returns>

		public override string ToString()

		{

			string text = string.Format( "Name: \"{0}\"", name );

			if( !string.IsNullOrEmpty( data ) )

				text += string.Format( ", Data: \"{0}\"", data );

			return text;

		}

	

		/// <summary>

		/// Parses the text with data of the block and his children.

		/// </summary>

		/// <param name="str">The data string.</param>

		/// <param name="errorString">The information on an error.</param>

		/// <returns><see cref="Engine.Utils.TextBlock"/> if the block has been parsed; otherwise, <b>null</b>.</returns>

		/// <seealso cref="Engine.Utils.TextBlock.DumpToString()"/>

		/// <remarks>

		/// For convenience of loading of blocks there is auxiliary class <see cref="Engine.Utils.TextBlockUtils"/>.

		/// </remarks>

		/// <example>Example of loading of data of the block from a stream.

		/// <code>

		/// FileStream stream = ...;

		/// StreamReader streamReader = new StreamReader( stream );

		/// string error;

		/// TextBlock block = TextBlock.Parse( streamReader.ReadToEnd(), out error );

		/// streamReader.Dispose();

		/// </code>

		/// </example>

		public static ETF_TextBlock Parse( string str, out string errorString )

		{

			return ETF_TextBlockParser.Parse( str, out errorString );

		}

	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	static class ETF_TextBlockParser

	{

		static string streamString;

		static int streamPosition;

		static string error;

		static int linePosition;

		static ETF_TextBlock root;



		static bool StreamEOF

		{

			get { return streamPosition >= streamString.Length; }

		}



		static bool StreamReadChar( out char character )

		{

			if( StreamEOF )

			{

				character = (char)0;

				return false;

			}

			character = streamString[ streamPosition ];

			streamPosition++;

			return true;

		}



		static void StreamSeek( int position )

		{

			streamPosition = position;

		}



		static void Error( string str )

		{

			if( error == null )

				error = string.Format( "{0} (line - {1})", str, linePosition );

		}



		static StringBuilder tempStringBuilder = new StringBuilder();



		static string GetLexeme( bool stopOnlyAtSeparatorOrQuotes, out bool intoQuotes )

		{

			intoQuotes = false;



			StringBuilder lex = tempStringBuilder;

			lex.Length = 0;



			while( true )

			{

				char c;

				if( !StreamReadChar( out c ) )

				{

					if( StreamEOF )

						return lex.ToString().Trim();

					Error( "Unexpected end of file" );

					return "";

				}



				//comments

				if( c == '/' )

				{

					char cc;

					if( !StreamReadChar( out cc ) )

					{

						Error( "Unexpected end of file" );

						return "";

					}



					if( cc == '/' )

					{

						while( true )

						{

							if( !StreamReadChar( out c ) )

							{

								if( StreamEOF )

								{

									c = '\n';

									break;

								}

								Error( "Unexpected end of file" );

								return "";

							}

							if( c == '\n' )

								break;

						}

					}

					else if( cc == '*' )

					{

						char oldChar = (char)0;



						while( true )

						{

							if( !StreamReadChar( out c ) )

							{

								if( StreamEOF )

								{

									c = ';';

									break;

								}

								Error( "Unexpected end of file" );

								return "";

							}



							if( c == '\n' )

								linePosition++;



							if( oldChar == '*' && c == '/' )

							{

								c = ';';

								break;

							}



							oldChar = c;

						}

					}

					else

					{

						StreamSeek( streamPosition - 1 );

					}

				}



				if( c == '\n' )

					linePosition++;



				if( c == '=' || c == '{' || c == '}' )

				{

					if( lex.Length != 0 )

					{

						StreamSeek( streamPosition - 1 );

						return lex.ToString().Trim();

					}

					return c.ToString();

				}

			

				//////////////////////////////////////

				if (c=='@')

				{

					

					if( lex.Length != 0 )

					{

						StreamSeek( streamPosition - 1 );

						return lex.ToString().Trim();

					}

				

					char cc;

					if( !StreamReadChar( out cc ) )

					{

						Error( "Unexpected end of file" );

						return "";

					}

				

					return c.ToString()+cc.ToString();

				}

				//////////////////////////////////////



				if( ( !stopOnlyAtSeparatorOrQuotes && ( c <= 32 || c == ';' ) ) ||

					( stopOnlyAtSeparatorOrQuotes && ( c == '\n' || c == '\r' || c == ';' ) ) )

				{

					if( lex.Length != 0 || stopOnlyAtSeparatorOrQuotes )

						return lex.ToString().Trim();

					continue;

				}



				if( c == '"' )

				{

					if( lex.Length != 0 )

					{

						StreamSeek( streamPosition - 1 );

						return lex.ToString().Trim();

					}



					//quotes

					while( true )

					{

						if( !StreamReadChar( out c ) )

						{

							Error( "Unexpected end of file" );

							return "";

						}

						if( c == '\n' )

							linePosition++;

                        /*

						if( c == '\\' )

						{

							char c2;

							if( !StreamReadChar( out c2 ) )

							{

								Error( "Unexpected end of file" );

								return "";

							}



							string ss = "\\" + c2;

							if( c2 == 'x' )

							{

								for( int z = 0; z < 4; z++ )

								{

									if( !StreamReadChar( out c2 ) )

									{

										Error( "Unexpected end of file" );

										return "";

									}

									ss += c2;

								}

							}

							//StringUtils.DecodeDelimiterFormatString( lex, ss );

							lex.Append( StringUtils.DecodeDelimiterFormatString( ss ) );

							continue;

						}*/



						if( c == '"' )

						{

							intoQuotes = true;

                            return ETF_StringUtils.DecodeDelimiterFormatString(lex.ToString());

						}

						lex.Append( c );

					}



				}



				if( lex.Length == 0 && ( c == ' ' || c == '\t' ) )

					continue;



				lex.Append( c );

			}

		}



		static string GetLexeme( bool stopOnlyAtSeparatorOrQuotes )

		{

			bool intoQuotes;

			return GetLexeme( stopOnlyAtSeparatorOrQuotes, out intoQuotes );

		}



		static bool LoadChild( ETF_TextBlock child, bool ifEmptyLexReturnTrue )

		{

			while( true )

			{

				bool lexIntoQuotes;

				string lex = GetLexeme( false, out lexIntoQuotes );

				if( lex.Length == 0 )//if( lex == "" )

				{

					if( ifEmptyLexReturnTrue )

						return true;



					Error( "Unexpected end of file" );

					return false;

				}



				if( lex == "}" )

					return true;



				string lex2 = GetLexeme( false );

				if( lex2.Length == 0 )//if( lex2 == "" )

				{

					Error( "Unexpected end of file" );

					return false;

				}



				if( lex2 == "=" )

				{

					string s_type = GetLexeme( true );

					TermType t_type=ETF_TextBlock.StringToTermType(s_type,true);					

					if (t_type==TermType.ERROR)

					{

						Error( "Invalid type description" );

						return false;

					}

					if (t_type==TermType.ERLANG_BINARY)

						child.SetAttribute( lex, s_type,TermType.STRING );

					else

					{

						

						string value = GetLexeme( true );

						child.SetAttribute( lex, value,t_type );

					}

					continue;

				}



				if( lex2 == "{" )

				{

                    ETF_TextBlock c = child.AddChild(lex);

					if( !LoadChild( c, false ) )

						return false;

					continue;

				}



				string lex3 = GetLexeme( false );

				if( lex3.Length == 0 )//if( lex3 == "" )

				{

					Error( "Unexpected end of file" );

					return false;

				}



				if( lex3 == "{" )

				{

                    ETF_TextBlock c = child.AddChild(lex, lex2);

					if( !LoadChild( c, false ) )

						return false;

					continue;

				}



				Error( "Invalid file format" );

				return false;

			}

		}



		public static ETF_TextBlock Parse( string str, out string errorString )

		{

			if( str == null )

				//Log.Fatal( "TextBlock.Parse: \"str\" is null." );

			throw new Exception( "TextBlock.Parse: \"str\" is null." );



			streamString = str;

			streamPosition = 0;

			error = null;

			linePosition = 1;

			root = new ETF_TextBlock();



			bool ret = LoadChild( root, true );

			if( !ret )

			{

				errorString = error;

				return null;

			}

			errorString = "";

			return root;

		}



}



/* Следующийе код недоделка для обмена  именоваными данными. 

 * т.е. каждому Свойству класса приписывается имя, таким образом

 * положение св-ва не имеет значения, ножно лишь знать его имя.

public class ETFClass

{

	public string Name;

	public Dictionary<string,ETFProperty> Prperties;

	

	public ETFProperty CreateProperty(string PropertyName)

	{

		ETFProperty p=new ETFProperty(PropertyName);

		Prperties.Add(PropertyName,p);

		return p;

	}

	

	public void SetProperty(string PropertyName,)

	

}



public class ETFProperty

{

	public string Name;

	public ETFValue Value;

	

	public ETFProperty(string name)

	{

		Name=name;

		Value=new ETFValue();

	}

}



public class ETFStruct

{

	public string Name;

	public List<ETFValue> Values;

}



public class ETFValue

{

	int IntVal;

	float FloatVal;

	Object ObjVal;

	

	public TermType ValueType;

	public byte AsSMALL_INTEGER

	{

		get {return (byte)IntVal;}

		set {IntVal=value;}

	}

	public Int32 AsINTEGER

	{

		get {return IntVal;}

		set {IntVal=value;}		

	}

	public string AsATOM

	{

		get {return ObjVal;}

		set {ObjVal=value;}

	}

	public List<Object> AsObjectList

	{

		get {return ObjVal;}

		set {ObjVal=value;}

	}

	

	//SMALL_TUPLE=104,

	//LARGE_TUPLE=105,

	public string AsSTRING

	{

		get {return ObjVal;}

		set {ObjVal=value;}

	}

	

	public byte[] AsBINARY

	{

		get {return ObjVal;}

		set {ObjVal=value;}

	}

	public float AsFloat

	{

		get {return FloatVal;}

		set {FloatVal=value;}

	}

}

*/

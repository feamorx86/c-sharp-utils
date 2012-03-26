using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;


public enum TermType
{
	ERLANG_BINARY=131,//this is not term
	SMALL_INTEGER=97,
	INTEGER=98,
	ATOM=100,
	SMALL_TUPLE=104,
	LARGE_TUPLE=105,
	NIL=106,
	STRING=107,
	LIST=108,
	BINARY=109,
	SMALL_ATOM=115,
	NEW_FLOAT=70,
	

	UNKNOWN=254,
	ERROR=255
}

public enum BtoT_Result
{
	Ok=0,
	InputDataEmpty,
	NoErlangBinary,		//ERLANG_BINARY=131
	ErroredErlangBinary	//ERLANG_BINARY=131
}

public class Term
{
	public int IntVal;
	public string StrVal;

	public List<Term> TupleVal;
	public byte [] DataVal;
	public double FloatVal; 

	TermType TypeOfTerm;

	public TermType GetTermType()
	{
		return TypeOfTerm;	
	}

	public Term(TermType typeofterm)
	{
		TypeOfTerm=typeofterm;
		switch(typeofterm)
		{
			case TermType.ATOM: StrVal="";break;
			case TermType.SMALL_ATOM: StrVal="";break;
			case TermType.STRING: StrVal="";break;

			case TermType.INTEGER: IntVal=0;break;
			case TermType.SMALL_INTEGER: IntVal=0;break;

			case TermType.LARGE_TUPLE: TupleVal=new List<Term>(); IntVal=0; break;
            case TermType.LIST: TupleVal = new List<Term>(); IntVal = 0; break;
			case TermType.SMALL_TUPLE: TupleVal=new List<Term>(); IntVal=0; break;

			case TermType.NEW_FLOAT: FloatVal=0.0f;break;
			//Default: do nothinc))
		}		
	}
	

	#region Static Constructors

	public static Term Create_SMALL_INTEGER(byte Value)

	{

		Term t=new Term(TermType.SMALL_INTEGER);

		t.IntVal=Value;

		return t;

	}

	public static Term Create_INTEGER(Int32 Value)

	{

		Term t=new Term(TermType.INTEGER);

		t.IntVal=Value;

		return t;

	}

	public static Term Create_ATOM(string Name)

	{

		Term t=new Term(TermType.ATOM);

		t.StrVal=Name;

		return t;

	}

	public static Term Create_SMALL_TUPLE()

	{

		return new Term(TermType.SMALL_TUPLE);

	}

	public static Term CreateCopy_SMALL_TUPLE(List<Term> terms)

	{

		Term t=new Term(TermType.SMALL_TUPLE);

		t.TupleVal.AddRange(terms);

		return t;

	}


	public static Term Create_LARGE_TUPLE()

	{

		return new Term(TermType.LARGE_TUPLE);

	}

	public static Term CreateCopy_LARGE_TUPLE(List<Term> terms)

	{

		Term t=new Term(TermType.LARGE_TUPLE);

		t.TupleVal.AddRange(terms);

		return t;

	}

    public static Term Create_LIST()
    {

        return new Term(TermType.LIST);

    }

    public static Term CreateCopy_LIST(List<Term> terms)
    {

        Term t = new Term(TermType.LIST);

        t.TupleVal.AddRange(terms);

        return t;

    }

	public static Term Create_NIL()

	{

		return new Term(TermType.NIL);

	}

	public static Term Create_STRING(String Value)

	{

		Term t=new Term(TermType.STRING);

		t.StrVal=Value;

		return t;

	}

	//LIST=108, !! Not Used !!

	public static Term Create_BINARY(byte[] Value)

	{

		Term t=new Term(TermType.BINARY);

		t.DataVal=Value;

		return t;

	}

	public static Term Create_BINARY()

	{

		return new Term(TermType.BINARY);

	}

    public static Term CreateCopy_BINARY(byte[] Value)

	{

		Term t=new Term(TermType.BINARY);

		Array.Copy(Value,t.DataVal,Value.Length);

		return t;

	}

	public static Term Create_SMALL_ATOM(String Value)

	{

		Term t=new Term(TermType.SMALL_ATOM);

		t.StrVal=Value;

		return t;

	}

	public static Term Create_NEW_FLOAT(double Value)

	{

		Term t=new Term(TermType.NEW_FLOAT);

		t.FloatVal=Value;

		return t;

	}

	#endregion

	#region Add Term Value

	

	public void PushTerm(Term term)

	{

		TupleVal.Add(term);

	}

	

	public Term PopTerm()

	{

		Term t;

		if (TupleVal.Count>0)

		{

			t=TupleVal[TupleVal.Count-1];

			TupleVal.RemoveAt(TupleVal.Count-1);

			return t;

		}

		else

			return null;

	}

	

	public Term GetTerm(int Index)

	{

		if (Index<TupleVal.Count)

			return TupleVal[Index];

		return null;

	}

	

	public void AddByte(byte Value)

	{

		 TupleVal.Add(Term.Create_SMALL_INTEGER(Value));		

	}

	

	public void AddInt(int Value)

	{

		TupleVal.Add(Term.Create_INTEGER(Value));

	}


	public void AddAtom(string Name)

	{

		TupleVal.Add(Term.Create_ATOM(Name));

	}

	

	public void AddFloat(float Value)

	{

		TupleVal.Add(Term.Create_NEW_FLOAT(Value));

	}

	

	public void AddString(String Value)

	{

		TupleVal.Add(Term.Create_STRING(Value));

	}

	public void AddData(byte[] Value)

	{

		TupleVal.Add(Term.Create_BINARY(Value));

	}

	

	public void AddNull()

	{

		TupleVal.Add(Term.Create_NIL());

	}

	

	public void AddSmallAtom(string Value)

	{

		TupleVal.Add(Term.Create_SMALL_ATOM(Value));

	}

	

	public Term AddSmallTuple()

	{

		Term t=Term.Create_SMALL_TUPLE();

		TupleVal.Add(t);

		return t;

	}

	

	public Term AddTuple()

	{

		Term t=Term.Create_LARGE_TUPLE();

		TupleVal.Add(t);

		return t;

	}

    public Term AddList()
    {

        Term t = Term.Create_LIST();

        TupleVal.Add(t);

        return t;

    }	

	#endregion
	
	public int GetInt(int index)
	{
		return TupleVal[index].IntVal;
	}
	
	public string GetStr(int index)
	{
		return TupleVal[index].StrVal;
	}
	
	public float GetFloat(int index)
	{
		return (float)TupleVal[index].FloatVal;
	}

    public double GetDouble(int index)
    {
        return TupleVal[index].FloatVal;
    }
	
	public byte [] GetData(int index)
	{
		return TupleVal[index].DataVal;
	}
	
	public override string ToString ()
	{
		string data;
		switch(TypeOfTerm)
		{
			case TermType.SMALL_INTEGER: return IntVal.ToString(); 
			case TermType.INTEGER: return IntVal.ToString();
			case TermType.ATOM: return StrVal; 
			case TermType.SMALL_TUPLE:
				data="{ ";
				for (int i=0;i<TupleVal.Count-1;i++)
				{
					data=data+TupleVal[i].ToString()+", ";
				}
				if (TupleVal.Count>0)
					return data+TupleVal[TupleVal.Count-1].ToString()+" }";
				return data+"}";		
			
			case TermType.LARGE_TUPLE:
			data="{ ";
				for (int i=0;i<TupleVal.Count-1;i++)
				{
					data=data+TupleVal[i].ToString()+", ";
				}
				if (TupleVal.Count>0)
					return data+TupleVal[TupleVal.Count-1].ToString()+" }";
				return data+"}";

            case TermType.LIST:
                data = "[ ";
                for (int i = 0; i < TupleVal.Count - 1; i++)
                {
                    data = data + TupleVal[i].ToString() + ", ";
                }
                if (TupleVal.Count > 0)
                    return data + TupleVal[TupleVal.Count - 1].ToString() + " ]";
                return data + "]";		
			
			case TermType.NIL: return "NIL";
			case TermType.STRING: return StrVal;
			case TermType.BINARY:
				if (DataVal.Length==0)
					return "[Empty Data]";
				data="[data ("+DataVal.Length+") :";
				for (int i=0;i<DataVal.Length-1;i++)
					data=data+DataVal[i].ToString()+", ";
				return data+DataVal[DataVal.Length-1]+"]";
			case TermType.SMALL_ATOM:return StrVal;
			case TermType.NEW_FLOAT: return FloatVal.ToString();
            default: return "Term";
		}
	}

	public static int IncDataSize=256;

	#region binary and term

	byte [] IncDataIfNeed(byte [] Data,int NeedBytes)
	{

		if (NeedBytes>=Data.Length)

		{

			//can just inc?

			if (NeedBytes>=Data.Length+IncDataSize)

			{

				//no 

				int incs=NeedBytes-Data.Length/NeedBytes+1;

				byte []newData=new byte[Data.Length+IncDataSize*incs];

				Array.Copy(Data,newData,Data.Length);

				return newData;

			}

			else

			{

				byte []newData=new byte[Data.Length+IncDataSize];

				Array.Copy(Data,newData,Data.Length);

				return newData;

			}

		}

		return Data;

	}

	public byte [] term_to_binary(ref byte []Data, ref int Index)

	{

		byte []val;

		byte []val2;

		byte []newData;

		switch(TypeOfTerm)

		{

			case TermType.SMALL_INTEGER: //(byte)97, (byte)Int

				Index+=2;

				newData=IncDataIfNeed(Data,Index);

                newData[Index - 2] = (byte)TermType.SMALL_INTEGER;

                newData[Index - 1] = (byte)IntVal;

			break;

			case TermType.INTEGER:	// (byte)98, (Int32)Int

				Index+=5;

                newData = IncDataIfNeed(Data, Index);				

				val=BitConverter.GetBytes( ((Int32)IntVal) );



                newData[Index - 5] = (byte)TermType.INTEGER;

				newData[Index-4]=val[3];	newData[Index-3]=val[2];	newData[Index-2]=val[1];	newData[Index -1 ]=val[0];

			break;

			case TermType.ATOM: // (byte)100, (UInt16) Len, byte[Len] AtomName

				val=Encoding.ASCII.GetBytes(StrVal);



                newData = IncDataIfNeed(Data, Index + val.Length + 3);				

				val2=BitConverter.GetBytes(((UInt16)val.Length));

			

				newData[Index]=(byte)TermType.ATOM;

				newData[Index+1]=val2[1];	newData[Index+2]=val2[0];

				Index+=3;

				

				Array.Copy(val,0,newData,Index,val.Length);

				Index+=val.Length;

			break;

			case TermType.SMALL_TUPLE://(byte)104, (byte)Arity, Type[Arity]

                newData = IncDataIfNeed(Data, Index + 2);

				newData[Index]=(byte)TermType.SMALL_TUPLE;				

				newData[Index+1]=(byte)TupleVal.Count;

				Index+=2;				

				foreach(Term t in TupleVal)

					newData=t.term_to_binary(ref newData,ref Index);

			break;

			case TermType.LARGE_TUPLE://(byte)105, (UInt32)Arity, Type[Arity]

            newData = IncDataIfNeed(Data, Index + 5);

				val=BitConverter.GetBytes(((UInt32)TupleVal.Count));

				newData[Index]=(byte)TermType.LARGE_TUPLE;				

				newData[Index+1]=val[3];newData[Index+2]=val[2];newData[Index+3]=val[1];newData[Index+4]=val[0];

				Index+=5;				

				foreach(Term t in TupleVal)

					newData=t.term_to_binary(ref newData,ref Index);		

			break;

            case TermType.LIST://(byte)108, (UInt32)Arity, Type[Arity], NIL

                //Termtype and List Length
                newData = IncDataIfNeed(Data, Index + 5);

                val = BitConverter.GetBytes(((UInt32)TupleVal.Count));

                newData[Index] = (byte)TermType.LIST;

                newData[Index + 1] = val[3]; newData[Index + 2] = val[2]; newData[Index + 3] = val[1]; newData[Index + 4] = val[0];

                Index += 5;

                foreach (Term t in TupleVal)

                    newData = t.term_to_binary(ref newData, ref Index);
                //NIL

                newData = IncDataIfNeed(Data, Index+1);

                newData[Index] = (byte)TermType.NIL;
                Index += 1;

            break;

			case TermType.NIL:  //(byte)106 

                newData = IncDataIfNeed(Data, Index+1);

                newData[Index] = (byte)TermType.NIL;
                Index += 1;

			break;

			case TermType.STRING://(byte)107, (UInt16)Length, Characters[Length]

				val=Encoding.Unicode.GetBytes(StrVal);



                newData = IncDataIfNeed(Data, Index + val.Length + 3);				

				val2=BitConverter.GetBytes(((UInt16)val.Length));

			

				newData[Index]=(byte)TermType.STRING;

				newData[Index+1]=val2[1];	newData[Index+2]=val2[0];

				Index+=3;

				

				Array.Copy(val,0,newData,Index,val.Length);

				Index+=val.Length;

			break;

			//case TermType.//LIST=108, !! Not Used !!

			case TermType.BINARY://(byte)109, (UInt32) Len, byte[Len]				

            newData = IncDataIfNeed(Data, Index + DataVal.Length + 5);

				

				val=BitConverter.GetBytes(((UInt32)DataVal.Length));

				newData[Index]=(byte)TermType.BINARY;				

				newData[Index+1]=val[3];newData[Index+2]=val[2];newData[Index+3]=val[1];newData[Index+4]=val[0];

				Index+=5;				

				Array.Copy(DataVal,0,newData,Index,DataVal.Length);

				Index+=DataVal.Length;

			break;

			case TermType.SMALL_ATOM://(byte)115, (byte) Len, byte[Len] AtomName

                val = Encoding.ASCII.GetBytes(StrVal);

                newData = IncDataIfNeed(Data, Index + val.Length + 2);

			

				newData[Index]=(byte)TermType.SMALL_ATOM;

				newData[Index+1]=(byte)val.Length;

				Index+=2;

				

				Array.Copy(val,0,newData,Index,val.Length);

				Index+=val.Length;

			break;

			case TermType.NEW_FLOAT: //(byte)70, float64(double)

				Index+=9;

                newData = IncDataIfNeed(Data, Index);				

				val=BitConverter.GetBytes( FloatVal);



                newData[Index - 9] = (byte)TermType.NEW_FLOAT;

				newData[Index-8]=val[0];	newData[Index-7]=val[1];	newData[Index-6]=val[2];	newData[Index-5]=val[3];

				newData[Index-4]=val[4];	newData[Index-3]=val[5];	newData[Index-2]=val[6];	newData[Index-1]=val[7];

			break;

            default: newData = Data; break;

		}

		return newData;

	}

	public static byte[] term_to_binary(Term term)

	{

		byte [] Data=new byte[IncDataSize];

		int Index=1;

		

		Data[0]=(byte)131;//ErlangBinary;		

		return term.term_to_binary(ref Data,ref Index);

	}

    public static void term_to_binary(ref byte[] OutData,ref int OutStartIndex, Term term)
    {
        if (OutData.Length<IncDataSize)
            OutData= new byte[IncDataSize];

        OutData[OutStartIndex] = (byte)131;//ErlangBinary;		
        OutStartIndex++;

        OutData=term.term_to_binary(ref OutData, ref OutStartIndex);
    }

    public static byte[] term_to_binary(Term term,bool trim)
    {

        byte[] Data = new byte[IncDataSize];

        int Index = 1;



        Data[0] = (byte)131;//ErlangBinary;		

        Data=term.term_to_binary(ref Data, ref Index);
        if (trim)
        {
            byte[] newData = new byte[Index + 1];
            Array.Copy(Data, newData, newData.Length);
            return newData;
        }
        else
            return Data;

    }

	public static Term binary_to_term(ref byte [] Data,ref String Err)

	{

		if (Data.Length==0) { Err="Data is empty";return null; }

		int index=0;

		if (index>=Data.Length)

			{ Err="Wait `ErlangBinary` (131), but Data ends At:"+index.ToString(); return null; }

		//Erlang Binary: 131

		if (Data[index]!=131)

			{ Err="Wait `ErlangBinary` (131), but Have"+Data[index].ToString()+", At:"+index.ToString();return null;}

		

		index++;

		//ParsedTermToError=null;

		

		return binary_to_term(ref Data,ref index,ref Err);

	}

	public static Term binary_to_term(ref byte [] Data,ref int Index,ref String Err)

	{

		if (Index>=Data.Length)

		{Err="Wait `TypeID`, but Data ends At:"+Index.ToString();return null;}

		TermType t=(TermType)Data[Index];

		Index++;

		Term result=null,local;

		int len;

		byte [] val;

		switch(t)

		{

		case TermType.SMALL_INTEGER://(byte)97, (byte)Int

			if (Index>=Data.Length){Err="Wait `SMALL_INTEGER`, but Data ends At:"+Index.ToString();return null;}

			result=new Term(TermType.SMALL_INTEGER);

			result.IntVal=Data[Index];

			Index++;

			break;

		case TermType.INTEGER://(byte)98, (Int32)Int

			if (Index+3>=Data.Length){Err="Wait `INTEGER`, but Data ends At:"+Index.ToString();return null;}

			result=new Term(TermType.INTEGER);
            val = new byte[4];
            val[3] = Data[Index];val[2] = Data[Index+1];val[1] = Data[Index+2];val[0] = Data[Index+3];
			result.IntVal=BitConverter.ToInt32(val,0);

			Index+=4;

			break;

		case TermType.ATOM://(byte)100, (UInt16) Len, byte[Len] AtomName

			if (Index+1>=Data.Length){Err="Wait `ATOM`, but Data ends At:"+Index.ToString();return null;}
            val = new byte[2];
            val[1] = Data[Index];val[0] = Data[Index+1];
			
			len=BitConverter.ToUInt16(val,0);

			Index+=2;

			if (len>0)

			{

				if (Index+len>=Data.Length){Err="Wait `ATOM`Name, but Data ends (not enough "+

												(Data.Length-Index-len).ToString()+" bytes) At:"+Index.ToString();return null;}

				result=new Term(TermType.ATOM);

				val=new byte[len];

				Array.Copy(Data,Index,val,0,len);

				result.StrVal=Encoding.ASCII.GetString(val);

				Index+=len;

			}

			else

				result=new Term(TermType.ATOM);

			break;

		case TermType.SMALL_TUPLE://(byte)104, (byte)Arity, Type[Arity]

			if (Index>=Data.Length){Err="Wait `SMALL_TUPLE`, but Data ends At:"+Index.ToString();return null;}

			len=Data[Index];

			Index+=1;

			result=new Term(TermType.SMALL_TUPLE);

			if (len>0)

			{

				for (int i=0;i<len;i++)

				{

					local=binary_to_term(ref Data,ref Index,ref Err);

					if (local==null)

					{

						return null;

					}

					else

						result.TupleVal.Add(local);

				}

				result.IntVal=len-1;

			}

			else

				result=new Term(TermType.SMALL_TUPLE);	

			break;

		case TermType.LARGE_TUPLE://(byte)105, (UInt32)Arity, Type[Arity]

			if (Index+3>=Data.Length){Err="Wait `LARGE_TUPLE`, but Data ends At:"+Index.ToString();return null;}
            val = new byte[4];
            val[3] = Data[Index];val[2] = Data[Index+1];val[1] = Data[Index+2];val[0] = Data[Index+3];			
			len=(int)BitConverter.ToUInt32(val,0);

			Index+=4;

			result=new Term(TermType.LARGE_TUPLE);

			if (len>0)

			{

				for (int i=0;i<len;i++)

				{

					local=binary_to_term(ref Data,ref Index,ref Err);

					if (local==null)

					{

						return null;

					}

					else

						result.TupleVal.Add(local);

				}

				result.IntVal=len-1;

			}

			else

				result=new Term(TermType.LARGE_TUPLE);	

			break;

        case TermType.LIST://(byte)108, (UInt32)Arity, Type[Arity], TAIL(NIL)

            if (Index + 3 >= Data.Length) { Err = "Wait `LIST`, but Data ends At:" + Index.ToString(); return null; }
            val = new byte[4];
            val[3] = Data[Index]; val[2] = Data[Index + 1]; val[1] = Data[Index + 2]; val[0] = Data[Index + 3];
            len = (int)BitConverter.ToUInt32(val, 0);

            Index += 4;

            result = new Term(TermType.LIST);

            if (len > 0)
            {

                for (int i = 0; i < len; i++)
                {

                    local = binary_to_term(ref Data, ref Index, ref Err);

                    if (local == null)
                    {

                        return null;

                    }

                    else

                        result.TupleVal.Add(local);

                }

                result.IntVal = len - 1;

            }

            else

               result = new Term(TermType.LIST);

            //Parse TAIL == NULL
            local = binary_to_term(ref Data, ref Index, ref Err);
            if (local == null)
                return null;
            if (local.TypeOfTerm != TermType.NIL)
            {
                Err = "Wait `TAIL == NIL`, but Have:" + local.ToString(); 
                return null;
            }
            break;

		case TermType.NIL:

				result=new Term(TermType.NIL);

			break;

		case TermType.STRING:// (byte)107, (UInt16)Length, Characters[Length]

			if (Index+1>=Data.Length){Err="Wait `STRING`, but Data ends At:"+Index.ToString();return null;}
            
            val = new byte[2];
            val[1] = Data[Index];val[0] = Data[Index+1];
			
			len=(int)BitConverter.ToUInt16(val,0);

			Index+=2;

			if (len>0)

			{

				if (Index+len>=Data.Length){Err="Wait `STRING`Name, but Data ends (not enough "+

												(Data.Length-Index-len).ToString()+" bytes) At:"+Index.ToString();return null;}

				result=new Term(TermType.STRING);

				val=new byte[len];

				Array.Copy(Data,Index,val,0,len);

				result.StrVal=Encoding.Unicode.GetString(val);

				Index+=len;

			}

			else

				result=new Term(TermType.STRING);

			break;

	    case TermType.BINARY://(byte)109, (UInt32) Len, byte[Len]

			if (Index+3>=Data.Length){Err="Wait `BINARY`, but Data ends At:"+Index.ToString();return null;}
            val = new byte[4];
            val[3] = Data[Index];val[2] = Data[Index+1];val[1] = Data[Index+2];val[0] = Data[Index+3];
			
			len=(int)BitConverter.ToUInt32(val,0);

			Index+=4;

			if (len>0)

			{

				if (Index+len>=Data.Length){Err="Wait `BINARY`Data, but Data ends (not enough "+

												(Data.Length-Index-len).ToString()+" bytes) At:"+Index.ToString();return null;}

				result=new Term(TermType.BINARY);

				result.DataVal=new byte[len];

				Array.Copy(Data,Index,result.DataVal,0,len);

				Index+=len;

			}

			else

				result=new Term(TermType.BINARY);

				result.DataVal=new byte[0];

			break;

		case TermType.SMALL_ATOM:

			if (Index>=Data.Length){Err="Wait `SMALL_ATOM`, but Data ends At:"+Index.ToString();return null;}

			len=Data[Index];

			Index+=1;

			if (len>0)

			{

				if (Index+len>=Data.Length){Err="Wait `SMALL_ATOM`Name, but Data ends (not enough "+

												(Data.Length-Index-len).ToString()+" bytes) At:"+Index.ToString();return null;}

				result=new Term(TermType.SMALL_ATOM);

				val=new byte[len];

				Array.Copy(Data,Index,val,0,len);

                result.StrVal = Encoding.ASCII.GetString(val);

				Index+=len;

			}

			else

				result=new Term(TermType.SMALL_ATOM);



			break;

		case TermType.NEW_FLOAT:

			if (Index+7>=Data.Length){Err="Wait `NEW_FLOAT`, but Data ends At:"+Index.ToString();return null;}

			result=new Term(TermType.NEW_FLOAT);
            val = new byte[8];
            val[7] = Data[Index+7];val[6] = Data[Index+6];val[5] = Data[Index+5];val[4] = Data[Index+4];
			val[3] = Data[Index+3];val[2] = Data[Index+2];val[1] = Data[Index+1];val[0] = Data[Index+0];
			result.FloatVal=BitConverter.ToDouble(Data,Index);

			Index+=8;

			break;



		}

		return result;

	}

    public static Term binary_to_term_recursion(ref byte[] Data, ref int Index, ref int Length, ref String Err)
    {
        TermType t = (TermType)Data[Index];

        Index++;

        Term result = null, local;

        int len;

        byte[] val;

        switch (t)
        {

            case TermType.SMALL_INTEGER://(byte)97, (byte)Int

                if (Index >= Length) { Err = "Wait `SMALL_INTEGER`, but Data ends At:" + Index.ToString(); return null; }

                result = new Term(TermType.SMALL_INTEGER);

                result.IntVal = Data[Index];

                Index++;

                break;

            case TermType.INTEGER://(byte)98, (Int32)Int

                if (Index + 3 >= Length) { Err = "Wait `INTEGER`, but Data ends At:" + Index.ToString(); return null; }

                result = new Term(TermType.INTEGER);
                val = new byte[4];
                val[3] = Data[Index]; val[2] = Data[Index + 1]; val[1] = Data[Index + 2]; val[0] = Data[Index + 3];
                result.IntVal = BitConverter.ToInt32(val, 0);

                Index += 4;

                break;

            case TermType.ATOM://(byte)100, (UInt16) Len, byte[Len] AtomName

                if (Index + 1 >= Length) { Err = "Wait `ATOM`, but Data ends At:" + Index.ToString(); return null; }
                val = new byte[2];
                val[1] = Data[Index]; val[0] = Data[Index + 1];

                len = BitConverter.ToUInt16(val, 0);

                Index += 2;

                if (len > 0)
                {

                    if (Index + len >= Length)
                    {
                        Err = "Wait `ATOM`Name, but Data ends (not enough " +

                            (Length - Index - len).ToString() + " bytes) At:" + Index.ToString(); return null;
                    }

                    result = new Term(TermType.ATOM);

                    val = new byte[len];

                    Array.Copy(Data, Index, val, 0, len);

                    result.StrVal = Encoding.ASCII.GetString(val);

                    Index += len;

                }

                else

                    result = new Term(TermType.ATOM);

                break;

            case TermType.SMALL_TUPLE://(byte)104, (byte)Arity, Type[Arity]

                if (Index >= Length) { Err = "Wait `SMALL_TUPLE`, but Data ends At:" + Index.ToString(); return null; }

                len = Data[Index];

                Index += 1;

                result = new Term(TermType.SMALL_TUPLE);

                if (len > 0)
                {

                    for (int i = 0; i < len; i++)
                    {

                        local = binary_to_term_recursion(ref Data, ref Index, ref Length, ref Err);

                        if (local == null)
                        {

                            return null;

                        }

                        else

                            result.TupleVal.Add(local);

                    }

                    result.IntVal = len - 1;

                }

                else

                    result = new Term(TermType.SMALL_TUPLE);

                break;

            case TermType.LARGE_TUPLE://(byte)105, (UInt32)Arity, Type[Arity]

                if (Index + 3 >= Length) { Err = "Wait `LARGE_TUPLE`, but Data ends At:" + Index.ToString(); return null; }
                val = new byte[4];
                val[3] = Data[Index]; val[2] = Data[Index + 1]; val[1] = Data[Index + 2]; val[0] = Data[Index + 3];
                len = (int)BitConverter.ToUInt32(val, 0);

                Index += 4;

                result = new Term(TermType.LARGE_TUPLE);

                if (len > 0)
                {

                    for (int i = 0; i < len; i++)
                    {

                        local = binary_to_term_recursion(ref Data, ref Index, ref Length, ref Err);

                        if (local == null)
                        {

                            return null;

                        }

                        else

                            result.TupleVal.Add(local);

                    }

                    result.IntVal = len - 1;

                }

                else

                    result = new Term(TermType.LARGE_TUPLE);

                break;

            case TermType.LIST://(byte)108, (UInt32)Arity, Type[Arity], TAIL(NIL)

                if (Index + 3 >= Length) { Err = "Wait `LIST`, but Data ends At:" + Index.ToString(); return null; }
                val = new byte[4];
                val[3] = Data[Index]; val[2] = Data[Index + 1]; val[1] = Data[Index + 2]; val[0] = Data[Index + 3];
                len = (int)BitConverter.ToUInt32(val, 0);

                Index += 4;

                result = new Term(TermType.LIST);

                if (len > 0)
                {

                    for (int i = 0; i < len; i++)
                    {

                        local = binary_to_term_recursion(ref Data, ref Index,ref Length, ref Err);

                        if (local == null)
                        {

                            return null;

                        }

                        else

                            result.TupleVal.Add(local);

                    }

                    result.IntVal = len - 1;

                }

                else

                    result = new Term(TermType.LIST);

                //Parse TAIL == NULL
                local = binary_to_term_recursion(ref Data, ref Index, ref Length, ref Err);
                if (local == null)
                    return null;
                if (local.TypeOfTerm != TermType.NIL)
                {
                    Err = "Wait `TAIL == NIL`, but Have:" + local.ToString();
                    return null;
                }
                break;

            case TermType.NIL:

                result = new Term(TermType.NIL);

                break;

            case TermType.STRING:// (byte)107, (UInt16)Length, Characters[Length]

                if (Index + 1 >= Length) { Err = "Wait `STRING`, but Data ends At:" + Index.ToString(); return null; }

                val = new byte[2];
                val[1] = Data[Index]; val[0] = Data[Index + 1];

                len = (int)BitConverter.ToUInt16(val, 0);

                Index += 2;

                if (len > 0)
                {

                    if (Index + len >= Length)
                    {
                        Err = "Wait `STRING`Name, but Data ends (not enough " +

                            (Length - Index - len).ToString() + " bytes) At:" + Index.ToString(); return null;
                    }

                    result = new Term(TermType.STRING);

                    val = new byte[len];

                    Array.Copy(Data, Index, val, 0, len);

                    result.StrVal = Encoding.Unicode.GetString(val);

                    Index += len;

                }

                else

                    result = new Term(TermType.STRING);

                break;

            case TermType.BINARY://(byte)109, (UInt32) Len, byte[Len]

                if (Index + 3 >= Length) { Err = "Wait `BINARY`, but Data ends At:" + Index.ToString(); return null; }
                val = new byte[4];
                val[3] = Data[Index]; val[2] = Data[Index + 1]; val[1] = Data[Index + 2]; val[0] = Data[Index + 3];

                len = (int)BitConverter.ToUInt32(val, 0);

                Index += 4;

                if (len > 0)
                {

                    if (Index + len >= Length)
                    {
                        Err = "Wait `BINARY`Data, but Data ends (not enough " +

                            (Length - Index - len).ToString() + " bytes) At:" + Index.ToString(); return null;
                    }

                    result = new Term(TermType.BINARY);

                    result.DataVal = new byte[len];

                    Array.Copy(Data, Index, result.DataVal, 0, len);

                    Index += len;

                }

                else

                    result = new Term(TermType.BINARY);

                result.DataVal = new byte[0];

                break;

            case TermType.SMALL_ATOM:

                if (Index >= Length) { Err = "Wait `SMALL_ATOM`, but Data ends At:" + Index.ToString(); return null; }

                len = Data[Index];

                Index += 1;

                if (len > 0)
                {

                    if (Index + len >= Length)
                    {
                        Err = "Wait `SMALL_ATOM`Name, but Data ends (not enough " +

                            (Length - Index - len).ToString() + " bytes) At:" + Index.ToString(); return null;
                    }

                    result = new Term(TermType.SMALL_ATOM);

                    val = new byte[len];

                    Array.Copy(Data, Index, val, 0, len);

                    result.StrVal = Encoding.ASCII.GetString(val);

                    Index += len;

                }

                else

                    result = new Term(TermType.SMALL_ATOM);



                break;

            case TermType.NEW_FLOAT:

                if (Index + 7 >= Length) { Err = "Wait `NEW_FLOAT`, but Data ends At:" + Index.ToString(); return null; }

                result = new Term(TermType.NEW_FLOAT);
                val = new byte[8];
                val[7] = Data[Index + 7]; val[6] = Data[Index + 6]; val[5] = Data[Index + 5]; val[4] = Data[Index + 4];
                val[3] = Data[Index + 3]; val[2] = Data[Index + 2]; val[1] = Data[Index + 1]; val[0] = Data[Index + 0];
                result.FloatVal = BitConverter.ToDouble(Data, Index);

                Index += 8;

                break;
        }

        return result;

    }

    public static Term binary_to_term(ref byte[] Data, ref int Index, ref int Length, ref String Err)
    {

        if (Data.Length == 0) { Err = "Data is empty"; return null; }

        if (Data.Length < Length || Index >= Data.Length)
        { Err = "Index or Length > Data.Length"; return null; }
        if (Index >= Length)

        { Err = "Index >= Length"; return null; }


        if (Index >= Data.Length)

        { Err = "Wait `ErlangBinary` (131), but Data ends At:" + Index.ToString(); return null; }

        //Erlang Binary: 131

        if (Data[Index] != 131)

        { Err = "Wait `ErlangBinary` (131), but Have" + Data[Index].ToString() + ", At:" + Index.ToString(); return null; }



        Index++;

        //ParsedTermToError=null;

        return binary_to_term_recursion(ref Data, ref Index,ref Length, ref Err);

    }

	#endregion

    static void TermToThisBlock(ETF_TextBlock block,Term term)
    {
        //this is Block        
        if (!string.IsNullOrEmpty(term.TupleVal[0].StrVal))
            block.Name = term.TupleVal[0].StrVal;
        //other 
        for (int i = 1; i < term.TupleVal.Count; i++)
        {
            if (term.TupleVal[i].GetTermType() == TermType.SMALL_TUPLE)
            {
                //this is Attribute
                //TODO : Может и стоит добавить пару лишних проверок)), но пока можно
                //       отлаживать, это тут мы увидем, а потом нахуй)
                block.SetAttribute(term.TupleVal[i].TupleVal[0].StrVal,     //atom Name
                                   term.TupleVal[i].TupleVal[1].ToString(), //Some value
                                   term.TupleVal[i].TupleVal[1].GetTermType());//type
            }
            else
                if (term.TupleVal[i].GetTermType() == TermType.LIST)
                {
                    //this is childrens
                    foreach (Term t in term.TupleVal[i].TupleVal)
                    {
                        ETF_TextBlock tmp = block.AddChild("t");
                        TermToThisBlock(tmp, t);
                    }
                }
                else
                    throw new Exception("TermToBlock: wait List, have" + term.TupleVal[i].GetTermType().ToString() + " at:" + i.ToString() + "\n Term: " + term.ToString());
        }
    }

	public static ETF_TextBlock TermToBlock(Term term)
	{
        //this is Block
        ETF_TextBlock block = new ETF_TextBlock();
        if (!string.IsNullOrEmpty(term.TupleVal[0].StrVal))
            block.Name = term.TupleVal[0].StrVal;
        //other 
        for (int i = 1; i < term.TupleVal.Count;i++ )
        {
            if (term.TupleVal[i].GetTermType() == TermType.SMALL_TUPLE)
            {
                //this is Attribute
                //TODO : Может и стоит добавить пару лишних проверок)), но пока можно
                //       отлаживать, это тут мы увидем, а потом нахуй)
                block.SetAttribute(term.TupleVal[i].TupleVal[0].StrVal,     //atom Name
                                   term.TupleVal[i].TupleVal[1].ToString(), //Some value
                                   term.TupleVal[i].TupleVal[1].GetTermType());//type
            }
            else
                if (term.TupleVal[i].GetTermType() == TermType.LIST)
                {
                    //this is childrens
                    foreach (Term t in term.TupleVal[i].TupleVal)
                    {
                        ETF_TextBlock tmp = block.AddChild("Block");
                        TermToThisBlock(tmp,t);    
                    }
                }
                else
                    throw new Exception("TermToBlock: wait List, have" + term.TupleVal[i].GetTermType().ToString()+ " at:" + i.ToString() + "\n Term: " + term.ToString());
        }
        return block;
    }

    public static Term AttributeToTerm(ETF_TextBlock.ETF_Attribute attribute)
    {
        Term t=Term.Create_SMALL_TUPLE();
        if (attribute.Name==null)
            t.AddSmallAtom("");
        else
            t.AddSmallAtom(attribute.Name);
        switch (attribute.Type)
        {
            case TermType.ATOM:
                t.AddAtom(attribute.Value); break;
            /*case TermType.BINARY:
                t.AddAtom(attribute.Value); break;*/
            case TermType.INTEGER:
                t.AddInt(int.Parse(attribute.Value)); break;
            case TermType.NEW_FLOAT:
                t.AddFloat(float.Parse(attribute.Value)); break;
            case TermType.NIL:
                t.AddNull(); break;
            case TermType.SMALL_ATOM:
                t.AddSmallAtom(attribute.Value); break;
            case TermType.SMALL_INTEGER:
                t.AddByte(byte.Parse(attribute.Value)); break;
            case TermType.STRING:
                t.AddString(attribute.Value); break;
            default:
                throw new Exception("AttributeToTerm: not supported term type="+attribute.Type.ToString());
        }
        return t;
    }

    public static Term BlockToTerm(ETF_TextBlock Block)

	{
        if (Block == null) return null;
        Term root=Term.Create_LIST();
        if (Block.Name == null)
            root.AddSmallAtom("");
        else
            root.AddSmallAtom(Block.Name);
        Term child;        
        //attributes
        foreach (ETF_TextBlock.ETF_Attribute a in Block.Attributes)
        {
            root.TupleVal.Add(AttributeToTerm(a));       
        }
        if (Block.Children.Count > 0)
        {
            child = root.AddList();
            foreach (ETF_TextBlock c_block in Block.Children)
            {
                child.TupleVal.Add(BlockToTerm(c_block));
            }
        }

        return root;
	}
}


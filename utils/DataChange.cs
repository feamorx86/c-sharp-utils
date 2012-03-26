using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
/*
public class DataChange
{
	public static int DeafultIncrementSize=256;
	public static int DeafultMaxSize=int.MaxValue/4;
	
	byte []buffer;
	int MaxSize;
	int CurrentSize;
	int IncrementSize;
	int Position;//Текущее место записи
	
	public DataChange(int aMaxBufferSize,int aIncrementSize)
	{
		if (aMaxBufferSize==0)
			MaxSize=DataChange.DeafultMaxSize;
		else
			MaxSize=aMaxBufferSize;
		if (aIncrementSize==0)
			IncrementSize=DataChange.DeafultIncrementSize;
		else
			IncrementSize=aIncrementSize;
		CurrentSize=0;
		Position=0;
		buffer=null;
	}
	
	protected void ExpandBuffer()
	{
		#if UNITY_EDITOR
		if (CurrentSize+IncrementSize>MaxSize)
		{
			Debug.LogError("DataChange.ExpandBuffer: Превышен максимальный размер Буфера");
			return; 
		}
		#endif
		byte[] newData=new byte[CurrentSize+IncrementSize];
		
		Array.Copy(buffer,newData,buffer.Length-1);
		CurrentSize+=IncrementSize;
		buffer=newData;
	}
	
	protected void ReduceBuffer()
	{
		#if UNITY_EDITOR
		if (CurrentSize>=0)
		{
			Debug.LogError("DataChange.ReduceBuffer: CurrentSize>=0");
			return; 
		}
		#endif
		CurrentSize-=IncrementSize;		
		if (CurrentSize<=0)
		{
			CurrentSize=0;
			Position=0;
			buffer=null;			
		}
		else
		{
			byte[] newData=new byte[CurrentSize];		
			Array.Copy(buffer,IncrementSize,newData,0,CurrentSize);
			Position-=IncrementSize;
			buffer=newData;
		}
	}
	#region Write Methods
	public void WriteBytes(byte[] Value,bool WriteArraySize)
	{
		if (Position+Value.Length+4>=CurrentSize)
			ExpandBuffer();
		if (WriteArraySize)
			WriteInt32(Value.Length);
		
		for (int i=0;i<Value.Length;++i)
		{
			buffer[Position]=Value[i];
			Position++;
		}	
	}
	
	public void WriteString(string value)
	{
		WriteBytes(System.Text.Encoding.Unicode.GetBytes(value),true);
	}
	
	public void WriteByte(byte Value)
	{
		if (Position+1>=CurrentSize)
			ExpandBuffer();
		buffer[Position]=Value;
		Position++;
	}
	
	public void WriteInt32(Int32 Value)
	{
		if (Position+4>=CurrentSize)
			ExpandBuffer();
		
		byte []val=BitConverter.GetBytes(Value);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		Position+=4;		
	}
	
	public void WriteUInt32(UInt32 Value)
	{
		if (Position+4>=CurrentSize)
			ExpandBuffer();
		
		byte []val=BitConverter.GetBytes(Value);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		Position+=4;		
	}
	
	public void WriteInt16(Int16 Value)
	{
		if (Position+2>=CurrentSize)
			ExpandBuffer();
		
		byte []val=BitConverter.GetBytes(Value);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		Position+=2;		
	}
	
	public void WriteUInt16(UInt16 Value)
	{
		if (Position+2>=CurrentSize)
			ExpandBuffer();
		
		byte []val=BitConverter.GetBytes(Value);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		Position+=2;		
	}
	
	public void WriteFloat(float Value)
	{
		if (Position+4>=CurrentSize)
			ExpandBuffer();
		
		byte []val=BitConverter.GetBytes(Value);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		Position+=4;		
	}
	
	public void WriteVector2(Vector2 Value)
	{
		if (Position+8>=CurrentSize)
			ExpandBuffer();
		
		byte []val=BitConverter.GetBytes(Value.x);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		
		val=BitConverter.GetBytes(Value.y);
		buffer[Position+4]=val[0];
		buffer[Position+5]=val[1];
		buffer[Position+6]=val[2];
		buffer[Position+7]=val[3];
		
		Position+=8;		
	}
	
	public void WriteVector3(Vector3 Value)
	{
		if (Position+12>=CurrentSize)
			ExpandBuffer();
		
		byte []val=BitConverter.GetBytes(Value.x);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		
		val=BitConverter.GetBytes(Value.y);
		buffer[Position+4]=val[0];
		buffer[Position+5]=val[1];
		buffer[Position+6]=val[2];
		buffer[Position+7]=val[3];
		
		val=BitConverter.GetBytes(Value.z);
		buffer[Position+8]=val[0];
		buffer[Position+9]=val[1];
		buffer[Position+10]=val[2];
		buffer[Position+11]=val[3];
		
		Position+=12;		
	}
	
	public void WriteVector4(Vector4 Value)
	{
		if (Position+16>=CurrentSize)
			ExpandBuffer();
		
		byte []val=BitConverter.GetBytes(Value.x);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		
		val=BitConverter.GetBytes(Value.y);
		buffer[Position+4]=val[0];
		buffer[Position+5]=val[1];
		buffer[Position+6]=val[2];
		buffer[Position+7]=val[3];
		
		val=BitConverter.GetBytes(Value.z);
		buffer[Position+8]=val[0];
		buffer[Position+9]=val[1];
		buffer[Position+10]=val[2];
		buffer[Position+11]=val[3];
		
		val=BitConverter.GetBytes(Value.w);
		buffer[Position+12]=val[0];
		buffer[Position+13]=val[1];
		buffer[Position+14]=val[2];
		buffer[Position+15]=val[3];
		
		Position+=16;		
	}
	
	public void WriteQuaternion(Quaternion Value)
	{
		if (Position+16>=CurrentSize)
			ExpandBuffer();
		
		byte []val=BitConverter.GetBytes(Value.x);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		
		val=BitConverter.GetBytes(Value.y);
		buffer[Position+4]=val[0];
		buffer[Position+5]=val[1];
		buffer[Position+6]=val[2];
		buffer[Position+7]=val[3];
		
		val=BitConverter.GetBytes(Value.z);
		buffer[Position+8]=val[0];
		buffer[Position+9]=val[1];
		buffer[Position+10]=val[2];
		buffer[Position+11]=val[3];
		
		val=BitConverter.GetBytes(Value.w);
		buffer[Position+12]=val[0];
		buffer[Position+13]=val[1];
		buffer[Position+14]=val[2];
		buffer[Position+15]=val[3];
		
		Position+=16;		
	}
	#endregion
	
	#region Write NoExpand Methods
	public void WriteBytesNoExpand(byte[] Value,bool WriteArraySize)
	{
		if (WriteArraySize)
			WriteInt32(Value.Length);
		
		for (int i=0;i<Value.Length;++i)
		{
			buffer[Position]=Value[i];
			Position++;
		}			
	}
	///
	public void WriteStringNoExpand(string value)
	{
		WriteBytesNoExpand(System.Text.Encoding.Unicode.GetBytes(value),true);
	}
	
	public void WriteByteNoExpand(byte Value)
	{		
		buffer[Position]=Value;
		Position++;
	}
	
	public void WriteInt32NoExpand(Int32 Value)
	{
		byte []val=BitConverter.GetBytes(Value);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		Position+=4;		
	}
	
	public void WriteUInt32NoExpand(UInt32 Value)
	{
		byte []val=BitConverter.GetBytes(Value);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		Position+=4;		
	}
	
	public void WriteInt16NoExpand(Int16 Value)
	{
		byte []val=BitConverter.GetBytes(Value);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		Position+=2;		
	}
	
	public void WriteUInt16NoExpand(UInt16 Value)
	{
		byte []val=BitConverter.GetBytes(Value);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		Position+=2;		
	}
	
	public void WriteFloatNoExpand(float Value)
	{		
		byte []val=BitConverter.GetBytes(Value);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		Position+=4;		
	}
	
	public void WriteVector2NoExpand(Vector2 Value)
	{
		if (Position+8>=CurrentSize)
			ExpandBuffer();
		
		byte []val=BitConverter.GetBytes(Value.x);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		
		val=BitConverter.GetBytes(Value.y);
		buffer[Position+4]=val[0];
		buffer[Position+5]=val[1];
		buffer[Position+6]=val[2];
		buffer[Position+7]=val[3];
		
		Position+=8;		
	}
	
	public void WriteVector3NoExpand(Vector3 Value)
	{	
		byte []val=BitConverter.GetBytes(Value.x);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		
		val=BitConverter.GetBytes(Value.y);
		buffer[Position+4]=val[0];
		buffer[Position+5]=val[1];
		buffer[Position+6]=val[2];
		buffer[Position+7]=val[3];
		
		val=BitConverter.GetBytes(Value.z);
		buffer[Position+8]=val[0];
		buffer[Position+9]=val[1];
		buffer[Position+10]=val[2];
		buffer[Position+11]=val[3];
		
		Position+=12;		
	}
	
	public void WriteVector4NoExpand(Vector4 Value)
	{	
		byte []val=BitConverter.GetBytes(Value.x);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		
		val=BitConverter.GetBytes(Value.y);
		buffer[Position+4]=val[0];
		buffer[Position+5]=val[1];
		buffer[Position+6]=val[2];
		buffer[Position+7]=val[3];
		
		val=BitConverter.GetBytes(Value.z);
		buffer[Position+8]=val[0];
		buffer[Position+9]=val[1];
		buffer[Position+10]=val[2];
		buffer[Position+11]=val[3];
		
		val=BitConverter.GetBytes(Value.w);
		buffer[Position+12]=val[0];
		buffer[Position+13]=val[1];
		buffer[Position+14]=val[2];
		buffer[Position+15]=val[3];
		
		Position+=16;		
	}
	
	public void WriteQuaternionNoExpand(Quaternion Value)
	{	
		byte []val=BitConverter.GetBytes(Value.x);
		
		buffer[Position]=val[0];
		buffer[Position+1]=val[1];
		buffer[Position+2]=val[2];
		buffer[Position+3]=val[3];
		
		val=BitConverter.GetBytes(Value.y);
		buffer[Position+4]=val[0];
		buffer[Position+5]=val[1];
		buffer[Position+6]=val[2];
		buffer[Position+7]=val[3];
		
		val=BitConverter.GetBytes(Value.z);
		buffer[Position+8]=val[0];
		buffer[Position+9]=val[1];
		buffer[Position+10]=val[2];
		buffer[Position+11]=val[3];
		
		val=BitConverter.GetBytes(Value.w);
		buffer[Position+12]=val[0];
		buffer[Position+13]=val[1];
		buffer[Position+14]=val[2];
		buffer[Position+15]=val[3];
		
		Position+=16;		
	}
	#endregion
	
	#region Read Reduce Methods
	public byte[] ReadBytesReduce(int count)
	{
		byte [] out_d=new byte[count];
		
		Array.Copy(buffer,Position,out_d,0,count);
		Position+=count;
		
		if (Position>=IncrementSize)
			ReduceBuffer();
		 
		return out_d;
	}
	
	public byte[] ReadBytesReduce()
	{
		int count=ReadInt32Reduce();
		byte [] out_d=new byte[count];
		
		Array.Copy(buffer,Position,out_d,0,count);
		Position+=count;
		
		if (Position>=IncrementSize)
			ReduceBuffer();
		 
		return out_d;
	}
	
	public string ReadStringReduce()
	{
		int count=ReadInt32Reduce();		
		byte [] out_d=ReadBytesReduce(count);
		return System.Text.Encoding.Unicode.GetString(out_d);
	}
	
	public  byte ReadByteReduce()
	{
		byte val=buffer[Position];
		Position++; 
		if (Position>=IncrementSize)
			ReduceBuffer();
		return val;
	}
	
	public Int32 ReadInt32Reduce()
	{		
		Int32 val= BitConverter.ToInt32(buffer,Position);
		Position+=4; 
		if (Position>=IncrementSize)
			ReduceBuffer();
		return val;
	}
	
	public UInt32 ReadUInt32Reduce()
	{
		UInt32 val= BitConverter.ToUInt32(buffer,Position);
		Position+=4;
		if (Position>=IncrementSize)
			ReduceBuffer();
		return val;
	}
	
	public Int16 ReadInt16Reduce()
	{
		Int16 val= BitConverter.ToInt16(buffer,Position);
		Position+=2; 
		if (Position>=IncrementSize)
			ReduceBuffer();
		return val;		
	}
	
	public  UInt16 ReadUInt16Reduce()
	{
		UInt16 val= BitConverter.ToUInt16(buffer,Position);
		Position+=2; 
		if (Position>=IncrementSize)
			ReduceBuffer();
		return val;				
	}
	
	public  float ReadFloatReduce()
	{
		float val= BitConverter.ToSingle(buffer,Position);
		Position+=4; 
		if (Position>=IncrementSize)
			ReduceBuffer();
		return val;				
	}
	
	public  Vector2 ReadVector2Reduce()
	{
		Vector2 result=new Vector2(BitConverter.ToSingle(buffer,Position),
		                           BitConverter.ToSingle(buffer,Position+4));
		Position+=8; 			
		if (Position>=IncrementSize)
			ReduceBuffer();
		return result;			
	}
	
	public Vector3 ReadVector3Reduce()
	{
		Vector3 result=new Vector3(BitConverter.ToSingle(buffer,Position),
		                           BitConverter.ToSingle(buffer,Position+4),
		                           BitConverter.ToSingle(buffer,Position+8));
		Position+=12;
		if (Position>=IncrementSize)
			ReduceBuffer();
		return result;			
	}
	
	public Vector4 ReadVector4Reduce()
	{
		Vector4 result=new Vector4(BitConverter.ToSingle(buffer,Position),
		                           BitConverter.ToSingle(buffer,Position+4),
		                           BitConverter.ToSingle(buffer,Position+8),
		                           BitConverter.ToSingle(buffer,Position+12));
		Position+=16;
		if (Position>=IncrementSize)
			ReduceBuffer();
		return result;	
	}
	
	public Quaternion ReadQuaternionReduce()
	{
		Quaternion result=new Quaternion( BitConverter.ToSingle(buffer,Position),
		                     		      BitConverter.ToSingle(buffer,Position+4),
		                 		          BitConverter.ToSingle(buffer,Position+8),
		                		          BitConverter.ToSingle(buffer,Position+12));
		Position+=16;
		if (Position>=IncrementSize)
			ReduceBuffer();
		return result;
	}
	#endregion
	
		
	#region Read No Reduce Methods
	public byte[] ReadBytes(int count)
	{
		byte [] out_d=new byte[count];
		
		Array.Copy(buffer,Position,out_d,0,count);
		Position+=count;
		
		return out_d;
	}
	
	public byte[] ReadBytes()
	{
		int count=ReadInt32();
		byte [] out_d=new byte[count];
		
		Array.Copy(buffer,Position,out_d,0,count);
		Position+=count;
		
		return out_d;
	}
	
	public string ReadString()
	{
		int count=ReadInt32();		
		byte [] out_d=ReadBytes(count);
		return System.Text.Encoding.Unicode.GetString(out_d);
	}
	
	public  byte ReadByte()
	{
		return buffer[Position++];		
	}
	
	public Int32 ReadInt32()
	{		
		Int32 val= BitConverter.ToInt32(buffer,Position);
		Position+=4; 		
		return val;
	}
	
	public UInt32 ReadUInt32()
	{
		UInt32 val= BitConverter.ToUInt32(buffer,Position);
		Position+=4;		
		return val;
	}
	
	public Int16 ReadInt16()
	{
		Int16 val= BitConverter.ToInt16(buffer,Position);
		Position+=2; 		
		return val;		
	}
	
	public  UInt16 ReadUInt16()
	{
		UInt16 val= BitConverter.ToUInt16(buffer,Position);
		Position+=2; 
		return val;				
	}
	
	public  float ReadFloat()
	{
		float val= BitConverter.ToSingle(buffer,Position);
		Position+=4; 
		if (Position>=IncrementSize)
			ReduceBuffer();
		return val;				
	}
	
	public  Vector2 ReadVector2()
	{
		Vector2 result=new Vector2(BitConverter.ToSingle(buffer,Position),
		                           BitConverter.ToSingle(buffer,Position+4));
		Position+=8; 					
		return result;			
	}
	
	public Vector3 ReadVector3()
	{
		Vector3 result=new Vector3(BitConverter.ToSingle(buffer,Position),
		                           BitConverter.ToSingle(buffer,Position+4),
		                           BitConverter.ToSingle(buffer,Position+8));
		Position+=12;
		return result;			
	}
	
	public Vector4 ReadVector4()
	{
		Vector4 result=new Vector4(BitConverter.ToSingle(buffer,Position),
		                           BitConverter.ToSingle(buffer,Position+4),
		                           BitConverter.ToSingle(buffer,Position+8),
		                           BitConverter.ToSingle(buffer,Position+12));
		Position+=16;
		return result;	
	}
	
	public Quaternion ReadQuaternion()
	{
		Quaternion result=new Quaternion( BitConverter.ToSingle(buffer,Position),
		                     		      BitConverter.ToSingle(buffer,Position+4),
		                 		          BitConverter.ToSingle(buffer,Position+8),
		                		          BitConverter.ToSingle(buffer,Position+12));
		Position+=16;
		return result;
	}
	#endregion
	
	public void WriteDataToFile(string FileName)
	{
		FileStream fs=File.OpenWrite(Application.dataPath+FileName);
		fs.Write(buffer,0,CurrentSize);
		fs.Close();
	}
	
	public void ReadDataFromFile(string FileName)
	{
		buffer = File.ReadAllBytes(FileName);
		CurrentSize=buffer.Length;
		if (MaxSize	<CurrentSize)
			MaxSize=CurrentSize+IncrementSize+1;
		Position=CurrentSize;
	}
}
*/
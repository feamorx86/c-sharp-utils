using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Log
{
    public static List<String> Errors;

    public delegate void LogOnMessage(String Message);

    public static LogOnMessage ErrorHandler;
    public static LogOnMessage WarningHandler;
    public static LogOnMessage InfoHandler;
    public static BinaryWriter Writer=null;
    
    public static void InitLog(string FileName)
    {
        Errors = new List<string>();
        
        try
        {
        	if (File.Exists(FileName))
    	    {
        		if (File.Exists(FileName+"~"))
	    	    {
    	    		File.Delete(FileName+"~");
    		    }
        		
        		File.Move(FileName,FileName+"~");
    	    }
        }
        catch(Exception ex)
        {
        	Log.LogError("Init log error at Delete "+FileName+"~ : "+ex.Message);        	
        }
        
        try
        {
        	Writer=new BinaryWriter(new FileStream(FileName,FileMode.CreateNew));
        }
        catch(Exception ex)
        {
        	Log.LogError("Init log error: "+ex.Message);        	
        }
        
        if (Writer!=null && Writer.BaseStream.CanWrite)
    			Writer.Write(" Log Started at : "+DateTime.Now.ToLongTimeString()+"\n");

    }
    
    public static void Dispose()
    {
    	Errors.Clear();
    	if (Writer!=null)
    	{
    		if (Writer.BaseStream.CanWrite)
    			Writer.Write(" Log Finished at : "+DateTime.Now.ToLongTimeString());
	    	Writer.Close();
    	}
    }

    public static void LogError(string Message)
    {
        Errors.Add(Message);
        if (Writer!=null && Writer.BaseStream.CanWrite)
        {
        	Writer.Write(DateTime.Now.ToLongTimeString()+" <Err> "+Message+"\n");
        	//Writer.Flush();
        }
        
        if (ErrorHandler!=null)
        	ErrorHandler(Message);
    }

    public static void LogWarnings(string Message)
    {
    	Errors.Add(Message);
    	
        if (Writer!=null && Writer.BaseStream.CanWrite)    
        {
        	Writer.Write(DateTime.Now.ToLongTimeString()+" <Wrg> "+Message+"\n");
        	//Writer.Flush();
        }

    	if (WarningHandler!=null)
        	WarningHandler(Message);
    }

    public static void LogInfo (string Message)
	{
	  	if (Writer!=null && Writer.BaseStream.CanWrite)    
	  	{
	  		Writer.Write(DateTime.Now.ToLongTimeString()+" <Inf> "+Message+"\n");
        	//Writer.Flush();
	  	}

		Errors.Add (Message);
		if (InfoHandler!=null)
			InfoHandler (Message);
	}
	
	public static string AllAsString ()
	{
		string result = "";
		if (Errors!=null)
			foreach (string s in Errors)	
				result += s;
		return result;
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public delegate void OnMessage(ReceivedMessage message);

public class MessageToRead
{
    public int ID;
    public OnMessage OnReciveMessage;
	
	public static int CompareByID(MessageToRead A,MessageToRead B)
	{
		if (A.ID>B.ID) return 1;
		if (A.ID<B.ID) return -1;
		return 0;
	}
}

public class ReceivedMessage
{
    public LinkedListNode<ReceivedMessage> MyNode;
    public byte [] RawData;
    public Term Data;
    public ushort LifeTime;//in Seconds
    public bool IsReaded;
    public MessageToRead Message;

    public ReceivedMessage()
    {
        MyNode = null;
        Data = null;
        LifeTime = GameNetwork.MaxReciveLifeTime;
        IsReaded = false;
        RawData=null;
        Message = null;
    }
    public ReceivedMessage(Term aData, MessageToRead msg)
    {
        Data = aData;
        LifeTime = GameNetwork.MaxReciveLifeTime;
        IsReaded = false;
        MyNode = null;
        RawData=null;
        Message = msg;
    }

    public string SetHandler()
    {
        if (Data.GetTermType() != TermType.LIST)
            return "Type of root term is not List";
        if (Data.TupleVal.Count <= 0)
            return "Root term is Empty";

        if (Data.TupleVal[0].GetTermType() != TermType.INTEGER)
            return "ID (First in root) term is not Integer";
		
		Message=GameNetwork.Instance.ReadTable_Find(Data.TupleVal[0].IntVal);
		
        if (Message==null)
            return "Can`t Finde message in table, ID=" + Data.TupleVal[0].IntVal.ToString();

        return null;
    }
}

public enum SendingMessageStatus
{
    None=0,
    WaitOperation,
    InProgress,
    Successful,
    Failed
}

public class SendingMessage
{
    public LinkedListNode<SendingMessage> MyNode;
    public int ID;
    public ushort LifeTime;
    public Term Data;
    public SendingMessageStatus Status;

    public SendingMessage()
    {
        MyNode = null;
        ID = -1;
        Data = null;
        LifeTime = GameNetwork.MaxSendLifeTime;
        Status = SendingMessageStatus.None;
    }

    public SendingMessage(int aID, Term aData)
    {
        MyNode = null;
        ID = aID;
        Data = aData;
        LifeTime = GameNetwork.MaxSendLifeTime;
        Status = SendingMessageStatus.None;
    }

    public SendingMessage(int aID)
    {
        MyNode = null;
        ID = aID;
        Data = null;
        LifeTime = GameNetwork.MaxSendLifeTime;
        Status = SendingMessageStatus.None;
    }

    public SendingMessage(int aID, Term aData,SendingMessageStatus aStatus)
    {
        MyNode = null;
        ID = aID;
        Data = aData;
        LifeTime = GameNetwork.MaxSendLifeTime;
        Status = aStatus;
    }
}

public class GameNetwork
{
    public static ushort MaxSendLifeTime = 60;
    public static ushort MaxReciveLifeTime = 60;

    TcpClient client;
    public IPEndPoint RemotePoint;

    List<MessageToRead> ReadTable;
	
	public MessageToRead ReadTable_Find(int Message)
	{
		MessageToRead result;
		if (!Sorting.BinarySearch<MessageToRead>(ReadTable,ref result,MessageToRead.CompareByID))
			return null;
		else
			return result;
	}
	public MessageToRead RegisterMessageHandler(int msgID, OnMessage Handler)
    {
		if (ReadTable_Find(msgID)!=null) 
			return  null;
		
        MessageToRead msg = new MessageToRead();
        msg.ID = msgID;
        msg.OnReciveMessage = Handler;
		
		Sorting.BinaryInsert<MessageToRead>(ref ReadTable,msg,MessageToRead.CompareByID);
  
		return msg;
    }
    public bool UnRegisterMessageHandler(int msgID)
    {
        return ReadTable.Remove(msgID);
    }

    public LinkedList<SendingMessage> MessagesToSend;
    public LinkedList<ReceivedMessage> MessagesToRead;

    LinkedList<SendingMessage> WriteMessages;
    LinkedList<ReceivedMessage> ReadMessages;

    public static GameNetwork Instance;

    Thread ReadThread,WriteThread;

    public bool void Init()
    {
        if (Instance != null)
        {
            throw new Exception("GameNetwork already created");
        }
        Instance = new GameNetwork();

        Instance.ReadTable = new List<MessageToRead>();
        Instance.MessagesToRead = new LinkedList<ReceivedMessage>();
        Instance.MessagesToSend = new LinkedList<SendingMessage>();

        Instance.WriteMessages = new LinkedList<SendingMessage>();
        Instance.ReadMessages = new LinkedList<ReceivedMessage>();

        Instance.client = new TcpClient();

        Instance.rand = new System.Random();

        Instance.ReadLocker = new object();
        Instance.WriteLocker = new object();

        Instance.IsError = false;
    }

    public void Start(string Address,int Port)
    {
        RemotePoint = new IPEndPoint(IPAddress.Parse(Address), Port);

        ReadThread = new Thread(DoStartAndReadMessages);
        WriteThread = new Thread(DoWriteMessages);
        IsConnected = false;

        ReadThread.Start();
    }

    
    public static void AddMessage(SendingMessage msg)
    {
        msg.MyNode=Instance.MessagesToSend.AddLast(msg);
    }

    public void Stop()
    {
        NeedStopRead = true;
        NeedStopWrite = true;

        Thread.Sleep(1000);

        try
        {
            if (WriteThread.ThreadState != ThreadState.Stopped && WriteThread.ThreadState != ThreadState.Aborted)
                WriteThread.Abort();
        }
        catch (Exception ex)
        {
            IsError = true;
            LastErrorMessage+="GameNetwork.Stop. Error at stopping Write thread : "+ex.Message;
        }

        try
        {
            if (ReadThread.ThreadState != ThreadState.Stopped && ReadThread.ThreadState != ThreadState.Aborted)
                ReadThread.Abort();
        }
        catch (Exception ex)
        {
            IsError = true;
            LastErrorMessage += "GameNetwork.Stop. Error at stopping Read thread : " + ex.Message;

        }

        client.Close();

        ReadBuffer = null;
        WriteBuffer = null;
        p_Data = null;

        ReadTable.Clear(); ReadTable = null;
        MessagesToSend.Clear(); MessagesToSend = null;
        MessagesToRead.Clear(); MessagesToRead = null;
        WriteMessages.Clear(); WriteMessages = null;
        ReadMessages.Clear(); ReadMessages = null;
    }

    public static void StopAndRelase()
    {
        Instance.Stop();
        Instance=null;
    }

    public bool IsConnected=false;
    public bool IsError=false;

    public bool NeedStopRead = false,NeedStopWrite=false,NeedStartWrite=true;
   
    public System.Random rand;

    public string LastErrorMessage;
    public int LastErrorCode;

    public const int DefaultBufferSize = 100;

    public byte[] ReadBuffer = new byte[DefaultBufferSize];
    public byte[] WriteBuffer = new byte[DefaultBufferSize];

    object ReadLocker,WriteLocker;

    void DoStartAndReadMessages()
    {
        if (client == null)
        {
            LastErrorMessage = LastErrorMessage + "Client.Connect: Client is Null\n";
            IsError = true;
            return;
        }

        if (!client.Connected)
        {
            try
            {
                client.Connect(RemotePoint);
                IsConnected = true;
            }
            catch (System.Exception ex)
            {
                LastErrorMessage += ex.Message;
                IsError = true;
            }
        }
        if (!client.Connected) return;
        //Подключились

        //Начинаем писать
        if (NeedStartWrite)
        {
            try
            {
                NeedStartWrite = false;
                WriteThread.Start();
            }
            catch (Exception ex)
            {
                IsError = true;
                LastErrorMessage += "GameNetwork at TryStartWriteThread: " + ex.Message;
            }
        }
        

        //начинаем читать
        while (!NeedStopRead)
        {
            if (IsError)
            {
                Thread.Sleep(WasErrorSleepTime);
                continue;
            }

            if (!client.Connected)
            {
                IsError = true;
                LastErrorMessage = "GameNetwork.Read: connection breaks?";
                continue;
            }
            
            try
            {
                pd_oldLength = pd_length;
                pd_length = client.GetStream().Read(ReadBuffer, 0, ReadBuffer.Length);
                p_Data = ReadBuffer;
                pd_index = 0;
            }
            catch (Exception ex)
            {
                IsError = true;
                LastErrorMessage += ex.Message;
            }

            if (!client.Connected || pd_length==0)
            {
                IsError = true;
                LastErrorMessage = "GameNetwork.Read: connection breaks?";
                continue;
            }

            if (pd_length == 0)
            {
                IsError = true;
                LastErrorMessage = "GameNetwork.Read: Was read 0 bytes - maybe connection breaks?";
                continue;
            }

            if (IsError) continue;

            
            //Read message
            ParseMessage();

            if (p_state==0)
            {
                //Parse binary_to_term
                string err="";
                p_currentMsg.Data = Term.binary_to_term(ref p_currentMsg.RawData, ref err);
                if (p_currentMsg.Data == null)
                {
                     IsError = true;
                     LastErrorMessage += "GameNetwork.ReadMessage: BinToTerm Error: " + err;
                }
                else
                {
                    //Finde Recive Handler
                    err=p_currentMsg.SetHandler();
                    if (err!=null)
                    {
                        IsError = true;
                        LastErrorMessage += "GameNetwork.ReadMessage: Set Message Handler Error: "+err;
                    }
                    else
                    {   
                        // Add Message To Update;
                        lock (ReadLocker)
                        {
                            ReadMessages.AddLast(p_currentMsg);
                        }
                        p_currentMsg = null;
                    }
                }
            }
            else
                if (p_state >= 100)
                {
                    IsError = true;
                    LastErrorMessage += "GameNetwork.ReadMessage: ParseError, code=" + p_state.ToString();
                }
            //else ... continue read message.
        }
    }

    #region  Parse Message 

    // Parse Message States
    // 0 : Read Complete
    // 1 : Need continue read ID
    // 2 : Need Continue read Dynamic Data Length
    // 3 : Need Continue read Data
    // 100+ : Errors
    // 110 : Error: d_Index Is out of range.
    // 111 : Error: Data is Null or Empty.
    // 112 : Error: Wrong d_Length, is zero or out of range.
    // 113 : Error: Length of ID==4, but Index+4-Length != {1,2,3,4}
    // 114 : Error: Length of Data_Length==2, but Inex+2-Length != {1,2}
    // 115 : Error: Message.Length - have wrong value
    // 116 : Error: Wrong State
    // 120 : Error: can`t finde MessageToRead with such ID

    public byte p_state;//parse result 0=complete,1=id,2=len,3=data,100...255 =error;
    public int pd_index;//index in p_data, needed bytes if not complete
    public int pd_needbytes;
    public int pd_readedIndex;
    public int pd_length;//max used length in p_data;
    public int pd_oldLength;
    public ReceivedMessage p_currentMsg;
    byte[] p_Data;

    public byte[] p_tmp = new byte[4];//Message.ID | Message.Length
    public int p_itmp;

    public void ParseMessage()
    {
        #region  Handle Errors  

        if (p_state>=100) { LastErrorMessage+= "GameNetwork.ParseMessage: Unhandled Error state code= "+p_state.ToString(); IsError=true; return; };//необработанная ошибка
        if (p_Data == null || p_Data.Length <= 0) { p_state=110; LastErrorMessage+="GameNetwork.ParseMessage: Data is Null or empty"; return;}
        if (pd_length <= 0) { p_state = 111; LastErrorMessage += "GameNetwork.ParseMessage: Read length <= 0"; return; }
        if (pd_length >= p_Data.Length) { p_state = 112; LastErrorMessage += "GameNetwork.ParseMessage: Data.length<=Read Length"; return; }
        if (pd_index >= p_Data.Length) { p_state = 113; LastErrorMessage += "GameNetwork.ParseMessage: Index>Data.Length"; return; }

        #endregion

        #region State 0 - Read New Message
        if (p_state == 0)
        {
            p_currentMsg=new ReceivedMessage();
            //read ID
            
            #region  Need more bytes for Length
            if (pd_index+4>pd_length)
            {
                p_state=1;
                pd_needbytes = pd_index + 4 - pd_length;
                if (pd_needbytes==1)//have 3
                {
                    /*p_tmp[0]=p_Data[pd_index+3];*/p_tmp[1]=p_Data[pd_index+2];
                    p_tmp[2]=p_Data[pd_index+1];p_tmp[3]=p_Data[pd_index  ];
                    pd_index+=3;
                }
                else
                if (pd_needbytes==2)//have 2
                {
                    /*p_tmp[0]=p_Data[pd_index+3];p_tmp[1]=p_Data[pd_index+2];*/
                    p_tmp[2]=p_Data[pd_index+1];p_tmp[3]=p_Data[pd_index  ];
                    pd_index+=2;
                }
                else
                if (pd_needbytes==3)//have 1
                {
                    /*p_tmp[0]=p_Data[pd_index+3];p_tmp[1]=p_Data[pd_index+2];
                    p_tmp[2]=p_Data[pd_index+1];*/p_tmp[3]=p_Data[pd_index  ];
                    pd_index+=1;
                }
                else
                {
                    //nothinc 
                    p_currentMsg = null;
                    throw new Exception("GameNetwork.ParseMessage: Невозможная ситуация, Блядь!");

                }
                return;
            }
             #endregion

            p_tmp[0]=p_Data[pd_index+3];p_tmp[1]=p_Data[pd_index+2];
            p_tmp[2]=p_Data[pd_index+1];p_tmp[3]=p_Data[pd_index  ];
            pd_index+=4;

            p_itmp=BitConverter.ToInt32(p_tmp,0);

            if (p_itmp > 0)
            {
            //Create New Message
                p_currentMsg.IsReaded=false;
                p_currentMsg.LifeTime=MaxReciveLifeTime;

                p_currentMsg.RawData=new byte [p_itmp];
                    
                if (pd_index+p_itmp>pd_length)
                {
                    // Not enough data, need read more data
                    Array.Copy(p_Data, pd_index, p_currentMsg.RawData, 0, pd_length - pd_index);
                    pd_needbytes = p_itmp - pd_index - pd_length;
                    pd_readedIndex = pd_length - pd_index;
                    p_state = 3;
                }
                else
                {
                    //Enough data - complete message.
                    Array.Copy(p_Data,pd_index,p_currentMsg.RawData,0,p_itmp);
                    pd_index+=p_itmp;
                    p_state=0;
                    return;
                }
            }
            else
            {
                //error
                p_state = 115;
                LastErrorMessage += "GameNetwork.ParseMessage: Message length <=0";
                return;
            }
        }
        else
        #endregion

        #region State 1 - Continue read ID
        
        if (p_state==1)
        {
            #region  Parse `end of Length` or continue read.

            switch (pd_needbytes)
            {
                case 1:
                    if (pd_index + 1 > pd_length)
                    {
                        /*
                        //Need more bytes 
                        p_state = 1;
                        pd_needbytes = 1;
                        return;
                         */
                        throw new Exception("GameNetwork.ParseMessage: Т.е. было прочитано 0 байт. Блядь!");
                    }
                    else
                    {
                        p_tmp[0]=p_Data[pd_index]; /*p_tmp[1] = p_Data[pd_index + 2];
                        p_tmp[2] = p_Data[pd_index + 1]; p_tmp[3] = p_Data[pd_index];*/
                        pd_index += 1;
                    }
            	break;
                case 2:
                    if (pd_index + 2 > pd_length)
                    {
                        //Need more bytes 
                        p_state = 1;

                        pd_needbytes = pd_index + 2 - pd_length;
                        if (pd_needbytes == 1)//have 3
                        {
                            /*p_tmp[0]=p_Data[pd_index+3];*/p_tmp[1] = p_Data[pd_index];
                            //p_tmp[2] = p_Data[pd_index + 1]; p_tmp[3] = p_Data[pd_index];
                            pd_index += 1;
                        }
                        else
                            throw new Exception("GameNetwork.ParseMessage: Т.е. было прочитано 0 байт. Блядь!");
                        return;
                    }
                    else
                    {
                        p_tmp[1] = p_Data[pd_index]; p_tmp[0] = p_Data[pd_index+1];
                        pd_index += 2;
                    }
                break;

                case 3:
                if (pd_index + 3 > pd_length)
                {
                    //Need more bytes 
                    p_state = 1;

                    pd_needbytes = pd_index + 3 - pd_length;
                    if (pd_needbytes == 1)//have 3
                    {
                        p_tmp[1] = p_Data[pd_index];
                        p_tmp[0] = p_Data[pd_index+1];
                        pd_index += 2;
                    }
                    else
                    if (pd_needbytes == 2)
                    {
                        p_tmp[1] = p_Data[pd_index];
                        pd_index += 1;
                    }
                    else
                        throw new Exception("GameNetwork.ParseMessage: Т.е. было прочитано 0 байт. Блядь!");

                    return;
                }
                else
                {
                    p_tmp[2] = p_Data[pd_index]; p_tmp[1] = p_Data[pd_index+1]; p_tmp[0] = p_Data[pd_index + 2];
                    pd_index += 3;
                }
                break;

                default:
                    p_state = 113;
                    return;
            }
            #endregion

            p_itmp = BitConverter.ToInt32(p_tmp, 0);

            if (p_itmp > 0)
            {
                //Create New Message
                p_currentMsg.IsReaded = false;
                p_currentMsg.LifeTime = MaxReciveLifeTime;

                p_currentMsg.RawData = new byte[p_itmp];

                if (pd_index + p_itmp > pd_length)
                {
                    // Not enough data, need read more data
                    Array.Copy(p_Data, pd_index, p_currentMsg.RawData, 0, pd_length - pd_index);
                    pd_needbytes = p_itmp - pd_index - pd_length;
                    pd_readedIndex = pd_length - pd_index;
                    p_state = 3;
                }
                else
                {
                    //Enough data - complete message.
                    Array.Copy(p_Data, pd_index, p_currentMsg.RawData, 0, p_itmp);
                    pd_index += p_itmp;
                    p_state = 0;
                    return;
                }
            }
            else
            {
                p_state = 115;
                LastErrorMessage += "GameNetwork.ParseMessage: Message length <=0";
                return;
            }
        }
        else
        #endregion

        #region  State 3 - Continue read Data
        if (p_state == 3)
        {

            if (pd_index + pd_needbytes > pd_length)
            {
                Array.Copy(p_Data, pd_index, p_currentMsg.RawData, pd_readedIndex, pd_length - pd_index);
                pd_readedIndex += pd_length - pd_index;
                pd_needbytes = pd_needbytes - pd_index - pd_length;
                p_state = 3;
            }
            else
            {
                Array.Copy(p_Data, pd_index, p_currentMsg.RawData, pd_readedIndex, pd_needbytes);
                pd_index += pd_needbytes;
                p_state = 0;
                return;
            }
        }
        else
        #endregion

        #region  Wrong State        
        {
            p_state = 116;
            LastErrorMessage += "GameNetwork.ParseMessage:Wrong State ="+p_state.ToString();
            return;
        }
        #endregion
    }
#endregion

    LinkedListNode<SendingMessage> LastSending,PreLastSending;

    public static int NothincToWriteSleepTime = 100;
    public static int WasErrorSleepTime = 200;

    public bool NeedRestartWriteQueue=true;
    public int SendingLength = 0;

    byte[] s_IDtmp;
    byte[] s_tmp;

    void DoWriteMessages()
    {
        while (!NeedStopWrite)
        {
            
            if (IsError)
            {
                Thread.Sleep(WasErrorSleepTime);
                continue;
            }

           
            if (LastSending==null || NeedRestartWriteQueue)
            {
                lock(WriteLocker)
                {
                    LastSending=WriteMessages.First;
                    NeedRestartWriteQueue=false;
                }
                
                if (LastSending==null)
                {
                    Thread.Sleep(NothincToWriteSleepTime);
                    continue;
                }
            }

            //Find First not send message in Queue

            while (LastSending!=null && LastSending.Value.Status!=SendingMessageStatus.WaitOperation /*&&msg.livetime>0*/)
                LastSending=LastSending.Next;

            if (LastSending==null)
			{
				Thread.Sleep(NothincToWriteSleepTime);
                continue;
			}

            SendingMessage msg = LastSending.Value;
            if (msg == null)
            {
                LastErrorMessage = "GameNetwork: Message is null";
                IsError = true;
                continue;
            }

            #region Fromat Message

            if (msg.Data==null)
            {
                IsError = true;
                LastErrorMessage = "GameNetwork: Message.Data is Null";
                continue;
            }

            if (WriteBuffer.Length<4)
                WriteBuffer=new byte[4];

            try
            {
                SendingLength = 4;
                Term.term_to_binary(ref WriteBuffer, ref SendingLength, msg.Data);
                SendingLength -= 4;
            }
            catch (System.Exception ex)
            {
            	IsError = true;
                LastErrorMessage = "GameNetwork: Format message Error: "+ex.Message;
                continue;
            }

            if (WriteBuffer == null || WriteBuffer.Length == 0)
            {
                IsError = true;
                LastErrorMessage = "GameNetwork: WriteBuffer is Null or empty";
                continue;
            }

            //SendingLength=WriteBuffer.Length;
            byte [] len=BitConverter.GetBytes(SendingLength);

            WriteBuffer[0]=len[3];WriteBuffer[1]=len[2];WriteBuffer[2]=len[1];WriteBuffer[3]=len[0];
            #endregion

            if (!client.Connected)
            {
                IsError = true;
                LastErrorMessage = "GameNetwork.Write: connection breaks?";
                continue;
            }
            //Send Message
            try
            {
                msg.Status = SendingMessageStatus.InProgress;
                client.GetStream().Write(WriteBuffer, 0, SendingLength+4);
                msg.Status = SendingMessageStatus.Successful;
            }
            catch (System.Exception ex)
            {
                IsError = true;
                LastErrorMessage = "GameNetwork.Write: Send error = " + ex.Message;
                msg.Status = SendingMessageStatus.Failed;
            }
        }
    }

    public void Update()
    {
        //Recive
        lock (ReadLocker)
        {
            if (ReadMessages.Count > 0)
            {
                foreach (ReceivedMessage rm in ReadMessages)
                {
                    MessagesToRead.AddLast(rm);
                }
                ReadMessages.Clear();
            }
        }
        //Send
        lock (WriteLocker)
        {
            if (MessagesToSend.Count > 0)
            {
                foreach (SendingMessage sm in MessagesToSend)
                {
                    WriteMessages.AddLast(sm);
                }

                MessagesToSend.Clear();
            }
        }
        //Update recevied

        foreach (ReceivedMessage rmsg in MessagesToRead)
        {
            if (!rmsg.IsReaded)
            {
                rmsg.Message.OnReciveMessage(rmsg);
            }
        }
    }
}


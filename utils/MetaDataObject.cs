using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

//Для редактирования свойств игровых объектов (на примере для Unity), иногда сложно
// использовать игровые редакторы. Сложность может заключаться в их отсутствии, 
//сложности их испоользования и пр. (в частности в Unity сложно редактировать объекты, классов 
//не наследуемых от MonoBehaviour) вот для этих случаем и используются MedaDataObjects
//
//Классы из этого набора предназначены для сбора сведений (Метаданных) об классах
//для объектов которых необходимо организовать редактирование. Сбор осуществляется
//с помощю атрибутов (MetaDataClassAttribute, MetaDataFieldAttribute).
//
// Пример использоания аттрибутов:
//
//[MetaDataClassAttribute("Класс описывающий игровой корабль.")]
//public class GameShip:IShowable,ISimpleSerelize
//{
//	[MetaDataFieldAttribute("Номер корабля во флотилии игрока")]
//	public int ID;
//	
//	[MetaDataFieldAttribute("Имя корабля (мах 16 симв.)")]
//	public string Name;
//
//	[MetaDataFieldAttribute(TypeOfField.SimpleObject,"Текущие параметры корабля")]
//	public ShipProperties CurrentParams;
//
//Далее на основании собранных данных строится метакласс (MetaDataClass)
//содержащий помеченные атрибутами поля (MetaDataField).
//Все данные о метаклассах легко сереализуются\десереализуются.
//Затем по заданному Метаклассу можно создать Метаобъект.
//
//Метаобъект содержит список значений полей класса. Значение может быть
//представлено одним из 4-х вариантов:
// * Строка - для простых типов (ID,Name)
// * Список строк - для списков из простых типов
// * МетаОбъект - для Полей исходного класса (GameShip) являющихся ссылками на объекты
// * Список Метаобъектов - для Полей исходногок ласса являющихся списками ссылок на объекты.
// 
//Для каждого метакласса на лету элементарно генерируется форома для редактирования 
//Метаобъекта этого метакласса.
//
//Каждый метаобъект легко сереализуется / десереализуется в формат текстовый 
// формат XML \  ТеxtBlock \ JSON \...
//
// Итак мы получаем легко расширяемый \ изменяемый редактор полей для любого
// .Net класса. 
// И что дальше?
//
// Далее уже не метаобъект, а объект (например класса GameShip) может легко считывать
// данные записанные в текстовый файл (XML \  ТеxtBlock \ JSON \...) метообъектом.

[AttributeUsage(AttributeTargets.Field)]
public class MetaDataFieldAttribute:System.Attribute
{
	public TypeOfField fieldType;
	public Type SpetialType;
    public string Description;
	
	public MetaDataFieldAttribute()
	{
		fieldType=TypeOfField.SimpleType;
		SpetialType=null;
        Description = "";
	}

    public MetaDataFieldAttribute(string descr)
    {
        fieldType = TypeOfField.SimpleType;
        SpetialType = null;
        Description = descr;
    }
	
	public MetaDataFieldAttribute(Type Spetial)
	{
		fieldType=TypeOfField.SimpleType;
		SpetialType=Spetial;
        Description = "";
	}
	
	public MetaDataFieldAttribute(TypeOfField FieldType)
	{
		fieldType=FieldType;
		SpetialType=null;
        Description = "";
	}
	
	public MetaDataFieldAttribute(TypeOfField FieldType,Type Spetial)
	{
		fieldType=FieldType;
		SpetialType=Spetial;
        Description = "";
	}

    /////////////////////
    public MetaDataFieldAttribute(Type Spetial,string descr)
    {
        fieldType = TypeOfField.SimpleType;
        SpetialType = Spetial;
        Description = descr;
    }

    public MetaDataFieldAttribute(TypeOfField FieldType, string descr)
    {
        fieldType = FieldType;
        SpetialType = null;
        Description = descr;
    }

    public MetaDataFieldAttribute(TypeOfField FieldType, Type Spetial, string descr)
    {
        fieldType = FieldType;
        SpetialType = Spetial;
        Description = descr;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class MetaDataClassAttribute:Attribute
{
	public bool IsInstanced;
    public string Description;

    public MetaDataClassAttribute(bool isInstanced)
	{
        IsInstanced = isInstanced;
        Description = "";
	}
	
	public MetaDataClassAttribute()
	{
		IsInstanced=false;
        Description = "";
	}

    public MetaDataClassAttribute(bool isInstanced,string descr)
    {
        IsInstanced = isInstanced;
        Description = descr;
    }

    public MetaDataClassAttribute(string descr)
    {
        IsInstanced = false;
        Description = descr;
    }
}


public class MetaDataField
{
	public string Name;
	public string ClassName;
    public string Description;
	public TypeOfField FieldType;	
	
	public void SaveToBlock(ETF_TextBlock block)
	{
		if (!string.IsNullOrEmpty(Name))
			block.SetAttribute("Name",Name);
		if (!string.IsNullOrEmpty(ClassName))
			block.SetAttribute("ClassName",ClassName);
        if (!string.IsNullOrEmpty(Description))
            block.SetAttribute("Description", Description);
		block.SetAttribute("FieldType",((int)FieldType).ToString());
	}
	
	public void LoadFromBlock(ETF_TextBlock block)
	{
		Name=block.GetAttribute("Name","");
		ClassName=block.GetAttribute("ClassName","");
        Description = block.GetAttribute("Description", "");
		FieldType=(TypeOfField)int.Parse(block.GetAttribute("FieldType","0"));
	}
}

public enum TypeOfField
{
	SimpleType=1,//-> Name = "value"
	SimpleObject=2,//-> FieldName { objectField="value"\n...}
	
	SimpleList=3,//-> FieldName { count=10\n i1=1 \n i2=...i10=12323\n }
	ListOfObject=4//-> FieldName{ ClassName1 { ObjectField="value"\n...} \n ClassName2{...}...}
}

public class MetaDataClass
{
	public string Name;
    public string Description;
    public bool IsInstanced;
    public MetaDataObject Instance;
	public List<MetaDataField> Fields;
	
	public MetaDataClass()
	{
		Fields=new List<MetaDataField>();
        IsInstanced = false;
        Description = "";
	}
	
	public MetaDataClass(string aName)
	{
		Name=aName;
        IsInstanced = false;
		Fields=new List<MetaDataField>();
        Description = "";
	}
	
	public void SaveToBlock(ETF_TextBlock block)
	{
		block.SetAttribute("Name",Name);
        block.SetAttribute("Description", Description);
        block.SetAttribute("IsInstanced", IsInstanced.ToString());
		ETF_TextBlock tb=block.AddChild("Fields");
		
		foreach(MetaDataField field in Fields)
		{
			field.SaveToBlock(tb.AddChild("Field"));	
		}
	}

    public void LoadFromBlock(ETF_TextBlock block)
    {
        Name = block.GetAttribute("Name", "");
        Description = block.GetAttribute("Description", "");
        if (bool.Parse(block.GetAttribute("IsInstanced", bool.FalseString)))
        {
            IsInstanced = true;
        }
        else
            IsInstanced = false;
        ETF_TextBlock tb = block.FindChild("Fields");
        Fields.Clear();
        if (tb != null)
        {
            MetaDataField field;
            foreach (ETF_TextBlock t in tb.Children)
            {
                field = new MetaDataField();
                field.LoadFromBlock(t);
                Fields.Add(field);
            }
        }

        if (IsInstanced)
            Instance = new MetaDataObject(this);
    }

    public string PrintMetaDataAsClass()
    {
        string result="";
        if (!string.IsNullOrEmpty(Description))
            foreach (string s in Description.Split('\n'))
                result = result + DescrToComment(Description, "//", "\n");

        result = result +"class " + Name + "\n{";
        if (IsInstanced)
        {
            result = result + "\n\tstatic " + Name + " Instance;\n";
        }
        foreach (MetaDataField f in Fields)
        {
            switch (f.FieldType)
            {
            case TypeOfField.SimpleType:                    
                 result = result + DescrToComment(f.Description,"\n\t//","")+"\n\t" + f.ClassName + " " + f.Name+";";
            	break;
            case TypeOfField.SimpleObject:
                result = result + DescrToComment(f.Description, "\n\t//", "") + "\n\t" + f.ClassName + " " + f.Name + ";";
                break;
            case TypeOfField.SimpleList:
                result = result + DescrToComment(f.Description, "\n\t//", "") + "\n\tList<" + f.ClassName + "> " + f.Name + ";";
                break;
            case TypeOfField.ListOfObject:
                result = result + DescrToComment(f.Description, "\n\t//", "") + "\n\tList<" + f.ClassName + "> " + f.Name + ";";
                break;
            }
        }
        result = result + "\n}";
        return result;
    }

    string DescrToComment(string descr,string prefix,string postfix)
    {
        string result="";
        if (string.IsNullOrEmpty(descr)) return "";
        foreach (string s in descr.Split('\n'))
        {
            result = result + prefix + s + postfix;
        }
        return result;
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(Name))
            return base.ToString();
        return Name;
    }
}

public class MetaDataValue
{
	object Value;
	public string AsSimpleValue
	{
		get {return (string)Value;}
		set {Value=value;}
	}
	public MetaDataObject AsSimpleObject
	{
		get {return (MetaDataObject)Value;}
		set {Value=value;}
	}
	public List<String> AsSimpleList
	{
		get {return (List<String>)Value;}
		set {Value=value;}
	}
	public List<MetaDataObject> AsListOfObjects
	{
		get {return (List<MetaDataObject>)Value;}
		set {Value=value;}
	}
}

public class MetaDataObject
{
	public MetaDataObject(MetaDataClass ClassInfo)
	{
		Info=ClassInfo;
		Values=new Dictionary<MetaDataField, MetaDataValue>();
		ConstructValuesByClassInfo();
	}
	
	public MetaDataObject(string ClassName)
	{
		Info=MetaDataUsing.GetOrCreateClass(ClassName);
		Values=new Dictionary<MetaDataField, MetaDataValue>();
		ConstructValuesByClassInfo();
	}
	
	public void ConstructValuesByClassInfo()
	{
		foreach(MetaDataField field in Info.Fields)
		{
            Values.Add(field, new MetaDataValue());
		}
	}
	
	public Dictionary<MetaDataField,MetaDataValue> Values;
	public MetaDataClass Info;
	
	public void SaveToBlock(ETF_TextBlock block)
	{
		ETF_TextBlock tb;
		MetaDataObject mdo;
		string s;
		foreach(MetaDataField finfo in Values.Keys)
		{
			switch(finfo.FieldType)
			{
			case TypeOfField.SimpleType:
					s=Values[finfo].AsSimpleValue;
					if (!string.IsNullOrEmpty(s))
						block.SetAttribute(finfo.Name,s);
					break;
			case TypeOfField.SimpleObject:
					mdo=Values[finfo].AsSimpleObject;
					if (mdo!=null)
						mdo.SaveToBlock(block.AddChild(finfo.Name));
					break;
			case TypeOfField.SimpleList:
                    if (Values[finfo].AsSimpleList != null)
                    {
                        tb = block.AddChild(finfo.Name);
                        List<string> list = Values[finfo].AsSimpleList;
                        tb.SetAttribute("Count", list.Count.ToString());
                        for (int i = 0; i < list.Count; i++)
                            if (!string.IsNullOrEmpty(list[i]))
                                tb.SetAttribute("i" + i.ToString(), list[i]);
                    }
					break;
			case TypeOfField.ListOfObject:
                    if (Values[finfo].AsListOfObjects != null)
                    {
                        tb = block.AddChild(finfo.Name);
                        List<MetaDataObject> o_list = Values[finfo].AsListOfObjects;
                        for (int i = 0; i < o_list.Count; i++)
                            if (o_list[i] != null)
                                o_list[i].SaveToBlock(tb.AddChild("item" + i.ToString()));
                    }
					break;
			}
		}
	}
	
	public void LoadFromBlock(ETF_TextBlock block)
	{
		ETF_TextBlock tb;
		MetaDataObject mdo;
		foreach(MetaDataField finfo in Values.Keys)
		{
			switch(finfo.FieldType)
			{
			case TypeOfField.SimpleType:
					Values[finfo].AsSimpleValue=block.GetAttribute(finfo.Name,"");
					break;
			case TypeOfField.SimpleObject:					
					tb=block.FindChild(finfo.Name);
					if (tb!=null)
					{
						mdo=new MetaDataObject(finfo.ClassName);
						mdo.LoadFromBlock(tb);
						Values[finfo].AsSimpleObject=mdo;
					}
					else
						Values[finfo]=null;
					break;
			case TypeOfField.SimpleList:
					tb=block.FindChild(finfo.Name);
					if (tb!=null)
					{
						int count=int.Parse(tb.GetAttribute("Count","0"));
						List<string> list=new List<string>(count);
						for(int i=0;i<count;i++)
							list.Add(tb.GetAttribute("i"+i.ToString()));
					}
					else
						Values[finfo].AsSimpleList=null;
					break;
			case TypeOfField.ListOfObject:
					tb=block.FindChild(finfo.Name);
					if (tb!=null)
					{
						List<MetaDataObject> list=new List<MetaDataObject>();
						MetaDataClass cls=MetaDataUsing.GetOrCreateClass(finfo.ClassName);
						for(int i=0;i<tb.Children.Count;i++)
						{
							mdo=new MetaDataObject(cls);
							mdo.LoadFromBlock(tb.Children[i]);
							list.Add(mdo);
						}
						Values[finfo].AsListOfObjects=list;
					}
					else
						Values[finfo].AsListOfObjects=null;
					break;
			}
		}
	}

    public override string ToString()
    {
        return Info.Name;
    }
}

public class MetaDataUsing
{
	public static Dictionary<string,MetaDataClass> Classes=new Dictionary<string, MetaDataClass>();
    public static Dictionary<string, MetaDataClass> Instances = new Dictionary<string, MetaDataClass>();
	
	public static MetaDataClass GetOrCreateClass(string ClassName)
	{
		MetaDataClass cls=Classes[ClassName];
		if (cls==null)
		{
			cls=new MetaDataClass();
			cls.Name=ClassName;
			Classes.Add(ClassName,cls);
            if (cls.IsInstanced)
                Instances.Add(ClassName, cls);
		}
		return cls;		
	}
	
	public static void ReplaceOrRegisterClass(MetaDataClass cls)
	{
        if (Classes.ContainsKey(cls.Name))
        {
            Classes[cls.Name] = cls;
            if (cls.IsInstanced)
                Instances[cls.Name] = cls;
        }
        else
        {
            Classes.Add(cls.Name, cls);
            if (cls.IsInstanced)
                Instances.Add(cls.Name, cls);
        }
	}
	
	public static void Init(out string err)
	{
		err="";
		if (Classes==null) Classes=new Dictionary<string, MetaDataClass>();
        if (Instances == null) Instances = new Dictionary<string, MetaDataClass>();
		object [] objs;
		MetaDataClass cls;
		string terr;
		foreach(Type t in typeof(MetaDataUsing).Assembly.GetTypes())
		{
			if (t.IsClass)
			{
				objs=t.GetCustomAttributes(typeof(MetaDataClassAttribute),true);
				if (objs.Length>0)
				{
					terr="";
					cls=BuildMetaDataClass(t,out terr);
                    
					if (cls!=null)
						ReplaceOrRegisterClass(cls);
					else
						err+=terr+"\n";
				}
			}
		}
	}	
	public static MetaDataClass BuildMetaDataClass(Type ClassFor,out string Err)
	{
		Err="";
		if (!ClassFor.IsClass)
		{
			Err="Type="+ClassFor.Name+", Is Not Class";
			return null;
		}
		object[] attrs=ClassFor.GetCustomAttributes(typeof(MetaDataClassAttribute),true);
		MetaDataClassAttribute classAttr=null;
		if (attrs.Length!=1)
	    {
			Err="Type="+ClassFor.Name+", not contain [MetaDataClassAttribute],\n or contain more then one such attribute.";
			return null;
		}
		classAttr=(MetaDataClassAttribute)attrs[0];//not used
		
		MetaDataClass mdc=new MetaDataClass(ClassFor.Name);
        mdc.IsInstanced = classAttr.IsInstanced;
        mdc.Description = classAttr.Description;
		object [] fattrs;
		MetaDataField field;
		foreach(FieldInfo finfo in ClassFor.GetFields())
		{
			//fattrs finfo.GetCustomAttributes
			fattrs=finfo.GetCustomAttributes(typeof(MetaDataFieldAttribute),true);
			if (fattrs.Length>0)
			{
				field=new MetaDataField();
				field.Name=finfo.Name;
                field.Description = ((MetaDataFieldAttribute)fattrs[0]).Description;
				field.FieldType=((MetaDataFieldAttribute)fattrs[0]).fieldType;
								
				if (((MetaDataFieldAttribute)fattrs[0]).SpetialType==null)
				{
					field.ClassName=finfo.FieldType.Name;
				}
				else
					field.ClassName=((MetaDataFieldAttribute)fattrs[0]).SpetialType.Name;
				
				mdc.Fields.Add(field);
			}
		}
		return mdc;
	}
	
	public static void BuildMetaDataAndSaveToFile(string FileName,out string Err)
	{
		if (Classes==null)
			Classes=new Dictionary<string, MetaDataClass>();
        if (Instances == null)
            Instances = new Dictionary<string, MetaDataClass>();
		Err="";
		string terr;
		object [] objs;
		MetaDataClass cls;
		foreach(Type t in typeof(MetaDataUsing).Assembly.GetTypes())
		{
			if (t.IsClass)
			{
				objs=t.GetCustomAttributes(typeof(MetaDataClassAttribute),true);
				if (objs.Length>0)
				{
					terr="";
					cls=BuildMetaDataClass(t,out terr);
					if (cls==null)
						Err=Err+terr+"\n";
					else
						ReplaceOrRegisterClass(cls);
				}
			}
		}
		//all classes builded
		//save
		StreamWriter w;
		if (File.Exists(FileName))
	    {
			w=new StreamWriter(File.OpenWrite(FileName));			
		}
		else
			w=new StreamWriter(File.Create(FileName));
		
		ETF_TextBlock block=new ETF_TextBlock();
		foreach(MetaDataClass mdc in Classes.Values)
		{
			mdc.SaveToBlock(block.AddChild("class"));
		}
		
		w.Write(block.DumpToString());
		w.Close();
	}

    public static void LoadMetaDataFromFile(string FileName, out string Err)
    {
        if (Classes == null)
            Classes = new Dictionary<string, MetaDataClass>();
        if (Instances == null)
            Instances = new Dictionary<string, MetaDataClass>();

        Err = "";
        if (!File.Exists(FileName)) { Err="File="+FileName+" not found";return; }
        StreamReader r = new StreamReader(File.OpenRead(FileName));
        ETF_TextBlock block = ETF_TextBlock.Parse(r.ReadToEnd(), out Err);
        if (block == null || !string.IsNullOrEmpty(Err)) return;

        MetaDataClass cls;
        foreach (ETF_TextBlock tb in block.Children)
        {
            if (tb.Name != "class") continue;
            cls = new MetaDataClass();
            cls.LoadFromBlock(tb);
            Classes.Add(cls.Name, cls);
            if (cls.IsInstanced)
                Instances.Add(cls.Name, cls);
        }
    }
}
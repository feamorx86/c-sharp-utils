using System;
using System.Collections.Generic;

public static class Sorting
{
	#region QSort
	//
	//	QSort Arrays 
	//
	
	static int  qsort_partition_array<T>( T[] m, int a, int b) where T :IComparable<T>
	{
	    int i = a;
	    for (int j = a; j <= b; j++)         // просматриваем с a по b
	    {        
	        if (m[j].CompareTo( m[b]) <= 0)  // если элемент m[j] не превосходит m[b],
	        {
	            T t = m[i];                  // меняем местами m[j] и m[a], m[a+1], m[a+2] и так далее...
	            m[i] = m[j];                 // то есть переносим элементы меньшие m[b] в начало,
	            m[j] = t;                    // а затем и сам m[b] «сверху»
	            i++;                         // таким образом последний обмен: m[b] и m[i], после чего i++
	        }
	    }
	    return i - 1;                        // в индексе i хранится <новая позиция элемента m[b]> + 1
	}
	
	static int  qsort_partition_array<T>( T[] m, int a, int b,Comparison<T> comparer)
	{
	    int i = a;
	    for (int j = a; j <= b; j++)        
	    {        
	        if (comparer(m[j],m[b]) <= 0)  
	        {
	            T t = m[i];                
	            m[i] = m[j];               
	            m[j] = t;                  
	            i++;                       
	        }
	    }
	    return i - 1;                      
	}
	
	 
	public static void QuickSort<T>( T[] Data, int From, int To) where T : IComparable<T>
	{                                       
	    if (From >= To) return;
	    int c = qsort_partition_array<T>( Data, From, To);
	    QuickSort( Data, From, To - 1);
	    QuickSort( Data, c + 1, To);
	}
	
	public static void QuickSort<T>(T[] Data) where T:IComparable<T>
	{
		QuickSort<T>(Data,0,Data.Length-1);	
	}
	
	public static void QuickSort<T>( T[] Data, int From, int To,Comparison<T> comparer)
	{                                       
	    if (From >= To) return;
	    int c = qsort_partition_array<T>( Data, From, To,comparer);
	    QuickSort( Data, From, To - 1,comparer);
	    QuickSort( Data, c + 1, To,comparer);		
	}
	
	public static void QuickSort<T>(T[] Data,Comparison<T> comparer)
	{
		QuickSort<T>(Data,0,Data.Length-1,comparer);	
	}
	
	//
	//	QSort List
	//
	
	static int  qsort_partition_list<T>( List<T>m, int a, int b) where T :IComparable<T>
	{
	    int i = a;
	    for (int j = a; j <= b; j++)         // просматриваем с a по b
	    {        
	        if (m[j].CompareTo( m[b]) <= 0)  // если элемент m[j] не превосходит m[b],
	        {
	            T t = m[i];                  // меняем местами m[j] и m[a], m[a+1], m[a+2] и так далее...
	            m[i] = m[j];                 // то есть переносим элементы меньшие m[b] в начало,
	            m[j] = t;                    // а затем и сам m[b] «сверху»
	            i++;                         // таким образом последний обмен: m[b] и m[i], после чего i++
	        }
	    }
	    return i - 1;                        // в индексе i хранится <новая позиция элемента m[b]> + 1
	}
	
	static int  qsort_partition_list<T>( List<T> m, int a, int b,Comparison<T> comparer)
	{
	    int i = a;
	    for (int j = a; j <= b; j++)        
	    {        
	        if (comparer(m[j],m[b]) <= 0)  
	        {
	            T t = m[i];                
	            m[i] = m[j];               
	            m[j] = t;                  
	            i++;                       
	        }
	    }
	    return i - 1;                      
	}
	
	public static void QuickSort<T>( List<T> Data, int From, int To) where T : IComparable<T>
	{                                       
	    if (From >= To) return;
	    int c = qsort_partition_list<T>( Data, From, To);
	    QuickSort( Data, From, To - 1);
	    QuickSort( Data, c + 1, To);
	}
	
	public static void QuickSort<T>(List<T> Data) where T:IComparable<T>
	{
		QuickSort<T>(Data,0,Data.Count-1);	
	}
	
	public static void QuickSort<T>( List<T> Data, int From, int To,Comparison<T> comparer)
	{                                       
	    if (From >= To) return;
	    int c = qsort_partition_list<T>( Data, From, To,comparer);
	    QuickSort( Data, From, To - 1,comparer);
	    QuickSort( Data, c + 1, To,comparer);		
	}
	
	public static void QuickSort<T>(List<T> Data,Comparison<T> comparer)
	{
		QuickSort<T>(Data,0,Data.Count-1,comparer);	
	}
	#endregion
	
	#region BinarySearch
	
	public static bool BinarySearch<T>(T[] Data,ref T Result,BinarySearchPredicate<T> condition)
	{
		//Спасибо википедии родной, 
		//Пиздой не накрылся выходной))
    	int first = 0;				/* Первый элемент в массиве */
	    int last = Data.Length;		/* Элемент в массиве, СЛЕДУЮЩИЙ ЗА последним */
	                                /* Если просматриваемый участок непустой, first<last */
	    int mid; 
	 
	    if (last == 0)
	    {
	         return false;	/* массив пуст */
	    } 
	    else if (condition(Data[0])<0)// if (a[0] > x)
	    {
			return false;
	        /* не найдено; если вам надо вставить его со сдвигом - то в позицию 0    */
	    } 
	    else if (condition(Data[Data.Length - 1])>0)//a[n - 1] < x
	    {
			return false;
	        /* не найдено; если вам надо вставить его со сдвигом - то в позицию n    */
	    }
	 
	    while (first < last)
	    {
	        /* ВНИМАНИЕ! В отличие от более простого (first+last)/2, этот код стоек к переполнениям.
	           Если first и last знаковые, возможен код (unsigned)(first+last) >> 1.    */
	        mid = first + (last - first) / 2;
	 
	        if (condition(Data[mid])<=0)
	        {
	            last = mid;
	        }
	        else
	        {
	            first = mid + 1;
	        }
	    }
	 
	    /* Если проверка n==0 в начале опущена - значит, тут раскомментировать!    */
	    if (condition(Data[last]) == 0)
	    {
	        Result=Data[last];
			return true;
			/* Искомый элемент найден. last - искомый индекс */
	    } else
	    {
	        return false;
			/* Искомый элемент не найден. Но если вам вдруг надо его вставить со сдвигом, то его место - last.    */
	    }
	}
	
	public static bool BinarySearch<T>(List<T> Data,ref T Result,BinarySearchPredicate<T> condition)
	{
    	int first = 0;			
	    int last = Data.Count;	
	    int mid; 
	 
	    if (last == 0)
	         return false;
	    else if (condition(Data[0])<0)
			return false;
	    else if (condition(Data[Data.Count - 1])>0)
			return false;
	 
	    while (first < last)
	    {
	        mid = first + (last - first) / 2;
	 
	        if (condition(Data[mid])<=0)
	            last = mid;
	        else
	            first = mid + 1;
	    }
	 
	    if (condition(Data[last]) == 0)
	    {
	        Result=Data[last];
			return true;
	    } 
		else
	        return false;
	}
	
	public static bool BinarySearch<T>(T [] Data,ref int ResultIndex,BinarySearchPredicate<T> condition)
	{
    	int first = 0;			
	    int last = Data.Length;	
	    int mid; 
	 
	    if (last == 0)
	         return false;
	    else if (condition(Data[0])<0)
			return false;
	    else if (condition(Data[Data.Length - 1])>0)
			return false;
	 
	    while (first < last)
	    {
	        mid = first + (last - first) / 2;
	 
	        if (condition(Data[mid])<=0)
	            last = mid;
	        else
	            first = mid + 1;
	    }
	 
	    if (condition(Data[last]) == 0)
	    {
	        ResultIndex=last;
			return true;
	    } 
		else
	        return false;
	}
	
	public static bool BinarySearch<T>(List<T> Data,ref int ResultIndex,BinarySearchPredicate<T> condition)
	{
    	int first = 0;			
	    int last = Data.Count;	
	    int mid; 
	 
	    if (last == 0)
	         return false;
	    else if (condition(Data[0])<0)
			return false;
	    else if (condition(Data[Data.Count - 1])>0)
			return false;
	 
	    while (first < last)
	    {
	        mid = first + (last - first) / 2;
	 
	        if (condition(Data[mid])<=0)
	            last = mid;
	        else
	            first = mid + 1;
	    }
	 
	    if (condition(Data[last]) == 0)
	    {
	        ResultIndex=last;
			return true;
	    } 
		else
	        return false;
	}
	
	public static bool BinarySearch<T>(T[] Data,T item,ref T Result,Comparison<T> condition)
	{
		//Спасибо википедии родной, 
		//Пиздой не накрылся выходной))
    	int first = 0;				/* Первый элемент в массиве */
	    int last = Data.Length;		/* Элемент в массиве, СЛЕДУЮЩИЙ ЗА последним */
	                                /* Если просматриваемый участок непустой, first<last */
	    int mid; 
	 
	    if (last == 0)
	    {
	         return false;	/* массив пуст */
	    } 
	    else if (condition(item,Data[0])<0)// if (a[0] > x)
	    {
			return false;
	        /* не найдено; если вам надо вставить его со сдвигом - то в позицию 0    */
	    } 
	    else if (condition(item,Data[Data.Length - 1])>0)//a[n - 1] < x
	    {
			return false;
	        /* не найдено; если вам надо вставить его со сдвигом - то в позицию n    */
	    }
	 
	    while (first < last)
	    {
	        /* ВНИМАНИЕ! В отличие от более простого (first+last)/2, этот код стоек к переполнениям.
	           Если first и last знаковые, возможен код (unsigned)(first+last) >> 1.    */
	        mid = first + (last - first) / 2;
	 
	        if (condition(item,Data[mid])<=0)
	        {
	            last = mid;
	        }
	        else
	        {
	            first = mid + 1;
	        }
	    }
	 
	    /* Если проверка n==0 в начале опущена - значит, тут раскомментировать!    */
	    if (condition(item,Data[last]) == 0)
	    {
	        Result=Data[last];
			return true;
			/* Искомый элемент найден. last - искомый индекс */
	    } else
	    {
	        return false;
			/* Искомый элемент не найден. Но если вам вдруг надо его вставить со сдвигом, то его место - last.    */
	    }
	}
	
	public static bool BinarySearch<T>(List<T> Data,T item, ref T Result,Comparison<T> condition)
	{
    	int first = 0;			
	    int last = Data.Count;	
	    int mid; 
	 
	    if (last == 0)
	         return false;
	    else if (condition(item,Data[0])<0)
			return false;
	    else if (condition(item,Data[Data.Count - 1])>0)
			return false;
	 
	    while (first < last)
	    {
	        mid = first + (last - first) / 2;
	 
	        if (condition(item,Data[mid])<=0)
	            last = mid;
	        else
	            first = mid + 1;
	    }
	 
	    if (condition(item,Data[last]) == 0)
	    {
	        Result=Data[last];
			return true;
	    } 
		else
	        return false;
	}
	
	public static bool BinarySearch<T>(T [] Data,T item,ref int ResultIndex,Comparison<T> condition)
	{
    	int first = 0;			
	    int last = Data.Length;	
	    int mid; 
	 
	    if (last == 0)
	         return false;
	    else if (condition(item,Data[0])<0)
			return false;
	    else if (condition(item,Data[Data.Length - 1])>0)
			return false;
	 
	    while (first < last)
	    {
	        mid = first + (last - first) / 2;
	 
	        if (condition(item,Data[mid])<=0)
	            last = mid;
	        else
	            first = mid + 1;
	    }
	 
	    if (condition(item,Data[last]) == 0)
	    {
	        ResultIndex=last;
			return true;
	    } 
		else
	        return false;
	}
	
	public static bool BinarySearch<T>(List<T> Data,T item, ref int ResultIndex,Comparison<T> condition)
	{
    	int first = 0;			
	    int last = Data.Count;	
	    int mid; 
	 
	    if (last == 0)
	         return false;
	    else if (condition(item,Data[0])<0)
			return false;
	    else if (condition(item,Data[Data.Count - 1])>0)
			return false;
	 
	    while (first < last)
	    {
	        mid = first + (last - first) / 2;
	 
	        if (condition(item,Data[mid])<=0)
	            last = mid;
	        else
	            first = mid + 1;
	    }
	 
	    if (condition(item,Data[last]) == 0)
	    {
	        ResultIndex=last;
			return true;
	    } 
		else
	        return false;
	}

	/// <summary>
	///Делегат бинарного поиска.
	///T obj - очередной элемент из массива,
	///Если искомый равен  очередному, результат поиска ==  0 
	///Если искомый больше очередного, результат поиска ==  1 
	///Если искомый меньшн очередного, результат поиска == -1 
	/// </summary>
	public delegate int BinarySearchPredicate<T> (T obj);
	#endregion		
	
	#region BinaryIncert
	
	public static int BinaryInsert<T>(ref List<T> Data,T item,BinarySearchPredicate<T> condition)
	{
        int first = 0; 			/* Первый элемент в массиве */
        int last = Data.Count;  /* Элемент в массиве, СЛЕДУЮЩИЙ ЗА последним */
        						/* Если просматриваемый участок непустой, first<last */
        int mid;

        if (Data.Count == 0)
        {
            Data.Add(item);
			return 0;
        }
        else
        if (condition(Data[0])<=0)  //(a[0] > x)
        {
            Data.Insert(0, item);
            return 0;
            /* не найдено; если вам надо вставить его со сдвигом - то в позицию 0    */
        }
        else if (condition(Data[last - 1])>=0)
        {
            Data.Add(item);
            return Data.Count-1;
            /* не найдено; если вам надо вставить его со сдвигом - то в позицию n    */
        }

        while (first < last)
        {
            /* ВНИМАНИЕ! В отличие от более простого (first+last)/2, этот код стоек к переполнениям.
               Если first и last знаковые, возможен код (unsigned)(first+last) >> 1.    */
            mid = first + (last - first) / 2;

            if (condition(Data[mid])<=0)
            {
                last = mid;
            }
            else
            {
                first = mid + 1;
            }
        }
        Data.Insert(last,item);
		return last;
	}
	
	//Insert iten instend of First Operand in condition
	public static int BinaryInsert<T>(ref List<T> Data,T item,Comparison<T> condition)
	{
        int first = 0; 			/* Первый элемент в массиве */
        int last = Data.Count;  /* Элемент в массиве, СЛЕДУЮЩИЙ ЗА последним */
        						/* Если просматриваемый участок непустой, first<last */
        int mid;

        if (Data.Count == 0)
        {
            Data.Add(item);
			return 0;
        }
        else
        if (condition(item,Data[0])<=0)  //(a[0] > x)
        {
            Data.Insert(0, item);
            return 0;
            /* не найдено; если вам надо вставить его со сдвигом - то в позицию 0    */
        }
        else if (condition(item,Data[last - 1])>=0)
        {
            Data.Add(item);
            return Data.Count-1;
            /* не найдено; если вам надо вставить его со сдвигом - то в позицию n    */
        }

        while (first < last)
        {
            /* ВНИМАНИЕ! В отличие от более простого (first+last)/2, этот код стоек к переполнениям.
               Если first и last знаковые, возможен код (unsigned)(first+last) >> 1.    */
            mid = first + (last - first) / 2;

            if (condition(item,Data[mid])<=0)
            {
                last = mid;
            }
            else
            {
                first = mid + 1;
            }
        }
        Data.Insert(last,item);
		return last;
	}
	#endregion		
	
	#region sorted Linked List insert
	//Вставляет элемент в связныйсписок, не нарушая порядка сортировки.
	//Если встречается однотипная (cond==0) группа, элемент вставляется 
	//в конец группы 
	//ex: 2 -> 1,2,2,2,3 => 1,2,2,2,(new 2),3
	//Проходит по списку пока cond не станет -1
	/*public static LinkedListNode<T> SortedLinkedListInsert_End<T>(ref LinkedList<T> Data,T item,Comparison<T> condition)
	{
		LinkedListNode<T> i=Data.First;
		while (i!=null)
		{
			if (condition(item,i.Value)!=-1) //item не более i-го
				i=i.Next;
			else
				return Data.AddAfter(i,item);
				
		}
		return Data.AddLast(item);
	}*/
	
	//Вставляет элемент в связныйсписок, не нарушая порядка сортировки.
	//Если встречается однотипная (cond==0) группа, элемент вставляется 
	//в начало группы 
	//ex: 2 -> 1,2,2,2,3 => 1,(new 2),2,2,2,3
	//Проходит по списку пока cond равно -1
	public static LinkedListNode<T> SortedLinkedListInsert_End<T>(ref LinkedList<T> Data,T item,Comparison<T> condition)
	{
		LinkedListNode<T> i=Data.First;
		while (i!=null)
		{
			if (condition(item,i.Value)==-1) //i-й item не более i-го
				i=i.Next;
			else
				return Data.AddAfter(i,item);
				
		}
		return Data.AddLast(item);
	}
	#endregion

    #region  Hash

    public static UInt32 MurmurHash2 ( byte [] data)
    {
        uint m = 0x5bd1e995;
        uint seed = 0;
        uint r = 24;
        uint len=(uint)data.Length;
        
        uint h = seed ^ len;
 
        //const unsigned char * data = (const unsigned char *)key;

        uint i = 0;
        while (len >= 4)
        {
            uint k;
 
            k  = data[i];
            k |= (uint)(data[i+1] << ((byte)8));
            k |= (uint)(data[i+2] << ((byte)16));
            k |= (uint)(data[i+3] << ((byte)24));

            i += 4;

            k *= m;
            k ^= k >> ((byte)r);
            k *= m;
 
            h *= m;
            h ^= k;
 
            len -= 4;
        }
 
        switch (len)
        {
            case 3:
                h ^= (uint)(data[2] << ((byte)16));
                break;
            case 2:
                h ^= (uint)(data[1] << ((byte)8));
                break;
            case 1:
                h ^= data[0];
                h *= m;
                break;
        };
 
        h ^= h >> 13;
        h *= m;
        h ^= h >> 15;
 
        return h;
    }

    public static UInt32 MurmurHash2( char[] data)
    {
        uint m = 0x5bd1e995;
        uint seed = 0;
        uint r = 24;
        uint len = (uint)data.Length;

        uint h = seed ^ len;

        //const unsigned char * data = (const unsigned char *)key;

        uint i = 0;
        while (len >= 4)
        {
            uint k;

            k = data[i];
            k |= (uint)(data[i + 1] << ((byte)8));
            k |= (uint)(data[i + 2] << ((byte)16));
            k |= (uint)(data[i + 3] << ((byte)24));

            i += 4;

            k *= m;
            k ^= k >> ((byte)r);
            k *= m;

            h *= m;
            h ^= k;

            len -= 4;
        }

        switch (len)
        {
            case 3:
                h ^= (uint)(data[2] << ((byte)16));
                break;
            case 2:
                h ^= (uint)(data[1] << ((byte)8));
                break;
            case 1:
                h ^= data[0];
                h *= m;
                break;
        };

        h ^= h >> 13;
        h *= m;
        h ^= h >> 15;

        return h;
    }

    public static UInt32 MurmurHash2( string data)
    {
        uint m = 0x5bd1e995;
        uint seed = 0;
        uint r = 24;
        uint len = (uint)data.Length;

        uint h = seed ^ len;

        //const unsigned char * data = (const unsigned char *)key;

        int i = 0;
        while (len >= 4)
        {
            uint k;

            k = data[i];
            k |= (uint)(data[i + 1] << ((byte)8));
            k |= (uint)(data[i + 2] << ((byte)16));
            k |= (uint)(data[i + 3] << ((byte)24));

            i += 4;

            k *= m;
            k ^= k >> ((byte)r);
            k *= m;

            h *= m;
            h ^= k;

            len -= 4;
        }

        switch (len)
        {
            case 3:
                h ^= (uint)(data[2] << ((byte)16));
                break;
            case 2:
                h ^= (uint)(data[1] << ((byte)8));
                break;
            case 1:
                h ^= data[0];
                h *= m;
                break;
        };

        h ^= h >> 13;
        h *= m;
        h ^= h >> 15;

        return h;
    }

    #endregion
}

